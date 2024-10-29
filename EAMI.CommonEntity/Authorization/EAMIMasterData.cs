using System.Runtime.Serialization;

namespace EAMI.CommonEntity
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public class EAMIMasterData : BaseEntity
    {
        [DataMember]
        public string DataType { get; set; }

        [DataMember]
        public long? ID { get; set; }

        [DataMember]
        public string Code { get; set; }

        [DataMember]
        public string OriginalCode { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string DepartmentName { get; set; }

        [DataMember]
        public string OrganizationCode { get; set; }

        [DataMember]
        public string RADepartmentAddrLine { get; set; }

        [DataMember]
        public string RADepartmentAddrCSZ { get; set; }

        [DataMember]
        public string RAInquiryPhNo { get; set; }

        [DataMember]
        public string FEIN_Number { get; set; }

        [DataMember]
        public string MaxPmtRecAmt { get; set; }

        [DataMember]
        public string MaxPmtRecPerTran { get; set; }

        [DataMember]
        public string MaxFundingDtlPerPmtRec { get; set; }

        [DataMember]
        public bool TraceIncomingPmtData { get; set; }

        [DataMember]
        public bool ValidateFundingSource { get; set; }
        public SystemProperty SystemProperty { get; set; }

        [DataMember]
        public string TitleTransferLetter { get; set; }

        [DataMember]
        public List<EAMIAuthBase> AssociatedData { get; set; }
        
        public string DelimitedAssociations
        {
            get
            {
                if (AssociatedData != null)
                    return string.Join(", ", AssociatedData.Select(a => a.Code).ToArray());
                else
                    return "";
            }
        }
        
        public string Status
        {
            get
            {
                return (base.IsActive ? "Active" : "Inactive");
            }
        }

        public DateTime LastUpdateDate
        {
            get
            {

                return UpdateDate == null ? CreateDate : Convert.ToDateTime(UpdateDate);
            }
        }

        public int Compare(EAMIUser a, EAMIUser b)
        {
            if (a.LastUpdateDate > b.LastUpdateDate)
                return 1;
            else
                return 0;
        }
    }
}
