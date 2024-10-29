namespace EAMI.Respository.EAMI_Dental_Repository
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TB_PERMISSION
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TB_PERMISSION()
        {
            TB_ROLE_PERMISSION = new HashSet<TB_ROLE_PERMISSION>();
        }

        [Key]
        public int Permission_ID { get; set; }

        [Required]
        [StringLength(50)]
        public string Permission_Code { get; set; }

        [Required]
        [StringLength(200)]
        public string Permission_Name { get; set; }

        public short Sort_Order { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreateDate { get; set; }

        [Required]
        [StringLength(20)]
        public string CreatedBy { get; set; }

        public DateTime? UpdateDate { get; set; }

        [StringLength(20)]
        public string UpdatedBy { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TB_ROLE_PERMISSION> TB_ROLE_PERMISSION { get; set; }
    }
}
