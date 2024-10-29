namespace EAMI.Respository.EAMI_Dental_Repository
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TB_PAYMENT_KVP
    {
        [Key]
        public int Payment_Kvp_ID { get; set; }

        public int Payment_Record_ID { get; set; }

        public int SOR_Kvp_Key_ID { get; set; }

        [Required]
        [StringLength(100)]
        public string Kvp_Value { get; set; }

        public virtual TB_PAYMENT_RECORD TB_PAYMENT_RECORD { get; set; }

        public virtual TB_SOR_KVP_KEY TB_SOR_KVP_KEY { get; set; }
    }
}
