USE [EAMI-Dental]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetAuditFilesForNotification]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetAuditFilesForNotification]
GO


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/******************************************************************************
* PROCEDURE:  [dbo].[GetAuditFilesForNotification]
* PURPOSE: Gets list of Audit File records where Notified Date is null
* NOTES:
* CREATED: 07/13/2022  Genady G.
* MODIFIED
* DATE     AUTHOR      DESCRIPTION
*------------------------------------------------------------------------------
*****************************************************************************/
CREATE PROCEDURE [dbo].[GetAuditFilesForNotification]  
AS

BEGIN
	BEGIN TRY	
	
		SELECT 
			Audit_File_ID 
			, Audit_File_Name
			, Audit_File_Size
			, PayDate
			, UploadDate
		FROM
			[dbo].[TB_AUDIT_FILE]
		WHERE
			HasError = 0
			AND UploadDate IS NOT NULL
			AND NotifiedDate IS NULL

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