using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using OHC.EAMI.Common;
using OHC.EAMI.CommonEntity;
using OHC.EAMI.DataAccess.Shared;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace OHC.EAMI.DataAccess
{
    public class ClaimScheduleDataDbMgr : DataAccessBase
    {
        #region Public Methods

        public static CommonStatus SetClaimScheduleStatus(RefCode statusType, List<int> claimScheduleIDs, string user, string note)
        {
            CommonStatus cs = new CommonStatus(true);
            DateTime statusDate = DateTime.Now;
            Database db = new SqlDatabase(EAMIDBConnection.GetConnectionString());

            try
            {
                // dbConn instance scope
                using (DbConnection dbConn = db.CreateConnection())
                {
                    dbConn.Open();

                    // dbTran instance scope
                    using (DbTransaction dbTran = dbConn.BeginTransaction())
                    {
                        //UPDATE STATUS on a Payment Record
                        cs = InsertClaimScheduleStatus(db, dbTran, claimScheduleIDs, statusType, note, user, statusDate);

                        //ON SUCCESSFULL STATUS - COMMIT TRANSACTION
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

        /// <summary>
        /// persists Claim Schedule data
        /// </summary>
        /// <param name="rt"></param>
        /// <returns></returns>
        public static CommonStatus InsertClaimScheduleData(ClaimSchedule claimSchedule, string user)
        {
            CommonStatus cs = new CommonStatus(true);
            Database db = new SqlDatabase(EAMIDBConnection.GetConnectionString());
            List<int> paymentRecordIDList = claimSchedule.PaymentGroupList.SelectMany(_ => _.PaymentRecordList).Select(__ => __.PrimaryKeyID).ToList();

            try
            {
                // get a temp copy of ref codes
                RefCodeTableList rcTableList = RefCodeDBMgr.GetRefCodeTableList();
                RefCode cs_Created_StatusType = RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_CLAIM_SCHEDULE_STATUS_TYPE).Where(_ => _.Code == "CREATED").FirstOrDefault();
                RefCode cs_Assigned_StatusType = RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_CLAIM_SCHEDULE_STATUS_TYPE).Where(_ => _.Code == "ASSIGNED").FirstOrDefault();
                RefCode pr_StatusType_ADDED_TO_CS = RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_PAYMENT_STATUS_TYPE).Where(_ => _.Code == "ADDED_TO_CS").FirstOrDefault();

                if (claimSchedule != null)
                {
                    // dbConn instance scope
                    using (DbConnection dbConn = db.CreateConnection())
                    {
                        dbConn.Open();

                        // dbTran instance scope
                        int claimScheduleID = 0;
                        DateTime statusDate = DateTime.Now;

                        using (DbTransaction dbTran = dbConn.BeginTransaction())
                        {
                            //persist claim schedule 
                            using (var reader = db.ExecuteReader(dbTran, DbStoredProcs.spInsertClaimSchedule, DbStoredProcs.GetSPInsertClaimScheduleParams(claimSchedule, statusDate)))
                            {
                                if (reader.Read())
                                {
                                    int.TryParse(reader[0].ToString(), out claimScheduleID);
                                }
                            }

                            if (claimScheduleID != 0)
                            {
                                //UPDATE STATUS on a Payment Record
                                cs = InsertPaymentRecordStatus(db, dbTran, paymentRecordIDList, pr_StatusType_ADDED_TO_CS, string.Empty, user, statusDate);

                                if (cs.Status)
                                {
                                    //INSERT PAYMENT CLAIM SCHEDULE RECORD
                                    db.ExecuteNonQuery(dbTran, DbStoredProcs.spInsertPaymentClaimSchedule, DbStoredProcs.GetSPInsertPaymentClaimScheduleParams(String.Join(",", paymentRecordIDList), claimScheduleID));

                                    //INSERT STATUS - NEW
                                    db.ExecuteNonQuery(dbTran, DbStoredProcs.spInsertClaimScheduleStatus, DbStoredProcs.GetSPInsertClaimScheduleStatusParams(new EntityStatus { EntityID = claimScheduleID, StatusType = cs_Created_StatusType, StatusNote = string.Empty, CreatedBy = user, StatusDate = statusDate }));

                                    //INSERT STATUS - ASSIGNED
                                    db.ExecuteNonQuery(dbTran, DbStoredProcs.spInsertClaimScheduleStatus, DbStoredProcs.GetSPInsertClaimScheduleStatusParams(new EntityStatus { EntityID = claimScheduleID, StatusType = cs_Assigned_StatusType, StatusNote = string.Empty, CreatedBy = user, StatusDate = statusDate }));

                                    //INSERT USER ASSIGNMENintT
                                    db.ExecuteNonQuery(dbTran, DbStoredProcs.spInsertClaimScheduleUserAssignment, DbStoredProcs.GetSPInsertClaimScheduleUserAssignmentParams(claimScheduleID, (int)claimSchedule.AssignedUser.User_ID));

                                    //COMMIT TRANSACTION
                                    dbTran.Commit();
                                }
                            }
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

        public static CommonStatus InsertClaimScheduleListData(List<ClaimSchedule> claimScheduleList, string user)
        {
            CommonStatus cs = new CommonStatus(true);
            Database db = new SqlDatabase(EAMIDBConnection.GetConnectionString());
            DateTime statusDate = DateTime.Now;

            try
            {
                // get a temp copy of ref codes
                RefCodeTableList rcTableList = RefCodeDBMgr.GetRefCodeTableList();
                RefCode cs_Created_StatusType = RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_CLAIM_SCHEDULE_STATUS_TYPE).Where(_ => _.Code == "CREATED").FirstOrDefault();
                RefCode cs_Assigned_StatusType = RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_CLAIM_SCHEDULE_STATUS_TYPE).Where(_ => _.Code == "ASSIGNED").FirstOrDefault();
                RefCode pr_StatusType_ADDED_TO_CS = RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_PAYMENT_STATUS_TYPE).Where(_ => _.Code == "ADDED_TO_CS").FirstOrDefault();

                if (claimScheduleList != null)
                {
                    // dbConn instance scope
                    using (DbConnection dbConn = db.CreateConnection())
                    {
                        dbConn.Open();

                        using (DbTransaction dbTran = dbConn.BeginTransaction())
                        {
                            foreach (ClaimSchedule claimSchedule in claimScheduleList)
                            {
                                // dbTran instance scope
                                int claimScheduleID = 0;

                                //string priorityGroupHashList = String.Join(",", claimSchedule.PaymentGroupList.Select(_ => _.UniqueNumber));
                                List<int> paymentRecordIDList = claimSchedule.PaymentGroupList.SelectMany(_ => _.PaymentRecordList).Select(__ => __.PrimaryKeyID).ToList();
                                //string paymentRecordIDList = String.Join(",", paymentRecordIDList);

                                //persist claim schedule 
                                using (var reader = db.ExecuteReader(dbTran, DbStoredProcs.spInsertClaimSchedule, DbStoredProcs.GetSPInsertClaimScheduleParams(claimSchedule, statusDate)))
                                {
                                    if (reader.Read())
                                    {
                                        int.TryParse(reader[0].ToString(), out claimScheduleID);
                                    }
                                }

                                if (claimScheduleID != 0)
                                {
                                    //UPDATE STATUS on a Payment Record
                                    cs = InsertPaymentRecordStatus(db, dbTran, paymentRecordIDList, pr_StatusType_ADDED_TO_CS, string.Empty, user, statusDate);

                                    if (cs.Status)
                                    {
                                        //INSERT PAYMENT CLAIM SCHEDULE RECORD
                                        db.ExecuteNonQuery(dbTran, DbStoredProcs.spInsertPaymentClaimSchedule, DbStoredProcs.GetSPInsertPaymentClaimScheduleParams(String.Join(",", paymentRecordIDList), claimScheduleID));

                                        //INSERT STATUS - NEW
                                        db.ExecuteNonQuery(dbTran, DbStoredProcs.spInsertClaimScheduleStatus, DbStoredProcs.GetSPInsertClaimScheduleStatusParams(new EntityStatus { EntityID = claimScheduleID, StatusType = cs_Created_StatusType, StatusNote = string.Empty, CreatedBy = user, StatusDate = statusDate }));

                                        //INSERT STATUS - ASSIGNED
                                        db.ExecuteNonQuery(dbTran, DbStoredProcs.spInsertClaimScheduleStatus, DbStoredProcs.GetSPInsertClaimScheduleStatusParams(new EntityStatus { EntityID = claimScheduleID, StatusType = cs_Assigned_StatusType, StatusNote = string.Empty, CreatedBy = user, StatusDate = statusDate }));

                                        //INSERT USER ASSIGNMENintT
                                        db.ExecuteNonQuery(dbTran, DbStoredProcs.spInsertClaimScheduleUserAssignment, DbStoredProcs.GetSPInsertClaimScheduleUserAssignmentParams(claimScheduleID, (int)claimSchedule.AssignedUser.User_ID));
                                    }
                                }
                            }
                            //COMMIT TRANSACTION
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

        public static CommonStatus SetClaimSchedulesStatusToReturnToProcessor(List<int> claimScheduleIDs, string note, string user)
        {
            CommonStatus cs = new CommonStatus(false);
            Database db = new SqlDatabase(EAMIDBConnection.GetConnectionString());
            DateTime statusDate = DateTime.Now;

            try
            {
                RefCode statusType = RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_CLAIM_SCHEDULE_STATUS_TYPE).Where(_ => _.Code == "RETURN_TO_PROCESSOR").FirstOrDefault();

                // dbConn instance scope
                using (DbConnection dbConn = db.CreateConnection())
                {
                    dbConn.Open();

                    // dbTran instance scope
                    using (DbTransaction dbTran = dbConn.BeginTransaction())
                    {
                        //UPDATE STATUS on a Payment Record
                        cs = InsertClaimScheduleStatus(db, dbTran, claimScheduleIDs, statusType, note, user, statusDate);

                        //ON SUCCESSFULL STATUS 
                        if (cs.Status)
                        {
                            //COMMIT TRANSACTION
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



        public static CommonStatus SetClaimSchedulesStatusToApproved(List<int> claimScheduleIDs, List<ElectronicClaimSchedule> escList, string note, string user)
        {
            CommonStatus cs = new CommonStatus(false);
            Database db = new SqlDatabase(EAMIDBConnection.GetConnectionString());
            DateTime statusDate = DateTime.Now;

            try
            {
                RefCode statusType = RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_CLAIM_SCHEDULE_STATUS_TYPE).Where(_ => _.Code == "APPROVED").First();

                // dbConn instance scope
                using (DbConnection dbConn = db.CreateConnection())
                {
                    dbConn.Open();

                    // dbTran instance scope
                    using (DbTransaction dbTran = dbConn.BeginTransaction())
                    {
                        //UPDATE STATUS on a Payment Record
                        cs = InsertClaimScheduleStatus(db, dbTran, claimScheduleIDs, statusType, note, user, statusDate);

                        //ON SUCCESSFULL STATUS 
                        if (cs.Status)
                        {
                            //EXECUTE - UPDATE CS AMOUNT
                            db.ExecuteNonQuery(dbTran, DbStoredProcs.spUpdateClaimScheduleAmount, DbStoredProcs.GetSPUpdateClaimScheduleAmountParams(String.Join(",", claimScheduleIDs)));

                            //INSERT ECS
                            foreach (ElectronicClaimSchedule ecs in escList)
                            {
                                //EXECUTE
                                db.ExecuteNonQuery(dbTran, DbStoredProcs.spInsertElectronicClaimSchedule, DbStoredProcs.GetSPInsertEcsParams(ecs, statusDate));
                            }

                            //COMMIT TRANSACTION
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

        public static CommonStatus AddPaymentGroupsToClaimSchedule(ClaimSchedule claimSchedule, List<string> priorityGroupHashList, string loginUserName)
        {
            CommonStatus cs = new CommonStatus(true);
            Database db = new SqlDatabase(EAMIDBConnection.GetConnectionString());
            DateTime statusDate = DateTime.Now;

            try
            {
                RefCode pr_StatusType_ADDED_TO_CS = RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_PAYMENT_STATUS_TYPE).Where(_ => _.Code == "ADDED_TO_CS").FirstOrDefault();

                List<int> paymentRecordIDList = GetPaymentRecordIDsByHash(priorityGroupHashList);

                // dbConn instance scope
                using (DbConnection dbConn = db.CreateConnection())
                {
                    dbConn.Open();

                    // dbTran instance scope
                    using (DbTransaction dbTran = dbConn.BeginTransaction())
                    {
                        //UPDATE STATUS on a Payment Record
                        cs = InsertPaymentRecordStatus(db, dbTran, paymentRecordIDList, pr_StatusType_ADDED_TO_CS, string.Empty, loginUserName, statusDate);

                        if (cs.Status)
                        {
                            //INSERT PAYMENT CLAIM SCHEDULE RECORD
                            db.ExecuteNonQuery(dbTran, DbStoredProcs.spInsertPaymentClaimSchedule, DbStoredProcs.GetSPInsertPaymentClaimScheduleParams(String.Join(",", paymentRecordIDList), claimSchedule.PrimaryKeyID));

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

        public static CommonStatus RemovePaymentGroupsFromClaimSchedule(ClaimSchedule claimSchedule, List<string> priorityGroupHashList, string loginUserName)
        {
            CommonStatus cs = new CommonStatus(true);
            Database db = new SqlDatabase(EAMIDBConnection.GetConnectionString());
            DateTime statusDate = DateTime.Now;

            try
            {
                RefCode pr_StatusType_ASSIGNED = RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_PAYMENT_STATUS_TYPE).Where(_ => _.Code == "ASSIGNED").FirstOrDefault();
                List<int> paymentRecordIDList = GetPaymentRecordIDsByHash(priorityGroupHashList);

                // dbConn instance scope
                using (DbConnection dbConn = db.CreateConnection())
                {
                    dbConn.Open();

                    //GET CLAIM SCHEDULE LIST
                    string priorityGroupHashListParam = String.Join(",", priorityGroupHashList);
                    string claimScheduleListParam = String.Join(",", new List<int>() { claimSchedule.PrimaryKeyID });

                    // dbTran instance scope
                    using (DbTransaction dbTran = dbConn.BeginTransaction())
                    {
                        //UPDATE STATUS on a Payment Record
                        cs = InsertPaymentRecordStatus(db, dbTran, paymentRecordIDList, pr_StatusType_ASSIGNED, string.Empty, loginUserName, statusDate);

                        if (cs.Status)
                        {
                            //REMOVE PAYMENT CLAIM SCHEDULE RECORD
                            db.ExecuteNonQuery(dbTran, DbStoredProcs.spDeletePaymentClaimSchedule, DbStoredProcs.GetSPDeletePaymentClaimScheduleParams(priorityGroupHashListParam, claimScheduleListParam));

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

        public static CommonStatus DeleteClaimSchedules(List<int> claimScheduleIDList, string loginUserName)
        {
            CommonStatus cs = new CommonStatus(true);
            Database db = new SqlDatabase(EAMIDBConnection.GetConnectionString());
            DateTime statusDate = DateTime.Now;

            try
            {
                RefCode pr_StatusType_ASSIGNED = RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_PAYMENT_STATUS_TYPE).Where(_ => _.Code == "ASSIGNED").FirstOrDefault();

                // dbConn instance scope
                using (DbConnection dbConn = db.CreateConnection())
                {
                    dbConn.Open();

                    //GET CLAIM SCHEDULE LIST
                    List<ClaimSchedule> claimScheduleList = GetClaimSchedulesByID(claimScheduleIDList);
                    List<int> pamentRecordIDList = claimScheduleList.SelectMany(__ => __.PaymentGroupList.SelectMany(_ => _.PaymentRecordList).Select(___ => ___.PrimaryKeyID)).ToList();
                    string priorityGroupHashListParam = String.Join(",", claimScheduleList.SelectMany(__ => __.PaymentGroupList.Select(_ => _.UniqueNumber)).Distinct());
                    string claimScheduleListParam = String.Join(",", claimScheduleList.Select(_ => _.PrimaryKeyID));

                    // dbTran instance scope
                    using (DbTransaction dbTran = dbConn.BeginTransaction())
                    {
                        //UPDATE STATUS on a Payment Record
                        cs = InsertPaymentRecordStatus(db, dbTran, pamentRecordIDList, pr_StatusType_ASSIGNED, string.Empty, loginUserName, statusDate);

                        if (cs.Status)
                        {
                            //REMOVE PAYMENT CLAIM SCHEDULE RECORD
                            db.ExecuteNonQuery(dbTran, DbStoredProcs.spDeletePaymentClaimSchedule, DbStoredProcs.GetSPDeletePaymentClaimScheduleParams(priorityGroupHashListParam, claimScheduleListParam));

                            //DELETE CLAIM SCHEDULE RECORD
                            db.ExecuteNonQuery(dbTran, DbStoredProcs.spDeleteClaimSchedule, DbStoredProcs.GetSPDeleteClaimScheduleParams(claimScheduleListParam));

                            //COMMIT Transaction
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

        public static List<ClaimSchedule> GetClaimSchedulesByID(List<int> claimScheduleIDList)
        {
            List<ClaimSchedule> claimScheduleList;
            Database db = new SqlDatabase(EAMIDBConnection.GetConnectionString());

            try
            {
                // dbConn instance scope
                using (DbConnection dbConn = db.CreateConnection())
                {
                    dbConn.Open();

                    using (DataSet ds = db.ExecuteDataSet(DbStoredProcs.spGetClaimSchedulesByID, DbStoredProcs.GetSPGetClaimSchedulesByIDParams(String.Join(",", claimScheduleIDList))))
                    {
                        claimScheduleList = PopulateClaimScheduleListFromDataset(ds);
                    }
                }
            }
            catch (Exception ex)
            {
                EAMILogger.Instance.Error(ex);
                claimScheduleList = null;
                throw ex;
            }
            return claimScheduleList;
        }

           public static List<ClaimSchedule> GetClaimSchedulesByStatusType(List<int> statusTypeIDList)
        {
            List<ClaimSchedule> claimScheduleList;
            Database db = new SqlDatabase(EAMIDBConnection.GetConnectionString());

            try
            {
                // dbConn instance scope
                using (DbConnection dbConn = db.CreateConnection())
                {
                    dbConn.Open();

                    using (DataSet ds = db.ExecuteDataSet(DbStoredProcs.spGetClaimSchedulesByStatusType, DbStoredProcs.GetSPGetClaimSchedulesByStatusTypeParams(String.Join(",", statusTypeIDList))))
                    {
                        claimScheduleList = PopulateClaimScheduleListFromDataset(ds);
                    }
                }
            }
            catch (Exception ex)
            {
                EAMILogger.Instance.Error(ex);
                claimScheduleList = null;
                throw new Exception("Error getting records from database");
            }
            return claimScheduleList;
        }

        public static List<ClaimSchedule> GetClaimSchedulesByUser(int userID, List<int> statusTypeIDList)
        {
            List<ClaimSchedule> claimScheduleList;
            Database db = new SqlDatabase(EAMIDBConnection.GetConnectionString());

            try
            {
                // dbConn instance scope
                using (DbConnection dbConn = db.CreateConnection())
                {
                    dbConn.Open();

                    using (DataSet ds = db.ExecuteDataSet(DbStoredProcs.spGetClaimSchedulesByUser, DbStoredProcs.GetSPGetClaimSchedulesByUserParams(userID, String.Join(",", statusTypeIDList))))
                    {
                        claimScheduleList = PopulateClaimScheduleListFromDataset(ds);
                    }
                }
            }
            catch (Exception ex)
            {
                EAMILogger.Instance.Error(ex);
                claimScheduleList = null;
                throw new Exception("Error getting records from database");
            }
            return claimScheduleList;
        }

        public static CommonStatus SetRemittanceAdviceNote(int claimScheduleID, string note, string loginUserName)
        {
            CommonStatus cs = new CommonStatus(true);
            Database db = new SqlDatabase(EAMIDBConnection.GetConnectionString());

            try
            {
                // dbConn instance scope
                using (DbConnection dbConn = db.CreateConnection())
                {
                    dbConn.Open();

                    // dbTran instance scope
                    using (DbTransaction dbTran = dbConn.BeginTransaction())
                    {
                        //INSERT PAYMENT CLAIM SCHEDULE RECORD
                        db.ExecuteNonQuery(dbTran, DbStoredProcs.spInsertClaimScheduleRemitAdviceNote, DbStoredProcs.GetSPInsertClaimScheduleRemitAdviceNoteParams(claimScheduleID, note, loginUserName));

                        //COMMIT transaction
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

        public static CommonStatus DeleteRemittanceAdviceNote(int claimScheduleID)
        {
            CommonStatus cs = new CommonStatus(true);
            Database db = new SqlDatabase(EAMIDBConnection.GetConnectionString());

            try
            {
                // dbConn instance scope
                using (DbConnection dbConn = db.CreateConnection())
                {
                    dbConn.Open();

                    // dbTran instance scope
                    using (DbTransaction dbTran = dbConn.BeginTransaction())
                    {
                        //INSERT PAYMENT CLAIM SCHEDULE RECORD
                        db.ExecuteNonQuery(dbTran, DbStoredProcs.spDeleteClaimScheduleRemitAdviceNote, DbStoredProcs.GetSPDeleteClaimScheduleRemitAdviceNoteParams(claimScheduleID));

                        //COMMIT transaction
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

        /// <summary>
        /// updates ecs status type
        /// </summary>
        /// <param name="ecs"></param>
        public static CommonStatus UpdateElectronicClaimScheduleStatus(ElectronicClaimSchedule ecs, string user)
        {
            CommonStatus result = new CommonStatus(false);
            int retValue = 0;
            Database db = new SqlDatabase(EAMIDBConnection.GetConnectionString());
            bool updateCSStatus = false;
            int csStatusTypeId = 0;
            int prStatusTypeId = 0;
            DateTime statusDate = DateTime.Now;

            try
            {
                if (ecs != null)
                {
                    // dbConn instance scope
                    using (DbConnection dbConn = db.CreateConnection())
                    {
                        dbConn.Open();
                        using (DbTransaction dbTran = dbConn.BeginTransaction())
                        {
                            DbCommand dbCmd = db.GetStoredProcCommand("UpdateElectronicClaimScheduleStatus");
                            db.AddInParameter(dbCmd, "@EcsId", DbType.Int32, ecs.EcsId);
                            db.AddInParameter(dbCmd, "@EcsStatusTypeId", DbType.Int32, ecs.CurrentStatusType.ID);
                            db.AddInParameter(dbCmd, "@StatusNote", DbType.String, ecs.CurrentStatusNote);
                            db.AddInParameter(dbCmd, "@User", DbType.String, user);
                            db.AddInParameter(dbCmd, "@StatusDate", DbType.DateTime, statusDate);
                            db.AddOutParameter(dbCmd, "@Status", DbType.String, Int32.MaxValue); //output params from SP

                            //update ecs status
                            db.ExecuteNonQuery(dbCmd);

                            // post execute 
                            string resturnedStatus = Convert.ToString(db.GetParameterValue(dbCmd, "@Status"));                            
                            if (resturnedStatus == "OK")
                            {
                                if (ecs.CurrentStatusType.Code == "APPROVED")
                                {
                                    // Update CS Number (on ECS Approval)
                                    db.ExecuteNonQuery(dbTran, DbStoredProcs.spUpdateClaimScheduleNumber, DbStoredProcs.GetSPUpdateClaimScheduleNumberParams(ecs.EcsId));
                                }
                                else if (ecs.CurrentStatusType.Code == "SENT_TO_SCO")
                                {
                                    // Update CS DATE (on ECS SENT_TO_SCO)
                                    db.ExecuteNonQuery(dbTran, DbStoredProcs.spUpdateClaimScheduleDate, DbStoredProcs.GetSPUpdateClaimScheduleDateParams(ecs.EcsId, statusDate));

                                    // get a temp copy of ref codes
                                    RefCodeTableList rcTableList = RefCodeDBMgr.GetRefCodeTableList();
                                    csStatusTypeId = rcTableList.GetRefCodeListByTableName(enRefTables.TB_CLAIM_SCHEDULE_STATUS_TYPE).GetRefCodeByCode("SENT_TO_SCO").ID;
                                    prStatusTypeId = rcTableList.GetRefCodeListByTableName(enRefTables.TB_PAYMENT_STATUS_TYPE).GetRefCodeByCode("SENT_TO_SCO").ID;
                                    updateCSStatus = true;
                                }
                                else if (ecs.CurrentStatusType.Code == "WARRANT_RECEIVED")
                                {
                                    // get a temp copy of ref codes
                                    RefCodeTableList rcTableList = RefCodeDBMgr.GetRefCodeTableList();
                                    csStatusTypeId = rcTableList.GetRefCodeListByTableName(enRefTables.TB_CLAIM_SCHEDULE_STATUS_TYPE).GetRefCodeByCode("WARRANT_RECEIVED").ID;
                                    prStatusTypeId = rcTableList.GetRefCodeListByTableName(enRefTables.TB_PAYMENT_STATUS_TYPE).GetRefCodeByCode("WARRANT_RECEIVED").ID;
                                    updateCSStatus = true;
                                }
                                else if (ecs.CurrentStatusType.Code == "PENDING")
                                {
                                    RefCodeTableList rcTableList = RefCodeDBMgr.GetRefCodeTableList();
                                    csStatusTypeId = rcTableList.GetRefCodeListByTableName(enRefTables.TB_CLAIM_SCHEDULE_STATUS_TYPE).GetRefCodeByCode("APPROVED").ID;
                                    prStatusTypeId = rcTableList.GetRefCodeListByTableName(enRefTables.TB_PAYMENT_STATUS_TYPE).GetRefCodeByCode("ADDED_TO_CS").ID;
                                    updateCSStatus = true;
                                }

                                //UPDATE CS STATUS
                                if (updateCSStatus)
                                {
                                    // get cs ids
                                    List<int> idList = new List<int>();
                                    foreach (ClaimSchedule cs in ecs.ClaimScheduleList)
                                    {
                                        if (!idList.Contains(cs.PrimaryKeyID))
                                        {
                                            idList.Add(cs.PrimaryKeyID);
                                        }
                                    }

                                    // set appropriate status for all coresponding CS's and PR's
                                    db.ExecuteNonQuery(dbTran, DbStoredProcs.spInsertClaimScheduleStatusMultiple, DbStoredProcs.GetSPInsertMultipleClaimScheduleStatusesParams(idList, csStatusTypeId, prStatusTypeId, string.Empty, statusDate));
                                }

                                //COMMIT TRANSACTION
                                dbTran.Commit();
                                result.Status = true;
                            }
                            else
                            {
                                dbTran.Commit();
                                result.Status = false;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                EAMILogger.Instance.Error(ex);
                result.AddMessageDetail(ex.Message);
                result.IsFatal = true;
                throw new Exception("Error updating ecs,cs,[pr] status in db", ex);
            }

            return result;
        }


        public static List<ElectronicClaimSchedule> GetElectronicClaimSchedulesByDateRangeStatusType(int statusTypeID, DateTime? dateFrom, DateTime? dateTo)
        {
            List<ElectronicClaimSchedule> ecsList;
            Database db = new SqlDatabase(EAMIDBConnection.GetConnectionString());

            try
            {
                // dbConn instance scope
                using (DbConnection dbConn = db.CreateConnection())
                {
                    dbConn.Open();

                    using (DataSet ds = db.ExecuteDataSet(DbStoredProcs.spGetECSByDateRangeStatusType, DbStoredProcs.GetSPGetECSByDateRangeStatusTypeParams(statusTypeID, dateFrom, dateTo)))
                    {
                        ecsList = PopulateElectronicClaimScheduleListFromDataset(ds);
                    }
                }
            }
            catch (Exception ex)
            {
                EAMILogger.Instance.Error(ex);
                ecsList = null;
                throw new Exception("Error getting records from database");
            }
            return ecsList;
        }


        public static List<ElectronicClaimSchedule> GetECSAndCSByECSID(int ecsID, DateTime? dateFrom, DateTime? dateTo)
        {
            List<ElectronicClaimSchedule> ecsList;
            Database db = new SqlDatabase(EAMIDBConnection.GetConnectionString());

            try
            {
                // dbConn instance scope
                using (DbConnection dbConn = db.CreateConnection())
                {
                    dbConn.Open();

                    using (DataSet ds = db.ExecuteDataSet(DbStoredProcs.spGetECSAndCSByECSID, DbStoredProcs.GetSPGetECSAndCSByECSIDParams(ecsID, dateFrom, dateTo)))
                    {
                        ecsList = PopulateElectronicClaimScheduleListFromDataset(ds);
                    }
                }
            }
            catch (Exception ex)
            {
                EAMILogger.Instance.Error(ex);
                ecsList = null;
                throw new Exception("Error getting records from database");
            }
            return ecsList;
        }


        public static CommonStatus UpdateClaimScheduleAmount(List<int> claimScheduleIDList)
        {
            CommonStatus cs = new CommonStatus(true);
            Database db = new SqlDatabase(EAMIDBConnection.GetConnectionString());

            try
            {
                // dbConn instance scope
                using (DbConnection dbConn = db.CreateConnection())
                {
                    dbConn.Open();

                    // dbTran instance scope
                    using (DbTransaction dbTran = dbConn.BeginTransaction())
                    {
                        //EXECUTE
                        db.ExecuteNonQuery(dbTran, DbStoredProcs.spUpdateClaimScheduleAmount, DbStoredProcs.GetSPUpdateClaimScheduleAmountParams(String.Join(",", claimScheduleIDList)));

                        //COMMIT transaction
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


        /// <summary>
        /// gets funding summary based on list of provided CS ids
        /// </summary>
        /// <param name="csIdList"></param>
        /// <returns></returns>
        public static List<AggregatedFundingDetail> GetFundingSummaryCSIdList(List<int> csIdList)
        {
            List<AggregatedFundingDetail> afdList = new List<AggregatedFundingDetail>();
            Database db = new SqlDatabase(EAMIDBConnection.GetConnectionString());
            try
            {
                // dbConn instance scope
                using (DbConnection dbConn = db.CreateConnection())
                {
                    dbConn.Open();
                    using (var reader = db.ExecuteReader(DbStoredProcs.spGetFundingSummaryForMultipleCS, DbStoredProcs.GetSPGetFundingSummaryForMultipleCSParams(csIdList)))
                    {
                        while (reader.Read())
                        {
                            AggregatedFundingDetail afd = DataAccessSharedFunctions.GetAggregatedFundingDetailFromDataRow(reader);
                            afdList.Add(afd);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                EAMILogger.Instance.Error(ex);
                afdList = null;
                throw new Exception("Error getting records from database");
            }
            return afdList;
        }

        /// <summary>
        /// DeleteECS
        /// </summary>
        /// <param name="ecsID"></param>
        /// <param name="note"></param>
        /// <param name="loginUserName"></param>
        /// <returns></returns>
        public static CommonStatus DeleteECS(int ecsID, string note, string loginUserName)
        {
            CommonStatus cs = new CommonStatus(false);
            int retValue = 0;
            Database db = new SqlDatabase(EAMIDBConnection.GetConnectionString());
            DateTime statusDate = DateTime.Now;

            try
            {
                // dbConn instance scope
                using (DbConnection dbConn = db.CreateConnection())
                {
                    dbConn.Open();

                    using (DbTransaction dbTran = dbConn.BeginTransaction())
                    {
                        using (var reader = db.ExecuteReader(dbTran, DbStoredProcs.spDeleteECS, DbStoredProcs.GetSPDeleteECSParams(ecsID, note, loginUserName, statusDate)))
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

                        //COMMIT TRANSACTION
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
                cs.AddMessageDetail(ex.Message);
                cs.IsFatal = true;
            }

            return cs;
        }

        public static CommonStatus UpdateElectronicClaimScheduleFileName(int ecsID, string fileName, int fileLineCount)
        {
            CommonStatus cs = new CommonStatus(true);
            Database db = new SqlDatabase(EAMIDBConnection.GetConnectionString());

            try
            {
                // dbConn instance scope
                using (DbConnection dbConn = db.CreateConnection())
                {
                    dbConn.Open();

                    // dbTran instance scope
                    using (DbTransaction dbTran = dbConn.BeginTransaction())
                    {
                        //EXECUTE
                        db.ExecuteNonQuery(dbTran, DbStoredProcs.spUpdateElectronicClaimScheduleFileName, DbStoredProcs.GetSPUpdateElectronicClaimScheduleFileName(ecsID, fileName, fileLineCount));

                        //COMMIT transaction
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

        public static CommonStatus UpdateElectronicClaimScheduleTaskNumber(List<int> ecsIDList, string sentToScoTaskNumber, string warrantReceivedTaskNumber)
        {
            CommonStatus cs = new CommonStatus(true);
            Database db = new SqlDatabase(EAMIDBConnection.GetConnectionString());

            try
            {
                // dbConn instance scope
                using (DbConnection dbConn = db.CreateConnection())
                {
                    dbConn.Open();

                    // dbTran instance scope
                    using (DbTransaction dbTran = dbConn.BeginTransaction())
                    {
                        //EXECUTE
                        db.ExecuteNonQuery(dbTran, DbStoredProcs.spUpdateElectronicClaimScheduleTaskNumber, DbStoredProcs.GetSPUpdateElectronicClaimScheduleTaskNumber(String.Join(",", ecsIDList), sentToScoTaskNumber, warrantReceivedTaskNumber));

                        //COMMIT transaction
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

        public static string GetECSFileNameIncrement(string fileName, DateTime dateStamp)
        {
            string incrementValue = string.Empty;
            Database db = new SqlDatabase(EAMIDBConnection.GetConnectionString());

            try
            {
                // dbConn instance scope
                using (DbConnection dbConn = db.CreateConnection())
                {
                    dbConn.Open();

                    using (DataSet ds = db.ExecuteDataSet(DbStoredProcs.spGetECSFileNameIncrement, DbStoredProcs.GetSPGetECSFileNameIncrementParams(fileName, dateStamp)))
                    {
                        incrementValue = ds.Tables[0].Rows[0]["NEXT_INCREMENT_VALUE"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                EAMILogger.Instance.Error(ex);
            }

            return incrementValue;
        }

        public static CommonStatusPayload<ElectronicClaimSchedule> ReconcileECS(string ecsNumber, string dexFileName, DateTime dexFileReceiveDate, string warrantReceivedTaskNumber, List<WarrantRec> warrantRecList)
        {
            Database db = new SqlDatabase(EAMIDBConnection.GetConnectionString());
            ElectronicClaimSchedule ecs = new ElectronicClaimSchedule();
            CommonStatusPayload<ElectronicClaimSchedule> commonStatusPayload = new CommonStatusPayload<ElectronicClaimSchedule>(ecs, false);

            try
            {
                // dbConn instance scope
                using (DbConnection dbConn = db.CreateConnection())
                {
                    dbConn.Open();

                    // dbTran instance scope
                    using (DbTransaction dbTran = dbConn.BeginTransaction())
                    {
                        //RECONCILE ECS
                        using (DataSet ds = db.ExecuteDataSet(DbStoredProcs.spReconcileECS, DbStoredProcs.GetSpReconcileECSParams(ecsNumber, dexFileName, dexFileReceiveDate, warrantReceivedTaskNumber, String.Join(",", warrantRecList.Select(_ => _.SEQ_NUMBER)))))
                        {
                            int ecsID = 0;
                            int.TryParse(ds.Tables[0].Rows[0]["ECS_ID"].ToString(), out ecsID);
                            string errorMessage = ds.Tables[0].Rows[0]["Message"].ToString();

                            if (ecsID == 0)
                            {
                                //CAPTURE VALIDATION ERROR MESSAGE
                                commonStatusPayload.MessageDetailList.Add(errorMessage);
                            }
                            else if (ecsID > 0 && !string.IsNullOrEmpty(errorMessage))
                            {
                                //CAPTURE VALIDATION ERROR MESSAGE
                                commonStatusPayload.MessageDetailList.Add(errorMessage);

                                //CAPTURE ECS_ID
                                ecs.EcsId = ecsID;
                            }
                            else
                            {
                                //SUCCESS. CAPTURE ECS_ID
                                ecs.EcsId = ecsID;
                                ecs.ClaimScheduleList = new List<ClaimSchedule>();

                                //CAPTURE LIST of CS ID's
                                foreach (DataRow dr in ds.Tables[1].Rows)
                                {
                                    int csID = 0;
                                    int.TryParse(dr["CS_ID"].ToString(), out csID);
                                    ecs.ClaimScheduleList.Add(new ClaimSchedule() { PrimaryKeyID = csID });
                                }

                                //SET SUCCESS STATUS
                                commonStatusPayload.Status = true;
                            }
                        }

                        //RECONCILE CLAIM SCHEDULE
                        if (commonStatusPayload.Status)
                        {
                            foreach (WarrantRec wr in warrantRecList)
                            {
                                using (DataSet ds = db.ExecuteDataSet(dbTran, DbStoredProcs.spReconcileCS, DbStoredProcs.GetSpReconcileCSParams(ecs.EcsId, dexFileReceiveDate, wr)))
                                {
                                    if (string.IsNullOrEmpty(ds.Tables[0].Rows[0]["CS_ID"].ToString()))
                                    {
                                        //CAPTURE VALIDATION ERROR MESSAGE
                                        commonStatusPayload.MessageDetailList.Add(ds.Tables[0].Rows[0]["Message"].ToString());
                                        commonStatusPayload.Status = false;
                                        break;
                                    }
                                }
                            }
                        }

                        //COMMIT TRANS IF SUCCESS
                        if (commonStatusPayload.Status)
                        {
                            dbTran.Commit();
                        }
                        else
                        {
                            dbTran.Rollback();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                commonStatusPayload.AddMessageDetail(ex.Message);
                commonStatusPayload.IsFatal = true;
                EAMILogger.Instance.Error(ex);
            }

            return commonStatusPayload;
        }

        public static CommonStatusPayload<DataSet> GetRemittanceAdviceDataByCSID(int csId, int systemID)
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

                    using (DataSet ds = db.ExecuteDataSet(DbStoredProcs.spGetRemittanceAdviceDataByCSID, DbStoredProcs.GetSPGetRemittanceAdviceDataByCSIDParams(csId, systemID)))
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

        public static CommonStatus UpdateElectronicClaimScheduleDexFileName(int ecsID, string dexFileName)
        {
            CommonStatus cs = new CommonStatus(true);
            Database db = new SqlDatabase(EAMIDBConnection.GetConnectionString());

            try
            {
                // dbConn instance scope
                using (DbConnection dbConn = db.CreateConnection())
                {
                    dbConn.Open();

                    // dbTran instance scope
                    using (DbTransaction dbTran = dbConn.BeginTransaction())
                    {
                        //EXECUTE
                        db.ExecuteNonQuery(dbTran, DbStoredProcs.spUpdateElectronicClaimScheduleDexFileName, DbStoredProcs.GetSPUpdateElectronicClaimScheduleDexFileName(ecsID, dexFileName));

                        //COMMIT transaction
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

        public static CommonStatusPayload<List<string>> GetDexFileNameList(List<string> dexFileNameList)
        {
            Database db = new SqlDatabase(EAMIDBConnection.GetConnectionString());
            DataSet dataSet = new DataSet();
            CommonStatus status = new CommonStatus(true);
            List<string> nonexistentDexFileNameList = new List<string>();

            try
            {
                // dbConn instance scope
                using (DbConnection dbConn = db.CreateConnection())
                {
                    dbConn.Open();

                    using (DataSet ds = db.ExecuteDataSet(DbStoredProcs.spGetDexFileNameList, DbStoredProcs.GetSPGetDexFileNameListParams(String.Join(",", dexFileNameList))))
                    {
                        foreach (DataRow dataRow in ds.Tables[0].Rows)
                        {
                            nonexistentDexFileNameList.Add(dataRow["DexFileName"].ToString());
                        }
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

            return new CommonStatusPayload<List<string>>(nonexistentDexFileNameList, status.Status, status.IsFatal, status.GetCombinedMessage());
        }

        public static List<DateTime> GetAvailableAuditFilePayDates()
        {
            Database db = new SqlDatabase(EAMIDBConnection.GetConnectionString());
            DataSet dataSet = new DataSet();
            CommonStatus status = new CommonStatus(true);
            List<DateTime> availableAuditFilePayDates = new List<DateTime>();

            try
            {
                // dbConn instance scope
                using (DbConnection dbConn = db.CreateConnection())
                {
                    dbConn.Open();

                    using (DataSet ds = db.ExecuteDataSet(DbStoredProcs.spGetAvailableAuditFilePayDates, DbStoredProcs.GetSPGetAvailableAuditFilePayDatesParams()))
                    {
                        foreach (DataRow dataRow in ds.Tables[0].Rows)
                        {
                            availableAuditFilePayDates.Add(DateTime.Parse(dataRow["PayDate"].ToString()));
                        }
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

            return availableAuditFilePayDates;
        }

        public static CommonStatus InsertAuditFileData(string fileName, long fileSize, DateTime payDate, string taskNumber, bool hasError, DateTime createDate, DateTime? uploadDate)
        {
            CommonStatus cs = new CommonStatus(true);
            Database db = new SqlDatabase(EAMIDBConnection.GetConnectionString());

            try
            {
                // dbConn instance scope
                using (DbConnection dbConn = db.CreateConnection())
                {
                    dbConn.Open();

                    // dbTran instance scope
                    using (DbTransaction dbTran = dbConn.BeginTransaction())
                    {
                        //EXECUTE
                        db.ExecuteNonQuery(dbTran, DbStoredProcs.spInsertAuditFile, DbStoredProcs.GetSPInsertAuditFileParams(fileName, fileSize, payDate, taskNumber, hasError, createDate, uploadDate));

                        //COMMIT transaction
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

        public static List<Tuple<int, string, DateTime, DateTime>> GetAuditFilesForNotification()
        {
            Database db = new SqlDatabase(EAMIDBConnection.GetConnectionString());
            DataSet dataSet = new DataSet();
            CommonStatus status = new CommonStatus(true);
            List<Tuple<int, string, DateTime, DateTime>> auditFileList = new List<Tuple<int, string, DateTime, DateTime>>();

            try
            {
                // dbConn instance scope
                using (DbConnection dbConn = db.CreateConnection())
                {
                    dbConn.Open();

                    using (DataSet ds = db.ExecuteDataSet(DbStoredProcs.spGetAuditFilesForNotification, DbStoredProcs.GetSPGetAuditFileForNotificationParams()))
                    {
                        dataSet = ds;
                    }


                    using (var reader = db.ExecuteReader(DbStoredProcs.spGetAuditFilesForNotification, DbStoredProcs.GetSPGetAuditFileForNotificationParams()))
                    {
                        while (reader.Read())
                        {
                            auditFileList.Add(new Tuple<int, string, DateTime, DateTime>(int.Parse(reader["Audit_File_ID"].ToString())
                                , reader["Audit_File_Name"].ToString()
                                , DateTime.Parse(reader["PayDate"].ToString())
                                , DateTime.Parse(reader["UploadDate"].ToString())));
                        }
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

            return auditFileList;
        }

        public static CommonStatus UpdateAuditFileNotifiedDate(int auditFileID, DateTime notifiedDate)
        {
            CommonStatus cs = new CommonStatus(true);
            Database db = new SqlDatabase(EAMIDBConnection.GetConnectionString());

            try
            {
                // dbConn instance scope
                using (DbConnection dbConn = db.CreateConnection())
                {
                    dbConn.Open();

                    // dbTran instance scope
                    using (DbTransaction dbTran = dbConn.BeginTransaction())
                    {
                        //EXECUTE
                        db.ExecuteNonQuery(dbTran, DbStoredProcs.spUpdateAuditFileNotifiedDate, DbStoredProcs.GetSPUpdateAuditFilesNotifiedDateParams(auditFileID, notifiedDate));

                        //COMMIT transaction
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

        public static DataSet GetSCOFileProperty(int ecsFundId, int systemId, string pmtType, string env, bool isActive = true)
        {
            //string scoFilePropertyValue = string.Empty;
            //List<SCOFileSetting> scoFilePropertyList = new List<SCOFileSetting>();
            Database db = new SqlDatabase(EAMIDBConnection.GetConnectionString());
            DataSet dataSet = null;

            try
            {
                // dbConn instance scope
                using (DbConnection dbConn = db.CreateConnection())
                {
                    dbConn.Open();

                    using (DataSet ds = db.ExecuteDataSet(DbStoredProcs.spGetSCOFileProperty, DbStoredProcs.GetSPGetSCOFilePropertyParams(ecsFundId, systemId, pmtType, env, isActive)))
                    {
                        dataSet = ds;
                    }
                }
            }
            catch (Exception ex)
            {
                EAMILogger.Instance.Error(ex);
            }

            return dataSet;
        }

        #endregion

        #region Private Methods

        private static List<ClaimSchedule> PopulateClaimScheduleListFromDataset(DataSet ds)
        {
            List<ClaimSchedule> claimScheduleList = new List<ClaimSchedule>();
            RefCodeTableList rctl = RefCodeDBMgr.GetRefCodeTableList();

            foreach (DataRow claimSchRow in ds.Tables[0].Rows)
            {
                //GET CLAIM SCHEDULE from DATASET 1 
                ClaimSchedule cs = DataAccessSharedFunctions.GetClaimScheduleFromDataSet(claimSchRow, rctl);

                //GET PAYMENT RECORDS - Filter by Claim_Schedule_ID form DATASET 2
                string expression = string.Format("Claim_Schedule_ID = {0}", cs.PrimaryKeyID);
                List<PaymentRec> paymentRecList = new List<PaymentRec>();
                foreach (DataRow pmtRecRow in ds.Tables[1].Select(expression))
                {
                    //Populate Payment records
                    paymentRecList.Add(DataAccessSharedFunctions.GetPaymentRecFromDataSet(pmtRecRow, rctl));
                }

                //Create a group and ADD to Claim Schedule
                cs.PaymentGroupList = DataAccessSharedFunctions.GetPaymentGroupListFromPaymentRecList(paymentRecList);

                /* NOTE: We may need to load basic aggregated funding with each claim schedule
                 * as this is much quicker than to drilling down CS > PS > PR > FD
                 * Eugene S
                
                // GET AGGREGATE FUNDING DETAILS - Filter by Claim_Schedule_ID from DATASET 3
                cs.AggregatedFundingDetailList = new List<AggregatedFundingDetail>();
                foreach (DataRow afdRecRow in ds.Tables[2].Select(expression))
                {
                    // populate aggregate funding list
                    cs.AggregatedFundingDetailList.Add(DataAccessSharedFunctions.GetAggregatedFundingDetailFromDataRow(afdRecRow));
                }  
                */

                //Capture populated claim schedule
                claimScheduleList.Add(cs);
            }

            return claimScheduleList;
        }

        private static List<ElectronicClaimSchedule> PopulateElectronicClaimScheduleListFromDataset(DataSet ds)
        {
            List<ElectronicClaimSchedule> electronicClaimScheduleList = new List<ElectronicClaimSchedule>();
            RefCodeTableList rctl = RefCodeDBMgr.GetRefCodeTableList();

            foreach (DataRow ecsRow in ds.Tables[0].Rows)
            {
                //GET ELECTRONIC CLAIM SCHEDULE from DATASET 1 
                ElectronicClaimSchedule ecs = DataAccessSharedFunctions.GetElectronicClaimScheduleFromDataSet(ecsRow, rctl);

                //GET CLAIM SCHEDULES - Filter by ECS_ID form DATASET 2
                string expression = string.Format("ECS_ID = {0}", ecs.EcsId);
                List<ClaimSchedule> claimScheduleList = new List<ClaimSchedule>();
                foreach (DataRow csRow in ds.Tables[1].Select(expression))
                {
                    //Populate Claim Schedule
                    claimScheduleList.Add(DataAccessSharedFunctions.GetClaimScheduleFromDataSet(csRow, rctl));
                }

                // ADD Claim Schedule List to ECS
                ecs.ClaimScheduleList = claimScheduleList;

                //Capture populated ecs
                electronicClaimScheduleList.Add(ecs);
            }

            return electronicClaimScheduleList;
        }

        private static CommonStatus InsertPaymentRecordStatus(Database db, DbTransaction dbTran, List<int> paymentRecIDList, RefCode statusType, string statusNote, string loginUserName, DateTime statusDate)
        {
            CommonStatus cs = new CommonStatus(true);

            //UPDATE STATUS on a Payment Record
            foreach (int paymentRecordID in paymentRecIDList)
            {
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

                //If Valid status to continue
                if (cs.Status)
                {
                    //UPDATE PAYMENT REORD STATUS  
                    db.ExecuteNonQuery(dbTran, DbStoredProcs.spInsertPaymentStatus, DbStoredProcs.GetSPInsertPaymentStatusParams(new EntityStatus { EntityID = paymentRecordID, StatusType = statusType, StatusNote = statusNote, CreatedBy = loginUserName, StatusDate = statusDate }));
                }
                else break;
            }
            return cs;
        }

        private static CommonStatus InsertClaimScheduleStatus(Database db, DbTransaction dbTran, List<int> claimScheduleIDs, RefCode statusType, string statusNote, string loginUserName, DateTime statusDate)
        {
            CommonStatus cs = new CommonStatus(true);

            foreach (int claimScheduleID in claimScheduleIDs)
            {
                // Validate the new Payment Status - Check if new status is appropriate to continue with
                using (var reader = db.ExecuteReader(dbTran, DbStoredProcs.spValidateClaimScheduleStatus, DbStoredProcs.GetSPValidateClaimScheduleStatusParams(claimScheduleID, statusType.ID)))
                {
                    if (reader.Read())
                    {
                        if (!bool.Parse(reader[0].ToString()))
                        {
                            cs.Status = false;
                            //Capture message
                            cs.AddMessageDetail(reader[1].ToString());
                        }
                    }
                }

                //IF VALID, CONTINUE WITH NEW STATUS
                if (cs.Status)
                {
                    //INSERT CLAIM SCHEDULE NEW STATUS
                    db.ExecuteNonQuery(dbTran, DbStoredProcs.spInsertClaimScheduleStatus,
                        DbStoredProcs.GetSPInsertClaimScheduleStatusParams(new EntityStatus { EntityID = claimScheduleID, StatusType = statusType, StatusNote = statusNote, CreatedBy = loginUserName, StatusDate = statusDate }));
                }
                else break;
            }

            return cs;
        }

        private static List<int> GetPaymentRecordIDsByHash(List<string> paymentSetNumberList)
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

        #endregion
    }
}
