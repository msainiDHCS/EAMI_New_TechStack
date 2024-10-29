using EAMI.Controllers;
using EAMI.WebApi.Models;
using OHC.EAMI.Common;
using OHC.EAMI.Common.ServiceInvoke;
using OHC.EAMI.CommonEntity;
using System.Data;

namespace EAMI.Data
{
    public class PaymentProcessingQueries
    {

        public static RemittanceCSModel GetRemittanceAdviceDataByCSID(int csID, int systemID, out string strPEE_Name)
        {
            WcfServiceInvoker _wcfService = new WcfServiceInvoker();
            CommonStatusPayload<DataSet> cspl;
            RemittanceCSModel raCSModel = new RemittanceCSModel();
            raCSModel.remittanceAdviceDetail = new RemittanceAdviceDetail();
            raCSModel.claimSchedule = new ClaimSchedule();
            raCSModel.claimSchedule.PayeeInfo = new PaymentExcEntityInfo();
            raCSModel.claimSchedule.PayeeInfo.PEE = new PayeeEntity();
            raCSModel.claimSchedule.PayeeInfo.PEE_EftInfo = new EftInfo();

            cspl = _wcfService.InvokeService<IEAMIWebUIDataService, CommonStatusPayload<DataSet>>(svc => svc.GetRemittanceAdviceDataByCSID(csID,systemID));     //get from db via service
            ErrorHandlerController.CheckFatalException(cspl.Status, cspl.IsFatal, cspl.GetCombinedMessage());

            raCSModel.remittanceAdviceDetail.DepartmentName = cspl.Payload.Tables[0].Rows[0]["Department_Name"].ToString();
            raCSModel.remittanceAdviceDetail.DepartmentAddress = cspl.Payload.Tables[0].Rows[0]["Department_Address_Line"].ToString();
            raCSModel.remittanceAdviceDetail.DepartmentAddressCSZ = cspl.Payload.Tables[0].Rows[0]["Department_CSZ"].ToString();
            raCSModel.remittanceAdviceDetail.OrgranizationCode = cspl.Payload.Tables[0].Rows[0]["Org_Code"].ToString();
            raCSModel.remittanceAdviceDetail.Agency_Inquiries_Phone_Number = cspl.Payload.Tables[0].Rows[0]["Agency_Inquiries_Phone_Number"].ToString();
            raCSModel.claimSchedule.UniqueNumber = cspl.Payload.Tables[0].Rows[0]["Claim_Schedule_Number"].ToString();
            raCSModel.claimSchedule.PrimaryKeyID = Convert.ToInt32(cspl.Payload.Tables[0].Rows[0]["Claim_Schedule_ID"]);
            //raCSModel.claimSchedule.PayeeInfo.PEE_Name = cspl.Payload.Tables[0].Rows[0]["Entity_Name"].ToString();
            strPEE_Name = cspl.Payload.Tables[0].Rows[0]["Entity_Name"].ToString();
            raCSModel.claimSchedule.PayeeInfo.PEE_AddressLine1 = cspl.Payload.Tables[0].Rows[0]["Vendor_ADDRESS_LINE1"].ToString().ToUpper();
            raCSModel.claimSchedule.PayeeInfo.PEE_AddressLine2 = cspl.Payload.Tables[0].Rows[0]["Vendor_ADDRESS_LINE2"].ToString().ToUpper();
            raCSModel.claimSchedule.PayeeInfo.PEE_AddressLine3 = cspl.Payload.Tables[0].Rows[0]["Vendor_ADDRESS_LINE3"].ToString().ToUpper();
            raCSModel.claimSchedule.PayeeInfo.PEE_City = cspl.Payload.Tables[0].Rows[0]["Vendor_City"].ToString().ToUpper();
            raCSModel.claimSchedule.PayeeInfo.PEE_State = cspl.Payload.Tables[0].Rows[0]["Vendor_State"].ToString().ToUpper();
            raCSModel.claimSchedule.PayeeInfo.PEE_Zip = cspl.Payload.Tables[0].Rows[0]["Vendor_Zip"].ToString();
            raCSModel.claimSchedule.PayeeInfo.PEE.EntityEIN = cspl.Payload.Tables[0].Rows[0]["Vendor_TAX_ID"].ToString();
            raCSModel.claimSchedule.PayeeInfo.PEE.Code = cspl.Payload.Tables[0].Rows[0]["Entity_ID"].ToString();
            raCSModel.claimSchedule.PayeeInfo.PEE_IdSfx = string.Empty;
            raCSModel.claimSchedule.PayeeInfo.PEE_EftInfo.FIRoutingNumber= cspl.Payload.Tables[0].Rows[0]["FIRoutingNumber"].ToString() ;
            raCSModel.claimSchedule.PayeeInfo.PEE_EftInfo.PrvAccountNo = cspl.Payload.Tables[0].Rows[0]["PrvAccountNo"].ToString();

            WcfServiceInvoker _wcfService_PaymentMethodType = new WcfServiceInvoker();
            CommonStatusPayload<List<RefCodeList>> cs_PaymentMethodType = _wcfService_PaymentMethodType.InvokeService<IEAMIWebUIDataService, CommonStatusPayload<List<RefCodeList>>>(
                    svc => svc.GetReferenceCodeList(enRefTables.TB_PAYMENT_METHOD_TYPE));

            ErrorHandlerController.CheckFatalException(cs_PaymentMethodType.Status, cs_PaymentMethodType.IsFatal, cs_PaymentMethodType.GetCombinedMessage());

            string payment_method_type = cspl.Payload.Tables[0].Rows[0]["PaymentMethodType"].ToString();
            raCSModel.claimSchedule.PaymentMethodType = cs_PaymentMethodType.Payload[0].GetRefCodeByCode(payment_method_type);

            raCSModel.claimSchedule.FiscalYear = cspl.Payload.Tables[0].Rows[0]["FFY"].ToString();
            //Commented out below since on PaymentProcessing tab, amount on claim schedule not set yet so will need to retrieve this amount from underlying pg's instead.
            //raCSModel.claimSchedule.AmountSetAtPgLevel = Convert.ToDecimal(cspl.Payload.Tables[0].Rows[0]["Amount"] is DBNull ? 0 : cspl.Payload.Tables[0].Rows[0]["Amount"]);
            raCSModel.claimSchedule.RemittanceAdviceNote = cspl.Payload.Tables[0].Rows[0]["Note"].ToString();
            raCSModel.claimSchedule.ContractNumber = cspl.Payload.Tables[0].Rows[0]["ContractNumber"].ToString();
            raCSModel.claimSchedule.PaymentGroupList = new List<PaymentGroup>();

            foreach (DataRow pgRow in cspl.Payload.Tables[1].Rows)
            {
                var pg = new PaymentGroup();
                pg.PaymentDate =  Convert.ToDateTime(pgRow["Payment_Date"]);
                pg.UniqueNumber = pgRow["PaymentSet_Number"].ToString();
                pg.AmountSetAtPgLevel = Convert.ToDecimal(pgRow["Amount"] is DBNull ? 0 : pgRow["Amount"]);
                pg.PaymentRecordList = new List<PaymentRec>();
                pg.PaymentRecordList.Add(new PaymentRec());
                pg.PaymentRecordList.FirstOrDefault().RPICode = pgRow["RPICode"] is DBNull ? "" : pgRow["RPICode"].ToString();
                raCSModel.claimSchedule.PaymentGroupList.Add(pg);
            }

            return raCSModel;
        }

    }   
}