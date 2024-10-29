namespace EAMI.Respository.EAMI_RX_Repository
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TB_WARRANT
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TB_WARRANT()
        {
            TB_CLAIM_SCHEDULE = new HashSet<TB_CLAIM_SCHEDULE>();
        }

        [Key]
        public int Warrant_ID { get; set; }

        [Required]
        [StringLength(20)]
        public string Warrant_Number { get; set; }

        [Column(TypeName = "money")]
        public decimal Amount { get; set; }

        public DateTime Warrant_Date { get; set; }

        public DateTime? Date_Info_Received { get; set; }

        [Required]
        [StringLength(6)]
        public string Warrant_Date_Short { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TB_CLAIM_SCHEDULE> TB_CLAIM_SCHEDULE { get; set; }
    }
}
