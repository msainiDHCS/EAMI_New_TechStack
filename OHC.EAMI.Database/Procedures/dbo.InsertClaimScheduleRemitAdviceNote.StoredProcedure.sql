USE [EAMI-PRX]
GO
/****** Object:  StoredProcedure [dbo].[InsertClaimScheduleRemitAdviceNote]    Script Date: 5/28/2019 10:08:03 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertClaimScheduleRemitAdviceNote]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[InsertClaimScheduleRemitAdviceNote]
GO
/****** Object:  StoredProcedure [dbo].[InsertClaimScheduleRemitAdviceNote]    Script Date: 5/28/2019 10:08:03 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/******************************************************************************
* PROCEDURE:  [dbo].[InsertClaimScheduleRemitAdviceNote]
* PURPOSE: Insert Claim Schedule Remit Advice Note
* NOTES:
* CREATED: 1/30/2018  Genady G.
* MODIFIED
* DATE     AUTHOR      DESCRIPTION
*------------------------------------------------------------------------------
*****************************************************************************/
CREATE PROCEDURE [dbo].[InsertClaimScheduleRemitAdviceNote] 
	@Claim_Schedule_ID INT
	,@Note VARCHAR(500)
	,@NoteDate DATETIME
	,@User VARCHAR(20)
AS
BEGIN
	BEGIN TRY
		-- check for existing pee
		declare @claimScheduleID INT 
		set @claimScheduleID = (select Claim_Schedule_ID 
							from [dbo].[TB_CLAIM_SCHEDULE_REMITTANCE_ADVICE_NOTE] (nolock)
							where Claim_Schedule_ID = @Claim_Schedule_ID )
		
		-- insert if not found
		if @claimScheduleID is null
		BEGIN
			INSERT INTO [dbo].[TB_CLAIM_SCHEDULE_REMITTANCE_ADVICE_NOTE] (
				[Claim_Schedule_ID]
				,[Note]
				,[CreateDate]
				,[CreatedBy]
				,[UpdateDate]
				,[UpdatedBy]
				)
			VALUES (
				@Claim_Schedule_ID
				,@Note
				,@NoteDate
				,@User
				,NULL
				,NULL
				)
			set @claimScheduleID = SCOPE_IDENTITY()
		END
		ELSE
		BEGIN
			UPDATE [dbo].[TB_CLAIM_SCHEDULE_REMITTANCE_ADVICE_NOTE]
			SET 
				[Note] = @Note				
				,[UpdateDate] = @NoteDate
				,[UpdatedBy] = @User
			WHERE [Claim_Schedule_ID] = @claimScheduleID
		END
		
		SELECT @claimScheduleID as [SCOPE_IDENTITY];
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
