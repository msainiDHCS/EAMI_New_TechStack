using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using OHC.EAMI.Common;
using OHC.EAMI.CommonEntity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OHC.EAMI.DataAccess
{
    public static class PaymentDataSubmissionDBMgr
    {
        /// <summary>
        /// Persists complete payment submission dataset
        /// </summary>
        /// <param name="rt"></param>
        /// <returns></returns>
        public static CommonStatus InsertPaymentSubmissionDataToDB(RequestTransaction rt)
        {
            CommonStatus status = InsertPaymentSubmissionRequestResponseData(rt);
            if (status.Status && rt.RespStatusTypeID != RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_RESPONSE_STATUS_TYPE).GetRefCodeByCode("REJECTED").ID)
            {
                status = InsertPaymentData(rt);
            }
            return status;
        }

        /// <summary>
        /// persists status inquiry request/response data
        /// </summary>
        /// <param name="rt"></param>
        /// <returns></returns>
        public static CommonStatus InsertPaymentStatusInquiryToDB(RequestTransaction rt)
        {
            return InsertPaymentInquiryRequestResponseData(rt);
        }

        /// <summary>
        /// persists rejected status inquery request/response data
        /// </summary>
        /// <param name="rt"></param>
        /// <returns></returns>
        public static CommonStatus InsertRejectedPaymentStatusInquiryToDB(RequestTransaction rt)
        {
            return InsertPaymentInquiryRequestResponseData(rt);
        }


        /// <summary>
        /// gets duplicate transaction num based on provided new tran number
        /// </summary>
        /// <param name="msgTransactionId"></param>
        /// <returns></returns>
        public static bool RequestTransactionExist(string msgTransactionId)
        {
            bool retFlag = false;
            Database db = new SqlDatabase(EAMIDBConnection.GetConnectionString());
            try
            {
                // dbConn instance scope
                using (DbConnection dbConn = db.CreateConnection())
                {
                    dbConn.Open();
                    // persist request                
                    using (var reader = db.ExecuteReader(DbStoredProcs.spCheckDuplicateTransaction, msgTransactionId))
                    {
                        if (reader.Read())
                        {
                            if (msgTransactionId == reader[0].ToString())
                            {
                                retFlag = true;                                
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                EAMILogger.Instance.Error(ex);              
            }
            return retFlag;
        }


        /// <summary>
        /// gets existing PaymentSetNumber list from provided list of numbers
        /// </summary>
        /// <param name="newPaymentSetNumberList"></param>
        /// <returns></returns>
        public static List<string> GetDuplicatePaymentSetNumberList(List<string> newPaymentSetNumberList)
        {
            List<string> returnDups = new List<string>();
            string psListString = String.Join(",", newPaymentSetNumberList);
            Database db = new SqlDatabase(EAMIDBConnection.GetConnectionString());
            try
            {
                // dbConn instance scope
                using (DbConnection dbConn = db.CreateConnection())
                {
                    dbConn.Open();
                    // persist request                
                    using (var reader = db.ExecuteReader(DbStoredProcs.spCheckDuplicatePaymentSetNumbers, psListString))
                    {
                        while (reader.Read())
                        {
                            returnDups.Add(reader[0].ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                EAMILogger.Instance.Error(ex);
                returnDups = null;
            }
            return returnDups;
        }


        /// <summary>
        /// gets existing payment records based on provided list
        /// </summary>
        /// <param name="newPaymentRecNumbers"></param>
        /// <returns></returns>
        public static List<string> GetDuplicatePaymentRecordNumbers(List<string> newPaymentRecNumbers)
        {
            List<string> returnDups = new List<string>();
            string recNumListString = String.Join(",", newPaymentRecNumbers);
            Database db = new SqlDatabase(EAMIDBConnection.GetConnectionString());
            try
            {
                // dbConn instance scope
                using (DbConnection dbConn = db.CreateConnection())
                {
                    dbConn.Open();
                    // persist request                
                    using (var reader = db.ExecuteReader(DbStoredProcs.spCheckDuplicatePaymentRecords, recNumListString))
                    {
                        while (reader.Read())
                        {                        
                            returnDups.Add(reader[0].ToString());                            
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                EAMILogger.Instance.Error(ex);
                returnDups = null;
            }
            return returnDups;
        }


        /// <summary>
        /// gets DataSet of payment submission transactions for notification
        /// </summary>
        /// <returns></returns>
        public static DataSet GetPymtSubmissionTransForNotification()
        {
            Database db = new SqlDatabase(EAMIDBConnection.GetConnectionString());
            DataSet ds = null;
            try
            {
                using (DbConnection dbConn = db.CreateConnection())
                {
                    dbConn.Open();
                    ds = db.ExecuteDataSet(CommandType.StoredProcedure, DbStoredProcs.spGetPymtSubmissionTransForNotification);
                }
            }
            catch (Exception ex)
            {
                EAMILogger.Instance.Error(ex);
            }
            return ds;
        }

        /// <summary>
        /// updates payment submission transaction notification flag
        /// </summary>
        /// <param name="tranIdList"></param>
        public static void UpdatePymtSubmissionTransNonificationFlag(List<int> tranIdList) 
        {
            string tranIdListString = String.Join(",", tranIdList);
            Database db = new SqlDatabase(EAMIDBConnection.GetConnectionString());
            try
            {
                using (DbConnection dbConn = db.CreateConnection())
                {
                    dbConn.Open();
                    db.ExecuteNonQuery(DbStoredProcs.spUpdatePymtSubmissionTransNotificationFlag, tranIdListString);
                }
            }
            catch (Exception ex)
            {
                EAMILogger.Instance.Error(ex);
            }
        }


        /// <summary>
        /// insert request/response and optional trace details
        /// </summary>
        /// <param name="rt"></param>
        /// <returns></returns>
        private static CommonStatus InsertPaymentSubmissionRequestResponseData(RequestTransaction rt)
        {
            CommonStatus status = new CommonStatus(true);
            Database db = new SqlDatabase(EAMIDBConnection.GetConnectionString());
            // get a temp copy of all refcodes
            RefCodeTableList rcTableList = RefCodeDBMgr.GetRefCodeTableList();
            // get trace flag
            
            EAMIMasterData eamiMasterData = new EAMIMasterData();
            eamiMasterData.SystemProperty = rcTableList.GetRefCodeListByTableName(enRefTables.TB_System).GetRefCodeByCode<SystemProperty>(rcTableList.GetRefCodeListByTableName(enRefTables.TB_SYSTEM_OF_RECORD)[0].Code);
            bool traceAll = eamiMasterData.SystemProperty.TraceIncomingPmtData;
            try
            {
                // dbConn instance scope
                using (DbConnection dbConn = db.CreateConnection())
                {
                    dbConn.Open();

                    // dbTran instance scope
                    using (DbTransaction dbTran = dbConn.BeginTransaction())
                    {
                        // persist request                
                        using (var reader = db.ExecuteReader(dbTran, DbStoredProcs.spInsertRequest, DbStoredProcs.GetSPInsertRequestParams(rt)))
                        {
                            if (reader.Read())
                            {
                                rt.RequestTransactionID = int.Parse(reader[0].ToString());
                            }
                        }

                        // persist response                    
                        db.ExecuteNonQuery(dbTran, DbStoredProcs.spInsertResponse, DbStoredProcs.GetSPInsertResponseParams(rt));
                        
                        if (traceAll)
                        {
                            // persist trace transaction (when trace flag is on)       
                            db.ExecuteNonQuery(dbTran, DbStoredProcs.spInsertTraceTrasaction, DbStoredProcs.GetSPInsertTraceTranParams(rt));

                            // persist trace payment list 
                            if (rt.PaymentRecList == null) rt.PaymentRecList = new List<PaymentRec>();
                            foreach (PaymentRecTr pr in rt.PaymentRecList)
                            {
                                // set transaction id
                                pr.TransactionID = rt.RequestTransactionID;

                                if (traceAll)
                                {
                                    // persist trace payment
                                    db.ExecuteNonQuery(dbTran, DbStoredProcs.spInsertTracePayment, DbStoredProcs.GetSPInsertTracePaymentParams(pr));
                                }
                            }
                        }
                        
                        dbTran.Commit();
                        // note: DbTransaction will automatically rollback when disposed (assuming it hasn't been committed).
                    }
                }
            }
            catch (Exception ex)
            {
                EAMILogger.Instance.Error(ex);
                status.Status = false;
                status.IsFatal = true;
                status.AddMessageDetail(ex.Message); 
            }
            return status;
        }


        /// <summary>
        /// insert request/response and optional trace details
        /// </summary>
        /// <param name="rt"></param>
        /// <returns></returns>
        private static CommonStatus InsertPaymentInquiryRequestResponseData(RequestTransaction rt)
        {
            CommonStatus status = new CommonStatus(true);
            Database db = new SqlDatabase(EAMIDBConnection.GetConnectionString());
            // get a temp copy of all refcodes
            RefCodeTableList rcTableList = RefCodeDBMgr.GetRefCodeTableList();
            // get trace flag
            EAMIMasterData eamiMasterData = new EAMIMasterData();
            eamiMasterData.SystemProperty = rcTableList.GetRefCodeListByTableName(enRefTables.TB_System).GetRefCodeByCode<SystemProperty>(rcTableList.GetRefCodeListByTableName(enRefTables.TB_SYSTEM_OF_RECORD)[0].Code);
            bool traceAll = eamiMasterData.SystemProperty.TraceIncomingPmtData;

            try
            {
                // dbConn instance scope
                using (DbConnection dbConn = db.CreateConnection())
                {
                    dbConn.Open();

                    // dbTran instance scope
                    using (DbTransaction dbTran = dbConn.BeginTransaction())
                    {
                        // persist request                
                        using (var reader = db.ExecuteReader(dbTran, DbStoredProcs.spInsertRequest, DbStoredProcs.GetSPInsertRequestParams(rt)))
                        {
                            if (reader.Read())
                            {
                                rt.RequestTransactionID = int.Parse(reader[0].ToString());
                            }
                        }

                        // persist response                    
                        db.ExecuteNonQuery(dbTran, DbStoredProcs.spInsertResponse, DbStoredProcs.GetSPInsertResponseParams(rt));

                        if (traceAll)
                        {
                            // persist trace transaction (when trace flag is on)       
                            db.ExecuteNonQuery(dbTran, DbStoredProcs.spInsertTraceTrasaction, DbStoredProcs.GetSPInsertTraceTranParams(rt));                            
                        }

                        dbTran.Commit();
                        // note: DbTransaction will automatically rollback when disposed (assuming it hasn't been committed).
                    }
                }
            }
            catch (Exception ex)
            {
                EAMILogger.Instance.Error(ex);
                status.Status = false;
                status.IsFatal = true;
                status.AddMessageDetail(ex.Message);
            }
            return status;
        }


        /// <summary>
        /// persists Payment data
        /// </summary>
        /// <param name="rt"></param>
        /// <returns></returns>
        private static CommonStatus InsertPaymentData(RequestTransaction rt)
        {
            CommonStatus status = new CommonStatus(true);
            Database db = new SqlDatabase(EAMIDBConnection.GetConnectionString());
            // get a temp copy of ref codes
            RefCodeTableList rcTableList = RefCodeDBMgr.GetRefCodeTableList();
            DateTime statusDate = DateTime.Now;

            try
            {
                // dbConn instance scope
                using (DbConnection dbConn = db.CreateConnection())
                {
                    dbConn.Open();
                    
                    // dbTran instance scope
                    using (DbTransaction dbTran = dbConn.BeginTransaction())
                    {                        
                        // persist transaction
                        db.ExecuteNonQuery(dbTran, DbStoredProcs.spInsertTransaction, DbStoredProcs.GetSPInsertTransactionParams(rt));
                                                                        
                        // persist payment list
                        if (rt.PaymentRecList == null) rt.PaymentRecList = new List<PaymentRec>();
                        foreach (PaymentRec pr in rt.PaymentRecList)
                        {
                            // only persist valid payment records
                            if (pr.CurrentStatus.StatusType.ID != rcTableList.GetRefCodeListByTableName(enRefTables.TB_PAYMENT_STATUS_TYPE).GetRefCodeByCode("RECEIVED").ID)
                            {
                                continue;
                            }

                            // set transaction id
                            pr.TransactionID = rt.RequestTransactionID;

                            // persist payee info
                            pr.PayeeInfo.PEE_ContractNumber = pr.PaymentKvpList["CONTRACT_NUMBER"];
                            using (var reader = db.ExecuteReader(dbTran, DbStoredProcs.spInsertPaymentExchangeEntityInfo, DbStoredProcs.GetSPInsertPayeeInfoParams(rt.SOR_ID, pr.PayeeInfo)))
                            {
                                if (reader.Read())
                                {
                                    pr.PayeeInfo.PEEInfo_PK_ID = int.Parse(reader[0].ToString());
                                   // pr.PayeeInfo.PEE_VendorTypeCode = reader[1].ToString();
                                }
                            }               

                            // persist payment record
                            pr.RPICode = "6"; // hardcoding until EAMY can handle other payment types (per accounting)
                           // pr.IsReportableRPI = ((pr.PayeeInfo.PEE_VendorTypeCode == "C" || pr.PayeeInfo.PEE_VendorTypeCode == "P") && pr.ObjDetailCode == "705");
                            pr.ContractNumber = pr.PaymentKvpList["CONTRACT_NUMBER"];
                            pr.ContractDateFrom = Convert.ToDateTime(pr.PaymentKvpList["CONTRACT_DATE_FROM"]);
                            pr.ContractDateTo = Convert.ToDateTime(pr.PaymentKvpList["CONTRACT_DATE_TO"]);
                            pr.ExclusivePaymentType = rcTableList.GetRefCodeListByTableName(enRefTables.TB_EXCLUSIVE_PAYMENT_TYPE).GetRefCodeByCode(pr.PaymentKvpList["EXCLUSIVE_PYMT_CODE"]);                        
                            pr.PaymentMethodType = rcTableList.GetRefCodeListByTableName(enRefTables.TB_PAYMENT_METHOD_TYPE).Where(_ => _.Code == "UNKNOWN").First();
                            using (var reader = db.ExecuteReader(dbTran, DbStoredProcs.spInsertPaymentRecord, DbStoredProcs.GetSPInsertPaymentParams(pr)))
                            {
                                if (reader.Read())
                                {
                                    pr.PrimaryKeyID = int.Parse(reader[0].ToString());
                                }
                            }                         
                            
                            /* Eugene - disabling inserting dynamic KVP entity, the value is persisted into payment-rec extension table
                            // persist payment KVP values (key value pair list)
                            if (pr.PaymentKvpList == null) pr.PaymentKvpList = new Dictionary<string, string>();
                            foreach (KeyValuePair<string, string> kvp in pr.PaymentKvpList)
                            {
                                KvpDefinition kvpDefPymnt = rcTableList.GetRefCodeListByTableName(enRefTables.TB_SOR_KVP_KEY).GetRefCodeByCode<KvpDefinition>(kvp.Key);
                                db.ExecuteNonQuery(dbTran, DbStoredProcs.spInsertPaymentKvp, DbStoredProcs.GetSPInsertPaymentKVPParams(pr, kvpDefPymnt, kvp.Value));
                            }
                            */

                            // persist funding detail
                            if (pr.FundingDetailList == null) pr.FundingDetailList = new List<PaymentFundingDetail>();
                            foreach (PaymentFundingDetail pfd in pr.FundingDetailList)
                            {
                                pfd.PaymentRecID = pr.PrimaryKeyID;
                                pfd.FedFundCode = (pfd.FundingKvpList == null || pfd.FundingKvpList.Count == 0) ? null : pfd.FundingKvpList["FEDFUNDCODE"];
                                pfd.StateFundCode = (pfd.FundingKvpList == null || pfd.FundingKvpList.Count == 0) ? null : pfd.FundingKvpList["STATEFUNDCODE"];
                                using (var reader = db.ExecuteReader(dbTran, DbStoredProcs.spInsertFundingDetail, DbStoredProcs.GetSPInsertFundingDetailParams(pfd)))
                                {
                                    if (reader.Read())
                                    {
                                        pfd.FundingDetailID = int.Parse(reader[0].ToString());
                                    }
                                }

                                /* Eugene - disabling inserting dynamic KVP entity, the value is persisted into funding extension table
                                // persist funding KVP values
                                if (pfd.FundingKvpList == null) pfd.FundingKvpList = new Dictionary<string, string>();
                                foreach (KeyValuePair<string, string> kvp in pfd.FundingKvpList)
                                {
                                    KvpDefinition kvpDefFD = rcTableList.GetRefCodeListByTableName(enRefTables.TB_SOR_KVP_KEY).GetRefCodeByCode<KvpDefinition>(kvp.Key);
                                    db.ExecuteNonQuery(dbTran, DbStoredProcs.spInsertFundingDetailKvp, DbStoredProcs.GetSPInsertFundingDetailKVPParams(pfd, kvpDefFD, kvp.Value));
                                }
                                */
                            }


                            // persist payment status
                            pr.CurrentStatus.EntityID = pr.PrimaryKeyID;
                            pr.CurrentStatus.StatusDate = statusDate;
                            db.ExecuteNonQuery(dbTran, DbStoredProcs.spInsertPaymentStatus, DbStoredProcs.GetSPInsertPaymentStatusParams(pr.CurrentStatus));
                        }                                                                                                

                        dbTran.Commit();
                        // note: DbTransaction will automatically rollback when disposed (assuming it hasn't been committed).
                    }
                }

                /* Eugene - disabling insert/update 'UNASSIGNED' status after initial insert of payments for given transaction  
                // update status on all inserted payments for this transaction
                using (DbConnection dbConn = db.CreateConnection())
                {
                    dbConn.Open();
                    db.ExecuteNonQuery(DbStoredProcs.spUpdateTransactionPaymentsStatus, rt.RequestTransactionID);                    
                }
                */
            }
            catch (Exception ex)
            {
                EAMILogger.Instance.Error(ex);
                status.Status = false;
                status.IsFatal = true;
                status.AddMessageDetail(ex.Message);                
            }

            return status;
        }



    }
}
