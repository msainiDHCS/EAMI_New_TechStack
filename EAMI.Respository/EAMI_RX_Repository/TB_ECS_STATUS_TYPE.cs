namespace EAMI.Respository.EAMI_RX_Repository
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TB_ECS_STATUS_TYPE
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TB_ECS_STATUS_TYPE()
        {
            TB_ECS = new HashSet<TB_ECS>();
        }

        [Key]
        public int ECS_Status_Type_ID { get; set; }

        [Required]
        [StringLength(20)]
        public string CODE { get; set; }

        [StringLength(50)]
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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TB_ECS> TB_ECS { get; set; }
    }
}
