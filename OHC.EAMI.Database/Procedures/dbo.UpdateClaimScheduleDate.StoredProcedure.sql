USE [EAMI-MC]
GO
/****** Object:  StoredProcedure [dbo].[UpdateClaimScheduleDate]    Script Date: 5/28/2019 10:08:03 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UpdateClaimScheduleDate]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[UpdateClaimScheduleDate]
GO
/****** Object:  StoredProcedure [dbo].[UpdateClaimScheduleDate]    Script Date: 5/28/2019 10:08:03 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/******************************************************************************
* PROCEDURE:  [dbo].[UpdateClaimScheduleDate]
* PURPOSE: Updates Claim Schedule Date 
* NOTES:
* CREATED: 06/12/2019  Genady G.
* MODIFIED
* DATE       AUTHOR      DESCRIPTION
*------------------------------------------------------------------------------
* 
* 
* 
*****************************************************************************/
CREATE PROCEDURE [dbo].[UpdateClaimScheduleDate] 
	@ecs_id INT,
	@cs_date DATETIME
AS
BEGIN
	BEGIN TRY	
		
		--UPDATE CS DATE 
		UPDATE CS
		SET 
			CS.Claim_Schedule_Date = @cs_date
		FROM TB_CLAIM_SCHEDULE CS
			INNER JOIN TB_CLAIM_SCHEDULE_ECS CS_ECS ON CS.Claim_Schedule_ID = CS_ECS.Claim_Schedule_ID
		WHERE CS_ECS.ECS_ID = @ecs_id


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
