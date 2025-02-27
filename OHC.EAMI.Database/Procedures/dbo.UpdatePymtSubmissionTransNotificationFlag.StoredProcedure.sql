USE [EAMI-MC]
GO
/****** Object:  StoredProcedure [dbo].[UpdatePymtSubmissionTransNotificationFlag]    Script Date: 5/28/2019 10:08:03 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UpdatePymtSubmissionTransNotificationFlag]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[UpdatePymtSubmissionTransNotificationFlag]
GO
/****** Object:  StoredProcedure [dbo].[UpdatePymtSubmissionTransNotificationFlag]    Script Date: 5/28/2019 10:08:03 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/******************************************************************************
* PROCEDURE:  [dbo].[UpdatePymtSubmissionTransNotificationFlag]
* PURPOSE: Updates UsersNotified bit flag on payment submission transaction
* NOTES:
* CREATED: 10/25/2018  Eugene S.
* MODIFIED
* DATE     AUTHOR      DESCRIPTION
*------------------------------------------------------------------------------
*****************************************************************************/
CREATE PROCEDURE [dbo].[UpdatePymtSubmissionTransNotificationFlag]
	@TransactionIdList VARCHAR(max)
AS
BEGIN
	BEGIN TRY
		
		-- convert list to xml
		DECLARE @xml XML
		SET @xml = N'<root><r>' + replace(@TransactionIdList, ',', '</r><r>') + '</r></root>'

		-- update transactions
		update TB_TRANSACTION 
		SET UsersNotified = 1 
		where Transaction_ID in (
			SELECT t.value('.', 'int') AS [Transaction_ID]
			FROM @xml.nodes('//root/r') AS a(t) 
			)

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
