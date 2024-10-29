using OHC.EAMI.CommonEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OHC.EAMI.DataAccess
{
    internal class DbStoredProcs
    {
        #region SP Name variable

        // insert and other action stored procs
        internal static string spInsertRequest = "InsertRequest";
        internal static string spInsertResponse = "InsertResponse";
        internal static string spInsertTraceTrasaction = "InsertTraceTransaction";
        internal static string spInsertTracePayment = "InsertTracePayment";
        internal static string spInsertTransaction = "InsertTransaction";
        internal static string spInsertPaymentRecord = "InsertPaymentRecord";
        internal static string spInsertPaymentKvp = "InsertPaymentKvp";
        internal static string spInsertFundingDetail = "InsertFundingDetail";
        internal static string spInsertFundingDetailKvp = "InsertFundingDetailKvp";
        internal static string spInsertPaymentExchangeEntityInfo = "InsertPaymentExchangeEntityInfo";
        internal static string spInsertPaymentStatus = "InsertPaymentStatus";
        internal static string spInsertPaymentPaydate = "InsertPaymentPaydate";
        internal static string spInsertPaymentUserAssignment = "InsertPaymentUserAssignment";
        internal static string spInsertClaimSchedule = "InsertClaimSchedule";
        internal static string spInsertClaimScheduleStatus = "InsertClaimScheduleStatus";
        internal static string spInsertClaimScheduleUserAssignment = "InsertClaimScheduleUserAssignment";
        internal static string spInsertPaymentClaimSchedule = "InsertPaymentClaimSchedule";
        internal static string spValidateClaimScheduleStatus = "ValidateClaimScheduleStatus";
        internal static string spInsertClaimScheduleRemitAdviceNote = "InsertClaimScheduleRemitAdviceNote";
        internal static string spInsertScheduledTaskIntoQueue = "InsertScheduledTaskIntoQueue";
        internal static string spInsertElectronicClaimSchedule = "InsertElectronicClaimSchedule";
        internal static string spInsertClaimScheduleStatusMultiple = "InsertClaimScheduleStatusMultiple";
        internal static string spInsertEFTInfo = "InsertEFTInfo";
        internal static string spReconcileCS = "ReconcileCS";
        internal static string spReconcileECS = "ReconcileECS";
        internal static string spInsertAuditFile = "InsertAuditFile";

        // get sps
        internal static string spCheckDuplicatePaymentRecords = "CheckDuplicatePaymentRecords";
        internal static string spCheckDuplicatePaymentSetNumbers = "CheckDuplicatePaymentSetNumbers";
        internal static string spCheckDuplicateTransaction = "CheckDuplicateTransaction";
        internal static string spGetReferenceTableData = "GetReferenceTableData";
        internal static string spGetFundingDetailsForPaymentRec = "GetFundingDetailsForPaymentRec";
        internal static string spGetPaymentRecStatusList = "GetPaymentRecStatusList";
        internal static string spGetRejectedPaymentRecStatusList = "GetRejectedPaymentRecStatusList";
        internal static string spValidatePaymentStatus = "ValidatePaymentStatus";
        internal static string spGetPaymentRecordsByStatus = "GetPaymentRecordsByStatus";
        internal static string spGetPaymentRecordsAssignedByUser = "GetPaymentRecordsAssignedByUser";
        internal static string spGetPaymentRecordIDsByPaymentSetNumbers = "GetPaymentRecordIDsByPaymentSetNumbers";
        internal static string spGetPaymentRecStatusFlagsByClaimScheduleID = "GetPaymentRecStatusFlagsByClaimScheduleID";
        internal static string spGetClaimSchedulesByUser = "GetClaimSchedulesByUser";
        internal static string spGetClaimSchedulesByID = "GetClaimSchedulesByID";
        internal static string spGetPaymentRecordsByPaymentSetNumbers = "GetPaymentRecordsByPaymentSetNumbers";
        internal static string spGetClaimSchedulesByStatusType = "GetClaimSchedulesByStatusType";
        internal static string spGetScheduledTasksPendingExec = "GetScheduledTasksPendingExec";
        internal static string spGetTaskSchedule = "GetTaskSchedule";
        internal static string spGetUserListWithAssignedPaymentCount = "GetUserListWithAssignedPaymentCount";
        internal static string spGetPaymentRecordsByUser = "GetPaymentRecordsByUser";
        internal static string spGetScheduledTasksExpiredHungOrFaulted = "GetScheduledTasksExpiredHungOrFaulted";
        internal static string spGetEAMICounts = "GetEAMICounts";
        internal static string spGetECSByDateRangeStatusType = "GetECSByDateRangeStatusType";
        internal static string spGetFundingSummaryForMultipleCS = "GetFundingSummaryForMultipleCS";
        internal static string spGetPymtSubmissionTransForNotification = "GetPymtSubmissionTransForNotification";
        internal static string spGetECSFileNameIncrement = "GetECSFileNameIncrement";
        internal static string spGetRemittanceAdviceDataByCSID = "GetRemittanceAdviceDataByCSID";
        internal static string spGetPymtOnHoldNotification = "GetPymtOnHoldNotification";
        internal static string spGetDexFileNameList = "GetDexFileNameList";
        internal static string spGetUnresolvedEcsNotification = "GetUnresolvedEcsNotification";
        internal static string spCheckReceivedPaymentRecords = "CheckReceivedPaymentRecords";
        internal static string spGetEAMIUser = "usp_GetEAMIUser";
        internal static string spGetAllDataByType = "usp_GetAllDataByType";
        internal static string spGetGivenDataByType = "usp_GetGivenDataByType";
        //internal static string spGetPymtDataForEftCheckAndUserNotification = "GetPymtDataForEftCheckAndUserNotification";
        internal static string spGetPaymentRecordsByStatusAndTransaction = "GetPaymentRecordsByStatusAndTransaction";
        internal static string spGetAvailableAuditFilePayDates = "GetAvailableAuditFilePayDates";
        internal static string spGetAuditFilesForNotification = "GetAuditFilesForNotification";
        internal static string spGetDatasummaryReportData = "GetDataSummaryReportData";
        internal static string spGetECSAndCSByECSID = "GetECSAndCSByECSID";
        internal static string spGetFundList = "GetFundList";
        internal static string spGetFacesheetList = "GetFacesheetList";
        internal static string spGetExclusivePaymentTypeList = "GetExclusivePaymentTypeList";
        internal static string spGetFundingSourceList = "GetFundingSourceList";
        internal static string spGetSCOPropertyTypesList = "GetSCOPropertyTypes";
        internal static string spGetSCOPropertiesList = "GetSCOPropertiesList";
        internal static string spGetSCOPropertiesLookUp = "GetSCOPropertiesLookUp";
        internal static string spGetSCOFileProperty = "GetSCOFileProperty";

        // update sps
        internal static string spUpdateTransactionPaymentsStatus = "UpdateTransactionPaymentsStatus";
        internal static string spUpdateScheduledTaskExecutionStatus = "UpdateScheduledTaskExecutionStatus";
        internal static string spUpdateElectronicClaimScheduleStatus = "UpdateElectronicClaimScheduleStatus";
        internal static string spUpdateScheduledTaskSHEStatus = "UpdateScheduledTaskSHEStatus";
        internal static string spUpdateScheduledTaskSHEFNotificationCount = "UpdateScheduledTaskSHEFNotificationCount";
        internal static string spUpdateClaimScheduleAmount = "UpdateClaimScheduleAmount";
        internal static string spUpdateElectronicClaimScheduleFileName = "UpdateElectronicClaimScheduleFileName";
        internal static string spUpdateElectronicClaimScheduleTaskNumber = "UpdateElectronicClaimScheduleTaskNumber";
        internal static string spUpdateClaimScheduleNumber = "UpdateClaimScheduleNumber";
        internal static string spUpdateElectronicClaimScheduleDexFileName = "UpdateElectronicClaimScheduleDexFileName";
        internal static string spUpdateClaimScheduleDate = "UpdateClaimScheduleDate";
        internal static string spUpdateAuditFileNotifiedDate = "UpdateAuditFileNotifiedDate";
        internal static string spUpdatePaymentRecordPmtMethodType = "UpdatePaymentRecordPmtMethodType";

        //delete sps
        internal static string spDeleteClaimScheduleRemitAdviceNote = "DeleteClaimScheduleRemitAdviceNote";
        internal static string spDeletePaymentPaydate = "DeletePaymentPaydate";
        internal static string spDeletePaymentClaimSchedule = "DeletePaymentClaimSchedule";
        internal static string spDeleteClaimSchedule = "DeleteClaimSchedule";
        internal static string spDeleteECS = "DeleteElectronicClaimSchedule";
        internal static string spUpdatePymtSubmissionTransNotificationFlag = "UpdatePymtSubmissionTransNotificationFlag";

        //Reports
        internal static string spGetFaceSheetDataByECSID = "GetFaceSheetDataByECSID";
        internal static string spGetTransferLetterDataByECSID = "GetTransferLetterDataByECSID";
        internal static string spGetDrawSummaryReportData = "GetDrawSummaryReportData";
        internal static string spGetHoldReportData = "GetHoldReportData";
        internal static string spGetReturnToSORReportData = "GetReturnToSORReportData";
        internal static string spGetECSReportData = "GetECSReportData";
        internal static string spGetSTOReport = "GetSTOReport";

        #endregion

        #region Get SP Parameters

        internal static object[] GetSPInsertRequestParams(RequestTransaction rt)
        {
            return new object[]
            {
                rt.RequestReceivedTimeStamp,
                (rt.RequestSentTimeStamp == DateTime.MinValue) ? (object)DBNull.Value : (object)rt.RequestSentTimeStamp,
                rt.SenderID,
                rt.ReqMsgTransactionID,
                rt.ReqMsgTransactionType,
                rt.MsgTransactionVersion
            };
        }

        internal static object[] GetSPInsertResponseParams(RequestTransaction rt)
        {
            return new object[]
            {
                rt.RequestTransactionID,
                rt.RespMsgTransactionID,
                rt.RespMsgTransactionType,
                rt.ResponseTimeStamp,
                rt.RespStatusTypeID,
                rt.ResponseMessage
            };
        }

        internal static object[] GetSPInsertTraceTranParams(RequestTransaction rt)
        {
            return new object[]
            {
                rt.RequestTransactionID,                
                rt.MsgTotalRecAmount,
                rt.MsgTransactionRecCount,
                (rt.RejectedPaymentDateFrom == DateTime.MinValue) ? (object)DBNull.Value : (object)rt.RejectedPaymentDateFrom,
                (rt.RejectedPaymentDateTo == DateTime.MinValue) ? (object)DBNull.Value : (object)rt.RejectedPaymentDateTo
            };
        }
        
        internal static object[] GetSPInsertTracePaymentParams(PaymentRecTr pr)
        {
            PaymentStatus ps = pr.CurrentStatus as PaymentStatus;
            return new object[]
            {
                pr.TransactionID,
                ps.StatusType.ID,
                ps.StatusDate,
                ps.StatusNote,
                ps.ClaimScheduleNumber,
                (ps.ClaimScheduleDate == DateTime.MinValue) ? (object)DBNull.Value : (object)ps.ClaimScheduleDate,
                ps.WarrantNumber,
                (ps.WarrantDate == DateTime.MinValue) ? (object)DBNull.Value : (object)ps.WarrantDate,
                ps.WarrantAmount,
                pr.UniqueNumber,
                pr.PaymentRecNumberExt,
                pr.PaymentType,
                (pr.PaymentDate == DateTime.MinValue) ? (object)DBNull.Value : (object)pr.PaymentDate,
                pr.Amount,
                pr.FiscalYear,
                pr.IndexCode,
                pr.ObjDetailCode,
                pr.ObjAgencyCode,
                pr.PCACode,
                pr.ApprovedBy,
                pr.PaymentSetNumber,
                pr.PaymentSetNumberExt,
                pr.PayeeInfo.PEE.Code,
                pr.PayeeInfo.PEE.EntityIDType,
                pr.PayeeInfo.PEE_Name,
                pr.PayeeInfo.PEE_IdSfx,
                pr.PayeeInfo.PEE_AddressLine1,
                pr.PayeeInfo.PEE_AddressLine2,
                pr.PayeeInfo.PEE_AddressLine3,
                pr.PayeeInfo.PEE_City,
                pr.PayeeInfo.PEE_State,
                pr.PayeeInfo.PEE_Zip,
                pr.PayeeInfo.PEE.EntityEIN,
               // pr.PayeeInfo.PEE_VendorTypeCode,
                pr.TrPaymentKvpListXML,
                pr.TrFundingDetialListXML
            };            
        }

        internal static object[] GetSPInsertTransactionParams(RequestTransaction rt)
        {
            return new object[]
            {
                rt.RequestTransactionID,
                rt.ReqMsgTransactionID,
                rt.SOR_ID,
                rt.TransactionTypeId,
                rt.MsgTransactionVersion
            };
        }        

        internal static object[] GetSPInsertPaymentParams(PaymentRec pr)
        {
            return new object[]
            {
                pr.TransactionID,
                pr.UniqueNumber,
                pr.PaymentRecNumberExt,
                pr.PaymentSetNumber,
                pr.PaymentSetNumberExt,
                pr.PaymentType,
                pr.PaymentDate,
                pr.Amount,
                pr.FiscalYear,
                pr.IndexCode,
                pr.ObjDetailCode,
                pr.ObjAgencyCode,
                pr.PCACode,
                pr.ApprovedBy,
                pr.PayeeInfo.PEEInfo_PK_ID,
                pr.RPICode,
                pr.IsReportableRPI,
                pr.ContractNumber,
                pr.ContractDateFrom,
                pr.ContractDateTo,
                pr.ExclusivePaymentType.ID,
                pr.PaymentMethodType.ID
            };
        }

        internal static object[] GetSPInsertPayeeInfoParams(int sysId, PaymentExcEntityInfo peei)
        {
            return new object[]
            {
                sysId,
                peei.PEE.Code,
                peei.PEE.EntityIDType,
                peei.PEE_Name,
                //peei.PEE_IdSfx,
                peei.PEE.EntityEIN,
               // peei.PEE_VendorTypeCode,
                peei.PEE_ContractNumber,
                peei.PEE_AddressLine1,
                peei.PEE_AddressLine2,
                peei.PEE_AddressLine3,
                peei.PEE_City,
                peei.PEE_State,
                peei.PEE_Zip,
                true,
                0,
                DateTime.Now,
                "system",
                (object)DBNull.Value,
                (object)DBNull.Value
            };
        }

        internal static object[] GetSPInsertPaymentKVPParams(PaymentRec pr, KvpDefinition kvpDef, string kvpValue)
        {
            return new object[]
            {
                pr.PrimaryKeyID,
                kvpDef.ID,
                kvpValue
            };
        }

        internal static object[] GetSPInsertFundingDetailParams(PaymentFundingDetail pfd)
        {
            return new object[]
            {
                pfd.PaymentRecID,
                pfd.FundingSourceName,
                pfd.FFPAmount,
                pfd.SGFAmount,
                pfd.FiscalYear,
                pfd.FiscalQuarter,
                pfd.Title,
                pfd.FedFundCode,
                pfd.StateFundCode
            };
        }

        internal static object[] GetSPInsertFundingDetailKVPParams(PaymentFundingDetail pfd, KvpDefinition kvpDef, string kvpValue)
        {
            return new object[]
            {
                pfd.FundingDetailID,
                kvpDef.ID,
                kvpValue
            };
        }

        internal static object[] GetSPInsertPaymentStatusParams(EntityStatus es)
        {
            return new object[]
            {
                es.EntityID,
                es.StatusType.ID,
                es.StatusNote,
                es.CreatedBy,
                es.StatusDate
            };
        }

        internal static object[] GetSPValidatePaymentStatusParams(int paymentRecID, int satusTypeID)
        {
            return new object[]
            {
                paymentRecID,
                satusTypeID
            };
        }

        internal static object[] GetSPInsertPaymentPaydateParams(string paymentRecIDList, int paydateID)
        {
            return new object[]
            {
                paymentRecIDList,
                paydateID
            };
        }

        internal static object[] GetSPInsertPaymentUserAssignmentParams(int paymentRecID, int userID)
        {
            return new object[]
            {
                paymentRecID,
                userID
            };
        }

        internal static object[] GetSPDeletePaymentPaydateParams(string paymentRecIDList)
        {
            return new object[]
            {
                paymentRecIDList
            };
        }

        internal static object[] GetSPGetPaymentRecordsByStatusParams(int paymentStatusTypeID)
        {
            return new object[]
            {
                paymentStatusTypeID
            };
        }

        internal static object[] GetSPGetPaymentRecordsByStatusAndTransactionParams(int paymentStatusTypeID, string TransactionId)
        {
            return new object[]
           {
                paymentStatusTypeID,
                TransactionId
           };
        }

        internal static object[] spGetPaymentRecordsAssignedByUserParams(int userID)
        {
            return new object[]
            {
                userID
            };
        }

        internal static object[] GetSPInsertClaimScheduleParams(ClaimSchedule cs, DateTime statusDate)
        {
            return new object[]
            {
                cs.UniqueNumber,
                0,
                statusDate,
                cs.FiscalYear,
                cs.PaymentType,
                cs.ContractNumber,
                cs.ExclusivePaymentType.ID,
                cs.PayDate.ID,
                cs.PayeeInfo.PEEInfo_PK_ID,
                cs.IsLinked,
                cs.LinkedByPGNumber,
                (cs.PayeeInfo.IsEft) ? cs.PayeeInfo.PEE_EftInfo.EFTInfoID : (object)DBNull.Value,
                cs.PaymentMethodType.ID
            };
        }

        internal static object[] GetSPInsertClaimScheduleStatusParams(EntityStatus es)
        {
            return new object[]
            {
                es.EntityID,
                es.StatusType.ID,
                es.StatusNote,
                es.CreatedBy,
                es.StatusDate
            };
        }

        internal static object[] GetSPInsertClaimScheduleUserAssignmentParams(int claimScheduleID, int userID)
        {
            return new object[]
            {
                claimScheduleID,
                userID
            };
        }

        internal static object[] GetSPInsertPaymentClaimScheduleParams(string paymentRecordIDList, int claimScheduleID)
        {
            return new object[]
            {
                paymentRecordIDList,
                claimScheduleID
            };
        }

        internal static object[] GetSPGetPaymentRecordIDsByPaymentSetNumbersParams(string paymentSetNumberList)
        {
            return new object[]
            {
                paymentSetNumberList
            };
        }
        
        internal static object[] GetSPValidateClaimScheduleStatusParams(int claimScheduleId, int satusTypeID)
        {
            return new object[]
            {
                claimScheduleId,
                satusTypeID
            };
        }

        internal static object[] GetSPDeletePaymentClaimScheduleParams(string priorityGroupHashList, string claimScheduleIdList)
        {
            return new object[]
            {
                priorityGroupHashList,
                claimScheduleIdList
            };
        }

        internal static object[] GetSPDeleteClaimScheduleParams(string claimScheduleIdList)
        {
            return new object[]
            {
                claimScheduleIdList
            };
        }

        internal static object[] GetSPGetPaymentRecStatusFlagsByClaimScheduleIDParams(int claimScheduleID)
        {
            return new object[]
            {
                claimScheduleID
            };
        }

        internal static object[] GetSPGetClaimSchedulesByUserParams(int userID, string claimScheduleStatusTypeIDList)
        {
            return new object[]
            {
                userID,
                claimScheduleStatusTypeIDList
            };
        }

        internal static object[] GetSPGetClaimSchedulesByIDParams(string claimScheduleIDList)
        {
            return new object[]
            {
                claimScheduleIDList
            };
        }

        internal static object[] GetSPGetPaymentRecordsByPaymentSetNumbersParams(string paymentSetNumberList)
        {
            return new object[]
            {
                paymentSetNumberList
            };
        }

        internal static object[] GetSPGetClaimSchedulesByStatusTypeParams(string statusTypeIDList)
        {
            return new object[]
            {
                statusTypeIDList
            };
        }

        internal static object[] GetSPInsertClaimScheduleRemitAdviceNoteParams(int claimScheduleID, string note, string user)
        {
            return new object[]
            {
                claimScheduleID,
                note,
                DateTime.Now,
                user
            };
        }

        internal static object[] GetSPDeleteClaimScheduleRemitAdviceNoteParams(int claimScheduleID)
        {
            return new object[]
            {
                claimScheduleID
            };
        }

        internal static object[] GetSPGetUserListWithAssignedPaymentCountParams(int userID)
        {
            return new object[]
            {
                userID
            };
        }
        internal static object[] GetPaymentRecordsByUserParams(int userID)
        {
            return new object[]
            {
                userID
            };
        }

        internal static object[] GetSPGetEAMICountsParams()
        {
            return new object[]
            {                
            };
        }

        internal static object[] GetSPUpdateScheduledTaskExecutionStatusParams(int schedTaskId, int schedTaskExecStatusTypeId, string note, string outcome)
        {
            return new object[]
            {
                schedTaskId,
                schedTaskExecStatusTypeId,
                note,
                (string.IsNullOrWhiteSpace(outcome)) ? (object)DBNull.Value : outcome
            };
        }

        internal static object[] GetSPInsertEcsParams(ElectronicClaimSchedule ecs, DateTime statusDate)
        {
            List<string> idList = new List<string>();
            foreach(ClaimSchedule cs in ecs.ClaimScheduleList)
            {
                if(!idList.Contains(cs.PrimaryKeyID.ToString()))
                {
                    idList.Add(cs.PrimaryKeyID.ToString());
                }
            }
            string csIdList = String.Join(",", idList);
            return new object[]
            {
                csIdList,
                ecs.ExclusivePaymentType.ID,
                ecs.PayDate,
                ecs.Amount,
                ecs.CreatedBy,
                ecs.CurrentStatusNote,
                statusDate,
                ecs.PaymentMethodType.ID
            };
        }

        internal static object[] GetSPUpdateElectronicClaimScheduleStatus(int ecsId, int CurrentStatusTypeId, string statusNote, string user, DateTime statusDate)
        {
            return new object[]
            {
                ecsId,
                CurrentStatusTypeId,
                statusDate,
                statusNote,
                user
            };
        }

        internal static object[] GetSPInsertMultipleClaimScheduleStatusesParams(List<int> csIdList, int claimSchedStatusTypeId, int paymntRecStatusTypeId, string statusNote, DateTime statusDate)
        {
            List<string> list = csIdList.ConvertAll<string>(delegate(int i) { return i.ToString(); });
            string csIdListString = String.Join(",", list);
            return new object[]
            {
                csIdListString,
                claimSchedStatusTypeId,
                (paymntRecStatusTypeId == 0) ? (object)DBNull.Value : paymntRecStatusTypeId,
                statusNote,
                "system",
                statusDate
            };
        }

        internal static object[] GetSPGetECSByDateRangeStatusTypeParams(int statusTypeID, DateTime? dateFrom, DateTime? dateTo)
        {
            return new object[]
            {
                dateFrom,
                dateTo,
                statusTypeID
            };
        }

        internal static object[] GetSPGetECSAndCSByECSIDParams(int ecsID, DateTime? dateFrom, DateTime? dateTo)
        {
            return new object[]
            {
                dateFrom,
                dateTo,
                ecsID
            };
        }

        internal static object[] GetSPGetFaceSheetDataByECSIDParams(int ecsID, int systemID,bool isProdEnv)
        {
            return new object[]
            {
                ecsID
                , systemID
                ,isProdEnv
            };
        }

        internal static object[] GetSPUpdateClaimScheduleAmountParams(string claimScheduleIDList)
        {
            return new object[]
            {
                claimScheduleIDList
            };
        }

        internal static object[] GetSPGetFundingSummaryForMultipleCSParams(List<int> csIdList)
        {
            List<string> list = csIdList.ConvertAll<string>(delegate(int i) { return i.ToString(); });
            string csIdListString = String.Join(",", list);
            return new object[]
            {
                csIdListString                
            };
        }

        internal static object[] GetSPGetTransferLetterDataByECSIDParams(int ecsID, string userName)
        {
            return new object[]
            {
                ecsID,
                userName
            };
        }

        internal static object[] GetSPDeleteECSParams(int ecsID, string note, string user, DateTime statusDate)
        {
            return new object[]
            {
                ecsID,
                note,
                user,
                statusDate
            };
        }

        internal static object[] GetSPUpdateElectronicClaimScheduleFileName(int ecsId, string fileName, int fileLineCount)
        {
            return new object[]
            {
                ecsId,
                fileName,
                fileLineCount
            };
        }

        internal static object[] GetSPUpdateElectronicClaimScheduleTaskNumber(string ecsIdList, string sentToScoTaskNumber, string warrantReceivedTaskNumber)
        {
            return new object[]
            {
                ecsIdList,
                sentToScoTaskNumber,
                warrantReceivedTaskNumber
            };
        }

        internal static object[] GeSPGetDrawSummaryReportDataParams(DateTime payDate)
        {
            return new object[]
            {
                payDate
            };
        }

        internal static object[] GetSPGetHoldReportDataParams()
        {
            return new object[]
            {                
            };
        }

        internal static object[] GetSPGetReturnToSORReportDataParams(DateTime dateFrom, DateTime dateTo)
        {
            return new object[]
            {       
                dateFrom,
                dateTo         
            };
        }
        
        internal static object[] GetSPGetECSReportDataParams(DateTime dateFrom, DateTime dateTo)
        {
            return new object[]
            {       
                dateFrom,
                dateTo         
            };
        }

        internal static object[] GetSPGetESTOReportParams(DateTime payDate)
        {
            return new object[]
            {
                payDate
            };
        }

        internal static object[] GetSPGetECSFileNameIncrementParams(string fileName, DateTime dateStamp)
        {
            return new object[]
            {       
                fileName,
                dateStamp         
            };
        }


        internal static object[] GetSPGetRemittanceAdviceDataByCSIDParams(int csId, int systemID)
        {
            return new object[]
            {       
                csId  
                ,systemID
            };
        }

        internal static object[] GetSPUpdateClaimScheduleNumberParams(int ecsId)
        {
            return new object[]
            {
                ecsId
            };
        }

        internal static object[] GetSPUpdateClaimScheduleDateParams(int ecsId, DateTime dateStamp)
        {
            return new object[]
            {
                ecsId,
                dateStamp
            };
        }

        internal static object[] GetSpReconcileECSParams(string ecsNumber, string dexFileName, DateTime dexFileReceiveDate, string warrantReceivedTaskNumber, string seqNumberList)
        {
            return new object[]
            {
                ecsNumber,
                dexFileName,
                dexFileReceiveDate,
                warrantReceivedTaskNumber,
                seqNumberList
            };
        }

        internal static object[] GetSpReconcileCSParams(int ecsId, DateTime dexFileReceiveDate, WarrantRec wr)
        {
            return new object[]
            {
                ecsId,
                wr.SEQ_NUMBER,
                dexFileReceiveDate,
                wr.ISSUE_DATE,
                wr.WARRANT_NUMBER,
                wr.WARRANT_AMOUNT
            };
        }

        internal static object[] GetSPUpdateElectronicClaimScheduleDexFileName(int ecsId, string dexFileName)
        {
            return new object[]
            {
                ecsId,
                dexFileName
            };
        }

        internal static object[] GetSPGetDexFileNameListParams(string dexFileNameList)
        {
            return new object[]
            {
                dexFileNameList
            };
        }

        internal static object[] GetSPInsertEFTInfoParams(int systemID, int peeId, string transMsgId, string routingNumber, string accountType, string accountNo, DateTime datePrenoted)
        {
            return new object[]
            {
                systemID,
                peeId,
                transMsgId,
                routingNumber,
                accountType,
                accountNo,
                datePrenoted
            };
        }

        internal static object[] GetEAMIUserParams(string userName, string domainName = null, string password = null)
        {
            return new object[]
            {
                userName,
                password,
                domainName
            };
        }

        internal static object[] GetFundListParams(bool includeInactive, long systemID)
        {
            return new object[]
            {
                includeInactive,
                systemID
            };
        }        

        internal static object[] GetFacesheetListParams(bool includeInactive, long systemID)
        {
            return new object[]
            {
                includeInactive,
                systemID
            };
        }

        internal static object[] GetExclusivePmtTypeListParams(bool includeInactive,long systemID)
        {
            return new object[]
            {
                includeInactive
                ,systemID
            };
        }

        internal static object[] GetFundingSourceListParams(bool includeInactive, long systemID)
        {
            return new object[]
            {
                includeInactive
                ,systemID
            };
        }

        internal static object[] GetSCOPropertiesLookUp()
        {
            return new object[]
            {               
            };
        }

        internal static object[] GetSCOPropertiesListParams(bool includeInactive, long systemID)
        {
            return new object[]
            {
                includeInactive,
                systemID
            };
        }
        internal static object[] GetAllDataTypeParams(string dataType)
        {
            return new object[]
            {
                dataType
            };
        }
        internal static object[] GetGivenDataByTypeParams(string dataType, long DataTID)
        {
            return new object[]
            {
                dataType,
                DataTID
            };
        }
        
        internal static object[] GetSPGetPaymentRecStatusListParams(string recNumListString)
        {
            return new object[]
            {
                recNumListString
            };
        }

        internal static object[] GetSPGetAvailableAuditFilePayDatesParams()
        {
            return new object[]
            {
            };
        }

        internal static object[] GetSPGetDataSummaryReportDataParams(DateTime dateFrom, DateTime dateTo)
        {
            return new object[]
            {
                dateFrom,
                dateTo
            };
        }

        internal static object[] GetSPInsertAuditFileParams(string fileName, long fileSize, DateTime payDate, string taskNumber, bool hasError, DateTime createDate, DateTime? uploadDate)
        {
            return new object[]
            {
                fileName,
                fileSize,
                payDate,
                taskNumber,
                hasError,
                createDate,
                uploadDate
            };
        }

        internal static object[] GetSPGetAuditFileForNotificationParams()
        {
            return new object[]
            {
            };
        }

        internal static object[] GetSPUpdateAuditFilesNotifiedDateParams(int auditFileID, DateTime notifiedDate)
        {
            return new object[]
            {
                auditFileID,
                notifiedDate
            };
        }

        internal static object[] GetSPUpdatePaymentRecordPmtMethodTypeParams(string paymentRecordIDList, int paymentMethodTypeID)
        {
            return new object[]
            {
                paymentRecordIDList,
                paymentMethodTypeID
            };
        }

        internal static object[] GetSPGetSCOFilePropertyParams(int ecsFundId, int systemId, string pmtType, string env, bool isActive)
        {
            return new object[]
            {
                ecsFundId,
                systemId,
                pmtType,
                env,
                isActive
            };
        }

        #endregion
    }
}
