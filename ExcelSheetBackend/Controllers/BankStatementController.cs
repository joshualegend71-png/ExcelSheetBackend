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
    public IActionResult Process(
    [FromBody] List<BankStatementDto> bankStatements,
    [FromQuery] string transactionType = "Withdrawal",
    [FromQuery] string bankCode = "BNK")
    {
        if (bankStatements == null || !bankStatements.Any())
            return BadRequest("No data provided.");

        var validTransactionTypes = new[] { "Deposit", "Withdrawal" };
        if (!validTransactionTypes.Contains(transactionType))
            return BadRequest($"Invalid transactionType '{transactionType}'. Must be 'Deposit' or 'Withdrawal'.");

        if (string.IsNullOrWhiteSpace(bankCode))
            return BadRequest("bankCode cannot be empty.");

        var allProcessed = new List<ProcessedBankStatement>();
        var allGLRecords = new List<GLRecord>();

        foreach (var statement in bankStatements)
        {
            var processed = ValidateData(statement);
            allProcessed.AddRange(processed);

            var glRecords = TransformData(transactionType, bankCode, processed);
            allGLRecords.AddRange(glRecords);
        }

        var excelBytes = GenerateExcelFile(allGLRecords);

        return File(
            excelBytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "FeePlan.xlsx"
        );
    }

    private static readonly Dictionary<string, string> BankCodeMap = new Dictionary<string, string>
    {
        { "BankCharges", "81158" },
        { "DeferredRevenueRefund", "50001" },
        { "LicensingAndPermit", "81205" },
        { "StaffLoanAndAdvances", "24000" },
        { "OperationalExpenses", "81233" },
        { "Flightoperationexpenses", "83004" },
        { "FreightExpenses", "81232" },
        { "AirportExpenses", "81226" },
        { "OfficeExpenses", "81223" },
        { "OperationalStaffCost", "83018" },
        { "DieselAndFuel", "81138" },
        { "CateringFees", "80009" },
        { "Securityexp", "81227" },
        { "RepairsAndMaintenance_Office", "81120" },
        { "RepairsAndMaintAircraftParts", "81219" },
        { "RepairsAndMaintenance_MV", "81119" },
        { "BrandingPublicity", "81216" },
        { "CrewTraining", "80049" },
        { "AviationFuel", "80008" },
        { "CharterExpenses", "80036" },
        { "CharterCommission", "80012" },
        { "NamaOtherCharges", "80005" },
        { "VisaFeeCerpacImmigrationODC", "80019" },
        { "PscChargesBicourtney", "80000" },
        { "NcaaChargesCommandCheck", "80046" },
        { "VipLoungeServices", "81246" },
        { "NamaNavigationalCharges", "80004" },
        { "FaanLandingcharges", "80007" },
        { "IataLandingAndSubscriptions", "83016" },
        { "CostofSales", "80037" },
        { "OtherCostofSales", "80037" },
        { "CleaningAndSanitation", "81242" },
        { "Salary", "81130" },
        { "StationElectricity", "80027" },
        { "ComputerAndOfficeEquipment", "29000" },
        { "FurnitureAndFittings", "29100" },
        { "Entertainment", "81214" },
        { "Telephoneexpenses", "81134" },
        { "ProfessionAndLegalFees", "80034" },
        { "Accrual_NSITF", "40031" },
        { "MarketingExpenses", "81124" },
        { "Insurance", "81217" },
        { "VisaFeeCerpacImmigrationODC", "80019" },
        { "HotelAccomodation", "81121" },
        { "OfficeExpenses", "81223" },
        { "MedicalExpensesOthers", "81142" },
        { "InternetServices", "81128" },
        { "Officerent", "81229" },
        { "StationeryAndPrintingPapers", "81136" },
        { "Publicrelationexpenses", "83003" },
        { "GiftandDonations", "81122" },
        { "Transport", "81215" }
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
                        TransactionDate = bankStatement.TxnDate,
                        ValueDate = bankStatement.ValueDate,
                        Description = bankStatement.Narration,
                        BankCode = BankCodeMap[prop.Name]
                    };

                    if (decimal.TryParse(val, System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture,
                    out decimal amt))
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
            else if (documentType == "Withdrawal")
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
            else continue;

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