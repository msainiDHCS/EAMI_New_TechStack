namespace EAMI.Respository.EAMI_RX_Repository
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TB_FACESHEET
    {
        [Key]
        public int Facesheet_ID { get; set; }

        public int Fund_ID { get; set; }

        [Required]
        [StringLength(50)]
        public string Chapter { get; set; }

        [StringLength(50)]
        public string Reference_Item { get; set; }

        [StringLength(50)]
        public string Program { get; set; }

        [StringLength(50)]
        public string Element { get; set; }

        [Required]
        [StringLength(100)]
        public string Description { get; set; }

        public DateTime CreateDate { get; set; }

        [Required]
        [StringLength(50)]
        public string CreatedBy { get; set; }

        public DateTime? UpdateDate { get; set; }

        [StringLength(50)]
        public string UpdatedBy { get; set; }

        public bool IsActive { get; set; }

        public virtual TB_FUND TB_FUND { get; set; }
    }
}
