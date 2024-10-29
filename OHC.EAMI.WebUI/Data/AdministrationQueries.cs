using System;
using System.Collections.Generic;
using OHC.EAMI.Common;
using OHC.EAMI.Common.ServiceInvoke;
using OHC.EAMI.CommonEntity;
using OHC.EAMI.WebUI.Controllers;

namespace OHC.EAMI.WebUI.Data
{
    public static class AdministrationQueries
    {
        private enum enumPaymentRecordStatus
        {
             RECEIVED
            ,UNASSIGNED
            ,ASSIGNED
            ,ADDED_TO_CS
            ,SENT_TO_SCO
            ,WARRANT_RECEIVED
            ,SENT_TO_CALSTARS
            ,RETURNED_TO_SOR
            ,RETURNED_TO_SUP
            ,RELEASED_FROM_SUP
            ,HOLD
            ,UNHOLD
        }

        #region EAMI Home Page...
        public static List<Tuple<string, int>> GetEAMICounts()
        {
            WcfServiceInvoker _wcfService = new WcfServiceInvoker();
            CommonStatusPayload<List<Tuple<string, int>>> cs = _wcfService.InvokeService <IEAMIWebUIDataService, 
                CommonStatusPayload<List<Tuple<string, int>>>>(svc => svc.GetEAMICounts());     //get from db via service
            ErrorHandlerController.CheckFatalException(cs.Status, cs.IsFatal, cs.GetCombinedMessage());
            return cs.Payload;
        }

        public static List<Tuple<EAMIUser, int>> GetAssignees(int userID)
        {
            WcfServiceInvoker _wcfService = new WcfServiceInvoker();
            CommonStatusPayload < List < Tuple < EAMIUser, int>>> cs = _wcfService.InvokeService<IEAMIWebUIDataService, 
                CommonStatusPayload<List<Tuple<EAMIUser, int>>>>(svc => svc.GetProcessorUserListWithAssignedPaymentCounts(userID));     //get from db via service
            ErrorHandlerController.CheckFatalException(cs.Status, cs.IsFatal, cs.GetCombinedMessage());
            return cs.Payload;
        }

        /// <summary>
        /// Returns only specific Payment records assigned to the given user.
        /// This method is used to get user assignments for specific statuses only until Payment life Cycle ends.
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>

        public static List<PaymentRecForUser> GetPaymentRecordsByUser(int userID)
        {
            WcfServiceInvoker _wcfService = new WcfServiceInvoker();
            CommonStatusPayload<List<PaymentRecForUser>> cs = _wcfService.InvokeService<IEAMIWebUIDataService,
                CommonStatusPayload<List<PaymentRecForUser>>>(svc => svc.GetAllPaymentRecordsByUser(userID));     //get from db via service

            ErrorHandlerController.CheckFatalException(cs.Status, cs.IsFatal, cs.GetCombinedMessage());
            return cs.Payload;
        }
        #endregion

        #region Manage System Queries...

        #region Fund...
        public static List<Fund> GetAllFunds(bool includeInactive, long systemID)
        {
            WcfServiceInvoker _wcfService = new WcfServiceInvoker();
            CommonStatusPayload<List<Fund>> lstFunds = _wcfService.InvokeService<IEAMIWebUIDataService, CommonStatusPayload<List<Fund>>>(svc => svc.GetAllFunds(includeInactive, systemID)); 
            ErrorHandlerController.CheckFatalException(lstFunds.Status, lstFunds.IsFatal, lstFunds.GetCombinedMessage());
            return lstFunds.Payload;
        }
        #endregion

        #region Exclusive Payment Type...
        public static List<ExclusivePmtType> GetAllExclusivePmtTypes(bool includeInactive, long systemID)
        {
            WcfServiceInvoker _wcfService = new WcfServiceInvoker();
            CommonStatusPayload<List<ExclusivePmtType>> lstExclusivePmtType = _wcfService.InvokeService<IEAMIWebUIDataService, CommonStatusPayload<List<ExclusivePmtType>>>(svc => svc.GetAllExclusivePmtTypes(includeInactive,systemID));

            ErrorHandlerController.CheckFatalException(lstExclusivePmtType.Status, lstExclusivePmtType.IsFatal, lstExclusivePmtType.GetCombinedMessage());
            return lstExclusivePmtType.Payload;
        }
        #endregion

        #region Facesheet...
        public static List<FacesheetValues> GetAllFacesheetValues(bool includeInactive, long systemID)
        {
            WcfServiceInvoker _wcfService = new WcfServiceInvoker();
            CommonStatusPayload<List<FacesheetValues>> lstFunds = _wcfService.InvokeService<IEAMIWebUIDataService, CommonStatusPayload<List<FacesheetValues>>>(svc => svc.GetAllFacesheetValues(includeInactive, systemID));
            ErrorHandlerController.CheckFatalException(lstFunds.Status, lstFunds.IsFatal, lstFunds.GetCombinedMessage());
            return lstFunds.Payload;
        }
        #endregion

        #region Funding Source...
        public static List<FundingSource> GetAllFundingSources(bool includeInactive, long systemID)
        {
            WcfServiceInvoker _wcfService = new WcfServiceInvoker();
            CommonStatusPayload<List<FundingSource>> lstFundingSources = _wcfService.InvokeService<IEAMIWebUIDataService, CommonStatusPayload<List<FundingSource>>>(svc => svc.GetAllFundingSources(includeInactive, systemID));
            ErrorHandlerController.CheckFatalException(lstFundingSources.Status, lstFundingSources.IsFatal, lstFundingSources.GetCombinedMessage());
            return lstFundingSources.Payload;
        }
        #endregion


        #region SCO Properties...
        public static List<SCOProperty> GetAllSCOProperties(bool includeInactive, long systemID)
        {
            WcfServiceInvoker _wcfService = new WcfServiceInvoker();
            CommonStatusPayload<List<SCOProperty>> lstSCOProperties = _wcfService.InvokeService<IEAMIWebUIDataService, CommonStatusPayload<List<SCOProperty>>>(svc => svc.GetAllSCOProperties(includeInactive, systemID));
            ErrorHandlerController.CheckFatalException(lstSCOProperties.Status, lstSCOProperties.IsFatal, lstSCOProperties.GetCombinedMessage());
            return lstSCOProperties.Payload;
        }

        public static SCOProperty GetAllSCOPropertyTypes()
        {
            WcfServiceInvoker _wcfService = new WcfServiceInvoker();
            var lstScoPrpties = _wcfService.InvokeService<IEAMIWebUIDataService, CommonStatusPayload<SCOProperty>>(svc => svc.GetAllSCOPropertyTypes());
            ErrorHandlerController.CheckFatalException(lstScoPrpties.Status, lstScoPrpties.IsFatal, lstScoPrpties.GetCombinedMessage());
            return lstScoPrpties.Payload;
        }
        #endregion

        #endregion
    }
}