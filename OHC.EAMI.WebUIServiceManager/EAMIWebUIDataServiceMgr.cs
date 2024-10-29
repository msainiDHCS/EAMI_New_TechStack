using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using OHC.EAMI.Common;
using OHC.EAMI.CommonEntity;
using OHC.EAMI.DataAccess;
using OHC.EAMI.Util;

namespace OHC.EAMI.WebUIServiceManager
{
    public class EAMIWebUIDataServiceMgr : IEAMIWebUIDataService, IDisposable
    {
        #region Infrastructure

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        ~EAMIWebUIDataServiceMgr()
        {
            Dispose(false);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
        }

        #endregion

        #region Administration

        #region EAMI Users...

        public List<EAMIElement> GetEAMIDataElements(string userName)
        {
            List<EAMIElement> lst = new List<EAMIElement>();

            try
            {
                lst.Add(new EAMIElement() { ID = 999, Description = "EAMI Description", Text = "EAMI Text", Value = "EAMI Value" });
            }
            catch (Exception ex)
            {
                EAMILogger.Instance.Error(ex);
            }

            return lst;
        }

        public Common.CommonStatusPayload<EAMIUser> GetEAMIUser(string userName, string domainName = null, string password = null, bool verifyPassword = false)
        {
            using (DataAccess.EAMIAuthorization acc = new DataAccess.EAMIAuthorization())
            {
                return acc.GetEAMIUser(userName, domainName, password, verifyPassword);
            }
        }

        public Common.CommonStatusPayload<EAMIUser> GetEAMIUserByID(long userID)
        {
            using (DataAccess.EAMIAuthorization acc = new DataAccess.EAMIAuthorization())
            {
                return acc.GetEAMIUserByID(userID);
            }
        }

        public Common.CommonStatus AddUpdateEAMIUser(EAMIUser inputUser, string loginUserName)
        {
            using (DataAccess.EAMIAuthorization acc = new DataAccess.EAMIAuthorization())
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
        public Common.CommonStatus DeactivateEAMIUser(EAMIUser inputUser, string loginUserName)
        {
            using (DataAccess.EAMIAuthorization acc = new DataAccess.EAMIAuthorization())
            {
                return acc.DeactivateEAMIUser(inputUser, loginUserName);
            }
        }

        public Common.CommonStatusPayload<List<Tuple<string, EAMIAuthBase>>> GetEAMIAuthorizationLookUps(string lookUpType = null)
        {
            using (DataAccess.EAMIAuthorization acc = new DataAccess.EAMIAuthorization())
            {
                return acc.GetEAMIAuthorizationLookUps(lookUpType);
            }
        }

        public Common.CommonStatus CheckEAMIUserValidity(string userName, long userType)
        {
            using (DataAccess.EAMIAuthorization acc = new DataAccess.EAMIAuthorization())
            {
                return acc.CheckEAMIUserValidity(userName, userType);
            }
        }

        public Common.CommonStatusPayload<List<EAMIUser>> GetAllEAMIUsers()
        {
            using (DataAccess.EAMIAuthorization acc = new DataAccess.EAMIAuthorization())
            {
                return acc.GetAllEAMIUsers();
            }
        }

        public Common.CommonStatus AddUpdateEAMIMasterData(EAMIMasterData inputData, string loginUserName)
        {
            using (DataAccess.EAMIAuthorization acc = new DataAccess.EAMIAuthorization())
            {
                return acc.AddUpdateEAMIMasterData(inputData, loginUserName);
            }
        }

        public Common.CommonStatusPayload<List<EAMIMasterData>> GetAllEAMIMasterData(string DataType)
        {
            using (DataAccess.EAMIAuthorization acc = new DataAccess.EAMIAuthorization())
            {
                return acc.GetAllEAMIMasterData(DataType);
            }
        }

        public Common.CommonStatusPayload<EAMIMasterData> GetEAMIMasterDataByID(string DataType, long DataTID)
        {
            using (DataAccess.EAMIAuthorization acc = new DataAccess.EAMIAuthorization())
            {
                return acc.GetEAMIMasterDataByID(DataType, DataTID);
            }
        }

        public CommonStatus UpdateYearlyCalendarEntry(List<Tuple<EAMIDateType, EAMICalendarAction, string, DateTime>> dates, string loginUserName)
        {
            using (DataAccess.EAMIAuthorization acc = new DataAccess.EAMIAuthorization())
            {
                CommonStatus cs = acc.UpdateYearlyCalendarEntry(dates, loginUserName);

                if (cs.Status)//refresh cached data if updates wa successful
                    RefCodeDBMgr.GetRefCodeTableList(true);

                return cs;
            }
        }

        public CommonStatusPayload<List<Tuple<EAMIDateType, string, DateTime>>> GetYearlyCalendarEntries(int activeYear, string loginUserName)
        {
            using (DataAccess.EAMIAuthorization acc = new DataAccess.EAMIAuthorization())
            {
                return acc.GetYearlyCalendarEntries(activeYear, loginUserName);
            }
        }

        public CommonStatusPayload<List<RefCodeList>> GetReferenceCodeList(params enRefTables[] refTableNames)
        {
            try
            {
                List<RefCodeList> lst = RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableNames(refTableNames);

                return new CommonStatusPayload<List<RefCodeList>>(lst, true, "");
            }
            catch (Exception ex)
            {
                EAMILogger.Instance.Error(ex);
                return new CommonStatusPayload<List<RefCodeList>>(null, false, true, ex.Message);
            }
        }

        public CommonStatusPayload<RefCodeTableList> GetRefCodeTableList()
        {
            try
            {
                return new CommonStatusPayload<RefCodeTableList>(RefCodeDBMgr.GetRefCodeTableList(), true);
            }
            catch (Exception ex)
            {
                EAMILogger.Instance.Error(ex);
                return new CommonStatusPayload<RefCodeTableList>(null, false, true, ex.Message);
            }
        }

        public SCOSettingList GetSCOSettingList()
        {
            return RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName<SCOSettingList>(enRefTables.TB_SCO_PROPERTY);
        }

        public CommonStatusPayload<List<Tuple<string, int>>> GetEAMICounts()
        {
            return DataAccess.PaymentDataDbMgr.GetEAMICounts();
        }

        #endregion

        #region Fund...

        /// <summary>
        /// Add fund in system...
        /// </summary>
        /// <param name="inputFund"></param>
        /// <param name="loginUserName"></param>
        /// <returns></returns>
        public CommonStatus AddEAMIFund(Fund inputFund)
        {
            using (EAMIManageSystem da = new EAMIManageSystem())
            {
                return da.AddEAMIFund(inputFund);
            }
        }

        /// <summary>
        /// Get all funds from the system...
        /// </summary>
        /// <returns></returns>
        public CommonStatusPayload<List<Fund>> GetAllFunds(bool includeInactive, long systemID)
        {
            using (EAMIManageSystem da = new EAMIManageSystem())
            {
                return da.GetAllFunds(includeInactive, systemID);
            }
        }

        /// <summary>
        /// Update Fund information...
        /// </summary>
        /// <param name="inputFund"></param>
        /// <param name="loginUserName"></param>
        /// <returns></returns>
        public CommonStatus UpdateEAMIFund(Fund inputFund)
        {
            using (EAMIManageSystem da = new EAMIManageSystem())
            {
                return da.UpdateEAMIFund(inputFund);
            }
        }

        #endregion

        #region Facesheet Values...

        /// <summary>
        /// Add Facesheet Values in system...
        /// </summary>
        /// <param name="inputFS"></param>
        /// <returns></returns>
        public CommonStatus AddFacesheetValues(FacesheetValues inputFS)
        {
            using (EAMIManageSystem da = new EAMIManageSystem())
            {
                return da.AddFacesheetValues(inputFS);
            }
        }

        /// <summary>
        /// Get all Facesheet Values from the system...
        /// </summary>
        /// <returns></returns>
        public CommonStatusPayload<List<FacesheetValues>> GetAllFacesheetValues(bool includeInactive, long systemID)
        {
            using (EAMIManageSystem da = new EAMIManageSystem())
            {
                return da.GetAllFacesheetValues(includeInactive, systemID);
            }
        }

        /// <summary>
        /// Update Facesheet Values information...
        /// </summary>
        /// <param name="inputFS"></param>
        /// <returns></returns>
        public CommonStatus UpdateFacesheetValues(FacesheetValues inputFS)
        {
            using (EAMIManageSystem da = new EAMIManageSystem())
            {
                return da.UpdatFacesheetValues(inputFS);
            }
        }

        #endregion

        #region Exclusive Payment Type...

        /// <summary>
        /// Add fund in system...
        /// </summary>
        /// <param name="inputExclusivePmtType"></param>
        /// <param name="loginUserName"></param>
        /// <returns></returns>
        public CommonStatus AddEAMIExclusivePmtType(ExclusivePmtType inputExclusivePmtType)
        {
            using (EAMIManageSystem da = new EAMIManageSystem())
            {
                return da.AddEAMIExclusivePmtType(inputExclusivePmtType);
            }
        }

        /// <summary>
        /// Get all funds from the system...
        /// </summary>
        /// <returns></returns>
        public CommonStatusPayload<List<ExclusivePmtType>> GetAllExclusivePmtTypes(bool includeInactive, long systemID)
        {
            using (EAMIManageSystem da = new EAMIManageSystem())
            {
                return da.GetAllExclusivePmtTypes(includeInactive, systemID);
            }
        }

        /// <summary>
        /// Update Fund information...
        /// </summary>
        /// <param name="inputExclusivePmtType"></param>
        /// <param name="loginUserName"></param>
        /// <returns></returns>
        public CommonStatus UpdateEAMIExclusivePmtType(ExclusivePmtType inputExclusivePmtType)
        {
            using (EAMIManageSystem da = new EAMIManageSystem())
            {
                return da.UpdateEAMIExclusivePmtType(inputExclusivePmtType);
            }
        }

        ///// <summary>
        ///// Soft delete fund from system...
        ///// </summary>
        ///// <param name="inputExclusivePmtType"></param>
        ///// <param name="loginUserName"></param>
        ///// <returns></returns>
        //public CommonStatus DeleteEAMIExclusivePmtType(ExclusivePmtType inputExclusivePmtType)
        //{
        //    using (EAMIManageSystem da = new EAMIManageSystem())
        //    {
        //        return da.DeleteEAMIExclusivePmtType(inputExclusivePmtType);
        //    }
        //}

        #endregion

        #region Funding Source...

        /// <summary>
        /// Add Funding Source in system...
        /// </summary>
        /// <param name="inputFundingSource"></param>
        /// <returns></returns>
        public CommonStatus AddEAMIFundingSource(FundingSource inputFundingSource)
        {
            using (EAMIManageSystem da = new EAMIManageSystem())
            {
                return da.AddEAMIFundingSource(inputFundingSource);
            }
        }

        /// <summary>
        /// Get all Funding Source from the system...
        /// </summary>
        /// <returns></returns>
        public CommonStatusPayload<List<FundingSource>> GetAllFundingSources(bool includeInactive, long systemID)
        {
            using (EAMIManageSystem da = new EAMIManageSystem())
            {
                return da.GetAllFundingSources(includeInactive, systemID);
            }
        }

        /// <summary>
        /// Update Funding Source information...
        /// </summary>
        /// <param name="inputFundingSource"></param>
        /// <returns></returns>
        public CommonStatus UpdateEAMIFundingSource(FundingSource inputFundingSource)
        {
            using (EAMIManageSystem da = new EAMIManageSystem())
            {
                return da.UpdateEAMIFundingSource(inputFundingSource);
            }
        }

        ///// <summary>
        ///// Soft delete Funding Source from system...
        ///// </summary>
        ///// <param name="inputFundingSource"></param>
        ///// <returns></returns>
        //public CommonStatus DeleteEAMIFundingSource(FundingSource inputFundingSource)
        //{
        //    using (EAMIManageSystem da = new EAMIManageSystem())
        //    {
        //        return da.DeleteEAMIFundingSource(inputFundingSource);
        //    }
        //}

        #endregion

        #region SCO Properties...

        /// <summary>
        /// Add SCOProperty in system...
        /// </summary>
        /// <param name="inputSCOProperties"></param>
        /// <param name="loginUserName"></param>
        /// <returns></returns>
        public CommonStatus AddSCOProperties(SCOProperty inputSCOProperties)
        {
            using (EAMIManageSystem da = new EAMIManageSystem())
            {
                return da.AddSCOProperties(inputSCOProperties);
            }
        }

        /// <summary>
        /// Get all SCO Properties from the system...
        /// </summary>
        /// <returns></returns>
        public CommonStatusPayload<List<SCOProperty>> GetAllSCOProperties(bool includeInactive, long systemID)
        {
            using (EAMIManageSystem da = new EAMIManageSystem())
            {
                return da.GetAllSCOProperties(includeInactive, systemID);
            }
        }

        public CommonStatusPayload<SCOProperty> GetAllSCOPropertyTypes()
        {
            using (EAMIManageSystem da = new EAMIManageSystem())
            {
                return da.GetAllSCOPropertyTypes();
            }
        }

        /// <summary>
        /// Update SCO Properties information...
        /// </summary>
        /// <param name="inputSCOProperties"></param>
        /// <param name="loginUserName"></param>
        /// <returns></returns>
        public CommonStatus UpdateSCOProperties(SCOProperty inputSCOProperties)
        {
            using (EAMIManageSystem da = new EAMIManageSystem())
            {
                return da.UpdateSCOProperties(inputSCOProperties);
            }
        }

        #endregion

        #endregion

        #region Payment assignment

        /// <summary>
        /// gets a list of PaymentSuperGroup based on assigned flag
        /// </summary>
        /// <param name="assigned"></param>
        /// <returns></returns>
        public CommonStatusPayload<List<PaymentSuperGroup>> GetPaymentSuperGroupsForAssignment(bool assigned)
        {
            CommonStatusPayload<List<PaymentSuperGroup>> commonStatusPayload = new CommonStatusPayload<List<PaymentSuperGroup>>(new List<PaymentSuperGroup>(), true);

            try
            {
                CommonStatusPayload<List<PaymentGroup>> commonStatusPayload_PaymentGroup = GetPaymentGroupsForAssignment(assigned);

                if (commonStatusPayload_PaymentGroup.Status)
                {
                    List<PaymentGroup> pgList = commonStatusPayload_PaymentGroup.Payload;
                    commonStatusPayload.Payload.AddRange(GetPaymentSuperGroupsFromPaymentGroup(pgList));
                }
                else
                {
                    throw new Exception(commonStatusPayload_PaymentGroup.GetCombinedMessage());
                }
            }
            catch (Exception ex)
            {
                commonStatusPayload.Status = false;
                commonStatusPayload.IsFatal = true;
                commonStatusPayload.AddMessageDetail(ex.Message);
            }

            return commonStatusPayload;
        }

        public CommonStatusPayload<List<PaymentGroup>> GetPaymentGroupsForAssignment(bool assigned)
        {
            CommonStatusPayload<List<PaymentGroup>> commonStatusPayload = new CommonStatusPayload<List<PaymentGroup>>(new List<PaymentGroup>(), true);

            try
            {
                string paymentStatusType = assigned ? "ASSIGNED" : "UNASSIGNED";
                int paymentStatusTypeId = RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_PAYMENT_STATUS_TYPE).GetRefCodeByCode(paymentStatusType).ID;
                commonStatusPayload.Payload.AddRange(DataAccess.PaymentDataDbMgr.GetPaymentGroupsByStatus(paymentStatusTypeId));
            }
            catch (Exception ex)
            {
                commonStatusPayload.Status = false;
                commonStatusPayload.IsFatal = true;
                commonStatusPayload.AddMessageDetail(ex.Message);
            }

            return commonStatusPayload;
        }

        public CommonStatusPayload<List<PaymentGroup>> GetPaymentGroupsAssignedByUser(int userID)
        {
            List<PaymentGroup> paymentGroupList = new List<PaymentGroup>();
            CommonStatusPayload<List<PaymentGroup>> commonStatusPayload = new CommonStatusPayload<List<PaymentGroup>>(paymentGroupList, true);

            try
            {
                paymentGroupList = DataAccess.PaymentDataDbMgr.GetPaymentGroupsAssignedByUser(userID);
            }
            catch (Exception ex)
            {
                paymentGroupList = null;
                commonStatusPayload.Status = false;
                commonStatusPayload.IsFatal = true;
                commonStatusPayload.AddMessageDetail(ex.Message);
            }

            return commonStatusPayload;
        }

        public CommonStatus AssignPaymentGroups(List<PaymentGroup> paymentGroupList, string user)
        {
            CommonStatus commonStatus = new CommonStatus(false);

            try
            {
                RefCode statusType = RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_PAYMENT_STATUS_TYPE).GetRefCodeByCode("ASSIGNED");
                commonStatus = DataAccess.PaymentDataDbMgr.AssignPaymentGroup(statusType, paymentGroupList, user);
                if (commonStatus.Status)
                {
                    this.SendUserAssignmentNotificationEmail(paymentGroupList, user);
                }
            }
            catch (Exception ex)
            {
                commonStatus.Status = false;
                commonStatus.IsFatal = true;
                commonStatus.AddMessageDetail(ex.Message);
            }

            return commonStatus;
        }

        public CommonStatus UnAssignPaymentGroups(List<PaymentGroup> paymentGroupList, string user)
        {
            CommonStatus commonStatus = new CommonStatus(false);

            try
            {
                int paydateCalendarID = 0;
                RefCode statusType = RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_PAYMENT_STATUS_TYPE).GetRefCodeByCode("UNASSIGNED");
                commonStatus = DataAccess.PaymentDataDbMgr.UnAssignPaymentGroup(statusType, paymentGroupList, user);
            }
            catch (Exception ex)
            {
                commonStatus.Status = false;
                commonStatus.IsFatal = true;
                commonStatus.AddMessageDetail(ex.Message);
            }

            return commonStatus;
        }

        public CommonStatus ReAssignPaymentGroups(List<PaymentGroup> paymentGroupList, string user)
        {
            CommonStatus commonStatus = new CommonStatus(false);

            try
            {
                RefCode statusType = RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_PAYMENT_STATUS_TYPE).GetRefCodeByCode("ASSIGNED");
                commonStatus = DataAccess.PaymentDataDbMgr.ReAssignPaymentGroup(statusType, paymentGroupList, user);
                if (commonStatus.Status)
                {
                    this.SendUserAssignmentNotificationEmail(paymentGroupList, user);
                }
            }
            catch (Exception ex)
            {
                commonStatus.Status = false;
                commonStatus.IsFatal = true;
                commonStatus.AddMessageDetail(ex.Message);
            }

            return commonStatus;
        }

        public CommonStatus SetPaymentGroupsToHold(List<string> priorityGroupHashList, string user, string note)
        {
            CommonStatus commonStatus = new CommonStatus(false);

            try
            {
                RefCode statusType = RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_PAYMENT_STATUS_TYPE).GetRefCodeByCode("HOLD");
                commonStatus = DataAccess.PaymentDataDbMgr.SetPaymentGroupStatus(statusType, priorityGroupHashList, user, note);
            }
            catch (Exception ex)
            {
                commonStatus.Status = false;
                commonStatus.IsFatal = true;
                commonStatus.AddMessageDetail(ex.Message);
            }

            return commonStatus;
        }

        public CommonStatus SetPaymentGroupsToUnHold(List<string> priorityGroupHashList, string user, string note)
        {
            CommonStatus commonStatus = new CommonStatus(false);

            try
            {
                RefCode statusType = RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_PAYMENT_STATUS_TYPE).GetRefCodeByCode("UNHOLD");
                commonStatus = DataAccess.PaymentDataDbMgr.SetPaymentGroupStatus(statusType, priorityGroupHashList, user, note);
            }
            catch (Exception ex)
            {
                commonStatus.Status = false;
                commonStatus.IsFatal = true;
                commonStatus.AddMessageDetail(ex.Message);
            }

            return commonStatus;
        }

        public CommonStatus SetPaymentGroupsToReturnToSup(List<string> priorityGroupHashList, string user, string note)
        {
            CommonStatus commonStatus = new CommonStatus(false);

            try
            {
                RefCode statusType = RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_PAYMENT_STATUS_TYPE).GetRefCodeByCode("RETURNED_TO_SUP");
                commonStatus = DataAccess.PaymentDataDbMgr.SetPaymentGroupStatus(statusType, priorityGroupHashList, user, note);
            }
            catch (Exception ex)
            {
                commonStatus.Status = false;
                commonStatus.IsFatal = true;
                commonStatus.AddMessageDetail(ex.Message);
            }

            return commonStatus;
        }

        public CommonStatus SetPaymentGroupsToReleaseFromSup(List<string> priorityGroupHashList, string user, string note)
        {
            CommonStatusPayload<string> status = DataAccess.PaymentDataDbMgr.SetPaymentGroupToReleaseFromSup(priorityGroupHashList, user, note);
            if (status.Status)
            {
                SendProcessorNotificationOfDeniedPaymentReturn(priorityGroupHashList[0], status.Payload, note, user);
            }
            return new CommonStatus(status.Status, status.IsFatal, status.MessageDetailList);
        }

        public CommonStatusPayload<List<PaymentFundingDetail>> GetPaymentFundingDetails(int paymentRecordID)
        {
            CommonStatusPayload<List<PaymentFundingDetail>> commonStatusPayload = new CommonStatusPayload<List<PaymentFundingDetail>>(new List<PaymentFundingDetail>(), true);

            try
            {
                commonStatusPayload.Payload.AddRange(DataAccess.PaymentDataDbMgr.GetFundingDetailsByPaymentRecID(paymentRecordID));
            }
            catch (Exception ex)
            {
                commonStatusPayload.Status = false;
                commonStatusPayload.IsFatal = true;
                commonStatusPayload.AddMessageDetail(ex.Message);
            }

            return commonStatusPayload;
        }

        public CommonStatusPayload<List<Tuple<EAMIUser, int>>> GetProcessorUserListWithAssignedPaymentCounts(int userID)
        {
            CommonStatusPayload<List<Tuple<EAMIUser, int>>> commonStatusPayload = new CommonStatusPayload<List<Tuple<EAMIUser, int>>>(new List<Tuple<EAMIUser, int>>(), true);

            try
            {
                commonStatusPayload.Payload.AddRange(DataAccess.PaymentDataDbMgr.GetUserListWithAssignedPaymentCount(userID));
            }
            catch (Exception ex)
            {
                commonStatusPayload.Status = false;
                commonStatusPayload.IsFatal = true;
                commonStatusPayload.AddMessageDetail(ex.Message);
            }

            return commonStatusPayload;
        }

        public CommonStatusPayload<List<PaymentRecForUser>> GetAllPaymentRecordsByUser(int userID)
        {
            CommonStatusPayload<List<PaymentRecForUser>> commonStatusPayload = new CommonStatusPayload<List<PaymentRecForUser>>(new List<PaymentRecForUser>(), true);

            try
            {
                commonStatusPayload.Payload.AddRange(DataAccess.PaymentDataDbMgr.GetAllPaymentRecordsByUser(userID));
            }
            catch (Exception ex)
            {
                commonStatusPayload.Status = false;
                commonStatusPayload.IsFatal = true;
                commonStatusPayload.AddMessageDetail(ex.Message);
            }

            return commonStatusPayload;
        }

        #endregion

        #region Payment Processing

        public CommonStatusPayload<List<PaymentSuperGroup>> GetAssignedPaymentSuperGroupsForAddingToClaimSchedule(int userID, string loginUsername)
        {
            CommonStatusPayload<List<PaymentSuperGroup>> commonStatusPayload = new CommonStatusPayload<List<PaymentSuperGroup>>(new List<PaymentSuperGroup>(), true);

            try
            {
                List<PaymentGroup> pgList = this.GetAssignedPaymentGroupsForAddingToClaimSchedule(userID, loginUsername).Payload;
                commonStatusPayload.Payload.AddRange(GetPaymentSuperGroupsFromPaymentGroup(pgList));
            }
            catch (Exception ex)
            {
                commonStatusPayload.Status = false;
                commonStatusPayload.IsFatal = true;
                commonStatusPayload.AddMessageDetail(ex.Message);
            }

            return commonStatusPayload;
        }

        public CommonStatusPayload<List<ClaimSchedule>> GetClaimSchedulesForProcessorByUser(int userID, string loginUsername)
        {
            CommonStatusPayload<List<ClaimSchedule>> commonStatusPayload = new CommonStatusPayload<List<ClaimSchedule>>(new List<ClaimSchedule>(), true);

            try
            {
                int statusTypeID_ASSIGNED = RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_CLAIM_SCHEDULE_STATUS_TYPE).GetRefCodeByCode("ASSIGNED").ID;
                int statusTypeID_RETURN_TO_PROCESSOR = RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_CLAIM_SCHEDULE_STATUS_TYPE).GetRefCodeByCode("RETURN_TO_PROCESSOR").ID;

                commonStatusPayload.Payload.AddRange(DataAccess.ClaimScheduleDataDbMgr.GetClaimSchedulesByUser(userID, new List<int>() { statusTypeID_ASSIGNED, statusTypeID_RETURN_TO_PROCESSOR }));
            }
            catch (Exception ex)
            {
                commonStatusPayload.Status = false;
                commonStatusPayload.IsFatal = true;
                commonStatusPayload.AddMessageDetail(ex.Message);
            }

            return commonStatusPayload;
        }

        public CommonStatus CreateNewClaimSchedule(List<string> paymentSetNumberList, int userID, string loginUserName)
        {
            CommonStatus comonStatus = new CommonStatus(false);
            double maxClaimScheduleAmount = 0;

            try
            {
                maxClaimScheduleAmount = double.Parse(SCOSetting.GetSCOSettingValue<string>(RefCodeDBMgr.GetRefCodeTableList(), "CS_MAX_TOTAL_AMOUNT")) / 100;
                RefCodeTableList rctl = RefCodeDBMgr.GetRefCodeTableList();

                List<PaymentGroup> paymentGroupList = DataAccess.PaymentDataDbMgr.GetPaymentGroupsByPaymentSetNumberList(paymentSetNumberList);

                if (paymentGroupList.Count > 0)
                {
                    //Determine if to Create a single Claim Schedule or Multiple Linked Claim Schedules  
                    if (paymentGroupList.Where(_ => Convert.ToDouble(_.Amount) > maxClaimScheduleAmount).Count() == 0)
                    {
                        //SINGLE CLAIM SCHEDULE - assign ALL payment groups to a single claim schedule
                        comonStatus = CreateNewClaimSchedule_Single(paymentGroupList, userID, loginUserName);
                    }
                    else if (paymentGroupList.Where(_ => Convert.ToDouble(_.Amount) < maxClaimScheduleAmount).Count() == 0)
                    {
                        //MULTIPLE CLAIM SCHEDULES - split payment group between multiple claim schedules
                        comonStatus = CreateNewClaimSchedule_MultipleLinked(paymentGroupList, userID, loginUserName, maxClaimScheduleAmount);
                    }
                    else
                    {
                        comonStatus.AddMessageDetail(string.Format("Cannot create Claim Schedule from selected payment sets. Cannot have a mixed selection of payment sets with both above and below the Amount of {0:C}.", maxClaimScheduleAmount));
                    }
                }
                else
                {
                    comonStatus.AddMessageDetail("Cannot create Claim Schedule. No payment sets available.");
                }
            }
            catch (Exception ex)
            {
                comonStatus.Status = false;
                comonStatus.IsFatal = true;
                comonStatus.AddMessageDetail(ex.Message);
            }

            //return status
            return comonStatus;
        }

        public CommonStatus DeleteClaimSchedule(int claimScheduleID, string loginUserName)
        {
            CommonStatus comonStatus = new CommonStatus(false);

            try
            {
                //Get Claim Schedule from db
                ClaimSchedule claimSchedule = DataAccess.ClaimScheduleDataDbMgr.GetClaimSchedulesByID(new List<int>() { claimScheduleID }).Where(__ => __.PrimaryKeyID == claimScheduleID).First();

                // VALIDATE Claim Schedule
                comonStatus = this.ValidateIfCanMofdifyClaimSchedule(new List<ClaimSchedule>() { claimSchedule });

                if (comonStatus.Status)
                {
                    comonStatus = DataAccess.ClaimScheduleDataDbMgr.DeleteClaimSchedules(new List<int>() { claimScheduleID }, loginUserName);
                }
            }
            catch (Exception ex)
            {
                comonStatus.Status = false;
                comonStatus.IsFatal = true;
                comonStatus.AddMessageDetail(ex.Message);
            }

            return comonStatus;
        }

        public CommonStatus AddPaymentGroupsToClaimSchedule(int claimScheduleID, List<string> paymentSetNumberList, string loginUserName)
        {
            CommonStatus comonStatus = new CommonStatus(true);

            try
            {
                //Get Claim Schedule from db
                ClaimSchedule claimSchedule = DataAccess.ClaimScheduleDataDbMgr.GetClaimSchedulesByID(new List<int>() { claimScheduleID }).Where(__ => __.PrimaryKeyID == claimScheduleID).First();

                if (claimSchedule.IsLinked)
                {
                    comonStatus.Status = false;
                    comonStatus.AddMessageDetail("Cannot add records to a linked Claim Schedule.");
                }

                if (comonStatus.Status)
                {
                    //Get Payment Record Groups from db
                    List<PaymentGroup> paymentGroupList = DataAccess.PaymentDataDbMgr.GetPaymentGroupsByPaymentSetNumberList(paymentSetNumberList);

                    // VALIDATE Claim Schedule
                    comonStatus = this.ValidateClaimSchedule(claimSchedule, paymentGroupList, false);

                    if (comonStatus.Status)
                    {
                        //POPULATE  Claim Schedule
                        claimSchedule.PaymentGroupList.AddRange(paymentGroupList);

                        //PERSIST Claim Schedule
                        comonStatus = DataAccess.ClaimScheduleDataDbMgr.AddPaymentGroupsToClaimSchedule(claimSchedule, paymentSetNumberList, loginUserName);
                    }
                }
            }
            catch (Exception ex)
            {
                comonStatus.Status = false;
                comonStatus.IsFatal = true;
                comonStatus.AddMessageDetail(ex.Message);
            }

            return comonStatus;
        }

        public CommonStatus RemovePaymentGroupsFromClaimSchedule(int claimScheduleID, List<string> paymentSetNumberList, string loginUserName)
        {
            CommonStatus comonStatus = new CommonStatus(true);

            try
            {
                //Get Claim Schedule from db
                List<ClaimSchedule> claimScheduleList = DataAccess.ClaimScheduleDataDbMgr.GetClaimSchedulesByID(new List<int>() { claimScheduleID });

                ClaimSchedule claimSchedule = claimScheduleList.Where(_ => _.PrimaryKeyID == claimScheduleID).First();

                //DELETE Claim Schedule if its existing Payment Group count equals provided list to be removed
                if (DetermineIfToDeleteClaimSchedule(claimSchedule, paymentSetNumberList))
                {
                    // VALIDATE Claim Schedule
                    comonStatus = this.ValidateIfCanMofdifyClaimSchedule(new List<ClaimSchedule>() { claimSchedule });

                    if (comonStatus.Status)
                    {
                        //DELETE THE CLAIM SCHEDULE
                        comonStatus = DataAccess.ClaimScheduleDataDbMgr.DeleteClaimSchedules(new List<int>() { claimScheduleID }, loginUserName);
                    }
                }
                else
                {
                    // VALIDATE Claim Schedule
                    comonStatus = this.ValidateIfCanMofdifyClaimSchedule(new List<ClaimSchedule>() { claimSchedule });

                    if (comonStatus.Status)
                    {
                        //REMOVE PROVIDED Payment Record groups from Claim Schedule
                        comonStatus = DataAccess.ClaimScheduleDataDbMgr.RemovePaymentGroupsFromClaimSchedule(claimSchedule, paymentSetNumberList, loginUserName);
                    }
                }
            }
            catch (Exception ex)
            {
                comonStatus.Status = false;
                comonStatus.IsFatal = true;
                comonStatus.AddMessageDetail(ex.Message);
            }

            return comonStatus;
        }

        public CommonStatus SetClaimSchedulesStatusToSubmitForApproval(List<int> claimScheduleIDs, string note, string loginUserName)
        {
            CommonStatus commonStatus = new CommonStatus(false);

            try
            {
                RefCode statusType = RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_CLAIM_SCHEDULE_STATUS_TYPE).GetRefCodeByCode("SUBMIT_FOR_APPROVAL");
                List<ClaimSchedule> claimScheduleList = DataAccess.ClaimScheduleDataDbMgr.GetClaimSchedulesByID(claimScheduleIDs);

                //VALIDATE CLAIM SCHDULE before proceeding to new status
                commonStatus = this.ValidateClaimScheduleListStatus(statusType, claimScheduleList);

                if (commonStatus.Status)
                {
                    //ADVANCE CLAIM SCHEDULE STATUS
                    commonStatus = DataAccess.ClaimScheduleDataDbMgr.SetClaimScheduleStatus(statusType, claimScheduleList.Select(_ => _.PrimaryKeyID).ToList(), loginUserName, note);
                }
            }
            catch (Exception ex)
            {
                commonStatus.Status = false;
                commonStatus.IsFatal = true;
                commonStatus.AddMessageDetail(ex.Message);
            }

            return commonStatus;
        }

        public CommonStatusPayload<DataSet> GetRemittanceAdviceDataByCSID(int csID, int systemID)
        {
            return DataAccess.ClaimScheduleDataDbMgr.GetRemittanceAdviceDataByCSID(csID, systemID);
        }


        #endregion

        #region Supervisor Approval Screen

        public CommonStatusPayload<List<PaymentSuperGroup>> GetPaymentSuperGroupsForSupervisorScreen()
        {
            CommonStatusPayload<List<PaymentSuperGroup>> commonStatusPayload = new CommonStatusPayload<List<PaymentSuperGroup>>(new List<PaymentSuperGroup>(), true);

            try
            {
                int statusTypeID_RETURNED_TO_SUP = RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_PAYMENT_STATUS_TYPE).GetRefCodeByCode("RETURNED_TO_SUP").ID;

                List<PaymentGroup> pgList = DataAccess.PaymentDataDbMgr.GetPaymentGroupsByStatus(statusTypeID_RETURNED_TO_SUP);
                commonStatusPayload.Payload.AddRange(GetPaymentSuperGroupsFromPaymentGroup(pgList));
            }
            catch (Exception ex)
            {
                commonStatusPayload.Status = false;
                commonStatusPayload.IsFatal = true;
                commonStatusPayload.AddMessageDetail(ex.Message);
            }

            return commonStatusPayload;
        }

        public CommonStatus SetClaimSchedulesStatusToApproved(List<int> claimScheduleIDs, string note, string loginUserName)
        {
            CommonStatus commonStatus = new CommonStatus(false);

            try
            {
                RefCode statusType = RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_CLAIM_SCHEDULE_STATUS_TYPE).GetRefCodeByCode("APPROVED");
                List<ClaimSchedule> claimScheduleList = DataAccess.ClaimScheduleDataDbMgr.GetClaimSchedulesByID(claimScheduleIDs);

                //VALIDATE CLAIM SCHDULE before proceeding to new status
                commonStatus = this.ValidateClaimScheduleListStatus(statusType, claimScheduleList);

                if (commonStatus.Status)
                {

                    //CREATE AND PACKAGE ECS; SET TO PENDING STATUS
                    List<ElectronicClaimSchedule> ecsList = CreateAndPackageElectronicClaimSchedule(claimScheduleList, loginUserName, note);

                    //PROCEED IF ECS EXIST                
                    if (ecsList.Count() > 0)
                    {
                        //ADVANCE CLAIM SCHEDULE STATUS TO APPROVED
                        commonStatus = DataAccess.ClaimScheduleDataDbMgr.SetClaimSchedulesStatusToApproved(claimScheduleList.Select(_ => _.PrimaryKeyID).ToList(), ecsList, note, loginUserName);
                    }
                }
            }
            catch (Exception ex)
            {
                commonStatus.Status = false;
                commonStatus.IsFatal = true;
                commonStatus.AddMessageDetail(ex.Message);
            }

            return commonStatus;
        }

        public CommonStatus SetClaimSchedulesStatusToReturnToProcessor(List<int> claimScheduleIDs, string note, string loginUserName)
        {
            CommonStatus commonStatus = new CommonStatus(false);

            try
            {
                RefCode statusType = RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_CLAIM_SCHEDULE_STATUS_TYPE).GetRefCodeByCode("RETURN_TO_PROCESSOR");
                List<ClaimSchedule> claimScheduleList = DataAccess.ClaimScheduleDataDbMgr.GetClaimSchedulesByID(claimScheduleIDs);

                //VALIDATE CLAIM SCHDULE before proceeding to new status
                commonStatus = this.ValidateClaimScheduleListStatus(statusType, claimScheduleList);

                if (commonStatus.Status)
                {
                    //SET CLAIM SCHEDULE STATUS
                    commonStatus = DataAccess.ClaimScheduleDataDbMgr.SetClaimSchedulesStatusToReturnToProcessor(claimScheduleList.Select(_ => _.PrimaryKeyID).ToList(), note, loginUserName);
                }

                // notify processor user of denied/returned CSs
                if (commonStatus.Status)
                {
                    this.SendProcessorNotificationOfDeniedCS(claimScheduleList, note, loginUserName);
                }
            }
            catch (Exception ex)
            {
                commonStatus.Status = false;
                commonStatus.IsFatal = true;
                commonStatus.AddMessageDetail(ex.Message);
            }

            return commonStatus;
        }

        public CommonStatusPayload<List<ClaimSchedule>> GetClaimSchedulesByStatusType(List<int> claimScheduleStatusTypeID, string loginUsername)
        {
            CommonStatusPayload<List<ClaimSchedule>> commonStatusPayload = new CommonStatusPayload<List<ClaimSchedule>>(new List<ClaimSchedule>(), true);

            try
            {
                commonStatusPayload.Payload.AddRange(DataAccess.ClaimScheduleDataDbMgr.GetClaimSchedulesByStatusType(claimScheduleStatusTypeID));
            }
            catch (Exception ex)
            {
                commonStatusPayload.Status = false;
                commonStatusPayload.IsFatal = true;
                commonStatusPayload.AddMessageDetail(ex.Message);
            }

            return commonStatusPayload;
        }

        public CommonStatus SetRemittanceAdviceNote(int claimScheduleID, string note, string loginUsername)
        {
            return DataAccess.ClaimScheduleDataDbMgr.SetRemittanceAdviceNote(claimScheduleID, note, loginUsername);
        }

        public CommonStatus DeleteRemittanceAdviceNote(int claimScheduleID)
        {
            return DataAccess.ClaimScheduleDataDbMgr.DeleteRemittanceAdviceNote(claimScheduleID);
        }

        public CommonStatus ReturnPaymentGroupsToSystemOfRecord(List<string> priorityGroupHashList, string note, string loginUserName)
        {
            CommonStatus commonStatus = new CommonStatus(false);

            try
            {
                RefCode statusType = RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_PAYMENT_STATUS_TYPE).GetRefCodeByCode("RETURNED_TO_SOR");
                commonStatus = DataAccess.PaymentDataDbMgr.SetPaymentGroupStatus(statusType, priorityGroupHashList, loginUserName, note);
            }
            catch (Exception ex)
            {
                commonStatus.Status = false;
                commonStatus.IsFatal = true;
                commonStatus.AddMessageDetail(ex.Message);
            }

            return commonStatus;
        }

        public CommonStatusPayload<List<ElectronicClaimSchedule>> GetElectronicClaimSchedulesByDateRangeStatusType(DateTime dateFrom, DateTime dateTo, int statusTypeID)
        {
            CommonStatusPayload<List<ElectronicClaimSchedule>> commonStatusPayload = new CommonStatusPayload<List<ElectronicClaimSchedule>>(new List<ElectronicClaimSchedule>(), true);

            try
            {
                commonStatusPayload.Payload.AddRange(DataAccess.ClaimScheduleDataDbMgr.GetElectronicClaimSchedulesByDateRangeStatusType(statusTypeID, dateFrom.Date, dateTo.Date));
            }
            catch (Exception ex)
            {
                commonStatusPayload.Status = false;
                commonStatusPayload.IsFatal = true;
                commonStatusPayload.AddMessageDetail(ex.Message);
            }

            return commonStatusPayload;
        }

        public CommonStatusPayload<List<ElectronicClaimSchedule>> GetECSAndCSByECSID(DateTime dateFrom, DateTime dateTo, int ecsID)
        {
            CommonStatusPayload<List<ElectronicClaimSchedule>> commonStatusPayload = new CommonStatusPayload<List<ElectronicClaimSchedule>>(new List<ElectronicClaimSchedule>(), true);

            try
            {
                commonStatusPayload.Payload.AddRange(DataAccess.ClaimScheduleDataDbMgr.GetECSAndCSByECSID(ecsID, dateFrom.Date, dateTo.Date));
            }
            catch (Exception ex)
            {
                commonStatusPayload.Status = false;
                commonStatusPayload.IsFatal = true;
                commonStatusPayload.AddMessageDetail(ex.Message);
            }

            return commonStatusPayload;
        }

        public CommonStatusPayload<List<ClaimSchedule>> GetClaimSchedulesSubmettedForApproval()
        {
            CommonStatusPayload<List<ClaimSchedule>> commonStatusPayload = new CommonStatusPayload<List<ClaimSchedule>>(new List<ClaimSchedule>(), true);

            try
            {
                List<int> statusTypeID = new List<int> { RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_CLAIM_SCHEDULE_STATUS_TYPE).GetRefCodeByCode("SUBMIT_FOR_APPROVAL").ID };
                commonStatusPayload.Payload.AddRange(DataAccess.ClaimScheduleDataDbMgr.GetClaimSchedulesByStatusType(statusTypeID));
            }
            catch (Exception ex)
            {
                commonStatusPayload.Status = false;
                commonStatusPayload.IsFatal = true;
                commonStatusPayload.AddMessageDetail(ex.Message);
            }

            return commonStatusPayload;
        }

        public Common.CommonStatusPayload<List<AggregatedFundingDetail>> GetECSFundingSummary(List<int> csIdList)
        {
            CommonStatus cs = new CommonStatus(true);
            List<AggregatedFundingDetail> paymentFundingDetail = new List<AggregatedFundingDetail>();

            try
            {
                List<ClaimSchedule> csList = DataAccess.ClaimScheduleDataDbMgr.GetClaimSchedulesByID(csIdList);
                paymentFundingDetail = DataAccess.ClaimScheduleDataDbMgr.GetFundingSummaryCSIdList(csList.Select(_ => _.PrimaryKeyID).ToList());
                cs = ValidateECS(csList);
            }
            catch (Exception ex)
            {
                cs.Status = false;
                cs.IsFatal = true;
                cs.AddMessageDetail(ex.Message);
            }

            return new CommonStatusPayload<List<AggregatedFundingDetail>>(paymentFundingDetail, cs.Status, cs.IsFatal, cs.MessageDetailList);
        }

        public CommonStatus SetECSStatusToApproved(int ecsID, string note, string loginUserName)
        {
            CommonStatus commonStatus = new CommonStatus(false);

            try
            {
                RefCode ecs_StatusType_APPROVED = RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_ECS_STATUS_TYPE).GetRefCodeByCode("APPROVED");

                ElectronicClaimSchedule ecs = new ElectronicClaimSchedule()
                {
                    EcsId = ecsID,
                    CurrentStatusType = ecs_StatusType_APPROVED,
                    CurrentStatusNote = note
                };

                //SET CLAIM SCHEDULE STATUS
                commonStatus = DataAccess.ClaimScheduleDataDbMgr.UpdateElectronicClaimScheduleStatus(ecs, loginUserName);
            }
            catch (Exception ex)
            {
                commonStatus.Status = false;
                commonStatus.IsFatal = true;
                commonStatus.AddMessageDetail(ex.Message);
            }

            return commonStatus;
        }

        public CommonStatus SetECSStatusToPending(int ecsID, string note, string loginUserName)
        {
            CommonStatus commonStatus = new CommonStatus(false);

            try
            {
                RefCode ecs_StatusType_PENDING = RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_ECS_STATUS_TYPE).GetRefCodeByCode("PENDING");

                List<ElectronicClaimSchedule> ecsList = DataAccess.ClaimScheduleDataDbMgr.GetECSAndCSByECSID(ecsID, null, null);

                ElectronicClaimSchedule ecs = new ElectronicClaimSchedule()
                {
                    EcsId = ecsList[0].EcsId,
                    CurrentStatusType = ecs_StatusType_PENDING,
                    CurrentStatusNote = note,
                    ClaimScheduleList = ecsList[0].ClaimScheduleList
                };

                commonStatus = DataAccess.ClaimScheduleDataDbMgr.UpdateElectronicClaimScheduleFileName(ecs.EcsId, ecs.EcsFileName = "", ecs.SCO_File_Line_Count = 0);

                //SET CLAIM SCHEDULE STATUS
                commonStatus = DataAccess.ClaimScheduleDataDbMgr.UpdateElectronicClaimScheduleStatus(ecs, loginUserName);
            }
            catch (Exception ex)
            {
                commonStatus.Status = false;
                commonStatus.IsFatal = true;
                commonStatus.AddMessageDetail(ex.Message);
            }

            return commonStatus;
        }

        public CommonStatus DeleteECS(int ecsID, string note, string loginUserName)
        {
            //SET CLAIM SCHEDULE STATUS
            CommonStatus commonStatus = DataAccess.ClaimScheduleDataDbMgr.DeleteECS(ecsID, note, loginUserName);

            return commonStatus;
        }

        #endregion

        #region Reports

        public CommonStatusPayload<DataSet> GetFaceSheetDataByECSID(int ecsID, int systemID, bool isProdEnv)
        {
            return DataAccess.ReportDataDbMgr.GetFaceSheetDataByECSID(ecsID, systemID, isProdEnv);
        }

        public CommonStatusPayload<DataSet> GetTransferLetterDataByECSID(int ecsID, string userName)
        {
            return DataAccess.ReportDataDbMgr.GetTransferLetterDataByECSID(ecsID, userName);
        }

        public CommonStatusPayload<DataSet> GetDrawSummaryReportData(DateTime payDate)
        {
            return DataAccess.ReportDataDbMgr.GetDrawSummaryReportData(payDate);
        }

        public CommonStatusPayload<DataSet> GetHoldReportData()
        {
            return DataAccess.ReportDataDbMgr.GetHoldReportData();
        }

        public CommonStatusPayload<DataSet> GetReturnToSORReportData(DateTime dateFrom, DateTime dateTo)
        {
            return DataAccess.ReportDataDbMgr.GetReturnToSORReportData(dateFrom, dateTo);
        }

        public CommonStatusPayload<DataSet> GetECSReportData(DateTime dateFrom, DateTime dateTo)
        {
            return DataAccess.ReportDataDbMgr.GetECSReportData(dateFrom, dateTo);
        }

        public CommonStatusPayload<DataSet> GetESTOReport(DateTime payDate)
        {
            return DataAccess.ReportDataDbMgr.GetESTOReport(payDate);
        }

        public CommonStatusPayload<DataSet> GetDataSummaryReportData(DateTime dateFrom, DateTime dateTo)
        {
            return DataAccess.ReportDataDbMgr.GetDataSummaryReport(dateFrom, dateTo);
        }

        #endregion

        #region Private methods

        private List<PaymentGroup> GetPaymentGroupsByStatus(int paymentStatusTypeId)
        {
            return DataAccess.PaymentDataDbMgr.GetPaymentGroupsByStatus(paymentStatusTypeId);
        }

        /// <summary>
        /// groups the payment groups into a super group
        /// </summary>
        /// <param name="pgList"></param>
        /// <returns></returns>
        private List<PaymentSuperGroup> GetPaymentSuperGroupsFromPaymentGroup(List<PaymentGroup> pgList)
        {
            List<PaymentSuperGroup> retList = new List<PaymentSuperGroup>();

            if (pgList == null || pgList.Count == 0)
                return retList;

            // crete unique payment supergroup key list
            List<string> psgKeyList = new List<string>();
            foreach (PaymentGroup pg in pgList)
            {
                if (!psgKeyList.Contains(pg.PaymentSuperGroupKey))
                {
                    psgKeyList.Add(pg.PaymentSuperGroupKey);
                }
            }

            // build list of payment super groups to return
            foreach (string psgKey in psgKeyList)
            {
                // search Payment Groups for a given Payee code, Payment Type, and Fiscal year
                List<PaymentGroup> pgListFiltered = pgList.FindAll(pg => (pg.PaymentSuperGroupKey == psgKey)).ToList();
                if (pgListFiltered != null && pgListFiltered.Count != 0)
                {
                    // sort the list by UniqueNumber
                    pgListFiltered.Sort((x, y) => x.UniqueNumber.CompareTo(y.UniqueNumber));
                    PaymentSuperGroup psg = new PaymentSuperGroup();
                    psg.UniqueKey = psgKey;
                    psg.PayeeInfo = pgListFiltered[0].PayeeInfo;
                    psg.PaymentType = pgListFiltered[0].PaymentType;
                    psg.ContractNumber = pgListFiltered[0].ContractNumber;
                    psg.FiscalYear = pgListFiltered[0].FiscalYear;
                    psg.PaymentGroupList = pgListFiltered;
                    retList.Add(psg);
                }
            }

            return retList;
        }

        private CommonStatus ValidateClaimScheduleListStatus(RefCode statusType, List<ClaimSchedule> claimScheduleList)
        {
            CommonStatus comonStatus = new CommonStatus(true);

            //VALIDATE if can make chage to CLAIM SCHEDULE
            comonStatus = ValidateIfCanMofdifyClaimSchedule(claimScheduleList);

            //COntinue with further validation
            if (comonStatus.Status)
            {
                if (statusType.Code == "SUBMIT_FOR_APPROVAL")
                {
                    //NO VALIDATION IS NEEDED 
                }
                else if (statusType.Code == "APPROVED")
                {
                    //NO VALIDATION IS NEEDED YET
                }
                else if (statusType.Code == "RETURN_TO_PROCESSOR")
                {
                    //NO VALIDATION IS NEEDED YET
                }
            }

            return comonStatus;
        }

        private CommonStatus ValidateClaimSchedule(ClaimSchedule claimSchedule, List<PaymentGroup> paymentGroupList, bool addToNewCS)
        {
            CommonStatus comonStatus = new CommonStatus(true);

            //double maxClaimScheduleAmount = double.Parse(DBSetting.GetDBSettingValue<string>(RefCodeDBMgr.GetRefCodeTableList(), "CS_MAX_TOTAL_AMOUNT")) / 100;
            double maxClaimScheduleAmount = double.Parse(SCOSetting.GetSCOSettingValue<string>(RefCodeDBMgr.GetRefCodeTableList(), "CS_MAX_TOTAL_AMOUNT")) / 100;
            RefCodeTableList rctl = RefCodeDBMgr.GetRefCodeTableList();

            // double maxClaimScheduleAmount = double.Parse(rctl.GetRefCodeListByTableName(enRefTables.TB_SCO_PROPERTY).GetRefCodeByCode("CS_MAX_TOTAL_AMOUNT").ToString());

            string validationMessage = addToNewCS ? "Cannot create new Claim Schedule" : "Cannot add to existing Claim Schedule";

            List<PaymentGroup> paymentGroupListToValidate = claimSchedule.PaymentGroupList.Union(paymentGroupList).ToList();



            //PAYMENT RECORD COUNT
            if (paymentGroupListToValidate.Count == 0)
            {
                comonStatus.Status = false;
                comonStatus.AddMessageDetail(string.Format("{0}. Claim Schedule does not have Payment Record Groups.", validationMessage));
            }

            //PAYMENT METHOS TYPE EFT/NON-EFT
            if (paymentGroupListToValidate.GroupBy(_ => _.PaymentMethodType.ID).Count() > 1)
            {
                comonStatus.Status = false;
                comonStatus.AddMessageDetail(string.Format("{0}. Claim Schedule contains payments of mixed payment method type (EFT/Non-EFT).", validationMessage));
            }

            //USER ASSIGNMENT
            if (paymentGroupListToValidate.Any(_ => _.AssignedUser == null))
            {
                comonStatus.Status = false;
                comonStatus.AddMessageDetail(string.Format("{0}. Assigned User is not set.", validationMessage));
            }
            else if (paymentGroupListToValidate.GroupBy(_ => _.AssignedUser.User_ID).Count() > 1)
            {
                comonStatus.Status = false;
                comonStatus.AddMessageDetail(string.Format("{0}. Assigned User is not distinct.", validationMessage));
            }

            //AMOUNT
            if (paymentGroupListToValidate.Any(_ => _.Amount <= 0))
            {
                comonStatus.Status = true;
                //comonStatus.Status = false;
                //comonStatus.AddMessageDetail(string.Format("{0}. Amount zero or negative for one or more Payment Record groups.", validationMessage));
            }
            else if (Convert.ToDouble(paymentGroupListToValidate.Sum(_ => _.Amount)) > maxClaimScheduleAmount)
            {
                comonStatus.Status = false;
                comonStatus.AddMessageDetail(string.Format("{0}. Claim Schedule Amount is greater than maximum allowed.", validationMessage));
            }
            else if (paymentGroupListToValidate.Sum(_ => _.Amount) <= 0)
            {
                comonStatus.Status = false;
                comonStatus.AddMessageDetail(string.Format("{0}. Claim Schedule Amount is zero or negative.", validationMessage));
            }

            //PAYDATE
            if (paymentGroupListToValidate.Any(_ => _.PayDate == null))
            {
                comonStatus.Status = false;
                comonStatus.AddMessageDetail(string.Format("{0}. Pay Date is not set.", validationMessage));
            }
            else if (paymentGroupListToValidate.GroupBy(_ => _.PayDate.ID).Count() > 1)
            {
                comonStatus.Status = false;
                comonStatus.AddMessageDetail(string.Format("{0}. Pay Date is not distinct.", validationMessage));
            }

            //VENDOR
            if (paymentGroupListToValidate.Any(_ => _.PayeeInfo == null))
            {
                comonStatus.Status = false;
                comonStatus.AddMessageDetail(string.Format("{0}. Vendor is not set.", validationMessage));
            }
            else if (paymentGroupListToValidate.GroupBy(_ => _.PayeeInfo.PEE.ID).Count() > 1)
            {
                comonStatus.Status = false;
                comonStatus.AddMessageDetail(string.Format("{0}. Vendor is not distinct.", validationMessage));
            }
            else if (paymentGroupListToValidate.GroupBy(_ => _.PayeeInfo.PEEInfo_PK_ID).Count() > 1)
            {
                comonStatus.Status = false;
                comonStatus.AddMessageDetail(string.Format("{0}. Vendor Info such as Address or Name are not distinct.", validationMessage));
            }

            //FISCAL YEAR
            if (paymentGroupListToValidate.Any(_ => string.IsNullOrEmpty(_.FiscalYear)))
            {
                comonStatus.Status = false;
                comonStatus.AddMessageDetail(string.Format("{0}. Fiscal Year is not set.", validationMessage));
            }
            else if (paymentGroupListToValidate.GroupBy(_ => _.FiscalYear).Count() > 1)
            {
                comonStatus.Status = false;
                comonStatus.AddMessageDetail(string.Format("{0}. Fiscal Year is not distinct.", validationMessage));
            }

            //PAYMENT TYPE
            if (paymentGroupListToValidate.Any(_ => string.IsNullOrEmpty(_.PaymentType)))
            {
                comonStatus.Status = false;
                comonStatus.AddMessageDetail(string.Format("{0}. Payment Type is not set.", validationMessage));
            }
            else if (paymentGroupListToValidate.GroupBy(_ => _.PaymentType).Count() > 1)
            {
                comonStatus.Status = false;
                comonStatus.AddMessageDetail(string.Format("{0}. Payment Type is not distinct.", validationMessage));
            }

            //CONTRACT NUMBER
            if (paymentGroupListToValidate.Any(_ => string.IsNullOrEmpty(_.ContractNumber)))
            {
                comonStatus.Status = false;
                comonStatus.AddMessageDetail(string.Format("{0}. Contract Number is not set.", validationMessage));
            }
            else if (paymentGroupListToValidate.GroupBy(_ => _.ContractNumber).Count() > 1)
            {
                comonStatus.Status = false;
                comonStatus.AddMessageDetail(string.Format("{0}. Contract Number is not distinct.", validationMessage));
            }

            if (paymentGroupListToValidate.GroupBy(item => item.ExclusivePaymentType.Code).Count() > 1)
            {
                comonStatus.Status = false;
                comonStatus.AddMessageDetail("Claim Schedule contains multiple payments of mixed exclusive payment type.");
            }

            return comonStatus;
        }

        private string ComposeClaimScheduleNumber(DateTime claimScheduleTimeStamp, int counter)
        {
            return string.Format("{0:yyyyMMddHHmmssfff}{1:000}", claimScheduleTimeStamp, counter);
        }

        private void PopulateNewClaimSchedule(ClaimSchedule claimSchedule, List<PaymentGroup> paymentGroupList)
        {
            claimSchedule.PaymentGroupList.AddRange(paymentGroupList);
            claimSchedule.FiscalYear = paymentGroupList[0].FiscalYear;
            claimSchedule.PaymentType = paymentGroupList[0].PaymentType;
            claimSchedule.ContractNumber = paymentGroupList[0].ContractNumber;
            claimSchedule.PayDate = paymentGroupList[0].PayDate;
            claimSchedule.PayeeInfo = paymentGroupList[0].PayeeInfo;
            claimSchedule.ExclusivePaymentType = RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_EXCLUSIVE_PAYMENT_TYPE).GetRefCodeByID(paymentGroupList[0].ExclusivePaymentType.ID);
            claimSchedule.PaymentMethodType = paymentGroupList[0].PaymentMethodType;
        }

        private bool DetermineIfToDeleteClaimSchedule(ClaimSchedule claimSchedule, List<string> priorityGroupHashList)
        {
            return claimSchedule.PaymentGroupList.Where(_ => priorityGroupHashList.Contains(_.UniqueNumber)).Count() == claimSchedule.PaymentGroupList.Count();
        }

        private CommonStatus ValidateIfCanMofdifyClaimSchedule(List<ClaimSchedule> claimSCheduleList)
        {
            CommonStatus comonStatus = new CommonStatus(true);

            RefCode statusType_ASSIGNED = RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_CLAIM_SCHEDULE_STATUS_TYPE).GetRefCodeByCode("ASSIGNED");
            RefCode statusType_RETURN_TO_PROCESSOR = RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_CLAIM_SCHEDULE_STATUS_TYPE).GetRefCodeByCode("RETURN_TO_PROCESSOR");

            if (!claimSCheduleList.Select(_ => _.CurrentStatus.StatusType.ID == statusType_ASSIGNED.ID).Any()
                    && !claimSCheduleList.Select(_ => _.CurrentStatus.StatusType.ID == statusType_RETURN_TO_PROCESSOR.ID).Any())
            {
                comonStatus.Status = false;
                comonStatus.AddMessageDetail("Cannot Modify Claim Schedule. Status is not ASSIGNED and is not RETURN_TO_PROCESSOR");
            }

            return comonStatus;
        }

        private CommonStatusPayload<List<PaymentGroup>> GetAssignedPaymentGroupsForAddingToClaimSchedule(int userID, string loginUsername)
        {
            List<PaymentGroup> paymentGroupList = DataAccess.PaymentDataDbMgr.GetPaymentGroupsAssignedByUser(userID);
            return new CommonStatusPayload<List<PaymentGroup>>(paymentGroupList, true, string.Empty);
        }

        private CommonStatus CreateNewClaimSchedule_Single(List<PaymentGroup> paymentGroupList, int userID, string loginUserName)
        {
            //INITIALIZE CLAIM SCHEDULE
            ClaimSchedule claimSchedule = new CommonEntity.ClaimSchedule
            {
                UniqueNumber = this.ComposeClaimScheduleNumber(DateTime.Now, 1),
                PaymentGroupList = new List<PaymentGroup>(),
                AssignedUser = new EAMIUser { User_ID = userID }
            };

            // VALIDATE Claim Schedule
            CommonStatus comonStatus = this.ValidateClaimSchedule(claimSchedule, paymentGroupList, true);

            if (comonStatus.Status)
            {
                //POPULATE  Claim Schedule
                this.PopulateNewClaimSchedule(claimSchedule, paymentGroupList);

                //PERSIST Claim Schedule
                comonStatus = DataAccess.ClaimScheduleDataDbMgr.InsertClaimScheduleData(claimSchedule, loginUserName);
            }

            //return status
            return comonStatus;
        }

        private CommonStatus CreateNewClaimSchedule_MultipleLinked(List<PaymentGroup> paymentGroupList, int userID, string loginUserName, double maxClaimScheduleAmount)
        {
            CommonStatus comonStatus = new CommonStatus(false);
            List<ClaimSchedule> claimScheduleList = new List<ClaimSchedule>();
            int claimScheduleCounter = 0;

            //CREATE CLAIM SCHEDULES
            foreach (PaymentGroup paymentGroup in paymentGroupList)
            {
                claimScheduleCounter = claimScheduleCounter + 100;
                claimScheduleList.AddRange(SplitClaimSChedule(paymentGroup, userID, maxClaimScheduleAmount, claimScheduleCounter));
            }

            //VALIDATE CLAIM SCHEDULES
            foreach (ClaimSchedule claimSchedule in claimScheduleList)
            {
                comonStatus = ValidateClaimSchedule(claimSchedule, new List<PaymentGroup>(), true);

                if (!comonStatus.Status) break;
            }

            if (comonStatus.Status)
            {
                //PERSIST Claim Schedule
                comonStatus = DataAccess.ClaimScheduleDataDbMgr.InsertClaimScheduleListData(claimScheduleList, loginUserName);
            }

            //return status
            return comonStatus;
        }

        private List<ClaimSchedule> SplitClaimSChedule(PaymentGroup paymentGroup, int userID, double maxClaimScheduleAmount, int claimScheduleCounter)
        {
            string linkedByPGNumber = paymentGroup.UniqueNumber;
            List<ClaimSchedule> linkedClaimScheduleList = new List<ClaimSchedule>();
            List<PaymentRec> paymentRecList = new List<PaymentRec>();
            paymentRecList = paymentGroup.PaymentRecordList.OrderBy(_ => _.Amount).ToList();
            DateTime claimScheduleTimeStamp = DateTime.Now;

            //GO THROUGH the Payment Record list, assign Payment Records to Claim Schedules 
            while (paymentRecList.Count > 0)
            {
                List<PaymentRec> prList = new List<PaymentRec>();
                claimScheduleCounter++;

                //INITIALIZE NEW CLAIM SCHEDULE
                ClaimSchedule claimSchedule = new CommonEntity.ClaimSchedule
                {
                    UniqueNumber = this.ComposeClaimScheduleNumber(claimScheduleTimeStamp, claimScheduleCounter),
                    PaymentGroupList = new List<PaymentGroup>(),
                    AssignedUser = new EAMIUser { User_ID = userID },
                    IsLinked = true,
                    LinkedByPGNumber = linkedByPGNumber
                };

                //POPULATE CLAIM SCHEDULE, with clonned paymentGroup entity
                PopulateNewClaimSchedule(claimSchedule, new List<PaymentGroup>() { ClonePaymentGroupEntity(paymentGroup) });

                //POPULATE PAYMENT RECORD LIST with Payment Records 
                //DO NOT exceed maximum ClaimSCheduleAmount
                foreach (PaymentRec pr in paymentRecList)
                {
                    //SPLIT LOGIC IS HERE 
                    if (Convert.ToDouble(prList.Sum(_ => _.Amount) + pr.Amount) <= maxClaimScheduleAmount)
                    {
                        prList.Add(pr);
                    }
                    else break;
                }

                //REMOVE the chosen Payment Records from the list 
                paymentRecList.RemoveAll(_ => prList.Select(__ => __.PrimaryKeyID).Contains(_.PrimaryKeyID));

                //assign the chosen Payment Records to a claim schedule
                claimSchedule.PaymentGroupList[0].PaymentRecordList = prList;

                //ADD LINKED CLAIM SCHEDULE TO THE CLAIM SCHEDULE LIST
                linkedClaimScheduleList.Add(claimSchedule);
            }

            return linkedClaimScheduleList;
        }

        private PaymentGroup ClonePaymentGroupEntity(PaymentGroup paymentGroup)
        {
            return new PaymentGroup
            {
                AssignedUser = paymentGroup.AssignedUser,
                ContractNumber = paymentGroup.ContractNumber,
                CurrentStatus = paymentGroup.CurrentStatus,
                FiscalYear = paymentGroup.FiscalYear,
                LatestStatus = paymentGroup.LatestStatus,
                PayDate = paymentGroup.PayDate,
                PayeeInfo = paymentGroup.PayeeInfo,
                PaymentDate = paymentGroup.PaymentDate,
                PaymentType = paymentGroup.PaymentType,
                ExclusivePaymentType = paymentGroup.ExclusivePaymentType,
                PaymentMethodType = paymentGroup.PaymentMethodType
            };
        }

        private CommonStatus ValidateECS(List<ClaimSchedule> csList)
        {
            CommonStatus cs = new CommonStatus(true);
            
            double maxTotalECSAmountAllowed = double.Parse(SCOSetting.GetSCOSettingValue<string>(RefCodeDBMgr.GetRefCodeTableList(), "ECS_MAX_TOTAL_AMOUNT")) / 100;
            int maxRecordCountAllowed = SCOSetting.GetSCOSettingValue<int>(RefCodeDBMgr.GetRefCodeTableList(), "ECS_MAX_RECORD_COUNT");

            RefCodeTableList rctl = RefCodeDBMgr.GetRefCodeTableList();
            //Validate Count
            if (csList.Count() > maxRecordCountAllowed)
            {
                cs.AddMessageDetail("Electronic Claim Schedule will be split due to Max Count of Claim Schedule exceeded per single Electronic Claim Schedule.");
            }

            //Validate Amount
            if (Convert.ToDouble(csList.SelectMany(_ => _.PaymentGroupList).SelectMany(__ => __.PaymentRecordList).Sum(___ => ___.Amount)) > maxTotalECSAmountAllowed)
            {
                cs.AddMessageDetail("Electronic Claim Schedule will be split due to Max Total Amount of  Claim Schedules exceeded per single Electronic Claim Schedule.");
            }

            //Validate Paydate
            if (csList.GroupBy(_ => new { _.PayDate.ID }).Count() > 1)
            {
                cs.AddMessageDetail("Electronic Claim Schedule will be split due to Pay Date is not distinct between selected Claim Schedules.");
            }

            //Linked Claim Schedules
            if (csList.Where(_ => _.IsLinked).Count() > 1)
            {
                cs.AddMessageDetail("Linked Claim Schedules are included in the funding sumary.");
            }

            return cs;
        }

        private List<ElectronicClaimSchedule> CreateAndPackageElectronicClaimSchedule(List<ClaimSchedule> claimScheduleList, string user, string note)
        {
            List<ElectronicClaimSchedule> ecsList = new List<ElectronicClaimSchedule>();

            //SETTINGs VALUES
            RefCodeTableList rcTableList = RefCodeDBMgr.GetRefCodeTableList();
            
            double maxTotalAmountAllowed = double.Parse(SCOSetting.GetSCOSettingValue<string>(RefCodeDBMgr.GetRefCodeTableList(), "ECS_MAX_TOTAL_AMOUNT")) / 100;
            int maxRecordCountAllowed = SCOSetting.GetSCOSettingValue<int>(rcTableList, "ECS_MAX_RECORD_COUNT");

            RefCodeTableList rctl = RefCodeDBMgr.GetRefCodeTableList();
            
            int fileCounter = 0;

            var ecs = claimScheduleList.GroupBy(_ => new
            {
                _.PayDate.ID,
                ExID = _.ExclusivePaymentType.ID,
                PMId = _.PaymentMethodType.ID
            }
           ).OrderBy(o => o.Key.ExID).ThenBy(p => p.Key.PMId).ToList();


            foreach (var e in ecs)
            {
                RefCode payDate = RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_PAYDATE_CALENDAR).GetRefCodeByID(e.Key.ID);
                RefCode exclusivepmtId = RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_EXCLUSIVE_PAYMENT_TYPE).GetRefCodeByID(e.Key.ExID);
                RefCode pmtMethodId = RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_PAYMENT_METHOD_TYPE).GetRefCodeByID(e.Key.PMId);

                List<ClaimSchedule> csListGroup = new List<ClaimSchedule>(claimScheduleList.Where(_ => _.PayDate.ID == payDate.ID
               && (_.ExclusivePaymentType.ID == exclusivepmtId.ID)
               && (_.PaymentMethodType.ID == pmtMethodId.ID)
               ));

                while (csListGroup.Count > 0)
                {
                    //COMPOSE CLAIM SCHEDULE LIST FOR ECS - take MAX RECORD COUNT
                    List<ClaimSchedule> csList = csListGroup.OrderBy(_ => _.PrimaryKeyID).Take(maxRecordCountAllowed).ToList();

                    //COMPOSE CLAIM SCHEDULE LIST FOR ECS - keep the list total under MAX AMOUNT
                    csList = KeepCSListUnderMaxAmountAllowed(csList, maxTotalAmountAllowed);

                    //CREATE new Electronic Claim Schedule
                    fileCounter++;
                    ecsList.Add(CreateNewElectronicClaimSchedule(
                        csList
                        , payDate
                        , exclusivepmtId
                        , string.Empty // ComposeElectronicClaimScheduleNumber(fileCounter)
                        , user
                        , note
                        , pmtMethodId));

                    //REMOVE claim schedules that have been assigned to Electronic Claim Schedule
                    csListGroup.RemoveAll(_ => csList.Select(__ => __.PrimaryKeyID).Contains(_.PrimaryKeyID));
                }
            }


            return ecsList;
        }

        private List<ClaimSchedule> KeepCSListUnderMaxAmountAllowed(List<ClaimSchedule> claimScheduleList, double maxTotalAmountAllowed)
        {
            //order list by descending AMOUNT 
            List<ClaimSchedule> csList = new List<ClaimSchedule>(claimScheduleList.OrderByDescending(_ => _.Amount));

            //Ensure list is under MAX amount
            while (csList.Count > 0 && Convert.ToDouble(csList.Sum(_ => _.Amount)) > maxTotalAmountAllowed)
            {
                //if SUM(AMOUNT) is more than MAX allowed, 
                //then continue to REMOVE records from top of the list
                //do this until amount is less than MAX allowed
                csList.RemoveAt(csList.Count - 1);
            }

            return csList;
        }

        private ElectronicClaimSchedule CreateNewElectronicClaimSchedule(List<ClaimSchedule> claimScheduleList, RefCode payDate, RefCode exclusivePaymentType, string ecsNumber, string user, string note, RefCode paymentMethodType)
        {
            RefCode CurrentStatusType = RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_ECS_STATUS_TYPE).GetRefCodeByCode("PENDING");
            DateTime ecsDateStamp = DateTime.Now;

            //POPULATE Electronic Claim Schedule
            ElectronicClaimSchedule electronicClaimSchedule = new ElectronicClaimSchedule()
            {
                ClaimScheduleList = claimScheduleList,
                Amount = claimScheduleList.Sum(_ => _.Amount),
                CreateDate = ecsDateStamp,
                CreatedBy = user,
                EcsNumber = ecsNumber,
                CurrentStatusDate = ecsDateStamp,
                CurrentStatusNote = note,
                CurrentStatusType = CurrentStatusType,
                PayDate = DateTime.Parse(payDate.Code),
                ExclusivePaymentType = exclusivePaymentType,
                PaymentMethodType = paymentMethodType

            };

            return electronicClaimSchedule;
        }


        /// <summary>
        /// perform email notification to the assigned users
        /// </summary>
        /// <param name="assignedPymtGrpList"></param>
        /// <param name="assigningUser"></param>
        private void SendUserAssignmentNotificationEmail(List<PaymentGroup> assignedPymtGrpList, string assigningUser)
        {
            try
            {
                List<string> emailList = new List<string>();
                // create user/paymnts list
                foreach (PaymentGroup pg in assignedPymtGrpList)
                {
                    if (pg.AssignedUser.User_ID > 0)
                    {
                        string email = RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_USER).GetRefCodeByID<UserAcc>((int)pg.AssignedUser.User_ID).User_EmailAddr;
                        pg.AssignedUser.User_EmailAddr = email;
                        if (!emailList.Contains(email))
                        {
                            emailList.Add(email);
                        }
                    }
                }

                bool isProdEnv = bool.Parse(ConfigurationManager.AppSettings["IsProductionEnvironment"].ToString());
                string serverName = "smtpoutbound.dhs.ca.gov";
                string email_from = "noReply@dhcs.ca.gov";
                string sysName = RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_SYSTEM_OF_RECORD).First().Code;
                string subject = "EAMI(" + sysName + ") NOTIFICATION - NEW ASSIGNMENT" + (isProdEnv ? string.Empty : " (" + Environment.MachineName + ")");

                int portID = 0;
                bool useDefaultCredentials = true;

                foreach (string emailAddr in emailList)
                {
                    List<PaymentGroup> pgList = assignedPymtGrpList.FindAll(t => t.AssignedUser.User_EmailAddr == emailAddr).ToList();

                    string[] email_to = new string[] { emailAddr };
                    string[] email_to_cc = new string[] { "" };
                    string[] email_to_bcc = new string[] { "" };

                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("You have been assigned the following Payment Sets by " + assigningUser + ":");
                    sb.AppendLine();
                    foreach (PaymentGroup pg in pgList)
                    {
                        sb.AppendLine(pg.UniqueNumber);
                    }
                    sb.AppendLine();
                    sb.AppendLine();
                    sb.AppendLine("Please do not reply to this email.");

                    EmailAccess ea = EmailAccess.GetInstance(
                        serverName,
                        portID,
                        useDefaultCredentials
                    );

                    bool success = ea.SendMessage(email_to, email_to_cc, email_to_bcc, email_from, subject, sb.ToString(), false);
                    if (!success)
                    {
                        StringBuilder sbError = new StringBuilder();
                        sbError.AppendLine("Error sending user assignment notification email");
                        sbError.AppendLine("  EmailTo: " + string.Join(",", email_to));
                        sbError.AppendLine("  Subject: " + subject);
                        sbError.AppendLine("  Message: " + sb.ToString());
                        throw new Exception(sbError.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                EAMILogger.Instance.Error(ex.Message + Environment.NewLine + Environment.NewLine + ex.StackTrace);
            }
        }


        /// <summary>
        /// perform email notification to processor user about denied CS submission
        /// </summary>
        /// <param name="claimScheduleList"></param>
        /// <param name="note"></param>
        /// <param name="supUser"></param>
        private void SendProcessorNotificationOfDeniedCS(List<ClaimSchedule> claimScheduleList, string note, string supUser)
        {
            try
            {
                List<string> emailList = new List<string>();
                // create user/paymnts list
                foreach (ClaimSchedule cs in claimScheduleList)
                {
                    if (cs.AssignedUser.User_ID > 0)
                    {
                        string email = RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_USER).GetRefCodeByID<UserAcc>((int)cs.AssignedUser.User_ID).User_EmailAddr;
                        cs.AssignedUser.User_EmailAddr = email;
                        if (!emailList.Contains(email))
                        {
                            emailList.Add(email);
                        }
                    }
                }

                bool isProdEnv = bool.Parse(ConfigurationManager.AppSettings["IsProductionEnvironment"].ToString());
                string serverName = ConfigurationManager.AppSettings["SMTPServer"].ToString();
                string email_from = ConfigurationManager.AppSettings["NoReplyEmailAddr"].ToString();
                string sysName = RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_SYSTEM_OF_RECORD).First().Code;
                string subject = "EAMI(" + sysName + ") NOTIFICATION - CS SUBMISSION DENIED" + (isProdEnv ? string.Empty : " (" + Environment.MachineName + ")");

                int portID = 0;
                bool useDefaultCredentials = true;

                foreach (string emailAddr in emailList)
                {
                    List<ClaimSchedule> csList = claimScheduleList.FindAll(t => t.AssignedUser.User_EmailAddr == emailAddr).ToList();

                    string[] email_to = new string[] { emailAddr };
                    string[] email_to_cc = new string[] { "" };
                    string[] email_to_bcc = new string[] { "" };

                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("Your claim schedule submission for supervisor approval is denied by " + supUser + ".");
                    sb.AppendLine();
                    sb.AppendLine("Reason: " + note);
                    foreach (ClaimSchedule cs in csList)
                    {
                        sb.AppendLine("CS#:   " + cs.UniqueNumber);
                    }
                    sb.AppendLine();
                    sb.AppendLine();
                    sb.AppendLine("Please do not reply to this email.");

                    EmailAccess ea = EmailAccess.GetInstance(
                        serverName,
                        portID,
                        useDefaultCredentials
                    );

                    bool success = ea.SendMessage(email_to, email_to_cc, email_to_bcc, email_from, subject, sb.ToString(), false);
                    if (!success)
                    {
                        StringBuilder sbError = new StringBuilder();
                        sbError.AppendLine("Error sending notification email of denied CS submission by processor");
                        sbError.AppendLine("  EmailTo: " + string.Join(",", email_to));
                        sbError.AppendLine("  Subject: " + subject);
                        sbError.AppendLine("  Message: " + sb.ToString());
                        throw new Exception(sbError.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                EAMILogger.Instance.Error(ex.Message + Environment.NewLine + Environment.NewLine + ex.StackTrace);
            }
        }


        /// <summary>
        /// perform email notification to processor user about denied payment return
        /// </summary>
        /// <param name="paymentSetNumber"></param>
        /// <param name="emailAddr"></param>
        /// <param name="note"></param>
        /// <param name="supUser"></param>
        private void SendProcessorNotificationOfDeniedPaymentReturn(string paymentSetNumber, string emailAddr, string note, string supUser)
        {
            try
            {
                bool isProdEnv = bool.Parse(ConfigurationManager.AppSettings["IsProductionEnvironment"].ToString());
                string serverName = ConfigurationManager.AppSettings["SMTPServer"].ToString();
                string email_from = ConfigurationManager.AppSettings["NoReplyEmailAddr"].ToString();
                string sysName = RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_SYSTEM_OF_RECORD).First().Code;
                string subject = "EAMI(" + sysName + ") NOTIFICATION - PAYMENT RETURN DENIED " + (isProdEnv ? string.Empty : " (" + Environment.MachineName + ")");

                int portID = 0;
                bool useDefaultCredentials = true;

                string[] email_to = new string[] { emailAddr };
                string[] email_to_cc = new string[] { "" };
                string[] email_to_bcc = new string[] { "" };

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Your submission of payment return for supervisor approval is denied by " + supUser + ".");
                sb.AppendLine();
                sb.AppendLine("PymtSet# : " + paymentSetNumber);
                sb.AppendLine("Reason   : " + note);
                sb.AppendLine();
                sb.AppendLine();
                sb.AppendLine("Please do not reply to this email.");

                EmailAccess ea = EmailAccess.GetInstance(
                    serverName,
                    portID,
                    useDefaultCredentials
                );

                bool success = ea.SendMessage(email_to, email_to_cc, email_to_bcc, email_from, subject, sb.ToString(), false);
                if (!success)
                {
                    StringBuilder sbError = new StringBuilder();
                    sbError.AppendLine("Error sending notification email of denied CS submission by processor");
                    sbError.AppendLine("  EmailTo: " + string.Join(",", email_to));
                    sbError.AppendLine("  Subject: " + subject);
                    sbError.AppendLine("  Message: " + sb.ToString());
                    throw new Exception(sbError.ToString());
                }
            }
            catch (Exception ex)
            {
                EAMILogger.Instance.Error(ex.Message + Environment.NewLine + Environment.NewLine + ex.StackTrace);
            }
        }

        public CommonStatus GetActivestatusEAMIUserInfo(long userID)
        {
            using (DataAccess.EAMIAuthorization acc = new DataAccess.EAMIAuthorization())
            {
                return acc.GetActivestatusEAMIUserInfo(userID);
            }
        }

        #endregion

    }
}
