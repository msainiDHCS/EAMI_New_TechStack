namespace EAMI.Respository.EAMI_MC_Repository
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TB_SCO_PROPERTY
    {
        [Key]
        public int SCO_Property_ID { get; set; }

        [Required]
        [StringLength(50)]
        public string SCO_Property_Name { get; set; }

        [Required]
        [StringLength(50)]
        public string SCO_Property_Value { get; set; }

        public int SCO_Property_Type_ID { get; set; }

        public int SCO_Property_Enum_ID { get; set; }

        [StringLength(100)]
        public string Description { get; set; }

        public int System_ID { get; set; }

        public bool IsActive { get; set; }

        public byte Sort_Value { get; set; }

        public DateTime CreateDate { get; set; }

        [Required]
        [StringLength(20)]
        public string CreatedBy { get; set; }

        public DateTime? UpdateDate { get; set; }

        [StringLength(20)]
        public string UpdatedBy { get; set; }

        public virtual TB_SCO_PROPERTY_ENUM TB_SCO_PROPERTY_ENUM { get; set; }

        public virtual TB_SCO_PROPERTY_TYPE TB_SCO_PROPERTY_TYPE { get; set; }

        public virtual TB_SYSTEM TB_SYSTEM { get; set; }
    }
}
