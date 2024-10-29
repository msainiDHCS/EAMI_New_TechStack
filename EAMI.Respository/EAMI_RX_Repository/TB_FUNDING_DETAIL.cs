namespace EAMI.Respository.EAMI_RX_Repository
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TB_FUNDING_DETAIL
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TB_FUNDING_DETAIL()
        {
            TB_FUNDING_DETAIL_KVP = new HashSet<TB_FUNDING_DETAIL_KVP>();
        }

        [Key]
        public int Funding_Detail_ID { get; set; }

        public int Payment_Record_ID { get; set; }

        [Required]
        [StringLength(50)]
        public string Funding_Source_Name { get; set; }

        [Column(TypeName = "money")]
        public decimal FFPAmount { get; set; }

        [Column(TypeName = "money")]
        public decimal SGFAmount { get; set; }

        [Column(TypeName = "money")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public decimal? TotalAmount { get; set; }

        [Required]
        [StringLength(5)]
        public string FiscalYear { get; set; }

        [Required]
        [StringLength(5)]
        public string FiscalQuarter { get; set; }

        [Required]
        [StringLength(10)]
        public string Title { get; set; }

        public virtual TB_FUNDING_DETAIL_EXT_CAPMAN TB_FUNDING_DETAIL_EXT_CAPMAN { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TB_FUNDING_DETAIL_KVP> TB_FUNDING_DETAIL_KVP { get; set; }

        public virtual TB_PAYMENT_RECORD TB_PAYMENT_RECORD { get; set; }
    }
}
