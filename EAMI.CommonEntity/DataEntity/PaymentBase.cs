using System.ComponentModel.DataAnnotations;

namespace EAMI.CommonEntity
{
    public class PaymentBase
    {
        public int PrimaryKeyID { get; set; }
        public string UniqueNumber { get; set; }
        public string PaymentType { get; set; }

        [DataType(DataType.Currency)]
        public virtual decimal Amount { get; set; }
        public string FiscalYear { get; set; }
        public string ContractNumber { get; set; }
        public DateTime ContractDateFrom { get; set; }
        public DateTime ContractDateTo { get; set; }        

        public RefCode ExclusivePaymentType { get; set; }        
        public RefCode PayDate { get; set; }
        public PaymentExcEntityInfo PayeeInfo { get; set; }      
         
        public EAMIUser AssignedUser { get; set; }
        public EntityStatus CurrentStatus { get; set; }
        public EntityStatus LatestStatus { get; set; }
        public RefCode PaymentMethodType { get; set; }
    }
}
