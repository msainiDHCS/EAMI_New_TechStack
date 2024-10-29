namespace EAMI.Respository.EAMI_Dental_Repository
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TB_ECS_SEQUENCE
    {
        [Key]
        public long ECS_Sequence { get; set; }

        public DateTime UpdateDate { get; set; }
    }
}
