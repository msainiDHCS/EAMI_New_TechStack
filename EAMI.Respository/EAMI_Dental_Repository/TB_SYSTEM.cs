namespace EAMI.Respository.EAMI_Dental_Repository
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TB_SYSTEM
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TB_SYSTEM()
        {
            TB_EXCLUSIVE_PAYMENT_TYPE = new HashSet<TB_EXCLUSIVE_PAYMENT_TYPE>();
            TB_FUND = new HashSet<TB_FUND>();
            TB_FUNDING_SOURCE = new HashSet<TB_FUNDING_SOURCE>();
            TB_SCO_FILE_PROPERTY = new HashSet<TB_SCO_FILE_PROPERTY>();
            TB_SCO_PROPERTY = new HashSet<TB_SCO_PROPERTY>();
            TB_SYSTEM_USER = new HashSet<TB_SYSTEM_USER>();
        }

        [Key]
        public int System_ID { get; set; }

        [Required]
        [StringLength(50)]
        public string System_Code { get; set; }

        [Required]
        [StringLength(50)]
        public string System_Name { get; set; }

        [Required]
        [StringLength(100)]
        public string RA_DEPARTMENT_NAME { get; set; }

        [Required]
        [StringLength(100)]
        public string RA_DEPARTMENT_ADDR_LINE { get; set; }

        [Required]
        [StringLength(100)]
        public string RA_DEPARTMENT_ADDR_CSZ { get; set; }

        [Required]
        [StringLength(100)]
        public string RA_ORGANIZATION_CODE { get; set; }

        [Required]
        [StringLength(14)]
        public string RA_INQUIRIES_PHONE_NUMBER { get; set; }

        [Required]
        [StringLength(50)]
        public string FEIN_Number { get; set; }

        [Required]
        [StringLength(50)]
        public string MAX_PYMT_REC_AMOUNT { get; set; }

        [Required]
        [StringLength(50)]
        public string MAX_PYMT_REC_PER_TRAN { get; set; }

        [Required]
        [StringLength(50)]
        public string MAX_FUNDING_DTL_PER_PYMT_REC { get; set; }

        public bool TRACE_INCOMING_PAYMENT_DATA { get; set; }

        public bool VALIDATE_FUNDING_SOURCE { get; set; }

        public short Sort_Order { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreateDate { get; set; }

        [Required]
        [StringLength(20)]
        public string CreatedBy { get; set; }

        public DateTime? UpdateDate { get; set; }

        [StringLength(20)]
        public string UpdatedBy { get; set; }

        [Required]
        [StringLength(50)]
        public string TITLE_TRANSFER_LETTER { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TB_EXCLUSIVE_PAYMENT_TYPE> TB_EXCLUSIVE_PAYMENT_TYPE { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TB_FUND> TB_FUND { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TB_FUNDING_SOURCE> TB_FUNDING_SOURCE { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TB_SCO_FILE_PROPERTY> TB_SCO_FILE_PROPERTY { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TB_SCO_PROPERTY> TB_SCO_PROPERTY { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TB_SYSTEM_USER> TB_SYSTEM_USER { get; set; }
    }
}
