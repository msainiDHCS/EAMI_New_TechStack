using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using OHC.EAMI.Common;
using OHC.EAMI.CommonEntity;
using OHC.EAMI.WebUIServiceManager;

namespace OHC.EAMI.WebUIService
{
    public class EAMIWebUIDataService : IEAMIWebUIDataService
    {
        #region Administration

        #region EAMI User Setup...
        public List<EAMIElement> GetEAMIDataElements(string userName)
        {
            CheckAuthorization();
            using (EAMIWebUIDataServiceMgr acc = new EAMIWebUIDataServiceMgr())
            {
                return acc.GetEAMIDataElements(userName);
            }
        }

        public CommonStatusPayload<EAMIUser> GetEAMIUser(string userName, string domainName = null, string password = null, bool verifyPassword = false)
        {
            CheckAuthorization();
            using (EAMIWebUIDataServiceMgr acc = new EAMIWebUIDataServiceMgr())
            {
                return acc.GetEAMIUser(userName, domainName, password, verifyPassword);
            }
        }

        public CommonStatusPayload<EAMIUser> GetEAMIUserByID(long userID)
        {
            CheckAuthorization();
            using (EAMIWebUIDataServiceMgr acc = new EAMIWebUIDataServiceMgr())
            {
                return acc.GetEAMIUserByID(userID);
            }
        }

        public CommonStatus AddUpdateEAMIUser(EAMIUser inputUser, string loginUserName)
        {
            CheckAuthorization();
            using (EAMIWebUIDataServiceMgr acc = new EAMIWebUIDataServiceMgr())
            {
                return acc.AddUpdateEAMIUser(inputUser, loginUserName);
            }
        }

        /// <summary>
        /// Deactivate user if User is not in Active Directory
        /// </summary>
        /// <param name="inputUser"></param>  
        ///  <param name="loginUserName"></param>
        /// <returns></returns>
        public CommonStatus DeactivateEAMIUser(EAMIUser inputUser, string loginUserName)
        {
            CheckAuthorization();
            using (EAMIWebUIDataServiceMgr acc = new EAMIWebUIDataServiceMgr())
            {
                return acc.DeactivateEAMIUser(inputUser, loginUserName);
            }
        }
        /// <summary>
        /// Get Active status for EAMI User
        /// </summary>
        /// <param name="inputUser"></param>
        /// <returns></returns>
        public CommonStatus GetActivestatusEAMIUserInfo(long userID)
        {
            CheckAuthorization();
            using (EAMIWebUIDataServiceMgr acc = new EAMIWebUIDataServiceMgr())
            {
                return acc.GetActivestatusEAMIUserInfo(userID);
            }
        }

        public CommonStatusPayload<List<Tuple<string, EAMIAuthBase>>> GetEAMIAuthorizationLookUps(string lookUpType = null)
        {
            CheckAuthorization();
            using (EAMIWebUIDataServiceMgr acc = new EAMIWebUIDataServiceMgr())
            {
                return acc.GetEAMIAuthorizationLookUps(lookUpType);
            }
        }

        public CommonStatus CheckEAMIUserValidity(string userName, long userType)
        {
            CheckAuthorization();
            using (EAMIWebUIDataServiceMgr acc = new EAMIWebUIDataServiceMgr())
            {
                return acc.CheckEAMIUserValidity(userName, userType);
            }
        }

        public CommonStatusPayload<List<EAMIUser>> GetAllEAMIUsers()
        {
            CheckAuthorization();
            using (EAMIWebUIDataServiceMgr acc = new EAMIWebUIDataServiceMgr())
            {
                return acc.GetAllEAMIUsers();
            }
        }

        public CommonStatus AddUpdateEAMIMasterData(EAMIMasterData inputData, string loginUserName)
        {
            CheckAuthorization();
            using (EAMIWebUIDataServiceMgr acc = new EAMIWebUIDataServiceMgr())
            {
                return acc.AddUpdateEAMIMasterData(inputData, loginUserName);
            }
        }

        public CommonStatusPayload<List<EAMIMasterData>> GetAllEAMIMasterData(string DataType)
        {
            CheckAuthorization();
            using (EAMIWebUIDataServiceMgr acc = new EAMIWebUIDataServiceMgr())
            {
                return acc.GetAllEAMIMasterData(DataType);
            }
        }

        public CommonStatusPayload<EAMIMasterData> GetEAMIMasterDataByID(string DataType, long DataTID)
        {
            CheckAuthorization();
            using (EAMIWebUIDataServiceMgr acc = new EAMIWebUIDataServiceMgr())
            {
                return acc.GetEAMIMasterDataByID(DataType, DataTID);
            }
        }

        public CommonStatus UpdateYearlyCalendarEntry(List<Tuple<EAMIDateType, EAMICalendarAction, string, DateTime>> dates, string loginUserName)
        {
            CheckAuthorization();
            using (EAMIWebUIDataServiceMgr acc = new EAMIWebUIDataServiceMgr())
            {
                return acc.UpdateYearlyCalendarEntry(dates, loginUserName);
            }
        }

        public CommonStatusPayload<List<Tuple<EAMIDateType, string, DateTime>>> GetYearlyCalendarEntries(int activeYear, string loginUserName)
        {
            CheckAuthorization();
            using (EAMIWebUIDataServiceMgr acc = new EAMIWebUIDataServiceMgr())
            {
                return acc.GetYearlyCalendarEntries(activeYear, loginUserName);
            }
        }

        public CommonStatusPayload<List<RefCodeList>> GetReferenceCodeList(params enRefTables[] refTableName)
        {
            CheckAuthorization();
            using (EAMIWebUIDataServiceMgr acc = new EAMIWebUIDataServiceMgr())
            {
                return acc.GetReferenceCodeList(refTableName);
            }
        }

        public CommonStatusPayload<RefCodeTableList> GetRefCodeTableList()
        {
            CheckAuthorization();
            using (EAMIWebUIDataServiceMgr acc = new EAMIWebUIDataServiceMgr())
            {
                return acc.GetRefCodeTableList();
            }
        }


        public SCOSettingList GetSCOSettingList()
        {
            CheckAuthorization();
            using (EAMIWebUIDataServiceMgr acc = new EAMIWebUIDataServiceMgr())
            {
                return acc.GetSCOSettingList();
            }
        }

        public CommonStatusPayload<List<Tuple<string, int>>> GetEAMICounts()
        {
            CheckAuthorization();
            return new EAMIWebUIDataServiceMgr().GetEAMICounts();
        }

        public CommonStatusPayload<List<PaymentRecForUser>> GetAllPaymentRecordsByUser(int userID)
        {
            CheckAuthorization();
            return new EAMIWebUIDataServiceMgr().GetAllPaymentRecordsByUser(userID);
        }

        #endregion

        #region Manage System...

        #region Fund...

        /// <summary>
        /// Add a fund...
        /// </summary>
        /// <param name="inputFund"></param>
        /// <param name="loginUserName"></param>
        /// <returns></returns>
        public CommonStatus AddEAMIFund(Fund inputFund)
        {
            CheckAuthorization();
            using (EAMIWebUIDataServiceMgr fund = new EAMIWebUIDataServiceMgr())
            {
                return fund.AddEAMIFund(inputFund);
            }
        }

        /// <summary>
        /// Get all funds...
        /// </summary>
        /// <returns></returns>
        public CommonStatusPayload<List<Fund>> GetAllFunds(bool includeInactive, long systemID)
        {
            CheckAuthorization();
            return new EAMIWebUIDataServiceMgr().GetAllFunds(includeInactive, systemID);
        }

        /// <summary>
        /// Update a Fund...
        /// </summary>
        /// <param name="inputFund"></param>
        /// <param name="loginUserName"></param>
        /// <returns></returns>
        public CommonStatus UpdateEAMIFund(Fund inputFund)
        {
            CheckAuthorization();
            using (EAMIWebUIDataServiceMgr sm = new EAMIWebUIDataServiceMgr())
            {
                return sm.UpdateEAMIFund(inputFund);
            }
        }

        #endregion

        #region Facesheet...

        /// <summary>
        /// Add a fund...
        /// </summary>
        /// <param name="inputFund"></param>
        /// <param name="loginUserName"></param>
        /// <returns></returns>
        public CommonStatus AddFacesheetValues(FacesheetValues inputFS)
        {
            CheckAuthorization();
            using (EAMIWebUIDataServiceMgr fund = new EAMIWebUIDataServiceMgr())
            {
                return fund.AddFacesheetValues(inputFS);
            }
        }

        /// <summary>
        /// Get all funds...
        /// </summary>
        /// <returns></returns>
        public CommonStatusPayload<List<FacesheetValues>> GetAllFacesheetValues(bool includeInactive, long systemID)
        {
            CheckAuthorization();
            return new EAMIWebUIDataServiceMgr().GetAllFacesheetValues(includeInactive, systemID);
        }

        /// <summary>
        /// Update a Fund...
        /// </summary>
        /// <param name="inputFund"></param>
        /// <param name="loginUserName"></param>
        /// <returns></returns>
        public CommonStatus UpdateFacesheetValues(FacesheetValues inputFS)
        {
            CheckAuthorization();
            using (EAMIWebUIDataServiceMgr sm = new EAMIWebUIDataServiceMgr())
            {
                return sm.UpdateFacesheetValues(inputFS);
            }
        }

        #endregion

        #region Exclusive Payment Types

        /// <summary>
        /// Add an Exclusive Payment Type...
        /// </summary>
        /// <param name="inputExclusivePmtType"></param>
        /// <param name="loginUserName"></param>
        /// <returns></returns>
        public CommonStatus AddEAMIExclusivePmtType(ExclusivePmtType inputExclusivePmtType)
        {
            CheckAuthorization();
            using (EAMIWebUIDataServiceMgr exclusivePmtType = new EAMIWebUIDataServiceMgr())
            {
                return exclusivePmtType.AddEAMIExclusivePmtType(inputExclusivePmtType);
            }
        }

        /// <summary>
        /// Get all Exclusive Payment Types...
        /// </summary>
        /// <returns></returns>
        public CommonStatusPayload<List<ExclusivePmtType>> GetAllExclusivePmtTypes(bool includeInactive, long systemID)
        {
            CheckAuthorization();
            return new EAMIWebUIDataServiceMgr().GetAllExclusivePmtTypes(includeInactive, systemID);
        }

        /// <summary>
        /// Update a Fund...
        /// </summary>
        /// <param name="inputFund"></param>
        /// <param name="loginUserName"></param>
        /// <returns></returns>
        public CommonStatus UpdateEAMIExclusivePmtType(ExclusivePmtType inputExclusivePmtType)
        {
            CheckAuthorization();
            using (EAMIWebUIDataServiceMgr sm = new EAMIWebUIDataServiceMgr())
            {
                return sm.UpdateEAMIExclusivePmtType(inputExclusivePmtType);
            }
        }

        ///// <summary>
        ///// Delete a Fund...
        ///// </summary>
        ///// <param name="inputFund"></param>
        ///// <param name="loginUserName"></param>
        ///// <returns></returns>
        //public CommonStatus DeleteEAMIExclusivePmtType(ExclusivePmtType inputExclusivePmtType)
        //{
        //    CheckAuthorization();
        //    using (EAMIWebUIDataServiceMgr sm = new EAMIWebUIDataServiceMgr())
        //    {
        //        return sm.DeleteEAMIExclusivePmtType(inputExclusivePmtType);
        //    }
        //}

        #endregion

        #region Funding Source....

        /// <summary>
        /// Add a funding Source...
        /// </summary>
        /// <param name="inputFundingSource"></param>        
        /// <returns></returns>
        public CommonStatus AddEAMIFundingSource(FundingSource inputFundingSource)
        {
            CheckAuthorization();
            using (EAMIWebUIDataServiceMgr fundingSource = new EAMIWebUIDataServiceMgr())
            {
                return fundingSource.AddEAMIFundingSource(inputFundingSource);
            }
        }

        /// <summary>
        /// Get all funding Source...
        /// </summary>
        /// <returns></returns>
        public CommonStatusPayload<List<FundingSource>> GetAllFundingSources(bool includeInactive, long systemID)
        {
            CheckAuthorization();
            return new EAMIWebUIDataServiceMgr().GetAllFundingSources(includeInactive, systemID);
        }

        /// <summary>
        /// Update a Funding Source...
        /// </summary>
        /// <param name="inputFundingSource"></param>        
        /// <returns></returns>
        public CommonStatus UpdateEAMIFundingSource(FundingSource inputFundingSource)
        {
            CheckAuthorization();
            using (EAMIWebUIDataServiceMgr sm = new EAMIWebUIDataServiceMgr())
            {
                return sm.UpdateEAMIFundingSource(inputFundingSource);
            }
        }

        ///// <summary>
        ///// Delete a Funding Source...
        ///// </summary>
        ///// <param name="inputFundingSource"></param>       
        ///// <returns></returns>
        //public CommonStatus DeleteEAMIFundingSource(FundingSource inputFundingSource)
        //{
        //    CheckAuthorization();
        //    using (EAMIWebUIDataServiceMgr sm = new EAMIWebUIDataServiceMgr())
        //    {
        //        return sm.DeleteEAMIFundingSource(inputFundingSource);
        //    }
        //}

        #endregion

        #region SCO Properties...

        /// <summary>
        /// Add a SCO Properties...
        /// </summary>
        /// <param name="inputAddSCOProperties"></param>
        /// <param name="loginUserName"></param>
        /// <returns></returns>
        public CommonStatus AddSCOProperties(SCOProperty inputSCOProperties)
        {
            CheckAuthorization();
            using (EAMIWebUIDataServiceMgr scoPrpty = new EAMIWebUIDataServiceMgr())
            {
                return scoPrpty.AddSCOProperties(inputSCOProperties);
            }
        }

        /// <summary>
        /// Get all Add SCO Properties...
        /// </summary>
        /// <returns></returns>
        public CommonStatusPayload<List<SCOProperty>> GetAllSCOProperties(bool includeInactive, long systemID)
        {
            CheckAuthorization();
            return new EAMIWebUIDataServiceMgr().GetAllSCOProperties(includeInactive, systemID);
        }

        public CommonStatusPayload<SCOProperty> GetAllSCOPropertyTypes()
        {
            CheckAuthorization();
            var res = new EAMIWebUIDataServiceMgr().GetAllSCOPropertyTypes();
            return res;
        }

        /// <summary>
        /// Update a SCOProperty...
        /// </summary>
        /// <param name="inputAddSCOProperties"></param>
        /// <param name="loginUserName"></param>
        /// <returns></returns>
        public CommonStatus UpdateSCOProperties(SCOProperty prptyToBeUpdated)
        {
            CheckAuthorization();
            using (EAMIWebUIDataServiceMgr sm = new EAMIWebUIDataServiceMgr())
            {
                return sm.UpdateSCOProperties(prptyToBeUpdated);
            }
        }

        #endregion

        #endregion

        #endregion

        #region Payment Assignment

        public CommonStatusPayload<List<PaymentSuperGroup>> GetPaymentSuperGroupsForAssignment(bool assigned)
        {
            CheckAuthorization();
            return new EAMIWebUIDataServiceMgr().GetPaymentSuperGroupsForAssignment(assigned);
        }

        public CommonStatusPayload<List<PaymentGroup>> GetPaymentGroupsForAssignment(bool assigned)
        {
            CheckAuthorization();
            return new EAMIWebUIDataServiceMgr().GetPaymentGroupsForAssignment(assigned);
        }

        public CommonStatus SetPaymentGroupsToHold(List<string> priorityGroupHashList, string user, string note)
        {
            CheckAuthorization();
            return new EAMIWebUIDataServiceMgr().SetPaymentGroupsToHold(priorityGroupHashList, user, note);
        }

        public CommonStatus SetPaymentGroupsToUnHold(List<string> priorityGroupHashList, string user, string note)
        {
            CheckAuthorization();
            return new EAMIWebUIDataServiceMgr().SetPaymentGroupsToUnHold(priorityGroupHashList, user, note);
        }

        public CommonStatus SetPaymentGroupsToReturnToSup(List<string> priorityGroupHashList, string user, string note)
        {
            CheckAuthorization();
            return new EAMIWebUIDataServiceMgr().SetPaymentGroupsToReturnToSup(priorityGroupHashList, user, note);
        }

        public CommonStatus SetPaymentGroupsToReleaseFromSup(List<string> priorityGroupHashList, string user, string note)
        {
            CheckAuthorization();
            return new EAMIWebUIDataServiceMgr().SetPaymentGroupsToReleaseFromSup(priorityGroupHashList, user, note);
        }

        public CommonStatus AssignPaymentGroups(List<PaymentGroup> paymentGroupList, string user)
        {
            CheckAuthorization();
            return new EAMIWebUIDataServiceMgr().AssignPaymentGroups(paymentGroupList, user);
        }

        public CommonStatus UnAssignPaymentGroups(List<PaymentGroup> paymentGroupList, string user)
        {
            CheckAuthorization();
            return new EAMIWebUIDataServiceMgr().UnAssignPaymentGroups(paymentGroupList, user);
        }

        public CommonStatus ReAssignPaymentGroups(List<PaymentGroup> paymentGroupList, string user)
        {
            CheckAuthorization();
            return new EAMIWebUIDataServiceMgr().ReAssignPaymentGroups(paymentGroupList, user);
        }

        public CommonStatusPayload<List<PaymentFundingDetail>> GetPaymentFundingDetails(int paymentRecordID)
        {
            CheckAuthorization();
            return new EAMIWebUIDataServiceMgr().GetPaymentFundingDetails(paymentRecordID);
        }

        public CommonStatusPayload<List<PaymentGroup>> GetPaymentGroupsAssignedByUser(int userID)
        {
            CheckAuthorization();
            return new EAMIWebUIDataServiceMgr().GetPaymentGroupsAssignedByUser(userID);
        }

        public CommonStatusPayload<List<Tuple<EAMIUser, int>>> GetProcessorUserListWithAssignedPaymentCounts(int userID)
        {
            CheckAuthorization();
            return new EAMIWebUIDataServiceMgr().GetProcessorUserListWithAssignedPaymentCounts(userID);
        }

        #endregion

        #region Payment Processing

        public CommonStatusPayload<List<PaymentSuperGroup>> GetAssignedPaymentSuperGroupsForAddingToClaimSchedule(int userID, string loginUsername)
        {
            CheckAuthorization();
            return new EAMIWebUIDataServiceMgr().GetAssignedPaymentSuperGroupsForAddingToClaimSchedule(userID, loginUsername);
        }

        public CommonStatus SetClaimSchedulesStatusToSubmitForApproval(List<int> claimScheduleIDs, string note, string loginUserName)
        {
            CheckAuthorization();
            return new EAMIWebUIDataServiceMgr().SetClaimSchedulesStatusToSubmitForApproval(claimScheduleIDs, note, loginUserName);
        }

        public CommonStatus CreateNewClaimSchedule(List<string> priorityGroupHashList, int userID, string loginUserName)
        {
            CheckAuthorization();
            return new EAMIWebUIDataServiceMgr().CreateNewClaimSchedule(priorityGroupHashList, userID, loginUserName);
        }

        public CommonStatus AddPaymentGroupsToClaimSchedule(int claimScheduleID, List<string> priorityGroupHashList, string loginUserName)
        {
            CheckAuthorization();
            return new EAMIWebUIDataServiceMgr().AddPaymentGroupsToClaimSchedule(claimScheduleID, priorityGroupHashList, loginUserName);
        }

        public CommonStatus RemovePaymentGroupsFromClaimSchedule(int claimScheduleID, List<string> priorityGroupHashList, string loginUserName)
        {
            CheckAuthorization();
            return new EAMIWebUIDataServiceMgr().RemovePaymentGroupsFromClaimSchedule(claimScheduleID, priorityGroupHashList, loginUserName);
        }

        public CommonStatus DeleteClaimSchedule(int claimScheduleID, string loginUserName)
        {
            CheckAuthorization();
            return new EAMIWebUIDataServiceMgr().DeleteClaimSchedule(claimScheduleID, loginUserName);
        }

        public CommonStatusPayload<List<ClaimSchedule>> GetClaimSchedulesForProcessorByUser(int userID, string loginUsername)
        {
            CheckAuthorization();
            return new EAMIWebUIDataServiceMgr().GetClaimSchedulesForProcessorByUser(userID, loginUsername);
        }

        public CommonStatusPayload<DataSet> GetRemittanceAdviceDataByCSID(int csID, int systemID)
        {
            CheckAuthorization();
            return new EAMIWebUIDataServiceMgr().GetRemittanceAdviceDataByCSID(csID, systemID);
        }


        #endregion

        #region Supervisor Approval Screen

        public CommonStatusPayload<List<PaymentSuperGroup>> GetPaymentSuperGroupsForSupervisorScreen()
        {
            CheckAuthorization();
            return new EAMIWebUIDataServiceMgr().GetPaymentSuperGroupsForSupervisorScreen();
        }

        public CommonStatus SetClaimSchedulesStatusToApproved(List<int> claimScheduleIDs, string note, string loginUserName)
        {
            CheckAuthorization();
            return new EAMIWebUIDataServiceMgr().SetClaimSchedulesStatusToApproved(claimScheduleIDs, note, loginUserName);
        }

        public CommonStatus SetClaimSchedulesStatusToReturnToProcessor(List<int> claimScheduleIDs, string note, string loginUserName)
        {
            CheckAuthorization();
            return new EAMIWebUIDataServiceMgr().SetClaimSchedulesStatusToReturnToProcessor(claimScheduleIDs, note, loginUserName);
        }

        public CommonStatusPayload<List<ClaimSchedule>> GetClaimSchedulesByStatusType(List<int> claimScheduleStatusTypeIDList, string loginUsername)
        {
            CheckAuthorization();
            return new EAMIWebUIDataServiceMgr().GetClaimSchedulesByStatusType(claimScheduleStatusTypeIDList, loginUsername);
        }

        public CommonStatus SetRemittanceAdviceNote(int claimScheduleID, string note, string loginUsername)
        {
            CheckAuthorization();
            return new EAMIWebUIDataServiceMgr().SetRemittanceAdviceNote(claimScheduleID, note, loginUsername);
        }

        public CommonStatus DeleteRemittanceAdviceNote(int claimScheduleID)
        {
            CheckAuthorization();
            return new EAMIWebUIDataServiceMgr().DeleteRemittanceAdviceNote(claimScheduleID);
        }

        public CommonStatus ReturnPaymentGroupsToSystemOfRecord(List<string> priorityGroupHashList, string user, string note)
        {
            CheckAuthorization();
            return new EAMIWebUIDataServiceMgr().ReturnPaymentGroupsToSystemOfRecord(priorityGroupHashList, note, user);
        }

        public CommonStatusPayload<List<ElectronicClaimSchedule>> GetElectronicClaimSchedulesByDateRangeStatusType(DateTime dateFrom, DateTime dateTo, int statusTypeID)
        {
            CheckAuthorization();
            return new EAMIWebUIDataServiceMgr().GetElectronicClaimSchedulesByDateRangeStatusType(dateFrom, dateTo, statusTypeID);
        }

        public CommonStatusPayload<List<ElectronicClaimSchedule>> GetECSAndCSByECSID(DateTime dateFrom, DateTime dateTo, int ecsID)
        {
            CheckAuthorization();
            return new EAMIWebUIDataServiceMgr().GetECSAndCSByECSID(dateFrom, dateTo, ecsID);
        }

        public CommonStatusPayload<List<ClaimSchedule>> GetClaimSchedulesSubmettedForApproval()
        {
            CheckAuthorization();
            return new EAMIWebUIDataServiceMgr().GetClaimSchedulesSubmettedForApproval();
        }

        public Common.CommonStatusPayload<List<AggregatedFundingDetail>> GetECSFundingSummary(List<int> csIdList)
        {
            CheckAuthorization();
            return new EAMIWebUIDataServiceMgr().GetECSFundingSummary(csIdList);
        }

        public CommonStatus SetECSStatusToApproved(int ecsID, string note, string loginUserName)
        {
            CheckAuthorization();
            return new EAMIWebUIDataServiceMgr().SetECSStatusToApproved(ecsID, note, loginUserName);
        }

        public CommonStatus SetECSStatusToPending(int ecsID, string note, string loginUserName)
        {
            CheckAuthorization();
            return new EAMIWebUIDataServiceMgr().SetECSStatusToPending(ecsID, note, loginUserName);
        }

        public CommonStatus DeleteECS(int ecsID, string note, string loginUserName)
        {
            CheckAuthorization();
            return new EAMIWebUIDataServiceMgr().DeleteECS(ecsID, note, loginUserName);
        }

        #endregion

        #region Reports

        public CommonStatusPayload<DataSet> GetFaceSheetDataByECSID(int ecsID, int systemID, bool isProdEnv )
        {
            CheckAuthorization();
            return new EAMIWebUIDataServiceMgr().GetFaceSheetDataByECSID(ecsID, systemID,isProdEnv);
        }

        public CommonStatusPayload<DataSet> GetTransferLetterDataByECSID(int ecsID, string userName)
        {
            CheckAuthorization();
            return new EAMIWebUIDataServiceMgr().GetTransferLetterDataByECSID(ecsID, userName);
        }

        public CommonStatusPayload<DataSet> GetDrawSummaryReportData(DateTime payDate)
        {
            CheckAuthorization();
            return new EAMIWebUIDataServiceMgr().GetDrawSummaryReportData(payDate);
        }

        public CommonStatusPayload<DataSet> GetHoldReportData()
        {
            CheckAuthorization();
            return new EAMIWebUIDataServiceMgr().GetHoldReportData();
        }

        public CommonStatusPayload<DataSet> GetECSReportData(DateTime dateFrom, DateTime dateTo)
        {
            CheckAuthorization();
            return new EAMIWebUIDataServiceMgr().GetECSReportData(dateFrom, dateTo);
        }

        public CommonStatusPayload<DataSet> GetReturnToSORReportData(DateTime dateFrom, DateTime dateTo)
        {
            CheckAuthorization();
            return new EAMIWebUIDataServiceMgr().GetReturnToSORReportData(dateFrom, dateTo);
        }

        public CommonStatusPayload<DataSet> GetESTOReport(DateTime payDate)
        {
            CheckAuthorization();
            return new EAMIWebUIDataServiceMgr().GetESTOReport(payDate);
        }

        public CommonStatusPayload<DataSet> GetDataSummaryReportData(DateTime dateFrom, DateTime dateTo)
        {
            //CheckAuthorization();
            return new EAMIWebUIDataServiceMgr().GetDataSummaryReportData(dateFrom, dateTo);
        }

        #endregion

        #region web service authorization impalementation


        private void CheckAuthorization()
        {
            string sscUserName = string.Empty;
            string userName = string.Empty;
            string upnUserName = string.Empty;
            try
            {
                // check and exit if custom authorization is not enabled or app setting is missing
                if (!bool.Parse(ConfigurationManager.AppSettings["WebUISvcAuthorizationEnabled"].ToString()))
                {
                    return;
                }

                sscUserName = ServiceSecurityContext.Current.PrimaryIdentity.Name;
                userName = sscUserName.Contains("\\") ? sscUserName.Substring(9) : sscUserName;
                upnUserName = userName + "@intra.dhs.ca.gov";

                // check against local group of EAMI Admins (including service account) who's authorized to access web service methods
                if (!IsInEAMIAdminGroup(upnUserName))
                {
                    throw new Exception();
                }

                #region disabled Authorization against both the local admin group and EAMI app users
                /* 
                // this implementation checks against both local EAMI Admin group and EAMI application users
                // we are not using this for now because web server runs under service account whose identity
                // passed to the web service.
                if (!IsInEAMIAdminGroup(upnUserName) && !IsEAMIUser(userName))
                {
                    throw new Exception();
                }
                */
                #endregion
            }
            catch (Exception ex)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Access Denied");
                sb.AppendLine("sscUser : " + sscUserName);
                sb.AppendLine(ex.Message);
                sb.AppendLine(ex.StackTrace);
                EAMILogger.Instance.Error(sb.ToString());
                throw new Exception("Access Denied");
            }
        }

        private bool IsInEAMIAdminGroup(string userName, string group = "EAMIAppDataServiceUsers")
        {
            // must use upn name
            using (WindowsIdentity identity = new WindowsIdentity(userName))
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                return principal.IsInRole(group);
            }
        }

        #endregion
    }
}
