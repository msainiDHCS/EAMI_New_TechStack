namespace EAMI.Respository.EAMI_Dental_Repository
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TB_PAYDATE_CALENDAR
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TB_PAYDATE_CALENDAR()
        {
            TB_CLAIM_SCHEDULE = new HashSet<TB_CLAIM_SCHEDULE>();
            TB_PAYMENT_RECORD = new HashSet<TB_PAYMENT_RECORD>();
        }

        [Key]
        public int Paydate_Calendar_ID { get; set; }

        [Column(TypeName = "date")]
        public DateTime Paydate { get; set; }

        [StringLength(50)]
        public string Note { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreateDate { get; set; }

        [Required]
        [StringLength(20)]
        public string CreatedBy { get; set; }

        public DateTime? UpdateDate { get; set; }

        [StringLength(20)]
        public string UpdatedBy { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TB_CLAIM_SCHEDULE> TB_CLAIM_SCHEDULE { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TB_PAYMENT_RECORD> TB_PAYMENT_RECORD { get; set; }
    }
}
