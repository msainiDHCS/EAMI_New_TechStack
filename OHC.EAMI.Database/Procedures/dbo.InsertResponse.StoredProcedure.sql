USE [EAMI-MC]
GO
/****** Object:  StoredProcedure [dbo].[InsertResponse]    Script Date: 5/28/2019 10:08:03 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertResponse]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[InsertResponse]
GO
/****** Object:  StoredProcedure [dbo].[InsertResponse]    Script Date: 5/28/2019 10:08:03 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/******************************************************************************
* PROCEDURE:  [dbo].[InsertResponse]
* PURPOSE: Inserts Response message
* NOTES:
* CREATED: 09/29/2017  Eugene S.
* MODIFIED
* DATE     AUTHOR      DESCRIPTION
*------------------------------------------------------------------------------
*****************************************************************************/
CREATE PROCEDURE [dbo].[InsertResponse]
	@ResponseId int,
	@MsgTransactionId	varchar(50), 
	@MsgTransactionType	varchar(50),
	@ResponseTimeStamp	Datetime,   
	@ResponseStatusTypeId int,		
	@ResponseMsg varchar(200)
AS

begin
	begin try
		INSERT INTO [dbo].[TB_RESPONSE] (
			[Response_ID]
			,[Msg_Transaction_ID]
			,[Msg_Transaction_Type]
			,[Response_TimeStamp]
			,[Response_Status_Type_ID]
			,[Response_Message]
			)
		VALUES (
			@ResponseId
			,@MsgTransactionId
			,@MsgTransactionType
			,@ResponseTimeStamp
			,@ResponseStatusTypeId
			,@ResponseMsg
			)


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
