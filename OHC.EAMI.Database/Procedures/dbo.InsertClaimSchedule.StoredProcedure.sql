USE [EAMI-PRX]
GO
/****** Object:  StoredProcedure [dbo].[InsertClaimSchedule]    Script Date: 5/28/2019 10:08:03 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertClaimSchedule]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[InsertClaimSchedule]
/****** Object:  StoredProcedure [dbo].[InsertClaimSchedule]    Script Date: 7/21/2020 2:26:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/******************************************************************************
* PROCEDURE:  [dbo].[InsertClaimSchedule]
* PURPOSE: Inserts InsertClaimSchedule and returns identity
* NOTES:
* CREATED: 12/21/2017  Genady G.
* MODIFIED
* DATE			AUTHOR      DESCRIPTION
* 7/11/2020		Genady G	Add new insert parameters: @PEEAddressID, @EFTInfoID
*------------------------------------------------------------------------------
*****************************************************************************/
CREATE PROCEDURE [dbo].[InsertClaimSchedule] 
	@ClaimScheduleNumber VARCHAR(20)
	,@Amount MONEY
	,@ClaimScheduleDate DATETIME
	,@FiscalYear VARCHAR(10)
	,@PaymentType VARCHAR(50)
	,@ContractNumber VARCHAR(20)	
	,@ExclusivePaymentTypeID INT
	,@PaydateCalendarID INT
	,@PEEAddressID INT
	,@IsLinked bit
	,@LinkedByPGNumber varchar(15)
	,@EFTInfoID INT
	,@PaymentMethodTypeID INT
	
AS
BEGIN
	BEGIN TRY				

		-- insert payment record
		INSERT INTO [dbo].[TB_CLAIM_SCHEDULE] (
			[Claim_Schedule_Number]
			,[Amount]
			,[Claim_Schedule_Date]
			,[FiscalYear]
			,[Payment_Type]
			,[ContractNumber]			
			,[Exclusive_Payment_Type_ID]
			,[Paydate_Calendar_ID]
			,[PEE_Address_ID]
			,[IsLinked]
			,[LinkedByPGNumber]
			,[PEE_EFT_Info_ID]
			,[Payment_Method_Type_ID]
			)
		VALUES (
			@ClaimScheduleNumber
			,@Amount
			,@ClaimScheduleDate
			,@FiscalYear
			,@PaymentType
			,@ContractNumber			
			,@ExclusivePaymentTypeID
			,@PaydateCalendarID
			,@PEEAddressID
			,@IsLinked
			,@LinkedByPGNumber
			,@EFTInfoID
			,@PaymentMethodTypeID
			)

		SELECT SCOPE_IDENTITY() AS [SCOPE_IDENTITY]
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
