namespace EAMI.Respository.EAMI_Dental_Repository
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TB_EXCLUSIVE_PAYMENT_TYPE
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TB_EXCLUSIVE_PAYMENT_TYPE()
        {
            TB_CLAIM_SCHEDULE = new HashSet<TB_CLAIM_SCHEDULE>();
            TB_ECS = new HashSet<TB_ECS>();
            TB_PAYMENT_RECORD_EXT_CAPMAN = new HashSet<TB_PAYMENT_RECORD_EXT_CAPMAN>();
        }

        [Key]
        public int Exclusive_Payment_Type_ID { get; set; }

        [Required]
        [StringLength(20)]
        public string Code { get; set; }

        [StringLength(100)]
        public string Description { get; set; }

        public bool IsActive { get; set; }

        public byte? Sort_Value { get; set; }

        public DateTime CreateDate { get; set; }

        [Required]
        [StringLength(20)]
        public string CreatedBy { get; set; }

        public DateTime? UpdateDate { get; set; }

        [StringLength(20)]
        public string UpdatedBy { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        public int System_ID { get; set; }

        public DateTime? DeactivatedDate { get; set; }

        [StringLength(20)]
        public string DeactivatedBy { get; set; }

        public int Fund_ID { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TB_CLAIM_SCHEDULE> TB_CLAIM_SCHEDULE { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TB_ECS> TB_ECS { get; set; }

        public virtual TB_FUND TB_FUND { get; set; }

        public virtual TB_SYSTEM TB_SYSTEM { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TB_PAYMENT_RECORD_EXT_CAPMAN> TB_PAYMENT_RECORD_EXT_CAPMAN { get; set; }
    }
}
