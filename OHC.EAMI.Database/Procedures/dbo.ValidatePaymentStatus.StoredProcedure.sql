USE [EAMI-MC]
GO
/****** Object:  StoredProcedure [dbo].[ValidatePaymentStatus]    Script Date: 5/28/2019 10:08:03 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ValidatePaymentStatus]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[ValidatePaymentStatus]
GO
/****** Object:  StoredProcedure [dbo].[ValidatePaymentStatus]    Script Date: 5/28/2019 10:08:03 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/******************************************************************************
* PROCEDURE:  [dbo].[ValidatePaymentStatus]
* PURPOSE: Validates if Payment Payment Status
* NOTES:
* CREATED: 11/30/2017  Genady G.
* MODIFIED
* DATE     AUTHOR      DESCRIPTION
*------------------------------------------------------------------------------
*****************************************************************************/
CREATE PROCEDURE [dbo].[ValidatePaymentStatus] 
	@PaymentRecId INT
	,@NewPaymentStatusTypeId INT	
AS
BEGIN
	BEGIN TRY
		DECLARE @IsStatusValid bit = 1
		DECLARE @Message varchar (150) = null

		DECLARE @ReceivedPaymentStatusTypeID int = (SELECT [Payment_Status_Type_ID] from [dbo].[TB_PAYMENT_STATUS_TYPE] WHERE [Code] = 'RECEIVED')
		DECLARE @AssignedPaymentStatusTypeID int = (SELECT [Payment_Status_Type_ID] from [dbo].[TB_PAYMENT_STATUS_TYPE] WHERE [Code] = 'ASSIGNED')
		DECLARE @UnassignedPaymentStatusTypeID int = (SELECT [Payment_Status_Type_ID] from [dbo].[TB_PAYMENT_STATUS_TYPE] WHERE [Code] = 'UNASSIGNED')
		DECLARE @OnHoldPaymentStatusTypeID int = (SELECT [Payment_Status_Type_ID] from [dbo].[TB_PAYMENT_STATUS_TYPE] WHERE [Code] = 'HOLD')
		DECLARE @UnHoldPaymentStatusTypeID int = (SELECT [Payment_Status_Type_ID] from [dbo].[TB_PAYMENT_STATUS_TYPE] WHERE [Code] = 'UNHOLD')
		DECLARE @ReturnedToSupStatusTypeID int = (SELECT [Payment_Status_Type_ID] from [dbo].[TB_PAYMENT_STATUS_TYPE] WHERE [Code] = 'RETURNED_TO_SUP')
		DECLARE @ReleasedFromSupStatusTypeID int = (SELECT [Payment_Status_Type_ID] from [dbo].[TB_PAYMENT_STATUS_TYPE] WHERE [Code] = 'RELEASED_FROM_SUP')
		DECLARE @AddedToCSStatusTypeID int = (SELECT [Payment_Status_Type_ID] from [dbo].[TB_PAYMENT_STATUS_TYPE] WHERE [Code] = 'ADDED_TO_CS')
		DECLARE @ReturnedToSORStatusTypeID int = (SELECT [Payment_Status_Type_ID] from [dbo].[TB_PAYMENT_STATUS_TYPE] WHERE [Code] = 'RETURNED_TO_SOR')
		
		DECLARE @CurrentPaymentStatusID int;
		DECLARE @CurrentUserAssignmentID int;
		DECLARE @CurrentHoldStatusID int;
		DECLARE @CurrentPaymentStatusTypeID int;
		
		--capture status list for a given payment record
		--SELECT * INTO #tmpStatusList FROM TB_PAYMENT_STATUS WHERE Payment_Record_ID = @PaymentRecId
		
		--capture Curerent status for a given payment record
		SELECT 
			@CurrentPaymentStatusID = dn.CurrentPaymentStatusID,
			@CurrentPaymentStatusTypeID = ps.Payment_Status_Type_ID,
			@CurrentUserAssignmentID = dn.CurrentUserAssignmentID,
			@CurrentHoldStatusID = dn.CurrentHoldStatusID
		FROM TB_PAYMENT_DN_STATUS dn 
			INNER JOIN TB_PAYMENT_STATUS ps on dn.CurrentPaymentStatusID = ps.Payment_Status_ID
		WHERE dn.Payment_Record_ID = @PaymentRecId
		
		-- VLIDATE ASSIGNED STATUS
		IF @NewPaymentStatusTypeId = @AssignedPaymentStatusTypeID AND @CurrentPaymentStatusTypeID = @AssignedPaymentStatusTypeID
		BEGIN
			set @IsStatusValid = 0;
			set @Message = 'One or more selected Payment Record Sets have already been assigned. Please check with the other person doing the assigning.';
		END	

		-- VLIDATE UNASSIGNED STATUS
		IF @NewPaymentStatusTypeId = @UnassignedPaymentStatusTypeID AND @CurrentPaymentStatusTypeID = @UnassignedPaymentStatusTypeID
		BEGIN
			set @IsStatusValid = 0;
			set @Message = 'One or more selected Payment Record Sets have already been unassigned. Please check with the other person doing the unassigned.';
		END	

		-- VLIDATE ON HOLD STATUS
		IF @NewPaymentStatusTypeId = @OnHoldPaymentStatusTypeID AND @CurrentPaymentStatusTypeID = @ReturnedToSupStatusTypeID
		BEGIN
			set @IsStatusValid = 0;
			set @Message = 'Cannot Hold. Payment Record is in Return To Sup status';			
		END
		ELSE IF @NewPaymentStatusTypeId = @OnHoldPaymentStatusTypeID AND @CurrentHoldStatusID IS NOT NULL
		BEGIN
			set @IsStatusValid = 0;
			set @Message = 'Cannot Hold. Payment Record is on Hold';
		END		
		
		-- VLIDATE UNHOLD STATUS		
		IF @NewPaymentStatusTypeId = @UnHoldPaymentStatusTypeID AND @CurrentPaymentStatusTypeID = @ReturnedToSupStatusTypeID
		BEGIN
			set @IsStatusValid = 0;
			set @Message = 'Cannot Unhold. Payment Record is in Return To Sup status';			
		END
		ELSE IF @NewPaymentStatusTypeId = @UnHoldPaymentStatusTypeID AND @CurrentHoldStatusID IS NULL
		BEGIN
			set @IsStatusValid = 0;
			set @Message = 'Cannot Unhold. Payment Record is not on Hold';
		END
		
		-- VLIDATE RETURN TO SUP STATUS
		IF @NewPaymentStatusTypeId = @ReturnedToSupStatusTypeID AND @CurrentHoldStatusID IS NOT NULL
		BEGIN
			set @IsStatusValid = 0;
			set @Message = 'Cannot Return to Sup. Payment Record is on Hold';			
		END
		ELSE IF @NewPaymentStatusTypeId = @ReturnedToSupStatusTypeID AND @CurrentPaymentStatusTypeID = @ReturnedToSupStatusTypeID
		BEGIN
			set @IsStatusValid = 0;
			set @Message = 'Cannot Return to Sup. Payment Record is in Return To Sup status';
		END
				
		-- VLIDATE RELEASE FROM SUP STATUS
		IF @NewPaymentStatusTypeId = @ReleasedFromSupStatusTypeID AND @CurrentHoldStatusID IS NOT NULL
		BEGIN
			set @IsStatusValid = 0;
			set @Message = 'Cannot Release from Sup. Payment Record is on Hold';			
		END
		ELSE IF @NewPaymentStatusTypeId = @ReleasedFromSupStatusTypeID AND @CurrentPaymentStatusTypeID <> @ReturnedToSupStatusTypeID
		BEGIN
			set @IsStatusValid = 0;
			set @Message = 'Cannot Release from Sup. Payment Record is not in Return To Sup status';
		END
		
		-- VLIDATE ADDED TO CLAIM SCHEDULE STATUS
		IF @NewPaymentStatusTypeId = @AddedToCSStatusTypeID AND @CurrentHoldStatusID IS NOT NULL
		BEGIN
			set @IsStatusValid = 0;
			set @Message = 'Cannot Add to Claim Schedule. Payment Record is on Hold';			
		END
		ELSE IF @NewPaymentStatusTypeId = @AddedToCSStatusTypeID AND @CurrentPaymentStatusTypeID <> @AssignedPaymentStatusTypeID
		BEGIN
			set @IsStatusValid = 0;
			set @Message = 'Cannot Add to Claim Schedule. Payment Record is not Assigned';
		END
		ELSE IF @NewPaymentStatusTypeId = @AddedToCSStatusTypeID AND @CurrentPaymentStatusTypeID = @AddedToCSStatusTypeID
		BEGIN
			set @IsStatusValid = 0;
			set @Message = 'Cannot Add to Claim Schedule. Payment Record is already added to Claim Schedule';
		END
		ELSE IF @NewPaymentStatusTypeId = @AddedToCSStatusTypeID AND @CurrentPaymentStatusTypeID = @ReturnedToSORStatusTypeID
		BEGIN
			set @IsStatusValid = 0;
			set @Message = 'Cannot Add to Claim Schedule. Payment Record is Returned to System of Record';
		END
				
		-- VLIDATE RETURN TO SYSTEM OF RECORD STATUS (HARD RETURN)
		IF @NewPaymentStatusTypeId = @ReturnedToSORStatusTypeID AND @CurrentHoldStatusID IS NOT NULL
		BEGIN
			set @IsStatusValid = 0;
			set @Message = 'Cannot Return to System of Record. Payment Record is on Hold';			
		END
		ELSE IF @NewPaymentStatusTypeId = @ReturnedToSORStatusTypeID AND @CurrentPaymentStatusTypeID = @ReturnedToSORStatusTypeID
		BEGIN
			set @IsStatusValid = 0;
			set @Message = 'Cannot Return to System of Record. Payment Record is already Returned to System of Record';
		END
		ELSE IF @NewPaymentStatusTypeId = @ReturnedToSORStatusTypeID AND (@CurrentPaymentStatusTypeID <> @UnassignedPaymentStatusTypeID AND @CurrentPaymentStatusTypeID <> @ReturnedToSupStatusTypeID)
		BEGIN
			set @IsStatusValid = 0;
			set @Message = 'Cannot Return to System of Record. Payment Record is not Unassigned or is not in Return To Sup status';
		END
		
		--RETURN Status and message on failed validation
		SELECT @IsStatusValid as [IsStatusValid], @Message as [Message]

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
