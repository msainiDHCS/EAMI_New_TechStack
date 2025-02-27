USE [EAMI-MC]
GO
/****** Object:  StoredProcedure [dbo].[InsertTransaction]    Script Date: 5/28/2019 10:08:03 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertTransaction]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[InsertTransaction]
GO
/****** Object:  StoredProcedure [dbo].[InsertTransaction]    Script Date: 5/28/2019 10:08:03 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/******************************************************************************
* PROCEDURE:  [dbo].[InsertTransaction]
* PURPOSE: Inserts Transction and returns identity
* NOTES:
* CREATED: 09/19/2017  Eugene S.
* MODIFIED
* DATE     AUTHOR      DESCRIPTION
*------------------------------------------------------------------------------
*****************************************************************************/
CREATE PROCEDURE [dbo].[InsertTransaction] 
	@TransactionId INT
	,@MsgTransactionId VARCHAR(50)
	,@SorId INT
	,@TransactionTypeId INT
	,@TransactionVersion VARCHAR(5)
AS
BEGIN
	BEGIN TRY

		---- check if transaction is duplicate
		--IF EXISTS(select * from TB_TRANSACTION where [Msg_Transaction_ID] = @MsgTransactionId)
		--begin
		--	RAISERROR('Duplicate Transaction', 16, 1) WITH NOWAIT; 
		--end

		INSERT INTO [dbo].[TB_TRANSACTION] (
			[Transaction_ID]
			,[Msg_Transaction_ID]
			,[SOR_ID]
			,[Transaction_Type_ID]
			,[TransactionVersion]
			,[UsersNotified]
			)
		VALUES (
			@TransactionId
			,@MsgTransactionId
			,@SorId
			,@TransactionTypeId
			,@TransactionVersion
			,0
			)

		SELECT SCOPE_IDENTITY() AS [SCOPE_IDENTITY];
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
