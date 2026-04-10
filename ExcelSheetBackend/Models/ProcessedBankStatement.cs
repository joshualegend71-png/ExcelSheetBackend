namespace ExcelSheetBackend.Models
{
    public class ProcessedBankStatement
    {
        public int SerialNumber { get; set; }
        public DateTime? TransactionDate { get; set; }
        public DateTime? ValueDate { get; set; }
        public string? Narration { get; set; }
        public string? RefNo { get; set; }
        public decimal? Amount { get; set; }
        public decimal? NegativeValue => Amount.HasValue ? -Math.Abs(Amount.Value) : null;
        public string? DrErp { get; set; }
        public string? CrErp { get; set; }
        public string? Balance { get; set; }
        public string? Comments { get; set; }
    }
}