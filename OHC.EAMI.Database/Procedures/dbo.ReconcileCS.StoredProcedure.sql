USE [EAMI-MC]
GO
/****** Object:  StoredProcedure [dbo].[ReconcileCS]    Script Date: 5/28/2019 10:08:03 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ReconcileCS]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[ReconcileCS]
GO
/****** Object:  StoredProcedure [dbo].[ReconcileCS]    Script Date: 5/28/2019 10:08:03 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/******************************************************************************
* PROCEDURE:  [dbo].[ReconcileCS]
* PURPOSE: Reconcile CS
* NOTES:
* CREATED: 11/30/2018  Genady G.
* MODIFIED
* DATE           AUTHOR      DESCRIPTION
*------------------------------------------------------------------------------
*****************************************************************************/
CREATE PROCEDURE [dbo].[ReconcileCS] 
	@ECS_ID int
	,@SeqNumber int
	,@DexFileReceiveDate DATETIME
	,@WarrantIssueDate DATETIME
	,@WarrantNumber varchar(20) 
	,@WarrantAmount money 
AS
BEGIN
	BEGIN TRY	
		
		DECLARE @validation_Message VARCHAR (200)
		
		--GET CS_ID by ECS_ID and @SeqNumber
		DECLARE @CS_ID  INT = (SELECT 
				CS_ECS.Claim_Schedule_ID
			FROM TB_CLAIM_SCHEDULE CS 
				INNER JOIN TB_CLAIM_SCHEDULE_ECS CS_ECS on CS.Claim_Schedule_ID = CS_ECS.Claim_Schedule_ID
			WHERE CS_ECS.ECS_ID = @ECS_ID AND CS.SeqNumber = @SeqNumber)
				
		DECLARE @Warrant_date_short VARCHAR (6) = format(@WarrantIssueDate,'yyMMdd')
					
		IF @CS_ID IS NULL
		BEGIN
			SET @validation_Message = 'Failed SCO DEX File Validation. Cannot determine Claim Schedule by Sequence Number in DEX file. ECS_ID=' + Convert(varchar, @ECS_ID) + '; SeqNumber=' + Convert(varchar, @SeqNumber)
			SET @CS_ID = NULL
		END
		ELSE IF EXISTS(SELECT NULL FROM TB_WARRANT WR
							INNER JOIN TB_CLAIM_SCHEDULE_WARRANT CS_WR on WR.Warrant_ID = CS_WR.Warrant_ID
							WHERE CS_WR.Claim_Schedule_ID = @CS_ID AND WR.Warrant_Date_Short = @warrant_date_short)
		BEGIN
			SET @validation_Message = 'Failed SCO DEX File Validation. Claim Schedule already has warrant assigned. ECS_ID=' + Convert(varchar, @ECS_ID) + '; CS_ID=' + Convert(varchar, @CS_ID)
			SET @CS_ID = NULL
		END
		ELSE IF EXISTS(SELECT NULL FROM TB_WARRANT WR
							WHERE WR.Warrant_Number = @WarrantNumber AND WR.Warrant_Date_Short = @Warrant_date_short)
		BEGIN
			SET @validation_Message = 'Failed SCO DEX File Validation. Warrant Number already exists in EAMI. Warrant_Number=' + @WarrantNumber 
			SET @CS_ID = NULL
		END
		ELSE
		BEGIN 
			DECLARE @cs_amount money = (SELECT Amount from TB_CLAIM_SCHEDULE WHERE Claim_Schedule_ID = @CS_ID)
		
			IF @WarrantAmount = @cs_amount
			BEGIN
				--IF MATCH IS FOUND, INSERT WARRANT RECORD
				INSERT INTO [dbo].[TB_WARRANT] (
					[Warrant_Number]
					,[Amount]
					,[Warrant_Date]
					,[Date_Info_Received]
					,[Warrant_Date_Short]
					)
				VALUES (
					@WarrantNumber
					,@WarrantAmount
					,@WarrantIssueDate
					,@DexFileReceiveDate
					,@Warrant_date_short
					)
			
				--GET WARRANT ID		
				DECLARE @Warrant_Id INT
				SET @Warrant_Id = SCOPE_IDENTITY()
		
				INSERT INTO [dbo].[TB_CLAIM_SCHEDULE_WARRANT] VALUES (@CS_ID, @Warrant_Id)
			END
			ELSE
			BEGIN
				SET @validation_Message = 'Failed SCO DEX File Validation. Warrant Amount does not match Claim Schedule Amount.' + Convert(varchar, @cs_amount) + '; ' + Convert(varchar, @WarrantAmount)
				SET @CS_ID = NULL
			END
		END
		
		SELECT @CS_ID AS [CS_ID], @validation_Message AS [Message]

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
