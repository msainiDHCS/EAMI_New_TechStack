USE [EAMI-MC]
GO
/****** Object:  StoredProcedure [dbo].[UpdateClaimScheduleNumber]    Script Date: 5/28/2019 10:08:03 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UpdateClaimScheduleNumber]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[UpdateClaimScheduleNumber]
GO
/****** Object:  StoredProcedure [dbo].[UpdateClaimScheduleNumber]    Script Date: 5/28/2019 10:08:03 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/******************************************************************************
* PROCEDURE:  [dbo].[UpdateClaimScheduleNumber]
* PURPOSE: Updates Claim Schedule Number 
* NOTES:
* CREATED: 11/14/2018  Genady G.
* MODIFIED
* DATE       AUTHOR      DESCRIPTION
*------------------------------------------------------------------------------
* 1/4/2018   Genady     Determine Sequence number based on ZIP CODE ASCENDING
* 2/21/19   Eugene		Changed to directly return Zip code value
* 2/25/19   Genady		Bug fix with assigning Zip code
*****************************************************************************/
CREATE PROCEDURE [dbo].[UpdateClaimScheduleNumber] 
	@ecs_id VARCHAR(max)
AS
BEGIN
	BEGIN TRY	

		--TMP TABLE TO CAPTURE CLAIM SCHEDULE
		CREATE TABLE #cs (
			[ECS_Number] VARCHAR (20)
			,[Claim_Schedule_ID] INT
			,[Zip_Code] INT
		);

		--CAPTURE CLAIM SCHEDULE, CAPTURE AND PARSE ZIPCODE
		INSERT INTO #cs
		SELECT
			ECS.ECS_Number,
			CS.Claim_Schedule_ID,
			CONVERT (INT, LEFT(CAST(REPLACE(REPLACE(PEEA.Zip,'-', ''),' ', '') AS VARCHAR(9))+ '0000',9))
		FROM TB_ECS ECS
			INNER JOIN TB_CLAIM_SCHEDULE_ECS CS_ECS ON ECS.ECS_ID = CS_ECS.ECS_ID
			INNER JOIN TB_CLAIM_SCHEDULE CS ON CS_ECS.Claim_Schedule_ID = CS.Claim_Schedule_ID
			INNER JOIN TB_PEE_ADDRESS(NOLOCK) PEEA ON PEEA.PEE_Address_ID = CS.PEE_Address_ID	
		WHERE
			ECS.ECS_ID = @ecs_id

		--TMP TABLE TO SEQUENCE CS BASED ON ZIP CODE
		CREATE TABLE #cs_sorted (
			[SeqNumber] INT IDENTITY (1, 1)
			,[ECS_Number] VARCHAR (20)
			,[Claim_Schedule_ID] INT
		);
		
		--ORDER RECORDS BASED ON ZIP CODE ASCENDING
		INSERT INTO #cs_sorted
		SELECT
			ECS_Number,
			Claim_Schedule_ID			
		FROM #cs
		ORDER BY
			[Zip_Code] ASC
		
		--UPDATE CS NUMBER AND SEQUENCE NUMBER
		UPDATE CS
		SET 
			CS.Claim_Schedule_Number = TMP_CS.ECS_Number + '-' + RIGHT('000' + CAST(TMP_CS.SeqNumber AS VARCHAR(3)),3)
			,CS.SeqNumber = TMP_CS.SeqNumber
		FROM TB_CLAIM_SCHEDULE CS
			INNER JOIN #cs_sorted TMP_CS ON CS.Claim_Schedule_ID = TMP_CS.Claim_Schedule_ID

		DROP TABLE #cs
		DROP TABLE #cs_sorted

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
