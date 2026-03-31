namespace ExcelSheetBackend.Models
{
    public class ProcessedBankStatement
    {
        public int SerialNumber { get; set; }          // S/N
        public DateTime? TransactionDate { get; set; } // TRAN DATE
        public DateTime? ValueDate { get; set; }       // VALUE DATE
        public string Description { get; set; }        // DESCRIPTION
        public string BankCode { get; set; }        // DESCRIPTION
        public decimal? Amount { get; set; }  // Interbank Transfers
    }
}