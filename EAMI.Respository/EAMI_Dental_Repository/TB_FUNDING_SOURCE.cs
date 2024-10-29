namespace EAMI.Respository.EAMI_Dental_Repository
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TB_FUNDING_SOURCE
    {
        [Key]
        public int Funding_Source_ID { get; set; }

        [Required]
        [StringLength(50)]
        public string Code { get; set; }

        [StringLength(100)]
        public string Description { get; set; }

        public int System_ID { get; set; }

        public bool IsActive { get; set; }

        public byte? Sort_Value { get; set; }

        public DateTime CreateDate { get; set; }

        [Required]
        [StringLength(20)]
        public string CreatedBy { get; set; }

        public DateTime? UpdateDate { get; set; }

        [StringLength(20)]
        public string UpdatedBy { get; set; }

        [StringLength(20)]
        public string DeactivatedBy { get; set; }

        public DateTime? DeactivatedDate { get; set; }

        public virtual TB_SYSTEM TB_SYSTEM { get; set; }
    }
}
