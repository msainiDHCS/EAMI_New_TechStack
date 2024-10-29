using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OHC.EAMI.Common;
using OHC.EAMI.CommonEntity;
using OHC.EAMI.DataAccess;

using OHC.EAMI.ServiceContract;
using System.Configuration;

namespace OHC.EAMI.ServiceManager
{
    /// <summary>
    /// this class serves as validation context container that is passed to individual validators
    /// </summary>
    /// <typeparam name="TRequestEntity"></typeparam>
    public class EAMIServiceValidationContext<TRequestEntity>
    {
        public EAMIServiceValidationContext(ValidationDataContext dataContext, TRequestEntity requestEntity)
        {
            this.DataContext = dataContext;
            this.RequestEntity = requestEntity;
        }
        public TRequestEntity RequestEntity { get; private set; }
        public ValidationDataContext DataContext { get; private set; }
    }

    /// <summary>
    /// this class provides common data context and functionality for service message validation
    /// </summary>
    public class ValidationDataContext
    {

        public ValidationDataContext(EAMITransaction requestTransaction, TransactionType transactionType)
        {
            // preload db ref code values to be used for validation
            this.RefCodeTableList = RefCodeDBMgr.GetRefCodeTableList();

            // pull SOR application code and set it as actual-sender-id
            // and the actual Transaction based on the envoked service operator
            requestTransaction.ActualSenderID = this.RefCodeTableList.GetRefCodeListByTableName(enRefTables.TB_SYSTEM_OF_RECORD)[0].Code; // SENDER_RECEIVER_ID.MEDICAL_RX.ToString();
            requestTransaction.ActualTransactionType = transactionType;
            this.Transaction = requestTransaction;
            this.RequestReceivedTimeStamp = DateTime.Now;
            this.TransactionVersion = CONST.TRANSACTION_VERSION;
            EAMIMasterData eamiMasterData = new EAMIMasterData();
            //eamiMasterData.SystemProperty = new SystemProperty(); 

            eamiMasterData.SystemProperty = this.RefCodeTableList.GetRefCodeListByTableName(enRefTables.TB_System).GetRefCodeByCode<SystemProperty>(this.RefCodeTableList.GetRefCodeListByTableName(enRefTables.TB_SYSTEM_OF_RECORD)[0].Code);
            this.MaxPaymentRecAmount = eamiMasterData.SystemProperty.MaxPmtRecAmt;
            this.MaxPaymentRecordsPerTransaction = eamiMasterData.SystemProperty.MaxPmtRecPerTran;
            this.MaxFundingDetailsPerPaymentRec = eamiMasterData.SystemProperty.MaxFundingDtlPerPmtRec;
            this.MaxStatusRecordsPerTtransaction = this.MaxPaymentRecordsPerTransaction;
            this.ValidateFundingSource = eamiMasterData.SystemProperty.ValidateFundingSource;
            // need to disable when we go live
            this.IncludeExceptionErrorMsg = true;
        }

        public DateTime RequestReceivedTimeStamp { get; private set; }

        public EAMITransaction Transaction { get; private set; }

        public string TransactionVersion { get; private set; }

        public decimal MaxPaymentRecAmount { get; private set; }

        public int MaxPaymentRecordsPerTransaction { get; private set; }

        public int MaxStatusRecordsPerTtransaction { get; private set; }

        public int MaxFundingDetailsPerPaymentRec { get; private set; }

        public bool IncludeExceptionErrorMsg { get; private set; }

        public bool ValidateFundingSource { get; private set; }

        public RefCodeTableList RefCodeTableList { get; private set; }


        public int GetSenderRefCodePKID(string senderId)
        {
            return RefCodeTableList.GetRefCodeListByTableName(enRefTables.TB_SYSTEM_OF_RECORD).GetRefCodeByCode(senderId).ID;
        }

        public int GetTransactionTypePKID(string msgTransactionId)
        {
            return RefCodeTableList.GetRefCodeListByTableName(enRefTables.TB_TRANSACTION_TYPE).GetRefCodeByCode(msgTransactionId).ID;
        }

        public int GetInvoiceStatusTypePKID(string statusType)
        {
            return RefCodeTableList.GetRefCodeListByTableName(enRefTables.TB_PAYMENT_STATUS_TYPE).GetRefCodeByCode(statusType).ID;
        }

        public bool IsDuplicateTransaction(string msgTransactionId)
        {
            return DataAccess.PaymentDataSubmissionDBMgr.RequestTransactionExist(msgTransactionId);
        }

        // holds the list of expected payment rec key/value pair sets to be used in submitted payment validation
        private Dictionary<string, KvpDefinition> expectedPaymentRecKvpList = null;
        public Dictionary<string, KvpDefinition> ExpectedPaymentRecKvpList
        {
            get
            {
                if (expectedPaymentRecKvpList == null)
                {
                    expectedPaymentRecKvpList = new Dictionary<string, KvpDefinition>();
                    foreach (KvpDefinition kd in this.RefCodeTableList.GetRefCodeListByTableName(enRefTables.TB_SOR_KVP_KEY))
                    {
                        if (kd.KvpOwner == KvpOwnerEntity.PaymentRec && kd.SOR_ID == this.RefCodeTableList.GetRefCodeListByTableName(enRefTables.TB_SYSTEM_OF_RECORD).GetRefCodeByCode(this.Transaction.ActualSenderID).ID)
                        {
                            expectedPaymentRecKvpList.Add(kd.Code, kd);
                        }
                    }
                }
                return expectedPaymentRecKvpList;
            }
        }

        // holds the list of expected funding key/value pair sets to be used in submitted payment validation
        private Dictionary<string, KvpDefinition> expectedFundingKvpList = null;
        public Dictionary<string, KvpDefinition> ExpectedFundingKvpList
        {
            get
            {
                if (expectedFundingKvpList == null)
                {
                    expectedFundingKvpList = new Dictionary<string, KvpDefinition>();
                    foreach (KvpDefinition kd in this.RefCodeTableList.GetRefCodeListByTableName(enRefTables.TB_SOR_KVP_KEY))
                    {
                        if (kd.KvpOwner == KvpOwnerEntity.Funding && kd.SOR_ID == this.RefCodeTableList.GetRefCodeListByTableName(enRefTables.TB_SYSTEM_OF_RECORD).GetRefCodeByCode(this.Transaction.ActualSenderID).ID)
                        {
                            expectedFundingKvpList.Add(kd.Code, kd);
                        }
                    }
                }
                return expectedFundingKvpList;
            }
        }


        // holds existing list of payment rec numbers (based on submitted payment rec list)
        // these will be considered as duplicate values when validating submitted payment data
        private List<string> DupRecNumberList { get; set; }
        public bool IsDuplicatePaymentRecord(string paymentRecordNum, PaymentSubmissionRequest request)
        {
            if (DupRecNumberList == null)
            {
                List<string> newRecNumList = new List<string>();
                foreach (PaymentRecord pr in request.PaymentRecordList)
                {
                    newRecNumList.Add(pr.PaymentRecNumber);
                }
                DupRecNumberList = DataAccess.PaymentDataSubmissionDBMgr.GetDuplicatePaymentRecordNumbers(newRecNumList);
            }
            if (DupRecNumberList == null)
            {
                DupRecNumberList = new List<string>();
            }
            return DupRecNumberList.Contains(paymentRecordNum);
        }


        public List<string> GetExistingPaymentSetNumberList(List<string> psnList)
        {
            List<string> retList = new List<string>();
            retList = DataAccess.PaymentDataSubmissionDBMgr.GetDuplicatePaymentSetNumberList(psnList);
            return retList == null ? new List<string>() : retList;
        }


        public List<string> GetExistingPaymentRecordNumList(List<string> prnList)
        {
            List<string> retList = new List<string>();
            retList = DataAccess.PaymentDataSubmissionDBMgr.GetDuplicatePaymentRecordNumbers(prnList);
            return retList == null ? new List<string>() : retList;
        }



        internal static bool HasValidAndInvalidRecords(IEnumerable<BaseRecord> recordList)
        {
            return (HasValidPaymentRecords(recordList) && HasInvalidPaymentRecords(recordList));
        }

        internal static bool HasValidPaymentRecords(IEnumerable<BaseRecord> recordList)
        {
            return (recordList.FirstOrDefault(item => item.IsValid == true) != null);
        }

        internal static bool HasInvalidPaymentRecords(IEnumerable<BaseRecord> recordList)
        {
            return (recordList.FirstOrDefault(item => item.IsValid == false) != null);
        }


        internal static List<PaymentRecord> GetInvalidPaymentRecordList(List<PaymentRecord> recordList)
        {
            List<PaymentRecord> retList = recordList.FindAll(s => s.IsValid == false);
            return (retList != null && retList.Count > 0) ? retList : new List<PaymentRecord>();
        }

        internal static decimal GetPaymentRecordListTotalAmount(List<PaymentRecord> recordList)
        {
            return (recordList != null && recordList.Count > 0) ? recordList.Sum(s => decimal.Parse(s.Amount)) : 0;
        }

    }




}
