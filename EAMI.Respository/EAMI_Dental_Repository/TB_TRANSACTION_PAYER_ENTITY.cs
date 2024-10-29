namespace EAMI.Respository.EAMI_Dental_Repository
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TB_TRANSACTION_PAYER_ENTITY
    {
        [Key]
        public int Transaction_Payer_Entity_ID { get; set; }

        public int Transactioin_ID { get; set; }

        public int Payment_Exchange_Entity_ID { get; set; }

        public virtual TB_PAYMENT_EXCHANGE_ENTITY TB_PAYMENT_EXCHANGE_ENTITY { get; set; }
    }
}
