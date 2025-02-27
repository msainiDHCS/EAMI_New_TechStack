USE [EAMI-PRX]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertEFTInfo]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[InsertEFTInfo]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/******************************************************************************
* PROCEDURE:  [dbo].[InsertEFTInfo]
* PURPOSE: Inserts EFT Info and returns identity
* NOTES:
* CREATED: 08/10/2020  Genady G.
* MODIFIED
* DATE			AUTHOR      DESCRIPTION
*------------------------------------------------------------------------------
* 08/27/2020	Genady G.	Refactor SP to use PEE_ID instead of PaymentRecordListID and PR Status 'RECEIVED'
* 01/05/2022	Eugene S.	Adding transaction-id parameter to limit the scope of payments to a specific transaction 
*****************************************************************************/

ALTER PROCEDURE [dbo].[InsertEFTInfo] 
	@SystemID INT
	,@PEE_ID INT
	,@TransactionID varchar(50)
	,@FIRoutingNumber VARCHAR(20)
	,@FIAccountType VARCHAR(20)
	,@PrvAccountNo VARCHAR(20)
	,@DatePrenoted DATETIME
AS
BEGIN
	BEGIN TRY
				
		DECLARE @PR_Status_TypeID_RECEIVED int = (SELECT [Payment_Status_Type_ID] from [dbo].[TB_PAYMENT_STATUS_TYPE] WHERE [Code] = 'RECEIVED')
		DECLARE @PR_Status_TypeID_UNASSIGNED int = (SELECT [Payment_Status_Type_ID] from [dbo].[TB_PAYMENT_STATUS_TYPE] WHERE [Code] = 'UNASSIGNED')

		CREATE TABLE #PaymentRecordIDList
		(
			PaymentRecordID int
		)

		INSERT INTO #PaymentRecordIDList 
		SELECT PR.Payment_Record_ID FROM dbo.[TB_PAYMENT_RECORD] PR
			INNER JOIN dbo.[TB_TRANSACTION] T on T.Transaction_ID = PR.Transaction_ID
			INNER JOIN dbo.[TB_PEE_ADDRESS] PEEA ON PR.[PEE_Address_ID] = PEEA.[PEE_Address_ID]
			INNER JOIN dbo.[TB_PEE_SYSTEM] PEES ON PEEA.[PEE_System_ID] = PEES.[PEE_System_ID]
			INNER JOIN dbo.[TB_PAYMENT_DN_STATUS] PRDN ON PR.[Payment_Record_ID] = PRDN.[Payment_Record_ID]
			INNER JOIN dbo.[TB_PAYMENT_STATUS] PRST ON PRDN.[CurrentPaymentStatusID] = PRST.[Payment_Status_ID]
		WHERE PEES.[Payment_Exchange_Entity_ID] = @PEE_ID 
			AND T.Msg_Transaction_ID = @TransactionID
			AND PEES.SOR_ID = @SystemID 
			AND PRST.Payment_Status_Type_ID = @PR_Status_TypeID_RECEIVED

		-- 1. check for existing PEE_EFT_INFO
		DECLARE @EFTInfoID INT
		SELECT TOP 1 @EFTInfoID = [PEE_EFT_Info_ID]
		FROM [dbo].[TB_PEE_EFT_INFO] (NOLOCK) EFT	
			INNER JOIN dbo.[TB_PEE_SYSTEM] PEES ON EFT.[PEE_System_ID] = PEES.[PEE_System_ID]
		WHERE 
			PEES.[SOR_ID] = @SystemID
			AND [Payment_Exchange_Entity_ID] = @PEE_ID 
			AND [FIRoutingNumber] = @FIRoutingNumber
			AND [FIAccountType] = @FIAccountType
			AND [PrvAccountNo] = @PrvAccountNo
			AND [DatePrenoted] = @DatePrenoted

		--INSERT IF NO PEE_EFT_INFO EXIST  
		IF @EFTInfoID IS NULL
		BEGIN 
		DECLARE @PEE_System_ID int = (SELECT TOP 1 PEE_System_ID FROM TB_PEE_SYSTEM WHERE SOR_ID = @SystemID and Payment_Exchange_Entity_ID = @PEE_ID)
		INSERT INTO [dbo].[TB_PEE_EFT_INFO] (
			[PEE_System_ID]
			,[FIRoutingNumber]
			,[FIAccountType]
			,[PrvAccountNo]
			,[DatePrenoted]
			,[CreateDate]
			)
		VALUES (
			@PEE_System_ID
			,@FIRoutingNumber
			,@FIAccountType
			,@PrvAccountNo
			,@DatePrenoted
			,GETDATE()
			)
			
			SET @EFTInfoID = SCOPE_IDENTITY()
		END

		--UPDATE PAYMENT RECORD WITH @EFTInfoID
		UPDATE [dbo].[TB_PAYMENT_RECORD] 
		SET [PEE_EFT_Info_ID] = @EFTInfoID 
		WHERE [Payment_Record_ID] IN
		(SELECT PaymentRecordID FROM #PaymentRecordIDList)
		
		--UPDATE PAYMENR RECORD STATUS TO UNASSIGNED
		INSERT INTO [dbo].[TB_PAYMENT_STATUS] (
			[Payment_Record_ID]
			,[Payment_Status_Type_ID]
			,[Status_Date]
			,[Status_Note]
			,[CreatedBy]
			)
		SELECT 
			PaymentRecordID  
			,@PR_Status_TypeID_UNASSIGNED
			, GETDATE()
			,NULL
			,'system'
		FROM #PaymentRecordIDList

		--UPDATE DN STATUS
		UPDATE PR_DNST 
		SET 
			PR_DNST.[CurrentPaymentStatusID]  = PR_ST.Payment_Status_ID
			,PR_DNST.[LatestPaymentStatusID] = PR_ST.Payment_Status_ID
		FROM dbo.[TB_PAYMENT_DN_STATUS] AS PR_DNST
			INNER JOIN #PaymentRecordIDList PR ON PR_DNST.Payment_Record_ID = PR.PaymentRecordID
			INNER JOIN dbo.[TB_PAYMENT_STATUS] AS PR_ST ON PR_DNST.Payment_Record_ID = PR_ST.Payment_Record_ID 
												AND PR_ST.Payment_Status_Type_ID = @PR_Status_TypeID_UNASSIGNED

		DROP TABLE #PaymentRecordIDList

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

