using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System.Data.Common;
using System.Data;
using EAMI.CommonEntity;
using EAMI.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace EAMI.DataEngine
{
    static class DataAccessSharedFunctions
    {
        #region Public Methods

        public static ClaimSchedule GetClaimScheduleFromDataSet(DataRow claimSchRow, RefCodeTableList rctl)
        {
            ClaimSchedule cs = new ClaimSchedule();
            cs.PrimaryKeyID = int.Parse(claimSchRow["Claim_Schedule_ID"].ToString());
            cs.UniqueNumber = claimSchRow["Claim_Schedule_Number"].ToString();
            cs.PaymentType = claimSchRow["Payment_Type"].ToString();
            cs.Amount = decimal.Parse(claimSchRow["Amount"].ToString());
            cs.FiscalYear = claimSchRow["FiscalYear"].ToString();
            cs.ContractNumber = claimSchRow["ContractNumber"].ToString();
            
            int eptId = int.Parse(claimSchRow["Exclusive_Payment_Type_ID"].ToString());
            cs.ExclusivePaymentType = rctl.GetRefCodeListByTableName(enRefTables.TB_EXCLUSIVE_PAYMENT_TYPE).GetRefCodeByID(eptId);

            cs.IsLinked = bool.Parse(claimSchRow["IsLinked"].ToString());
            cs.LinkedByPGNumber = claimSchRow["LinkedByPGNumber"].ToString();
            cs.HasNegativeFundingSource = bool.Parse(claimSchRow["HasNegativeFundingSource"].ToString());
            cs.SeqNumber = string.IsNullOrWhiteSpace(claimSchRow["SeqNumber"].ToString()) ? 0 : int.Parse(claimSchRow["SeqNumber"].ToString());

            int payment_method_type_id = int.Parse(claimSchRow["Payment_Method_Type_ID"].ToString());
            cs.PaymentMethodType = rctl.GetRefCodeListByTableName(enRefTables.TB_PAYMENT_METHOD_TYPE).GetRefCodeByID(payment_method_type_id);

            // payee info/entity
            PaymentExcEntityInfo peei = new PaymentExcEntityInfo();
            peei.PEEInfo_PK_ID = int.Parse(claimSchRow["PEE_Address_ID"].ToString());
           // peei.PEE_Name = claimSchRow["Entity_Name"].ToString();
            peei.PEE_AddressLine1 = claimSchRow["Address_Line1"].ToString();
            peei.PEE_AddressLine2 = claimSchRow["Address_Line2"].ToString();
            peei.PEE_AddressLine3 = claimSchRow["Address_Line3"].ToString();
            peei.PEE_City = claimSchRow["City"].ToString();
            peei.PEE_State = claimSchRow["State"].ToString();
            peei.PEE_Zip = claimSchRow["Zip"].ToString();
            peei.PEE_ContractNumber = claimSchRow["ContractNumber"].ToString();          
            int payeeEnityId = int.Parse(claimSchRow["Payment_Exchange_Entity_ID"].ToString());
            peei.PEE = rctl.GetRefCodeListByTableName(enRefTables.TB_PAYMENT_EXCHANGE_ENTITY).GetRefCodeByID<PayeeEntity>(payeeEnityId);
            cs.PayeeInfo = peei;

            //Current Status
            cs.CurrentStatus = GetCurrentCaimSCheduleStatusFromDataSet(new EntityStatus(), claimSchRow, rctl);
            //Latest Status
            cs.LatestStatus = GetLatestCaimSCheduleStatusFromDataSet(new EntityStatus(), claimSchRow, rctl);
            // assigned user
            cs.AssignedUser = GetAssignedUserFromDataSet(null, claimSchRow, rctl);
            // assigned PayDate
            cs.PayDate = GetAssignedPayDateFromDataSet(null, claimSchRow, rctl);

            //Remittance Advice
            string remittanceAdviceNote = claimSchRow["Remittance_Advice_Note"].ToString();
            cs.HasRemittanceAdviceNote = !string.IsNullOrEmpty(remittanceAdviceNote);
            cs.RemittanceAdviceNote = cs.HasRemittanceAdviceNote ? remittanceAdviceNote : null;

            //Initialize the LIST
            cs.PaymentGroupList = new List<PaymentGroup>();

            //Linked Claim Schedule Number List
            cs.LinkedCSNumberList = new List<string>();
            string LinkedCSNumberList = claimSchRow["Linked_Claim_Schedule_Numbers"].ToString().Trim();
            if (!String.IsNullOrEmpty(LinkedCSNumberList))
            {
                cs.LinkedCSNumberList.AddRange(LinkedCSNumberList.Split(',').ToList());
            }

            // optional warrant info
            if (claimSchRow.Table.Columns.Contains("Warrant_Number") && 
                !String.IsNullOrEmpty(claimSchRow["Warrant_Number"].ToString()))
            {
                WarrantRec wr = new WarrantRec();
                wr.WARRANT_NUMBER = claimSchRow["Warrant_Number"].ToString();
                wr.WARRANT_AMOUNT = decimal.Parse(claimSchRow["Warrant_Amount"].ToString());
                wr.ISSUE_DATE = DateTime.Parse(claimSchRow["Warrant_Date"].ToString());
                wr.SEQ_NUMBER = int.Parse(claimSchRow["SeqNumber"].ToString());
                cs.Warrant = wr;
            }

            //optional EFT Info
            string prvAccountNo = claimSchRow["EFT_PrvAccountNo"].ToString();
            if (!string.IsNullOrEmpty(prvAccountNo))
            {
                EftInfo eftInfo = new EftInfo();
                eftInfo.EFTInfoID = int.Parse((claimSchRow["EFT_Info_ID"].ToString()));
                eftInfo.CreateDate = DateTime.Parse(claimSchRow["EFT_CreateDate"].ToString());
                eftInfo.DatePrenoted = DateTime.Parse(claimSchRow["EFT_DatePrenoted"].ToString());
                eftInfo.FIAccountType = claimSchRow["EFT_FIAccountType"].ToString();
                eftInfo.FIRoutingNumber = claimSchRow["EFT_FIRoutingNumber"].ToString();
                eftInfo.PrvAccountNo = claimSchRow["EFT_PrvAccountNo"].ToString();
                peei.PEE_EftInfo = eftInfo;
            }            

            return cs;
        }

        public static PaymentRec GetPaymentRecFromDataSet(DataRow dataRow, RefCodeTableList rctl)
        {
            PaymentRec pr = new PaymentRec();
            pr.PrimaryKeyID = int.Parse(dataRow["Payment_Record_ID"].ToString());
            pr.UniqueNumber = dataRow["PaymentRec_Number"].ToString();
            pr.PaymentRecNumberExt = dataRow["PaymentRec_NumberExt"].ToString();
            pr.PaymentType = dataRow["Payment_Type"].ToString();
            pr.PaymentDate = DateTime.Parse(dataRow["Payment_Date"].ToString());
            pr.Amount = decimal.Parse(dataRow["Amount"].ToString());
            pr.FiscalYear = dataRow["FiscalYear"].ToString();
            pr.IndexCode = dataRow["IndexCode"].ToString();
            pr.ObjDetailCode = dataRow["ObjectDetailCode"].ToString();
            pr.ObjAgencyCode = dataRow["ObjectAgencyCode"].ToString();
            pr.PCACode = dataRow["PCACode"].ToString();
            pr.ApprovedBy = dataRow["ApprovedBy"].ToString();
            
            pr.PaymentSetNumber = dataRow["PaymentSet_Number"].ToString();
            pr.PaymentSetNumberExt = dataRow["PaymentSet_NumberExt"].ToString();

            pr.RPICode = dataRow["RPICode"].ToString();
            pr.IsReportableRPI = bool.Parse(dataRow["IsReportableRPI"].ToString());
            pr.ContractNumber = dataRow["ContractNumber"].ToString();
            pr.ContractDateFrom = DateTime.Parse(dataRow["ContractDateFrom"].ToString());
            pr.ContractDateTo = DateTime.Parse(dataRow["ContractDateTo"].ToString());
            int eptId = int.Parse(dataRow["Exclusive_Payment_Type_ID"].ToString());
            pr.ExclusivePaymentType = rctl.GetRefCodeListByTableName(enRefTables.TB_EXCLUSIVE_PAYMENT_TYPE).GetRefCodeByID(eptId);
            int payment_method_type_id = int.Parse(dataRow["Payment_Method_Type_ID"].ToString());
            pr.PaymentMethodType = rctl.GetRefCodeListByTableName(enRefTables.TB_PAYMENT_METHOD_TYPE).GetRefCodeByID(payment_method_type_id);

            // payee info/entity
            PaymentExcEntityInfo peei = new PaymentExcEntityInfo();
            peei.PEEInfo_PK_ID = int.Parse(dataRow["PEE_Address_ID"].ToString());
           // peei.PEE_Name = dataRow["Entity_Name"].ToString();
            //peei.PEE_IdSfx = dataRow["Entity_Code_Suffix"].ToString();
            peei.PEE_AddressLine1 = dataRow["Address_Line1"].ToString();
            peei.PEE_AddressLine2 = dataRow["Address_Line2"].ToString();
            peei.PEE_AddressLine3 = dataRow["Address_Line3"].ToString();
            peei.PEE_City = dataRow["City"].ToString();
            peei.PEE_State = dataRow["State"].ToString();
            peei.PEE_Zip = dataRow["Zip"].ToString();
            peei.PEE_ContractNumber = dataRow["ContractNumber"].ToString();
            int payeeEnityId = int.Parse(dataRow["Payment_Exchange_Entity_ID"].ToString());
            peei.PEE = rctl.GetRefCodeListByTableName(enRefTables.TB_PAYMENT_EXCHANGE_ENTITY).GetRefCodeByID<PayeeEntity>(payeeEnityId);

            // add a complete PayeeInfo to payment rec
            pr.PayeeInfo = peei;

            // payment status
            pr.CurrentStatus = GetCurrentPaymentStatusFromDataSet(new EntityStatus(), dataRow, rctl);
            pr.LatestStatus = GetLatestPaymentStatusFromDataSet(new EntityStatus(), dataRow, rctl);
            pr.OnHoldFlagStatus = GetOnHoldPaymentStatusFromDataSet(null, dataRow, rctl);
            pr.ReleasedFromHoldFlagStatus = GetUnHoldPaymentStatusFromDataSet(null, dataRow, rctl);
            pr.ReleaseFromSupFlagStatus = GetReleasePaymentStatusFromDataSet(null, dataRow, rctl);

            // assigned user
            pr.AssignedUser = GetAssignedUserFromDataSet(null, dataRow, rctl);

            // assigned PayDate
            pr.PayDate = GetAssignedPayDateFromDataSet(null, dataRow, rctl);

            //optional EFT Info
            string prvAccountNo = dataRow["EFT_PrvAccountNo"].ToString();
            if (!string.IsNullOrEmpty(prvAccountNo))
            {
                EftInfo eftInfo = new EftInfo();
                eftInfo.EFTInfoID = int.Parse((dataRow["EFT_Info_ID"].ToString()));
                eftInfo.CreateDate = DateTime.Parse(dataRow["EFT_CreateDate"].ToString());
                eftInfo.DatePrenoted = DateTime.Parse(dataRow["EFT_DatePrenoted"].ToString());
                eftInfo.FIAccountType = dataRow["EFT_FIAccountType"].ToString();
                eftInfo.FIRoutingNumber = dataRow["EFT_FIRoutingNumber"].ToString();
                eftInfo.PrvAccountNo = dataRow["EFT_PrvAccountNo"].ToString();
                peei.PEE_EftInfo = eftInfo;
            }

            return pr;
        }

        public static EntityStatus GetCurrentPaymentStatusFromDataSet(EntityStatus es, DataRow dataRow, RefCodeTableList rctl)
        {
            es.StatusID = int.Parse(dataRow["Current_Payment_Status_ID"].ToString());
            es.EntityID = int.Parse(dataRow["Payment_Record_ID"].ToString());
            es.StatusType = rctl.GetRefCodeListByTableName(enRefTables.TB_PAYMENT_STATUS_TYPE).GetRefCodeByID(int.Parse(dataRow["Current_Payment_Status_Type_ID"].ToString()));
            es.StatusDate = DateTime.Parse(dataRow["Current_Status_Date"].ToString());
            es.StatusNote = dataRow["Current_Status_Note"].ToString();
            es.CreatedBy = dataRow["Current_Status_CreatedBy"].ToString();
            return es;
        }

        public static EntityStatus GetLatestPaymentStatusFromDataSet(EntityStatus es, DataRow dataRow, RefCodeTableList rctl)
        {
            es.StatusID = int.Parse(dataRow["Latest_Payment_Status_ID"].ToString());
            es.EntityID = int.Parse(dataRow["Payment_Record_ID"].ToString());
            es.StatusType = rctl.GetRefCodeListByTableName(enRefTables.TB_PAYMENT_STATUS_TYPE).GetRefCodeByID(int.Parse(dataRow["Latest_Payment_Status_Type_ID"].ToString()));
            es.StatusDate = DateTime.Parse(dataRow["Latest_Status_Date"].ToString());
            es.StatusNote = dataRow["Latest_Status_Note"].ToString();
            es.CreatedBy = dataRow["Latest_Status_CreatedBy"].ToString();
            return es;
        }

        public static EntityStatus GetOnHoldPaymentStatusFromDataSet(EntityStatus es, DataRow dataRow, RefCodeTableList rctl)
        {
            if (!string.IsNullOrEmpty(dataRow["Current_Hold_Payment_Status_ID"].ToString()))
            {
                es = new EntityStatus();
                es.StatusID = int.Parse(dataRow["Current_Hold_Payment_Status_ID"].ToString());
                es.EntityID = int.Parse(dataRow["Payment_Record_ID"].ToString());
                es.StatusType = rctl.GetRefCodeListByTableName(enRefTables.TB_PAYMENT_STATUS_TYPE).GetRefCodeByID(int.Parse(dataRow["Current_Hold_Payment_Status_Type_ID"].ToString()));
                es.StatusDate = DateTime.Parse(dataRow["Current_Hold_Status_Date"].ToString());
                es.StatusNote = dataRow["Current_Hold_Status_Note"].ToString();
                es.CreatedBy = dataRow["Current_Hold_Status_CreatedBy"].ToString();
            }

            return es;
        }

        public static EntityStatus GetUnHoldPaymentStatusFromDataSet(EntityStatus es, DataRow dataRow, RefCodeTableList rctl)
        {
            if (!string.IsNullOrEmpty(dataRow["Current_Unhold_Payment_Status_ID"].ToString()))
            {
                es = new EntityStatus();
                es.StatusID = int.Parse(dataRow["Current_Unhold_Payment_Status_ID"].ToString());
                es.EntityID = int.Parse(dataRow["Payment_Record_ID"].ToString());
                es.StatusType = rctl.GetRefCodeListByTableName(enRefTables.TB_PAYMENT_STATUS_TYPE).GetRefCodeByID(int.Parse(dataRow["Current_Unhold_Payment_Status_Type_ID"].ToString()));
                es.StatusDate = DateTime.Parse(dataRow["Current_Unhold_Status_Date"].ToString());
                es.StatusNote = dataRow["Current_Unhold_Status_Note"].ToString();
                es.CreatedBy = dataRow["Current_Unhold_Status_CreatedBy"].ToString();
            }

            return es;
        }

        public static EntityStatus GetReleasePaymentStatusFromDataSet(EntityStatus es, DataRow dataRow, RefCodeTableList rctl)
        {
            if (!string.IsNullOrEmpty(dataRow["Current_Release_Payment_Status_ID"].ToString()))
            {
                es = new EntityStatus();
                es.StatusID = int.Parse(dataRow["Current_Release_Payment_Status_ID"].ToString());
                es.EntityID = int.Parse(dataRow["Payment_Record_ID"].ToString());
                es.StatusType = rctl.GetRefCodeListByTableName(enRefTables.TB_PAYMENT_STATUS_TYPE).GetRefCodeByID(int.Parse(dataRow["Current_Release_Payment_Status_Type_ID"].ToString()));
                es.StatusDate = DateTime.Parse(dataRow["Current_Release_Status_Date"].ToString());
                es.StatusNote = dataRow["Current_Release_Status_Note"].ToString();
                es.CreatedBy = dataRow["Current_Release_Status_CreatedBy"].ToString();
            }

            return es;
        }

        public static EntityStatus GetReviewPaymentStatusFromDataSet(EntityStatus es, DataRow dataRow, RefCodeTableList rctl)
        {
            if (!string.IsNullOrEmpty(dataRow["Current_Review_Payment_Status_ID"].ToString()))
            {
                es = new EntityStatus();
                es.StatusID = int.Parse(dataRow["Current_Review_Payment_Status_ID"].ToString());
                es.EntityID = int.Parse(dataRow["Payment_Record_ID"].ToString());
                es.StatusType = rctl.GetRefCodeListByTableName(enRefTables.TB_PAYMENT_STATUS_TYPE).GetRefCodeByID(int.Parse(dataRow["Current_Review_Payment_Status_Type_ID"].ToString()));
                es.StatusDate = DateTime.Parse(dataRow["Current_Review_Status_Date"].ToString());
                es.StatusNote = dataRow["Current_Review_Status_Note"].ToString();
                es.CreatedBy = dataRow["Current_Review_Status_CreatedBy"].ToString();
            }

            return es;
        }

        public static EAMIUser GetAssignedUserFromDataSet(EAMIUser user, DataRow row, RefCodeTableList rctl)
        {
            if (!string.IsNullOrEmpty(row["User_ID"].ToString()))
            {
                UserAcc ua = rctl.GetRefCodeListByTableName(enRefTables.TB_USER).GetRefCodeByID<UserAcc>(int.Parse(row["User_ID"].ToString()));
                user = new EAMIUser();
                user.User_ID = ua.ID; 
                user.User_Name = ua.Code;
                user.Display_Name = ua.Description;
                user.User_EmailAddr = ua.User_EmailAddr;               
            }
            return user;
        }

        public static RefCode GetAssignedPayDateFromDataSet(RefCode payDate, DataRow row, RefCodeTableList rctl)
        {
            if (!string.IsNullOrEmpty(row["Paydate_Calendar_ID"].ToString()))
            {
                payDate = rctl.GetRefCodeListByTableName(enRefTables.TB_PAYDATE_CALENDAR).GetRefCodeByID(int.Parse(row["Paydate_Calendar_ID"].ToString()));
            }
            //if (payDate != null)
            //{
            //    payDate.Code = (!string.IsNullOrWhiteSpace(payDate.Code)) ? Convert.ToDateTime(payDate.Code).ToString("MM/dd/yyyy") : string.Empty;
            //}
            return payDate;
        }

        public static PaymentStatus GetExtendedCurrentPaymentStatusFromDataRow(DataRow dataRow, RefCodeTableList rctl, bool extendedStatusLoad)
        {
            PaymentStatus ps = new PaymentStatus();
            GetCurrentPaymentStatusFromDataSet(ps, dataRow, rctl);
            ps.PaymentRecNumber = dataRow["PaymentRec_Number"].ToString();
            ps.PaymentRecNumberExt = dataRow["PaymentRec_NumberExt"].ToString();
            ps.PaymentSetNumber = dataRow["PaymentSet_Number"].ToString();
            ps.PaymentSetNumberExt = dataRow["PaymentSet_NumberExt"].ToString();
            ps.ExternalStatusType = rctl.GetRefCodeListByTableName(enRefTables.TB_PAYMENT_STATUS_TYPE_EXTERNAL).GetRefCodeByID(int.Parse(dataRow["Payment_Status_Type_Ext_ID"].ToString()));
            ps.PaymentMethodType = rctl.GetRefCodeListByTableName(enRefTables.TB_PAYMENT_METHOD_TYPE).GetRefCodeByID(int.Parse(dataRow["Payment_Method_Type_ID"].ToString()));
            if (extendedStatusLoad)
            {
                ps.ClaimScheduleNumber = DBNull.Value.Equals(dataRow["Claim_Schedule_Number"]) ?
                                        string.Empty :
                                        dataRow["Claim_Schedule_Number"].ToString();
                ps.ClaimScheduleDate = DBNull.Value.Equals(dataRow["Claim_Schedule_Date"]) ?
                                        DateTime.MinValue :
                                        DateTime.Parse(dataRow["Claim_Schedule_Date"].ToString());
                ps.WarrantNumber = DBNull.Value.Equals(dataRow["Warrant_Number"]) ?
                                    string.Empty :
                                    dataRow["Warrant_Number"].ToString();                
                ps.WarrantDate = DBNull.Value.Equals(dataRow["Warrant_Date"]) ?
                                    DateTime.MinValue :
                                    DateTime.Parse(dataRow["Warrant_Date"].ToString());
                ps.WarrantAmount = DBNull.Value.Equals(dataRow["WARRANT_AMOUNT"]) ?
                                    0 :
                                    decimal.Parse(dataRow["WARRANT_AMOUNT"].ToString());
                
                // only provide Claim Schedule information when warrant number is available (it is possible to have CS but not WAR info)
                if (string.IsNullOrWhiteSpace(ps.WarrantNumber))
                {
                    ps.ClaimScheduleNumber = string.Empty;
                    ps.ClaimScheduleDate = DateTime.MinValue;
                }
            }
            return ps;
        }

        public static List<PaymentGroup> GetPaymentGroupListFromPaymentRecList(List<PaymentRec> prList)
        {
            List<PaymentGroup> pgList = new List<PaymentGroup>();
            if (prList == null || prList.Count == 0)
                return pgList;

            // build unique payment set num list
            List<string> unqPymtSetNumList = new List<string>();
            foreach (PaymentRec pr in prList)
            {
                if (!unqPymtSetNumList.Contains(pr.PaymentSetNumber))
                {
                    unqPymtSetNumList.Add(pr.PaymentSetNumber);
                }
            }

            // create payment group/set list
            foreach (string pymtSetNum in unqPymtSetNumList)
            {
                PaymentGroup pg = new PaymentGroup();
                pg.UniqueNumber = pymtSetNum;

                pg.PaymentRecordList = prList.FindAll(pr => pr.PaymentSetNumber == pymtSetNum);
                pg.PaymentSetNumberExt = pg.PaymentRecordList[0].PaymentSetNumberExt;
                pg.PayeeInfo = pg.PaymentRecordList[0].PayeeInfo;
                pg.PayDate =  pg.PaymentRecordList[0].PayDate;
                pg.Amount = pg.PaymentRecordList.Sum(pr => pr.Amount);
                pg.ApprovedBy = pg.PaymentRecordList[0].ApprovedBy;
                pg.ExclusivePaymentType = pg.PaymentRecordList[0].ExclusivePaymentType;

                pg.AssignedUser = pg.PaymentRecordList[0].AssignedUser;
                pg.ContractDateFrom = pg.PaymentRecordList[0].ContractDateFrom;
                pg.ContractDateTo = pg.PaymentRecordList[0].ContractDateTo;
                pg.ContractNumber = pg.PaymentRecordList[0].ContractNumber;
                pg.CurrentStatus = pg.PaymentRecordList[0].CurrentStatus;
                pg.FiscalYear = pg.PaymentRecordList[0].FiscalYear;
                pg.LatestStatus = pg.PaymentRecordList[0].LatestStatus;
                pg.OnHoldFlagStatus = pg.PaymentRecordList[0].OnHoldFlagStatus;
                pg.PaymentDate = pg.PaymentRecordList[0].PaymentDate;
                pg.PaymentType = pg.PaymentRecordList[0].PaymentType;
                pg.ReleasedFromHoldFlagStatus = pg.PaymentRecordList[0].ReleasedFromHoldFlagStatus;
                pg.ReleaseFromSupFlagStatus = pg.PaymentRecordList[0].ReleaseFromSupFlagStatus;
                pg.PaymentMethodType = pg.PaymentRecordList[0].PaymentMethodType;

                //create super group key
                pg.PaymentSuperGroupKey = string.Format("{0}_{1}_{2}", pg.PayeeInfo.PEE_FullCode, pg.ContractNumber, pg.FiscalYear);

                pgList.Add(pg);
            }

            return pgList;
        }

        public static EntityStatus GetCurrentCaimSCheduleStatusFromDataSet(EntityStatus es, DataRow dr, RefCodeTableList rctl)
        {
            es.StatusID = int.Parse(dr["Current_Claim_Schedule_Status_ID"].ToString());
            es.EntityID = int.Parse(dr["Claim_Schedule_ID"].ToString());
            es.StatusType = rctl.GetRefCodeListByTableName(enRefTables.TB_CLAIM_SCHEDULE_STATUS_TYPE).GetRefCodeByID(int.Parse(dr["Current_Claim_Schedule_Status_Type_ID"].ToString()));
            es.StatusDate = DateTime.Parse(dr["Current_Status_Date"].ToString());
            es.StatusNote = dr["Current_Status_Note"].ToString();
            es.CreatedBy = dr["Current_Status_CreatedBy"].ToString();
            return es;
        }

        public static EntityStatus GetLatestCaimSCheduleStatusFromDataSet(EntityStatus es, DataRow dr, RefCodeTableList rctl)
        {
            es.StatusID = int.Parse(dr["Latest_Claim_Schedule_Status_ID"].ToString());
            es.EntityID = int.Parse(dr["Claim_Schedule_ID"].ToString());
            es.StatusType = rctl.GetRefCodeListByTableName(enRefTables.TB_CLAIM_SCHEDULE_STATUS_TYPE).GetRefCodeByID(int.Parse(dr["Latest_Claim_Schedule_Status_Type_ID"].ToString()));
            es.StatusDate = DateTime.Parse(dr["Latest_Status_Date"].ToString());
            es.StatusNote = dr["Latest_Status_Note"].ToString();
            es.CreatedBy = dr["Latest_Status_CreatedBy"].ToString();
            return es;
        }

        public static ElectronicClaimSchedule GetElectronicClaimScheduleFromDataSet(DataRow dr, RefCodeTableList rctl)
        {
            ElectronicClaimSchedule ecs = new ElectronicClaimSchedule();
            ecs.EcsId = int.Parse(dr["ECS_ID"].ToString());
            ecs.EcsNumber = dr["ECS_Number"].ToString();
            ecs.EcsFileName = dr["ECS_File_Name"].ToString();
            int eptId = int.Parse(dr["Exclusive_Payment_Type_ID"].ToString());
            ecs.ExclusivePaymentType = rctl.GetRefCodeListByTableName(enRefTables.TB_EXCLUSIVE_PAYMENT_TYPE).GetRefCodeByID(eptId);

            ecs.PayDate = DateTime.Parse(dr["PayDate"].ToString());
            ecs.Amount = decimal.Parse(dr["Amount"].ToString());
            ecs.SentToScoTaskNumber = dr["SentToScoTaskNumber"].ToString();
            ecs.WarrantReceivedTaskNumber = dr["WarrantReceivedTaskNumber"].ToString();
            ecs.CreateDate = DateTime.Parse(dr["CreateDate"].ToString());
            ecs.CreatedBy = dr["CreatedBy"].ToString();
            ecs.ApproveDate = DBNull.Value.Equals(dr["ApproveDate"]) ? 
                                        DateTime.MinValue :
                                        DateTime.Parse(dr["ApproveDate"].ToString());
            ecs.ApprovedBy = dr["ApprovedBy"].ToString();
            ecs.SentToScoDate = DBNull.Value.Equals(dr["SentToScoDate"]) ?
                                        DateTime.MinValue :
                                        DateTime.Parse(dr["SentToScoDate"].ToString());
            ecs.WarrantReceivedDate = DBNull.Value.Equals(dr["WarrantReceivedDate"]) ?
                                        DateTime.MinValue :
                                        DateTime.Parse(dr["WarrantReceivedDate"].ToString());
            ecs.CurrentStatusType = rctl.GetRefCodeListByTableName(enRefTables.TB_ECS_STATUS_TYPE).GetRefCodeByID(int.Parse(dr["Current_ECS_Status_Type_ID"].ToString())); 
            ecs.CurrentStatusDate = DateTime.Parse(dr["CurrentStatusDate"].ToString());
            ecs.CurrentStatusNote = dr["CurrentStatusNote"].ToString();

            int payment_method_type_id = int.Parse(dr["Payment_Method_Type_ID"].ToString());
            ecs.PaymentMethodType = rctl.GetRefCodeListByTableName(enRefTables.TB_PAYMENT_METHOD_TYPE).GetRefCodeByID(payment_method_type_id);

            return ecs;
        }


        public static PaymentFundingDetail GetFundingDetailFromReader(IDataReader reader)
        {
            PaymentFundingDetail fd = new PaymentFundingDetail();
            fd.FundingDetailID = int.Parse(reader["Funding_Detail_ID"].ToString());
            fd.PaymentRecID = int.Parse(reader["Payment_Record_ID"].ToString());
            fd.FundingSourceName = reader["Funding_Source_Name"].ToString();
            fd.FFPAmount = decimal.Parse(reader["FFPAmount"].ToString());
            fd.SGFAmount = decimal.Parse(reader["SGFAmount"].ToString());
            fd.TotalAmount = decimal.Parse(reader["TotalAmount"].ToString());
            fd.FiscalYear = reader["FiscalYear"].ToString();
            fd.FiscalQuarter = reader["FiscalQuarter"].ToString();
            fd.Title = reader["Title"].ToString();
            return fd;
        }


        public static AggregatedFundingDetail GetAggregatedFundingDetailFromDataRow(IDataReader reader)
        {
            AggregatedFundingDetail afd = new AggregatedFundingDetail();
            afd.FundingSourceName = reader["Funding_Source_Name"].ToString();
            afd.FFPAmount = decimal.Parse(reader["FFPAmount"].ToString());
            afd.SGFAmount = decimal.Parse(reader["SGFAmount"].ToString());
            afd.TotalAmount = decimal.Parse(reader["TotalAmount"].ToString());
            return afd;
        }

        #endregion
    }
}
