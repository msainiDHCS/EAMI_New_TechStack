namespace EAMI.Respository.EAMI_RX_Repository
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TB_CLAIM_SCHEDULE
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TB_CLAIM_SCHEDULE()
        {
            TB_CLAIM_SCHEDULE_ECS = new HashSet<TB_CLAIM_SCHEDULE_ECS>();
            TB_CLAIM_SCHEDULE_STATUS = new HashSet<TB_CLAIM_SCHEDULE_STATUS>();
            TB_CLAIM_SCHEDULE_USER_ASSIGNMENT = new HashSet<TB_CLAIM_SCHEDULE_USER_ASSIGNMENT>();
            TB_PAYMENT_RECORD = new HashSet<TB_PAYMENT_RECORD>();
        }

        [Key]
        public int Claim_Schedule_ID { get; set; }

        [Required]
        [StringLength(20)]
        public string Claim_Schedule_Number { get; set; }

        [Column(TypeName = "money")]
        public decimal? Amount { get; set; }

        public DateTime? Claim_Schedule_Date { get; set; }

        [Required]
        [StringLength(10)]
        public string FiscalYear { get; set; }

        [Required]
        [StringLength(50)]
        public string Payment_Type { get; set; }

        [Required]
        [StringLength(20)]
        public string ContractNumber { get; set; }

        public int Exclusive_Payment_Type_ID { get; set; }

        public int Paydate_Calendar_ID { get; set; }

        public int? Payee_Entity_Info_ID { get; set; }

        public bool IsLinked { get; set; }

        [StringLength(15)]
        public string LinkedByPGNumber { get; set; }

        public int? SeqNumber { get; set; }

        public int PEE_Address_ID { get; set; }

        public int? PEE_EFT_Info_ID { get; set; }

        public int Payment_Method_Type_ID { get; set; }

        public virtual TB_CLAIM_SCHEDULE_DN_STATUS TB_CLAIM_SCHEDULE_DN_STATUS { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TB_CLAIM_SCHEDULE_ECS> TB_CLAIM_SCHEDULE_ECS { get; set; }

        public virtual TB_CLAIM_SCHEDULE_REMITTANCE_ADVICE_NOTE TB_CLAIM_SCHEDULE_REMITTANCE_ADVICE_NOTE { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TB_CLAIM_SCHEDULE_STATUS> TB_CLAIM_SCHEDULE_STATUS { get; set; }

        public virtual TB_EXCLUSIVE_PAYMENT_TYPE TB_EXCLUSIVE_PAYMENT_TYPE { get; set; }

        public virtual TB_PAYDATE_CALENDAR TB_PAYDATE_CALENDAR { get; set; }

        public virtual TB_PAYMENT_EXCHANGE_ENTITY_INFO TB_PAYMENT_EXCHANGE_ENTITY_INFO { get; set; }

        public virtual TB_PAYMENT_METHOD_TYPE TB_PAYMENT_METHOD_TYPE { get; set; }

        public virtual TB_PEE_ADDRESS TB_PEE_ADDRESS { get; set; }

        public virtual TB_PEE_EFT_INFO TB_PEE_EFT_INFO { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TB_CLAIM_SCHEDULE_USER_ASSIGNMENT> TB_CLAIM_SCHEDULE_USER_ASSIGNMENT { get; set; }

        public virtual TB_WARRANT TB_WARRANT { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TB_PAYMENT_RECORD> TB_PAYMENT_RECORD { get; set; }
    }
}
