USE [EAMI-MC]
GO
/****** Object:  StoredProcedure [dbo].[ValidateClaimScheduleStatus]    Script Date: 5/28/2019 10:08:03 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ValidateClaimScheduleStatus]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[ValidateClaimScheduleStatus]
GO
/****** Object:  StoredProcedure [dbo].[ValidateClaimScheduleStatus]    Script Date: 5/28/2019 10:08:03 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/******************************************************************************
* PROCEDURE:  [dbo].[ValidateClaimScheduleStatus]
* PURPOSE: Validates if ClaimSchedule Status
* NOTES:
* CREATED:	1/9/2018  Genady G.
* MODIFIED
* DATE			AUTHOR			DESCRIPTION
* 3/28/2019		Alex Hoang		Increased @Message to varchar (75) to accomodate all chars in messages.
*------------------------------------------------------------------------------
*****************************************************************************/
CREATE PROCEDURE [dbo].[ValidateClaimScheduleStatus] 
	@ClaimScheduleId INT
	,@NewClaimScheduleStatusTypeId INT	
AS
BEGIN
	BEGIN TRY
		DECLARE @IsStatusValid bit = 1
		DECLARE @Message varchar (75) = null

		DECLARE @Cretaed_ClaimScheduleStatusTypeID int = (SELECT [Claim_Schedule_Status_Type_ID] from [dbo].[TB_Claim_Schedule_Status_TYPE] WHERE [Code] = 'CREATED')
		DECLARE @Assigned_ClaimScheduleStatusTypeID int = (SELECT [Claim_Schedule_Status_Type_ID] from [dbo].[TB_Claim_Schedule_Status_TYPE] WHERE [Code] = 'ASSIGNED')
		DECLARE @SubmitForApproval_ClaimScheduleStatusTypeID int = (SELECT [Claim_Schedule_Status_Type_ID] from [dbo].[TB_Claim_Schedule_Status_TYPE] WHERE [Code] = 'SUBMIT_FOR_APPROVAL')
		DECLARE @Approved_ClaimScheduleStatusTypeID int = (SELECT [Claim_Schedule_Status_Type_ID] from [dbo].[TB_Claim_Schedule_Status_TYPE] WHERE [Code] = 'APPROVED')
		DECLARE @ReturnToProcessor_ClaimScheduleStatusTypeID int = (SELECT [Claim_Schedule_Status_Type_ID] from [dbo].[TB_Claim_Schedule_Status_TYPE] WHERE [Code] = 'RETURN_TO_PROCESSOR')
		DECLARE @SentToSCO_ClaimScheduleStatusTypeID int = (SELECT [Claim_Schedule_Status_Type_ID] from [dbo].[TB_Claim_Schedule_Status_TYPE] WHERE [Code] = 'SENT_TO_SCO')
		DECLARE @ReturnFromSCO_ClaimScheduleStatusTypeID int = (SELECT [Claim_Schedule_Status_Type_ID] from [dbo].[TB_Claim_Schedule_Status_TYPE] WHERE [Code] = 'REJETED_BY_SCO')
		DECLARE @WarrantReceived_ClaimScheduleStatusTypeID int = (SELECT [Claim_Schedule_Status_Type_ID] from [dbo].[TB_Claim_Schedule_Status_TYPE] WHERE [Code] = 'WARRANT_RECEIVED')
		DECLARE @SentTOCALSTARS_ClaimScheduleStatusTypeID int = (SELECT [Claim_Schedule_Status_Type_ID] from [dbo].[TB_Claim_Schedule_Status_TYPE] WHERE [Code] = 'SENT_TO_CALSTARS')
		DECLARE @Deleted_ClaimScheduleStatusTypeID int = (SELECT [Claim_Schedule_Status_Type_ID] from [dbo].[TB_Claim_Schedule_Status_TYPE] WHERE [Code] = 'DELETED')
		
		DECLARE @CurrentClaimScheduleStatusID int;
		DECLARE @CurrentUserAssignmentID int;
		DECLARE @CurrentClaimScheduleStatusTypeID int;
		
		
		--capture Curerent status for a given ClaimSchedule record
		SELECT 
			@CurrentClaimScheduleStatusID = dn.CurrentCSStatusID,
			@CurrentClaimScheduleStatusTypeID = ps.Claim_Schedule_Status_Type_ID,
			@CurrentUserAssignmentID = dn.CurrentUserAssignmentID
		FROM TB_CLAIM_SCHEDULE_DN_STATUS dn 
			INNER JOIN TB_Claim_Schedule_Status ps on dn.CurrentCSStatusID = ps.Claim_Schedule_Status_ID
		WHERE dn.Claim_Schedule_ID = @ClaimScheduleId
		
		-- VLIDATE ASSIGNED STATUS
		IF @NewClaimScheduleStatusTypeId = @Assigned_ClaimScheduleStatusTypeID AND @CurrentClaimScheduleStatusTypeID = @Assigned_ClaimScheduleStatusTypeID
		BEGIN
			set @IsStatusValid = 0;
			set @Message = 'Cannot Assign, Claim Schedule is already ASSIGNED';
		END	

		-- SUBMIT FOR APPROVAL STATUS
		IF @NewClaimScheduleStatusTypeId = @SubmitForApproval_ClaimScheduleStatusTypeID AND @CurrentClaimScheduleStatusTypeID not in ( @Assigned_ClaimScheduleStatusTypeID, @ReturnToProcessor_ClaimScheduleStatusTypeID)
		BEGIN
			set @IsStatusValid = 0;
			set @Message = 'Cannot submit for approval';
		END	
		
		-- APPROVED STATUS
		IF @NewClaimScheduleStatusTypeId = @Approved_ClaimScheduleStatusTypeID AND @CurrentClaimScheduleStatusTypeID not in (@SubmitForApproval_ClaimScheduleStatusTypeID)
		BEGIN
			set @IsStatusValid = 0;
			set @Message = 'Cannot Approve';
		END	
		
		-- RETURN_TO_PROCESSOR STATUS
		IF @NewClaimScheduleStatusTypeId = @ReturnToProcessor_ClaimScheduleStatusTypeID AND @CurrentClaimScheduleStatusTypeID not in (@SubmitForApproval_ClaimScheduleStatusTypeID, @Approved_ClaimScheduleStatusTypeID)
		BEGIN
			set @IsStatusValid = 0;
			set @Message = 'Cannot Return To Processor';
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
