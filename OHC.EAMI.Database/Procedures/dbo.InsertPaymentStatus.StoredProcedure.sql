USE [EAMI-MC]
GO
/****** Object:  StoredProcedure [dbo].[InsertPaymentStatus]    Script Date: 5/28/2019 10:08:03 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertPaymentStatus]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[InsertPaymentStatus]
GO
/****** Object:  StoredProcedure [dbo].[InsertPaymentStatus]    Script Date: 5/28/2019 10:08:03 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/******************************************************************************
* PROCEDURE:  [dbo].[InsertPaymentStatus]
* PURPOSE: Inserts Payment Status
* NOTES:
* CREATED: 10/05/2017  Eugene S.
* MODIFIED
* DATE				AUTHOR					DESCRIPTION
*02/05/2019			Genady G				Add @StatusDate parameter
*------------------------------------------------------------------------------
*****************************************************************************/
CREATE PROCEDURE [dbo].[InsertPaymentStatus] 
	@PaymentRecId INT
	,@PaymentStatusTypeId INT
	,@StatusNote varchar(200)	
	,@CreatedBy varchar(20)
	,@StatusDate datetime
AS
BEGIN
	BEGIN TRY
		DECLARE @ReceivedPaymentStatusTypeID int = (SELECT [Payment_Status_Type_ID] from [dbo].[TB_PAYMENT_STATUS_TYPE] WHERE [Code] = 'RECEIVED')
		DECLARE @AssignedPaymentStatusTypeID int = (SELECT [Payment_Status_Type_ID] from [dbo].[TB_PAYMENT_STATUS_TYPE] WHERE [Code] = 'ASSIGNED')
		DECLARE @UnassignedPaymentStatusTypeID int = (SELECT [Payment_Status_Type_ID] from [dbo].[TB_PAYMENT_STATUS_TYPE] WHERE [Code] = 'UNASSIGNED')
		DECLARE @OnHoldPaymentStatusTypeID int = (SELECT [Payment_Status_Type_ID] from [dbo].[TB_PAYMENT_STATUS_TYPE] WHERE [Code] = 'HOLD')
		DECLARE @UnHoldPaymentStatusTypeID int = (SELECT [Payment_Status_Type_ID] from [dbo].[TB_PAYMENT_STATUS_TYPE] WHERE [Code] = 'UNHOLD')
		DECLARE @ReturnedToSupStatusTypeID int = (SELECT [Payment_Status_Type_ID] from [dbo].[TB_PAYMENT_STATUS_TYPE] WHERE [Code] = 'RETURNED_TO_SUP')
		DECLARE @ReleasedFromSupStatusTypeID int = (SELECT [Payment_Status_Type_ID] from [dbo].[TB_PAYMENT_STATUS_TYPE] WHERE [Code] = 'RELEASED_FROM_SUP')
		
		-- INSERT values into historical inv status table
		INSERT INTO [dbo].[TB_PAYMENT_STATUS] (
			[Payment_Record_ID]
			,[Payment_Status_Type_ID]
			,[Status_Date]
			,[Status_Note]
			,[CreatedBy]
			)
		VALUES (
			@PaymentRecId
			,@PaymentStatusTypeId
			,@StatusDate
			,@StatusNote
			,@CreatedBy
			)

		-- get newly inserted inv status identity
		declare @pymtStatusId INT = (SELECT SCOPE_IDENTITY() AS [SCOPE_IDENTITY])		

		
		-- INSERT OR UPDATE CURRENT PAYMENT_DN_STATUS
		IF @PaymentStatusTypeId = @ReceivedPaymentStatusTypeID
		BEGIN
			-- INSERT to PAYMENT_DN_STATUS on initial RECEIVED status
			INSERT INTO [dbo].[TB_PAYMENT_DN_STATUS] (
				[Payment_Record_ID]
				,[CurrentPaymentStatusID]
				,[LatestPaymentStatusID]
				)
			VALUES (
				@PaymentRecId
				,@pymtStatusId
				,@pymtStatusId
				)
		END
		--UPDATE CURRENT PAYMENT STATUS on all statuses but ON_HOLD, UN_HOLD, RETURNED_TO_SUP and RELEASE_FROM_SUP
		--ELSE IF @PaymentStatusTypeId NOT IN (@OnHoldPaymentStatusTypeID, @UnHoldPaymentStatusTypeID, @ReturnedToSupStatusTypeID, @ReleasedFromSupStatusTypeID)
		ELSE IF @PaymentStatusTypeId NOT IN (@OnHoldPaymentStatusTypeID, @UnHoldPaymentStatusTypeID, @ReleasedFromSupStatusTypeID)
		BEGIN
			--UPDATE
			UPDATE [TB_PAYMENT_DN_STATUS]
			SET [CurrentPaymentStatusID] = @pymtStatusId, [LatestPaymentStatusID] = @pymtStatusId
			WHERE Payment_Record_ID = @PaymentRecId
		END
		ELSE 
		BEGIN
			--UPDATE
			UPDATE [TB_PAYMENT_DN_STATUS]
			SET [LatestPaymentStatusID] = @pymtStatusId
			WHERE Payment_Record_ID = @PaymentRecId
		END

		-- HANDLE SPECIFIC STATUS CASES BELOW

		-- ONHOLD STATUS
		IF @PaymentStatusTypeId = @OnHoldPaymentStatusTypeID
		BEGIN
			UPDATE [TB_PAYMENT_DN_STATUS]
			SET [CurrentHoldStatusID] = @pymtStatusId, [CurrentUnHoldStatusID] = NULL
			WHERE Payment_Record_ID = @PaymentRecId
		END
		
		-- UNHOLD STATUS
		ELSE IF @PaymentStatusTypeId = @UnHoldPaymentStatusTypeID
		BEGIN
			UPDATE [TB_PAYMENT_DN_STATUS]
			SET [CurrentHoldStatusID] = NULL, [CurrentUnHoldStatusID] = @pymtStatusId
			WHERE Payment_Record_ID = @PaymentRecId
		END
				
		-- RELEASE FROM SUP STATUS
		ELSE IF @PaymentStatusTypeId = @ReleasedFromSupStatusTypeID
		BEGIN
			UPDATE [TB_PAYMENT_DN_STATUS]
			SET [CurrentReleaseFromSupStatusID] = @pymtStatusId
			WHERE Payment_Record_ID = @PaymentRecId
		END		

		--UNASSIGNED STATUS - Reset [CurrentUserAssignmentID] to Null on unassignment
		ELSE IF @PaymentStatusTypeId = @UnassignedPaymentStatusTypeID
		BEGIN
			UPDATE [TB_PAYMENT_DN_STATUS]
			SET [CurrentUserAssignmentID] = NULL
			WHERE Payment_Record_ID = @PaymentRecId
		END

		--RETURN newly created PAYMENT_STATUS_ID to caller
		SELECT @pymtStatusId
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
