using OHC.EAMI.CommonEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace OHC.EAMI.WebUI.Models
{
    public class EAMIMasterDataModel
    {
        public string DataType { get; set; }
        public long? ID { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Code")]
        public string Code { get; set; }
        public string OriginalCode { get; set; }

        [Display(Name = "Department Name")]
        public string DepartmentName { get; set; }

        [Display(Name = "Organization Code")]
        public string OrganizationCode { get; set; }
        
        [Display(Name = "RA Department Addr Line")]
        public string RADepartmentAddrLine { get; set; }
        
        [Display(Name = "RA Department Addr CSZ")]
        public string RADepartmentAddrCSZ { get; set; }

        [Display(Name = "RA Inquiry Phone No")]
        public string RAInquiryPhNo { get; set; }

        [Display(Name = "FEIN Number")]
        public string FEIN_Number { get; set; }

        [Display(Name = "Max Payment Rec Amt ")]
        public string MaxPmtRecAmt { get; set; }

        [Display(Name = "Max Payment Rec Per Tran")]
        public string MaxPmtRecPerTran { get; set; }

        [Display(Name = "Max Funding Dtl Per Pmt Rec")]
        public string MaxFundingDtlPerPmtRec { get; set; }

        [Display(Name = "Trace Incoming Pmt Data")]
        public bool TraceIncomingPmtData { get; set; }
        public bool ValidateFundingSource { get; set; }

        [Display(Name = "Title Transfer Letter")]
        public string TitleTransferLetter { get; set; }

        public bool IsActive { get; set; }

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
        public string Status { get; set; }
        
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime CreateDate
        {
            get; set;
        }

        public DateTime LastUpdateDate
        {
            get;set;
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