USE [EAMI-PRX]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DeleteClaimScheduleByUser]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[DeleteClaimScheduleByUser]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


/******************************************************************************
* PROCEDURE:  [dbo].[DeleteClaimScheduleByUser]
* PURPOSE: Deletes Claim Schedule records for a given user, changes the Status of the Payment Record to ASSIGNED
* NOTES:
* CREATED: 6/4/2019  Genady G.
* MODIFIED
* DATE		AUTHOR      DESCRIPTION
*------------------------------------------------------------------------------
*****************************************************************************/
CREATE PROCEDURE [dbo].[DeleteClaimScheduleByUser] 
	@userName VARCHAR(20)
AS
BEGIN
	BEGIN TRY	
		
	--Determine UserID
	DECLARE @UserID INT = null
	SET @UserID = (SELECT TOP 1 [User_ID] FROM TB_USER WHERE [User_Name] = @userName)
	
	--CS Status Type
	DECLARE @Assigned_Claim_Schedule_StatusTypeID int = (SELECT [Claim_Schedule_Status_Type_ID] from [dbo].[TB_CLAIM_SCHEDULE_STATUS_TYPE] WHERE [Code] = 'ASSIGNED')

	IF (@UserID IS NOT NULL)
	BEGIN

		---------------------------------------------------
		-- CAPTURE CLAIM SCHEDULES
		---------------------------------------------------
		CREATE TABLE #cs (
			[Claim_Schedule_ID] int
		);

		INSERT INTO #cs
		SELECT 
			CSDS.[Claim_Schedule_ID]
		FROM TB_CLAIM_SCHEDULE_DN_STATUS(NOLOCK) CSDS
			--CURRENT STATUS
			INNER JOIN TB_CLAIM_SCHEDULE_STATUS(NOLOCK) CSS_CURRENT ON CSS_CURRENT.Claim_Schedule_Status_ID = CSDS.CurrentCSStatusID			
			--USER ASSIGNMENT
			INNER JOIN TB_CLAIM_SCHEDULE_USER_ASSIGNMENT(NOLOCK) CSUA ON CSUA.Claim_Schedule_User_Assignment_ID = CSDS.CurrentUserAssignmentID
		WHERE CSUA.[User_ID] = @UserID 
			AND  CSS_CURRENT.Claim_Schedule_Status_Type_ID = @Assigned_Claim_Schedule_StatusTypeID
	
		DECLARE @cs_count int = (SELECT COUNT(*) FROM #cs)

		IF (@cs_count > 0)
		BEGIN
			
			---------------------------------------------------
			-- HANDLE PAYMENT RECORD STATUS
			---------------------------------------------------

			DECLARE @status_datetime_stamp datetime = GetDate()
			DECLARE @status_note varchar (200) = 'Payment Record is released from Claim Schedule via script and is set to Assigned status.'
			DECLARE @Assigned_PaymentStatusTypeID int = (SELECT [Payment_Status_Type_ID] from [dbo].[TB_PAYMENT_STATUS_TYPE] WHERE [Code] = 'ASSIGNED')

			--GET PAYMENT RECORDS
			CREATE TABLE #pr (
				[Payment_Record_ID] int
			);
		
			INSERT INTO #pr
			SELECT PR_CS.[Payment_Record_ID]
			FROM TB_PAYMENT_CLAIM_SCHEDULE(NOLOCK) PR_CS WHERE PR_CS.Claim_Schedule_ID	IN (SELECT [Claim_Schedule_ID]  FROM #cs)
			
			--UPDATE PAYMENT STATUS
			INSERT INTO [dbo].[TB_PAYMENT_STATUS] (
				[Payment_Record_ID]
				,[Payment_Status_Type_ID]
				,[Status_Date]
				,[Status_Note]
				,[CreatedBy]
				)
			SELECT PR.[Payment_Record_ID]
				,@Assigned_PaymentStatusTypeID
				,@status_datetime_stamp
				,@status_note
				,'system'
			FROM #pr PR

			-- update payment DN status table
			UPDATE [TB_PAYMENT_DN_STATUS]
			SET CurrentPaymentStatusID = PS.Payment_Status_ID
				,LatestPaymentStatusID = PS.Payment_Status_ID
			FROM TB_PAYMENT_RECORD PR(NOLOCK)
				INNER JOIN TB_PAYMENT_STATUS PS(NOLOCK) ON PS.Payment_Record_ID = PR.Payment_Record_ID
					AND PS.Payment_Status_Type_ID = @Assigned_PaymentStatusTypeID AND PS.[Status_Date] = @status_datetime_stamp
				INNER JOIN TB_PAYMENT_DN_STATUS PDS(NOLOCK) ON PDS.Payment_Record_ID = PR.Payment_Record_ID
			WHERE PR.[Payment_Record_ID] IN (SELECT [Payment_Record_ID] FROM #pr)
			
			---------------------------------------------------
			-- DELETE CLAIM SCHEDULE
			---------------------------------------------------

			--DELETE HERE
			DELETE FROM [dbo].[TB_CLAIM_SCHEDULE_ECS] WHERE [Claim_Schedule_ID] in (SELECT [Claim_Schedule_ID] from  #cs)
			DELETE FROM [dbo].[TB_CLAIM_SCHEDULE_WARRANT] WHERE [Claim_Schedule_ID] in (SELECT [Claim_Schedule_ID] from  #cs)
			DELETE FROM [dbo].[TB_PAYMENT_CLAIM_SCHEDULE] WHERE [Claim_Schedule_ID] in (SELECT [Claim_Schedule_ID] from  #cs)
			DELETE FROM [dbo].[TB_CLAIM_SCHEDULE_REMITTANCE_ADVICE_NOTE] WHERE [Claim_Schedule_ID] in (SELECT [Claim_Schedule_ID] from  #cs)
	
			--ASSIGNMENT
			DELETE FROM [dbo].[TB_CLAIM_SCHEDULE_USER_ASSIGNMENT] WHERE [Claim_Schedule_ID] in (SELECT [Claim_Schedule_ID] from  #cs)

			--STATUS
			DELETE FROM [dbo].[TB_CLAIM_SCHEDULE_DN_STATUS] WHERE [Claim_Schedule_ID] in (SELECT [Claim_Schedule_ID] from  #cs)
			DELETE FROM [dbo].[TB_CLAIM_SCHEDULE_STATUS] WHERE [Claim_Schedule_ID] in (SELECT [Claim_Schedule_ID] from  #cs)

			--DELETE CLAIM SCHEDULE HERE
			DELETE FROM [dbo].[TB_CLAIM_SCHEDULE] WHERE [Claim_Schedule_ID] in (SELECT [Claim_Schedule_ID] from  #cs)
						
			---------------------------------------------------
			-- SUCCESS - SEND RESULT BACK
			---------------------------------------------------

			SELECT 1 as [IsSuccess], 'Total of ' + Convert(varchar, @cs_count) + ' Claim Schedules were deleted.' as [Message]

			DROP TABLE #pr
		END 
		ELSE
		BEGIN
			SELECT 0 as [IsSuccess], 'No Claim Schedules exist to delete.' as [Message]
		END

		DROP TABLE #cs

	END
	ELSE
	BEGIN
		SELECT 0 as [IsSuccess], 'No such user exist. ' + 'UserName=' + @userName  as [Message]
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
