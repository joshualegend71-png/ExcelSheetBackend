using ClosedXML.Excel;
using ExcelSheetBackend.Models;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace ExcelSheetBackend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BankStatementController : ControllerBase
{
    [HttpPost("process")]
    public IActionResult Process([FromBody] List<BankStatementDto> bankStatements)
    {
        if (bankStatements == null || !bankStatements.Any())
            return BadRequest("No data provided.");

        var allProcessed = new List<ProcessedBankStatement>();
        var allGLRecords = new List<GLRecord>();

        foreach (var statement in bankStatements)
        {
            // Step 1: Validate & Extract relevant financial data
            var processed = ValidateData(statement);
            allProcessed.AddRange(processed);

            // Step 2: Transform into GL Records
            var documentType = "Deposit"; // Change logic if needed
            var documentBankCode = "MAIN-BANK";

            var glRecords = TransformData(documentType, documentBankCode, processed);
            allGLRecords.AddRange(glRecords);
        }

        // Step 3: Generate Excel
        var excelBytes = GenerateExcelFile(allGLRecords);

        return File(
            excelBytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "FeePlan.xlsx"
        );
    }

    private static readonly Dictionary<string, string> BankCodeMap = new Dictionary<string, string>
    {
        { "BankCharges", "CHG-001" },
        { "Refund", "REF-102" },
        { "Salary", "PAY-SAL" },
        { "StaffCost", "EXP-STF" },
        { "DeferredIncome", "INC-DEF" },
        { "Reversal", "REV-999" },
        { "InterbankTransfers", "TRF-INT" }
    };

    [NonAction]
    public List<ProcessedBankStatement> ValidateData(BankStatementDto bankStatement)
    {
        var processedBankStatements = new List<ProcessedBankStatement>();

        PropertyInfo[] props = bankStatement.GetType().GetProperties();

        foreach (var prop in props)
        {
            if (BankCodeMap.ContainsKey(prop.Name))
            {
                var val = prop.GetValue(bankStatement)?.ToString();

                // FIXED: Proper condition
                if (!string.IsNullOrWhiteSpace(val) && val != "0" && val != "0.00")
                {
                    var processed = new ProcessedBankStatement
                    {
                        SerialNumber = bankStatement.SerialNumber,
                        TransactionDate = bankStatement.TransactionDate,
                        ValueDate = bankStatement.ValueDate,
                        Description = bankStatement.Description,
                        BankCode = BankCodeMap[prop.Name]
                    };

                    if (decimal.TryParse(val, out decimal amt))
                    {
                        processed.Amount = amt;
                    }

                    processedBankStatements.Add(processed);
                }
            }
        }

        return processedBankStatements;
    }

    [NonAction]
    public List<GLRecord> TransformData(string documentType, string documentBankCode, List<ProcessedBankStatement> processedBankStatements)
    {
        var glRecords = new List<GLRecord>();

        foreach (var statement in processedBankStatements)
        {
            var amount = Math.Abs(statement.Amount ?? 0);

            GLRecord debitGlRecord;
            GLRecord creditGlRecord;

            if (documentType == "Deposit")
            {
                // Deposit → Bank increases
                debitGlRecord = new GLRecord
                {
                    ID = $"{documentType}-{statement.SerialNumber}",
                    Description = statement.Description,
                    BankCode = documentBankCode,
                    Amount = amount // Debit (+)
                };

                creditGlRecord = new GLRecord
                {
                    ID = $"{documentType}-{statement.SerialNumber}",
                    Description = statement.Description,
                    BankCode = statement.BankCode,
                    Amount = -amount // Credit (-)
                };
            }
            else
            {
                // Withdrawal → Bank decreases
                debitGlRecord = new GLRecord
                {
                    ID = $"{documentType}-{statement.SerialNumber}",
                    Description = statement.Description,
                    BankCode = documentBankCode,
                    Amount = -amount // Debit (-)
                };

                creditGlRecord = new GLRecord
                {
                    ID = $"{documentType}-{statement.SerialNumber}",
                    Description = statement.Description,
                    BankCode = statement.BankCode,
                    Amount = amount // Credit (+) ✅
                };
            }

            glRecords.Add(debitGlRecord);
            glRecords.Add(creditGlRecord);
        }

        return glRecords;
    }

    private byte[] GenerateExcelFile(List<GLRecord> records)
    {
        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.Worksheets.Add("Bank Statements");

            var headers = new[]
            {
                "ID", "Description", "Bank Code", "Amount"
            };

            // Add Headers
            for (var i = 0; i < headers.Length; i++)
            {
                worksheet.Cell(1, i + 1).Value = headers[i];
            }

            // Add Data
            for (int i = 0; i < records.Count; i++)
            {
                worksheet.Cell(i + 2, 1).Value = records[i].ID;
                worksheet.Cell(i + 2, 2).Value = records[i].Description;
                worksheet.Cell(i + 2, 3).Value = records[i].BankCode;
                worksheet.Cell(i + 2, 4).Value = records[i].Amount;
            }

            // Auto-fit columns (nice improvement)
            worksheet.Columns().AdjustToContents();

            using (var stream = new MemoryStream())
            {
                workbook.SaveAs(stream);
                return stream.ToArray();
            }
        }
    }
}