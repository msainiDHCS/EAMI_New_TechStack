using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using EAMI.Common;
using EAMI.CommonEntity;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace EAMI.DataEngine
{
    public class UserAuthorizeDE : IUserAuthorizeDE
    {
        private readonly DataAccessBase _dataAccessBase;
        private Database _contextDb;
        public UserAuthorizeDE(DataAccessBase dataAccessBase)
        {
            _dataAccessBase = dataAccessBase;
            _contextDb = dataAccessBase.contextDb;
        }

        public CommonStatusPayload<EAMIUser> GetEAMIUser(string userName, string domainName = null, string password = null, bool verifyPassword = false)
        {
            CommonStatus cs = new CommonStatus(false);
            EAMIUser retUser = new EAMIUser();

            try
            {
                DataSet ds = new DataSet();

                string[] tableNames = new string[] { "user", "role", "permission", "system" };
                _contextDb.LoadDataSet(DbStoredProcs.spGetEAMIUser, ds, tableNames, DbStoredProcs.GetEAMIUserParams(userName, domainName, password));

                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataTable tb in ds.Tables)
                    {
                        if (tb.TableName == "user")
                        {
                            if (tb.Rows.Count == 1)
                            {

                                retUser.User_ID = Convert.ToInt32(tb.Rows[0]["User_ID"].ToString());
                                retUser.User_Name = tb.Rows[0]["User_Name"].ToString();
                                retUser.Display_Name = tb.Rows[0]["Display_Name"].ToString();
                                retUser.User_EmailAddr = tb.Rows[0]["User_EmailAddr"].ToString();
                                retUser.User_Password = tb.Rows[0]["User_Password"].ToString();
                                retUser.Domain_Name = tb.Rows[0]["Domain_Name"].ToString();
                                retUser.User_Type = new EAMIAuthBase();
                                retUser.User_Type.ID = Convert.ToInt32(tb.Rows[0]["User_Type_ID"]);
                                retUser.User_Type.Code = tb.Rows[0]["User_Type_Code"].ToString();
                                retUser.User_Type.Name = tb.Rows[0]["User_Type_Name"].ToString();

                                //verify password
                                if (verifyPassword && password != null)
                                {
                                    if (!EAMIHashGenerator.VerifyHash(password, "SHA512", retUser.User_Password))
                                    {
                                        retUser = null;
                                        cs.Status = false;
                                        cs.MessageDetailList.Add("Incorrect password");
                                        break;
                                    }
                                }

                                cs.Status = true;
                            }
                            else
                            {
                                retUser = null;
                                cs.Status = false;
                                cs.MessageDetailList.Add("Invalid user");
                                break;
                            }
                        }

                        if (tb.TableName == "role" && tb.Rows.Count > 0)
                        {
                            retUser.User_Roles = new List<EAMIAuthBase>();

                            for (int i = 0; i < tb.Rows.Count; i++)
                            {
                                EAMIAuthBase ab = new EAMIAuthBase();

                                ab.ID = Convert.ToInt32(tb.Rows[i]["Role_ID"]);
                                ab.Code = tb.Rows[i]["Role_Code"].ToString();
                                ab.Name = tb.Rows[i]["Role_Name"].ToString();

                                retUser.User_Roles.Add(ab);
                            }
                        }

                        if (tb.TableName == "permission" && tb.Rows.Count > 0)
                        {
                            retUser.Permissions = new List<EAMIAuthBase>();

                            for (int i = 0; i < tb.Rows.Count; i++)
                            {
                                EAMIAuthBase ab = new EAMIAuthBase();

                                ab.ID = Convert.ToInt32(tb.Rows[i]["Permission_ID"]);
                                ab.Code = tb.Rows[i]["Permission_Code"].ToString();
                                ab.Name = tb.Rows[i]["Permission_Name"].ToString();

                                retUser.Permissions.Add(ab);
                            }
                        }

                        if (tb.TableName == "system" && tb.Rows.Count > 0)
                        {
                            retUser.User_Systems = new List<EAMIAuthBase>();

                            for (int i = 0; i < tb.Rows.Count; i++)
                            {
                                EAMIAuthBase ab = new EAMIAuthBase();

                                ab.ID = Convert.ToInt32(tb.Rows[i]["System_ID"]);
                                ab.Code = tb.Rows[i]["System_Code"].ToString();
                                ab.Name = tb.Rows[i]["System_Name"].ToString();

                                retUser.User_Systems.Add(ab);
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                //EAMILogger.Instance.Error(ex);
                cs.Status = false;
                cs.IsFatal = true;
                cs.MessageDetailList.Add(ex.Message);
            }

            return new CommonStatusPayload<EAMIUser>(retUser, cs.Status, cs.IsFatal, cs.MessageDetailList);
        }

        public CommonStatusPayload<EAMIUser> GetEAMIUserByID(long userID)
        {
            CommonStatus cs = new CommonStatus(false);
            EAMIUser retUser = new EAMIUser();

            try
            {
                DataSet ds = new DataSet();

                string[] tableNames = new string[] { "user", "role", "permission", "system" };
                object[] spparams = new object[] { userID };
                _contextDb.LoadDataSet("usp_GetEAMIUserByID", ds, tableNames, spparams);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataTable tb in ds.Tables)
                    {
                        if (tb.TableName == "user")
                        {
                            if (tb.Rows.Count == 1)
                            {
                                retUser.User_Name = tb.Rows[0]["User_Name"].ToString();
                                retUser.Display_Name = tb.Rows[0]["Display_Name"].ToString();
                                retUser.User_EmailAddr = tb.Rows[0]["User_EmailAddr"].ToString();
                                retUser.User_Password = tb.Rows[0]["User_Password"].ToString();
                                retUser.Domain_Name = tb.Rows[0]["Domain_Name"].ToString();
                                retUser.User_Type = new EAMIAuthBase();
                                retUser.User_Type.ID = Convert.ToInt32(tb.Rows[0]["User_Type_ID"]);
                                retUser.User_Type.Code = tb.Rows[0]["User_Type_Code"].ToString();
                                retUser.User_Type.Name = tb.Rows[0]["User_Type_Name"].ToString();

                                retUser.IsActive = Convert.ToBoolean(tb.Rows[0]["UserStatus"]);

                                cs.Status = true;
                            }
                            else
                            {
                                retUser = null;
                                cs.Status = false;
                                cs.MessageDetailList.Add("Invalid user");
                                break;
                            }
                        }

                        if (tb.TableName == "role" && tb.Rows.Count > 0)
                        {
                            retUser.User_Roles = new List<EAMIAuthBase>();

                            for (int i = 0; i < tb.Rows.Count; i++)
                            {
                                EAMIAuthBase ab = new EAMIAuthBase();

                                ab.ID = Convert.ToInt32(tb.Rows[i]["Role_ID"]);
                                ab.Code = tb.Rows[i]["Role_Code"].ToString();
                                ab.Name = tb.Rows[i]["Role_Name"].ToString();

                                retUser.User_Roles.Add(ab);
                            }
                        }

                        if (tb.TableName == "permission" && tb.Rows.Count > 0)
                        {
                            retUser.Permissions = new List<EAMIAuthBase>();

                            for (int i = 0; i < tb.Rows.Count; i++)
                            {
                                EAMIAuthBase ab = new EAMIAuthBase();

                                ab.ID = Convert.ToInt32(tb.Rows[i]["Permission_ID"]);
                                ab.Code = tb.Rows[i]["Permission_Code"].ToString();
                                ab.Name = tb.Rows[i]["Permission_Name"].ToString();

                                retUser.Permissions.Add(ab);
                            }
                        }

                        if (tb.TableName == "system" && tb.Rows.Count > 0)
                        {
                            retUser.User_Systems = new List<EAMIAuthBase>();

                            for (int i = 0; i < tb.Rows.Count; i++)
                            {
                                EAMIAuthBase ab = new EAMIAuthBase();

                                ab.ID = Convert.ToInt32(tb.Rows[i]["System_ID"]);
                                ab.Code = tb.Rows[i]["System_Code"].ToString();
                                ab.Name = tb.Rows[i]["System_Name"].ToString();

                                retUser.User_Systems.Add(ab);
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                //EAMILogger.Instance.Error(ex);
                cs.Status = false;
                cs.IsFatal = true;
                cs.MessageDetailList.Add(ex.Message);
            }

            return new CommonStatusPayload<EAMIUser>(retUser, cs.Status, cs.IsFatal, cs.MessageDetailList);
        }

        /// <summary>
        ///  Deactivate user if User is not in Active Directory
        /// </summary>
        /// <param name="inputUser"></param>
        ///  <param name="loginUserName"></param>
        /// <returns></returns>
        public CommonStatus DeactivateEAMIUser(EAMIUser inputUser, string loginUserName)
        {
            CommonStatus cs = new CommonStatus(false);

            try
            {
                DbCommand cmd = _contextDb.GetStoredProcCommand("usp_DeactivateEAMIUser");

                _contextDb.AddInParameter(cmd, "UserID", DbType.Int64, inputUser.User_ID);               
                _contextDb.AddInParameter(cmd, "IsActive", DbType.Boolean, 0);
                _contextDb.AddInParameter(cmd, "LoginUserName", DbType.String, loginUserName);

                _contextDb.AddOutParameter(cmd, "Status", DbType.String, 10);
                _contextDb.AddOutParameter(cmd, "Message", DbType.String, -1);
               

                _contextDb.ExecuteNonQuery(cmd);

                string status = _contextDb.GetParameterValue(cmd, "Status").ToString();
                string message = _contextDb.GetParameterValue(cmd, "Message").ToString();

                cs.Status = (status == "OK");

                if (!cs.Status)
                {
                    cs.AddMessageDetail(message);
                }

            }
            catch (Exception ex)
            {
                //EAMILogger.Instance.Error(ex);
                cs.Status = false;
                cs.IsFatal = true;
                cs.MessageDetailList.Add(ex.Message);
            }

            return cs;
        }



        /// <summary>
        ///  Deactivate user if User is not in Active Directory
        /// </summary>
        /// <param name="inputUser"></param>       
        /// <returns></returns>
        public CommonStatus GetActivestatusEAMIUserInfo(long inputUser )
        {
            CommonStatus cs = new CommonStatus(false);
            try
            {           
                DbCommand cmd = _contextDb.GetStoredProcCommand("usp_GetEAMIUserActiveInfo");
                _contextDb.AddInParameter(cmd, "UserID", DbType.Int64, inputUser);
                _contextDb.AddOutParameter(cmd, "Status", DbType.Boolean, 0);

                int cnt = _contextDb.ExecuteNonQuery(cmd);

                bool isValid = Convert.ToBoolean(_contextDb.GetParameterValue(cmd, "Status"));

                cs.Status = isValid;
            }
            catch (Exception ex)
            {
                //EAMILogger.Instance.Error(ex);
                cs.Status = false;
                cs.IsFatal = true;
                cs.MessageDetailList.Add(ex.Message);
            }
            return cs;
        }


        public CommonStatus AddUpdateEAMIUser(EAMIUser inputUser, string loginUserName)
        {
            CommonStatus cs = new CommonStatus(false);

            try
            {
                DbCommand cmd = _contextDb.GetStoredProcCommand("usp_AddUpdateEAMIUser");

                _contextDb.AddInParameter(cmd, "UserID", DbType.Int64, inputUser.User_ID);
                _contextDb.AddInParameter(cmd, "UserName", DbType.String, inputUser.User_Name);
                _contextDb.AddInParameter(cmd, "DisplayName", DbType.String, inputUser.Display_Name);
                _contextDb.AddInParameter(cmd, "UserEmailAddr", DbType.String, inputUser.User_EmailAddr);
                if (!string.IsNullOrWhiteSpace(inputUser.User_Password))
                    _contextDb.AddInParameter(cmd, "UserPassword", DbType.String, EAMIHashGenerator.ComputeHash(inputUser.User_Password));
                else
                    _contextDb.AddInParameter(cmd, "UserPassword", DbType.String, null);
                _contextDb.AddInParameter(cmd, "DomainName", DbType.String, inputUser.Domain_Name);
                _contextDb.AddInParameter(cmd, "UserTypeID", DbType.Int64, inputUser.User_Type.ID);
                _contextDb.AddInParameter(cmd, "IsActive", DbType.Boolean, inputUser.IsActive);

                //-------
                DataTable tbRoles = new DataTable();
                tbRoles.Columns.Add("ID", typeof(Int64));

                foreach (var r in inputUser.User_Roles)
                {
                    tbRoles.Rows.Add(r.ID);
                }

                SqlParameter pRole = new SqlParameter("Roles", tbRoles);
                pRole.SqlDbType = SqlDbType.Structured;
                cmd.Parameters.Add(pRole);
                //------

                //-------
                DataTable tbSystems = new DataTable();
                tbSystems.Columns.Add("ID", typeof(Int64));

                foreach (var r in inputUser.User_Systems)
                {
                    tbSystems.Rows.Add(r.ID);
                }

                SqlParameter pSystem = new SqlParameter("Systems", tbSystems);
                pSystem.SqlDbType = SqlDbType.Structured;
                cmd.Parameters.Add(pSystem);
                //------

                _contextDb.AddInParameter(cmd, "LoginUserName", DbType.String, loginUserName);
                _contextDb.AddOutParameter(cmd, "Status", DbType.String, 10);
                _contextDb.AddOutParameter(cmd, "Message", DbType.String, -1);

                _contextDb.ExecuteNonQuery(cmd);

                string status = _contextDb.GetParameterValue(cmd, "Status").ToString();
                string message = _contextDb.GetParameterValue(cmd, "Message").ToString();

                cs.Status = (status == "OK");

                if (!cs.Status)
                {
                    cs.AddMessageDetail(message);
                }

            }
            catch (Exception ex)
            {
                //EAMILogger.Instance.Error(ex);
                cs.Status = false;
                cs.IsFatal = true;
                cs.MessageDetailList.Add(ex.Message);
            }

            return cs;
        }

        public CommonStatusPayload<List<Tuple<string, EAMIAuthBase>>> GetEAMIAuthorizationLookUps(string lookUpType = null)
        {
            List<Tuple<string, EAMIAuthBase>> lstLookUps = new List<Tuple<string, EAMIAuthBase>>();
            CommonStatus cs = new CommonStatus(false);

            try
            {
                using (var reader = _contextDb.ExecuteReader(CommandType.StoredProcedure, "usp_GetEAMILookUps"))
                {
                    while (reader.Read())
                    {
                        EAMIAuthBase us = new EAMIAuthBase();

                        us.ID = Convert.ToInt32(reader["ID"]);
                        us.Code = reader["Code"].ToString();
                        us.Name = reader["Name"].ToString();

                        var type = reader["Type"].ToString();

                        lstLookUps.Add(new Tuple<string, EAMIAuthBase>(type, us));
                    }

                    cs.Status = true;
                }
            }
            catch (Exception ex)
            {
                //EAMILogger.Instance.Error(ex);
                cs.Status = false;
                cs.IsFatal = true;
                cs.MessageDetailList.Add(ex.Message);
            }

            return new CommonStatusPayload<List<Tuple<string, EAMIAuthBase>>>(lstLookUps, cs.Status, cs.IsFatal, cs.MessageDetailList);
        }

        public CommonStatus CheckEAMIUserValidity(string userName, long userType)
        {
            CommonStatus cs = new CommonStatus(false);

            try
            {
                DbCommand cmd = _contextDb.GetStoredProcCommand("usp_CheckUserValidity");

                _contextDb.AddInParameter(cmd, "UserName", DbType.String, userName);
                _contextDb.AddInParameter(cmd, "UserTypeID", DbType.Int64, userType);
                _contextDb.AddOutParameter(cmd, "Status", DbType.Boolean, 1);

                int cnt = _contextDb.ExecuteNonQuery(cmd);

                bool isValid = Convert.ToBoolean(_contextDb.GetParameterValue(cmd, "Status"));

                cs.Status = isValid;

            }
            catch (Exception ex)
            {
                //EAMILogger.Instance.Error(ex);
                cs.Status = false;
                cs.IsFatal = true;
                cs.MessageDetailList.Add(ex.Message);
            }

            return cs;
        }

        public CommonStatusPayload<List<EAMIUser>> GetAllEAMIUsers()
        {
            CommonStatus cs = new CommonStatus(false);
            List<EAMIUser> retUserList = new List<EAMIUser>();

            try
            {
                using (var reader = _contextDb.ExecuteReader(CommandType.StoredProcedure, "usp_GetAllEAMIUsers"))
                {
                    while (reader.Read())
                    {
                        EAMIUser us = new EAMIUser();
                        us.User_ID = Convert.ToInt32(reader["User_ID"]);
                        us.User_Name = reader["User_Name"].ToString();
                        us.Domain_Name = reader["Domain_Name"].ToString();
                        us.User_EmailAddr = reader["User_EmailAddr"].ToString();
                        us.Display_Name = reader["Display_Name"].ToString();
                        us.User_Type = new EAMIAuthBase();
                        us.User_Type.ID = Convert.ToInt32(reader["User_Type_ID"]);
                        us.User_Type.Code = reader["User_Type_Code"].ToString();
                        us.User_Type.Name = reader["User_Type_Name"].ToString();
                        us.CreateDate = Convert.ToDateTime(reader["CreateDate"]);
                        us.UpdateDate = reader["UpdateDate"] == null ? Convert.ToDateTime(reader["UpdateDate"]) : us.UpdateDate.GetValueOrDefault();

                        us.User_Roles = new List<EAMIAuthBase>();
                        string[] strArr = reader["Roles"].ToString().Split(new char[] { ',' });
                        foreach (string str in strArr)
                        {
                            us.User_Roles.Add(new EAMIAuthBase() { Name = str, IsActive = true });
                        }

                        us.User_Systems = new List<EAMIAuthBase>();
                        strArr = reader["Systems"].ToString().Split(new char[] { ',' });
                        foreach (string str in strArr)
                        {
                            us.User_Systems.Add(new EAMIAuthBase() { Name = str, IsActive = true });
                        }

                        us.IsActive = Convert.ToBoolean(reader["UserStatus"]);

                        retUserList.Add(us);
                    }

                    cs.Status = true;
                }
            }
            catch (Exception ex)
            {
                //EAMILogger.Instance.Error(ex);
                cs.Status = false;
                cs.IsFatal = true;
                cs.MessageDetailList.Add(ex.Message);
            }

            return new CommonStatusPayload<List<EAMIUser>>(retUserList, cs.Status, cs.IsFatal, cs.MessageDetailList);
        }

        public CommonStatus AddUpdateEAMIMasterData(EAMIMasterData inputData, string loginUserName)
        {
            CommonStatus cs = new CommonStatus(false);

            try
            {
                DbCommand cmd = _contextDb.GetStoredProcCommand("usp_AddUpdateDataByType");

                _contextDb.AddInParameter(cmd, "Type", DbType.String, inputData.DataType);
                _contextDb.AddInParameter(cmd, "ID", DbType.Int64, inputData.ID);
                _contextDb.AddInParameter(cmd, "Code", DbType.String, inputData.Code);
                _contextDb.AddInParameter(cmd, "Name", DbType.String, inputData.Name);

                _contextDb.AddInParameter(cmd, "RA_Dept_Name", DbType.String, inputData.DepartmentName);
                _contextDb.AddInParameter(cmd, "RA_ORG_Code", DbType.String, inputData.OrganizationCode);
                _contextDb.AddInParameter(cmd, "RA_Dept_Add_LN", DbType.String, inputData.RADepartmentAddrLine);
                _contextDb.AddInParameter(cmd, "RA_Dept_Add_CSZ", DbType.String, inputData.RADepartmentAddrCSZ);

                _contextDb.AddInParameter(cmd, "RA_Inq_PH_No", DbType.String, inputData.RAInquiryPhNo);
                _contextDb.AddInParameter(cmd, "FEIN_Number", DbType.String, inputData.FEIN_Number);

                _contextDb.AddInParameter(cmd, "MAX_Pymt_Rec_Amt", DbType.String, inputData.MaxPmtRecAmt);
                _contextDb.AddInParameter(cmd, "MAX_Pymt_Rec_Per_Tran", DbType.String, inputData.MaxPmtRecPerTran);

                _contextDb.AddInParameter(cmd, "MAX_Funding_DTL_Per_Pymt_Rec", DbType.String, inputData.MaxFundingDtlPerPmtRec);
                _contextDb.AddInParameter(cmd, "TRACE_Incoming_Pmt_Data", DbType.String, inputData.TraceIncomingPmtData);
                _contextDb.AddInParameter(cmd, "Validate_Funding_Source", DbType.String, inputData.ValidateFundingSource);
                _contextDb.AddInParameter(cmd, "Title_Transfer_Letter", DbType.String, inputData.TitleTransferLetter);
                _contextDb.AddInParameter(cmd, "IsActive", DbType.Boolean, inputData.IsActive);

                DataTable tbAssociatedData = new DataTable();
                tbAssociatedData.Columns.Add("ID", typeof(Int64));

                if (inputData.AssociatedData != null)
                {
                    foreach (var r in inputData.AssociatedData.Where(a => a.IsSelected))
                        tbAssociatedData.Rows.Add(r.ID);
                }

                SqlParameter pRole = new SqlParameter("AssociatedData", tbAssociatedData);
                pRole.SqlDbType = SqlDbType.Structured;
                cmd.Parameters.Add(pRole);


                _contextDb.AddInParameter(cmd, "LoginUserName", DbType.String, loginUserName);
                _contextDb.AddOutParameter(cmd, "Status", DbType.String, 10);
                _contextDb.AddOutParameter(cmd, "Message", DbType.String, -1);

                _contextDb.ExecuteNonQuery(cmd);

                string status = _contextDb.GetParameterValue(cmd, "Status").ToString();
                string message = _contextDb.GetParameterValue(cmd, "Message").ToString();

                cs.Status = (status == "OK");

                if (!cs.Status)
                {
                    cs.AddMessageDetail(message);
                }

            }
            catch (Exception ex)
            {
                //EAMILogger.Instance.Error(ex);
                cs.Status = false;
                cs.IsFatal = true;
                cs.MessageDetailList.Add(ex.Message);
            }

            return cs;
        }

        public CommonStatusPayload<List<EAMIMasterData>> GetAllEAMIMasterData(string DataType)
        {
            CommonStatus cs = new CommonStatus(false);
            List<EAMIMasterData> retData = new List<EAMIMasterData>();

            try
            {
                DataSet ds = new DataSet();

                string[] tableNames = new string[] { "data" };
                _contextDb.LoadDataSet(DbStoredProcs.spGetAllDataByType, ds, tableNames, DbStoredProcs.GetAllDataTypeParams(DataType));

                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataTable tb = ds.Tables[0];

                    for (int i = 0; i < tb.Rows.Count; i++)
                    {
                        EAMIMasterData ab = new EAMIMasterData();

                        ab.ID = Convert.ToInt32(tb.Rows[i]["ID"].ToString());
                        ab.Code = tb.Rows[i]["Code"].ToString();
                        ab.Name = tb.Rows[i]["Name"].ToString();
                        ab.IsActive = Convert.ToBoolean(tb.Rows[i]["IsActive"]);
                        ab.CreatedBy = tb.Rows[i]["CreatedBy"].ToString();
                        ab.CreateDate = Convert.ToDateTime(tb.Rows[i]["CreateDate"]);
                        ab.DepartmentName = tb.Rows[i]["DepartmentName"].ToString();
                        ab.OrganizationCode = tb.Rows[i]["OrganizationCode"].ToString();

                        if (tb.Rows[i]["UpdatedBy"] != null)
                            ab.UpdatedBy = tb.Rows[i]["UpdatedBy"].ToString();

                        if (tb.Rows[i]["UpdateDate"] != null)
                            ab.UpdateDate = !string.IsNullOrEmpty(tb.Rows[i]["UpdateDate"].ToString()) ? Convert.ToDateTime(tb.Rows[i]["UpdateDate"]) : ab.UpdateDate.GetValueOrDefault();

                        string permissioncodes = tb.Rows[i]["AssociatedData"].ToString();
                        if (permissioncodes.Length > 0)
                        {
                            ab.AssociatedData = new List<EAMIAuthBase>();
                            foreach (string permissioncode in permissioncodes.Split(new[] { ',' }))
                            {
                                ab.AssociatedData.Add(new EAMIAuthBase() { Code = permissioncode });
                            }
                        }

                        retData.Add(ab);
                    }

                    cs.Status = true;
                }
            }
            catch (Exception ex)
            {
                //EAMILogger.Instance.Error(ex);
                cs.Status = false;
                cs.IsFatal = true;
                cs.MessageDetailList.Add(ex.Message);
            }

            return new CommonStatusPayload<List<EAMIMasterData>>(retData, cs.Status, cs.IsFatal, cs.MessageDetailList);
        }

        public CommonStatusPayload<EAMIMasterData> GetEAMIMasterDataByID(string DataType, long DataTID)
        {
            CommonStatus cs = new CommonStatus(false);
            EAMIMasterData retData = new EAMIMasterData();

            try
            {
                DataSet ds = new DataSet();

                string[] tableNames = new string[] { "data" };
                _contextDb.LoadDataSet(DbStoredProcs.spGetGivenDataByType, ds, tableNames, DbStoredProcs.GetGivenDataByTypeParams(DataType, DataTID));

                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataTable tb = ds.Tables[0];

                    if (tb.Rows.Count == 1)
                    {
                        retData.ID = Convert.ToInt32(tb.Rows[0]["ID"].ToString());
                        retData.Code = tb.Rows[0]["Code"].ToString();
                        retData.Name = tb.Rows[0]["Name"].ToString();
                        if (DataType == "SYSTEM")
                        {
                            retData.DepartmentName = tb.Rows[0]["RA_Dept_Name"].ToString();
                            retData.RADepartmentAddrLine = tb.Rows[0]["RA_Dept_Add_LN"].ToString();
                            retData.RADepartmentAddrCSZ = tb.Rows[0]["RA_Dept_Add_CSZ"].ToString();
                            retData.OrganizationCode = tb.Rows[0]["RA_ORG_Code"].ToString();
                            retData.RAInquiryPhNo = tb.Rows[0]["RA_Inq_PH_No"].ToString();
                            retData.FEIN_Number = tb.Rows[0]["FEIN_Number"].ToString();
                            retData.MaxPmtRecAmt = tb.Rows[0]["MAX_Pymt_Rec_Amt"].ToString();
                            retData.MaxPmtRecPerTran = tb.Rows[0]["MAX_Pymt_Rec_Per_Tran"].ToString();
                            retData.MaxFundingDtlPerPmtRec = tb.Rows[0]["MAX_Funding_DTL_Per_Pymt_Rec"].ToString();
                            retData.TraceIncomingPmtData = (bool)tb.Rows[0]["TRACE_Incoming_Pmt_Data"];
                            retData.ValidateFundingSource = (bool)tb.Rows[0]["Validate_Funding_Source"];
                            retData.TitleTransferLetter = tb.Rows[0]["Title_Transfer_Letter"].ToString();
                        }

                        retData.IsActive = Convert.ToBoolean(tb.Rows[0]["IsActive"]);
                        retData.CreatedBy = tb.Rows[0]["CreatedBy"].ToString();
                        retData.CreateDate = Convert.ToDateTime(tb.Rows[0]["CreateDate"]);


                        if (tb.Rows[0]["UpdatedBy"] != null)
                            retData.UpdatedBy = tb.Rows[0]["UpdatedBy"].ToString();

                        if (tb.Rows[0]["UpdateDate"] != null)
                            retData.UpdateDate = !string.IsNullOrEmpty(tb.Rows[0]["UpdateDate"].ToString()) ? Convert.ToDateTime(tb.Rows[0]["UpdateDate"]) : retData.UpdateDate.GetValueOrDefault();


                        string permissionids = tb.Rows[0]["AssociatedData"].ToString();
                        if (permissionids.Length > 0)
                        {
                            retData.AssociatedData = new List<EAMIAuthBase>();
                            foreach (string permissionID in permissionids.Split(new[] { ',' }))
                            {
                                retData.AssociatedData.Add(new EAMIAuthBase() { ID = Convert.ToInt32(permissionID) });
                            }
                        }

                        cs.Status = true;
                    }
                }
            }
            catch (Exception ex)
            {
                //EAMILogger.Instance.Error(ex);
                cs.Status = false;
                cs.IsFatal = true;
                cs.MessageDetailList.Add(ex.Message);
            }

            return new CommonStatusPayload<EAMIMasterData>(retData, cs.Status, cs.IsFatal, cs.MessageDetailList);
        }

        public CommonStatusPayload<List<Tuple<EAMIDateType, string, DateTime>>> GetYearlyCalendarEntries(int activeYear, string loginUserName)
        {
            // get a pay and draw dates 
            CommonStatus cs = new CommonStatus(false);
            List<Tuple<EAMIDateType, string, DateTime>> retData = new List<Tuple<EAMIDateType, string, DateTime>>();

            try
            {
                DataSet ds = new DataSet();

                string[] tableNames = new string[] { "data" };
                object[] spparams = new object[] { activeYear };
                _contextDb.LoadDataSet("usp_GetCalendarByYear", ds, tableNames, spparams);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataTable tb = ds.Tables[0];

                    for (int i = 0; i < tb.Rows.Count; i++)
                    {
                        DateTime dt = Convert.ToDateTime(tb.Rows[i]["Date"]);
                        string type = tb.Rows[i]["Type"].ToString();
                        string note = tb.Rows[i]["Note"].ToString();

                        retData.Add(new Tuple<EAMIDateType, string, DateTime>((type == "P" ? EAMIDateType.PayDate : EAMIDateType.DrawDate), note, dt));

                    }

                    cs.Status = true;
                }
            }
            catch (Exception ex)
            {
                //EAMILogger.Instance.Error(ex);
                cs.Status = false;
                cs.IsFatal = true;
                cs.MessageDetailList.Add(ex.Message);
            }

            return new CommonStatusPayload<List<Tuple<EAMIDateType, string, DateTime>>>(retData, cs.Status, cs.IsFatal, cs.MessageDetailList);

        }

        public CommonStatus UpdateYearlyCalendarEntry(List<Tuple<EAMIDateType, EAMICalendarAction, string, DateTime>> dates, string loginUserName)
        {
            CommonStatus cs = new CommonStatus(false);

            try
            {
                DbCommand cmd = _contextDb.GetStoredProcCommand("usp_UpdateYearlyCalendarEntry");

                DataTable tbDates = new DataTable();
                tbDates.Columns.Add("EAMIDate", typeof(DateTime));
                tbDates.Columns.Add("EAMIDateType", typeof(string));
                tbDates.Columns.Add("EAMIDateActionType", typeof(string));
                tbDates.Columns.Add("Comment", typeof(string));

                foreach (var r in dates)
                {
                    tbDates.Rows.Add(r.Item4, (r.Item1 == EAMIDateType.PayDate ? "P" : "D"), (r.Item2 == EAMICalendarAction.Add ? "A" : (r.Item2 == EAMICalendarAction.Delete ? "D" : "U")), r.Item3);
                }

                SqlParameter pDates = new SqlParameter("Dates", tbDates);
                pDates.SqlDbType = SqlDbType.Structured;
                cmd.Parameters.Add(pDates);

                _contextDb.AddInParameter(cmd, "LoginUserName", DbType.String, loginUserName);
                _contextDb.AddOutParameter(cmd, "Status", DbType.String, 10);
                _contextDb.AddOutParameter(cmd, "Message", DbType.String, -1);

                _contextDb.ExecuteNonQuery(cmd);

                string status = _contextDb.GetParameterValue(cmd, "Status").ToString();
                string message = _contextDb.GetParameterValue(cmd, "Message").ToString();

                cs.Status = (status == "OK");

                if (!cs.Status)
                {
                    cs.AddMessageDetail(message);
                }

            }
            catch (Exception ex)
            {
                //EAMILogger.Instance.Error(ex);
                cs.Status = false;
                cs.IsFatal = true;
                cs.MessageDetailList.Add(ex.Message);
            }

            return cs;
        }
    }
}
