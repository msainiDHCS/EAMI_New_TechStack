USE [EAMI-MC]
GO
/****** Object:  StoredProcedure [dbo].[InsertRequest]    Script Date: 5/28/2019 10:08:03 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertRequest]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[InsertRequest]
GO
/****** Object:  StoredProcedure [dbo].[InsertRequest]    Script Date: 5/28/2019 10:08:03 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/******************************************************************************
* PROCEDURE:  [dbo].[InsertRequest]
* PURPOSE: Inserts Request message and resturns primary key
* NOTES:
* CREATED: 09/19/2017  Eugene S.
* MODIFIED
* DATE     AUTHOR      DESCRIPTION
*------------------------------------------------------------------------------
*****************************************************************************/
CREATE PROCEDURE [dbo].[InsertRequest]
	@RequestTimeStamp		Datetime,   
	@MsgTimeStamp			Datetime,
	@MsgSenderId			varchar(20),
	@MsgTransactionId		varchar(50), 
	@MsgTransactionType	varchar(50),
	@MsgTransactionVersion	varchar(5)
AS

begin
	begin try
		INSERT INTO [dbo].[TB_REQUEST]
			([Request_TimeStamp]
			,[Msg_TimeStamp]
			,[Msg_Sender_ID]
			,[Msg_Transaction_ID]
			,[Msg_Transaction_Type]
			,[Msg_Transaction_Version])
		VALUES
			(@RequestTimeStamp
			,@MsgTimeStamp
			,@MsgSenderId
			,@MsgTransactionId
			,@MsgTransactionType
			,@MsgTransactionVersion)

		SELECT SCOPE_IDENTITY() AS [SCOPE_IDENTITY];

	end try
	begin catch
		declare @ErrorMessage nvarchar(4000)
		declare @ErrorSeverity int
		declare @ErrorState int
		
		set @ErrorMessage = error_message()
		set @ErrorSeverity = error_severity()
        set @ErrorState = error_state()

		RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState)
	end catch			
end
GO
