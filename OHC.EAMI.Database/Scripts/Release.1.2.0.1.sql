--EAMI SCRIPT Release.1.2.0.1 

/******************************************************************************
* PURPOSE: Script modifies schema AND executes data converssion
* NOTES:   Release.1.2.0.1
	There are 3 parts of scripts that are interdependant.
		1. EAMI DB SCHEMA CHANGES

* CREATED: 12/20/2021  Genady G.
* MODIFIED
* DATE     AUTHOR      DESCRIPTION
*------------------------------------------------------------------------------
*****************************************************************************/

USE [EAMI-RX]
GO

BEGIN TRY

SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON

/******************************************************
	1. EAMI DB SCHEMA CHANGES
*******************************************************/

--ADD FIELD [Warrant_Date_Short] to [TB_WARRANT]
IF NOT EXISTS(SELECT NULL FROM sys.columns 
          WHERE Name = N'Warrant_Date_Short'
          AND Object_ID = Object_ID(N'TB_WARRANT'))
BEGIN
	ALTER TABLE [dbo].[TB_WARRANT] ADD [Warrant_Date_Short] VARCHAR(6) NOT NULL; 

END

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