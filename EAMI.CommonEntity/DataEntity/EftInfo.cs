
namespace EAMI.CommonEntity
{
    public class EftInfo
    {
        public int EFTInfoID { get; set; }
        public string FIRoutingNumber { get; set; }
        public string FIAccountType { get; set; }
        public string PrvAccountNo { get; set; }

        public DateTime DatePrenoted { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
