USE [EAMI-PRX]
GO
/****** Object:  StoredProcedure [dbo].[CheckDuplicateTransaction]    Script Date: 5/28/2019 10:08:03 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CheckDuplicateTransaction]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[CheckDuplicateTransaction]
GO
/****** Object:  StoredProcedure [dbo].[CheckDuplicateTransaction]    Script Date: 5/28/2019 10:08:03 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/******************************************************************************
* PROCEDURE:  [dbo].[CheckDuplicateTransaction]
* PURPOSE: Searches for existing payment submission transaction number
* NOTES:
* CREATED: 10/25/2017  Eugene S.
* MODIFIED
* DATE     AUTHOR      DESCRIPTION
*------------------------------------------------------------------------------
*****************************************************************************/
CREATE PROCEDURE [dbo].[CheckDuplicateTransaction]
	@MsgTransactionId VARCHAR(50)
AS
BEGIN
	BEGIN TRY
		
		SELECT Msg_Transaction_ID
		FROM TB_TRANSACTION
		WHERE Msg_Transaction_ID = @MsgTransactionId
		
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
