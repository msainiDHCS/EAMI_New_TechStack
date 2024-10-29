namespace EAMI.Respository.EAMI_Dental_Repository
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TB_RESPONSE
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Response_ID { get; set; }

        [Required]
        [StringLength(50)]
        public string Msg_Transaction_ID { get; set; }

        [Required]
        [StringLength(50)]
        public string Msg_Transaction_Type { get; set; }

        public DateTime Response_TimeStamp { get; set; }

        public int Response_Status_Type_ID { get; set; }

        [StringLength(200)]
        public string Response_Message { get; set; }

        public virtual TB_REQUEST TB_REQUEST { get; set; }

        public virtual TB_RESPONSE_STATUS_TYPE TB_RESPONSE_STATUS_TYPE { get; set; }
    }
}
