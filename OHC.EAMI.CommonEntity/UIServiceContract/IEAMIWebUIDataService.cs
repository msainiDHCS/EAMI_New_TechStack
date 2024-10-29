using System;
using System.Collections.Generic;
using System.Data;
using System.ServiceModel;
using System.ServiceModel.Web;
using OHC.EAMI.Common;

namespace OHC.EAMI.CommonEntity
{
    [ServiceContract]
    [ServiceKnownType(typeof(EAMIElement))]
    [ServiceKnownType(typeof(EAMIUser))]
    [ServiceKnownType(typeof(SCOSetting))]
    [ServiceKnownType(typeof(SCOSettingList))]
    [ServiceKnownType(typeof(SystemProperty))]
    [ServiceKnownType(typeof(SCOFileSetting))]
    [ServiceKnownType(typeof(ExclPmtType))]
    
    public interface IEAMIWebUIDataService
    {
        [OperationContract]
        [WebInvoke(Method = "*", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetEAMIDataElements?userName={userName}")]
        List<EAMIElement> GetEAMIDataElements(string userName);

        #region Authentication & Authorization

        [OperationContract]
        [WebInvoke(Method = "*", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetEAMIUser?userName={userName}&domainName={domainName}&password={password}&verifyPassword={verifyPassword}")]
        CommonStatusPayload<EAMIUser> GetEAMIUser(string userName, string domainName = null, string password = null, bool verifyPassword = false);

        [OperationContract]
        [WebInvoke(Method = "*", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetEAMIUserByID?userID={userID}")]
        CommonStatusPayload<EAMIUser> GetEAMIUserByID(long userID);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Xml, ResponseFormat = WebMessageFormat.Json, UriTemplate = "AddUpdateEAMIUser")]
        CommonStatus AddUpdateEAMIUser(EAMIUser inputUser, string loginUserName);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Xml, ResponseFormat = WebMessageFormat.Json, UriTemplate = "DeactivateEAMIUser")]
        CommonStatus DeactivateEAMIUser(EAMIUser inputUser, string loginUserName);

        [OperationContract]
        [WebInvoke(Method = "*", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetActivestatusEAMIUserInfo?userID={userID}")]
        CommonStatus GetActivestatusEAMIUserInfo(long userID);


        [OperationContract]
        [WebInvoke(Method = "*", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetEAMIAuthorizationLookUps?lookUpType={lookUpType}")]
        CommonStatusPayload<List<Tuple<string, EAMIAuthBase>>> GetEAMIAuthorizationLookUps(string lookUpType = null);

        [OperationContract]
        [WebInvoke(Method = "*", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "CheckEAMIUserValidity?userName={userName}&userType={userType}")]
        CommonStatus CheckEAMIUserValidity(string userName, long userType);

        [OperationContract]
        [WebInvoke(Method = "*", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetAllEAMIUsers")]
        CommonStatusPayload<List<EAMIUser>> GetAllEAMIUsers();

        #endregion

        #region Manage Master Data

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Xml, ResponseFormat = WebMessageFormat.Json, UriTemplate = "AddUpdateEAMIMasterData")]
        CommonStatus AddUpdateEAMIMasterData(EAMIMasterData inputData, string loginUserName);

        [OperationContract]
        [WebInvoke(Method = "*", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetAllEAMIMasterData?DataType={DataType}")]
        CommonStatusPayload<List<EAMIMasterData>> GetAllEAMIMasterData(string DataType);

        [OperationContract]
        [WebInvoke(Method = "*", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetEAMIMasterDataByID?DataType={DataType}&DataTID={DataTID}")]
        CommonStatusPayload<EAMIMasterData> GetEAMIMasterDataByID(string DataType, long DataTID);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Xml, ResponseFormat = WebMessageFormat.Json)]//, UriTemplate = "GetReferenceCode?refTableName={refTableName}"
        CommonStatusPayload<List<RefCodeList>> GetReferenceCodeList(params enRefTables[] refTableName);

        [OperationContract]
        [WebInvoke(Method = "*", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Xml, ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetRefCodeTableList")]
        CommonStatusPayload<RefCodeTableList> GetRefCodeTableList();

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Xml, ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetSCOSettingList")]
        SCOSettingList GetSCOSettingList();

        [OperationContract]
        [WebInvoke(Method = "*", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetEAMICounts")]
        CommonStatusPayload<List<Tuple<string, int>>> GetEAMICounts();

        #endregion

        #region Manage Pay Date Calendar

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Xml, ResponseFormat = WebMessageFormat.Json, UriTemplate = "UpdateYearlyCalendarEntry")]
        CommonStatus UpdateYearlyCalendarEntry(List<Tuple<EAMIDateType, EAMICalendarAction, string, DateTime>> dates, string loginUserName);

        [OperationContract]
        [WebInvoke(Method = "*", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetYearlyCalendarEntries?activeYear={activeYear}&loginUserName={loginUserName}")]
        CommonStatusPayload<List<Tuple<EAMIDateType, string, DateTime>>> GetYearlyCalendarEntries(int activeYear, string loginUserName);

        #endregion  

        #region Manage Payment Record Assignment

        [OperationContract]
        [WebInvoke(Method = "*", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetPaymentSuperGroupsForAssignment")]
        CommonStatusPayload<List<PaymentSuperGroup>> GetPaymentSuperGroupsForAssignment(bool assigned);

        [OperationContract]
        [WebInvoke(Method = "*", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetPaymentGroupsForAssignment")]
        CommonStatusPayload<List<PaymentGroup>> GetPaymentGroupsForAssignment(bool assigned);

        [OperationContract]
        [WebInvoke(Method = "*", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "SetPaymentGroupsToHold")]
        CommonStatus SetPaymentGroupsToHold(List<string> priorityGroupHashList, string user, string note);

        [OperationContract]
        [WebInvoke(Method = "*", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "SetPaymentGroupsToUnHold")]
        CommonStatus SetPaymentGroupsToUnHold(List<string> priorityGroupHashList, string user, string note);

        [OperationContract]
        [WebInvoke(Method = "*", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "SetPaymentGroupsToReturnToSup")]
        CommonStatus SetPaymentGroupsToReturnToSup(List<string> priorityGroupHashList, string user, string note);

        [OperationContract]
        [WebInvoke(Method = "*", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "SetPaymentGroupsToReleaseFromSup")]
        CommonStatus SetPaymentGroupsToReleaseFromSup(List<string> priorityGroupHashList, string user, string note);

        [OperationContract]
        [WebInvoke(Method = "*", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "AssignPaymentGroups")]
        CommonStatus AssignPaymentGroups(List<PaymentGroup> paymentGroupList, string user);

        [OperationContract]
        [WebInvoke(Method = "*", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "UnAssignPaymentGroups")]
        CommonStatus UnAssignPaymentGroups(List<PaymentGroup> paymentGroupList, string user);

        [OperationContract]
        [WebInvoke(Method = "*", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "ReAssignPaymentGroups")]
        CommonStatus ReAssignPaymentGroups(List<PaymentGroup> paymentGroupList, string user);

        [OperationContract]
        [WebInvoke(Method = "*", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetPaymentFundingDetails")]
        CommonStatusPayload<List<PaymentFundingDetail>> GetPaymentFundingDetails(int paymentRecordID);

        [OperationContract]
        [WebInvoke(Method = "*", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetPaymentGroupsAssignedByUser")]
        CommonStatusPayload<List<PaymentGroup>> GetPaymentGroupsAssignedByUser(int userID);

        [OperationContract]
        [WebInvoke(Method = "*", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetProcessorUserListWithAssignedPaymentCounts")]
        CommonStatusPayload<List<Tuple<EAMIUser, int>>> GetProcessorUserListWithAssignedPaymentCounts(int userID);

        [OperationContract]
        [WebInvoke(Method = "*", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetAllPaymentRecordsByUser")]
        CommonStatusPayload<List<PaymentRecForUser>> GetAllPaymentRecordsByUser(int userID);

        #endregion

        #region Manage Payment Record Processing

        /// <summary>
        /// Gets Assigned Super Groups For Adding To Claim Schedule
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="loginUsername"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "*", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetAssignedPaymentSuperGroupsForAddingToClaimSchedule")]
        CommonStatusPayload<List<PaymentSuperGroup>> GetAssignedPaymentSuperGroupsForAddingToClaimSchedule(int userID, string loginUsername);

        /// <summary>
        /// Sets Claim Schedules Status To Submit For Approval
        /// </summary>
        /// <param name="claimScheduleIDs"></param>
        /// <param name="note"></param>
        /// <param name="loginUserName"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "*", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "SetClaimSchedulesStatusToSubmitForApproval")]
        CommonStatus SetClaimSchedulesStatusToSubmitForApproval(List<int> claimScheduleIDs, string note, string loginUserName);

        /// <summary>
        /// Creates new Claim Schedule
        /// </summary>
        /// <param name="claimSchedule"></param>
        /// <param name="loginUserName"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "CreateNewClaimSchedule")]
        CommonStatus CreateNewClaimSchedule(List<string> priorityGroupHashList, int userID, string loginUserName);

        /// <summary>
        /// Adds Payment Groups to Claim Schedule
        /// </summary>
        /// <param name="claimScheduleID"></param>
        /// <param name="priorityGroupHashList"></param>
        /// <param name="loginUserName"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "*", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "AddPaymentGroupsToClaimSchedule")]
        CommonStatus AddPaymentGroupsToClaimSchedule(int claimScheduleID, List<string> priorityGroupHashList, string loginUserName);

        /// <summary>
        /// Removes Payment Groups from Claim Schedule
        /// </summary>
        /// <param name="claimScheduleID"></param>
        /// <param name="priorityGroupHashList"></param>
        /// <param name="loginUserName"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "*", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "RemovePaymentGroupsFromClaimSchedule")]
        CommonStatus RemovePaymentGroupsFromClaimSchedule(int claimScheduleID, List<string> priorityGroupHashList, string loginUserName);

        /// <summary>
        /// Deletes Claim Schedule
        /// </summary>
        /// <param name="claimScheduleID"></param>
        /// <param name="loginUserName"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "*", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "DeleteClaimSchedule")]
        CommonStatus DeleteClaimSchedule(int claimScheduleID, string loginUserName);

        /// <summary>
        /// Gets Claim Schedules For Processor By User
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="loginUsername"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "*", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetClaimSchedulesForProcessorByUser")]
        CommonStatusPayload<List<ClaimSchedule>> GetClaimSchedulesForProcessorByUser(int userID, string loginUsername);

        #endregion

        #region Manage Suppervisor Screen Processing

        /// <summary>
        /// 
        /// </summary>
        /// <param name="claimScheduleIDs"></param>
        /// <param name="note"></param>
        /// <param name="loginUserName"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "*", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "SetClaimSchedulesStatusToApproved")]
        CommonStatus SetClaimSchedulesStatusToApproved(List<int> claimScheduleIDs, string note, string loginUserName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="claimScheduleIDs"></param>
        /// <param name="note"></param>
        /// <param name="loginUserName"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "*", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "SetClaimSchedulesStatusToReturnToProcessor")]
        CommonStatus SetClaimSchedulesStatusToReturnToProcessor(List<int> claimScheduleIDs, string note, string loginUserName);

        /// <summary>
        /// Gets Claim Schedule list By Status Type
        /// </summary>
        /// <param name="claimScheduleStatusTypeID"></param>
        /// <param name="loginUsername"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "*", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetClaimSchedulesByStatusType")]
        CommonStatusPayload<List<ClaimSchedule>> GetClaimSchedulesByStatusType(List<int> claimScheduleStatusTypeIDList, string loginUsername);

        /// <summary>
        /// Adds new/Updates existing claim schedule remittance advice note
        /// </summary>
        /// <param name="claimScheduleID"></param>
        /// <param name="note"></param>
        /// <param name="loginUsername"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "*", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "SetRemittanceAdviceNote")]
        CommonStatus SetRemittanceAdviceNote(int claimScheduleID, string note, string loginUsername);

        /// <summary>
        /// Deletes existing claim schedule remittance advice note
        /// </summary>
        /// <param name="claimScheduleID"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "*", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "DeleteRemittanceAdviceNote")]
        CommonStatus DeleteRemittanceAdviceNote(int claimScheduleID);

        /// <summary>
        /// Sets ReturnPaymentGroups to ReturnToSystemOfRecord status
        /// </summary>
        /// <param name="priorityGroupHashList"></param>
        /// <param name="note"></param>
        /// <param name="loginUserName"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "*", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "ReturnPaymentGroupsToSystemOfRecord")]
        CommonStatus ReturnPaymentGroupsToSystemOfRecord(List<string> priorityGroupHashList, string user, string note);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <returns></returns>       
        [OperationContract]
        [WebInvoke(Method = "*", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetECSAndCSByECSID")]
        CommonStatusPayload<List<ElectronicClaimSchedule>> GetECSAndCSByECSID(DateTime dateFrom, DateTime dateTo, int ecsID);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <returns></returns>       
        [OperationContract]
        [WebInvoke(Method = "*", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetElectronicClaimSchedulesByDateRangeStatusType")]
        CommonStatusPayload<List<ElectronicClaimSchedule>> GetElectronicClaimSchedulesByDateRangeStatusType(DateTime dateFrom, DateTime dateTo, int statusTypeID);

        /// <summary>
        /// Gets a list of Payment Super Groups for a suppervisor screen
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "*", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetPaymentSuperGroupsForSupervisorScreen")]
        CommonStatusPayload<List<PaymentSuperGroup>> GetPaymentSuperGroupsForSupervisorScreen();

        /// <summary>
        /// Gets a list of Claim Schedules for a suppervisor Approval Screen
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "*", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetClaimSchedulesSubmettedForApproval")]
        CommonStatusPayload<List<ClaimSchedule>> GetClaimSchedulesSubmettedForApproval();

        /// <summary>
        /// Gets a list of ECS Funding Summary details
        /// </summary>
        /// <param name="csIdList"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "*", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetECSFundingSummary")]
        Common.CommonStatusPayload<List<AggregatedFundingDetail>> GetECSFundingSummary(List<int> csIdList);

        /// <summary>
        /// Sets ECS Status To Approved
        /// </summary>
        /// <param name="ecsID"></param>
        /// <param name="note"></param>
        /// <param name="loginUserName"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "*", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "SetECSStatusToApproved")]
        CommonStatus SetECSStatusToApproved(int ecsID, string note, string loginUserName);
        /// <summary>
        /// Sets ECS Status To Pending
        /// </summary>
        /// <param name="ecsID"></param>
        /// <param name="note"></param>
        /// <param name="loginUserName"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "*", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "SetECSStatusToPending")]
        CommonStatus SetECSStatusToPending(int ecsID, string note, string loginUserName);

        /// <summary>
        /// Delete ECS
        /// </summary>
        /// <param name="ecsID"></param>
        /// <param name="note"></param>
        /// <param name="loginUserName"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "*", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "DeleteECS")]
        CommonStatus DeleteECS(int ecsID, string note, string loginUserName);

        #endregion        

        #region Reports

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ecsID"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "*", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetFaceSheetDataByECSID")]
        CommonStatusPayload<DataSet> GetFaceSheetDataByECSID(int ecsID, int systemID,bool isProdEnv);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ecsID"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "*", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetTransferLetterDataByECSID")]
        CommonStatusPayload<DataSet> GetTransferLetterDataByECSID(int ecsID, string userName);

        /// <summary>
        /// Gets Draw Summary Report Data
        /// </summary>
        /// <param name="payDate"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "*", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetDrawSummaryReportData")]
        CommonStatusPayload<DataSet> GetDrawSummaryReportData(DateTime payDate);

        /// <summary>
        /// Gets Hold Report Data
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "*", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetHoldReportData")]
        CommonStatusPayload<DataSet> GetHoldReportData();

        /// <summary>
        /// Gets Return To SOR Report Data
        /// </summary>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "*", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetReturnToSORReportData")]
        CommonStatusPayload<DataSet> GetReturnToSORReportData(DateTime dateFrom, DateTime dateTo);

        /// <summary>
        /// GetECSReportData
        /// </summary>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "*", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetECSReportData")]
        CommonStatusPayload<DataSet> GetECSReportData(DateTime dateFrom, DateTime dateTo);

        /// <summary>
        /// GetESTOReport
        /// </summary>
        /// <param name="payDate"></param>        
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "*", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetESTOReport")]
        CommonStatusPayload<DataSet> GetESTOReport(DateTime payDate);

        /// <summary>
        /// GetRemittanceAdviceDataByCSID
        /// </summary>
        /// <param name="csID"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "*", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetRemittanceAdviceDataByCSID")]
        CommonStatusPayload<DataSet> GetRemittanceAdviceDataByCSID(int csID, int systemID);

        /// <summary>
        /// Gets Return To Data Summary Report Data
        /// </summary>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "*", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetDataSummaryReportData")]
        CommonStatusPayload<DataSet> GetDataSummaryReportData(DateTime dateFrom, DateTime dateTo);
        #endregion

        #region Manage System...

        #region Fund Information...

        /// <summary>
        /// Gets list of fund information...
        /// </summary>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <returns></returns>

        [OperationContract]
        [WebInvoke(Method = "*", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetAllFunds")]
        CommonStatusPayload<List<Fund>> GetAllFunds(bool includeInactive, long systemID);

        /// <summary>
        /// Add a new fund in the system...
        /// </summary>
        /// <param name="inputFund"></param>
        /// <param name="loginUserName"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Xml, ResponseFormat = WebMessageFormat.Json, UriTemplate = "AddEAMIFund")]
        CommonStatus AddEAMIFund(Fund inputFund);

        /// <summary>
        /// Update a fund...
        /// </summary>
        /// <param name="inputFund"></param>
        /// <param name="loginUserName"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Xml, ResponseFormat = WebMessageFormat.Json, UriTemplate = "UpdateEAMIFund")]
        CommonStatus UpdateEAMIFund(Fund inputFund);

        #endregion

        #region Facesheet Values...

        /// <summary>
        /// Gets list of FS information...
        /// </summary>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <returns></returns>

        [OperationContract]
        [WebInvoke(Method = "*", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetAllFacesheetValues")]
        CommonStatusPayload<List<FacesheetValues>> GetAllFacesheetValues(bool includeInactive, long systemID);

        /// <summary>
        /// Add a new FS values in the system...
        /// </summary>
        /// <param name="inputFund"></param>
        /// <param name="loginUserName"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Xml, ResponseFormat = WebMessageFormat.Json, UriTemplate = "AddFacesheetValues")]
        CommonStatus AddFacesheetValues(FacesheetValues inputFund);

        /// <summary>
        /// Update a Facesheet Values...
        /// </summary>
        /// <param name="inputFund"></param>
        /// <param name="loginUserName"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Xml, ResponseFormat = WebMessageFormat.Json, UriTemplate = "UpdateFacesheetValues")]
        CommonStatus UpdateFacesheetValues(FacesheetValues inputFund);

        #endregion

        #region Exclusive Payment Type Information...

        /// <summary>
        /// Gets list of Exclusive Payment Type information...
        /// </summary>
        /// <returns></returns>

        [OperationContract]
        [WebInvoke(Method = "*", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetAllExclusivePmtTypes")]
        CommonStatusPayload<List<ExclusivePmtType>> GetAllExclusivePmtTypes(bool includeInactive, long systemID);

        /// <summary>
        /// Add a new Exclusive Payment Type in the system...
        /// </summary>
        /// <param name="inputExclusivePmtType"></param>
        /// <param name="loginUserName"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Xml, ResponseFormat = WebMessageFormat.Json, UriTemplate = "AddEAMIExclusivePmtType")]
        CommonStatus AddEAMIExclusivePmtType(ExclusivePmtType inputExclusivePmtType);

        /// <summary>
        /// Update a Exclusive Payment Type...
        /// </summary>
        /// <param name="inputExclusivePmtType"></param>
        /// <param name="loginUserName"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Xml, ResponseFormat = WebMessageFormat.Json, UriTemplate = "UpdateEAMIExclusivePmtType")]
        CommonStatus UpdateEAMIExclusivePmtType(ExclusivePmtType inputExclusivePmtType);

        ///// <summary>
        ///// DElete a Exclusive Payment Type...
        ///// </summary>
        ///// <param name="inputExclusivePmtType"></param>
        ///// <param name="loginUserName"></param>
        ///// <returns></returns>
        //[OperationContract]
        //[WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Xml, ResponseFormat = WebMessageFormat.Json, UriTemplate = "DeleteEAMIExclusivePmtType")]
        //CommonStatus DeleteEAMIExclusivePmtType(ExclusivePmtType inputExclusivePmtType);


        //[OperationContract]
        //[WebInvoke(Method = "*", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetExclusivePmtTypeByID?exclusivePmtTypeID={exclusivePmtTypeID}")]
        //CommonStatusPayload<EAMIUser> GetExclusivePmtTypeByID(long exclusivePmtTypeID);

        #endregion

        #region Funding Source ...

        /// <summary>
        /// Gets list of Funding Source information...
        /// </summary>
        /// <param name="includeInactive"></param>
        /// <param name="systemID"></param>
        /// <returns></returns>

        [OperationContract]
        [WebInvoke(Method = "*", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetAllFundingSources")]
        CommonStatusPayload<List<FundingSource>> GetAllFundingSources(bool includeInactive, long systemID);

        /// <summary>
        /// Add a new Funding Source in the system...
        /// </summary>
        /// <param name="inputFundingSource"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Xml, ResponseFormat = WebMessageFormat.Json, UriTemplate = "AddEAMIFundingSource")]
        CommonStatus AddEAMIFundingSource(FundingSource inputFundingSource);

        /// <summary>
        /// Update a Funding Source...
        /// </summary>
        /// <param name="inputFundingSource"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Xml, ResponseFormat = WebMessageFormat.Json, UriTemplate = "UpdateEAMIFundingSource")]
        CommonStatus UpdateEAMIFundingSource(FundingSource inputFundingSource);

        ///// <summary>
        ///// Delete a Funding Source...
        ///// </summary>
        ///// <param name="inputFundingSource"></param>
        ///// <returns></returns>
        //[OperationContract]
        //[WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Xml, ResponseFormat = WebMessageFormat.Json, UriTemplate = "DeleteEAMIFundingSource")]
        //CommonStatus DeleteEAMIFundingSource(FundingSource inputFundingSource);

        #endregion

        #region SCO Properties...

        /// <summary>
        /// Gets list of SCO Properties
        /// </summary>
        /// <param name="includeInactive"></param>
        /// <param name="systemID"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "*", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetAllSCOProperties")]
        CommonStatusPayload<List<SCOProperty>> GetAllSCOProperties(bool includeInactive, long systemID);

        [OperationContract]
        [WebInvoke(Method = "*", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetAllSCOPropertyTypes")]
        CommonStatusPayload<SCOProperty> GetAllSCOPropertyTypes();

        /// <summary>
        /// Add a new fund in the system...
        /// </summary>
        /// <param name="inputSCOProperties"></param>
        /// <param name="loginUserName"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Xml, ResponseFormat = WebMessageFormat.Json, UriTemplate = "AddSCOProperties")]
        CommonStatus AddSCOProperties(SCOProperty inputSCOProperties);

        /// <summary>
        /// Update a fund...
        /// </summary>
        /// <param name="inputSCOProperties"></param>
        /// <param name="loginUserName"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Xml, ResponseFormat = WebMessageFormat.Json, UriTemplate = "UpdateSCOProperties")]
        CommonStatus UpdateSCOProperties(SCOProperty inputSCOProperties);

        #endregion

        #endregion
    }
}
