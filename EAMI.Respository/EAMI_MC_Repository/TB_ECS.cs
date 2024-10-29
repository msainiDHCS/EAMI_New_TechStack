namespace EAMI.Respository.EAMI_MC_Repository
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TB_ECS
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TB_ECS()
        {
            TB_CLAIM_SCHEDULE_ECS = new HashSet<TB_CLAIM_SCHEDULE_ECS>();
        }

        [Key]
        public int ECS_ID { get; set; }

        [Required]
        [StringLength(20)]
        public string ECS_Number { get; set; }

        [StringLength(50)]
        public string ECS_File_Name { get; set; }

        public int Exclusive_Payment_Type_ID { get; set; }

        public DateTime PayDate { get; set; }

        [Column(TypeName = "money")]
        public decimal Amount { get; set; }

        [StringLength(30)]
        public string SentToScoTaskNumber { get; set; }

        [StringLength(30)]
        public string WarrantReceivedTaskNumber { get; set; }

        public DateTime CreateDate { get; set; }

        [Required]
        [StringLength(20)]
        public string CreatedBy { get; set; }

        public DateTime? ApproveDate { get; set; }

        [StringLength(20)]
        public string ApprovedBy { get; set; }

        public DateTime? SentToScoDate { get; set; }

        public DateTime? WarrantReceivedDate { get; set; }

        public int Current_ECS_Status_Type_ID { get; set; }

        public DateTime CurrentStatusDate { get; set; }

        [StringLength(200)]
        public string CurrentStatusNote { get; set; }

        [StringLength(200)]
        public string DexFileName { get; set; }

        public int SCO_File_Line_Count { get; set; }

        public int Payment_Method_Type_ID { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TB_CLAIM_SCHEDULE_ECS> TB_CLAIM_SCHEDULE_ECS { get; set; }

        public virtual TB_ECS_STATUS_TYPE TB_ECS_STATUS_TYPE { get; set; }

        public virtual TB_EXCLUSIVE_PAYMENT_TYPE TB_EXCLUSIVE_PAYMENT_TYPE { get; set; }

        public virtual TB_PAYMENT_METHOD_TYPE TB_PAYMENT_METHOD_TYPE { get; set; }
    }
}
