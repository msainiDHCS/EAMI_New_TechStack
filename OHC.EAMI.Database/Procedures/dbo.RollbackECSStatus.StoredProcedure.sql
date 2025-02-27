USE [EAMI-MC]
GO
/****** Object:  StoredProcedure [dbo].[RollbackECSStatus]    Script Date: 5/28/2019 10:08:03 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RollbackECSStatus]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[RollbackECSStatus]
GO
/****** Object:  StoredProcedure [dbo].[RollbackECSStatus]    Script Date: 7/5/2019 9:51:14 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/******************************************************************************
* PROCEDURE:  [dbo].[RollbackECSStatus]
* PURPOSE: Roll back ECS Status fom APPROVED to PENDING
* NOTES:
* CREATED: 03/09/2018  Eugene S.
* MODIFIED
* DATE           AUTHOR      DESCRIPTION

*------------------------------------------------------------------------------
*****************************************************************************/
CREATE PROCEDURE [dbo].[RollbackECSStatus] 
	 @ecsNumber VARCHAR(20)
	, @ecsStatusTypeId INT
	, @reasonNote VARCHAR(200)
	, @user VARCHAR(20)
	, @statusDate DATETIME
AS
BEGIN
	BEGIN TRY		
	
	DECLARE @ecsId INT

	SELECT @ecsId = ECS.ECS_ID
		,@ecsStatusTypeId = (
			SELECT ECS_Status_Type_ID
			FROM TB_ECS_STATUS_TYPE
			WHERE CODE = 'PENDING'
			)
		,@reasonNote = 'Cant pay these vendors until next month'
		,@user = 'eami_prod_support'
		,@statusDate = getdate()
	FROM TB_ECS ECS
	INNER JOIN TB_ECS_STATUS_TYPE EST ON EST.ECS_Status_Type_ID = ECS.Current_ECS_Status_Type_ID
	WHERE EST.CODE = 'APPROVED'
		AND ECS.ECS_Number = @ecsNumber

	-- select to check pre status change
	SELECT 'PRE STATUS CHANGE'
		,ECS.ECS_ID
		,ECS.ECS_Number
		,ECS.ApproveDate
		,ECS.ApprovedBy
		,EST.CODE
	FROM TB_ECS ECS
	INNER JOIN TB_ECS_STATUS_TYPE EST ON EST.ECS_Status_Type_ID = ECS.Current_ECS_Status_Type_ID
	WHERE EST.CODE = 'APPROVED'
		AND ECS.ECS_ID = @ecsId

	-- execute ecs status change
	EXECUTE UpdateElectronicClaimScheduleStatus @ecsId
		,@ecsStatusTypeId
		,@statusDate
		,@reasonNote
		,@user

	-- select to check post status change
	SELECT 'POST STATUS CHANGE'
		,ECS.ECS_ID
		,ECS.ECS_Number
		,ECS.CurrentStatusDate
		,ECS.CurrentStatusNote
		,EST.CODE
	FROM TB_ECS ECS
	INNER JOIN TB_ECS_STATUS_TYPE EST ON EST.ECS_Status_Type_ID = ECS.Current_ECS_Status_Type_ID
	WHERE EST.CODE = 'PENDING'
		AND ECS.ECS_ID = @ecsId

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
