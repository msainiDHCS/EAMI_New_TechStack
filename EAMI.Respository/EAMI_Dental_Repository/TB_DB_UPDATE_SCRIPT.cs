namespace EAMI.Respository.EAMI_Dental_Repository
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TB_DB_UPDATE_SCRIPT
    {
        [Key]
        [Column(Order = 0)]
        public int DB_Update_Script_ID { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(50)]
        public string Release_Number { get; set; }

        [Key]
        [Column(Order = 2)]
        public DateTime Applied_Date_Time { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(200)]
        public string Script_Name { get; set; }

        [Key]
        [Column(Order = 4)]
        [StringLength(2000)]
        public string Script_Description { get; set; }
    }
}
