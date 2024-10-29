namespace EAMI.Respository.EAMI_Dental_Repository
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TB_CLAIM_SCHEDULE_USER_ASSIGNMENT
    {
        [Key]
        public int Claim_Schedule_User_Assignment_ID { get; set; }

        public int Claim_Schedule_ID { get; set; }

        public int User_ID { get; set; }

        public int Assigned_CS_Status_ID { get; set; }

        public virtual TB_CLAIM_SCHEDULE TB_CLAIM_SCHEDULE { get; set; }

        public virtual TB_CLAIM_SCHEDULE_STATUS TB_CLAIM_SCHEDULE_STATUS { get; set; }

        public virtual TB_USER TB_USER { get; set; }
    }
}
