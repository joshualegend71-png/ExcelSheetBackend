namespace ExcelSheetBackend.Models
{
    public class BankStatementDto
    {
        public int SerialNumber { get; set; }

        public DateTime TxnDate { get; set; }
        public DateTime ValueDate { get; set; }
        public string? Narration { get; set; }
        public string? RefNo { get; set; }
        public decimal? Debit { get; set; }

        public decimal? BankCharges { get; set; }
        public decimal? DeferredRevenueRefund { get; set; }
        public decimal? LicensingAndPermit { get; set; }
        public decimal? StaffLoanAndAdvances { get; set; }
        public decimal? OperationalExpenses { get; set; }

        public decimal? FlightOperationExpenses { get; set; }
        public decimal? FreightExpenses { get; set; }
        public decimal? AirportExpenses { get; set; }
        public decimal? OfficeExpenses { get; set; }
        public decimal? OperationalStaffCost { get; set; }
        public decimal? DieselAndFuel { get; set; }
        public decimal? CateringFees { get; set; }
        public decimal? SecurityExp { get; set; }
        public decimal? RepairsAndMaintenanceOffice { get; set; }
        public decimal? RepairsAndMaintAircraftParts { get; set; }
        public decimal? RepairsAndMaintenanceMV { get; set; }
        public decimal? BrandingPublicity { get; set; }
        public decimal? CrewTraining { get; set; }
        public decimal? AviationFuel { get; set; }
        public decimal? CharterExpenses { get; set; }
        public decimal? CharterCommission { get; set; }
        public decimal? NamaOtherCharges { get; set; }
        public decimal? VisaFeeCerpacImmigrationODC1 { get; set; }
        public decimal? PscChargesBicourtney { get; set; }
        public decimal? NcaaChargesCommandCheck { get; set; }
        public decimal? VipLoungeServices { get; set; }
        public decimal? NamaNavigationalCharges { get; set; }
        public decimal? FaanLandingCharges { get; set; }
        public decimal? IataLandingAndSubscriptions { get; set; }

        public decimal? CostOfSales { get; set; }
        public decimal? OtherCostOfSales { get; set; }
        public decimal? CleaningAndSanitation { get; set; }
        public decimal? Salary { get; set; }
        public decimal? StationElectricity { get; set; }
        public decimal? ComputerAndOfficeEquipment { get; set; }
        public decimal? FurnitureAndFittings { get; set; }
        public decimal? Entertainment { get; set; }
        public decimal? TelephoneExpenses { get; set; }
        public decimal? ProfessionalAndLegalFees { get; set; }
        public decimal? AccrualNsitf { get; set; }
        public decimal? MarketingExpenses { get; set; }
        public decimal? Insurance { get; set; }
        public decimal? HotelAccommodation { get; set; }
        public decimal? OfficeExpenses2 { get; set; }
        public decimal? MedicalExpensesOthers { get; set; }
        public decimal? InternetServices { get; set; }
        public decimal? OfficeRent { get; set; }
        public decimal? StationeryAndPrintingPapers { get; set; }
        public decimal? PublicRelationExpenses { get; set; }
        public decimal? GiftAndDonations { get; set; }
        public decimal? Transport { get; set; }
    }
}
