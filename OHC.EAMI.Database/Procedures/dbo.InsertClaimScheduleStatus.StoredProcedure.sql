USE [EAMI-PRX]
GO
/****** Object:  StoredProcedure [dbo].[InsertClaimScheduleStatus]    Script Date: 5/28/2019 10:08:03 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertClaimScheduleStatus]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[InsertClaimScheduleStatus]
GO
/****** Object:  StoredProcedure [dbo].[InsertClaimScheduleStatus]    Script Date: 5/28/2019 10:08:03 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/******************************************************************************
* PROCEDURE:  [dbo].[InsertClaimScheduleStatus]
* PURPOSE: Insert Claim Schedule Status
* NOTES:
* CREATED: 12/21/2017  Genady G.
* MODIFIED
* DATE				AUTHOR					DESCRIPTION
*02/05/2019			Genady G				Add @StatusDate parameter
*------------------------------------------------------------------------------
*****************************************************************************/
CREATE PROCEDURE [dbo].[InsertClaimScheduleStatus] 
	@ClaimScheduleId INT
	,@ClaimScheduleStatusTypeId INT
	,@StatusNote varchar(200)	
	,@CreatedBy varchar(20)
	,@StatusDate datetime
AS
BEGIN
	BEGIN TRY
		
		DECLARE @CreatedClaimScheduleStatusTypeID int = (SELECT [Claim_Schedule_Status_Type_ID] from [dbo].[TB_CLAIM_SCHEDULE_STATUS_TYPE] WHERE [Code] = 'CREATED')
		
		-- INSERT values into historical claim schedule status table
		INSERT INTO [dbo].[TB_CLAIM_SCHEDULE_STATUS] (
			[Claim_Schedule_ID]
			,[Claim_Schedule_Status_Type_ID]
			,[Status_Date]
			,[Status_Note]
			,[CreatedBy]
			)
		VALUES (
			@ClaimScheduleId
			,@ClaimScheduleStatusTypeId
			,@StatusDate
			,@StatusNote
			,@CreatedBy
			)

		-- get newly inserted inv status identity
		declare @claimScheduleStatusId INT = (SELECT SCOPE_IDENTITY() AS [SCOPE_IDENTITY])		
		
		-- INSERT OR UPDATE CURRENT CLAIM_SCHEDULE_DN_STATUS
		IF @ClaimScheduleStatusTypeId = @CreatedClaimScheduleStatusTypeID
		BEGIN
			-- INSERT into CLAIM_SCHEDULE_DN_STATUS on initial CREATED status
			INSERT INTO [dbo].[TB_CLAIM_SCHEDULE_DN_STATUS] (
				[Claim_Schedule_ID]
				,[CurrentCSStatusID]
				,[LatestCSStatusID]
				,[CurrentUserAssignmentID]
				)
			VALUES (
				@ClaimScheduleId
				,@claimScheduleStatusId
				,@claimScheduleStatusId
				,null
				)
		END
		ELSE 
		BEGIN
			--UPDATE
			UPDATE [TB_CLAIM_SCHEDULE_DN_STATUS]
			SET [CurrentCSStatusID] = @claimScheduleStatusId, [LatestCSStatusID] = @claimScheduleStatusId
			WHERE Claim_Schedule_ID = @ClaimScheduleId
		END
		
		--RETURN newly created PAYMENT_STATUS_ID to caller
		SELECT @claimScheduleStatusId
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
