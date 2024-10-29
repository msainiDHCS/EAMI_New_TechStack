namespace EAMI.Respository.EAMI_Dental_Repository
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TB_PEE_EFT_INFO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TB_PEE_EFT_INFO()
        {
            TB_CLAIM_SCHEDULE = new HashSet<TB_CLAIM_SCHEDULE>();
            TB_PAYMENT_RECORD = new HashSet<TB_PAYMENT_RECORD>();
        }

        [Key]
        public int PEE_EFT_Info_ID { get; set; }

        public int PEE_System_ID { get; set; }

        [Required]
        [StringLength(20)]
        public string FIRoutingNumber { get; set; }

        [Required]
        [StringLength(20)]
        public string FIAccountType { get; set; }

        [Required]
        [StringLength(20)]
        public string PrvAccountNo { get; set; }

        public DateTime DatePrenoted { get; set; }

        public DateTime CreateDate { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TB_CLAIM_SCHEDULE> TB_CLAIM_SCHEDULE { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TB_PAYMENT_RECORD> TB_PAYMENT_RECORD { get; set; }

        public virtual TB_PEE_SYSTEM TB_PEE_SYSTEM { get; set; }
    }
}
