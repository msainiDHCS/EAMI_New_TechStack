namespace EAMI.Respository.EAMI_MC_Repository
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TB_USER
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TB_USER()
        {
            TB_CLAIM_SCHEDULE_USER_ASSIGNMENT = new HashSet<TB_CLAIM_SCHEDULE_USER_ASSIGNMENT>();
            TB_PAYMENT_USER_ASSIGNMENT = new HashSet<TB_PAYMENT_USER_ASSIGNMENT>();
            TB_SYSTEM_USER = new HashSet<TB_SYSTEM_USER>();
            TB_USER_ROLE = new HashSet<TB_USER_ROLE>();
        }

        [Key]
        public int User_ID { get; set; }

        [Required]
        [StringLength(20)]
        public string User_Name { get; set; }

        [StringLength(50)]
        public string Display_Name { get; set; }

        [StringLength(50)]
        public string User_EmailAddr { get; set; }

        [StringLength(200)]
        public string User_Password { get; set; }

        public int User_Type_ID { get; set; }

        [StringLength(100)]
        public string Domain_Name { get; set; }

        public short Sort_Order { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreateDate { get; set; }

        [Required]
        [StringLength(20)]
        public string CreatedBy { get; set; }

        public DateTime? UpdateDate { get; set; }

        [StringLength(20)]
        public string UpdatedBy { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TB_CLAIM_SCHEDULE_USER_ASSIGNMENT> TB_CLAIM_SCHEDULE_USER_ASSIGNMENT { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TB_PAYMENT_USER_ASSIGNMENT> TB_PAYMENT_USER_ASSIGNMENT { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TB_SYSTEM_USER> TB_SYSTEM_USER { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TB_USER_ROLE> TB_USER_ROLE { get; set; }

        public virtual TB_USER_TYPE TB_USER_TYPE { get; set; }
    }
}
