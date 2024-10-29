using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace OHC.EAMI.WebUI.Models
{
    //public class SCOPropertiesLookUpModel
    //{
    //    public List<SCOPropertyTypeLookUpModel> SCOPropertyTypeLookUpModel = new List<SCOPropertyTypeLookUpModel>();
    //    public List<SCOPropertiesEnumsLookUpModel> SCOPropertiesEnumsLookUpModel = new List<SCOPropertiesEnumsLookUpModel>();
    //}

    public class SCOPropertyEnumsLookUpModel
    {
        public int? SCO_Property_Enum_ID { get; set; }
        public int? SCO_Property_Type_ID { get; set; }

        [Display(Name = "Code")]
        public string Code { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        public bool IsActive { get; set; }
    }
    public class SCOPropertyTypeLookUpModel
    {
        public int? SCO_Property_Type_ID { get; set; }

        [Display(Name = "Code")]
        public string Code { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }
        public bool IsActive { get; set; }
    }
}