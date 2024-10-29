
namespace EAMI.CommonEntity
{
    /// <summary>
    /// 
    /// </summary>
    public class EAMIUser : BaseEntity//, IComparer<EAMIUser>
    {
        public int? User_ID { get; set; }

        public string User_Name { get; set; }

        public string Display_Name { get; set; }

        public string User_EmailAddr { get; set; }

        public string User_Password { get; set; }

        public string Domain_Name { get; set; }

        public EAMIAuthBase User_Type { get; set; }

        public List<EAMIAuthBase> User_Roles { get; set; }

        public List<EAMIAuthBase> Permissions { get; set; }

        public List<EAMIAuthBase> User_Systems { get; set; }

        public string DelimitedRoleNames
        {
            get
            {
                return string.Join(", ", User_Roles.Select(a => a.Name).ToArray());
            }
        }

        public string DelimitedSystemNames
        {
            get
            {
                return string.Join(", ", User_Systems.Select(a => a.Name).ToArray());
            }
        }

        public string Status
        {
            get
            {
                return (base.IsActive ? "Active" : "Inactive");
            }
        }

        public DateTime LastUpdateDate
        {
            get
            {

                return UpdateDate != null ? CreateDate : Convert.ToDateTime(UpdateDate);
            }
        }

        public int Compare(EAMIUser a, EAMIUser b)
        {
            if (a.LastUpdateDate > b.LastUpdateDate)
                return 1;
            else
                return 0;
        }
    }
}
