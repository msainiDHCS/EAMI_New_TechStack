USE [EAMI-MC]
GO

/****** Object:  StoredProcedure [dbo].[GetECSAndCSByECSID]    Script Date: 9/7/2023 10:21:31 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


/******************************************************************************
* PROCEDURE:  [dbo].[GetECSByECSID]
* PURPOSE: Get Electronic Claim Schedule and Claim Schedule records by ECSID
* NOTES:
* CREATED: 9/6/2023  Gopal P.
* MODIFIED
* DATE			AUTHOR      DESCRIPTION
*------------------------------------------------------------------------------
* 9/6/2023		Gopal P	Initial Creation to get the ECS and Claim Schedule
*****************************************************************************/

CREATE PROCEDURE [dbo].[GetECSAndCSByECSID]
	@DateFrom VARCHAR (max) = null,
	@DateTo VARCHAR (max)  = null,
	@ID INT
AS
BEGIN
	BEGIN TRY
		
		DECLARE @DateFrom_local datetime = null 
		DECLARE @DateTo_local datetime = null 

		If(@DateFrom IS NOT NULL AND @DateTo IS NOT NULL)
		BEGIN
			SET @DateFrom_local = CONVERT(date, @DateFrom)
			SET @DateTo_local = DATEADD(day,1,CONVERT(date, @DateTo))
		END

		CREATE TABLE #ecs (
			[ECS_ID] int,
			Current_ECS_Status_Type_ID int
		);

		--PERFORM SEARCH
		INSERT INTO #ecs
		SELECT 
			ECS.[ECS_ID],
			ECS.Current_ECS_Status_Type_ID
		FROM TB_ECS(NOLOCK) ECS
		WHERE 
			ECS.ECS_ID = @ID
			AND ECS.CurrentStatusDate BETWEEN ISNULL(@DateFrom_local,ECS.CurrentStatusDate) AND ISNULL(@DateTo_local,ECS.CurrentStatusDate)

		--RETURN SET 1
		SELECT 
			ECS.ECS_ID
			,ECS.ECS_Number
			,ECS.ECS_File_Name
			,ECS.Exclusive_Payment_Type_ID
			,ECS.PayDate
			,ECS.Amount
			,ECS.SentToScoTaskNumber
			,ECS.WarrantReceivedTaskNumber
			,ECS.CreateDate
			,ECS.CreatedBy
			,ECS.ApproveDate
			,ECS.ApprovedBy
			,ECS.SentToScoDate
			,ECS.WarrantReceivedDate
			,ECS.Current_ECS_Status_Type_ID
			,ECS.CurrentStatusDate
			,ECS.CurrentStatusNote		
			,ECS.[Payment_Method_Type_ID]	
		FROM TB_ECS (NOLOCK) ECS
		WHERE ECS.ECS_ID in (SELECT ECS_ID FROM #ecs)
		

		--RETURN SET 2		
		SELECT 
			CS.[Claim_Schedule_ID]
			,CSECS.[ECS_ID]
			,CS.[Claim_Schedule_Number]
			,CS.[Payment_Type]
			,CS.[Claim_Schedule_Date]
			,CS.[Amount]
			,CS.[FiscalYear]
			,CS.ContractNumber			
			,CS.Exclusive_Payment_Type_ID
			,CS.[Paydate_Calendar_ID]
			,CS.[IsLinked]
			,CS.LinkedByPGNumber
			,CS.[SeqNumber]
			,CS.[Payment_Method_Type_ID]
			,[dbo].[fn_GetCSNegativeFundingIndicator](CS.Claim_Schedule_ID) as HasNegativeFundingSource
			,PEES.[Payment_Exchange_Entity_ID]
			,PEEA.[PEE_Address_ID]
			,PEEA.[Entity_Name]
			,PEEA.[Address_Line1]
			,PEEA.[Address_Line2]
			,PEEA.[Address_Line3]
			,PEEA.[City]
			,PEEA.[State]
			,PEEA.[Zip]
			,PEEA.[ContractNumber]
			,USR.[User_ID]
			--CURRENT
			,CSS_CURRENT.Claim_Schedule_Status_ID as [Current_Claim_Schedule_Status_ID]
			,CSS_CURRENT.Claim_Schedule_Status_Type_ID as [Current_Claim_Schedule_Status_Type_ID]
			,CSS_CURRENT.Status_Date as [Current_Status_Date]
			,CSS_CURRENT.Status_Note as [Current_Status_Note]
			,CSS_CURRENT.CreatedBy as [Current_Status_CreatedBy]
			--LATEST
			,CSS_LATEST.Claim_Schedule_Status_ID as [Latest_Claim_Schedule_Status_ID]
			,CSS_LATEST.Claim_Schedule_Status_Type_ID as [Latest_Claim_Schedule_Status_Type_ID]
			,CSS_LATEST.Status_Date as [Latest_Status_Date]
			,CSS_LATEST.Status_Note as [Latest_Status_Note]
			,CSS_LATEST.CreatedBy as [Latest_Status_CreatedBy]
			,RA.Note as [Remittance_Advice_Note]
			,CASE WHEN CS.[IsLinked]= 0 THEN ''
				ELSE (SELECT  STUFF((SELECT ',' + [Claim_Schedule_Number] FROM TB_CLAIM_SCHEDULE csl where csl.LinkedByPGNumber = CS.[LinkedByPGNumber] FOR XML PATH('')), 1, 1, ''))
			END AS [Linked_Claim_Schedule_Numbers]
			,W.Warrant_Number
			,W.Amount as Warrant_Amount
			,W.Warrant_Date 
			--EFT
			,EFT_INFO.[PEE_EFT_Info_ID] AS [EFT_Info_ID]
			,EFT_INFO.[CreateDate] AS [EFT_CreateDate]
			,EFT_INFO.[DatePrenoted] AS [EFT_DatePrenoted]
			,EFT_INFO.[FIRoutingNumber] AS [EFT_FIRoutingNumber]
			,EFT_INFO.[FIAccountType] AS [EFT_FIAccountType]
			,EFT_INFO.[PrvAccountNo] AS [EFT_PrvAccountNo]
		FROM TB_CLAIM_SCHEDULE (NOLOCK) CS
			--ECS 
			INNER JOIN TB_CLAIM_SCHEDULE_ECS(NOLOCK) CSECS ON CS.Claim_Schedule_ID = CSECS.Claim_Schedule_ID
			--CLAIM SCHEDULE DN STATUS
			INNER JOIN TB_CLAIM_SCHEDULE_DN_STATUS(NOLOCK) CSDN ON CS.Claim_Schedule_ID = CSDN.Claim_Schedule_ID
			--CURRENT STATUS
			INNER JOIN TB_CLAIM_SCHEDULE_STATUS(NOLOCK) CSS_CURRENT ON CSS_CURRENT.Claim_Schedule_Status_ID = CSDN.[CurrentCSStatusID]
			--LATEST STATUS
			INNER JOIN TB_CLAIM_SCHEDULE_STATUS(NOLOCK) CSS_LATEST ON CSS_LATEST.Claim_Schedule_Status_ID = CSDN.[LatestCSStatusID]
			--USER ASSIGNMENT
			INNER JOIN TB_CLAIM_SCHEDULE_USER_ASSIGNMENT(NOLOCK) CSUA ON CSDN.CurrentUserAssignmentID = CSUA.Claim_Schedule_User_Assignment_ID
			INNER JOIN TB_USER(NOLOCK) USR ON USR.[User_ID] = CSUA.[User_ID]
			--VENDOR		
			INNER JOIN TB_PEE_ADDRESS(NOLOCK) PEEA ON PEEA.PEE_Address_ID = CS.PEE_Address_ID			
			INNER JOIN TB_PEE_SYSTEM(NOLOCK) PEES ON PEES.PEE_System_ID = PEEA.PEE_System_ID
			--REMITTANCE ADVICE NOTE
			LEFT JOIN TB_CLAIM_SCHEDULE_REMITTANCE_ADVICE_NOTE RA ON CS.Claim_Schedule_ID = RA.Claim_Schedule_ID
			LEFT JOIN TB_CLAIM_SCHEDULE_WARRANT (NOLOCK) CSW ON CSW.Claim_Schedule_ID = CS.Claim_Schedule_ID
			LEFT JOIN TB_WARRANT (NOLOCK) W ON W.Warrant_ID = CSW.Warrant_ID
			--EFT
			LEFT JOIN TB_PEE_EFT_INFO EFT_INFO ON CS.PEE_EFT_Info_ID = EFT_INFO.PEE_EFT_Info_ID	
		WHERE 
			CSECS.ECS_ID in (SELECT ECS_ID FROM #ecs)
		
		--DROP TEMP TABLE
		drop table #ecs

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


