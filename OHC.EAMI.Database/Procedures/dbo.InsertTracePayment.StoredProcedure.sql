USE [EAMI-MC]
GO
/****** Object:  StoredProcedure [dbo].[InsertTracePayment]    Script Date: 5/28/2019 10:08:03 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertTracePayment]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[InsertTracePayment]
GO
/****** Object:  StoredProcedure [dbo].[InsertTracePayment]    Script Date: 5/28/2019 10:08:03 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/******************************************************************************
* PROCEDURE:  [dbo].[InsertTracePayment]
* PURPOSE: Inserts Trace Payment and resturns identity
* NOTES:
* CREATED: 09/19/2017  Eugene S.
* MODIFIED
* DATE     AUTHOR      DESCRIPTION
*------------------------------------------------------------------------------
*****************************************************************************/
CREATE PROCEDURE [dbo].[InsertTracePayment] 
	@TraceTransactionId INT
	,@PaymentStatusTypeId INT
	,@PaymentStatusDate datetime
	,@PaymentStatusMsg VARCHAR(300)
	,@ClaimScheduleNumber varchar(20)
	,@ClaimScheduleDate datetime
	,@WarrantNumber varchar(20)
	,@WarrantDate datetime
	,@WarrantAmount money
	,@PaymentRecNumber VARCHAR(30)
	,@PaymentRecNumberExt VARCHAR(30)
	,@PaymentType VARCHAR(50)
	,@PaymentDate DATETIME
	,@Amount MONEY
	,@FiscalYear VARCHAR(10)
	,@IndexCode VARCHAR(10)
	,@ObjectDetailCode VARCHAR(10)
	,@ObjectAgencyCode VARCHAR(10)
	,@PCACode VARCHAR(10)
	,@ApprovedBy VARCHAR(30)
	,@PaymentSetNumber VARCHAR(30)
	,@PaymentSetNumberExt VARCHAR(30)
	,@PayeeEntityId VARCHAR(10)
	,@PayeeEntityIdType VARCHAR(30)
	,@PayeeEntityName VARCHAR(100)
	,@PayeeEntityIdSfx VARCHAR(10)
	,@PayeeAddressLine1 VARCHAR(80)
	,@PayeeAddressLine2 VARCHAR(80)
	,@PayeeAddressLine3 VARCHAR(80)
	,@PayeeCity	VARCHAR(40)
	,@PayeeState VARCHAR(2)
	,@PayeeZip	VARCHAR(10)
	,@PayeeEIN VARCHAR(10)
	,@PayeeVendorTypeCode VARCHAR(1)
	,@PaymentKvpXml XML
	,@PaymentFundingDetailXml XML
AS
BEGIN
	BEGIN TRY
		INSERT INTO [dbo].[TB_TRACE_PAYMENT] (
			[Trace_Transaction_ID]
			,[Payment_Status_Type_ID]
			,[Payment_Status_Date]
			,[Payment_Status_Message]
			,[ClaimScheduleNumber]
			,[ClaimScheduleDate]
			,[WarrantNumber]
			,[WarrantDate]
			,[WarrantAmount]
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
			,[Payee_Entity_ID]
			,[Payee_Entity_ID_Type]
			,[Payee_Entity_Name]
			,[Payee_Entity_ID_Suffix]
			,[Payee_Address_Line1]
			,[Payee_Address_Line2]
			,[Payee_Address_Line3]
			,[Payee_City]
			,[Payee_State]
			,[Payee_Zip]
			,[Payee_EIN]
			,[Payee_VendorTypeCode]
			,[Payment_Kvp_Xml]
			,[Payment_Funding_Details_Xml]
			)
		VALUES (
			@TraceTransactionId
			,@PaymentStatusTypeId
			,@PaymentStatusDate
			,@PaymentStatusMsg
			,@ClaimScheduleNumber
			,@ClaimScheduleDate
			,@WarrantNumber
			,@WarrantDate
			,@WarrantAmount
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
			,@PayeeEntityId
			,@PayeeEntityIdType
			,@PayeeEntityName
			,@PayeeEntityIdSfx
			,@PayeeAddressLine1
			,@PayeeAddressLine2
			,@PayeeAddressLine3
			,@PayeeCity
			,@PayeeState
			,@PayeeZip
			,@PayeeEIN
			,@PayeeVendorTypeCode
			,@PaymentKvpXml
			,@PaymentFundingDetailXml
			)

		SELECT SCOPE_IDENTITY() AS [SCOPE_IDENTITY];
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
