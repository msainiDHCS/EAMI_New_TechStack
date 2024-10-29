namespace EAMI.Respository.EAMI_RX_Repository
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TB_TRACE_PAYMENT
    {
        [Key]
        public int Trace_Payment_ID { get; set; }

        public int Trace_Transaction_ID { get; set; }

        public int? Payment_Status_Type_ID { get; set; }

        public DateTime? Payment_Status_Date { get; set; }

        [StringLength(300)]
        public string Payment_Status_Message { get; set; }

        [StringLength(20)]
        public string ClaimScheduleNumber { get; set; }

        public DateTime? ClaimScheduleDate { get; set; }

        [StringLength(20)]
        public string WarrantNumber { get; set; }

        public DateTime? WarrantDate { get; set; }

        [Column(TypeName = "money")]
        public decimal? WarrantAmount { get; set; }

        [StringLength(30)]
        public string PaymentRec_Number { get; set; }

        [StringLength(30)]
        public string PaymentRec_NumberExt { get; set; }

        [StringLength(50)]
        public string Payment_Type { get; set; }

        public DateTime? Payment_Date { get; set; }

        [Column(TypeName = "money")]
        public decimal? Amount { get; set; }

        [StringLength(10)]
        public string FiscalYear { get; set; }

        [StringLength(10)]
        public string IndexCode { get; set; }

        [StringLength(10)]
        public string ObjectDetailCode { get; set; }

        [StringLength(10)]
        public string ObjectAgencyCode { get; set; }

        [StringLength(10)]
        public string PCACode { get; set; }

        [StringLength(30)]
        public string ApprovedBy { get; set; }

        [StringLength(30)]
        public string PaymentSet_Number { get; set; }

        [StringLength(30)]
        public string PaymentSet_NumberExt { get; set; }

        [StringLength(10)]
        public string Payee_Entity_ID { get; set; }

        [StringLength(30)]
        public string Payee_Entity_ID_Type { get; set; }

        [StringLength(100)]
        public string Payee_Entity_Name { get; set; }

        [StringLength(2)]
        public string Payee_Entity_ID_Suffix { get; set; }

        [StringLength(80)]
        public string Payee_Address_Line1 { get; set; }

        [StringLength(80)]
        public string Payee_Address_Line2 { get; set; }

        [StringLength(80)]
        public string Payee_Address_Line3 { get; set; }

        [StringLength(40)]
        public string Payee_City { get; set; }

        [StringLength(2)]
        public string Payee_State { get; set; }

        [StringLength(10)]
        public string Payee_Zip { get; set; }

        [StringLength(10)]
        public string Payee_EIN { get; set; }

        [StringLength(1)]
        public string Payee_VendorTypeCode { get; set; }

        [Column(TypeName = "xml")]
        public string Payment_Kvp_Xml { get; set; }

        [Column(TypeName = "xml")]
        public string Payment_Funding_Details_Xml { get; set; }

        public virtual TB_PAYMENT_STATUS_TYPE TB_PAYMENT_STATUS_TYPE { get; set; }

        public virtual TB_TRACE_TRANSACTION TB_TRACE_TRANSACTION { get; set; }
    }
}
