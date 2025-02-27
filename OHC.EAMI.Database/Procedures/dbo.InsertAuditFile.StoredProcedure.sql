USE [EAMI-Dental]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertAuditFile]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[InsertAuditFile]
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
CREATE PROCEDURE [dbo].[InsertAuditFile]
	
                @fileName VARCHAR(50),
                @fileSize NUMERIC (18,0),
                @payDate DATETIME,
                @taskNumber VARCHAR(30),
                @hasError BIT,
                @createDate DATETIME,
                @uploadDate DATETIME null
AS

BEGIN
	BEGIN TRY
		INSERT INTO [dbo].[TB_AUDIT_FILE]
			([Audit_File_Name]
			,[Audit_File_Size]
			,[PayDate]
			,[TaskNumber]
			,[HasError]
			,[CreateDate]
			,[UploadDate]
			,[NotifiedDate])
		VALUES
			(@fileName
			,@fileSize
			,@payDate
			,@taskNumber
			,@hasError
			,@createDate
			,@uploadDate
			,null)
		
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