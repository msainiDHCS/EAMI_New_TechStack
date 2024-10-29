namespace EAMI.Respository.EAMI_RX_Repository
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TB_TRACE_TRANSACTION
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TB_TRACE_TRANSACTION()
        {
            TB_TRACE_PAYMENT = new HashSet<TB_TRACE_PAYMENT>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Trace_Transaction_ID { get; set; }

        [StringLength(10)]
        public string Payer_Entity_ID { get; set; }

        [StringLength(30)]
        public string Payer_Entity_ID_Type { get; set; }

        [StringLength(100)]
        public string Payer_Entity_Name { get; set; }

        [StringLength(2)]
        public string Payer_Entity_ID_Suffix { get; set; }

        [StringLength(80)]
        public string Payer_Address_Line1 { get; set; }

        [StringLength(80)]
        public string Payer_Address_Line2 { get; set; }

        [StringLength(80)]
        public string Payer_Address_Line3 { get; set; }

        [StringLength(40)]
        public string Payer_City { get; set; }

        [StringLength(2)]
        public string Payer_State { get; set; }

        [StringLength(10)]
        public string Payer_Zip { get; set; }

        [StringLength(20)]
        public string TotalPymtAmount { get; set; }

        [StringLength(10)]
        public string TotalPymtRecCount { get; set; }

        public DateTime? RejectedPaymentDateFrom { get; set; }

        public DateTime? RejectedPaymentDateTo { get; set; }

        public virtual TB_REQUEST TB_REQUEST { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TB_TRACE_PAYMENT> TB_TRACE_PAYMENT { get; set; }
    }
}
