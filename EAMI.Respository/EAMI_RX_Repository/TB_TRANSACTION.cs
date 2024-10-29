namespace EAMI.Respository.EAMI_RX_Repository
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TB_TRANSACTION
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TB_TRANSACTION()
        {
            TB_PAYMENT_RECORD = new HashSet<TB_PAYMENT_RECORD>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Transaction_ID { get; set; }

        [Required]
        [StringLength(50)]
        public string Msg_Transaction_ID { get; set; }

        public int SOR_ID { get; set; }

        public int Transaction_Type_ID { get; set; }

        [Required]
        [StringLength(5)]
        public string TransactionVersion { get; set; }

        public bool UsersNotified { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TB_PAYMENT_RECORD> TB_PAYMENT_RECORD { get; set; }

        public virtual TB_REQUEST TB_REQUEST { get; set; }

        public virtual TB_SYSTEM_OF_RECORD TB_SYSTEM_OF_RECORD { get; set; }

        public virtual TB_TRANSACTION_TYPE TB_TRANSACTION_TYPE { get; set; }
    }
}
