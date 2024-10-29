using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OHC.EAMI.CommonEntity;
using System.Collections.Generic;
using System.Data;
using OHC.EAMI.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System.Data.Common;
using System.IO;
using System.Runtime.Serialization;
using System.Text;


namespace OHC.EAMI.DataAccess.Test
{   

    [TestClass]
    public class IncomingDataDBMgrTest
    {
        [TestMethod]
        public void InsertRequestTransactionTest()
        {

            RequestTransaction rt = GetMockTrnsactionEntity();

            CommonStatus status = PaymentDataSubmissionDBMgr.InsertPaymentSubmissionDataToDB(rt);
            Assert.IsTrue(status.Status);

            //status = PaymentDataSubmissionDBMgr.InsertPaymentSubmissionDataToDB(rt);

            //string connString = ConfigurationManager.ConnectionStrings["EugeneData"].ConnectionString;
            //Database db = new SqlDatabase(connString);

            //try
            //{
            //    // dbConn instance scope
            //    using (DbConnection dbConn = db.CreateConnection())
            //    {
            //        dbConn.Open();
            //        RequestTransaction rt = GetMockTrnsactionEntity();

            //        // dbTran instance scope
            //        using (DbTransaction dbTran = dbConn.BeginTransaction())
            //        {
            //            // persist request                
            //            using (var reader = db.ExecuteReader(dbTran, DatabaseStoredProcs.spInsertRequest, DatabaseStoredProcs.GetSPInsertRequestParams(rt)))
            //            {
            //                if (reader.Read())
            //                {
            //                    rt.RequestTransactionID = int.Parse(reader[0].ToString());
            //                }
            //            }

            //            // persist response                    
            //            db.ExecuteNonQuery(dbTran, DatabaseStoredProcs.spInsertResponse, DatabaseStoredProcs.GetSPInsertResponseParams(rt));

            //            // persist trace transaction                    
            //            db.ExecuteNonQuery(dbTran, DatabaseStoredProcs.spInsertTraceTrasaction, DatabaseStoredProcs.GetSPInsertTraceTransaction(rt));

            //            // persist trace invoice list
            //            foreach (InvoicePaymentTr ip in rt.InvoiceList)
            //            {
            //                ip.TrTransactionID = rt.RequestTransactionID;
            //                db.ExecuteNonQuery(dbTran, DatabaseStoredProcs.spInsertTraceInvoice, DatabaseStoredProcs.GetSPInsertTraceInvoice(ip));
            //            }

            //            dbTran.Commit();
            //            // note: DbTransaction will automatically rollback when disposed (assuming it hasn't been committed).
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
        }




        private RequestTransaction GetMockTrnsactionEntity()
        {
            RequestTransaction rt = new RequestTransaction();
            //rt.SenderID = "CAPMAN";
            //rt.SOR_ID = 1; 
            //rt.ReqMsgTransactionID = "ec2a4d36-5dea-4105-9fd2-4a66be5d9761";
            //rt.ReqMsgTransactionType = "PaymentSubmissionRequest";
            //rt.TransactionTypeId = 1;
            //rt.MsgTransactionVersion = "1.1";
            //rt.RequestSentTimeStamp = DateTime.Now.AddSeconds(-5);
            //rt.RequestReceivedTimeStamp = DateTime.Now;

            //rt.RejectedInvoiceDateFrom = DateTime.Now.AddMonths(-6);
            //rt.RejectedInvoiceDateTo = DateTime.Now.AddMonths(-3);
                      
            ////rt.PayerEntity = new PaymentExcEntity();
            ////rt.PayerEntity.PEE_ID = "1G55UUP44";
            ////rt.PayerEntity.PEE_IDType = "TAX-ID";
            ////rt.PayerEntity.PEE_Name = "State of California";
            ////rt.PayerEntity.PEE_NameSfx = "CA";
            ////rt.PayerEntity.PEE_AddressLine = "1000 State st.";
            ////rt.PayerEntity.PEE_AddressCSZ = "SACRAMENTO CA, 99999";
            
            //rt.InvoiceList = new List<InvoicePayment>() 
            //{ 
            //    GetMockInvoiceTrPaymentEntity() 
            //};

            //rt.RespMsgTransactionID = "xx2a4d36-5dea-4105-9fd2-4a66be5d9799";
            //rt.RespMsgTransactionType = "PaymentSubmissionResponse";
            //rt.ResponseTimeStamp = DateTime.Now;
            //rt.RespStatusTypeID = 1;
            //rt.ResponseMessage = "This request is rejected";

            return rt;
        }

        private PaymentRecTr GetMockPaymentRecTrEntity()
        {
            // invoice data
            PaymentRecTr prt = new PaymentRecTr();            
            //ip.InvoiceNumber = "MC506-00136379";
            //ip.PaymentRecordID = "NR-44665-7769-4456";
            //ip.InvoiceType = "IHSS";
            //ip.InvoiceDate = DateTime.Parse("7/30/2017 12:00:00 AM");
            //ip.Amount = decimal.Parse("1000000.50");
            //ip.FiscalYear = "2017";
            //ip.IndexCode = "9912";
            //ip.ObjDetailCode = "702";
            //ip.ObjAgencyCode = "MT";
            //ip.PCACode = "95915";
            //ip.ApprovedBy = "MCOD Mgr Name";

            //// payee entity
            //ip.Payee = new PaymentExcEntity();
            //ip.Payee.PEE_ID = "COHS130069-00";
            //ip.Payee.PEE_IDType = "VENDOR-ID";
            //ip.Payee.PEE_Name = "Orange County Organized Health System";
            //ip.Payee.PEE_NameSfx = "HCP 506";
            //ip.Payee.PEE_AddressLine = "444 Nice rd. #555";
            //ip.Payee.PEE_AddressCSZ = "Nicetown NT 77777";

            //// kvp data
            //ip.InvoiceKvpList = new Dictionary<string,string>()
            //{
            //    {"CONTRACT_NUMBER", "08-85214"}, 
            //    {"CONTRACT_DATE_FROM","2013-08-01T00:00:00"}, 
            //    {"CONTRACT_DATE_TO","2017-07-30T00:00:00"}, 
            //    {"MODEL_TYPE","COHS"}
            //};
            //ip.TrInvoiceKvpListXML = Serialize(ip.InvoiceKvpList, ip.InvoiceKvpList.GetType());
            //ip.TrInvoiceKvpListXML = CleanUpSerializedXml(ip.TrInvoiceKvpListXML);

            //// funding details
            //ip.FundingDetailList = new List<InvoiceFundingDetail>()
            //{
            //    GetMockFundingDetailEntity()
            //};
            //ip.TrFundingDetialListXML = Serialize(ip.FundingDetailList, ip.FundingDetailList.GetType());
            //ip.TrFundingDetialListXML = CleanUpSerializedXml(ip.TrFundingDetialListXML);

            //// invoice status
            //ip.InvoiceStatus = new InvoiceStatus();
            //ip.InvoiceStatus.InvoiceStatusTypeID = 7;
            //ip.InvoiceStatus.InvoiceStatusDate = DateTime.Now;
            //ip.InvoiceStatus.InvoiceStatusMsg = "This invoice is rejected";

            return prt;
        }

        private PaymentFundingDetail GetMockFundingDetailEntity()
        {
            PaymentFundingDetail pfd = new PaymentFundingDetail();            
            //ifd.FundingSourceName = "Regular FMAP";
            //ifd.Amount = decimal.Parse("500000.25");
            //ifd.FiscalQuarter = "FQ3";
            //ifd.FiscalMonth = "FQ8";
            //ifd.WaiverName = "1115";
            return pfd;
        }


        private static string Serialize(object obj, Type type)
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
    }
}
