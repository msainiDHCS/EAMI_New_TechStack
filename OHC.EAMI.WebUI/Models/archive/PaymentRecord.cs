using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web;
using OHC.EAMI.CommonEntity;

namespace OHC.EAMI.WebUI.Models
{
    public class PaymentRecord : PaymentRec
        //: PaymentRec
    {
        #region | Constructor |
        /// <summary>
        /// Ctor
        /// </summary>
        public PaymentRecord()
        {

        }

        #endregion

        #region | Public Properties  |

        //[Display(Name = "PaymentRecord ID")]
        //public int PaymentRecordID { get; set; }




        //[Display(Name = "Transaction ID")]
        //public int TransactionID { get; set; }
        //[Display(Name = "Payment Record #")]
        //public string PaymentRecordNumber { get; set; }
        //[Display(Name = "Payment Record ID")]
        //public string PaymentRecordID { get; set; }
        //[Display(Name = "PaymentRecord Type")]
        //public string PaymentRecordType { get; set; }
        //[Display(Name = "Payment Rec Date")]
        //[DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        //public DateTime PaymentRecordDate { get; set; }
        //[Display(Name = "Payment Rec Amt")]
        //[DataType(DataType.Currency)]
        //public decimal Amount { get; set; }
        //[Display(Name = "FY")]
        //public string FiscalYear { get; set; }
        //[Display(Name = "Index Code")]
        //public string IndexCode { get; set; }
        //[Display(Name = "ObjectDetail Code")]
        //public string ObjectDetailCode { get; set; }
        //[Display(Name = "ObjectAgency Code")]
        //public string ObjectAgencyCode { get; set; }
        //[Display(Name = "PCA Code")]
        //public string PCACode { get; set; }
        //[Display(Name = "Approved By")]
        //public string ApprovedBy { get; set; }



        //[Display(Name = "Payee Entity ID")]
        //public int PayeeEntityID { get; set; }
        //[Display(Name = "Contract Number")]
        //public string ContractNumber { get; set; }
        //[Display(Name = "System")]
        //public string System { get; set; }



        //[Display(Name = "AssignChecked")]
        //public bool AssignChecked { get; set; }
        //[Display(Name = "UnassignedChecked")]
        //public bool UnassignedChecked { get; set; }
        //[Display(Name = "RejectedChecked")]
        //public bool RejectedChecked { get; set; }

        //[Display(Name = "Pay Date")]
        //public string PayDate { get; set; }

        #region | Navagation Properties  |
        public PaymentExchangeEntity PaymentExchangeEntity { get; set; }
        //public ModelType ModelType { get; set; }
        public Contract Contract { get; set; }
        public EAMISystem System { get; set; }
        //public ICollection<PaymentRecordStatus> PaymentRecordStatuses { get; set; }
        public ICollection<FundingDetail> FundingDetails { get; set; }
        #endregion

        #endregion
    }
}