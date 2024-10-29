namespace EAMI.Respository.EAMI_MC_Repository
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TB_FUNDING_DETAIL_EXT_CAPMAN
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Funding_Detail_ID { get; set; }

        [StringLength(20)]
        public string FedFundCode { get; set; }

        [StringLength(20)]
        public string StateFundCode { get; set; }

        public virtual TB_FUNDING_DETAIL TB_FUNDING_DETAIL { get; set; }
    }
}
