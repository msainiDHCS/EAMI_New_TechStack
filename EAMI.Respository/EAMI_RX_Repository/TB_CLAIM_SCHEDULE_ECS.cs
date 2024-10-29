namespace EAMI.Respository.EAMI_RX_Repository
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TB_CLAIM_SCHEDULE_ECS
    {
        [Key]
        public int Claim_Schedule_ECS_ID { get; set; }

        public int Claim_Schedule_ID { get; set; }

        public int ECS_ID { get; set; }

        public virtual TB_CLAIM_SCHEDULE TB_CLAIM_SCHEDULE { get; set; }

        public virtual TB_ECS TB_ECS { get; set; }
    }
}
