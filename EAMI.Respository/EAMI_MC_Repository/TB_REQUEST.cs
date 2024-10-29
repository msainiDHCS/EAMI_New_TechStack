namespace EAMI.Respository.EAMI_MC_Repository
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TB_REQUEST
    {
        [Key]
        public int Request_ID { get; set; }

        public DateTime Request_TimeStamp { get; set; }

        public DateTime? Msg_TimeStamp { get; set; }

        [StringLength(20)]
        public string Msg_Sender_ID { get; set; }

        [StringLength(50)]
        public string Msg_Transaction_ID { get; set; }

        [StringLength(50)]
        public string Msg_Transaction_Type { get; set; }

        [StringLength(5)]
        public string Msg_Transaction_Version { get; set; }

        public virtual TB_RESPONSE TB_RESPONSE { get; set; }

        public virtual TB_TRACE_TRANSACTION TB_TRACE_TRANSACTION { get; set; }

        public virtual TB_TRANSACTION TB_TRANSACTION { get; set; }
    }
}
