namespace ExcelSheetBackend.Models
{
    public class BankStatementDto
    {
        public int SerialNumber { get; set; }
        public string? TxnDate { get; set; }
        public string? ValueDate { get; set; }
        public string? Narration { get; set; }
        public string? RefNo { get; set; }
        public string? Debit { get; set; }
        public string? Credit { get; set; }
        public string? DrErp { get; set; }
        public string? CrErp { get; set; }
        public string? Balance { get; set; }
        public string? Comments { get; set; }

    }
}
