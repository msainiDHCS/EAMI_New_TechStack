USE [EAMI-PRX]
GO
/****** Object:  StoredProcedure [dbo].[GetClaimSchedulesByStatusType]    Script Date: 5/28/2019 10:08:03 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetClaimSchedulesByStatusType]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetClaimSchedulesByStatusType]
GO
/****** Object:  StoredProcedure [dbo].[GetClaimSchedulesByStatusType]    Script Date: 7/16/2020 2:30:28 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/******************************************************************************
* PROCEDURE:  [dbo].[GetClaimSchedulesByStatusType]
* PURPOSE: Gets claim schedule records by Status Type ID List
* NOTES:
* CREATED: 1/25/2018  Genady G.
* MODIFIED
* DATE     AUTHOR      DESCRIPTION
*------------------------------------------------------------------------------
* 2/21/2019 Eugene S	updated entity info address details, changed to only retrun
						entity_id and user_id (the rest will be loaded from cached ref list)
* 7/11/2020 Genady G	Refactor to use new tables [TB_PEE_ADDRESS] and [TB_PEE_SYSTEM]
*****************************************************************************/
CREATE PROCEDURE [dbo].[GetClaimSchedulesByStatusType]
	@StatusTypeIDList VARCHAR(max)
AS
BEGIN
	BEGIN TRY
		
		-- convert list to xml
		DECLARE @xml XML
		SET @xml = N'<root><r>' + replace(@StatusTypeIDList, ',', '</r><r>') + '</r></root>'
		
		CREATE TABLE #cs (
			[Claim_Schedule_ID] int
			,[Current_Claim_Schedule_Status_ID] int
			,[Latest_Claim_Schedule_Status_ID] int
			,[User_ID] int
		);

		INSERT INTO #cs
		SELECT 
			CSDS.[Claim_Schedule_ID]
			,CSS_CURRENT.Claim_Schedule_Status_ID as [Current_Claim_Schedule_Status_ID]
			,CSS_LATEST.Claim_Schedule_Status_ID as [Latest_Claim_Schedule_Status_ID]
			,CSUA.[User_ID]
		FROM TB_CLAIM_SCHEDULE_DN_STATUS(NOLOCK) CSDS
			--CURRENT STATUS
			INNER JOIN TB_CLAIM_SCHEDULE_STATUS(NOLOCK) CSS_CURRENT ON CSS_CURRENT.Claim_Schedule_Status_ID = CSDS.CurrentCSStatusID		
			INNER JOIN TB_CLAIM_SCHEDULE_STATUS_TYPE(NOLOCK) CSST_CURRENT ON CSST_CURRENT.Claim_Schedule_Status_Type_ID = CSS_CURRENT.Claim_Schedule_Status_Type_ID 	
			--LATEST STATUS
			INNER JOIN TB_CLAIM_SCHEDULE_STATUS(NOLOCK) CSS_LATEST ON CSS_LATEST.Claim_Schedule_Status_ID = CSDS.LatestCSStatusID
			--USER ASSIGNMENT
			INNER JOIN TB_CLAIM_SCHEDULE_USER_ASSIGNMENT(NOLOCK) CSUA ON CSUA.Claim_Schedule_User_Assignment_ID = CSDS.CurrentUserAssignmentID
		WHERE CSST_CURRENT.[Claim_Schedule_Status_Type_ID] in ( 
				SELECT t.value('.', 'int') 
				FROM @xml.nodes('//root/r') AS a(t))

		--RETURN SET 1
		SELECT CS.[Claim_Schedule_ID]
			,CS.[Claim_Schedule_Number]
			,CS.[Payment_Type]
			,CS.[Claim_Schedule_Date]
			,CS.[Amount]
			,CS.[FiscalYear]
			,CS.ContractNumber
			,CS.Exclusive_Payment_Type_ID
			,CS.[Paydate_Calendar_ID]
			,CS.[IsLinked]
			,CS.[LinkedByPGNumber]
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
			--EFT
			,EFT_INFO.[PEE_EFT_Info_ID] AS [EFT_Info_ID]
			,EFT_INFO.[CreateDate] AS [EFT_CreateDate]
			,EFT_INFO.[DatePrenoted] AS [EFT_DatePrenoted]
			,EFT_INFO.[FIRoutingNumber] AS [EFT_FIRoutingNumber]
			,EFT_INFO.[FIAccountType] AS [EFT_FIAccountType]
			,EFT_INFO.[PrvAccountNo] AS [EFT_PrvAccountNo]
		FROM #CS (NOLOCK) CSTemp
			--CLAIM SCHEDULE
			INNER JOIN TB_CLAIM_SCHEDULE(NOLOCK) CS ON CS.Claim_Schedule_ID = CSTemp.Claim_Schedule_ID
			--CURRENT STATUS
			INNER JOIN TB_CLAIM_SCHEDULE_STATUS(NOLOCK) CSS_CURRENT ON CSS_CURRENT.Claim_Schedule_Status_ID = CSTemp.[Current_Claim_Schedule_Status_ID]
			--LATEST STATUS
			INNER JOIN TB_CLAIM_SCHEDULE_STATUS(NOLOCK) CSS_LATEST ON CSS_LATEST.Claim_Schedule_Status_ID = CSTemp.[Latest_Claim_Schedule_Status_ID]
			--USER ASSIGNMENT
			INNER JOIN TB_USER(NOLOCK) USR ON USR.[User_ID] = CSTemp.[User_ID]
			--VENDOR
			INNER JOIN TB_PEE_ADDRESS(NOLOCK) PEEA ON PEEA.PEE_Address_ID = CS.PEE_Address_ID			
			INNER JOIN TB_PEE_SYSTEM(NOLOCK) PEES ON PEES.PEE_System_ID = PEEA.PEE_System_ID			
			--REMITTANCE ADVICE NOTE
			LEFT JOIN TB_CLAIM_SCHEDULE_REMITTANCE_ADVICE_NOTE RA ON CSTemp.Claim_Schedule_ID = RA.Claim_Schedule_ID
			--EFT
			LEFT JOIN TB_PEE_EFT_INFO EFT_INFO ON CS.PEE_EFT_Info_ID = EFT_INFO.PEE_EFT_Info_ID

		--RETURN SET 2		
		SELECT PR.[Payment_Record_ID]
			,PRCS.[Claim_Schedule_ID]
			,PR.[PaymentRec_Number]
			,PR.[PaymentRec_NumberExt]
			,PR.[Payment_Type]
			,PR.[Payment_Date]
			,PR.[Amount]
			,PR.[FiscalYear]
			,PR.[IndexCode]
			,PR.[ObjectDetailCode]
			,PR.[ObjectAgencyCode]
			,PR.[PCACode]
			,PR.[ApprovedBy]
			,PR.[PaymentSet_Number]
			,PR.[PaymentSet_NumberExt]
			,PR.[RPICode]
			,PR.[IsReportableRPI]
			,PR.[Payment_Method_Type_ID]
			,PDT.[Paydate_Calendar_ID]
			,PRExt.[ContractNumber]
			,PRExt.[ContractDateFrom]
			,PRExt.[ContractDateTo]			
			,PRExt.[Exclusive_Payment_Type_ID]
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
			,PS_CURRENT.Payment_Status_ID as [Current_Payment_Status_ID]
			,PS_CURRENT.Payment_Status_Type_ID as [Current_Payment_Status_Type_ID]
			,PS_CURRENT.Status_Date as [Current_Status_Date]
			,PS_CURRENT.Status_Note as [Current_Status_Note]
			,PS_CURRENT.CreatedBy as [Current_Status_CreatedBy]
			--LATEST
			,PS_LATEST.Payment_Status_ID as [Latest_Payment_Status_ID]
			,PS_LATEST.Payment_Status_Type_ID as [Latest_Payment_Status_Type_ID]
			,PS_LATEST.Status_Date as [Latest_Status_Date]
			,PS_LATEST.Status_Note as [Latest_Status_Note]
			,PS_LATEST.CreatedBy as [Latest_Status_CreatedBy]
			--CURRENT_HOLD
			,PS_CURRENT_HOLD.Payment_Status_ID as [Current_Hold_Payment_Status_ID]
			,PS_CURRENT_HOLD.Payment_Status_Type_ID as [Current_Hold_Payment_Status_Type_ID]
			,PS_CURRENT_HOLD.Status_Date as [Current_Hold_Status_Date]
			,PS_CURRENT_HOLD.Status_Note as [Current_Hold_Status_Note]
			,PS_CURRENT_HOLD.CreatedBy as [Current_Hold_Status_CreatedBy]
			--CURRENT_UNHOLD
			,PS_CURRENT_UNHOLD.Payment_Status_ID as [Current_Unhold_Payment_Status_ID]
			,PS_CURRENT_UNHOLD.Payment_Status_Type_ID as [Current_Unhold_Payment_Status_Type_ID]
			,PS_CURRENT_UNHOLD.Status_Date as [Current_Unhold_Status_Date]
			,PS_CURRENT_UNHOLD.Status_Note as [Current_Unhold_Status_Note]
			,PS_CURRENT_UNHOLD.CreatedBy  as [Current_Unhold_Status_CreatedBy]
			--CURRENT_RELEASE
			,PS_CURRENT_RELEASE.Payment_Status_ID as [Current_Release_Payment_Status_ID]
			,PS_CURRENT_RELEASE.Payment_Status_Type_ID as [Current_Release_Payment_Status_Type_ID]
			,PS_CURRENT_RELEASE.Status_Date as [Current_Release_Status_Date]
			,PS_CURRENT_RELEASE.Status_Note as [Current_Release_Status_Note]
			,PS_CURRENT_RELEASE.CreatedBy as [Current_Release_Status_CreatedBy]
			--EFT
			,EFT_INFO.[PEE_EFT_Info_ID] AS [EFT_Info_ID]
			,EFT_INFO.[CreateDate] AS [EFT_CreateDate]
			,EFT_INFO.[DatePrenoted] AS [EFT_DatePrenoted]
			,EFT_INFO.[FIRoutingNumber] AS [EFT_FIRoutingNumber]
			,EFT_INFO.[FIAccountType] AS [EFT_FIAccountType]
			,EFT_INFO.[PrvAccountNo] AS [EFT_PrvAccountNo]
		FROM TB_PAYMENT_DN_STATUS(NOLOCK) PDS
			--CURRENT
			INNER JOIN TB_PAYMENT_STATUS(NOLOCK) PS_CURRENT ON PS_CURRENT.Payment_Status_ID = PDS.CurrentPaymentStatusID
			INNER JOIN TB_PAYMENT_STATUS_TYPE(NOLOCK) PST_CURRENT ON PST_CURRENT.Payment_Status_Type_ID = PS_CURRENT.Payment_Status_Type_ID
			--LATEST
			INNER JOIN TB_PAYMENT_STATUS(NOLOCK) PS_LATEST ON PS_LATEST.Payment_Status_ID = PDS.LatestPaymentStatusID
			--HOLD
			LEFT JOIN TB_PAYMENT_STATUS(NOLOCK) PS_CURRENT_HOLD ON PS_CURRENT_HOLD.Payment_Status_ID = PDS.CurrentHoldStatusID
			--UNHOLD
			LEFT JOIN TB_PAYMENT_STATUS(NOLOCK) PS_CURRENT_UNHOLD ON PS_CURRENT_UNHOLD.Payment_Status_ID = PDS.CurrentUnHoldStatusID
			--RELEASE FROM SUB
			LEFT JOIN TB_PAYMENT_STATUS(NOLOCK) PS_CURRENT_RELEASE ON PS_CURRENT_RELEASE.Payment_Status_ID = PDS.CurrentReleaseFromSupStatusID
		
			LEFT JOIN TB_PAYMENT_USER_ASSIGNMENT(NOLOCK) PUA ON PUA.Payment_User_Assignment_ID = PDS.CurrentUserAssignmentID
			LEFT JOIN TB_USER(NOLOCK) USR ON USR.[User_ID] = PUA.[User_ID]

			INNER JOIN TB_PAYMENT_RECORD(NOLOCK) PR ON PR.Payment_Record_ID = PDS.Payment_Record_ID
			INNER JOIN TB_PAYMENT_CLAIM_SCHEDULE(NOLOCK) PRCS ON PRCS.Payment_Record_ID = PR.Payment_Record_ID

			INNER JOIN TB_PAYMENT_RECORD_EXT_CAPMAN(NOLOCK) PRExt ON PRExt.Payment_Record_ID = PR.Payment_Record_ID	
			INNER JOIN TB_PEE_ADDRESS(NOLOCK) PEEA ON PEEA.PEE_Address_ID = PR.PEE_Address_ID			
			INNER JOIN TB_PEE_SYSTEM(NOLOCK) PEES ON PEES.PEE_System_ID = PEEA.PEE_System_ID
			LEFT JOIN TB_PAYMENT_PAYDATE_CALENDAR(NOLOCK) PDT ON PDT.Payment_Record_ID = PR.Payment_Record_ID
			--EFT
			LEFT JOIN TB_PEE_EFT_INFO EFT_INFO ON PR.PEE_EFT_Info_ID = EFT_INFO.PEE_EFT_Info_ID
		WHERE PRCS.Claim_Schedule_ID in (SELECT Claim_Schedule_ID from #cs)

		--DROP TEMP TABLE
		drop table #cs

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
