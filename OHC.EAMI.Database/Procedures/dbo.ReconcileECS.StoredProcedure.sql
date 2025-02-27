USE [EAMI-MC]
GO
/****** Object:  StoredProcedure [dbo].[ReconcileECS]    Script Date: 5/28/2019 10:08:03 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ReconcileECS]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[ReconcileECS]
GO
/****** Object:  StoredProcedure [dbo].[ReconcileECS]    Script Date: 5/28/2019 10:08:03 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/******************************************************************************
* PROCEDURE:  [dbo].[ReconcileECS]
* PURPOSE: Reconcile ECS
* NOTES:
* CREATED: 11/30/2018  Genady G.
* MODIFIED
* DATE           AUTHOR      DESCRIPTION
*------------------------------------------------------------------------------
* 8/15/2019      Genady      Bug Fix (line 34) Default @validation_Message variable to empty string
*
*****************************************************************************/
CREATE PROCEDURE [dbo].[ReconcileECS] 
	@ECS_Number VARCHAR(20)	
	,@DexFileName VARCHAR(50)
	,@DexFileReceiveDate DATETIME
	,@WarrantReceivedTaskNumber VARCHAR(30) 
	,@SeqNumberList VARCHAR(max)
AS
BEGIN
	BEGIN TRY		
		
		DECLARE @ECSStatusTypeID_SENT_TO_SCO int = (SELECT [ECS_Status_Type_ID] from [dbo].[TB_ECS_STATUS_TYPE] WHERE [Code] = 'SENT_TO_SCO')
		DECLARE @validation_Message VARCHAR (100) = ''
		DECLARE @ECS_ID INT = 0

		-- convert list to xml
		DECLARE @xml XML
		SET @xml = N'<root><r>' + replace(@SeqNumberList, ',', '</r><r>') + '</r></root>'
		
		--SEQ NUM LIST FROM PARAM
		CREATE TABLE #seqNumberList ([SeqNumber] int);
		INSERT INTO #seqNumberList
		SELECT t.value('.', 'int') FROM @xml.nodes('//root/r') AS a(t)

		--TEMP TABLE : CS ID LIST BY SEQ #
		CREATE TABLE #csIDList_By_SeqNumber (
			[cs_ID] int
		);
		
		--TEMP TABLE : CS ID LIST BY ECS_ID #
		CREATE TABLE #csIDList_By_ECS_ID (
			[cs_ID] int
		);

		--TEMP TABLE : ECS  
		CREATE TABLE #ecs (
			[ecs_ID] int,
			[Current_ECS_Status_Type_ID] int
		);

		--FIND ECS RECORD by ECS_NUMBER
		INSERT INTO #ecs
		SELECT [ECS_ID], [Current_ECS_Status_Type_ID]  
		FROM TB_ECS 
		WHERE ECS_Number = @ECS_Number
		
		--VALIDATE
		IF (NOT EXISTS(SELECT NULL FROM #ecs))
		BEGIN
			SET @validation_Message = 'Failed SCO DEX File Validation. Cannot find ECS with DEX ECS_Number.'
		END
		ELSE IF (SELECT COUNT(*) FROM #ecs) > 1
		BEGIN
			SET @validation_Message = 'Failed SCO DEX File Validation. Multiple ECS records found with DEX ECS_Number.'
		END
		ELSE IF (NOT EXISTS(SELECT NULL FROM #ecs WHERE [Current_ECS_Status_Type_ID] = @ECSStatusTypeID_SENT_TO_SCO))
		BEGIN
			SET @validation_Message = 'Failed SCO DEX File Validation. ECS is not in SENT_TO_SCO status.'
		END
		ELSE
		BEGIN 
			--VALIDATE CS RECORD COUNTS
			SET @ECS_ID = (SELECT [ecs_ID] FROM #ecs)

			--GET CS_ID's by SEQ #
			INSERT INTO #csIDList_By_SeqNumber 
			SELECT CS.Claim_Schedule_ID 
			FROM TB_CLAIM_SCHEDULE CS 
				INNER JOIN TB_CLAIM_SCHEDULE_ECS CS_ECS on CS.Claim_Schedule_ID = CS_ECS.Claim_Schedule_ID
			WHERE CS_ECS.ECS_ID = @ECS_ID AND CS.SeqNumber in (SELECT [SeqNumber] FROM #seqNumberList)
			
			--GET CS_ID's by ECS_ID
			INSERT INTO #csIDList_By_ECS_ID 
			SELECT CS.Claim_Schedule_ID 
			FROM TB_CLAIM_SCHEDULE CS 
				INNER JOIN TB_CLAIM_SCHEDULE_ECS CS_ECS on CS.Claim_Schedule_ID = CS_ECS.Claim_Schedule_ID
			WHERE CS_ECS.ECS_ID = @ECS_ID 

			--GET CS RECORD COUNTS
			DECLARE @seqCount INT = (SELECT COUNT(*) FROM #seqNumberList )
			DECLARE @csCount_By_SeqNumber INT = (SELECT COUNT(*) FROM #csIDList_By_SeqNumber )
			DECLARE @csCount_By_ECS_ID INT = (SELECT COUNT(*) FROM #csIDList_By_ECS_ID )
			
			--VALIDATE CS RECORD COUNTS
			IF (@seqCount <> @csCount_By_SeqNumber OR @seqCount <> @csCount_By_ECS_ID)
			BEGIN
				SET @validation_Message = 'Failed SCO DEX File Validation. DEX Warant and ECS Claim Schedule counts do not match.'
				DELETE FROM #csIDList_By_ECS_ID
			END
		END
			
		--IF PASSED VALIDATION, UPDATE ECS
		IF @ECS_ID > 0 AND LEN(@validation_Message) = 0
		BEGIN
			--UPDATE ECS
			UPDATE TB_ECS SET 
				WarrantReceivedTaskNumber = @WarrantReceivedTaskNumber
				,WarrantReceivedDate = @DexFileReceiveDate
				,DexFileName = @DexFileName
			WHERE ECS_ID = @ECS_ID
		END

		--RETURN ECS_ID AND MESSAGE
		SELECT @ECS_ID AS [ECS_ID], @validation_Message as [Message]
		
		--RETURN LIST CS_IDs
		SELECT [cs_ID] as [CS_ID] FROM #csIDList_By_ECS_ID

		--DROP TEMP TABLES
		DROP TABLE #seqNumberList
		DROP TABLE #csIDList_By_SeqNumber
		DROP TABLE #csIDList_By_ECS_ID
		DROP TABLE #ecs

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
