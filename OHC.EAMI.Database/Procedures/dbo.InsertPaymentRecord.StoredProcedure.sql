USE [EAMI-PRX]
GO
/****** Object:  StoredProcedure [dbo].[InsertPaymentRecord]    Script Date: 5/28/2019 10:08:03 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertPaymentRecord]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[InsertPaymentRecord]
GO
/****** Object:  StoredProcedure [dbo].[InsertPaymentRecord]    Script Date: 7/21/2020 2:33:35 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/******************************************************************************
* PROCEDURE:  [dbo].[InsertPaymentRecord]
* PURPOSE: Inserts PaymentRecord (with Payee info) and returns identity
* NOTES:
* CREATED: 09/19/2017  Eugene S.
* MODIFIED
* DATE			AUTHOR      DESCRIPTION
* 7/11/2020		Genady G	Add new insert parameters: @PEEAddressID, @EFTInfoID
* 9/28/2020		Eugene S	Removing EFTInfoID insert as it will happen later
*------------------------------------------------------------------------------
*****************************************************************************/
CREATE PROCEDURE [dbo].[InsertPaymentRecord] 
	@TransactionId INT
	,@PaymentRecNumber VARCHAR(30)
	,@PaymentRecNumberExt VARCHAR(30)
	,@PaymentSetNumber VARCHAR(30)
	,@PaymentSetNumberExt VARCHAR(30)
	,@PaymentType VARCHAR(50)
	,@PaymentDate DATETIME
	,@Amount MONEY
	,@FiscalYear VARCHAR(10)
	,@IndexCode VARCHAR(10)
	,@ObjectDetailCode VARCHAR(10)
	,@ObjectAgencyCode VARCHAR(10) = NULL
	,@PCACode VARCHAR(10)
	,@ApprovedBy VARCHAR(30)
	,@PEEAddressID INT
	,@RPICode VARCHAR(2)
	,@IsReportableRPI BIT
	,@ContractNumber VARCHAR(20)
	,@ContractDateFrom DATETIME
	,@ContractDateTo DATETIME
	,@ExclusivePaymentTypeId INT
	,@PaymentMethodTypeID INT
AS
BEGIN
	BEGIN TRY				

		-- insert payment record
		INSERT INTO [dbo].[TB_PAYMENT_RECORD] (
			[Transaction_ID]
			,[PaymentRec_Number]
			,[PaymentRec_NumberExt]
			,[Payment_Type]
			,[Payment_Date]
			,[Amount]
			,[FiscalYear]
			,[IndexCode]
			,[ObjectDetailCode]
			,[ObjectAgencyCode]
			,[PCACode]
			,[ApprovedBy]
			,[PaymentSet_Number]
			,[PaymentSet_NumberExt]
			,[PEE_Address_ID]
			,[RPICode]
			,[IsReportableRPI]
			,[Payment_Method_Type_ID]
			)
		VALUES (
			@TransactionId
			,@PaymentRecNumber
			,@PaymentRecNumberExt
			,@PaymentType
			,@PaymentDate
			,@Amount
			,@FiscalYear
			,@IndexCode
			,@ObjectDetailCode
			,@ObjectAgencyCode
			,@PCACode
			,@ApprovedBy
			,@PaymentSetNumber
			,@PaymentSetNumberExt
			,@PEEAddressID
			,@RPICode
			,@IsReportableRPI
			,@PaymentMethodTypeID
			)

		declare @paymentRecId int
		set @paymentRecId = SCOPE_IDENTITY()

		--insert payment-rec extension
		INSERT INTO [dbo].[TB_PAYMENT_RECORD_EXT_CAPMAN] (
			[Payment_Record_ID]
			,[ContractNumber]
			,[ContractDateFrom]
			,[ContractDateTo]
			,[Exclusive_Payment_Type_ID]
			)
		VALUES (
			@paymentRecId
			,@ContractNumber
			,@ContractDateFrom
			,@ContractDateTo
			,@ExclusivePaymentTypeId
			)			

		SELECT @paymentRecId AS [SCOPE_IDENTITY];
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
