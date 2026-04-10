namespace ExcelSheetBackend.Models
{
    public class GLRecord
    {
        public string? ID { get; set; }
        public DateTime? TransactionDate { get; set; }
        public DateTime? ValueDate { get; set; }
        public string? Narration { get; set; }
        public string? RefNo { get; set; }
        public decimal? Amount { get; set; }
        public decimal? NegativeValue => Amount.HasValue ? -Math.Abs(Amount.Value) : null;
        public decimal? DrErp { get; set; }
        public decimal? CrErp { get; set; }
        public decimal? Balance { get; set; }
        public string? Comments { get; set; }
    }
}
