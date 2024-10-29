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
    public class PaymentDataDbMgr : DataAccessBase
    {
        #region Public Methods

        /// <summary>
        /// gets funding details from database by payment record id
        /// </summary>
        /// <param name="paymentRecId"></param>
        /// <returns></returns>
        public static List<PaymentFundingDetail> GetFundingDetailsByPaymentRecID(int paymentRecordID)
        {
            List<PaymentFundingDetail> fdList = new List<PaymentFundingDetail>();
            Database db = new SqlDatabase(EAMIDBConnection.GetConnectionString());
            try
            {
                // dbConn instance scope
                using (DbConnection dbConn = db.CreateConnection())
                {
                    dbConn.Open();
                    using (var reader = db.ExecuteReader(DbStoredProcs.spGetFundingDetailsForPaymentRec, paymentRecordID))
                    {
                        while (reader.Read())
                        {
                            PaymentFundingDetail pfd = DataAccessSharedFunctions.GetFundingDetailFromReader(reader);
                            fdList.Add(pfd);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                EAMILogger.Instance.Error(ex);
                fdList = null;
                throw new Exception("Error getting records from database");
            }
            return fdList;
        }

        /// <summary>
        /// gets payment record statuses per given record number list
        /// </summary>
        /// <param name="paymentRecNumberList"></param>
        /// <returns></returns>
        public static Dictionary<string, PaymentStatus> GetCurrentPaymentStatusByPaymentRecNumberList(List<string> paymentRecNumberList)
        {
            Dictionary<string, PaymentStatus> prsList = new Dictionary<string, PaymentStatus>();
            Database db = new SqlDatabase(EAMIDBConnection.GetConnectionString());
            try
            {
                // dbConn instance scope
                using (DbConnection dbConn = db.CreateConnection())
                {
                    dbConn.Open();
                    // persist request                
                    using (DataSet ds = db.ExecuteDataSet(DbStoredProcs.spGetPaymentRecStatusList, DbStoredProcs.GetSPGetPaymentRecStatusListParams(String.Join(",", paymentRecNumberList))))
                    {
                        RefCodeTableList rctl = RefCodeDBMgr.GetRefCodeTableList();
                        foreach (DataRow dataRow in ds.Tables[0].Rows)
                        {
                            PaymentStatus ps = DataAccessSharedFunctions.GetExtendedCurrentPaymentStatusFromDataRow(dataRow, rctl, true);
                            prsList.Add(ps.PaymentRecNumber, ps);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                EAMILogger.Instance.Error(ex);
                prsList = null;
                throw new Exception("Error getting records from database");
            }
            return prsList;
        }

        /// <summary>
        /// gets rejected payment status list based on date range
        /// </summary>
        /// <param name="rejectedDateFrom"></param>
        /// <param name="rejectedDateTo"></param>
        /// <returns></returns>
        public static List<PaymentStatus> GetRejectedPaymentsStatusListByDateRange(DateTime rejectedDateFrom, DateTime rejectedDateTo)
        {
            List<PaymentStatus> psList = new List<PaymentStatus>();
            Database db = new SqlDatabase(EAMIDBConnection.GetConnectionString());
            try
            {
                // dbConn instance scope
                using (DbConnection dbConn = db.CreateConnection())
                {
                    dbConn.Open();
                    // persist request                
                    using (DataSet ds = db.ExecuteDataSet(DbStoredProcs.spGetRejectedPaymentRecStatusList, new object[] { rejectedDateFrom, rejectedDateTo }))
                    {
                        RefCodeTableList rctl = RefCodeDBMgr.GetRefCodeTableList();
                        foreach (DataRow dataRow in ds.Tables[0].Rows)
                        {
                            PaymentStatus ps = DataAccessSharedFunctions.GetExtendedCurrentPaymentStatusFromDataRow(dataRow, rctl, false);
                            psList.Add(ps);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                EAMILogger.Instance.Error(ex);
                psList = null;
                throw new Exception("Error getting records from database");
            }
            return psList;
        }
        
        public static CommonStatus SetPaymentGroupStatus(RefCode statusType, List<string> paymentSetNumberList, string user, string note)
        {
            CommonStatus cs = new CommonStatus(false);            
            Database db = new SqlDatabase(EAMIDBConnection.GetConnectionString());

            try
            {
                List<int> paymentRecordIDList = GetPaymentRecordIDsByPaymentSetNumberList(paymentSetNumberList);
                
                // dbConn instance scope
                using (DbConnection dbConn = db.CreateConnection())
                {
                    dbConn.Open();

                    // dbTran instance scope
                    using (DbTransaction dbTran = dbConn.BeginTransaction())
                    {
                        DateTime statusDate = DateTime.Now;

                        foreach (int paymentRecordID in paymentRecordIDList)
                        {
                            // Validate the new Payment Status - Check if new status is appropriate to continue with
                            cs = ValidatePaymentStatus(db, dbTran, paymentRecordID, statusType);
                            
                            if(cs.Status)
                            {
                                db.ExecuteNonQuery(dbTran, DbStoredProcs.spInsertPaymentStatus,
                                    DbStoredProcs.GetSPInsertPaymentStatusParams(new PaymentStatus { EntityID = paymentRecordID, StatusType = statusType, StatusNote = note, CreatedBy = user, StatusDate = statusDate}));
                            }
                            else break;
                        }

                        //COMMIT transaction
                        if (cs.Status)
                        {
                            dbTran.Commit();
                        }                        
                    }
                }
            }
            catch (Exception ex)
            {
                EAMILogger.Instance.Error(ex);
                cs.Status = false;
                cs.IsFatal = true;
                cs.AddMessageDetail(ex.Message);
            }

            return cs;
        }

        public static CommonStatus AssignPaymentGroup(RefCode statusType, List<PaymentGroup> paymentGroupList, string user)
        {
            CommonStatus cs = new CommonStatus(true);
            Database db = new SqlDatabase(EAMIDBConnection.GetConnectionString());
            DateTime statusDate = DateTime.Now;

            try
            {
                List<PaymentRec> paymentRecordList = GetPaymentRecordsByPaymentSetNumberList(paymentGroupList.Select(_ => _.UniqueNumber).ToList());

                // dbConn instance scope
                using (DbConnection dbConn = db.CreateConnection())
                {
                    dbConn.Open();
                    List<int> PaymentRecIDList;

                    // dbTran instance scope
                    using (DbTransaction dbTran = dbConn.BeginTransaction())
                    {                        
                        foreach (PaymentRec paymentRec in paymentRecordList)
                        {
                            string payDate = paymentGroupList.Where(_ => _.UniqueNumber == paymentRec.PaymentSetNumber).Select(__ => __.PayDate.Code).FirstOrDefault();
                            int payDateID = RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_PAYDATE_CALENDAR).Where(_ => _.Code == payDate).FirstOrDefault().ID;
                            int assignedUserID = (int)paymentGroupList.Where(_ => _.UniqueNumber == paymentRec.PaymentSetNumber).Select(__ => __.AssignedUser.User_ID).FirstOrDefault();

                            cs = ValidatePaymentStatus(db, dbTran, paymentRec.PrimaryKeyID, statusType);

                            //IF VALID STATUS, CONTINUE WITH USER AND PAYDATE ASSIGNMENT
                            if (cs.Status)
                            {
                                PaymentStatus ps = new PaymentStatus
                                { 
                                    EntityID = paymentRec.PrimaryKeyID, 
                                    StatusType = statusType, 
                                    StatusNote = string.Empty, 
                                    CreatedBy = user,
                                    StatusDate = statusDate
                                };

                                //INSERT STATUS
                                db.ExecuteNonQuery(dbTran, DbStoredProcs.spInsertPaymentStatus, DbStoredProcs.GetSPInsertPaymentStatusParams(ps));

                                //INSERT USER ASSIGNMENT
                                db.ExecuteNonQuery(dbTran, DbStoredProcs.spInsertPaymentUserAssignment, DbStoredProcs.GetSPInsertPaymentUserAssignmentParams(paymentRec.PrimaryKeyID, assignedUserID));

                                //INSERT PAYMENT REC PAY DATE
                                db.ExecuteNonQuery(dbTran, DbStoredProcs.spInsertPaymentPaydate, DbStoredProcs.GetSPInsertPaymentPaydateParams(paymentRec.PrimaryKeyID.ToString(), payDateID));

                                // set current status and usr name
                                //PaymentGroup pg = paymentGroupList.Find(t => t.UniqueNumber == paymentRec.PaymentSetNumber);
                                //pg.CurrentStatus = ps;
                                //pg.AssignedUser.User_Name = RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_USER).GetRefCodeByID(assignedUserID).Code;
                                //pg.AssignedUser.User_EmailAddr = RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_USER).GetRefCodeByID<UserAcc>(assignedUserID).User_EmailAddr;

                            }
                            else break;
                        }

                        if (cs.Status)
                        {
                            //COMMIT transaction
                            dbTran.Commit();
                        } 

                    }
                }
            }
            catch (Exception ex)
            {
                EAMILogger.Instance.Error(ex);
                cs.Status = false;
                cs.IsFatal = true;
                cs.AddMessageDetail(ex.Message);
            }

            return cs;
        }

        public static CommonStatus UnAssignPaymentGroup(RefCode statusType, List<PaymentGroup> paymentGroupList, string user)
        {
            CommonStatus cs = new CommonStatus(true);
            Database db = new SqlDatabase(EAMIDBConnection.GetConnectionString());
            DateTime statusDate = DateTime.Now;

            try
            {
                List<int> paymentRecordIDList = GetPaymentRecordIDsByPaymentSetNumberList(paymentGroupList.Select(_ => _.UniqueNumber).ToList());

                // dbConn instance scope
                using (DbConnection dbConn = db.CreateConnection())
                {
                    dbConn.Open();

                    // dbTran instance scope
                    using (DbTransaction dbTran = dbConn.BeginTransaction())
                    {
                        foreach (int paymentRecID in paymentRecordIDList)
                        {
                            cs = ValidatePaymentStatus(db, dbTran, paymentRecID, statusType);

                            //IF VALID STATUS, CONTINUE WITH USER AND PAYDATE ASSIGNMENT
                            if (cs.Status)
                            {
                                //INSERT STATUS
                                db.ExecuteNonQuery(dbTran, DbStoredProcs.spInsertPaymentStatus, DbStoredProcs.GetSPInsertPaymentStatusParams(new PaymentStatus { EntityID = paymentRecID, StatusType = statusType, StatusNote = string.Empty, CreatedBy = user, StatusDate = statusDate}));
                            }
                            else break;
                        }

                        if (cs.Status)
                        {
                            //DELETE PAYMENT REC PAY DATE
                            db.ExecuteNonQuery(dbTran, DbStoredProcs.spDeletePaymentPaydate, DbStoredProcs.GetSPDeletePaymentPaydateParams(String.Join(",", paymentRecordIDList)));

                            //COMMIT transaction
                            dbTran.Commit();
                        } 
                    }
                }
            }
            catch (Exception ex)
            {
                EAMILogger.Instance.Error(ex);
                cs.Status = false;
                cs.IsFatal = true;
                cs.AddMessageDetail(ex.Message);
            }

            return cs;
        }

        public static CommonStatus ReAssignPaymentGroup(RefCode statusType, List<PaymentGroup> paymentGroupList, string user)
        {
            CommonStatus cs = new CommonStatus(true);
            Database db = new SqlDatabase(EAMIDBConnection.GetConnectionString());
            DateTime statusDate = DateTime.Now;

            try
            {
                List<PaymentRec> paymentRecordList = GetPaymentRecordsByPaymentSetNumberList(paymentGroupList.Select(_ => _.UniqueNumber).ToList());

                // dbConn instance scope
                using (DbConnection dbConn = db.CreateConnection())
                {
                    dbConn.Open();

                    // dbTran instance scope
                    using (DbTransaction dbTran = dbConn.BeginTransaction())
                    {
                        foreach (PaymentRec paymentRec in paymentRecordList)
                        {
                            string payDate = paymentGroupList.Where(_ => _.UniqueNumber == paymentRec.PaymentSetNumber).Select(__ => __.PayDate.Code).FirstOrDefault();
                            int payDateID = RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_PAYDATE_CALENDAR).Where(_ => _.Code == payDate).FirstOrDefault().ID;
                            int assignedUserID = (int)paymentGroupList.Where(_ => _.UniqueNumber == paymentRec.PaymentSetNumber).Select(__ => __.AssignedUser.User_ID).FirstOrDefault();

                            PaymentStatus ps = new PaymentStatus
                            {
                                EntityID = paymentRec.PrimaryKeyID,
                                StatusType = statusType,
                                StatusNote = string.Empty,
                                CreatedBy = user,
                                StatusDate = statusDate
                            };

                            //INSERT STATUS
                            db.ExecuteNonQuery(dbTran, DbStoredProcs.spInsertPaymentStatus, DbStoredProcs.GetSPInsertPaymentStatusParams(ps));

                            //INSERT USER ASSIGNMENT
                            db.ExecuteNonQuery(dbTran, DbStoredProcs.spInsertPaymentUserAssignment, DbStoredProcs.GetSPInsertPaymentUserAssignmentParams(paymentRec.PrimaryKeyID, assignedUserID));

                            //INSERT PAYMENT REC PAY DATE
                            db.ExecuteNonQuery(dbTran, DbStoredProcs.spInsertPaymentPaydate, DbStoredProcs.GetSPInsertPaymentPaydateParams(String.Join(",", paymentRecordList.Select(_ => _.PrimaryKeyID)), payDateID));

                            // set current status and usr name
                            //PaymentGroup pg = paymentGroupList.Find(t => t.UniqueNumber == paymentRec.PaymentSetNumber);
                            //pg.CurrentStatus = ps;
                            //pg.AssignedUser.User_Name = RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_USER).GetRefCodeByID(assignedUserID).Code;
                            //pg.AssignedUser.User_EmailAddr = RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_USER).GetRefCodeByID<UserAcc>(assignedUserID).User_EmailAddr;
                        }

                        if (cs.Status)
                        {
                            //COMMIT transaction
                            dbTran.Commit();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                EAMILogger.Instance.Error(ex);
                cs.Status = false;
                cs.IsFatal = true;
                cs.AddMessageDetail(ex.Message);
            }

            return cs;
        }

        /// <summary>
        /// gets payment group from database by PaymentStatusTypeId
        /// </summary>
        /// <param name="paymentStatusTypeId"></param>
        /// <returns></returns>
        public static List<PaymentGroup> GetPaymentGroupsByStatus(int paymentStatusTypeId)
        {
            List<PaymentRec> prList = GetPaymentRecordsByStatus(paymentStatusTypeId);
            return DataAccessSharedFunctions.GetPaymentGroupListFromPaymentRecList(prList);
        }

        public static List<PaymentGroup> GetPaymentGroupsByStatusAndTransaction(int paymentStatusTypeId, string TrnasactionId)
        {
            List<PaymentRec> prList = GetPaymentRecordsByStatusAndTransaction(paymentStatusTypeId, TrnasactionId);
            return DataAccessSharedFunctions.GetPaymentGroupListFromPaymentRecList(prList);
        }

        public static List<PaymentGroup> GetPaymentGroupsAssignedByUser(int userID)
        {
            List<PaymentRec> prList = GetPaymentRecordsAssignedByUser(userID);
            return DataAccessSharedFunctions.GetPaymentGroupListFromPaymentRecList(prList);
        }
        
        public static List<PaymentGroup> GetPaymentGroupsByClaimScheduleID(int claimScheduleID)
        {
            List<PaymentRec> prList = GetPaymentRecordsByStatus(claimScheduleID);
            return DataAccessSharedFunctions.GetPaymentGroupListFromPaymentRecList(prList);
        }

        public static List<PaymentGroup> GetPaymentGroupsByPaymentSetNumberList(List<string> paymentSetNumberList)
        {
            List<PaymentRec> prList = GetPaymentRecordsByPaymentSetNumberList(paymentSetNumberList);
            return DataAccessSharedFunctions.GetPaymentGroupListFromPaymentRecList(prList);
        }

        public static CommonStatusPayload<string> SetPaymentGroupToReleaseFromSup(List<string> paymentSetNumberList, string user, string note)
        {
            CommonStatus cs = new CommonStatus(true);
            string assignedUserEmailAddr = string.Empty;
            Database db = new SqlDatabase(EAMIDBConnection.GetConnectionString());
            DateTime statusDate = DateTime.Now;

            try
            {
                RefCode statusType_ASSIGNED = RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_PAYMENT_STATUS_TYPE).Where(_ => _.Code == "ASSIGNED").FirstOrDefault();
                RefCode statusType_RELEASED_FROM_SUP = RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_PAYMENT_STATUS_TYPE).Where(_ => _.Code == "RELEASED_FROM_SUP").FirstOrDefault();

                List<PaymentRec> paymentRecordList = GetPaymentRecordsByPaymentSetNumberList(paymentSetNumberList);
                assignedUserEmailAddr = (paymentRecordList != null && paymentRecordList.Count > 0) ? 
                                        paymentRecordList[0].AssignedUser.User_EmailAddr : 
                                        string.Empty;

                // dbConn instance scope
                using (DbConnection dbConn = db.CreateConnection())
                {
                    dbConn.Open();

                    // dbTran instance scope
                    using (DbTransaction dbTran = dbConn.BeginTransaction())
                    {
                        foreach (PaymentRec paymentRec in paymentRecordList)
                        {
                            int payDateID = paymentRec.PayDate.ID;
                            int assignedUserID = (int)paymentRec.AssignedUser.User_ID;

                            //INSERT ASSIGNED STATUS
                            db.ExecuteNonQuery(dbTran, DbStoredProcs.spInsertPaymentStatus, DbStoredProcs.GetSPInsertPaymentStatusParams(new PaymentStatus { EntityID = paymentRec.PrimaryKeyID, StatusType = statusType_ASSIGNED, StatusNote = string.Empty, CreatedBy = user, StatusDate = statusDate }));

                            //INSERT USER ASSIGNMENT
                            db.ExecuteNonQuery(dbTran, DbStoredProcs.spInsertPaymentUserAssignment, DbStoredProcs.GetSPInsertPaymentUserAssignmentParams(paymentRec.PrimaryKeyID, assignedUserID));

                            //INSERT PAYMENT REC PAY DATE
                            db.ExecuteNonQuery(dbTran, DbStoredProcs.spInsertPaymentPaydate, DbStoredProcs.GetSPInsertPaymentPaydateParams(String.Join(",", paymentRecordList.Select(_ => _.PrimaryKeyID)), payDateID));

                            //INSERT RELEASED_FROM_SUP STATUS
                            db.ExecuteNonQuery(dbTran, DbStoredProcs.spInsertPaymentStatus, DbStoredProcs.GetSPInsertPaymentStatusParams(new PaymentStatus { EntityID = paymentRec.PrimaryKeyID, StatusType = statusType_RELEASED_FROM_SUP, StatusNote = note, CreatedBy = user, StatusDate = statusDate }));
                        }

                        if (cs.Status)
                        {
                            //COMMIT transaction
                            dbTran.Commit();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                EAMILogger.Instance.Error(ex);
                cs.Status = false;
                cs.IsFatal = true;
                cs.AddMessageDetail(ex.Message);
            }

            // NOTE: Eugene S. need to perform email notification to assigned user
            // return user email (from first payment record) as status payload
            return new CommonStatusPayload<string>(assignedUserEmailAddr, cs.Status, cs.IsFatal, cs.MessageDetailList);
        }

        public static List<Tuple<EAMIUser, int>> GetUserListWithAssignedPaymentCount(int userID)
        {
            Database db = new SqlDatabase(EAMIDBConnection.GetConnectionString());
            List<Tuple<EAMIUser, int>> lstUserAssignmentCount = new List<Tuple<EAMIUser, int>>();

            try
            {
                // dbConn instance scope
                using (DbConnection dbConn = db.CreateConnection())
                {
                    dbConn.Open();
                    // persist request   
                    using (DataSet ds = db.ExecuteDataSet(DbStoredProcs.spGetUserListWithAssignedPaymentCount, DbStoredProcs.GetSPGetUserListWithAssignedPaymentCountParams(userID)))
                    {
                        foreach (DataRow usrDataRow in ds.Tables[0].Rows)
                        {
                            //USER
                            EAMIUser usr = new EAMIUser();
                            usr.User_Systems = new List<EAMIAuthBase>();
                            usr.User_ID = Convert.ToInt32(usrDataRow["User_ID"]);
                            usr.User_Name = usrDataRow["User_Name"].ToString();
                            usr.Display_Name = usrDataRow["Display_Name"].ToString();
                            int assignmentCount = Convert.ToInt32(usrDataRow["Assignment_Count"]);

                            //ROLES
                            usr.User_Roles = new List<EAMIAuthBase>();
                            string expression = string.Format("User_ID = {0}", usr.User_ID);
                            foreach (DataRow roleRow in ds.Tables[1].Select(expression))
                            {
                                //Populate user role records
                                usr.User_Roles.Add(new EAMIAuthBase()
                                {
                                    ID = Convert.ToInt32(roleRow["Role_ID"]),
                                    Code = roleRow["Role_Code"].ToString(),
                                    Name = roleRow["Role_Name"].ToString(), 
                                    IsActive = true });
                            }

                            lstUserAssignmentCount.Add(new Tuple<EAMIUser, int>(usr, assignmentCount));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                EAMILogger.Instance.Error(ex);
                lstUserAssignmentCount = null;
                throw new Exception("Error getting records from database");
            }
            return lstUserAssignmentCount;
        }

        public static List<PaymentRecForUser> GetAllPaymentRecordsByUser(int userID)
        {
            Database db = new SqlDatabase(EAMIDBConnection.GetConnectionString());
            List<PaymentRecForUser> lstPaymentAssignments = new List<PaymentRecForUser>();

            try
            {
                // dbConn instance scope
                using (DbConnection dbConn = db.CreateConnection())
                {
                    dbConn.Open();
                    // persist request   
                    using (DataSet ds = db.ExecuteDataSet(DbStoredProcs.spGetPaymentRecordsByUser, DbStoredProcs.GetPaymentRecordsByUserParams(userID)))
                    {
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            PaymentRecForUser paymentAssignedUsr = new PaymentRecForUser();
                            paymentAssignedUsr.Payment_Record_ID = Convert.ToInt32(dr["Payment_Record_ID"]);
                            paymentAssignedUsr.User_ID = Convert.ToInt32(dr["UserID"]);
                            paymentAssignedUsr.PaymentStatusCode = Convert.ToString(dr["Code"]);

                            lstPaymentAssignments.Add(paymentAssignedUsr);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                EAMILogger.Instance.Error(ex);
                lstPaymentAssignments = null;
                throw new Exception("Error getting records from database");
            }
            return lstPaymentAssignments;
        }

        public static CommonStatusPayload<List<Tuple<string, int>>> GetEAMICounts()
        {
            Database db = new SqlDatabase(EAMIDBConnection.GetConnectionString());
            
            List<Tuple<string, int>> lstEAMICounts = new List<Tuple<string, int>>();
            CommonStatusPayload<List<Tuple<string, int>>> commonStatus = new CommonStatusPayload<List<Tuple<string, int>>>(lstEAMICounts, true);

            try
            {
                // dbConn instance scope
                using (DbConnection dbConn = db.CreateConnection())
                {
                    dbConn.Open();
                    // persist request  
                    using (var reader = db.ExecuteReader(DbStoredProcs.spGetEAMICounts, DbStoredProcs.GetSPGetEAMICountsParams()))
                    {
                        while (reader.Read())
                        {
                            lstEAMICounts.Add(new Tuple<string, int>(reader["Category"].ToString(), int.Parse(reader["Count"].ToString())));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                EAMILogger.Instance.Error(ex);
                commonStatus.Status = false;
                commonStatus.IsFatal = true;
                commonStatus.AddMessageDetail(ex.Message);
            }

            return commonStatus;
        }

        /// <summary>
        /// check if system has 'RECEIVED' payments 
        /// </summary>
        /// <param name="sorId"></param>
        /// <returns></returns>
        public static bool CheckReceivedPayments(int sorId)
        {
            Database db = new SqlDatabase(EAMIDBConnection.GetConnectionString());
            bool retValue = false;

            try
            {
                using (DbConnection dbConn = db.CreateConnection())
                {
                    dbConn.Open();
                    DataSet ds = db.ExecuteDataSet(DbStoredProcs.spCheckReceivedPaymentRecords, sorId);
                    if(ds != null && ds.Tables.Count == 1 && ds.Tables[0].Rows.Count > 0)
                    {
                        retValue = true;
                    }
                }
            }
            catch (Exception ex)
            {
                EAMILogger.Instance.Error(ex);
            }
            return retValue;            
        }

        // testing new data access methods combining new payment notification and EFT check
        /* 
        public static CommonStatusPayload<Tuple<DataTable, DataTable, List<PaymentGroup>>> GetReceivedPaymentsForEFTCheckAndNotification()
        {
            Database db = new SqlDatabase(EAMIDBConnection.GetConnectionString());
            //Tuple<DataTable, DataTable, List<PaymentGroup>> payload = null;
            CommonStatusPayload<Tuple<DataTable, DataTable, List<PaymentGroup>>> commonStatus = new CommonStatusPayload<Tuple<DataTable, DataTable, List<PaymentGroup>>>(null, true);

            try
            {
                //using (DbConnection dbConn = db.CreateConnection())
                //{
                //    dbConn.Open();
                //    DataSet ds = db.ExecuteDataSet(DbStoredProcs.spGetPymtDataForEftCheckAndUserNotification);
                //    if (ds != null && ds.Tables.Count == 3 && ds.Tables[0].Rows.Count > 0)
                //    {
                //        retValue = true;
                //    }
                //}


                using (DataSet ds = db.ExecuteDataSet(DbStoredProcs.spGetPaymentRecordsByStatus, DbStoredProcs.GetSPGetPaymentRecordsByStatusParams(paymentStatusTypeId)))
                {
                    RefCodeTableList rctl = RefCodeDBMgr.GetRefCodeTableList();
                    foreach (DataRow dataRow in ds.Tables[0].Rows)
                    {
                        PaymentRec pr = DataAccessSharedFunctions.GetPaymentRecFromDataSet(dataRow, rctl);
                        paymentRecordList.Add(pr);
                    }
                }
            }
            catch (Exception ex)
            {
                EAMILogger.Instance.Error(ex);
                commonStatus.Status = false;
                commonStatus.IsFatal = true;
                commonStatus.AddMessageDetail(ex.Message);
            }

            return commonStatus;
        }
        */



        /// <summary>
        /// get data set of payment set record being on hold for notification
        /// </summary>
        /// <param name="daysPassed"></param>
        /// <returns></returns>
        public static DataSet GetPymtOnHoldForNotification(int daysPassed)
        {
            Database db = new SqlDatabase(EAMIDBConnection.GetConnectionString());
            DataSet ds = null;
            try
            {
                using (DbConnection dbConn = db.CreateConnection())
                {
                    dbConn.Open();
                    ds = db.ExecuteDataSet(DbStoredProcs.spGetPymtOnHoldNotification, daysPassed);
                }
            }
            catch (Exception ex)
            {
                EAMILogger.Instance.Error(ex);
            }
            return ds;
        }



        /// <summary>
        /// get data set of ecs records with 'Send-to-Sco' status over N days for user notification
        /// </summary>
        /// <param name="daysPassed"></param>
        /// <returns></returns>
        public static DataSet GetUnresolvedEcsForNotification(int daysPassed)
        {
            Database db = new SqlDatabase(EAMIDBConnection.GetConnectionString());
            DataSet ds = null;
            try
            {
                using (DbConnection dbConn = db.CreateConnection())
                {
                    dbConn.Open();
                    ds = db.ExecuteDataSet(DbStoredProcs.spGetUnresolvedEcsNotification, daysPassed);
                }
            }
            catch (Exception ex)
            {
                EAMILogger.Instance.Error(ex);
            }
            return ds;
        }

        public static CommonStatus InsertEFTInfo(int systemID, int peeId, string tranMsgId, string routingNumber, string accountType, string accountNo, DateTime datePrenoted)
        {
            CommonStatus cs = new CommonStatus(true);

            try
            { 
                Database db = new SqlDatabase(EAMIDBConnection.GetConnectionString());

                using (DbConnection dbConn = db.CreateConnection())
                {
                    dbConn.Open();

                    using (DbTransaction dbTran = dbConn.BeginTransaction())
                    {
                        db.ExecuteNonQuery(dbTran, DbStoredProcs.spInsertEFTInfo, DbStoredProcs.GetSPInsertEFTInfoParams(systemID, peeId, tranMsgId, routingNumber, accountType, accountNo, datePrenoted));
                        dbTran.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                EAMILogger.Instance.Error(ex);
                cs.Status = false;
                cs.IsFatal = true;
                cs.AddMessageDetail(ex.Message);                
            }

            return cs;
        }

        public static void UpdatePaymentRecordPmtMethodType(List<string> paymentSetNumberList, int paymentMethodTypeID)
        {
            List<int> paymentRecordIDList = GetPaymentRecordIDsByPaymentSetNumberList(paymentSetNumberList);
            string paymentRecordIDListString = String.Join(",", paymentRecordIDList);

            Database db = new SqlDatabase(EAMIDBConnection.GetConnectionString());
            try
            {
                using (DbConnection dbConn = db.CreateConnection())
                {
                    dbConn.Open();
                    db.ExecuteNonQuery(DbStoredProcs.spUpdatePaymentRecordPmtMethodType, DbStoredProcs.GetSPUpdatePaymentRecordPmtMethodTypeParams(paymentRecordIDListString, paymentMethodTypeID));
                }
            }
            catch (Exception ex)
            {
                EAMILogger.Instance.Error(ex);
                throw new Exception(ex.Message, ex.InnerException);
            }            
        }

        #endregion

        #region Private Methods

        private static List<int> GetPaymentRecordIDsByPaymentSetNumberList(List<string> paymentSetNumberList)
        {
            List<int> paymentRecordIDList = new List<int>();
            Database db = new SqlDatabase(EAMIDBConnection.GetConnectionString());
            try
            {
                // dbConn instance scope
                using (DbConnection dbConn = db.CreateConnection())
                {
                    dbConn.Open();
                    // persist request                
                    using (var reader = db.ExecuteReader(DbStoredProcs.spGetPaymentRecordIDsByPaymentSetNumbers, DbStoredProcs.GetSPGetPaymentRecordIDsByPaymentSetNumbersParams(String.Join(",", paymentSetNumberList))))
                    {
                        RefCodeTableList rctl = RefCodeDBMgr.GetRefCodeTableList();
                        while (reader.Read())
                        {
                            paymentRecordIDList.Add(int.Parse(reader["Payment_Record_ID"].ToString()));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                EAMILogger.Instance.Error(ex);
                paymentRecordIDList = null;
                throw new Exception("Error getting records from database");
            }
            return paymentRecordIDList;
        }


        /// <summary>
        /// gets payment records from database by PaymentStatusTypeId
        /// </summary>
        /// <returns></returns>
        private static List<PaymentRec> GetPaymentRecordsByStatus(int paymentStatusTypeId)
        {
            List<PaymentRec> paymentRecordList = new List<PaymentRec>();
            Database db = new SqlDatabase(EAMIDBConnection.GetConnectionString());
            try
            {
                // dbConn instance scope
                using (DbConnection dbConn = db.CreateConnection())
                {
                    dbConn.Open();
                    // persist request 

                    using (DataSet ds = db.ExecuteDataSet(DbStoredProcs.spGetPaymentRecordsByStatus, DbStoredProcs.GetSPGetPaymentRecordsByStatusParams(paymentStatusTypeId)))
                    {
                        RefCodeTableList rctl = RefCodeDBMgr.GetRefCodeTableList();
                        foreach (DataRow dataRow in ds.Tables[0].Rows)
                        {
                            PaymentRec pr = DataAccessSharedFunctions.GetPaymentRecFromDataSet(dataRow, rctl);
                            paymentRecordList.Add(pr);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                EAMILogger.Instance.Error(ex);
                paymentRecordList = null;
                throw new Exception("Error getting records from database");
            }
            return paymentRecordList;
        }


        /// <summary>
        /// gets payment records from database by PaymentStatusTypeId and TransactionID
        /// </summary>
        /// <returns></returns>
        private static List<PaymentRec> GetPaymentRecordsByStatusAndTransaction(int paymentStatusTypeId, string TransactionID)
        {
            List<PaymentRec> paymentRecordList = new List<PaymentRec>();
            Database db = new SqlDatabase(EAMIDBConnection.GetConnectionString());
            try
            {
                // dbConn instance scope
                using (DbConnection dbConn = db.CreateConnection())
                {
                    dbConn.Open();
                    // persist request 

                    using (DataSet ds = db.ExecuteDataSet(DbStoredProcs.spGetPaymentRecordsByStatusAndTransaction, DbStoredProcs.GetSPGetPaymentRecordsByStatusAndTransactionParams(paymentStatusTypeId, TransactionID)))
                    {
                        RefCodeTableList rctl = RefCodeDBMgr.GetRefCodeTableList();
                        foreach (DataRow dataRow in ds.Tables[0].Rows)
                        {
                            PaymentRec pr = DataAccessSharedFunctions.GetPaymentRecFromDataSet(dataRow, rctl);
                            paymentRecordList.Add(pr);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                EAMILogger.Instance.Error(ex);
                paymentRecordList = null;
                throw new Exception("Error getting records from database");
            }
            return paymentRecordList;
        }


        private static List<PaymentRec> GetPaymentRecordsByPaymentSetNumberList(List<string> paymentSetNumberList)
        {
            List<PaymentRec> paymentRecordList = new List<PaymentRec>();
            Database db = new SqlDatabase(EAMIDBConnection.GetConnectionString());
            try
            {
                // dbConn instance scope
                using (DbConnection dbConn = db.CreateConnection())
                {
                    dbConn.Open();
                    // persist request                
                    using (DataSet ds = db.ExecuteDataSet(DbStoredProcs.spGetPaymentRecordsByPaymentSetNumbers, DbStoredProcs.GetSPGetPaymentRecordsByPaymentSetNumbersParams(String.Join(",", paymentSetNumberList))))
                    {
                        RefCodeTableList rctl = RefCodeDBMgr.GetRefCodeTableList();
                        foreach (DataRow dataRow in ds.Tables[0].Rows)
                        {
                            PaymentRec pr = DataAccessSharedFunctions.GetPaymentRecFromDataSet(dataRow, rctl);
                            paymentRecordList.Add(pr);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                EAMILogger.Instance.Error(ex);
                paymentRecordList = null;
                throw new Exception("Error getting records from database");
            }
            return paymentRecordList;
        }

        /// <summary>
        /// gets assigned payment records from database by User
        /// </summary>
        /// <returns></returns>
        private static List<PaymentRec> GetPaymentRecordsAssignedByUser(int userID)
        {
            List<PaymentRec> paymentRecordList = new List<PaymentRec>();
            Database db = new SqlDatabase(EAMIDBConnection.GetConnectionString());
            try
            {
                // dbConn instance scope
                using (DbConnection dbConn = db.CreateConnection())
                {
                    dbConn.Open();
                    // persist request                
                    using (DataSet ds = db.ExecuteDataSet(DbStoredProcs.spGetPaymentRecordsAssignedByUser, DbStoredProcs.spGetPaymentRecordsAssignedByUserParams(userID)))
                    {
                        RefCodeTableList rctl = RefCodeDBMgr.GetRefCodeTableList();
                        foreach (DataRow dataRow in ds.Tables[0].Rows)
                        {
                            PaymentRec pr = DataAccessSharedFunctions.GetPaymentRecFromDataSet(dataRow, rctl);
                            paymentRecordList.Add(pr);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                EAMILogger.Instance.Error(ex);
                paymentRecordList = null;
                throw new Exception("Error getting records from database");
            }
            return paymentRecordList;
        }
                
                        
        private static CommonStatus ValidatePaymentStatus(Database db, DbTransaction dbTran, int paymentRecordID, RefCode statusType)
        {
            CommonStatus cs = new CommonStatus(true);

            // Validate the new Payment Status - Check if new status is appropriate to continue with
            using (var reader = db.ExecuteReader(dbTran, DbStoredProcs.spValidatePaymentStatus, DbStoredProcs.GetSPValidatePaymentStatusParams(paymentRecordID, statusType.ID)))
            {
                if (reader.Read())
                {
                    cs.Status = bool.Parse(reader[0].ToString());
                    if (!cs.Status)
                    {
                        //Capture message
                        cs.AddMessageDetail(reader[1].ToString());
                    }
                }
            }

            return cs;
        }

        #endregion
    }
}
