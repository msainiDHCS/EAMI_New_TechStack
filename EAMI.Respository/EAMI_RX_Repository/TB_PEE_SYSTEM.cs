namespace EAMI.Respository.EAMI_RX_Repository
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TB_PEE_SYSTEM
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TB_PEE_SYSTEM()
        {
            TB_PEE_ADDRESS = new HashSet<TB_PEE_ADDRESS>();
            TB_PEE_EFT_INFO = new HashSet<TB_PEE_EFT_INFO>();
        }

        [Key]
        public int PEE_System_ID { get; set; }

        public int Payment_Exchange_Entity_ID { get; set; }

        public int SOR_ID { get; set; }

        public virtual TB_PAYMENT_EXCHANGE_ENTITY TB_PAYMENT_EXCHANGE_ENTITY { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TB_PEE_ADDRESS> TB_PEE_ADDRESS { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TB_PEE_EFT_INFO> TB_PEE_EFT_INFO { get; set; }

        public virtual TB_SYSTEM_OF_RECORD TB_SYSTEM_OF_RECORD { get; set; }
    }
}
