namespace EAMI.Respository.EAMI_MC_Repository
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TB_SYSTEM_USER
    {
        [Key]
        public int System_User_ID { get; set; }

        public int System_ID { get; set; }

        public int User_ID { get; set; }

        public short Sort_Order { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreateDate { get; set; }

        [Required]
        [StringLength(20)]
        public string CreatedBy { get; set; }

        public DateTime? UpdateDate { get; set; }

        [StringLength(20)]
        public string UpdatedBy { get; set; }

        public virtual TB_SYSTEM TB_SYSTEM { get; set; }

        public virtual TB_USER TB_USER { get; set; }
    }
}
