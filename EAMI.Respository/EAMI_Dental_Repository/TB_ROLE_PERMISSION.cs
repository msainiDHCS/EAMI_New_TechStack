namespace EAMI.Respository.EAMI_Dental_Repository
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TB_ROLE_PERMISSION
    {
        [Key]
        public int Role_Permission_ID { get; set; }

        public int Role_ID { get; set; }

        public int Permission_ID { get; set; }

        public short Sort_Order { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreateDate { get; set; }

        [Required]
        [StringLength(20)]
        public string CreatedBy { get; set; }

        public DateTime? UpdateDate { get; set; }

        [StringLength(20)]
        public string UpdatedBy { get; set; }

        public virtual TB_PERMISSION TB_PERMISSION { get; set; }

        public virtual TB_ROLE TB_ROLE { get; set; }
    }
}
