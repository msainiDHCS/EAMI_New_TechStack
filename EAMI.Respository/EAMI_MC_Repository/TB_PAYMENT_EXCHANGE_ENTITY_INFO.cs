namespace EAMI.Respository.EAMI_MC_Repository
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TB_PAYMENT_EXCHANGE_ENTITY_INFO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TB_PAYMENT_EXCHANGE_ENTITY_INFO()
        {
            TB_CLAIM_SCHEDULE = new HashSet<TB_CLAIM_SCHEDULE>();
            TB_PAYMENT_RECORD = new HashSet<TB_PAYMENT_RECORD>();
        }

        [Key]
        public int Payment_Exchange_Entity_Info_ID { get; set; }

        public int Payment_Exchange_Entity_ID { get; set; }

        [Required]
        [StringLength(100)]
        public string Entity_Name { get; set; }

        [Required]
        [StringLength(2)]
        public string Entity_Code_Suffix { get; set; }

        [Required]
        [StringLength(80)]
        public string Entity_Address_Line1 { get; set; }

        [StringLength(80)]
        public string Entity_Address_Line2 { get; set; }

        [StringLength(80)]
        public string Entity_Address_Line3 { get; set; }

        [Required]
        [StringLength(40)]
        public string Entity_City { get; set; }

        [Required]
        [StringLength(2)]
        public string Entity_State { get; set; }

        [Required]
        [StringLength(10)]
        public string Entity_Zip { get; set; }

        [Required]
        [StringLength(1)]
        public string Entity_VendorTypeCode { get; set; }

        [Required]
        [StringLength(20)]
        public string Entity_ContractNumber { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime? UpdateDate { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TB_CLAIM_SCHEDULE> TB_CLAIM_SCHEDULE { get; set; }

        public virtual TB_PAYMENT_EXCHANGE_ENTITY TB_PAYMENT_EXCHANGE_ENTITY { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TB_PAYMENT_RECORD> TB_PAYMENT_RECORD { get; set; }
    }
}
