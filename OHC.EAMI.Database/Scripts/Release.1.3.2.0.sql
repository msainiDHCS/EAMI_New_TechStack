--EAMI SCRIPT Release.1.3.2.0 

/******************************************************************************
* PURPOSE: Script modifies schema
* NOTES:   Release.1.3.2.0
	
		1. EAMI DB SCHEMA CHANGES - add new table [TB_AUDIT_FILE]


* CREATED: 8/04/2022  Genady G.
* MODIFIED
* DATE     AUTHOR      DESCRIPTION
*------------------------------------------------------------------------------
*****************************************************************************/

USE [EAMI-Dental]
GO

BEGIN TRY

		SET ANSI_NULLS ON
		GO

		SET QUOTED_IDENTIFIER ON
		GO

		CREATE TABLE [dbo].[TB_AUDIT_FILE](
			[Audit_File_ID] [int] IDENTITY(1,1) NOT NULL,
			[Audit_File_Name] [varchar](50) NOT NULL,
			[Audit_File_Size] [numeric](18, 0) NOT NULL,
			[PayDate] [datetime] NOT NULL,
			[TaskNumber] [varchar](30) NULL,
			[HasError] [bit] NOT NULL,
			[CreateDate] [datetime] NOT NULL,
			[UploadDate] [datetime] NULL,
			[NotifiedDate] [datetime] NULL,
		 CONSTRAINT [PK_TB_AUDIT_FILE] PRIMARY KEY CLUSTERED 
		(
			[Audit_File_ID] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
		) ON [PRIMARY]
		GO

		ALTER TABLE [dbo].[TB_AUDIT_FILE] ADD  CONSTRAINT [DF_TB_AUDIT_FILE_Audit_File_Size]  DEFAULT ((0)) FOR [Audit_File_Size]
		GO

		ALTER TABLE [dbo].[TB_AUDIT_FILE] ADD  CONSTRAINT [DF_TB_AUDIT_FILE_HasError]  DEFAULT ((0)) FOR [HasError]
		GO


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

