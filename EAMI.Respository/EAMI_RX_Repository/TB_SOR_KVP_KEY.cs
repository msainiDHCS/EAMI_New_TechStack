namespace EAMI.Respository.EAMI_RX_Repository
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TB_SOR_KVP_KEY
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TB_SOR_KVP_KEY()
        {
            TB_FUNDING_DETAIL_KVP = new HashSet<TB_FUNDING_DETAIL_KVP>();
            TB_PAYMENT_KVP = new HashSet<TB_PAYMENT_KVP>();
        }

        [Key]
        public int SOR_Kvp_Key_ID { get; set; }

        public int SOR_ID { get; set; }

        [Required]
        [StringLength(20)]
        public string Kvp_Key_Name { get; set; }

        [Required]
        [StringLength(10)]
        public string Kvp_Value_DataType { get; set; }

        public int Kvp_Value_Length { get; set; }

        [StringLength(100)]
        public string Kvp_Description { get; set; }

        [Required]
        [StringLength(15)]
        public string OwnerEntity { get; set; }

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
        public virtual ICollection<TB_FUNDING_DETAIL_KVP> TB_FUNDING_DETAIL_KVP { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TB_PAYMENT_KVP> TB_PAYMENT_KVP { get; set; }

        public virtual TB_SYSTEM_OF_RECORD TB_SYSTEM_OF_RECORD { get; set; }
    }
}
