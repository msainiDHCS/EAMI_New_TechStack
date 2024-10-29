namespace EAMI.Respository.EAMI_RX_Repository
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TB_PEE_ADDRESS
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TB_PEE_ADDRESS()
        {
            TB_CLAIM_SCHEDULE = new HashSet<TB_CLAIM_SCHEDULE>();
            TB_PAYMENT_RECORD = new HashSet<TB_PAYMENT_RECORD>();
        }

        [Key]
        public int PEE_Address_ID { get; set; }

        public int PEE_System_ID { get; set; }

        [Required]
        [StringLength(30)]
        public string Address_Line1 { get; set; }

        [StringLength(30)]
        public string Address_Line2 { get; set; }

        [StringLength(30)]
        public string Address_Line3 { get; set; }

        [Required]
        [StringLength(27)]
        public string City { get; set; }

        [Required]
        [StringLength(2)]
        public string State { get; set; }

        [Required]
        [StringLength(10)]
        public string Zip { get; set; }

        [StringLength(20)]
        public string ContractNumber { get; set; }

        public DateTime CreateDate { get; set; }

        [Required]
        [StringLength(100)]
        public string Entity_Name { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TB_CLAIM_SCHEDULE> TB_CLAIM_SCHEDULE { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TB_PAYMENT_RECORD> TB_PAYMENT_RECORD { get; set; }

        public virtual TB_PEE_SYSTEM TB_PEE_SYSTEM { get; set; }
    }
}
