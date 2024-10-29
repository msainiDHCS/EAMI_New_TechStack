namespace EAMI.Respository.EAMI_Dental_Repository
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TB_CLAIM_SCHEDULE_STATUS_TYPE
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TB_CLAIM_SCHEDULE_STATUS_TYPE()
        {
            TB_CLAIM_SCHEDULE_STATUS = new HashSet<TB_CLAIM_SCHEDULE_STATUS>();
        }

        [Key]
        public int Claim_Schedule_Status_Type_ID { get; set; }

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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TB_CLAIM_SCHEDULE_STATUS> TB_CLAIM_SCHEDULE_STATUS { get; set; }
    }
}
