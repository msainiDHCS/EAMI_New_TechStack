USE [EAMI-PRX]
GO
/****** Object:  StoredProcedure [dbo].[GetECSReportData]    Script Date: 5/28/2019 10:08:03 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetECSReportData]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetECSReportData]
GO
/****** Object:  StoredProcedure [dbo].[GetECSReportData]    Script Date: 7/21/2020 1:55:55 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/******************************************************************************
* PROCEDURE:  [dbo].[GetECSReportData]
* PURPOSE: Gets ECS Report Data
* NOTES:
* CREATED: 12/26/2018  Genady G.
* MODIFIED
* DATE			AUTHOR      DESCRIPTION
*------------------------------------------------------------------------------
* 02/21/2019	Eugene S	Changed to read Entity Name from PEEI instead of PEE
* 7/11/2020		Genady G	Refactor to use new tables [TB_PEE_ADDRESS] and [TB_PEE_SYSTEM]
* 8/25/2023		Genady G	Fix a bug to resolve duplicate results (EAMI-880)
* 8/28/2023		Genady G	Remove Claim Schedule status, Use ECS Status instead (EAMI-880)
*****************************************************************************/
CREATE PROCEDURE [dbo].[GetECSReportData]
	@DateFrom VARCHAR(100),
	@DateTo VARCHAR(100)
AS
BEGIN
	BEGIN TRY
		
	DECLARE @DateFrom_local datetime = CONVERT(date, @DateFrom)
	DECLARE @DateTo_local datetime = DATEADD(day,1,CONVERT(date, @DateTo))
	DECLARE @CS_Status_APPROVED int = (SELECT Claim_Schedule_Status_Type_ID from [dbo].[TB_CLAIM_SCHEDULE_STATUS_TYPE] WHERE [Code] = 'APPROVED')
	DECLARE @ECS_Status_SENT_TO_SCO int = (SELECT [ECS_Status_Type_ID] from [dbo].[TB_ECS_STATUS_TYPE] WHERE [Code] = 'SENT_TO_SCO')
	DECLARE @ECS_Status_WARRANT_RECEIVED int = (SELECT [ECS_Status_Type_ID] from [dbo].[TB_ECS_STATUS_TYPE] WHERE [Code] = 'WARRANT_RECEIVED')
	DECLARE @ECS_Status_REJECTED int = (SELECT [ECS_Status_Type_ID] from [dbo].[TB_ECS_STATUS_TYPE] WHERE [Code] = 'REJECTED')

	SELECT
		ECS.ECS_Number AS [ECS_Number]
		,ECS.SentToScoDate AS [ECS_Sent_Date]
		,ECS_TP.CODE AS [ECS_Status]
		,ECS.ApprovedBy AS [ECS_Approver]
		,PEEA.[Entity_Name] AS [Entity_Name]
		,CS.Payment_Type AS [Payment_Type]
		,PEE.[Entity_ID] AS [VendorCode]
		,CS.ContractNumber AS [ContractNumber]
		,EXL_PT.[Code] AS [Business_Indicator]
		,ECS.[Amount] AS [ECS_Amount]
		,CS.Claim_Schedule_Number AS [Claim_Schedule_Number]
		,PDT.Paydate AS [CS_PayDate]
		,CS.FiscalYear AS [FiscalYear]
		,CS.Amount AS [CS_Amount]
		,CSS_APPR.CreatedBy AS [CS_Approver]
		,WR.Warrant_Date AS [WarrantDate]
		,WR.Warrant_Number AS [WarrantNumber]
		,WR.Amount AS [WarrantAmount]
	FROM TB_ECS ECS
		INNER JOIN TB_ECS_STATUS_TYPE ECS_TP ON ECS.Current_ECS_Status_Type_ID = ECS_TP.ECS_Status_Type_ID
		INNER JOIN TB_CLAIM_SCHEDULE_ECS CS_ECS ON ECS.ECS_ID = CS_ECS.ECS_ID
		INNER JOIN TB_CLAIM_SCHEDULE CS ON CS_ECS.Claim_Schedule_ID = CS.Claim_Schedule_ID
		
		INNER JOIN TB_PEE_ADDRESS(NOLOCK) PEEA ON PEEA.PEE_Address_ID = CS.PEE_Address_ID			
		INNER JOIN TB_PEE_SYSTEM(NOLOCK) PEES ON PEES.PEE_System_ID = PEEA.PEE_System_ID		
		INNER JOIN TB_PAYMENT_EXCHANGE_ENTITY(NOLOCK) PEE ON PEES.Payment_Exchange_Entity_ID = PEE.Payment_Exchange_Entity_ID
		
		INNER JOIN TB_EXCLUSIVE_PAYMENT_TYPE(NOLOCK) EXL_PT ON CS.Exclusive_Payment_Type_ID = EXL_PT.Exclusive_Payment_Type_ID
		INNER JOIN TB_PAYDATE_CALENDAR(NOLOCK) PDT ON CS.Paydate_Calendar_ID = PDT.Paydate_Calendar_ID
		INNER JOIN (SELECT DISTINCT LIST.Claim_Schedule_ID
				,MAX(LIST.Claim_Schedule_Status_ID) as Claim_Schedule_Status_ID
				,MAX(LIST.Claim_Schedule_Status_Type_ID) as Claim_Schedule_Status_Type_ID
				,MAX(LIST.CreatedBy) as CreatedBy
			FROM TB_CLAIM_SCHEDULE_STATUS LIST
			INNER JOIN TB_CLAIM_SCHEDULE_STATUS_TYPE CSST2 ON CSST2.Claim_Schedule_Status_Type_ID = LIST.Claim_Schedule_Status_Type_ID AND CSST2.Code = 'APPROVED'
			GROUP BY LIST.Claim_Schedule_ID
			) CSS_APPR ON CSS_APPR.Claim_Schedule_ID = CS.Claim_Schedule_ID
		LEFT JOIN TB_CLAIM_SCHEDULE_WARRANT(NOLOCK) CS_WR ON CS.Claim_Schedule_ID = CS_WR.Claim_Schedule_ID 
		LEFT JOIN TB_WARRANT (NOLOCK) WR ON CS_WR.Warrant_ID = WR.Warrant_ID 
	WHERE
		ECS.SentToScoDate between @DateFrom_local AND @DateTo_local
		AND (ECS.Current_ECS_Status_Type_ID = @ECS_Status_SENT_TO_SCO 
				OR ECS.Current_ECS_Status_Type_ID = @ECS_Status_WARRANT_RECEIVED 
				OR ECS.Current_ECS_Status_Type_ID = @ECS_Status_REJECTED) 
				
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
