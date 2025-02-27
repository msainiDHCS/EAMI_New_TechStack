USE [EAMI-PRX]
GO
/****** Object:  StoredProcedure [dbo].[InsertClaimScheduleStatusMultiple]    Script Date: 5/28/2019 10:08:03 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertClaimScheduleStatusMultiple]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[InsertClaimScheduleStatusMultiple]
GO
/****** Object:  StoredProcedure [dbo].[InsertClaimScheduleStatusMultiple]    Script Date: 5/28/2019 10:08:03 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/******************************************************************************
* PROCEDURE:  [dbo].[InsertClaimScheduleStatusMultiple]
* PURPOSE: Updates/Inserts hist status and dn status for multiple claim schedules
* NOTES:
* CREATED: 03/03/2018  Eugene S.
* MODIFIED
* DATE				AUTHOR					DESCRIPTION
*------------------------------------------------------------------------------
*02/05/2019			Genady G				Add @StatusDate parameter
*****************************************************************************/
CREATE PROCEDURE [dbo].[InsertClaimScheduleStatusMultiple] 
	@ClaimScheduleIDList VARCHAR(max),
	@ClaimScheduleStatusTypeId int,
	@PaymentStatusTypeId int = null,
	@StatusNote VARCHAR(200) = null,
	@CreatedBy VARCHAR(20) = 'system',
	@StatusDate DATETIME
AS
BEGIN
	BEGIN TRY		
		BEGIN TRAN
			SET NOCOUNT ON;
			SET XACT_ABORT ON

			DECLARE @xml XML
			SET @xml = N'<root><r>' + replace(@ClaimScheduleIDList, ',', '</r><r>') + '</r></root>'

			--1. update CS hist status			
			INSERT INTO [dbo].[TB_CLAIM_SCHEDULE_STATUS] (
				[Claim_Schedule_ID]
				,[Claim_Schedule_Status_Type_ID]
				,[Status_Date]
				,[Status_Note]
				,[CreatedBy]
				)
			SELECT CS.Claim_Schedule_ID
				,@ClaimScheduleStatusTypeId
				,@StatusDate
				,@StatusNote
				,@CreatedBy
			FROM TB_CLAIM_SCHEDULE CS(NOLOCK)
			WHERE CS.Claim_Schedule_ID IN (
					SELECT t.value('.', 'int') AS [Claim_Schedule_ID]
					FROM @xml.nodes('//root/r') AS a(t)
					)

			--2. update CS DN status
			UPDATE [TB_CLAIM_SCHEDULE_DN_STATUS]
			SET [CurrentCSStatusID] = CST.Claim_Schedule_Status_ID
				,LatestCSStatusID = CST.Claim_Schedule_Status_ID
			FROM TB_CLAIM_SCHEDULE CS(NOLOCK)
			INNER JOIN TB_CLAIM_SCHEDULE_STATUS CST(NOLOCK) ON CST.Claim_Schedule_ID = CS.Claim_Schedule_ID
				AND CST.Claim_Schedule_Status_Type_ID = @ClaimScheduleStatusTypeId
				AND CST.Status_Date = @StatusDate
			INNER JOIN TB_CLAIM_SCHEDULE_DN_STATUS CSDS(NOLOCK) ON CSDS.Claim_Schedule_ID = CS.Claim_Schedule_ID
			WHERE CS.Claim_Schedule_ID IN (
					SELECT t.value('.', 'int') AS [Claim_Schedule_ID]
					FROM @xml.nodes('//root/r') AS a(t)
					)
			

			IF @PaymentStatusTypeId IS NOT NULL
			BEGIN							
				--3. insert status into payment status history table
				INSERT INTO [dbo].[TB_PAYMENT_STATUS] (
					[Payment_Record_ID]
					,[Payment_Status_Type_ID]
					,[Status_Date]
					,[Status_Note]
					,[CreatedBy]
					)
				SELECT PR.[Payment_Record_ID]
					,@PaymentStatusTypeId
					,@StatusDate
					,@StatusNote
					,@CreatedBy
				FROM TB_PAYMENT_RECORD PR(NOLOCK)
				INNER JOIN TB_PAYMENT_CLAIM_SCHEDULE PCS (NOLOCK) ON PCS.Payment_Record_ID = PR.Payment_Record_ID
				WHERE PCS.Claim_Schedule_ID IN (
					SELECT t.value('.', 'int') AS [Claim_Schedule_ID]
					FROM @xml.nodes('//root/r') AS a(t)
					)

				-- 4. update payment DN status table
				UPDATE [TB_PAYMENT_DN_STATUS]
				SET CurrentPaymentStatusID = PS.Payment_Status_ID
					,LatestPaymentStatusID = PS.Payment_Status_ID
				FROM TB_PAYMENT_RECORD PR(NOLOCK)
				INNER JOIN TB_PAYMENT_CLAIM_SCHEDULE PCS (NOLOCK) ON PCS.Payment_Record_ID = PR.Payment_Record_ID
				INNER JOIN TB_PAYMENT_STATUS PS(NOLOCK) ON PS.Payment_Record_ID = PR.Payment_Record_ID
					AND PS.Payment_Status_Type_ID = @paymentStatusTypeId
					AND PS.Status_Date = @StatusDate
				INNER JOIN TB_PAYMENT_DN_STATUS PDS(NOLOCK) ON PDS.Payment_Record_ID = PR.Payment_Record_ID
				WHERE PCS.Claim_Schedule_ID IN (
					SELECT t.value('.', 'int') AS [Claim_Schedule_ID]
					FROM @xml.nodes('//root/r') AS a(t)
					)
			END

			SET NOCOUNT OFF

		COMMIT TRAN
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
