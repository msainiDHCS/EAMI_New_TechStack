USE [EAMI-PRX]
GO
/****** Object:  StoredProcedure [dbo].[GetPaymentRecordsByPaymentSetNumbers]    Script Date: 5/28/2019 10:08:03 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetPaymentRecordsByPaymentSetNumbers]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetPaymentRecordsByPaymentSetNumbers]
GO
/****** Object:  StoredProcedure [dbo].[GetPaymentRecordsByPaymentSetNumbers]    Script Date: 7/21/2020 2:11:01 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/******************************************************************************
* PROCEDURE:  [dbo].[GetPaymentRecordsByPaymentSetNumbers]
* PURPOSE: Gets payment records by Payment Set Number list
* NOTES:
* CREATED: 1/23/2018  Genady G.
* MODIFIED
* DATE			AUTHOR      DESCRIPTION
*------------------------------------------------------------------------------
* 2/21/2019		Eugene S	updated entity info address details, changed to only return
							entity_id and user_id (the rest will be loaded from cached ref list)
* 7/11/2020		Genady G	Refactor to use new tables [TB_PEE_ADDRESS] and [TB_PEE_SYSTEM]
*****************************************************************************/
CREATE PROCEDURE [dbo].[GetPaymentRecordsByPaymentSetNumbers]
	@PaymentSetNumberList VARCHAR(max)	
AS
BEGIN
	BEGIN TRY

		-- convert list to xml
		DECLARE @xml XML
		SET @xml = N'<root><r>' + replace(@PaymentSetNumberList, ',', '</r><r>') + '</r></root>'
		
		SELECT PR.[Payment_Record_ID]
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
			--VENDOR
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
			--USER ASSIGNMENT
			,USR.[User_ID]
			--CURRENT STATUS
			,PS_CURRENT.Payment_Status_ID as [Current_Payment_Status_ID]
			,PS_CURRENT.Payment_Status_Type_ID as [Current_Payment_Status_Type_ID]
			,PS_CURRENT.Status_Date as [Current_Status_Date]
			,PS_CURRENT.Status_Note as [Current_Status_Note]
			,PS_CURRENT.CreatedBy as [Current_Status_CreatedBy]
			--LATEST STATUS
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
			--PAYMENT RECORD
			INNER JOIN TB_PAYMENT_RECORD(NOLOCK) PR ON PR.Payment_Record_ID = PDS.Payment_Record_ID
			INNER JOIN TB_PAYMENT_RECORD_EXT_CAPMAN(NOLOCK) PRExt ON PRExt.Payment_Record_ID = PR.Payment_Record_ID
			--VENDOR		
			INNER JOIN TB_PEE_ADDRESS(NOLOCK) PEEA ON PEEA.PEE_Address_ID = PR.PEE_Address_ID			
			INNER JOIN TB_PEE_SYSTEM(NOLOCK) PEES ON PEES.PEE_System_ID = PEEA.PEE_System_ID
			--CURRENT STATUS
			INNER JOIN TB_PAYMENT_STATUS(NOLOCK) PS_CURRENT ON PS_CURRENT.Payment_Status_ID = PDS.CurrentPaymentStatusID
			INNER JOIN TB_PAYMENT_STATUS_TYPE(NOLOCK) PST_CURRENT ON PST_CURRENT.Payment_Status_Type_ID = PS_CURRENT.Payment_Status_Type_ID 
			--LATEST STATUS
			INNER JOIN TB_PAYMENT_STATUS(NOLOCK) PS_LATEST ON PS_LATEST.Payment_Status_ID = PDS.LatestPaymentStatusID
			--HOLD
			LEFT JOIN TB_PAYMENT_STATUS(NOLOCK) PS_CURRENT_HOLD ON PS_CURRENT_HOLD.Payment_Status_ID = PDS.CurrentHoldStatusID
			--UNHOLD
			LEFT JOIN TB_PAYMENT_STATUS(NOLOCK) PS_CURRENT_UNHOLD ON PS_CURRENT_UNHOLD.Payment_Status_ID = PDS.CurrentUnHoldStatusID
			--RELEASE FROM SUB
			LEFT JOIN TB_PAYMENT_STATUS(NOLOCK) PS_CURRENT_RELEASE ON PS_CURRENT_RELEASE.Payment_Status_ID = PDS.CurrentReleaseFromSupStatusID
			--USER ASSIGNMENT
			LEFT JOIN TB_PAYMENT_USER_ASSIGNMENT(NOLOCK) PUA ON PUA.Payment_User_Assignment_ID = PDS.CurrentUserAssignmentID
			LEFT JOIN TB_USER(NOLOCK) USR ON USR.[User_ID] = PUA.[User_ID]
			--PAYDATE
			LEFT JOIN TB_PAYMENT_PAYDATE_CALENDAR(NOLOCK) PDT ON PDT.Payment_Record_ID = PR.Payment_Record_ID
			--EFT
			LEFT JOIN TB_PEE_EFT_INFO EFT_INFO ON PR.PEE_EFT_Info_ID = EFT_INFO.PEE_EFT_Info_ID
		WHERE PR.[PaymentSet_Number] IN (SELECT t.value('.', 'VARCHAR(15)') FROM @xml.nodes('//root/r') AS a(t))

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
