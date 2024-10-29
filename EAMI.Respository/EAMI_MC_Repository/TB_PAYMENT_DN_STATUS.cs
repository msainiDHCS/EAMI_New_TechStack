namespace EAMI.Respository.EAMI_MC_Repository
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TB_PAYMENT_DN_STATUS
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Payment_Record_ID { get; set; }

        public int CurrentPaymentStatusID { get; set; }

        public int LatestPaymentStatusID { get; set; }

        public int? CurrentUserAssignmentID { get; set; }

        public int? CurrentHoldStatusID { get; set; }

        public int? CurrentUnHoldStatusID { get; set; }

        public int? CurrentReleaseFromSupStatusID { get; set; }

        public virtual TB_PAYMENT_RECORD TB_PAYMENT_RECORD { get; set; }

        public virtual TB_PAYMENT_STATUS TB_PAYMENT_STATUS { get; set; }

        public virtual TB_PAYMENT_STATUS TB_PAYMENT_STATUS1 { get; set; }

        public virtual TB_PAYMENT_STATUS TB_PAYMENT_STATUS2 { get; set; }

        public virtual TB_PAYMENT_STATUS TB_PAYMENT_STATUS3 { get; set; }

        public virtual TB_PAYMENT_USER_ASSIGNMENT TB_PAYMENT_USER_ASSIGNMENT { get; set; }
    }
}
