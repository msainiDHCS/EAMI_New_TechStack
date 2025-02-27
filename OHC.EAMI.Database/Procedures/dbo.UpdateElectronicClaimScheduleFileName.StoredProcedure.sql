USE [EAMI-MC]
GO
/****** Object:  StoredProcedure [dbo].[UpdateElectronicClaimScheduleFileName]    Script Date: 5/28/2019 10:08:03 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UpdateElectronicClaimScheduleFileName]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[UpdateElectronicClaimScheduleFileName]
GO
/****** Object:  StoredProcedure [dbo].[UpdateElectronicClaimScheduleFileName]    Script Date: 5/28/2019 10:08:03 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/******************************************************************************
* PROCEDURE:  [dbo].[UpdateElectronicClaimScheduleFileName]
* PURPOSE: Updates Electronic Claim Schedule FileName by Ecs ID
* NOTES:
* CREATED: 08/03/2018  Genady G.
* MODIFIED
* DATE			AUTHOR				DESCRIPTION
* 03/05/2019	Genady G.			Update SCO_File_Line_Count along with the file name
*------------------------------------------------------------------------------
*****************************************************************************/
CREATE PROCEDURE [dbo].[UpdateElectronicClaimScheduleFileName] 
	@Ecs_Id int,
	@FileName VARCHAR(50),
	@FileLineCount int
AS
BEGIN
	BEGIN TRY	

		UPDATE [dbo].[TB_ECS] 
		SET 
			[ECS_File_Name] = @FileName,
			[SCO_File_Line_Count] = @FileLineCount
		WHERE [ECS_ID] = @Ecs_Id

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
