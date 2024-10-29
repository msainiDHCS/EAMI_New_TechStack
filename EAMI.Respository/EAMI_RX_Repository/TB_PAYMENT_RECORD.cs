namespace EAMI.Respository.EAMI_RX_Repository
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TB_PAYMENT_RECORD
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TB_PAYMENT_RECORD()
        {
            TB_FUNDING_DETAIL = new HashSet<TB_FUNDING_DETAIL>();
            TB_PAYMENT_KVP = new HashSet<TB_PAYMENT_KVP>();
            TB_PAYMENT_STATUS = new HashSet<TB_PAYMENT_STATUS>();
            TB_PAYMENT_USER_ASSIGNMENT = new HashSet<TB_PAYMENT_USER_ASSIGNMENT>();
        }

        [Key]
        public int Payment_Record_ID { get; set; }

        public int Transaction_ID { get; set; }

        [Required]
        [StringLength(30)]
        public string PaymentRec_Number { get; set; }

        [Required]
        [StringLength(30)]
        public string PaymentRec_NumberExt { get; set; }

        [Required]
        [StringLength(50)]
        public string Payment_Type { get; set; }

        public DateTime Payment_Date { get; set; }

        [Column(TypeName = "money")]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(10)]
        public string FiscalYear { get; set; }

        [Required]
        [StringLength(10)]
        public string IndexCode { get; set; }

        [Required]
        [StringLength(10)]
        public string ObjectDetailCode { get; set; }

        [StringLength(10)]
        public string ObjectAgencyCode { get; set; }

        [Required]
        [StringLength(10)]
        public string PCACode { get; set; }

        [Required]
        [StringLength(30)]
        public string ApprovedBy { get; set; }

        [Required]
        [StringLength(30)]
        public string PaymentSet_Number { get; set; }

        [Required]
        [StringLength(30)]
        public string PaymentSet_NumberExt { get; set; }

        public int? Payee_Entity_Info_ID { get; set; }

        [Required]
        [StringLength(2)]
        public string RPICode { get; set; }

        public bool IsReportableRPI { get; set; }

        public int PEE_Address_ID { get; set; }

        public int? PEE_EFT_Info_ID { get; set; }

        public int Payment_Method_Type_ID { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TB_FUNDING_DETAIL> TB_FUNDING_DETAIL { get; set; }

        public virtual TB_PAYMENT_DN_STATUS TB_PAYMENT_DN_STATUS { get; set; }

        public virtual TB_PAYMENT_EXCHANGE_ENTITY_INFO TB_PAYMENT_EXCHANGE_ENTITY_INFO { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TB_PAYMENT_KVP> TB_PAYMENT_KVP { get; set; }

        public virtual TB_PAYMENT_METHOD_TYPE TB_PAYMENT_METHOD_TYPE { get; set; }

        public virtual TB_PAYMENT_RECORD_EXT_CAPMAN TB_PAYMENT_RECORD_EXT_CAPMAN { get; set; }

        public virtual TB_PEE_ADDRESS TB_PEE_ADDRESS { get; set; }

        public virtual TB_PEE_EFT_INFO TB_PEE_EFT_INFO { get; set; }

        public virtual TB_TRANSACTION TB_TRANSACTION { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TB_PAYMENT_STATUS> TB_PAYMENT_STATUS { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TB_PAYMENT_USER_ASSIGNMENT> TB_PAYMENT_USER_ASSIGNMENT { get; set; }

        public virtual TB_CLAIM_SCHEDULE TB_CLAIM_SCHEDULE { get; set; }

        public virtual TB_PAYDATE_CALENDAR TB_PAYDATE_CALENDAR { get; set; }
    }
}
