namespace EAMI.Respository.EAMI_MC_Repository
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TB_FUNDING_DETAIL_KVP
    {
        [Key]
        public int Funding_Detail_Kvp_ID { get; set; }

        public int Funding_Detail_ID { get; set; }

        public int SOR_Kvp_Key_ID { get; set; }

        [Required]
        [StringLength(100)]
        public string Kvp_Value { get; set; }

        public virtual TB_FUNDING_DETAIL TB_FUNDING_DETAIL { get; set; }

        public virtual TB_SOR_KVP_KEY TB_SOR_KVP_KEY { get; set; }
    }
}
