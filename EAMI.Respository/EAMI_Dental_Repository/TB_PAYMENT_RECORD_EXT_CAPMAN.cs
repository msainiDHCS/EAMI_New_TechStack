namespace EAMI.Respository.EAMI_Dental_Repository
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TB_PAYMENT_RECORD_EXT_CAPMAN
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Payment_Record_ID { get; set; }

        [Required]
        [StringLength(20)]
        public string ContractNumber { get; set; }

        public DateTime ContractDateFrom { get; set; }

        public DateTime ContractDateTo { get; set; }

        public int Exclusive_Payment_Type_ID { get; set; }

        public virtual TB_EXCLUSIVE_PAYMENT_TYPE TB_EXCLUSIVE_PAYMENT_TYPE { get; set; }

        public virtual TB_PAYMENT_RECORD TB_PAYMENT_RECORD { get; set; }
    }
}
