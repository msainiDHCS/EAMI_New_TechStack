namespace EAMI.Respository.EAMI_RX_Repository
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TB_PAYMENT_USER_ASSIGNMENT
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TB_PAYMENT_USER_ASSIGNMENT()
        {
            TB_PAYMENT_DN_STATUS = new HashSet<TB_PAYMENT_DN_STATUS>();
        }

        [Key]
        public int Payment_User_Assignment_ID { get; set; }

        public int Payment_Record_ID { get; set; }

        public int User_ID { get; set; }

        public int Assigned_Payment_Status_ID { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TB_PAYMENT_DN_STATUS> TB_PAYMENT_DN_STATUS { get; set; }

        public virtual TB_PAYMENT_RECORD TB_PAYMENT_RECORD { get; set; }

        public virtual TB_PAYMENT_STATUS TB_PAYMENT_STATUS { get; set; }

        public virtual TB_USER TB_USER { get; set; }
    }
}
