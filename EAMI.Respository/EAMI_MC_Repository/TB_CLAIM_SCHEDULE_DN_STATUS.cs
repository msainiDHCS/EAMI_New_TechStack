namespace EAMI.Respository.EAMI_MC_Repository
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TB_CLAIM_SCHEDULE_DN_STATUS
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Claim_Schedule_ID { get; set; }

        public int CurrentCSStatusID { get; set; }

        public int LatestCSStatusID { get; set; }

        public int? CurrentUserAssignmentID { get; set; }

        public virtual TB_CLAIM_SCHEDULE TB_CLAIM_SCHEDULE { get; set; }

        public virtual TB_CLAIM_SCHEDULE_STATUS TB_CLAIM_SCHEDULE_STATUS { get; set; }

        public virtual TB_CLAIM_SCHEDULE_STATUS TB_CLAIM_SCHEDULE_STATUS1 { get; set; }
    }
}
