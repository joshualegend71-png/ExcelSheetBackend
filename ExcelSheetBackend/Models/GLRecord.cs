namespace ExcelSheetBackend.Models
{
    public class GLRecord
    {
        public string ID { get; set; }
        public string? Description { get; set; }
        public string BankCode { get; set; }
        public decimal Amount { get; set; }
    }
}
