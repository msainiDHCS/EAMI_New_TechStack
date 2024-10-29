namespace EAMI.CommonEntity
{

    /// <summary>
    /// a common entity status to be used as Payment or ClaimSchedule Status 
    /// </summary>
    public class EntityStatus
    {
        public int StatusID { get; set; }
        public int EntityID { get; set; }
        public RefCode StatusType { get; set; }
        public DateTime StatusDate { get; set; }
        public string StatusNote { get; set; }
        public string CreatedBy { get; set; }
    }


    /// <summary>
    /// extension of EntityStatus used in payment-data-submission and payment-status-inquiry
    /// </summary>
    public class PaymentStatus : EntityStatus
    {
        public string PaymentRecNumber { get; set; }
        public string PaymentRecNumberExt { get; set; }
        public string PaymentSetNumber { get; set; }
        public string PaymentSetNumberExt { get; set; }
        public RefCode ExternalStatusType { get; set; }
        public string ClaimScheduleNumber { get; set; }
        public DateTime ClaimScheduleDate { get; set; }
        public string WarrantNumber { get; set; }
        public DateTime WarrantDate { get; set; }
        public decimal WarrantAmount { get; set; }
        public RefCode PaymentMethodType { get; set; }
    }
}
