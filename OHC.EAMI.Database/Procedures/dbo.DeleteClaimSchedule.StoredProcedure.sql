USE [EAMI-PRX]
GO
/****** Object:  StoredProcedure [dbo].[DeleteClaimSchedule]    Script Date: 5/28/2019 10:08:03 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DeleteClaimSchedule]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[DeleteClaimSchedule]
GO
/****** Object:  StoredProcedure [dbo].[DeleteClaimSchedule]    Script Date: 5/28/2019 10:08:03 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/******************************************************************************
* PROCEDURE:  [dbo].[DeleteClaimSchedule]
* PURPOSE: Deletes Claim Schedule record and records within its relationship
* NOTES:
* CREATED: 1/18/2018  Genady G.
* MODIFIED
* DATE		AUTHOR      DESCRIPTION
  5/30/2018	ggidenko	Introduce ability to Delete a Claim Schedule list 
  8/24/2018	ggidenko	Include TB_CLAIM_SCHEDULE_REMITTANCE_ADVICE_NOTE in delete
*------------------------------------------------------------------------------
*****************************************************************************/
CREATE PROCEDURE [dbo].[DeleteClaimSchedule] 
	@claim_Schedule_ID_List VARCHAR(max)
AS
BEGIN
	BEGIN TRY	
	
	-- convert list to xml
	DECLARE @xml XML
	SET @xml = N'<root><r>' + replace(@claim_Schedule_ID_List, ',', '</r><r>') + '</r></root>'
		
	--PARSE provided Claim Schedule IDs into temp table
	CREATE TABLE #csID ([Claim_Schedule_ID] int);
	INSERT INTO #csID
	SELECT t.value('.', 'int') FROM @xml.nodes('//root/r') AS a(t)
	
	DELETE FROM [dbo].[TB_CLAIM_SCHEDULE_ECS] WHERE [Claim_Schedule_ID] in (SELECT [Claim_Schedule_ID] from  #csID)
	DELETE FROM [dbo].[TB_CLAIM_SCHEDULE_WARRANT] WHERE [Claim_Schedule_ID] in (SELECT [Claim_Schedule_ID] from  #csID)
	DELETE FROM [dbo].[TB_PAYMENT_CLAIM_SCHEDULE] WHERE [Claim_Schedule_ID] in (SELECT [Claim_Schedule_ID] from  #csID)
	DELETE FROM [dbo].[TB_CLAIM_SCHEDULE_REMITTANCE_ADVICE_NOTE] WHERE [Claim_Schedule_ID] in (SELECT [Claim_Schedule_ID] from  #csID)
	
	--ASSIGNMENT
	DELETE FROM [dbo].[TB_CLAIM_SCHEDULE_USER_ASSIGNMENT] WHERE [Claim_Schedule_ID] in (SELECT [Claim_Schedule_ID] from  #csID)

	--STATUS
	DELETE FROM [dbo].[TB_CLAIM_SCHEDULE_DN_STATUS] WHERE [Claim_Schedule_ID] in (SELECT [Claim_Schedule_ID] from  #csID)
	DELETE FROM [dbo].[TB_CLAIM_SCHEDULE_STATUS] WHERE [Claim_Schedule_ID] in (SELECT [Claim_Schedule_ID] from  #csID)

	--DELETE CLAIM SCHEDULE HERE
	DELETE FROM [dbo].[TB_CLAIM_SCHEDULE] WHERE [Claim_Schedule_ID] in (SELECT [Claim_Schedule_ID] from  #csID)

	DROP TABLE #csID

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
