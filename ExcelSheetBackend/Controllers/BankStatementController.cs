using ClosedXML.Excel;
using ExcelSheetBackend.Models;
using Microsoft.AspNetCore.Mvc;

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

        foreach (var statement in bankStatements)
        {
            var processed = ValidateData(statement);
            allProcessed.Add(processed);
        }

        var excelBytes = GenerateExcelFile(allProcessed);

        return File(
            excelBytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "BankStatement.xlsx"
        );
    }

    [NonAction]
    public ProcessedBankStatement ValidateData(BankStatementDto statement)
    {
        // Parse dates safely
        DateTime? txnDate = null;
        DateTime? valueDate = null;

        if (DateTime.TryParse(statement.TxnDate, out DateTime parsedTxn))
            txnDate = parsedTxn;

        if (DateTime.TryParse(statement.ValueDate, out DateTime parsedVal))
            valueDate = parsedVal;

        // Pick the positive value between Credit and Debit
        decimal? amount = null;

        if (decimal.TryParse(statement.Credit, System.Globalization.NumberStyles.Any,
            System.Globalization.CultureInfo.InvariantCulture, out decimal credit) && credit > 0)
            amount = credit;
        else if (decimal.TryParse(statement.Debit, System.Globalization.NumberStyles.Any,
            System.Globalization.CultureInfo.InvariantCulture, out decimal debit) && debit > 0)
            amount = debit;

        decimal.TryParse(statement.Balance, System.Globalization.NumberStyles.Any,
            System.Globalization.CultureInfo.InvariantCulture, out decimal balance);

        return new ProcessedBankStatement
        {
            SerialNumber = statement.SerialNumber,
            TransactionDate = txnDate,
            ValueDate = valueDate,
            Narration = statement.Narration,
            RefNo = statement.RefNo,
            Amount = amount,
            DrErp = statement.DrErp,
            CrErp = statement.CrErp,
            Balance = statement.Balance,
            Comments = statement.Comments
        };
    }

    private byte[] GenerateExcelFile(List<ProcessedBankStatement> records)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Bank Statements");

        var headers = new[]
        {
            "S/N", "Txn Date", "Val. Date", "Narration", "Ref. No",
            "Amount", "NegativeValue", "DR ERP", "CR ERP", "Balance", "Comments"
        };

        for (int i = 0; i < headers.Length; i++)
            worksheet.Cell(1, i + 1).Value = headers[i];

        for (int i = 0; i < records.Count; i++)
        {
            int row = i + 2;
            var record = records[i];

            worksheet.Cell(row, 1).Value = record.SerialNumber;
            worksheet.Cell(row, 2).Value = record.TransactionDate?.ToString("yyyy-MM-dd");
            worksheet.Cell(row, 3).Value = record.ValueDate?.ToString("yyyy-MM-dd");
            worksheet.Cell(row, 4).Value = record.Narration;
            worksheet.Cell(row, 5).Value = record.RefNo;
            worksheet.Cell(row, 6).Value = record.Amount.HasValue ? record.Amount.Value : 0;
            worksheet.Cell(row, 7).Value = record.NegativeValue.HasValue ? record.NegativeValue.Value : 0;
            worksheet.Cell(row, 8).Value = record.DrErp;
            worksheet.Cell(row, 9).Value = record.CrErp;
            worksheet.Cell(row, 10).Value = record.Balance;
            worksheet.Cell(row, 11).Value = record.Comments;
        }

        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}