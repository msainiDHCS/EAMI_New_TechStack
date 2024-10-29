namespace EAMI.Respository.EAMI_Dental_Repository
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TB_CLAIM_SCHEDULE_REMITTANCE_ADVICE_NOTE
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Claim_Schedule_ID { get; set; }

        [Required]
        [StringLength(500)]
        public string Note { get; set; }

        public DateTime CreateDate { get; set; }

        [Required]
        [StringLength(20)]
        public string CreatedBy { get; set; }

        public DateTime? UpdateDate { get; set; }

        [StringLength(20)]
        public string UpdatedBy { get; set; }

        public virtual TB_CLAIM_SCHEDULE TB_CLAIM_SCHEDULE { get; set; }
    }
}
