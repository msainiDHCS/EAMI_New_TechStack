USE [EAMI-PRX]
GO
/****** Object:  StoredProcedure [dbo].[DeleteDrawdate]    Script Date: 5/28/2019 10:08:03 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DeleteDrawdate]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[DeleteDrawdate]
GO
/****** Object:  StoredProcedure [dbo].[DeleteDrawdate]    Script Date: 5/28/2019 10:08:03 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/******************************************************************************
* PROCEDURE:  [dbo].[DeleteDrawdate]
* PURPOSE: Deletes Drawdate and returns 1 if success
* NOTES:
* CREATED: 11/16/2017  Genady G.
* MODIFIED
* DATE     AUTHOR      DESCRIPTION
*------------------------------------------------------------------------------
*****************************************************************************/
CREATE PROCEDURE [dbo].[DeleteDrawdate] 
	@Drawdate DATE
AS
BEGIN
	BEGIN TRY
		declare @success bit = 0;
		declare @message varchar(100) = null;

		-- delete if found
		if exists(select null from [TB_DRAWDATE_CALENDAR] (nolock) where [Drawdate] = @Drawdate)
		begin
			DELETE FROM [dbo].[TB_DRAWDATE_CALENDAR] WHERE [Drawdate] = @Drawdate
			set @success = 1
		end
		else		
		begin
			set @message = 'Drawdate ' + convert(varchar, @Drawdate, 101) + ' does not exist.'
		end
		
		SELECT @success as [SUCCESS], @message as [MESSAGE];
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
