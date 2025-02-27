USE [EAMI-PRX]
GO
/****** Object:  StoredProcedure [dbo].[DeletePaydate]    Script Date: 5/28/2019 10:08:03 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'DeletePaydate') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[DeletePaydate]
GO
/****** Object:  StoredProcedure [dbo].[DeletePaydate]    Script Date: 5/28/2019 10:08:03 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/******************************************************************************
* PROCEDURE:  [dbo].[DeletePaydate]
* PURPOSE: Deletes Paydate and returns 1 on success
* NOTES:
* CREATED: 11/16/2017  Genady G.
* MODIFIED
* DATE     AUTHOR      DESCRIPTION
*------------------------------------------------------------------------------
*****************************************************************************/
CREATE PROCEDURE [dbo].[DeletePaydate] 
	@Paydate DATE
AS
BEGIN
	BEGIN TRY
		declare @success bit = 0;
		declare @message varchar(100) = null;

		-- delete if found
		if exists(select null from [TB_Paydate_CALENDAR] (nolock) where [Paydate] = @Paydate)
		begin
			 if exists(select null from [TB_PAYMENT_RECORD] (nolock) where CONVERT(date,[Payment_Date]) = @Paydate)
			 begin
				set @message = 'Paydate ' + convert(varchar, @Paydate, 101) + ' cannot be deleted. It is assigned to one or more Payment Record.'
			 end
			else
			begin
				DELETE FROM [dbo].[TB_Paydate_CALENDAR] WHERE [Paydate] = @Paydate
				set @success = 1
			end
		end
		else		
		begin
			set @message = 'Paydate ' + convert(varchar, @Paydate, 101) + ' does not exist.'
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
