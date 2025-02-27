USE [EAMI-PRX]
GO
/****** Object:  StoredProcedure [dbo].[DeleteClaimScheduleRemitAdviceNote]    Script Date: 5/28/2019 10:08:03 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DeleteClaimScheduleRemitAdviceNote]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[DeleteClaimScheduleRemitAdviceNote]
GO
/****** Object:  StoredProcedure [dbo].[DeleteClaimScheduleRemitAdviceNote]    Script Date: 5/28/2019 10:08:03 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/******************************************************************************
* PROCEDURE:  [dbo].[DeleteClaimScheduleRemitAdviceNote]
* PURPOSE: Delete Claim Schedule Remit Advice Note
* NOTES:
* CREATED: 1/30/2018  Genady G.
* MODIFIED
* DATE     AUTHOR      DESCRIPTION
*------------------------------------------------------------------------------
*****************************************************************************/
CREATE PROCEDURE [dbo].[DeleteClaimScheduleRemitAdviceNote] 
	@Claim_Schedule_ID INT
AS
BEGIN
	BEGIN TRY
		-- delete if record exists
		IF EXISTS(select null from [dbo].[TB_CLAIM_SCHEDULE_REMITTANCE_ADVICE_NOTE] (nolock)
				WHERE [Claim_Schedule_ID] = @Claim_Schedule_ID)
			BEGIN
				DELETE FROM [dbo].[TB_CLAIM_SCHEDULE_REMITTANCE_ADVICE_NOTE] WHERE [Claim_Schedule_ID] = @Claim_Schedule_ID
			END
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
