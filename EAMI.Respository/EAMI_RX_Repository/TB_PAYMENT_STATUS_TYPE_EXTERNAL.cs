namespace EAMI.Respository.EAMI_RX_Repository
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TB_PAYMENT_STATUS_TYPE_EXTERNAL
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TB_PAYMENT_STATUS_TYPE_EXTERNAL()
        {
            TB_PAYMENT_STATUS_TYPE = new HashSet<TB_PAYMENT_STATUS_TYPE>();
        }

        [Key]
        public int Payment_Status_Type_Ext_ID { get; set; }

        [Required]
        [StringLength(20)]
        public string Code { get; set; }

        [StringLength(50)]
        public string Description { get; set; }

        public bool IsActive { get; set; }

        public byte? Sort_Value { get; set; }

        public DateTime CreateDate { get; set; }

        [Required]
        [StringLength(20)]
        public string CreatedBy { get; set; }

        public DateTime? UpdateDate { get; set; }

        [StringLength(20)]
        public string UpdatedBy { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TB_PAYMENT_STATUS_TYPE> TB_PAYMENT_STATUS_TYPE { get; set; }
    }
}
