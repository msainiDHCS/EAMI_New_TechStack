using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OHC.EAMI.CommonEntity;
using OHC.EAMI.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System.Data.Common;
using System.Data;
using OHC.EAMI.DataAccess.Shared;

namespace OHC.EAMI.DataAccess
{
    public static class ReportDataDbMgr
    {
        #region Public Methods

        public static CommonStatusPayload<DataSet> GetFaceSheetDataByECSID(int ecsID, int systemID, bool isProdEnv)
        {            
            Database db = new SqlDatabase(EAMIDBConnection.GetConnectionString());
            DataSet dataSet = new DataSet();
            CommonStatus status = new CommonStatus(true);

            try
            {
                // dbConn instance scope
                using (DbConnection dbConn = db.CreateConnection())
                {
                    dbConn.Open();

                    using (DataSet ds = db.ExecuteDataSet(DbStoredProcs.spGetFaceSheetDataByECSID, DbStoredProcs.GetSPGetFaceSheetDataByECSIDParams(ecsID, systemID,isProdEnv)))
                    {
                        dataSet = ds;
                    }
                }
            }
            catch (Exception ex)
            {
                status.Status = false;
                status.IsFatal = true;
                status.AddMessageDetail(ex.Message);
                EAMILogger.Instance.Error(ex);
            }

            return new CommonStatusPayload<DataSet>(dataSet, status.Status, status.IsFatal, status.GetCombinedMessage());
        }

        public static CommonStatusPayload<DataSet> GetTransferLetterDataByECSID(int ecsID, string userName)
        {
            Database db = new SqlDatabase(EAMIDBConnection.GetConnectionString());
            DataSet dataSet = new DataSet();
            CommonStatus status = new CommonStatus(true);

            try
            {
                // dbConn instance scope
                using (DbConnection dbConn = db.CreateConnection())
                {
                    dbConn.Open();

                    using (DataSet ds = db.ExecuteDataSet(DbStoredProcs.spGetTransferLetterDataByECSID, DbStoredProcs.GetSPGetTransferLetterDataByECSIDParams(ecsID, userName)))
                    {
                        dataSet = ds;
                    }
                }
            }
            catch (Exception ex)
            {
                status.Status = false;
                status.IsFatal = true;
                status.AddMessageDetail(ex.Message);
                EAMILogger.Instance.Error(ex);
            }

            return new CommonStatusPayload<DataSet>(dataSet, status.Status, status.IsFatal, status.GetCombinedMessage());
        }

        public static CommonStatusPayload<DataSet> GetDrawSummaryReportData(DateTime payDate)
        {
            Database db = new SqlDatabase(EAMIDBConnection.GetConnectionString());
            DataSet dataSet = new DataSet();
            CommonStatus status = new CommonStatus(true);

            try
            {
                // dbConn instance scope
                using (DbConnection dbConn = db.CreateConnection())
                {
                    dbConn.Open();

                    using (DataSet ds = db.ExecuteDataSet(DbStoredProcs.spGetDrawSummaryReportData, DbStoredProcs.GeSPGetDrawSummaryReportDataParams(payDate)))
                    {
                        dataSet = ds;
                    }
                }
            }
            catch (Exception ex)
            {
                status.Status = false;
                status.IsFatal = true;
                status.AddMessageDetail(ex.Message);
                EAMILogger.Instance.Error(ex);
            }

            return new CommonStatusPayload<DataSet>(dataSet, status.Status, status.IsFatal, status.GetCombinedMessage());
        }

        public static CommonStatusPayload<DataSet> GetHoldReportData()
        {
            Database db = new SqlDatabase(EAMIDBConnection.GetConnectionString());
            DataSet dataSet = new DataSet();
            CommonStatus status = new CommonStatus(true);

            try
            {
                // dbConn instance scope
                using (DbConnection dbConn = db.CreateConnection())
                {
                    dbConn.Open();

                    using (DataSet ds = db.ExecuteDataSet(DbStoredProcs.spGetHoldReportData, DbStoredProcs.GetSPGetHoldReportDataParams()))
                    {
                        dataSet = ds;
                    }
                }
            }
            catch (Exception ex)
            {
                status.Status = false;
                status.IsFatal = true;
                status.AddMessageDetail(ex.Message);
                EAMILogger.Instance.Error(ex);
            }

            return new CommonStatusPayload<DataSet>(dataSet, status.Status, status.IsFatal, status.GetCombinedMessage());
        }

        public static CommonStatusPayload<DataSet> GetReturnToSORReportData(DateTime dateFrom, DateTime dateTo)
        {
            Database db = new SqlDatabase(EAMIDBConnection.GetConnectionString());
            DataSet dataSet = new DataSet();
            CommonStatus status = new CommonStatus(true);

            try
            {
                // dbConn instance scope
                using (DbConnection dbConn = db.CreateConnection())
                {
                    dbConn.Open();

                    using (DataSet ds = db.ExecuteDataSet(DbStoredProcs.spGetReturnToSORReportData, DbStoredProcs.GetSPGetReturnToSORReportDataParams(dateFrom, dateTo)))
                    {
                        dataSet = ds;
                    }
                }
            }
            catch (Exception ex)
            {
                status.Status = false;
                status.IsFatal = true;
                status.AddMessageDetail(ex.Message);
                EAMILogger.Instance.Error(ex);
            }

            return new CommonStatusPayload<DataSet>(dataSet, status.Status, status.IsFatal, status.GetCombinedMessage());
        }

        public static CommonStatusPayload<DataSet> GetECSReportData(DateTime dateFrom, DateTime dateTo)
        {
            Database db = new SqlDatabase(EAMIDBConnection.GetConnectionString());
            DataSet dataSet = new DataSet();
            CommonStatus status = new CommonStatus(true);

            try
            {
                // dbConn instance scope
                using (DbConnection dbConn = db.CreateConnection())
                {
                    dbConn.Open();

                    using (DataSet ds = db.ExecuteDataSet(DbStoredProcs.spGetECSReportData, DbStoredProcs.GetSPGetECSReportDataParams(dateFrom, dateTo)))
                    {
                        dataSet = ds;
                    }
                }
            }
            catch (Exception ex)
            {
                status.Status = false;
                status.IsFatal = true;
                status.AddMessageDetail(ex.Message);
                EAMILogger.Instance.Error(ex);
            }

            return new CommonStatusPayload<DataSet>(dataSet, status.Status, status.IsFatal, status.GetCombinedMessage());
        }

        public static CommonStatusPayload<DataSet> GetESTOReport(DateTime payDate)
        {
            string connString = ConfigurationManager.ConnectionStrings["EAMIData"].ConnectionString;
            Database db = new SqlDatabase(connString);
            DataSet dataSet = new DataSet();
            CommonStatus status = new CommonStatus(true);

            try
            {
                // dbConn instance scope
                using (DbConnection dbConn = db.CreateConnection())
                {
                    dbConn.Open();

                    using (DataSet ds = db.ExecuteDataSet(DbStoredProcs.spGetSTOReport, DbStoredProcs.GetSPGetESTOReportParams(payDate)))
                    {
                        dataSet = ds;
                    }
                }
            }
            catch (Exception ex)
            {
                status.Status = false;
                status.IsFatal = true;
                status.AddMessageDetail(ex.Message);
                EAMILogger.Instance.Error(ex);
            }

            return new CommonStatusPayload<DataSet>(dataSet, status.Status, status.IsFatal, status.GetCombinedMessage());
        }

        public static CommonStatusPayload<DataSet> GetDataSummaryReport(DateTime dateFrom, DateTime dateTo)
        {
            Database db = new SqlDatabase(EAMIDBConnection.GetConnectionString());
            DataSet dataSet = new DataSet();
            CommonStatus status = new CommonStatus(true);

            try
            {
                // dbConn instance scope
                using (DbConnection dbConn = db.CreateConnection())
                {
                    dbConn.Open();

                    using (DataSet ds = db.ExecuteDataSet(DbStoredProcs.spGetDatasummaryReportData, DbStoredProcs.GetSPGetDataSummaryReportDataParams(dateFrom, dateTo)))
                    {
                        dataSet = ds;
                    }
                }
            }
            catch (Exception ex)
            {
                status.Status = false;
                status.IsFatal = true;
                status.AddMessageDetail(ex.Message);
                EAMILogger.Instance.Error(ex);
            }

            return new CommonStatusPayload<DataSet>(dataSet, status.Status, status.IsFatal, status.GetCombinedMessage());
        }

        #endregion

        #region Private Methods


        #endregion

    }
}
