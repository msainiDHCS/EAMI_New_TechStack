using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OHC.EAMI.ServiceContract;
using OHC.EAMI.Common;
using OHC.EAMI.CommonEntity;
using OHC.EAMI.DataAccess;
using System.IO;
using System.Runtime.Serialization;
using Ninject.Activation;
using System.Collections;

namespace OHC.EAMI.ServiceManager
{
    /// <summary>
    /// a manager class that implements IEAMIServiceOperations interface
    /// </summary>
    public class EAMIServiceManager : IEAMIServiceOperations
    {
        public ValidationDataContext validationDataContext = null;


        #region interface operation methods


        /// <summary>
        /// implementation for EAMIPaymentSubmission service operation
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public virtual PaymentSubmissionResponse EAMIPaymentSubmission(PaymentSubmissionRequest request)
        {               
            EAMILogger.Instance.Info("EAMIPaymentSubmission():EAMI PaymentSubmissionRequest from: " + request.ReceiverID + "/" + request.SenderID + ". TransactionID=> " + request.TransactionID + ". PaymentRecordCount=> " + request.PaymentRecordCount);
            CommonStatus status = new CommonStatus(true);
            PaymentSubmissionResponse response = new PaymentSubmissionResponse();
            RequestTransaction cdeRequestTran = new RequestTransaction();

            try
            {   
                // set validation context (required first step for any service operation)
                validationDataContext = new ValidationDataContext(request, TransactionType.PaymentSubmissionRequest);
                           
                // validate payment submission request
                PaymentSubmissionValidator psv = new PaymentSubmissionValidator();
                EAMIServiceValidationContext<PaymentSubmissionRequest> svc = 
                    new EAMIServiceValidationContext<PaymentSubmissionRequest>(validationDataContext, request);
                status = psv.Execute(svc);

                // build response
                response = this.BuildPaymentSubmissionResponse(request, status);                

                //create common data entity translating data from service data entity 
                cdeRequestTran = PaymentSubmission_PopulateCDEReqestTransaction(request, response);
                
                // persist data to database
                CommonStatus insertStatus = PaymentDataSubmissionDBMgr.InsertPaymentSubmissionDataToDB(cdeRequestTran);
                if (insertStatus.Status == false)
                {
                    EAMILogger.Instance.Error("EAMIPaymentSubmission(): " + request.ReceiverID + "/" + request.SenderID + ". Error occured during InsertPaymentSubmissionDataToDB=> " + "Error=> " + insertStatus.GetCombinedMessage());
                    throw new Exception(insertStatus.GetCombinedMessage());
                }
            }
            catch (Exception ex)
            {
                EAMILogger.Instance.Error("EAMIPaymentSubmission(): " + request.ReceiverID + "/" + request.SenderID + ". UnexpectedOperationError occured=> " + ex.Message + "RequestTransactionID=> " + request.TransactionID);
                this.HandleUnexpectedOperationError(ex, status, request);                
            }

            // finalize and return response
            this.FinalizePaymentSubmissionResponse(status, response);
            return response;
        }


        /// <summary>
        /// implementation for EAMIPaymentStatusInquiry service operation
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public virtual PaymentStatusInquiryResponse EAMIPaymentStatusInquiry(PaymentStatusInquiryRequest request)
        {
            CommonStatus status = new CommonStatus(true);
            RequestTransaction cdeRequestTran = new RequestTransaction();
            PaymentStatusInquiryResponse response = new PaymentStatusInquiryResponse();

            if (request is null || string.IsNullOrEmpty(request.PaymentRecordCount))
            {
                EAMILogger.Instance.Info("EAMIPaymentStatusInquiry():No value is provided for a required field 'PaymentRecordCount' in entity PaymentStatusInquiryRequest");
            }
            else
            {
                EAMILogger.Instance.Info("EAMIPaymentStatusInquiry():EAMI PaymentStatusInquiry from: " + request.ReceiverID + "/" + request.SenderID  + ". TransactionID=> " + request.TransactionID + ". PaymentRecordCount=> " + request.PaymentRecordCount);
                try
                {
                    // set validation context (required first step for any service operation)
                    validationDataContext = new ValidationDataContext(request, TransactionType.StatusInquiryRequest);

                    StatusInquiryValidator siv = new StatusInquiryValidator();
                    EAMIServiceValidationContext<PaymentStatusInquiryRequest> svc =
                        new EAMIServiceValidationContext<PaymentStatusInquiryRequest>(validationDataContext, request);
                    status = siv.Execute(svc);

                    // get matching payment record status from system db
                    Dictionary<string, PaymentStatus> prsListFromDb = new Dictionary<string, PaymentStatus>();
                    if (status.Status)
                    {
                        List<string> recNumList = new List<string>();
                        foreach (BaseRecord br in request.PaymentRecordList)
                        {
                            if (br.IsValid && !recNumList.Contains(br.PaymentRecNumber))
                            {
                                recNumList.Add(br.PaymentRecNumber);
                            }
                        }
                        prsListFromDb = PaymentDataDbMgr.GetCurrentPaymentStatusByPaymentRecNumberList(recNumList);
                        EAMILogger.Instance.Info("EAMIPaymentStatusInquiry(): " + request.ReceiverID + "/" + request.SenderID + ". Payment_Record_Status_From_System_DB=> " + string.Join(", ", prsListFromDb.Select(kv => $"{kv.Key}: {kv.Value}")));
                    }

                    // build response
                    response = this.BuildPaymentStatusInquiryResponse(request, status, prsListFromDb);

                    //create common data entity translating data from service data entity 
                    cdeRequestTran = PaymentStatusInquiry_PopulateCDERequestTransaction(request, response);

                    // persist data to database
                    CommonStatus insertStatus = PaymentDataSubmissionDBMgr.InsertPaymentStatusInquiryToDB(cdeRequestTran);
                    if (insertStatus.Status == false)
                    {
                        EAMILogger.Instance.Error("EAMIPaymentStatusInquiry(): " + request.ReceiverID + "/" + request.SenderID + ". Error occured during InsertPaymentStatusInquiryToDB=> " + "Error=> " + insertStatus.GetCombinedMessage());
                        throw new Exception(insertStatus.GetCombinedMessage());
                    }
                }
                catch (Exception ex)
                {
                    EAMILogger.Instance.Error("EAMIPaymentStatusInquiry(): " + request.ReceiverID + "/" + request.SenderID + ". UnexpectedOperationError occured=> " + ex.Message + "RequestTransactionID=> " + request.TransactionID);
                    this.HandleUnexpectedOperationError(ex, status, request);
                }

                // finalize and send response
                this.FinalizePaymentStatusInquiryResponse(status, response);                
            }
            return response;
        }


        /// <summary>
        /// implementation for EAMIRejectedPaymentInquiry operation
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public virtual RejectedPaymentInquiryResponse EAMIRejectedPaymentInquiry(RejectedPaymentInquiryRequest request)
        {
            EAMILogger.Instance.Info("EAMIRejectedPaymentInquiry():EAMI PaymentRejectedInquiry from: " + request.SenderID + "/" + request.ReceiverID);
            CommonStatus status = new CommonStatus(true);
            RejectedPaymentInquiryResponse response = new RejectedPaymentInquiryResponse();
            RequestTransaction cdeRequestTran = new RequestTransaction();
            if (request is null || (string.IsNullOrEmpty(request.RejectedDateFrom.ToString()) && string.IsNullOrEmpty(request.RejectedDateTo.ToString())))
            {
                EAMILogger.Instance.Info("EAMIRejectedPaymentInquiry(): " + request.ReceiverID + "/" + request.SenderID + ". No value is provided for a required field 'RejectedDateFrom' and 'RejectedDateTo' in entity RejectedPaymentInquiryRequest");
            }
            else
            {
                EAMILogger.Instance.Info("EAMIRejectedPaymentInquiry(): " + request.ReceiverID + "/" + request.SenderID + ". EAMI RejectedPaymentInquiry request with transaction=> " + request.TransactionID);

                try
                {
                    validationDataContext = new ValidationDataContext(request, TransactionType.RejectedPaymentInquiryRequest);
                    RejectedPaymentInquiryValidator rpiv = new RejectedPaymentInquiryValidator();
                    EAMIServiceValidationContext<RejectedPaymentInquiryRequest> svc =
                        new EAMIServiceValidationContext<RejectedPaymentInquiryRequest>(validationDataContext, request);
                    status = rpiv.Execute(svc);

                    // get rejected payments statuses by daterange
                    List<PaymentStatus> prsListFromDB = new List<PaymentStatus>();
                    if (status.Status)
                    {
                        prsListFromDB = PaymentDataDbMgr.GetRejectedPaymentsStatusListByDateRange(request.RejectedDateFrom, request.RejectedDateTo);
                        EAMILogger.Instance.Info("EAMIRejectedPaymentInquiry(): " + request.ReceiverID + "/" + request.SenderID + ". GetRejectedPaymentsStatusListByDateRange:PaymentRecNumbers=> " + string.Join(", ", prsListFromDB.Select(kv => kv.PaymentRecNumber)));
                    }

                    // build response
                    response = this.BuildRejectedPaymentInquiryResponse(request, status, prsListFromDB);

                    //create common data entity translating data from service data entity 
                    cdeRequestTran = RejectedPaymentInquiry_PopulateCDERequestTransaction(request, response);

                    // persist data to database
                    CommonStatus insertStatus = PaymentDataSubmissionDBMgr.InsertRejectedPaymentStatusInquiryToDB(cdeRequestTran);
                    if (insertStatus.Status == false)
                    {
                        EAMILogger.Instance.Error("EAMIRejectedPaymentInquiry(): " + request.ReceiverID + "/" + request.SenderID + ". Error occured during InsertRejectedPaymentStatusInquiryToDB=> " + "Error=> " + insertStatus.GetCombinedMessage());
                        throw new Exception(insertStatus.GetCombinedMessage());
                    }
                }
                catch (Exception ex)
                {
                    EAMILogger.Instance.Error("EAMIRejectedPaymentInquiry(): " + request.ReceiverID + "/" + request.SenderID + ". UnexpectedOperationError occured=> " + ex.Message + "RequestTransactionID=> " + request.TransactionID);
                    this.HandleUnexpectedOperationError(ex, status, request);
                }

                // finalize and send response
                this.FinalizeRejectedPaymentStatusInquiryResponse(status, response);
            }
            return response;
        }

       
        /// <summary>
        /// implementation for Ping service operation
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public virtual PingResponse Ping(PingRequest request)
        {
            EAMILogger.Instance.Info("Ping():Ping Request CT:" + request.ClientTimeStamp.ToString() + "  and ST:" + DateTime.Now.ToString()); 
            // here we set ASender (A prefex stand for Actual) based on client identity
            request.ASenderID = SENDER_RECEIVER_ID.CAPMAN.ToString();

            PingResponse pResp = new PingResponse();
            pResp.SenderID = request.ReceiverID;
            pResp.ReceiverID = request.SenderID;
            pResp.ServerTimeStamp = DateTime.Now;
            pResp.ClientTimeStamp = request.ClientTimeStamp;
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("-- EAMI Service Ping Response --");
            sb.AppendLine("Ping Request From/To     : " + request.SenderID + " / " + request.ReceiverID);
            sb.AppendLine("Ping Response From/To    : " + pResp.SenderID + " / " + pResp.ReceiverID);
            sb.AppendLine("Client Request TimeStamp : " + pResp.ClientTimeStamp.ToString());
            sb.AppendLine("Server Response TimeStamp: " + pResp.ServerTimeStamp.ToString());
            sb.AppendLine();
            EAMILogger.Instance.Info("Ping():Ping Response:" + sb.ToString());
            pResp.ResponseMessage = sb.ToString();
            return pResp;            
        }


        #endregion


        #region supporting private methods


        /// <summary>
        /// builds initial PaymentSubmissionResponse instance (less payment rec statuses) as response to EAMIPaymentSubmission operator
        /// </summary>
        /// <param name="request"></param>
        /// <param name="validationStatus"></param>
        /// <returns></returns>
        private PaymentSubmissionResponse BuildPaymentSubmissionResponse(PaymentSubmissionRequest request, CommonStatus validationStatus)
        {
            PaymentSubmissionResponse response = new PaymentSubmissionResponse();
            DateTime statusDate = DateTime.Now;

            // build transaction response
            response = BuildTransactionResponse(request, response) as PaymentSubmissionResponse;            
            // set response status
            if (validationStatus.Status == false)
            {
                response.ResponseStatusCode = TRANSACTION_STATUS.Rejected.ToString();
                response.ResponseMessage = validationStatus.GetCombinedMessage();
                EAMILogger.Instance.Info("BuildPaymentSubmissionResponse()=> " + response.ResponseStatusCode + ". ResponseMessage=> " + response.ResponseMessage);
            }
            else
            {
                response.ResponseStatusCode = 
                    ValidationDataContext.HasValidAndInvalidRecords(request.PaymentRecordList) ? 
                    TRANSACTION_STATUS.AcceptedPartially.ToString() : 
                    TRANSACTION_STATUS.Accepted.ToString();
                EAMILogger.Instance.Info("BuildPaymentSubmissionResponse()=> " + response.ResponseStatusCode);
            }

            // update payment statuses
            StringBuilder sb = new StringBuilder();
            response.PaymentRecordStatuList = new List<PaymentRecordStatus>();
            foreach (PaymentRecord pr in request.PaymentRecordList)
            {
                PaymentRecordStatus prs = new PaymentRecordStatus();
                prs.PaymentRecNumber = pr.PaymentRecNumber;
                prs.PaymentRecNumberExt = pr.PaymentRecNumberExt;
                prs.PaymentSetNumber = pr.PaymentSetNumber;
                prs.PaymentSetNumberExt = pr.PaymentSetNumberExt;
                // request payment becomes invalid if overall status is false otherwise it maintains its original status
                pr.IsValid = (validationStatus.Status && pr.IsValid);                
                prs.StatusCode = pr.IsValid ? PAYMENT_RECORD_STATUS.Accepted.ToString() : PAYMENT_RECORD_STATUS.RejectedFD.ToString();
                prs.StatusNote = pr.ValidationMessage;
                prs.StatusDate = statusDate;
                response.PaymentRecordStatuList.Add(prs);
            }
            return response;
        }


        /// <summary>
        /// builds transaction response
        /// </summary>
        /// <param name="tRequest"></param>
        /// <param name="tResponse"></param>
        /// <returns></returns>
        private EAMITransaction BuildTransactionResponse(EAMITransaction tRequest, EAMITransaction tResponse)
        {
            // set response transaction values
            tResponse.ReceiverID = tRequest.ActualSenderID;
            tResponse.SenderID = SENDER_RECEIVER_ID.EAMI.ToString();
            tResponse.TimeStamp = DateTime.Now;

            switch (tRequest.ActualTransactionType)
            {
                case TransactionType.PaymentSubmissionRequest:
                    tResponse.TransactionType = TransactionType.PaymentSubmissionResponse;
                    break;
                case TransactionType.StatusInquiryRequest:
                    tResponse.TransactionType = TransactionType.StatusInquiryResponse;
                    break;
                case TransactionType.RejectedPaymentInquiryRequest:
                    tResponse.TransactionType = TransactionType.RejectedPaymentInquiryResponse;
                    break;
            }            

            tResponse.TransactionID = tRequest.TransactionID; // Guid.NewGuid().ToString();
            tResponse.TransactionVersion = validationDataContext.TransactionVersion;
            return tResponse;
        }


        /// <summary>
        /// builds PaymentStatusInquiryResponse instance as response to EAMIPaymentStatusInquiry service request operator
        /// </summary>
        /// <param name="request"></param>
        /// <param name="validationStatus"></param>
        /// <param name="prsListFromDb"></param>
        /// <returns></returns>
        private PaymentStatusInquiryResponse BuildPaymentStatusInquiryResponse(PaymentStatusInquiryRequest request, CommonStatus validationStatus, Dictionary<string, PaymentStatus> prsListFromDb)
        {
            PaymentStatusInquiryResponse response = new PaymentStatusInquiryResponse();

            // build transaction response
            response = BuildTransactionResponse(request, response) as PaymentStatusInquiryResponse;

            // set response status
            if (validationStatus.Status == false)
            {
                response.ResponseStatusCode = TRANSACTION_STATUS.Rejected.ToString();
                response.ResponseMessage = validationStatus.GetCombinedMessage();
            }
            else
            {
                response.ResponseStatusCode = ValidationDataContext.HasValidAndInvalidRecords(request.PaymentRecordList) ?
                                        TRANSACTION_STATUS.AcceptedPartially.ToString() :
                                        TRANSACTION_STATUS.Accepted.ToString();                                
            }

            // set response record status
            response.PaymentRecordStatuList = new List<PaymentRecordStatusPlus>();
            foreach (BaseRecord br in request.PaymentRecordList)
            {
                PaymentRecordStatusPlus prsp = new PaymentRecordStatusPlus();
                prsp.PaymentRecNumber = br.PaymentRecNumber;
                prsp.StatusDate = response.TimeStamp;

                // these values should come from database
                if (br.IsValid == false)
                {
                    prsp.StatusCode = PAYMENT_RECORD_STATUS.RejectedFD.ToString();
                    prsp.StatusNote = br.ValidationMessage;
                }
                else
                {
                    if (prsListFromDb.ContainsKey(br.PaymentRecNumber))
                    {
                        PaymentStatus ps = prsListFromDb[br.PaymentRecNumber];
                        prsp.PaymentRecNumberExt = ps.PaymentRecNumberExt;
                        prsp.PaymentSetNumber = ps.PaymentSetNumber;
                        prsp.PaymentSetNumberExt = ps.PaymentSetNumberExt;
                        prsp.StatusCode = ps.ExternalStatusType.Code;
                        prsp.StatusDate = ps.StatusDate;
                        prsp.StatusNote = ps.ExternalStatusType.Code == "REJECTED" ? 
                                        string.Concat(ps.StatusNote, " [", ps.CreatedBy, "]") : 
                                        ps.StatusNote;
                        prsp.ClaimScheduleNumber = ps.ClaimScheduleNumber;
                        prsp.ClaimScheduleDate = ps.ClaimScheduleDate;
                        prsp.WarrantNumber = ps.WarrantNumber;
                        prsp.WarrantDate = ps.WarrantDate;
                        prsp.WarrantAmount = ps.WarrantAmount.ToString();
                        prsp.GenericNameValueList = new Dictionary<string, string>();
                        prsp.GenericNameValueList.Add("IsEFT", ps.PaymentMethodType.Code == "EFT" ? "true" : "false");
                    }
                    else
                    {
                        prsp.StatusCode = PAYMENT_RECORD_STATUS.RejectedFD.ToString();
                        prsp.StatusNote = "The payment record '" + br.PaymentRecNumber + "' not found in the system";
                    }
                }
                response.PaymentRecordStatuList.Add(prsp);
            }
            return response;
        }


        /// <summary>
        /// builds RejectedPaymentInquiryResponse instance as response to EAMIPaymentStatusInquiry service request operator
        /// </summary>
        /// <param name="request"></param>
        /// <param name="validationStatus"></param>
        /// <param name="prsListFromDb"></param>
        /// <returns></returns>
        private RejectedPaymentInquiryResponse BuildRejectedPaymentInquiryResponse(RejectedPaymentInquiryRequest request, CommonStatus validationStatus, List<PaymentStatus> prsListFromDb)
        {
            RejectedPaymentInquiryResponse response = new RejectedPaymentInquiryResponse();

            // build transaction response
            response = this.BuildTransactionResponse(request, response) as RejectedPaymentInquiryResponse;

            // set response status
            if (validationStatus.Status == false)
            {
                response.ResponseStatusCode = TRANSACTION_STATUS.Rejected.ToString();
                response.ResponseMessage = validationStatus.GetCombinedMessage();
            }
            else
            {
                response.ResponseStatusCode = TRANSACTION_STATUS.Accepted.ToString();
                response.PaymentRecordStatuList = new List<PaymentRecordStatus>();
                foreach (PaymentStatus ps in prsListFromDb)
                {
                    PaymentRecordStatus prs = new PaymentRecordStatus();
                    prs.PaymentRecNumber = ps.PaymentRecNumber;
                    prs.PaymentRecNumberExt = ps.PaymentRecNumberExt;
                    prs.PaymentSetNumber = ps.PaymentSetNumber;
                    prs.PaymentSetNumberExt = ps.PaymentSetNumberExt;
                    prs.StatusCode = ps.ExternalStatusType.Code;
                    prs.StatusDate = ps.StatusDate;
                    prs.StatusNote = string.Concat(ps.StatusNote, " [", ps.CreatedBy, "]");                    
                    response.PaymentRecordStatuList.Add(prs);
                }
            }                                        
            return response;
        }


        private void HandleUnexpectedOperationError(Exception exception, CommonStatus status, EAMITransaction transaction)
        {
            // log error
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("EAMI Interface Operation error !");
            sb.AppendLine("TransactionType : " + transaction.TransactionType.ToString());
            sb.AppendLine("TransactionID   : " + transaction.TransactionID);
            EAMILogger.Instance.Error(sb.ToString()); 
            EAMILogger.Instance.Error(exception);

            // update response with exception details
            status.Status = false;
            status.AddMessageDetail("ERROR: Unexpected error while processing payment submission request TRAN_ID: " + 
                transaction.TransactionID + ". Please contact EAMI system support.");

            if (validationDataContext.IncludeExceptionErrorMsg)
            {
                sb = new StringBuilder();
                sb.AppendLine("Exception Details:");
                sb.AppendLine("Exception Msg: " + exception.Message);
                sb.AppendLine("Source: " + exception.Source);
                sb.AppendLine("StackTrace: " + exception.StackTrace);
                status.AddMessageDetail(sb.ToString());
            }
        }

        /// <summary>
        /// finalize PaymentSubmissionResponse post insert response
        /// </summary>
        /// <param name="status"></param>
        /// <param name="request"></param>
        /// <param name="response"></param>
        private void FinalizePaymentSubmissionResponse(CommonStatus postInsertStatus, PaymentSubmissionResponse response)
        {
            DateTime statusDate = DateTime.Now;                     
            // set response status
            if (postInsertStatus.Status == false)
            {                
                response.ResponseStatusCode = TRANSACTION_STATUS.Rejected.ToString();
                response.ResponseMessage = postInsertStatus.GetCombinedMessage();

                StringBuilder sb = new StringBuilder("EAMI PaymentSubmissionResponse from " + response.SenderID + "/" + response.ReceiverID);
                EAMILogger.Instance.Info("FinalizePaymentSubmissionResponse(): " + sb.ToString() + ". TRANSACTION_STATUS => " + response.ResponseStatusCode + ". ResponseMessage=> " + response.ResponseMessage);

                foreach (PaymentRecordStatus psr in response.PaymentRecordStatuList)
                {
                    psr.StatusCode = PAYMENT_RECORD_STATUS.RejectedFD.ToString();
                    psr.StatusDate = statusDate;
                }
            }
            // update latest timestamp
            response.TimeStamp = statusDate;            
        }


        /// <summary>
        /// finalize PaymentStatusInquiryResponse post insert response
        /// </summary>
        /// <param name="status"></param>
        /// <param name="request"></param>
        /// <param name="response"></param>
        private void FinalizePaymentStatusInquiryResponse(CommonStatus postInsertStatus, PaymentStatusInquiryResponse response)
        {
            // set response status
            if (postInsertStatus.Status == false)
            {
                response.ResponseStatusCode = TRANSACTION_STATUS.Rejected.ToString();
                response.ResponseMessage = postInsertStatus.GetCombinedMessage();
                response.PaymentRecordStatuList = new List<PaymentRecordStatusPlus>();                
            }

            // update latest timestamp
            response.TimeStamp = DateTime.Now;
        }


        /// <summary>
        /// finalize RejectedPaymentStatusInquiryResponse post insert response
        /// </summary>
        /// <param name="status"></param>
        /// <param name="request"></param>
        /// <param name="response"></param>
        private void FinalizeRejectedPaymentStatusInquiryResponse(CommonStatus postInsertStatus, RejectedPaymentInquiryResponse response)
        {
            // set response status
            if (postInsertStatus.Status == false)
            {
                response.ResponseStatusCode = TRANSACTION_STATUS.Rejected.ToString();
                response.ResponseMessage = postInsertStatus.GetCombinedMessage();
                response.PaymentRecordStatuList = new List<PaymentRecordStatus>();                
            }

            // update latest timestamp
            response.TimeStamp = DateTime.Now;
        }


        /// <summary>
        /// copy PaymentSubmission data from service-data-entity (sde) to common-data-entity (cde)
        /// </summary>
        /// <param name="sdeRequest"></param>
        /// <param name="sdeResponse"></param>
        /// <returns></returns>
        private RequestTransaction PaymentSubmission_PopulateCDEReqestTransaction(PaymentSubmissionRequest sdeRequest, PaymentSubmissionResponse sdeResponse)
        {
            RequestTransaction cdeRequestTran = new RequestTransaction();
            cdeRequestTran.MsgTotalRecAmount = sdeRequest.PaymentRecordTotalAmount;
            cdeRequestTran.MsgTransactionRecCount = sdeRequest.PaymentRecordCount;
            PopulateCDERequestTransaction(cdeRequestTran, sdeRequest, sdeResponse);
            PopulateCDEPayments(cdeRequestTran, sdeRequest);
            return cdeRequestTran;
        }

        /// <summary>
        /// copy PaymentStatusInquiry data from service-data-entity (sde) to common-data-entity (cde)
        /// </summary>
        /// <param name="sdeRequest"></param>
        /// <param name="sdeResponse"></param>
        /// <returns></returns>
        private RequestTransaction PaymentStatusInquiry_PopulateCDERequestTransaction(PaymentStatusInquiryRequest sdeRequest, PaymentStatusInquiryResponse sdeResponse)
        {
            RequestTransaction cdeRequestTran = new RequestTransaction();
            cdeRequestTran.MsgTransactionRecCount = sdeRequest.PaymentRecordCount;
            PopulateCDERequestTransaction(cdeRequestTran, sdeRequest, sdeResponse);            
            return cdeRequestTran;
        }

        /// <summary>
        /// copy RejectedPaymentInquiry data from service-data-entity (sde) to common-data-entity (cde)
        /// </summary>
        /// <param name="sdeRequest"></param>
        /// <param name="sdeResponse"></param>
        /// <returns></returns>
        private RequestTransaction RejectedPaymentInquiry_PopulateCDERequestTransaction(RejectedPaymentInquiryRequest sdeRequest, RejectedPaymentInquiryResponse sdeResponse)
        {
            RequestTransaction cdeRequestTran = new RequestTransaction();            
            cdeRequestTran.RejectedPaymentDateFrom = sdeRequest.RejectedDateFrom;
            cdeRequestTran.RejectedPaymentDateTo = sdeRequest.RejectedDateTo;
            PopulateCDERequestTransaction(cdeRequestTran, sdeRequest, sdeResponse);
            return cdeRequestTran;
        }

        /// <summary>
        /// copy EAMITransaction (service-data-entity) data to RequestTransaction (common-data-entity)
        /// </summary>
        /// <param name="cdeRequestTran"></param>
        /// <param name="sdeRequestTran"></param>
        /// <param name="sdeResponseTran"></param>
        private void PopulateCDERequestTransaction(RequestTransaction cdeRequestTran, EAMITransaction sdeRequestTran, PaymentStatusResponse sdeResponseTran)
        {
            // transfer data from service data entity to common data entities
            cdeRequestTran.RequestSentTimeStamp = sdeRequestTran.TimeStamp;
            cdeRequestTran.RequestReceivedTimeStamp = validationDataContext.RequestReceivedTimeStamp;
            cdeRequestTran.SenderID = sdeRequestTran.SenderID;
            cdeRequestTran.SOR_ID = validationDataContext.GetSenderRefCodePKID(sdeRequestTran.ActualSenderID);
            cdeRequestTran.ReqMsgTransactionID = sdeRequestTran.TransactionID;
            cdeRequestTran.ReqMsgTransactionType = sdeRequestTran.TransactionType.ToString();
            cdeRequestTran.TransactionTypeId = validationDataContext.GetTransactionTypePKID(sdeRequestTran.ActualTransactionType.ToString());
            cdeRequestTran.MsgTransactionVersion = sdeRequestTran.TransactionVersion;            

            cdeRequestTran.RespMsgTransactionID = sdeResponseTran.TransactionID;
            cdeRequestTran.RespMsgTransactionType = sdeResponseTran.TransactionType.ToString();
            cdeRequestTran.ResponseTimeStamp = sdeResponseTran.TimeStamp;            
            cdeRequestTran.RespStatusTypeID = (int)EnumUtil.ToEnum<TRANSACTION_STATUS>(sdeResponseTran.ResponseStatusCode);            
            cdeRequestTran.ResponseMessage = sdeResponseTran.ResponseMessage;
        }

        /// <summary>
        /// copy List<PaymentRecord> (service-data-entity) data to List<InvoicePayment> (common-data-entity) 
        /// </summary>
        /// <param name="cdeRequestTran"></param>
        /// <param name="sdeRequest"></param>
        private void PopulateCDEPayments(RequestTransaction cdeRequestTran, PaymentSubmissionRequest sdeRequest)
        {
            cdeRequestTran.PaymentRecList = new List<PaymentRec>();            

            if (sdeRequest.PaymentRecordList != null && sdeRequest.PaymentRecordList.Count > 0)
            {
                foreach (PaymentRecord sdePr in sdeRequest.PaymentRecordList)
                {
                    PaymentRecTr cdePr = new PaymentRecTr();
                    cdePr.UniqueNumber = sdePr.PaymentRecNumber;
                    cdePr.PaymentRecNumberExt = sdePr.PaymentRecNumberExt;
                    cdePr.PaymentType = sdePr.PaymentType;
                    cdePr.PaymentDate = sdePr.PaymentDate;
                    cdePr.Amount = decimal.Parse(sdePr.Amount);
                    cdePr.FiscalYear = sdePr.FiscalYear;
                    cdePr.IndexCode = sdePr.IndexCode;
                    cdePr.ObjDetailCode = sdePr.ObjectDetailCode;
                    cdePr.ObjAgencyCode = sdePr.ObjectAgencyCode;
                    cdePr.PCACode = sdePr.PCACode;
                    cdePr.ApprovedBy = sdePr.ApprovedBy;
                    cdePr.PaymentSetNumber = sdePr.PaymentSetNumber;
                    cdePr.PaymentSetNumberExt = sdePr.PaymentSetNumberExt;                    
                    
                    // payment status
                    cdePr.CurrentStatus = new PaymentStatus();
                    cdePr.CurrentStatus.StatusType = sdePr.IsValid ? 
                        validationDataContext.RefCodeTableList.GetRefCodeListByTableName(enRefTables.TB_PAYMENT_STATUS_TYPE).GetRefCodeByCode("RECEIVED") :
                        validationDataContext.RefCodeTableList.GetRefCodeListByTableName(enRefTables.TB_PAYMENT_STATUS_TYPE).GetRefCodeByCode("RETURNED_TO_SOR");
                    cdePr.CurrentStatus.StatusDate = DateTime.Now;
                    cdePr.CurrentStatus.StatusNote = sdePr.IsValid ? string.Empty : sdePr.ValidationMessage;
                    cdePr.CurrentStatus.CreatedBy = "system";
                    
                    // payee entity                    
                    cdePr.PayeeInfo = new PaymentExcEntityInfo();
                    cdePr.PayeeInfo.PEE = new PayeeEntity();
                    cdePr.PayeeInfo.PEE.Code = sdePr.PayeeInfo.EntityID;
                    cdePr.PayeeInfo.PEE.EntityIDType = sdePr.PayeeInfo.EntityIDType;
                    cdePr.PayeeInfo.PEE.Description = sdePr.PayeeInfo.Name;
                    cdePr.PayeeInfo.PEE.EntityName = sdePr.PayeeInfo.Name;
                    cdePr.PayeeInfo.PEE_IdSfx = sdePr.PayeeInfo.EntityIDSuffix;
                    cdePr.PayeeInfo.PEE_AddressLine1 = sdePr.PayeeInfo.AddressLine1;
                    cdePr.PayeeInfo.PEE_AddressLine2 = sdePr.PayeeInfo.AddressLine2;
                    cdePr.PayeeInfo.PEE_AddressLine3 = sdePr.PayeeInfo.AddressLine3;
                    cdePr.PayeeInfo.PEE_City = sdePr.PayeeInfo.AddressCity;
                    cdePr.PayeeInfo.PEE_State = sdePr.PayeeInfo.AddressState;
                    cdePr.PayeeInfo.PEE_Zip = sdePr.PayeeInfo.AddressZip;
                    cdePr.PayeeInfo.PEE.EntityEIN = sdePr.PayeeInfo.EIN;                    
                    cdePr.PayeeInfo.PEE_VendorTypeCode = sdePr.PayeeInfo.VendorTypeCode;
                   
                    // populate kvp and funding lists as well as trace kvp and xml fields
                    cdePr.PaymentKvpList = new Dictionary<string, string>();
                    cdePr.TrPaymentKvpListXML = PopulateCDEKvp(cdePr.PaymentKvpList, sdePr.GenericNameValueList);
                    cdePr.TrFundingDetialListXML = PopulateCDEFundingDetails(cdePr, sdePr);

                    // add inv instance to list
                    cdeRequestTran.PaymentRecList.Add(cdePr);
                }
            }
        }

        /// <summary>
        /// copy List<FundingDetail> (service-data-entity) data to List<InvoiceFundingDetail> (common-data-entity)
        /// serializes List<FundingDetail> instance and returns as xml text 
        /// </summary>
        /// <param name="cdePr"></param>
        /// <param name="sdePr"></param>
        /// <returns></returns>
        private string PopulateCDEFundingDetails(PaymentRec cdePr, PaymentRecord sdePr)
        {
            string xmlText = string.Empty;
            cdePr.FundingDetailList = new List<PaymentFundingDetail>();
            if (sdePr.FundingDetailList != null && sdePr.FundingDetailList.Count > 0)
            {
                foreach (FundingDetail fd in sdePr.FundingDetailList)
                {
                    PaymentFundingDetail pfd = new PaymentFundingDetail();
                    pfd.FundingSourceName = fd.FundingSourceName;
                    pfd.FFPAmount = decimal.Parse(fd.FFPAmount);
                    pfd.SGFAmount = decimal.Parse(fd.SGFAmount);
                    pfd.FiscalYear = fd.FiscalYear;
                    pfd.FiscalQuarter = fd.FiscalQuarter;                    
                    pfd.Title = fd.Title;

                    // populate kvp and funding lists as well as trace kvp and xml fields
                    pfd.FundingKvpList = new Dictionary<string, string>();
                    PopulateCDEKvp(pfd.FundingKvpList, fd.GenericNameValueList);
                    cdePr.FundingDetailList.Add(pfd);
                }
                xmlText = Serialize(sdePr.FundingDetailList, sdePr.FundingDetailList.GetType());
                xmlText = CleanUpSerializedXml(xmlText);
            }
            return xmlText;
        }

        /// <summary>
        /// copy kvp from one dictionary to another
        /// serializes sdeKvpList and returns as xml text
        /// </summary>
        /// <param name="cdeKvpList"></param>
        /// <param name="sdeKvpList"></param>
        /// <returns></returns>
        private string PopulateCDEKvp(Dictionary<string, string> cdeKvpList, Dictionary<string, string> sdeKvpList)
        {
            string xmlText = string.Empty;            
            if (sdeKvpList != null && sdeKvpList.Count > 0)
            {
                foreach (KeyValuePair<string, string> kvp in sdeKvpList)
                {
                    cdeKvpList.Add(kvp.Key, kvp.Value);
                }

                xmlText = Serialize(sdeKvpList, sdeKvpList.GetType());
                xmlText = CleanUpSerializedXml(xmlText);
            }
            return xmlText;
        }
        

        public static object Deserialize(string xml, Type toType)
        {
            using (Stream stream = new MemoryStream())
            {
                byte[] data = System.Text.Encoding.UTF8.GetBytes(xml);
                stream.Write(data, 0, data.Length);
                stream.Position = 0;
                DataContractSerializer deserializer = new DataContractSerializer(toType);
                return deserializer.ReadObject(stream);
            }
        }


        internal static string Serialize(object obj, Type type)
        {
            DataContractSerializer serializer = new DataContractSerializer(type);
            using (MemoryStream memoryStream = new MemoryStream())
            {
                serializer.WriteObject(memoryStream, obj);
                //string xml =  @"<?xml version=""1.0"" encoding=""utf-8""?>" + Encoding.UTF8.GetString(memoryStream.GetBuffer());
                return Encoding.UTF8.GetString(memoryStream.GetBuffer());
            }
        }


        private static string CleanUpSerializedXml(string xmltext)
        {
            do
            {
                xmltext = xmltext.Replace("\0\0\0\0\0\0\0\0", string.Empty);
                xmltext = xmltext.Replace("\0\0\0\0", string.Empty);
                xmltext = xmltext.Replace("\0", string.Empty);
            } while (xmltext.Contains("\0") || xmltext.Contains("\0\0\0\0") || xmltext.Contains("\0\0\0\0\0\0\0\0"));
            return xmltext;
        }

        #endregion
    }
}
