USE [EAMI-MC]
GO
GO
/****** Object:  StoredProcedure [dbo].[GetDataSummaryReportData]    Script Date: 5/29/2023 10:08:03 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetDataSummaryReportData]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetDataSummaryReportData]
GO
/****** Object:  StoredProcedure [dbo].[GetDataSummaryReportData]    Script Date: 12/27/2021 11:28:39 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/******************************************************************************      
* PROCEDURE:  [dbo].[GetDataSummaryReportData]      
* PURPOSE: Gets Data Summary Report Data    
* NOTES:   Stored Procedure to fetch data for Data Sumarry for the given date range   
* CREATED: 05/19/2023  Meetu Saini.      
* MODIFIED      
* DATE          AUTHOR            DESCRIPTION       
* 8/31/2023     Genady Gidenko    Remove DATEADD +1 from @DateTo parameter. See EAMI-877 for details
*------------------------------------------------------------------------------      
-- exec GetDataSummaryReportData '03/24/2022', '05/18/2023'    
*****************************************************************************/
CREATE PROCEDURE [dbo].[GetDataSummaryReportData]      
 @DateFrom VARCHAR(100),      
 @DateTo VARCHAR(100)      
AS      
BEGIN      
 BEGIN TRY      
        
 DECLARE @DateFrom_local datetime = CONVERT(date, @DateFrom)      
 DECLARE @DateTo_local datetime = CONVERT(date, @DateTo)        
      
SELECT cs.Claim_Schedule_Number      
   , ecs.ECS_Number      
   , ecs.PayDate as CS_PayDate      
   , pee.Entity_ID       
   , peea.Entity_Name      
   , peea.Address_Line1 as VendorName1      
   , pr.PaymentSet_Number      
   , pr.Payment_Date as PaymentSet_Received_Date      
   , fd.FiscalYear as State_FiscalYear      
   , fd.FiscalQuarter as State_Service_Qtr      
   , pr.IndexCode      
   , pr.PCACode      
   , pr.ObjectDetailCode      
   , 'OMC' as ServiceCategory      
   , ept.Code as X_Type      
   , fd.Funding_Source_Name      
   , fd.TotalAmount as HCDFAmount      
   , fd.FFPAmount, fd.SGFAmount      
   
FROM TB_CLAIM_SCHEDULE_ECS CSECS    
    INNER JOIN TB_ECS ECS (nolock) on ecs.ECS_ID = csecs.ECS_ID    
    INNER JOIN TB_ECS_STATUS_TYPE ECSS (nolock) on ecss.ECS_Status_Type_ID = ecs.Current_ECS_Status_Type_ID    
    INNER JOIN TB_CLAIM_SCHEDULE cs (nolock) on cs.Claim_Schedule_ID = csecs.Claim_Schedule_ID 
    INNER JOIN TB_CLAIM_SCHEDULE_DN_STATUS csdn (nolock) on csdn.Claim_Schedule_ID = cs.Claim_Schedule_ID    
    INNER JOIN TB_CLAIM_SCHEDULE_USER_ASSIGNMENT csua (nolock) on csua.Claim_Schedule_ID = CSECS.Claim_Schedule_ID    
    INNER JOIN TB_USER usr (nolock) on usr.User_ID = csua.User_ID    
    INNER JOIN TB_CLAIM_SCHEDULE_STATUS css (nolock) on css.Claim_Schedule_Status_ID = csdn.CurrentCSStatusID    
    INNER JOIN TB_CLAIM_SCHEDULE_STATUS_TYPE csst (nolock) on csst.Claim_Schedule_Status_Type_ID = css.Claim_Schedule_Status_Type_ID  
    INNER JOIN TB_EXCLUSIVE_PAYMENT_TYPE ept (nolock) on ept.Exclusive_Payment_Type_ID = cs.Exclusive_Payment_Type_ID    
    INNER JOIN TB_PAYMENT_CLAIM_SCHEDULE pcs (nolock) on pcs.Claim_Schedule_ID = cs.Claim_Schedule_ID    
    INNER JOIN TB_FUNDING_DETAIL fd (nolock) on fd.Payment_Record_ID = pcs.Payment_Record_ID     --this makes records double 23 => 69
    INNER JOIN TB_PAYMENT_RECORD pr (nolock) on pr.Payment_Record_ID = pcs.Payment_Record_ID
    INNER JOIN TB_PEE_ADDRESS peea (nolock) on peea.PEE_Address_ID = pr.PEE_Address_ID
	LEFT  JOIN TB_PEE_SYSTEM ps (nolock) on peea.PEE_System_ID = ps.PEE_System_ID
	LEFT  JOIN TB_PAYMENT_EXCHANGE_ENTITY PEE (nolock) on ps.Payment_Exchange_Entity_ID = PEE.Payment_Exchange_Entity_ID
WHERE ecss.ECS_Status_Type_ID = '4'     
AND ecs.PayDate BETWEEN @DateFrom_local
AND  @DateTo_local --'3/24/23' and '05/18/23'
GROUP BY fd.Funding_Detail_ID      
  , cs.Claim_Schedule_Number      
  , cs.Payment_Type      
  , cs.ContractNumber      
  , cs.FiscalYear      
  , cs.Amount      
  , csst.Code      
  , USR.Display_Name      
  , ecs.ECS_Number      
  , ecss.CODE      
  , cs.Payee_Entity_Info_ID      
  , peea.Entity_Name      
  , ecs.PayDate      
  , ept.Description      
  , ecs.ApprovedBy      
  , ecs.SentToScoDate        
  , pee.Entity_ID      
  , pr.PaymentSet_Number      
  , pr.Payment_Date      
  , fd.Funding_Source_Name      
  , fd.FFPAmount      
  , fd.SGFAmount     
  , fd.FiscalQuarter      
  , pr.ObjectDetailCode      
  , pr.IndexCode      
  , pr.PCACode      
  , peea.Address_Line1      
  , fd.TotalAmount      
  , fd.FiscalYear      
  ,ept.Code 
 ORDER BY ecs.ECS_Number      
  , pr.PaymentSet_Number      
  , fd.FiscalQuarter  
          
 END TRY      
 BEGIN CATCH      
  DECLARE @ErrorMessage NVARCHAR(4000)      
  DECLARE @ErrorSeverity INT      
  DECLARE @ErrorState INT      
      
  SET @ErrorMessage = error_message()      
  SET @ErrorSeverity = error_severity()      
  SET @ErrorState = error_state()      
      
  RAISERROR (      
    @ErrorMessage      
    ,@ErrorSeverity      
    ,@ErrorState      
    )      
 END CATCH      
END
GO
