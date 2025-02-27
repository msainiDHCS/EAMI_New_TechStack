USE [EAMI-MC]
GO
/****** Object:  StoredProcedure [dbo].[InsertDrawdate]    Script Date: 5/28/2019 10:08:03 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertDrawdate]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[InsertDrawdate]
GO
/****** Object:  StoredProcedure [dbo].[InsertDrawdate]    Script Date: 5/28/2019 10:08:03 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/******************************************************************************
* PROCEDURE:  [dbo].[InsertDrawdate]
* PURPOSE: Inserts Drawdate and returns identity
* NOTES:
* CREATED: 11/16/2017  Genady G.
* MODIFIED
* DATE     AUTHOR      DESCRIPTION
*------------------------------------------------------------------------------
*****************************************************************************/
CREATE PROCEDURE [dbo].[InsertDrawdate] 
	@Drawdate DATE
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
		declare @Drawdate_Calendar_ID INT
		set @Drawdate_Calendar_ID = (select Drawdate_Calendar_ID 
							from [TB_DRAWDATE_CALENDAR] (nolock)
							where [Drawdate] = @Drawdate )
		
		-- insert if not found
		if @Drawdate_Calendar_ID is null
		begin
			INSERT INTO [dbo].[TB_DRAWDATE_CALENDAR] (
				[Drawdate]
				,[Note]
				,[IsActive]	
				,[CreateDate]
				,[CreatedBy]
				,[UpdateDate]
				,[UpdatedBy]
				)
			VALUES (
				@Drawdate
				,@Note
				,@IsActive	
				,@CreateDate
				,@CreatedBy
				,@UpdateDate
				,@UpdatedBy
				)
			set @Drawdate_Calendar_ID = SCOPE_IDENTITY()
		end
		
		SELECT @Drawdate_Calendar_ID as [SCOPE_IDENTITY];
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
