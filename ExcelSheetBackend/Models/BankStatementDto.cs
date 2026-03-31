namespace ExcelSheetBackend.Models
{
    public class BankStatementDto
    {
        public int SerialNumber { get; set; }          // S/N
        public DateTime? TransactionDate { get; set; } // TRAN DATE
        public DateTime? ValueDate { get; set; }       // VALUE DATE
        public string Description { get; set; }        // DESCRIPTION
        public decimal? BankCharges { get; set; }      // Bank Charges
        public decimal? Refund { get; set; }           // Refund
        public decimal? Salary { get; set; }           // Salary
        public decimal? StaffCost { get; set; }        // Staff Cost
        public decimal? DeferredIncome { get; set; }   // Deferred Income
        public decimal? Reversal { get; set; }         // Reversal
        public decimal? InterbankTransfers { get; set; } // Interbank Transfers
    }
}
