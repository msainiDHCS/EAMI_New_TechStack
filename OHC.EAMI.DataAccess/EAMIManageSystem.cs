using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using OHC.EAMI.Common;
using OHC.EAMI.CommonEntity;
using OHC.EAMI.CommonEntity.DataEntity;

namespace OHC.EAMI.DataAccess
{
    public class EAMIManageSystem : DataAccessBase
    {
        public EAMIManageSystem() { }

        #region Fund...

        /// <summary>
        /// Add a new Fund in system...
        /// </summary>
        /// <param name="inputFund"></param>
        /// <param name="loginUserName"></param>
        /// <returns></returns>
        public CommonStatus AddEAMIFund(Fund inputFund)
        {
            CommonStatus cs = new CommonStatus(false);

            try
            {
                DbCommand cmd = base.contextDb.GetStoredProcCommand("AddFund");

                base.contextDb.AddInParameter(cmd, "Fund_Code", DbType.String, inputFund.Fund_Code);
                base.contextDb.AddInParameter(cmd, "Fund_Description", DbType.String, inputFund.Fund_Description);
                base.contextDb.AddInParameter(cmd, "System_ID", DbType.String, inputFund.System_ID);
                base.contextDb.AddInParameter(cmd, "Fund_Name", DbType.String, inputFund.Fund_Name);
                base.contextDb.AddInParameter(cmd, "Stat_Year", DbType.String, inputFund.Stat_Year);
                base.contextDb.AddInParameter(cmd, "IsActive", DbType.Boolean, inputFund.IsActive);
                base.contextDb.AddInParameter(cmd, "CreatedBy", DbType.String, inputFund.CreatedBy);
                base.contextDb.AddOutParameter(cmd, "Status", DbType.String, 10); //output params from SP
                base.contextDb.AddOutParameter(cmd, "Message", DbType.String, -1); //output params from SP

                base.contextDb.ExecuteNonQuery(cmd);

                string status = base.contextDb.GetParameterValue(cmd, "Status").ToString();
                string message = base.contextDb.GetParameterValue(cmd, "Message").ToString();
                cs.Status = (status == "OK");
                cs.AddMessageDetail(message); //status and message is returned from the SP
            }
            catch (Exception ex)
            {
                EAMILogger.Instance.Error(ex);
                cs.Status = false;
                cs.IsFatal = true;
                cs.MessageDetailList.Add(ex.Message);
            }

            return cs;
        }

        /// <summary>
        /// Get list of all funds from the system...
        /// </summary>
        /// <returns></returns>
        public CommonStatusPayload<List<Fund>> GetAllFunds(bool includeInactive, long systemID)
        {
            CommonStatus cs = new CommonStatus(true);
            DataSet dataSet = new DataSet();
            List<Fund> lstFunds = new List<Fund>();

            try
            {
                Database db = new SqlDatabase(EAMIDBConnection.GetConnectionString());
                using (DbConnection dbConn = db.CreateConnection())
                {
                    dbConn.Open();

                    using (DataSet ds = db.ExecuteDataSet(DbStoredProcs.spGetFundList, DbStoredProcs.GetFundListParams(includeInactive, systemID)))
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            lstFunds = ds.Tables[0].AsEnumerable()
                                     .Select(dataRow => new Fund
                                     {
                                         Fund_ID = dataRow.Field<int>("Fund_ID"),
                                         Fund_Name = dataRow.Field<string>("Fund_Name"),
                                         Fund_Code = dataRow.Field<string>("Fund_Code"),
                                         Fund_Description = dataRow.Field<string>("Fund_Description"),
                                         Stat_Year = dataRow.Field<string>("Stat_Year"),
                                         System_ID = dataRow.Field<int>("System_ID"),
                                         //System_Code = dataRow.Field<string>("System_Code"),
                                         IsActive = dataRow.Field<bool>("IsActive"),
                                         CreatedBy = dataRow.Field<string>("CreatedBy"),
                                         CreateDate = dataRow.Field<DateTime>("CreateDate"),
                                         UpdatedBy = dataRow.Field<string>("UpdatedBy"),
                                         UpdateDate = dataRow.Field<DateTime?>("UpdateDate"),
                                     }).ToList();
                        }
                    }
                    cs.Status = true;
                }
            }
            catch (Exception ex)
            {
                EAMILogger.Instance.Error(ex);
                cs.Status = false;
                cs.IsFatal = true;
                cs.MessageDetailList.Add(ex.Message);
            }

            return new CommonStatusPayload<List<Fund>>(lstFunds, cs.Status, cs.IsFatal, cs.MessageDetailList);
        }

        /// <summary>
        /// Update the fund...
        /// </summary>
        /// <param name="inputFund"></param>
        /// <param name="loginUserName"></param>
        /// <returns></returns>
        public CommonStatus UpdateEAMIFund(Fund inputFund)
        {
            CommonStatus cs = new CommonStatus(false);

            try
            {
                DbCommand cmd = base.contextDb.GetStoredProcCommand("UpdateFund");

                base.contextDb.AddInParameter(cmd, "Fund_ID", DbType.Int64, inputFund.Fund_ID);
                base.contextDb.AddInParameter(cmd, "Fund_Name", DbType.String, inputFund.Fund_Name);
                base.contextDb.AddInParameter(cmd, "System_ID", DbType.String, inputFund.System_ID);
                base.contextDb.AddInParameter(cmd, "Stat_Year", DbType.String, inputFund.Stat_Year);
                base.contextDb.AddInParameter(cmd, "Fund_Description", DbType.String, inputFund.Fund_Description);
                base.contextDb.AddInParameter(cmd, "IsActive", DbType.Boolean, inputFund.IsActive);
                base.contextDb.AddInParameter(cmd, "UpdatedBy", DbType.String, inputFund.UpdatedBy);
                base.contextDb.AddOutParameter(cmd, "Status", DbType.String, 10); //output params from SP
                base.contextDb.AddOutParameter(cmd, "Message", DbType.String, -1); //output params from SP

                base.contextDb.ExecuteNonQuery(cmd);

                string status = base.contextDb.GetParameterValue(cmd, "Status").ToString();
                string message = base.contextDb.GetParameterValue(cmd, "Message").ToString();
                cs.Status = (status == "OK");
                cs.AddMessageDetail(message); //status and message is returned from the SP

            }
            catch (Exception ex)
            {
                EAMILogger.Instance.Error(ex);
                cs.Status = false;
                cs.IsFatal = true;
                cs.MessageDetailList.Add(ex.Message);
            }

            return cs;
        }

        #endregion

        #region Exclusive Payment Type...

        /// <summary>
        /// Add a new Fund in system...
        /// </summary>
        /// <param name="inputFund"></param>
        /// <param name="loginUserName"></param>
        /// <returns></returns>
        public CommonStatus AddEAMIExclusivePmtType(ExclusivePmtType inputExclusivePmtType)
        {
            CommonStatus cs = new CommonStatus(false);

            try
            {
                DbCommand cmd = base.contextDb.GetStoredProcCommand("AddExclusivePmtType");

                base.contextDb.AddInParameter(cmd, "Exclusive_Payment_Type_Code", DbType.String, inputExclusivePmtType.Exclusive_Payment_Type_Code);
                base.contextDb.AddInParameter(cmd, "Exclusive_Payment_Type_Description", DbType.String, inputExclusivePmtType.Exclusive_Payment_Type_Description);
                base.contextDb.AddInParameter(cmd, "Exclusive_Payment_Type_Name", DbType.String, inputExclusivePmtType.Exclusive_Payment_Type_Name);


                base.contextDb.AddInParameter(cmd, "Fund_ID", DbType.Int32, inputExclusivePmtType.Fund_ID);
                base.contextDb.AddInParameter(cmd, "System_ID", DbType.String, inputExclusivePmtType.System_ID);
                base.contextDb.AddInParameter(cmd, "IsActive", DbType.Boolean, inputExclusivePmtType.IsActive);
                base.contextDb.AddInParameter(cmd, "CreatedBy", DbType.String, inputExclusivePmtType.CreatedBy);
                base.contextDb.AddOutParameter(cmd, "Status", DbType.String, 10); //output params from SP
                base.contextDb.AddOutParameter(cmd, "Message", DbType.String, -1); //output params from SP

                base.contextDb.ExecuteNonQuery(cmd);

                string status = base.contextDb.GetParameterValue(cmd, "Status").ToString();
                string message = base.contextDb.GetParameterValue(cmd, "Message").ToString();
                cs.Status = (status == "OK");
                cs.AddMessageDetail(message); //status and message is returned from the SP
            }
            catch (Exception ex)
            {
                EAMILogger.Instance.Error(ex);
                cs.Status = false;
                cs.IsFatal = true;
                cs.MessageDetailList.Add(ex.Message);
            }

            return cs;
        }

        /// <summary>
        /// Get list of all Exclusive Payment Types from the system...
        /// </summary>
        /// <returns></returns>
        public CommonStatusPayload<List<ExclusivePmtType>> GetAllExclusivePmtTypes(bool includeInactive, long systemID)
        {
            CommonStatus cs = new CommonStatus(true);
            DataSet dataSet = new DataSet();
            List<ExclusivePmtType> lstExclusivePmtType = new List<ExclusivePmtType>();

            try
            {
                Database db = new SqlDatabase(EAMIDBConnection.GetConnectionString());
                using (DbConnection dbConn = db.CreateConnection())
                {
                    dbConn.Open();

                    using (DataSet ds = db.ExecuteDataSet(DbStoredProcs.spGetExclusivePaymentTypeList, DbStoredProcs.GetExclusivePmtTypeListParams(includeInactive, systemID)))
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            lstExclusivePmtType = ds.Tables[0].AsEnumerable()
                                     .Select(dataRow => new ExclusivePmtType
                                     {
                                         Exclusive_Payment_Type_ID = dataRow.Field<int>("Exclusive_Payment_Type_ID"),
                                         Exclusive_Payment_Type_Name = dataRow.Field<string>("Name"),
                                         Exclusive_Payment_Type_Code = dataRow.Field<string>("Code"),
                                         Exclusive_Payment_Type_Description = dataRow.Field<string>("Description"),
                                         Fund_ID = dataRow.Field<int>("Fund_ID"),
                                         Fund_Code = dataRow.Field<string>("Fund_Code"),
                                         System_ID = dataRow.Field<int>("System_ID"),
                                         IsActive = dataRow.Field<bool>("IsActive"),
                                         CreatedBy = dataRow.Field<string>("CreatedBy"),
                                         CreateDate = dataRow.Field<DateTime>("CreateDate"),
                                         UpdatedBy = dataRow.Field<string>("UpdatedBy"),
                                         UpdateDate = dataRow.Field<DateTime?>("UpdateDate"),
                                         DeactivatedDate = dataRow.Field<DateTime?>("DeactivatedDate"),
                                         DeactivatedBy = dataRow.Field<string>("DeactivatedBy"),
                                     }).ToList();
                        }
                    }
                    cs.Status = true;
                }
            }
            catch (Exception ex)
            {
                EAMILogger.Instance.Error(ex);
                cs.Status = false;
                cs.IsFatal = true;
                cs.MessageDetailList.Add(ex.Message);
            }

            return new CommonStatusPayload<List<ExclusivePmtType>>(lstExclusivePmtType, cs.Status, cs.IsFatal, cs.MessageDetailList);
        }

        /// <summary>
        /// Update the fund...
        /// </summary>
        /// <param name="inputFund"></param>
        /// <param name="loginUserName"></param>
        /// <returns></returns>
        public CommonStatus UpdateEAMIExclusivePmtType(ExclusivePmtType inputExclusivePmtType)
        {
            CommonStatus cs = new CommonStatus(false);

            try
            {
                DbCommand cmd = base.contextDb.GetStoredProcCommand("UpdateExclusivePmtType");

                base.contextDb.AddInParameter(cmd, "Exclusive_Payment_Type_ID", DbType.Int64, inputExclusivePmtType.Exclusive_Payment_Type_ID);
                base.contextDb.AddInParameter(cmd, "Exclusive_Payment_Type_Name", DbType.String, inputExclusivePmtType.Exclusive_Payment_Type_Name);
                base.contextDb.AddInParameter(cmd, "Exclusive_Payment_Type_Description", DbType.String, inputExclusivePmtType.Exclusive_Payment_Type_Description);
                base.contextDb.AddInParameter(cmd, "System_ID", DbType.String, inputExclusivePmtType.System_ID);
                //base.contextDb.AddInParameter(cmd, "Fund_ID", DbType.Int64, inputExclusivePmtType.Fund_ID);
                base.contextDb.AddInParameter(cmd, "IsActive", DbType.Boolean, inputExclusivePmtType.IsActive);
                base.contextDb.AddInParameter(cmd, "UpdatedBy", DbType.String, inputExclusivePmtType.UpdatedBy);
                base.contextDb.AddOutParameter(cmd, "Status", DbType.String, 10); //output params from SP
                base.contextDb.AddOutParameter(cmd, "Message", DbType.String, -1); //output params from SP

                base.contextDb.ExecuteNonQuery(cmd);

                string status = base.contextDb.GetParameterValue(cmd, "Status").ToString();
                string message = base.contextDb.GetParameterValue(cmd, "Message").ToString();
                cs.Status = (status == "OK");
                cs.AddMessageDetail(message); //status and message is returned from the SP

            }
            catch (Exception ex)
            {
                EAMILogger.Instance.Error(ex);
                cs.Status = false;
                cs.IsFatal = true;
                cs.MessageDetailList.Add(ex.Message);
            }

            return cs;
        }

        ///// <summary>
        ///// Delete a fund. This is a soft delete...
        ///// </summary>
        ///// <param name="inputFund"></param>
        ///// <param name="loginUserName"></param>
        ///// <returns></returns>
        //public CommonStatus DeleteEAMIExclusivePmtType(ExclusivePmtType inputExclusivePmtType)
        //{
        //    CommonStatus cs = new CommonStatus(false);

        //    try
        //    {
        //        DbCommand cmd = base.contextDb.GetStoredProcCommand("DeactivateEAMIExclusivePmtType");

        //        base.contextDb.AddInParameter(cmd, "Exclusive_Payment_Type_Code", DbType.Int64, inputExclusivePmtType.Exclusive_Payment_Type_Code);
        //        base.contextDb.AddInParameter(cmd, "Exclusive_Payment_Type_Description", DbType.String, inputExclusivePmtType.Exclusive_Payment_Type_Description);
        //        base.contextDb.AddInParameter(cmd, "Exclusive_Payment_Type_Name", DbType.String, inputExclusivePmtType.Exclusive_Payment_Type_Name);
        //        base.contextDb.AddInParameter(cmd, "IsActive", DbType.Boolean, inputExclusivePmtType.IsActive);
        //        base.contextDb.AddInParameter(cmd, "UpdatedBy", DbType.String, inputExclusivePmtType.UpdatedBy);

        //        base.contextDb.ExecuteNonQuery(cmd);

        //        string status = base.contextDb.GetParameterValue(cmd, "Status").ToString();
        //        string message = base.contextDb.GetParameterValue(cmd, "Message").ToString();

        //        cs.Status = (status == "OK");

        //        if (!cs.Status)
        //        {
        //            cs.AddMessageDetail(message);
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        EAMILogger.Instance.Error(ex);
        //        cs.Status = false;
        //        cs.IsFatal = true;
        //        cs.MessageDetailList.Add(ex.Message);
        //    }

        //    return cs;
        //}

        #endregion

        #region Facesheet Values...

        /// <summary>
        /// Add a new FS in system...
        /// </summary>
        /// <param name="inputFS"></param>
        /// <returns></returns>
        public CommonStatus AddFacesheetValues(FacesheetValues inputFS)
        {
            CommonStatus cs = new CommonStatus(false);

            try
            {
                DbCommand cmd = base.contextDb.GetStoredProcCommand("AddFacesheetValues");

                base.contextDb.AddInParameter(cmd, "Fund_ID", DbType.Int32, inputFS.Fund_ID);
                base.contextDb.AddInParameter(cmd, "System_ID", DbType.Int32, inputFS.Fund_ID);
                base.contextDb.AddInParameter(cmd, "Chapter", DbType.String, inputFS.Chapter);
                base.contextDb.AddInParameter(cmd, "Reference_Item", DbType.String, inputFS.Reference_Item);
                base.contextDb.AddInParameter(cmd, "Program", DbType.String, inputFS.Program);
                base.contextDb.AddInParameter(cmd, "Element", DbType.String, inputFS.Element);
                base.contextDb.AddInParameter(cmd, "Description", DbType.String, inputFS.Description);
                base.contextDb.AddInParameter(cmd, "IsActive", DbType.Boolean, inputFS.IsActive);
                base.contextDb.AddInParameter(cmd, "CreatedBy", DbType.String, inputFS.CreatedBy);
                base.contextDb.AddOutParameter(cmd, "Status", DbType.String, 10); //output params from SP
                base.contextDb.AddOutParameter(cmd, "Message", DbType.String, -1); //output params from SP

                base.contextDb.ExecuteNonQuery(cmd);

                string status = base.contextDb.GetParameterValue(cmd, "Status").ToString();
                string message = base.contextDb.GetParameterValue(cmd, "Message").ToString();
                cs.Status = (status == "OK");
                cs.AddMessageDetail(message); //status and message is returned from the SP
            }
            catch (Exception ex)
            {
                EAMILogger.Instance.Error(ex);
                cs.Status = false;
                cs.IsFatal = true;
                cs.MessageDetailList.Add(ex.Message);
            }

            return cs;
        }

        /// <summary>
        /// Get list of all FAcesheet values from the system...
        /// </summary>
        /// <returns></returns>
        public CommonStatusPayload<List<FacesheetValues>> GetAllFacesheetValues(bool includeInactive, long systemID)
        {
            CommonStatus cs = new CommonStatus(true);
            DataSet dataSet = new DataSet();
            List<FacesheetValues> lstFunds = new List<FacesheetValues>();

            try
            {
                Database db = new SqlDatabase(EAMIDBConnection.GetConnectionString());
                using (DbConnection dbConn = db.CreateConnection())
                {
                    dbConn.Open();

                    using (DataSet ds = db.ExecuteDataSet(DbStoredProcs.spGetFacesheetList, DbStoredProcs.GetFacesheetListParams(includeInactive, systemID)))
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            lstFunds = ds.Tables[0].AsEnumerable()
                                     .Select(dataRow => new FacesheetValues
                                     {
                                         Facesheet_ID = dataRow.Field<int>("Facesheet_ID"),
                                         Fund_ID = dataRow.Field<int>("Fund_ID"),
                                         Fund_Code = dataRow.Field<string>("Fund_Code"),
                                         Fund_Name = dataRow.Field<string>("Fund_Name"),
                                         Chapter = dataRow.Field<string>("Chapter"),
                                         Agency_Number = dataRow.Field<string>("Agency_Number"),
                                         Agency_Name = dataRow.Field<string>("Agency_Name"),
                                         Reference_Item = dataRow.Field<string>("Reference_Item"),
                                         Program = dataRow.Field<string>("Program"),
                                         Fiscal_Year = dataRow.Field<string>("Fiscal_Year"),
                                         Stat_Year = dataRow.Field<string>("Stat_Year"),
                                         Element = dataRow.Field<string>("Element"),
                                         IsActive = dataRow.Field<bool>("IsActive"),
                                         Description = dataRow.Field<string>("Description"),
                                         CreatedBy = dataRow.Field<string>("CreatedBy"),
                                         CreateDate = dataRow.Field<DateTime>("CreateDate"),
                                         UpdatedBy = dataRow.Field<string>("UpdatedBy"),
                                         UpdateDate = dataRow.Field<DateTime?>("UpdateDate"),
                                     }).ToList();
                        }
                    }
                    cs.Status = true;
                }
            }
            catch (Exception ex)
            {
                EAMILogger.Instance.Error(ex);
                cs.Status = false;
                cs.IsFatal = true;
                cs.MessageDetailList.Add(ex.Message);
            }

            return new CommonStatusPayload<List<FacesheetValues>>(lstFunds, cs.Status, cs.IsFatal, cs.MessageDetailList);
        }

        /// <summary>
        /// Update the Facesheet Values...
        /// </summary>
        /// <param name="inputFS"></param>
        /// <returns></returns>
        public CommonStatus UpdatFacesheetValues(FacesheetValues inputFS)
        {
            CommonStatus cs = new CommonStatus(false);

            try
            {
                DbCommand cmd = base.contextDb.GetStoredProcCommand("UpdateFacesheetValues");

                base.contextDb.AddInParameter(cmd, "Facesheet_ID", DbType.Int32, inputFS.Facesheet_ID);
                base.contextDb.AddInParameter(cmd, "Fund_ID", DbType.Int32, inputFS.Fund_ID);
                //base.contextDb.AddInParameter(cmd, "Fiscal_Year", DbType.Int32, inputFS.Fiscal_Year);
                base.contextDb.AddInParameter(cmd, "Chapter", DbType.String, inputFS.Chapter);
                base.contextDb.AddInParameter(cmd, "Reference_Item", DbType.String, inputFS.Reference_Item);
                base.contextDb.AddInParameter(cmd, "Program", DbType.String, inputFS.Program);
                base.contextDb.AddInParameter(cmd, "Element", DbType.String, inputFS.Element);
                base.contextDb.AddInParameter(cmd, "Description", DbType.String, inputFS.Description);
                base.contextDb.AddInParameter(cmd, "IsActive", DbType.Boolean, inputFS.IsActive);
                base.contextDb.AddInParameter(cmd, "UpdatedBy", DbType.String, inputFS.UpdatedBy);
                base.contextDb.AddOutParameter(cmd, "Status", DbType.String, 10); //output params from SP
                base.contextDb.AddOutParameter(cmd, "Message", DbType.String, -1); //output params from SP

                base.contextDb.ExecuteNonQuery(cmd);

                string status = base.contextDb.GetParameterValue(cmd, "Status").ToString();
                string message = base.contextDb.GetParameterValue(cmd, "Message").ToString();
                cs.Status = (status == "OK");
                cs.AddMessageDetail(message); //status and message is returned from the SP

            }
            catch (Exception ex)
            {
                EAMILogger.Instance.Error(ex);
                cs.Status = false;
                cs.IsFatal = true;
                cs.MessageDetailList.Add(ex.Message);
            }

            return cs;
        }

        #endregion

        #region Funding Source...

        /// <summary>
        /// Add a new Funding Source in system...
        /// </summary>
        /// <param name="inputFundingSource"></param>       
        /// <returns></returns>
        public CommonStatus AddEAMIFundingSource(FundingSource inputFundingSource)
        {
            CommonStatus cs = new CommonStatus(false);

            try
            {
                DbCommand cmd = base.contextDb.GetStoredProcCommand("AddFundingSource");

                base.contextDb.AddInParameter(cmd, "Code", DbType.String, inputFundingSource.Code);
                //base.contextDb.AddInParameter(cmd, "Title", DbType.String, inputFundingSource.Title);
                base.contextDb.AddInParameter(cmd, "System_ID", DbType.String, inputFundingSource.System_ID);
                base.contextDb.AddInParameter(cmd, "Description", DbType.String, inputFundingSource.Description);
                base.contextDb.AddInParameter(cmd, "IsActive", DbType.Boolean, inputFundingSource.IsActive);
                base.contextDb.AddInParameter(cmd, "CreatedBy", DbType.String, inputFundingSource.CreatedBy);
                base.contextDb.AddOutParameter(cmd, "Status", DbType.String, 10); //output params from SP
                base.contextDb.AddOutParameter(cmd, "Message", DbType.String, -1); //output params from SP

                base.contextDb.ExecuteNonQuery(cmd);

                string status = base.contextDb.GetParameterValue(cmd, "Status").ToString();
                string message = base.contextDb.GetParameterValue(cmd, "Message").ToString();
                cs.Status = (status == "OK");
                cs.AddMessageDetail(message); //status and message is returned from the SP
            }
            catch (Exception ex)
            {
                EAMILogger.Instance.Error(ex);
                cs.Status = false;
                cs.IsFatal = true;
                cs.MessageDetailList.Add(ex.Message);
            }

            return cs;
        }

        /// <summary>
        /// Get list of all funding sources from the system...
        /// </summary>
        /// <returns></returns>
        public CommonStatusPayload<List<FundingSource>> GetAllFundingSources(bool includeInactive, long systemID)
        {
            CommonStatus cs = new CommonStatus(true);
            DataSet dataSet = new DataSet();
            List<FundingSource> lstFundingSource = new List<FundingSource>();

            try
            {
                Database db = new SqlDatabase(EAMIDBConnection.GetConnectionString());
                using (DbConnection dbConn = db.CreateConnection())
                {
                    dbConn.Open();

                    using (DataSet ds = db.ExecuteDataSet(DbStoredProcs.spGetFundingSourceList, DbStoredProcs.GetFundingSourceListParams(includeInactive, systemID)))
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            lstFundingSource = ds.Tables[0].AsEnumerable()
                                     .Select(dataRow => new FundingSource
                                     {
                                         Funding_Source_ID = dataRow.Field<int>("Funding_Source_ID"),
                                         //Name = dataRow.Field<string>("Name"),
                                         Code = dataRow.Field<string>("Code"),
                                         Description = dataRow.Field<string>("Description"),
                                         System_ID = dataRow.Field<int>("System_ID"),
                                         //System_Code = dataRow.Field<string>("System_Code"),
                                         IsActive = dataRow.Field<bool>("IsActive"),
                                         CreatedBy = dataRow.Field<string>("CreatedBy"),
                                         CreateDate = dataRow.Field<DateTime>("CreateDate"),
                                         UpdatedBy = dataRow.Field<string>("UpdatedBy"),
                                         UpdateDate = dataRow.Field<DateTime?>("UpdateDate"),
                                         DeactivatedDate = dataRow.Field<DateTime?>("DeactivatedDate"),
                                         DeactivatedBy = dataRow.Field<string>("DeactivatedBy"),
                                     }).ToList();
                        }
                    }
                    cs.Status = true;
                }
            }
            catch (Exception ex)
            {
                EAMILogger.Instance.Error(ex);
                cs.Status = false;
                cs.IsFatal = true;
                cs.MessageDetailList.Add(ex.Message);
            }

            return new CommonStatusPayload<List<FundingSource>>(lstFundingSource, cs.Status, cs.IsFatal, cs.MessageDetailList);
        }

        /// <summary>
        /// Update the funding source...
        /// </summary>
        /// <param name="inputFundingSource"></param>        
        /// <returns></returns>
        public CommonStatus UpdateEAMIFundingSource(FundingSource inputFundingSource)
        {
            CommonStatus cs = new CommonStatus(false);

            try
            {
                DbCommand cmd = base.contextDb.GetStoredProcCommand("UpdateFundingSource");

                base.contextDb.AddInParameter(cmd, "Funding_Source_ID", DbType.Int64, inputFundingSource.Funding_Source_ID);
                base.contextDb.AddInParameter(cmd, "Code", DbType.String, inputFundingSource.Code);
                base.contextDb.AddInParameter(cmd, "Description", DbType.String, inputFundingSource.Description);
                base.contextDb.AddInParameter(cmd, "System_ID", DbType.String, inputFundingSource.System_ID);
                //base.contextDb.AddInParameter(cmd, "Title", DbType.String, inputFundingSource.Title);
                base.contextDb.AddInParameter(cmd, "IsActive", DbType.Boolean, inputFundingSource.IsActive);
                base.contextDb.AddInParameter(cmd, "UpdatedBy", DbType.String, inputFundingSource.UpdatedBy);
                base.contextDb.AddOutParameter(cmd, "Status", DbType.String, 10); //output params from SP
                base.contextDb.AddOutParameter(cmd, "Message", DbType.String, -1); //output params from SP

                base.contextDb.ExecuteNonQuery(cmd);

                string status = base.contextDb.GetParameterValue(cmd, "Status").ToString();
                string message = base.contextDb.GetParameterValue(cmd, "Message").ToString();
                cs.Status = (status == "OK");
                cs.AddMessageDetail(message); //status and message is returned from the SP

            }
            catch (Exception ex)
            {
                EAMILogger.Instance.Error(ex);
                cs.Status = false;
                cs.IsFatal = true;
                cs.MessageDetailList.Add(ex.Message);
            }

            return cs;
        }

        /// <summary>
        /// Delete a funding Source. This is a soft delete...
        /// </summary>
        /// <param name="inputFundingSource"></param>        
        /// <returns></returns>
        //public CommonStatus DeleteEAMIFundingSource(FundingSource inputFundingSource)
        //{
        //    CommonStatus cs = new CommonStatus(false);

        //    try
        //    {
        //        DbCommand cmd = base.contextDb.GetStoredProcCommand("DeleteEAMIFundingSource");

        //        base.contextDb.AddInParameter(cmd, "Code", DbType.Int64, inputFundingSource.Code);
        //        base.contextDb.AddInParameter(cmd, "Description", DbType.String, inputFundingSource.Description);
        //        //base.contextDb.AddInParameter(cmd, "Name", DbType.String, inputFundingSource.Name);
        //        base.contextDb.AddInParameter(cmd, "IsActive", DbType.Boolean, inputFundingSource.IsActive);
        //        base.contextDb.AddInParameter(cmd, "UpdatedBy", DbType.String, inputFundingSource.UpdatedBy);

        //        base.contextDb.ExecuteNonQuery(cmd);

        //        string status = base.contextDb.GetParameterValue(cmd, "Status").ToString();
        //        string message = base.contextDb.GetParameterValue(cmd, "Message").ToString();

        //        cs.Status = (status == "OK");

        //        if (!cs.Status)
        //        {
        //            cs.AddMessageDetail(message);
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        EAMILogger.Instance.Error(ex);
        //        cs.Status = false;
        //        cs.IsFatal = true;
        //        cs.MessageDetailList.Add(ex.Message);
        //    }

        //    return cs;
        //}

        #endregion

        #region SCO Properties...

        /// <summary>
        /// Add a new SCO Properties in system...
        /// </summary>
        /// <param name="inputSCOProperties"></param>
        /// <param name="loginUserName"></param>
        /// <returns></returns>
        public CommonStatus AddSCOProperties(SCOProperty inputSCOProperties)
        {
            CommonStatus cs = new CommonStatus(false);

            try
            {
                DbCommand cmd = base.contextDb.GetStoredProcCommand("AddSCOProperty");

                base.contextDb.AddInParameter(cmd, "SCO_Property_Name", DbType.String, inputSCOProperties.SCO_Property_Name);
                base.contextDb.AddInParameter(cmd, "SCO_Property_Value", DbType.String, inputSCOProperties.SCO_Property_Value);
                base.contextDb.AddInParameter(cmd, "SCO_Property_Type_ID", DbType.Int32, inputSCOProperties.SCO_Property_Type_ID);
                base.contextDb.AddInParameter(cmd, "Description", DbType.String, inputSCOProperties.Description);
                base.contextDb.AddInParameter(cmd, "Fund_ID", DbType.Int32, inputSCOProperties.Fund_ID);
                base.contextDb.AddInParameter(cmd, "System_ID", DbType.String, inputSCOProperties.System_ID);
                base.contextDb.AddInParameter(cmd, "SCO_Property_Enum_ID", DbType.String, inputSCOProperties.SCO_Property_Enum_ID);
                base.contextDb.AddInParameter(cmd, "Payment_Type", DbType.String, inputSCOProperties.PaymentTypeText);
                base.contextDb.AddInParameter(cmd, "Environment", DbType.String, inputSCOProperties.EnvironmentText);
                //base.contextDb.AddInParameter(cmd, "Stat_Year", DbType.String, inputSCOProperties.Stat_Year);
                base.contextDb.AddInParameter(cmd, "IsActive", DbType.Boolean, inputSCOProperties.IsActive);
                base.contextDb.AddInParameter(cmd, "CreatedBy", DbType.String, inputSCOProperties.CreatedBy);
                base.contextDb.AddOutParameter(cmd, "Status", DbType.String, 10); //output params from SP
                base.contextDb.AddOutParameter(cmd, "Message", DbType.String, -1); //output params from SP

                base.contextDb.ExecuteNonQuery(cmd);

                string status = base.contextDb.GetParameterValue(cmd, "Status").ToString();
                string message = base.contextDb.GetParameterValue(cmd, "Message").ToString();
                cs.Status = (status == "OK");
                cs.AddMessageDetail(message); //status and message is returned from the SP
            }
            catch (Exception ex)
            {
                EAMILogger.Instance.Error(ex);
                cs.Status = false;
                cs.IsFatal = true;
                cs.MessageDetailList.Add(ex.Message);
            }

            return cs;
        }

        /// <summary>
        /// Get list of all SCO Properties from the system...
        /// </summary>
        /// <returns></returns>
        public CommonStatusPayload<List<SCOProperty>> GetAllSCOProperties(bool includeInactive, long systemID)
        {
            CommonStatus cs = new CommonStatus(true);
            DataSet dataSet = new DataSet();
            List<SCOProperty> lstSCOProperties = new List<SCOProperty>();

            try
            {
                Database db = new SqlDatabase(EAMIDBConnection.GetConnectionString());
                using (DbConnection dbConn = db.CreateConnection())
                {
                    dbConn.Open();

                    using (DataSet ds = db.ExecuteDataSet(DbStoredProcs.spGetSCOPropertiesList, DbStoredProcs.GetSCOPropertiesListParams(includeInactive, systemID)))
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            lstSCOProperties = ds.Tables[0].AsEnumerable()
                                     .Select(dataRow => new SCOProperty
                                     {
                                         SCO_Property_ID = dataRow.Field<int>("ID"),
                                         SCO_Property_Name = (dataRow.Field<int>("Property_Type_ID") == 3)
                                                            ? (dataRow.Field<string>("SCO_Property_Name") + "_FUND_" + dataRow.Field<string>("Fund_Code") + "_" + dataRow.Field<string>("Payment_Type") + "_" + dataRow.Field<string>("Environment")) : dataRow.Field<string>("SCO_Property_Name"),
                                         SCO_Property_Value = dataRow.Field<string>("SCO_Property_Value"),
                                         Description = dataRow.Field<string>("Description"),
                                         Fund_ID = DBNull.Value.Equals(dataRow.Field<int?>("Fund_ID")) ? 0 : dataRow.Field<int?>("Fund_ID"),
                                         Fund_Code = DBNull.Value.Equals(dataRow.Field<string>("Fund_Code")) ? string.Empty : dataRow.Field<string>("Fund_Code"),
                                         System_ID = dataRow.Field<int>("System_ID"),
                                         SCO_Property_Type_ID = dataRow.Field<int>("Property_Type_ID"),
                                         PaymentTypeText = DBNull.Value.Equals(dataRow.Field<string>("Payment_Type")) ? string.Empty : dataRow.Field<string>("Payment_Type"),
                                         EnvironmentText = DBNull.Value.Equals(dataRow.Field<string>("Environment")) ? string.Empty : dataRow.Field<string>("Environment"),
                                         SCO_Property_Enum_ID = dataRow.Field<int>("SCO_Property_Enum_ID"),
                                         IsActive = dataRow.Field<bool>("IsActive"),
                                         CreatedBy = dataRow.Field<string>("CreatedBy"),
                                         CreateDate = dataRow.Field<DateTime>("CreateDate"),
                                         UpdatedBy = dataRow.Field<string>("UpdatedBy"),
                                         UpdateDate = dataRow.Field<DateTime?>("UpdateDate")
                                     }).ToList();
                        }
                    }
                    cs.Status = true;
                }
            }
            catch (Exception ex)
            {
                EAMILogger.Instance.Error(ex);
                cs.Status = false;
                cs.IsFatal = true;
                cs.MessageDetailList.Add(ex.Message);
            }

            return new CommonStatusPayload<List<SCOProperty>>(lstSCOProperties, cs.Status, cs.IsFatal, cs.MessageDetailList);
        }


        /// <summary>
        /// Update the SCO Properties for a given PropertyTypeID and PropertyID...
        /// </summary>
        /// <param name="scoPropertiesToBeUpdated"></param>
        /// <param name="loginUserName"></param>
        /// <returns></returns>
        public CommonStatus UpdateSCOProperties(SCOProperty scoPropertiesToBeUpdated)
        {
            CommonStatus cs = new CommonStatus(false);

            try
            {
                DbCommand cmd = base.contextDb.GetStoredProcCommand("UpdateScoProperty");

                base.contextDb.AddInParameter(cmd, "System_ID", DbType.String, scoPropertiesToBeUpdated.System_ID);
                base.contextDb.AddInParameter(cmd, "SCO_Property_ID", DbType.Int64, scoPropertiesToBeUpdated.SCO_Property_ID);
                base.contextDb.AddInParameter(cmd, "SCO_Property_Type_ID", DbType.Int64, scoPropertiesToBeUpdated.SCO_Property_Type_ID);
                base.contextDb.AddInParameter(cmd, "SCO_Property_Enum_ID", DbType.Int64, scoPropertiesToBeUpdated.SCO_Property_Enum_ID);
                base.contextDb.AddInParameter(cmd, "SCO_Property_Name", DbType.String, scoPropertiesToBeUpdated.SCO_Property_Name);
                base.contextDb.AddInParameter(cmd, "Fund_ID", DbType.Int64, scoPropertiesToBeUpdated.Fund_ID);               
                base.contextDb.AddInParameter(cmd, "SCO_Property_Value", DbType.String, scoPropertiesToBeUpdated.SCO_Property_Value);
                base.contextDb.AddInParameter(cmd, "Description", DbType.String, scoPropertiesToBeUpdated.Description);
                base.contextDb.AddInParameter(cmd, "EnvironmentText", DbType.String, scoPropertiesToBeUpdated.EnvironmentText);
                base.contextDb.AddInParameter(cmd, "PaymentTypeText", DbType.String, scoPropertiesToBeUpdated.PaymentTypeText);
                base.contextDb.AddInParameter(cmd, "IsActive", DbType.Boolean, scoPropertiesToBeUpdated.IsActive);
                base.contextDb.AddInParameter(cmd, "UpdatedBy", DbType.String, scoPropertiesToBeUpdated.UpdatedBy);
                base.contextDb.AddOutParameter(cmd, "Status", DbType.String, 10); //output params from SP
                base.contextDb.AddOutParameter(cmd, "Message", DbType.String, -1); //output params from SP

                base.contextDb.ExecuteNonQuery(cmd);

                string status = base.contextDb.GetParameterValue(cmd, "Status").ToString();
                string message = base.contextDb.GetParameterValue(cmd, "Message").ToString();
                cs.Status = (status == "OK");
                cs.AddMessageDetail(message); //status and message is returned from the SP

            }
            catch (Exception ex)
            {
                EAMILogger.Instance.Error(ex);
                cs.Status = false;
                cs.IsFatal = true;
                cs.MessageDetailList.Add(ex.Message);
            }

            return cs;
        }

        public CommonStatusPayload<SCOProperty> GetAllSCOPropertyTypes()
        {
            CommonStatus cs = new CommonStatus(true);
            DataSet dataSet = new DataSet();
            SCOProperty scoProperies = new SCOProperty();

            //SCOPropertiesLookUp lstSCOPropertiesLookUp = new SCOPropertiesLookUp();
            List<SCOPropertyEnumsLookup> lstSCOPropertyEnumsLookUp = new List<SCOPropertyEnumsLookup>();
            List<SCOPropertyTypeLookUp> lstSCOPropertyTypeLookUp = new List<SCOPropertyTypeLookUp>();

            CommonStatusPayload<SCOProperty> commonStatus = new CommonStatusPayload<SCOProperty>(scoProperies, true);

            try
            {
                // dbConn instance scope
                Database db = new SqlDatabase(EAMIDBConnection.GetConnectionString());
                using (DbConnection dbConn = db.CreateConnection())
                {
                    dbConn.Open();
                    using (DataSet ds = db.ExecuteDataSet(DbStoredProcs.spGetSCOPropertiesLookUp, DbStoredProcs.GetSCOPropertiesLookUp()))
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            lstSCOPropertyTypeLookUp = ds.Tables[0].AsEnumerable()
                                     .Select(dataRow => new SCOPropertyTypeLookUp
                                     {
                                         SCO_Property_Type_ID = dataRow.Field<int>("SCO_Property_Type_ID"),
                                         Code = dataRow.Field<string>("Code"),
                                         Description = dataRow.Field<string>("Description"),
                                         IsActive = dataRow.Field<bool>("IsActive"),
                                         CreatedBy = dataRow.Field<string>("CreatedBy"),
                                         CreateDate = dataRow.Field<DateTime>("CreateDate"),
                                         UpdatedBy = dataRow.Field<string>("UpdatedBy"),
                                         UpdateDate = dataRow.Field<DateTime?>("UpdateDate")
                                     }).ToList();
                        }

                        if (ds.Tables[1].Rows.Count > 0)
                        {
                            lstSCOPropertyEnumsLookUp = ds.Tables[1].AsEnumerable()
                                     .Select(dataRow => new SCOPropertyEnumsLookup
                                     {
                                         SCO_Property_Enum_ID = dataRow.Field<int>("SCO_Property_Enum_ID"),
                                         SCO_Property_Type_ID = dataRow.Field<int>("SCO_Property_Type_ID"),
                                         Code = dataRow.Field<string>("Code"),
                                         Description = dataRow.Field<string>("Description"),
                                         IsActive = dataRow.Field<bool>("IsActive"),
                                         CreatedBy = dataRow.Field<string>("CreatedBy"),
                                         CreateDate = dataRow.Field<DateTime>("CreateDate"),
                                         UpdatedBy = dataRow.Field<string>("UpdatedBy"),
                                         UpdateDate = dataRow.Field<DateTime?>("UpdateDate")
                                     }).ToList();
                        }
                    }
                    scoProperies.lstSCOPropertyEnumsLookUp.AddRange(lstSCOPropertyEnumsLookUp);
                    scoProperies.lstSCOPropertyTypeLookUp.AddRange(lstSCOPropertyTypeLookUp);
                    cs.Status = true;
                }
            }
            catch (Exception ex)
            {
                EAMILogger.Instance.Error(ex);
                commonStatus.Status = false;
                commonStatus.IsFatal = true;
                commonStatus.AddMessageDetail(ex.Message);
            }

            return new CommonStatusPayload<SCOProperty>(scoProperies, cs.Status, cs.IsFatal, cs.MessageDetailList);
        }

        #endregion

        #region Private Methods...

        #endregion
    }
}
