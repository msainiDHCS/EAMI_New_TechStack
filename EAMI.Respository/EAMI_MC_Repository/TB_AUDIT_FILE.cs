namespace EAMI.Respository.EAMI_MC_Repository
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TB_AUDIT_FILE
    {
        [Key]
        public int Audit_File_ID { get; set; }

        [Required]
        [StringLength(50)]
        public string Audit_File_Name { get; set; }

        [Column(TypeName = "numeric")]
        public decimal Audit_File_Size { get; set; }

        public DateTime PayDate { get; set; }

        [StringLength(30)]
        public string TaskNumber { get; set; }

        public bool HasError { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime? UploadDate { get; set; }

        public DateTime? NotifiedDate { get; set; }
    }
}
