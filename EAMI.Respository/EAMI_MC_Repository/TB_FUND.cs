namespace EAMI.Respository.EAMI_MC_Repository
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TB_FUND
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TB_FUND()
        {
            TB_EXCLUSIVE_PAYMENT_TYPE = new HashSet<TB_EXCLUSIVE_PAYMENT_TYPE>();
            TB_FACESHEET = new HashSet<TB_FACESHEET>();
            TB_SCO_FILE_PROPERTY = new HashSet<TB_SCO_FILE_PROPERTY>();
        }

        [Key]
        public int Fund_ID { get; set; }

        [Required]
        [StringLength(20)]
        public string Fund_Code { get; set; }

        [StringLength(50)]
        public string Fund_Name { get; set; }

        [StringLength(100)]
        public string Fund_Description { get; set; }

        public int System_ID { get; set; }

        public bool IsActive { get; set; }

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

        [Required]
        [StringLength(4)]
        public string Stat_Year { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TB_EXCLUSIVE_PAYMENT_TYPE> TB_EXCLUSIVE_PAYMENT_TYPE { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TB_FACESHEET> TB_FACESHEET { get; set; }

        public virtual TB_SYSTEM TB_SYSTEM { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TB_SCO_FILE_PROPERTY> TB_SCO_FILE_PROPERTY { get; set; }
    }
}
