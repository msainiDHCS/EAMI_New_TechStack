namespace EAMI.Respository.EAMI_MC_Repository
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TB_DRAWDATE_CALENDAR
    {
        [Key]
        public int Drawdate_Calendar_Id { get; set; }

        [Column(TypeName = "date")]
        public DateTime Drawdate { get; set; }

        [StringLength(50)]
        public string Note { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreateDate { get; set; }

        [Required]
        [StringLength(20)]
        public string CreatedBy { get; set; }

        public DateTime? UpdateDate { get; set; }

        [StringLength(20)]
        public string UpdatedBy { get; set; }
    }
}
