namespace EAMI.Respository.EAMI_MC_Repository
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TB_PAYMENT_EXCHANGE_ENTITY
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TB_PAYMENT_EXCHANGE_ENTITY()
        {
            TB_PAYMENT_EXCHANGE_ENTITY_INFO = new HashSet<TB_PAYMENT_EXCHANGE_ENTITY_INFO>();
            TB_PEE_SYSTEM = new HashSet<TB_PEE_SYSTEM>();
            TB_TRANSACTION_PAYER_ENTITY = new HashSet<TB_TRANSACTION_PAYER_ENTITY>();
        }

        [Key]
        public int Payment_Exchange_Entity_ID { get; set; }

        [Required]
        [StringLength(10)]
        public string Entity_ID { get; set; }

        [Required]
        [StringLength(30)]
        public string Entity_ID_Type { get; set; }

        [Required]
        [StringLength(100)]
        public string Entity_Name { get; set; }

        [Required]
        [StringLength(9)]
        public string Entity_EIN { get; set; }

        public bool IsActive { get; set; }

        public byte? Sort_Value { get; set; }

        public DateTime CreateDate { get; set; }

        [Required]
        [StringLength(20)]
        public string CreatedBy { get; set; }

        public DateTime? UpdateDate { get; set; }

        [StringLength(20)]
        public string UpdatedBy { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TB_PAYMENT_EXCHANGE_ENTITY_INFO> TB_PAYMENT_EXCHANGE_ENTITY_INFO { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TB_PEE_SYSTEM> TB_PEE_SYSTEM { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TB_TRANSACTION_PAYER_ENTITY> TB_TRANSACTION_PAYER_ENTITY { get; set; }
    }
}
