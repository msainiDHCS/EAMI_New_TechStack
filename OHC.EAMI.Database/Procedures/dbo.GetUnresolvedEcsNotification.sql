USE [EAMI-MC]
GO
/****** Object:  StoredProcedure [dbo].[GetUnresolvedEcsNotification]    Script Date: 5/28/2019 10:08:03 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetUnresolvedEcsNotification]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetUnresolvedEcsNotification]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/******************************************************************************
* PROCEDURE:  [dbo].[GetUnresolvedEcsNotification]
* PURPOSE: Gets ecs records with status as 'SENT_TO_SCO' for the passed N days 
* NOTES:
* CREATED: 09/17/2019  Eugene S.
* MODIFIED :10/24/2022  Sujitha K
* PURPOSE: Modifing automated "Unreconciled Payment" notification to be based on pay-date instead of the date file is 'sent to SCO'
* DATE		AUTHOR			DESCRIPTION
*------------------------------------------------------------------------------
*****************************************************************************/
CREATE PROCEDURE [dbo].[GetUnresolvedEcsNotification]
	@DaysPassed int
AS
BEGIN
	BEGIN TRY					
		
		SELECT ECS.ECS_Number
			,ECS.Amount
			,ECS.ECS_File_Name
			,ECS.PayDate
			,ECS.ApproveDate
			,ECS.ApprovedBy
			,ECS.SentToScoDate
			,ECS.PayDate
		FROM TB_ECS(NOLOCK) ECS
		INNER JOIN TB_ECS_STATUS_TYPE(NOLOCK) ECSST ON ECSST.ECS_Status_Type_ID = ECS.Current_ECS_Status_Type_ID
		WHERE 1 = 1
			AND ECSST.CODE = 'SENT_TO_SCO'
			AND ECS.PayDate < DATEADD(DAY, - 1 * @DaysPassed, getdate())
			-- for testing
			--AND ECS.SentToScoDate < DATEADD(DAY, - 1, '2019-09-15')
		ORDER BY ECS.SentToScoDate

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
