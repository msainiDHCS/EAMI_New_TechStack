namespace EAMI.Respository.EAMI_RX_Repository
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TB_USER_ROLE
    {
        [Key]
        public int User_Role_ID { get; set; }

        public int User_ID { get; set; }

        public int Role_ID { get; set; }

        public short Sort_Order { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreateDate { get; set; }

        [Required]
        [StringLength(20)]
        public string CreatedBy { get; set; }

        public DateTime? UpdateDate { get; set; }

        [StringLength(20)]
        public string UpdatedBy { get; set; }

        public virtual TB_ROLE TB_ROLE { get; set; }

        public virtual TB_USER TB_USER { get; set; }
    }
}
