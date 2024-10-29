namespace EAMI.Respository.EAMI_MC_Repository
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TB_TRANSACTION_TYPE
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TB_TRANSACTION_TYPE()
        {
            TB_TRANSACTION = new HashSet<TB_TRANSACTION>();
        }

        [Key]
        public int Transaction_Type_ID { get; set; }

        [Required]
        [StringLength(30)]
        public string Code { get; set; }

        [StringLength(50)]
        public string Description { get; set; }

        public bool IsActive { get; set; }

        public byte Sort_Value { get; set; }

        public DateTime CreateDate { get; set; }

        [Required]
        [StringLength(20)]
        public string CreatedBy { get; set; }

        public DateTime? UpdateDate { get; set; }

        [StringLength(20)]
        public string UpdatedBy { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TB_TRANSACTION> TB_TRANSACTION { get; set; }
    }
}
