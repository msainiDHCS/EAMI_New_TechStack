USE [EAMI-PRX]
GO
/****** Object:  StoredProcedure [dbo].[DeleteElectronicClaimSchedule]    Script Date: 5/28/2019 10:08:03 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DeleteElectronicClaimSchedule]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[DeleteElectronicClaimSchedule]
GO
/****** Object:  StoredProcedure [dbo].[DeleteElectronicClaimSchedule]    Script Date: 5/28/2019 10:08:03 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/******************************************************************************
* PROCEDURE:  [dbo].[DeleteElectronicClaimSchedule]
* PURPOSE: Delete ESC 
* NOTES:
* CREATED: 07/27/2018  Genady G.
* MODIFIED
* DATE				AUTHOR					DESCRIPTION
*02/05/2019			Genady G				Add @StatusDate parameter
*------------------------------------------------------------------------------
*****************************************************************************/
CREATE PROCEDURE [dbo].[DeleteElectronicClaimSchedule] 
	@EcsId int	
	,@Note varchar(200) = ''
	,@User varchar(20) = 'system'
	,@StatusDate datetime
AS
BEGIN
	BEGIN TRY		
		DECLARE @ECS_Current_StatusTypeID int = (SELECT Current_ECS_Status_Type_ID FROM [dbo].[TB_ECS] (NOLOCK) WHERE ECS_ID = @EcsId)
		DECLARE @CS_StatusTypeId_SUBMIT_FOR_APPROVAL int = (SELECT [Claim_Schedule_Status_Type_ID] FROM [dbo].[TB_CLAIM_SCHEDULE_STATUS_TYPE] (NOLOCK) WHERE [Code] = 'SUBMIT_FOR_APPROVAL')		
		DECLARE @result bit  = 1
		DECLARE @resultMessage varchar(200)  = ''				
		
		--CAPTURE STATUSES WHEN DELETION CAN BE DONE
		CREATE TABLE #ecs_status_typeID_list ([ECS_Status_Type_ID] int);
		INSERT INTO #ecs_status_typeID_list SELECT [ECS_Status_Type_ID] FROM [dbo].[TB_ECS_STATUS_TYPE] (NOLOCK) WHERE [Code] IN ('PENDING')		

		--CHECK IF ECS is in the right status
		IF EXISTS (SELECT NULL FROM #ecs_status_typeID_list WHERE [ECS_Status_Type_ID] = @ECS_Current_StatusTypeID)
		BEGIN
			--CAPTURE CS ID's
			CREATE TABLE #csID ([Claim_Schedule_ID] int);
			INSERT INTO #csID SELECT Claim_Schedule_ID FROM [dbo].[TB_CLAIM_SCHEDULE_ECS] (NOLOCK)  WHERE [ECS_ID] = @EcsId
		
			--DELETE
			DELETE FROM [dbo].[TB_CLAIM_SCHEDULE_ECS] WHERE [ECS_ID] = @EcsId
			DELETE FROM [dbo].[TB_ECS] WHERE [ECS_ID] = @EcsId

			--CHANGE STATUS ON CS	
			INSERT INTO [dbo].[TB_CLAIM_SCHEDULE_STATUS] (
				[Claim_Schedule_ID]
				,[Claim_Schedule_Status_Type_ID]
				,[Status_Date]
				,[Status_Note]
				,[CreatedBy]
				)
			SELECT [Claim_Schedule_ID]
				,@CS_StatusTypeId_SUBMIT_FOR_APPROVAL
				,@StatusDate
				,@Note
				,@User
			FROM #csID CS(NOLOCK)

			--UPDATE CS DN status
			UPDATE CSDS
			SET CSDS.[CurrentCSStatusID] = CST.Claim_Schedule_Status_ID
				,CSDS.[LatestCSStatusID] = CST.Claim_Schedule_Status_ID
			FROM [dbo].[TB_CLAIM_SCHEDULE_STATUS] CST(NOLOCK)
				INNER JOIN [dbo].[TB_CLAIM_SCHEDULE_DN_STATUS] CSDS (NOLOCK) ON CSDS.Claim_Schedule_ID = CST.Claim_Schedule_ID
			WHERE 
				CST.Claim_Schedule_ID IN (SELECT [Claim_Schedule_ID] FROM #csID)
				AND CST.Claim_Schedule_Status_Type_ID = @CS_StatusTypeId_SUBMIT_FOR_APPROVAL
				AND CST.Status_Date = @StatusDate
				
			DROP TABLE #csID
		END
		ELSE
		BEGIN
			--VALIDATION STATUS and MESSAGE
			SET @result = 0
			SET @resultMessage = 'Cannot delete ECS in current status of ' + (SELECT CODE FROM [dbo].[TB_ECS_STATUS_TYPE] (NOLOCK) WHERE ECS_Status_Type_ID = @ECS_Current_StatusTypeID)
		END

		SELECT @result, @resultMessage

		DROP TABLE #ecs_status_typeID_list

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
