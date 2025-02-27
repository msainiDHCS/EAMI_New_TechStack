USE [EAMI-PRX]
GO
/****** Object:  StoredProcedure [dbo].[GetSTOReportData]    Script Date: 5/28/2019 10:08:03 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetSTOReport]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetSTOReport]
GO
/****** Object:  StoredProcedure [dbo].[GetESTOReport]    Script Date: 7/21/2020 2:01:06 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/******************************************************************************
* PROCEDURE:  [dbo].[GetESTOReport]
* PURPOSE: Gets EAMI STO Report
* NOTES:
* CREATED: 5/27/2020  Joe Stark
* MODIFIED
* DATE		AUTHOR      DESCRIPTION
* 7/11/2020 Genady G	Refactor to use new tables [TB_PEE_ADDRESS] and [TB_PEE_SYSTEM]
* 8/24/2023 Genady G	Added TB_CLAIM_SCHEDULE_DN_STATUS and WARRANT_RECEIVED
* 8/28/2023 Genady G	Remove Claim Schedule status, Use ECS Status instead
*****************************************************************************/
CREATE PROCEDURE [dbo].[GetSTOReport]
	@PayDate VARCHAR(100)
AS
BEGIN
	BEGIN TRY
	SELECT

    PEEA.[Entity_Name] AS [Entity_Name]
    ,CS.Payment_Type AS [Payment_Type]
    ,CS.Claim_Schedule_Number AS [Claim_Schedule_Number]
    ,PDT.Paydate AS [CS_PayDate]
    ,CS.FiscalYear AS [FiscalYear]
    ,CS.Amount AS [CS_Amount]
    ,SUM(FD.FFPAmount) as [CS_FFPAmount]
    ,SUM(FD.SGFAmount) as [CS_SGFAmount]

FROM TB_ECS ECS
INNER JOIN TB_ECS_STATUS_TYPE ECS_TP ON ECS.Current_ECS_Status_Type_ID = ECS_TP.ECS_Status_Type_ID AND ECS_TP.Code IN ('SENT_TO_SCO', 'WARRANT_RECEIVED')
INNER JOIN TB_CLAIM_SCHEDULE_ECS CS_ECS ON ECS.ECS_ID = CS_ECS.ECS_ID
INNER JOIN TB_CLAIM_SCHEDULE CS ON CS_ECS.Claim_Schedule_ID = CS.Claim_Schedule_ID
INNER JOIN TB_PAYMENT_CLAIM_SCHEDULE PCS ON PCS.Claim_Schedule_ID = CS.Claim_Schedule_ID
INNER JOIN TB_PAYMENT_RECORD PR ON PR.Payment_Record_ID = PCS.Payment_Record_ID
INNER JOIN TB_FUNDING_DETAIL FD ON FD.Payment_Record_ID = PR.Payment_Record_ID
INNER JOIN TB_PEE_ADDRESS(NOLOCK) PEEA ON PEEA.PEE_Address_ID = CS.PEE_Address_ID            
INNER JOIN TB_EXCLUSIVE_PAYMENT_TYPE(NOLOCK) EXL_PT ON CS.Exclusive_Payment_Type_ID = EXL_PT.Exclusive_Payment_Type_ID
INNER JOIN TB_PAYDATE_CALENDAR(NOLOCK) PDT ON CS.Paydate_Calendar_ID = PDT.Paydate_Calendar_ID
LEFT JOIN TB_CLAIM_SCHEDULE_WARRANT(NOLOCK) CS_WR ON CS.Claim_Schedule_ID = CS_WR.Claim_Schedule_ID 
LEFT JOIN TB_WARRANT (NOLOCK) WR ON CS_WR.Warrant_ID = WR.Warrant_ID 
WHERE 1 = 1 and PDT.Paydate = @PayDate
GROUP BY PEEA.[Entity_Name]
    ,CS.Payment_Type
    ,CS.Claim_Schedule_Number
    ,PDT.Paydate
    ,CS.FiscalYear
    ,CS.Amount

ORDER BY PEEA.[Entity_Name], CS.Claim_Schedule_Number

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
