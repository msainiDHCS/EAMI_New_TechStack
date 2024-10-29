using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using OHC.EAMI.CommonEntity.Base;


using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;



namespace OHC.EAMI.CommonEntity
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public class ExclusivePmtType : BaseEntity
    {
        [DataMember]
        public int? Exclusive_Payment_Type_ID { get; set; }

        [DataMember]
        public string Exclusive_Payment_Type_Code { get; set; }
        [DataMember]
        public string Exclusive_Payment_Type_Name { get; set; }

        [DataMember]
        public string Exclusive_Payment_Type_Description { get; set; }

        [DataMember]
        public EAMIAuthBase Fund { get; set; }


        [DataMember]
        public string Fund_Code { get; set; }

        [DataMember]
        public int Fund_ID { get; set; }
        [DataMember]
        public string DeactivatedBy { get; set; }
        [DataMember]
        public DateTime? DeactivatedDate { get; set; }

        //[DataMember]
        //public List<EAMIAuthBase> Funds { get; set; }

        [DataMember]
        public List<EAMIAuthBase> Funds { get; set; }

        [DataMember]
        public int System_ID { get; set; }

        [DataMember]
        public string System_Code { get; set; }

        public string Status
        {
            get
            {
                return (base.IsActive ? "Active" : "Inactive");
            }
        }

        public ExclPmtType EPT { get; set; }
    }





}
