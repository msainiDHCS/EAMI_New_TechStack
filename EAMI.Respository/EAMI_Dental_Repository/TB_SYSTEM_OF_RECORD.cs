namespace EAMI.Respository.EAMI_Dental_Repository
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TB_SYSTEM_OF_RECORD
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TB_SYSTEM_OF_RECORD()
        {
            TB_PEE_SYSTEM = new HashSet<TB_PEE_SYSTEM>();
            TB_SOR_KVP_KEY = new HashSet<TB_SOR_KVP_KEY>();
            TB_TRANSACTION = new HashSet<TB_TRANSACTION>();
        }

        [Key]
        public int SOR_ID { get; set; }

        [Required]
        [StringLength(20)]
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
        public virtual ICollection<TB_PEE_SYSTEM> TB_PEE_SYSTEM { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TB_SOR_KVP_KEY> TB_SOR_KVP_KEY { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TB_TRANSACTION> TB_TRANSACTION { get; set; }
    }
}
