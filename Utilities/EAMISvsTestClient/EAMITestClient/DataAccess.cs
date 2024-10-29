
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Runtime.InteropServices;

using System.Security;
using System.Security.Principal;
using System.Diagnostics;
using System.IO;

using EAMITestClient.EAMISvcRef;
//using OHC.EAMI.ServiceContract;
//using OHC.EAMI.ServiceManager;

namespace EAMITestClient
{
    
    public static class DataAccess
    {

        internal const string DOMAIN_NAME = "intra.dhs.ca.gov";

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool LogonUser(
                string lpszUsername,
                string lpszDomain,
                string lpszPassword,
                int dwLogonType,
                int dwLogonProvider,
                out IntPtr phToken);

        public static bool TestConnection(string serverName, string dbName, string username, string password)
        {
            bool goodConnection = false;
            string connString = ConfigurationManager.ConnectionStrings["CAPMANData"].ConnectionString;
            connString = connString.Replace(GetDefaultDBConnectionStringServer(), serverName);
            connString = connString.Replace(GetDefaultDBConnectionStringDatabase(), dbName);

            try
            {
                IntPtr userToken = IntPtr.Zero;
                bool success = LogonUser(
                              username,
                              DOMAIN_NAME,
                              password,
                              2, //(int) AdvApi32Utility.LogonType.LOGON32_LOGON_INTERACTIVE, //2
                              0, //(int) AdvApi32Utility.LogonProvider.LOGON32_PROVIDER_DEFAULT, //0
                              out userToken);

                if (!success)
                {
                    throw new SecurityException("Logon user failed");
                }

                using (WindowsIdentity.Impersonate(userToken))
                {
                    using (SqlConnection sqlConn = new SqlConnection(connString))
                    {
                        sqlConn.Open();
                        goodConnection = true;
                    }
                }                
            }
            catch { }           
            return goodConnection;
        }

        public static string GetDefaultDBConnectionStringServer()
        {
            string connString = ConfigurationManager.ConnectionStrings["CAPMANData"].ConnectionString;
            SqlConnection sqlConn = new SqlConnection(connString);
            return sqlConn.DataSource;
        }

        public static string GetDefaultDBConnectionStringDatabase()
        {
            string connString = ConfigurationManager.ConnectionStrings["CAPMANData"].ConnectionString;
            SqlConnection sqlConn = new SqlConnection(connString);
            return sqlConn.Database;
        }


        public static PaymentSubmissionRequest GetPaymentSubmissionRequestFromDB(string serverName, string dbName, string username, string password, string transactionId, DbLogin loginForm)
        {
            PaymentSubmissionRequest psr = new PaymentSubmissionRequest();
            string connString = ConfigurationManager.ConnectionStrings["CAPMANData"].ConnectionString;
            connString = connString.Replace(GetDefaultDBConnectionStringServer(), serverName);
            connString = connString.Replace(GetDefaultDBConnectionStringDatabase(), dbName);

            IntPtr userToken = IntPtr.Zero;
            bool success = LogonUser(
                          username,
                          DOMAIN_NAME,
                          password,
                          2, //(int) AdvApi32Utility.LogonType.LOGON32_LOGON_INTERACTIVE, //2
                          0, //(int) AdvApi32Utility.LogonProvider.LOGON32_PROVIDER_DEFAULT, //0
                          out userToken);

            try
            {
                if (!success)
                {
                    throw new SecurityException("Logon user failed");
                }

                string paymentSubmitTransactionQuery = GetQueryTextFromFile(Helper.GetExecutingDefaultXmlFilesLocationPath() + "\\ImportPaymentSubmissionTransaction.sql");                
                DataSet ds = new DataSet();

                using (WindowsIdentity.Impersonate(userToken))
                {
                    using (SqlConnection conn = new SqlConnection(connString))
                    {
                        conn.Open();                        
                        using (SqlCommand cmd = new SqlCommand(paymentSubmitTransactionQuery, conn))
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.Add(new SqlParameter("@transactionid", transactionId));

                            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                            {
                                da.Fill(ds);
                            }                            
                        }                                                                       
                    }                    
                }

                if (ds.Tables.Count > 1 && ds.Tables[0].Rows.Count > 0)
                {
                    // load pymt submission transaction
                    psr = GetPaymentSubmissionRequestFromDataRow(ds.Tables[0].Rows[0]);


                    // load payment record list (with funding details)
                    List<PaymentRecord> prList = new List<PaymentRecord>();
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        PaymentRecord pr = GetPaymentRecFromDataRow(dr, ds.Tables[1]);
                        prList.Add(pr);
                    }
                    psr.PaymentRecordList = prList;
                    loginForm.UpdateLoadStatus(ds.Tables[0].Rows.Count, ds.Tables[0].Rows.Count);
                }
                else
                {
                    psr = null;
                }
                
            }
            catch (Exception ex)
            {
                throw ex;
            }                        

            return psr;
        }


        public static PaymentStatusInquiryRequest GetPaymentStatusInquiryRequestFromDB(string serverName, string dbName, string username, string password, string transactionId, DbLogin loginForm)
        {
            PaymentStatusInquiryRequest psir = new PaymentStatusInquiryRequest();
            string connString = ConfigurationManager.ConnectionStrings["CAPMANData"].ConnectionString;
            connString = connString.Replace(GetDefaultDBConnectionStringServer(), serverName);
            connString = connString.Replace(GetDefaultDBConnectionStringDatabase(), dbName);

            IntPtr userToken = IntPtr.Zero;
            bool success = LogonUser(
                          username,
                          DOMAIN_NAME,
                          password,
                          2, //(int) AdvApi32Utility.LogonType.LOGON32_LOGON_INTERACTIVE, //2
                          0, //(int) AdvApi32Utility.LogonProvider.LOGON32_PROVIDER_DEFAULT, //0
                          out userToken);

            try
            {
                if (!success)
                {
                    throw new SecurityException("Logon user failed");
                }

                string paymentStatusInquiryTransactionQuery = GetQueryTextFromFile(Helper.GetExecutingDefaultXmlFilesLocationPath() + "\\ImportPaymentStatusInquiryTransaction.sql");
                DataSet ds = new DataSet();

                using (WindowsIdentity.Impersonate(userToken))
                {
                    using (SqlConnection conn = new SqlConnection(connString))
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand(paymentStatusInquiryTransactionQuery, conn))
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.Add(new SqlParameter("@transactionid", transactionId));

                            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                            {
                                da.Fill(ds);
                            }
                        }
                    }
                }

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    // load pymt status inquiry transaction
                    psir = GetPaymentStatusInquiryRequestFromDataRow(ds.Tables[0].Rows[0]);


                    // load payment record list (with funding details)
                    List<PaymentRecord> prList = new List<PaymentRecord>();
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        PaymentRecord pr = GetPaymentRecFromDataRow(dr, null);
                        prList.Add(pr);
                    }

                    // convert payment rec list to base payment list
                    psir.PaymentRecordList = prList.ConvertAll(x => new BaseRecord
                    {
                        PaymentRecNumber = x.PaymentRecNumber
                    });

                    loginForm.UpdateLoadStatus(ds.Tables[0].Rows.Count, ds.Tables[0].Rows.Count);

                }
                else
                {
                    psir = null;
                }
                

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return psir;
        }


        private static PaymentSubmissionRequest GetPaymentSubmissionRequestFromDataRow(DataRow dr)
        {
            PaymentSubmissionRequest psr = new PaymentSubmissionRequest();
            PopulateRequestTransactionFromDataRow(dr, psr);
            psr.TransactionType = TransactionType.PaymentSubmissionRequest;
            psr.PaymentRecordCount = dr["TransactionRecCount"].ToString();
            psr.PaymentRecordTotalAmount = decimal.Parse(dr["TransactionAmount"].ToString()).ToString("0.00");
            // string.Format("{0:n}", decimal.Parse(dr["Amount"].ToString()));
            return psr;
        }

        private static PaymentStatusInquiryRequest GetPaymentStatusInquiryRequestFromDataRow(DataRow dr)
        {
            PaymentStatusInquiryRequest psir = new PaymentStatusInquiryRequest();
            PopulateRequestTransactionFromDataRow(dr, psir);
            psir.TransactionType = TransactionType.StatusInquiryRequest;
            psir.PaymentRecordCount = dr["TransactionRecCount"].ToString();
            return psir;
        }


        private static void PopulateRequestTransactionFromDataRow(DataRow dr, EAMITransaction tran)
        {
            tran.SenderID = "CAPMAN";
            tran.ReceiverID = "EAMI";
            tran.TransactionID = dr["Transaction_Id"].ToString();            
            tran.TransactionVersion = "1.1";
            tran.TimeStamp = DateTime.Now;
            // string.Format("{0:n}", decimal.Parse(dr["Amount"].ToString()));
        }


        private static PaymentRecord GetPaymentRecFromDataRow(DataRow dr, DataTable dtFundDetails)
        {
            PaymentRecord pr = new PaymentRecord();
            pr.PaymentRecNumber = dr["PaymentRecNum"].ToString();
            pr.PaymentRecNumberExt = dr["PaymentRecNumExt"].ToString();
            pr.PaymentSetNumber = dr["PaymentSetNum"].ToString();
            pr.PaymentSetNumberExt = dr["PaymentSetNumExt"].ToString();

            if (dr.Table.Columns.Contains("Payment_Type"))
            {
                pr.PaymentType = dr["Payment_Type"].ToString();
                pr.PaymentDate = DateTime.Parse(dr["Payment_Date"].ToString());
                pr.Amount = decimal.Parse(dr["Amount"].ToString()).ToString("0.00");
                pr.FiscalYear = dr["Fiscal_Year"].ToString();
                pr.IndexCode = dr["Index_Code"].ToString();
                pr.ObjectDetailCode = dr["Object_Detail_Code"].ToString();
                pr.ObjectAgencyCode = dr["Object_Agency_Code"].ToString();
                pr.PCACode = dr["PCA_Code"].ToString();
                pr.ApprovedBy = dr["Approved_By"].ToString();
                pr.PayeeInfo = GetPayeeFromDataRow(dr);
                pr.GenericNameValueList = GetPaymentRecKVPList(dr);
                pr.FundingDetailList = GetFundingDetailListFromDB(dr, dtFundDetails);
            }
            
            return pr;
        }

        private static PaymentExchangeEntity GetPayeeFromDataRow(DataRow dr)
        {
            PaymentExchangeEntity pee = new PaymentExchangeEntity();
            pee.EntityID = dr["Payee_Entity_ID"].ToString();
            pee.EntityIDType = dr["Payee_Entity_ID_Type"].ToString();
            pee.Name = dr["Payee_Name"].ToString();
            pee.EntityIDSuffix = dr["Payee_Entity_ID_Suffix"].ToString();
            pee.EIN = dr["Payee_EIN"].ToString();
            pee.VendorTypeCode = dr["Vendor_Type_Code"].ToString();
            pee.AddressLine1 = dr["Payee_Address1"].ToString();
            pee.AddressLine2 = dr["Payee_Address2"].ToString();
            pee.AddressLine3 = dr["Payee_Address3"].ToString();
            pee.AddressCity = dr["Payee_City"].ToString();
            pee.AddressState = dr["Payee_State"].ToString();
            pee.AddressZip = dr["Payee_Zip"].ToString();
            return pee;
        }

        private static Dictionary<string, string> GetPaymentRecKVPList(DataRow dr)
        {
            Dictionary<string, string> kvpList = new Dictionary<string, string>();
            kvpList.Add(dr["KVP1_KEY"].ToString(), dr["KVP1_VALUE"].ToString());
            kvpList.Add(dr["KVP2_KEY"].ToString(), dr["KVP2_VALUE"].ToString());
            kvpList.Add(dr["KVP3_KEY"].ToString(), dr["KVP3_VALUE"].ToString());
            kvpList.Add(dr["KVP4_KEY"].ToString(), dr["KVP4_VALUE"].ToString());
            return kvpList;
        }

        private static List<FundingDetail> GetFundingDetailListFromDB(DataRow dr, DataTable dtFundDetails)
        {
            int prId = int.Parse(dr["Payment_Record_Id"].ToString());
            List<FundingDetail> fdList = new List<FundingDetail>();
            DataRow[] filtRows = dtFundDetails.Select("Payment_Record_Id = " + prId);
            DataTable dtFilteredFd = filtRows.CopyToDataTable();
            foreach (DataRow fdRow in dtFilteredFd.Rows)
            {
                FundingDetail fd = GetFundingDetailFromDataRow(fdRow);
                fdList.Add(fd);
            }

            return fdList;
        }

        private static FundingDetail GetFundingDetailFromDataRow(DataRow dr)
        {
            FundingDetail fd = new FundingDetail();
            fd.FundingSourceName = dr["Funding_Source_Name"].ToString();
            fd.FFPAmount = decimal.Parse(dr["FFP_Amount"].ToString()).ToString("0.00");
            fd.SGFAmount = decimal.Parse(dr["SGF_Amount"].ToString()).ToString("0.00");
            fd.FiscalYear = dr["Fiscal_Year"].ToString();
            fd.FiscalQuarter = dr["Fiscal_Quarter"].ToString();
            fd.Title = dr["Funding_Title"].ToString();
            return fd;
        }




        private static string GetQueryTextFromFile(string fileName)
        {
            if (!File.Exists(fileName))
                return string.Empty;

            string queryText = File.ReadAllText(fileName).Replace("\r\n\t", " ");

            // validate query to only allow select statment
            if (queryText.ToUpper().Contains("DELETE") ||
                queryText.ToUpper().Contains("INSERT") ||
                queryText.ToUpper().Contains("UPDATE") ||
                queryText.ToUpper().Contains("CREATE") ||
                queryText.ToUpper().Contains("ALTER") ||
                queryText.ToUpper().Contains("DROP") ||
                queryText.ToUpper().Contains("GRANT") ||
                queryText.ToUpper().Contains("REVOKE") ||
                queryText.ToUpper().Contains("COMMIT") ||
                queryText.ToUpper().Contains("ROLLBACK") ||
                queryText.ToUpper().Contains("--"))
            {
                throw new Exception("Invalid Query !");
            }

            return queryText;   
        }
              

    }
}
