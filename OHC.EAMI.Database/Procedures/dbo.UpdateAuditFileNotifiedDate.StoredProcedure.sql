USE [EAMI-Dental]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UpdateAuditFileNotifiedDate]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[UpdateAuditFileNotifiedDate]
GO


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/******************************************************************************
* PROCEDURE:  [dbo].[InsertAuditFile]
* PURPOSE: Inserts Audit File
* NOTES:
* CREATED: 07/13/2022  Genady G.
* MODIFIED
* DATE     AUTHOR      DESCRIPTION
*------------------------------------------------------------------------------
*****************************************************************************/

CREATE PROCEDURE [dbo].[UpdateAuditFileNotifiedDate]
		@Audit_File_ID int,
    @NotifiedDate DATETIME 
AS

BEGIN
	BEGIN TRY
		UPDATE [dbo].[TB_AUDIT_FILE]
		SET NotifiedDate = @NotifiedDate
		WHERE Audit_File_ID = @Audit_File_ID			
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