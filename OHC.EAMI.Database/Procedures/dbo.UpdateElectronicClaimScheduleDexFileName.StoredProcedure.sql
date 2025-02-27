USE [EAMI-MC]
GO
/****** Object:  StoredProcedure [dbo].[UpdateElectronicClaimScheduleDexFileName]    Script Date: 5/28/2019 10:08:03 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UpdateElectronicClaimScheduleDexFileName]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[UpdateElectronicClaimScheduleDexFileName]
GO
/****** Object:  StoredProcedure [dbo].[UpdateElectronicClaimScheduleDexFileName]    Script Date: 5/28/2019 10:08:03 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/******************************************************************************
* PROCEDURE:  [dbo].[UpdateElectronicClaimScheduleDexFileName]
* PURPOSE: Updates Electronic Claim Schedule DexFileName by Ecs ID
* NOTES:
* CREATED: 02/03/2019  Genady G.
* MODIFIED
* DATE     AUTHOR      DESCRIPTION
*------------------------------------------------------------------------------
*****************************************************************************/
CREATE PROCEDURE [dbo].[UpdateElectronicClaimScheduleDexFileName] 
	@Ecs_Id int,
	@DexFileName VARCHAR(50)
AS
BEGIN
	BEGIN TRY	

		UPDATE [dbo].[TB_ECS] SET [DexFileName] = @DexFileName
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
