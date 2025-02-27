USE [EAMI-MC]
GO
/****** Object:  StoredProcedure [dbo].[InsertPaydate]    Script Date: 5/28/2019 10:08:03 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertPaydate]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[InsertPaydate]
GO
/****** Object:  StoredProcedure [dbo].[InsertPaydate]    Script Date: 5/28/2019 10:08:03 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/******************************************************************************
* PROCEDURE:  [dbo].[InsertPaydate]
* PURPOSE: Inserts Paydate and returns identity
* NOTES:
* CREATED: 11/16/2017  Genady G.
* MODIFIED
* DATE     AUTHOR      DESCRIPTION
*------------------------------------------------------------------------------
*****************************************************************************/
CREATE PROCEDURE [dbo].[InsertPaydate] 
	@Paydate DATE
	,@Note VARCHAR(50)
	,@IsActive BIT
	,@CreateDate DATETIME
	,@CreatedBy VARCHAR(20)
	,@UpdateDate DATETIME
	,@UpdatedBy VARCHAR(20)
AS
BEGIN
	BEGIN TRY
		-- check for existing pee
		declare @paydate_Calendar_ID INT
		set @paydate_Calendar_ID = (select Paydate_Calendar_ID 
							from TB_PAYDATE_CALENDAR (nolock)
							where [Paydate] = @Paydate )
		
		-- insert if not found
		if @paydate_Calendar_ID is null
		begin
			INSERT INTO [dbo].[TB_PAYDATE_CALENDAR] (
				[Paydate]
				,[Note]
				,[IsActive]	
				,[CreateDate]
				,[CreatedBy]
				,[UpdateDate]
				,[UpdatedBy]
				)
			VALUES (
				@Paydate
				,@Note
				,@IsActive	
				,@CreateDate
				,@CreatedBy
				,@UpdateDate
				,@UpdatedBy
				)
			set @paydate_Calendar_ID = SCOPE_IDENTITY()
		end
		
		SELECT @paydate_Calendar_ID as [SCOPE_IDENTITY];
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
