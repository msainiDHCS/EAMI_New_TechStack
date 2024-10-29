
namespace EAMI.CommonEntity
{
    public class ElectronicClaimSchedule
    {
        public int EcsId { get; set; }
        public string EcsNumber { get; set; }
        public string EcsFileName { get; set; }
        //public bool IsSCHIP { get; set; }
        public RefCode ExclusivePaymentType { get; set; }
        public DateTime PayDate { get; set; }
        public decimal Amount { get; set; }
        public string SentToScoTaskNumber { get; set; }
        public string WarrantReceivedTaskNumber { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime ApproveDate { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime SentToScoDate { get; set; }
        public DateTime WarrantReceivedDate { get; set; }
        public RefCode CurrentStatusType { get; set; }
        public DateTime CurrentStatusDate { get; set; }
        public string CurrentStatusNote { get; set; }
        public RefCode PaymentMethodType { get; set; }

        public int SCO_File_Line_Count { get; set; }
        public List<ClaimSchedule> ClaimScheduleList { get; set; }

        public bool HasExclusivePaymentType()
        {
            return HasScheduleItems() && (ClaimScheduleList.FirstOrDefault(item => item.ExclusivePaymentType.Code != "NONE")) != null;
        }

        private bool HasScheduleItems()
        {
            return this.ClaimScheduleList != null && this.ClaimScheduleList.Count > 0;
        }
    }
}
