namespace EAMI.Respository.EAMI_Dental_Repository
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TB_CLAIM_SCHEDULE_STATUS
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TB_CLAIM_SCHEDULE_STATUS()
        {
            TB_CLAIM_SCHEDULE_DN_STATUS = new HashSet<TB_CLAIM_SCHEDULE_DN_STATUS>();
            TB_CLAIM_SCHEDULE_DN_STATUS1 = new HashSet<TB_CLAIM_SCHEDULE_DN_STATUS>();
            TB_CLAIM_SCHEDULE_USER_ASSIGNMENT = new HashSet<TB_CLAIM_SCHEDULE_USER_ASSIGNMENT>();
        }

        [Key]
        public int Claim_Schedule_Status_ID { get; set; }

        public int Claim_Schedule_ID { get; set; }

        public int Claim_Schedule_Status_Type_ID { get; set; }

        public DateTime Status_Date { get; set; }

        [StringLength(200)]
        public string Status_Note { get; set; }

        [Required]
        [StringLength(20)]
        public string CreatedBy { get; set; }

        public virtual TB_CLAIM_SCHEDULE TB_CLAIM_SCHEDULE { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TB_CLAIM_SCHEDULE_DN_STATUS> TB_CLAIM_SCHEDULE_DN_STATUS { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TB_CLAIM_SCHEDULE_DN_STATUS> TB_CLAIM_SCHEDULE_DN_STATUS1 { get; set; }

        public virtual TB_CLAIM_SCHEDULE_STATUS_TYPE TB_CLAIM_SCHEDULE_STATUS_TYPE { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TB_CLAIM_SCHEDULE_USER_ASSIGNMENT> TB_CLAIM_SCHEDULE_USER_ASSIGNMENT { get; set; }
    }
}
