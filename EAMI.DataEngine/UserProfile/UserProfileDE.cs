using System.Data;
using System.Data.Common;
using Dapper;

namespace EAMI.DataEngine
{
    public class UserProfileDE : IUserProfileDE//, IComparer<EAMIUser>
    {
        private readonly DataAccessBase _dataAccessBase;
        //private readonly DapperContext _dapConext;
        //public UserProfileDE(DataAccessBase dataAccessBase, DapperContext dapConext)
        //{
        //    _dataAccessBase = dataAccessBase;
        //    _dapConext = dapConext;
        //}
        public UserProfileDE(DataAccessBase dataAccessBase)
        {
            _dataAccessBase = dataAccessBase;
        }
        public List<UserProfileModelDE> GetEAMIUser(string userName)
        {
            try
            {
                var contextDb = _dataAccessBase.contextDb;
                List<UserProfileModelDE> lstUserProfile = new List<UserProfileModelDE>();

                // Define the SQL query to fetch users
                string sqlQuery = "SELECT * FROM TB_User WHERE User_Name = @UserName and IsActive = 1";

                //using var connection= _dapConext.CreateConnection();
                //var result = connection.Query<UserProfileModelDE>(sqlQuery, new { UserName = userName }).ToList();

                // Create a command object
                DbCommand dbCommand = contextDb.GetSqlStringCommand(sqlQuery);
                contextDb.AddInParameter(dbCommand, "@UserName", DbType.String, userName);
                // Execute the query and read the results
                using (IDataReader dataReader = contextDb.ExecuteReader(dbCommand))
                {
                    while (dataReader.Read())
                    {
                        UserProfileModelDE userProfile = new UserProfileModelDE
                        {

                            User_ID = dataReader.GetInt32(dataReader.GetOrdinal("User_Id")),
                            User_Name = dataReader.GetString(dataReader.GetOrdinal("User_Name")),
                            Display_Name = dataReader.GetString(dataReader.GetOrdinal("Display_Name")),
                            User_EmailAddr = dataReader.GetString(dataReader.GetOrdinal("User_EmailAddr")),
                            // Map other properties as needed
                        };
                        lstUserProfile.Add(userProfile);
                    }
                }

                return lstUserProfile;
            }
            catch (Exception ex)
            {
                // Handle exceptions as needed
                throw ex;
            }
        }
    }
}
