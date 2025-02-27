USE [EAMI-PRX]
GO
/****** Object:  StoredProcedure [dbo].[InsertClaimScheduleUserAssignment]    Script Date: 5/28/2019 10:08:03 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertClaimScheduleUserAssignment]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[InsertClaimScheduleUserAssignment]
GO
/****** Object:  StoredProcedure [dbo].[InsertClaimScheduleUserAssignment]    Script Date: 5/28/2019 10:08:03 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/******************************************************************************
* PROCEDURE:  [dbo].[InsertClaimScheduleUserAssignment]
* PURPOSE: Inserts Claim Schedule User Assignment 
* NOTES:
* CREATED: 12/26/2017  Genady G.
* MODIFIED
* DATE     AUTHOR      DESCRIPTION
*------------------------------------------------------------------------------
*****************************************************************************/
CREATE PROCEDURE [dbo].[InsertClaimScheduleUserAssignment] 
	@ClaimScheduleID INT
	,@UserID INT
AS
BEGIN
	BEGIN TRY
		
		DECLARE @AssignedClaimScheduleStatusID INT = (SELECT CurrentCSStatusID FROM TB_CLAIM_SCHEDULE_DN_STATUS WHERE Claim_Schedule_ID = @ClaimScheduleID)	

		--INSERT ClaimSchedule USER ASSIGNMENT
		INSERT INTO [dbo].[TB_CLAIM_SCHEDULE_USER_ASSIGNMENT] (
			[Claim_Schedule_ID]
			,[User_ID]
			,[Assigned_CS_Status_ID]
			)
		VALUES (
			@ClaimScheduleID
			,@UserID
			,@AssignedClaimScheduleStatusID
			)
						
		-- get newly inserted ClaimSchedule User Assignment identity
		DECLARE @ClaimScheduleUserAssignmentID INT = (SELECT SCOPE_IDENTITY() AS [SCOPE_IDENTITY])	
		
		--UPDATE ClaimSchedule DN STATUS
		UPDATE [dbo].[TB_CLAIM_SCHEDULE_DN_STATUS]
		SET [CurrentUserAssignmentID] = @ClaimScheduleUserAssignmentID
		WHERE [Claim_Schedule_ID] = @ClaimScheduleID

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
