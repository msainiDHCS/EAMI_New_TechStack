namespace EAMI.Respository.EAMI_RX_Repository
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TB_PAYMENT_STATUS
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TB_PAYMENT_STATUS()
        {
            TB_PAYMENT_DN_STATUS = new HashSet<TB_PAYMENT_DN_STATUS>();
            TB_PAYMENT_DN_STATUS1 = new HashSet<TB_PAYMENT_DN_STATUS>();
            TB_PAYMENT_DN_STATUS2 = new HashSet<TB_PAYMENT_DN_STATUS>();
            TB_PAYMENT_DN_STATUS3 = new HashSet<TB_PAYMENT_DN_STATUS>();
            TB_PAYMENT_USER_ASSIGNMENT = new HashSet<TB_PAYMENT_USER_ASSIGNMENT>();
        }

        [Key]
        public int Payment_Status_ID { get; set; }

        public int Payment_Record_ID { get; set; }

        public int Payment_Status_Type_ID { get; set; }

        public DateTime Status_Date { get; set; }

        [StringLength(200)]
        public string Status_Note { get; set; }

        [Required]
        [StringLength(20)]
        public string CreatedBy { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TB_PAYMENT_DN_STATUS> TB_PAYMENT_DN_STATUS { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TB_PAYMENT_DN_STATUS> TB_PAYMENT_DN_STATUS1 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TB_PAYMENT_DN_STATUS> TB_PAYMENT_DN_STATUS2 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TB_PAYMENT_DN_STATUS> TB_PAYMENT_DN_STATUS3 { get; set; }

        public virtual TB_PAYMENT_RECORD TB_PAYMENT_RECORD { get; set; }

        public virtual TB_PAYMENT_STATUS_TYPE TB_PAYMENT_STATUS_TYPE { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TB_PAYMENT_USER_ASSIGNMENT> TB_PAYMENT_USER_ASSIGNMENT { get; set; }
    }
}
