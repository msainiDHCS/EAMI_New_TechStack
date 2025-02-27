USE [EAMI-MC]
GO
/****** Object:  StoredProcedure [dbo].[UpdateTransactionPaymentsStatus]    Script Date: 5/28/2019 10:08:03 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UpdateTransactionPaymentsStatus]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[UpdateTransactionPaymentsStatus]
GO
/****** Object:  StoredProcedure [dbo].[UpdateTransactionPaymentsStatus]    Script Date: 5/28/2019 10:08:03 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/******************************************************************************
* PROCEDURE:  [dbo].[UpdatePaymentStatusPerTransaction]
* PURPOSE: Updates payment statuses for given transaction id
* NOTES:
* CREATED: 11/30/2017  Eugene S.
* MODIFIED
* DATE     AUTHOR      DESCRIPTION
*------------------------------------------------------------------------------
*****************************************************************************/
CREATE PROCEDURE [dbo].[UpdateTransactionPaymentsStatus] 
	@TransactionID INT		
AS
BEGIN
	BEGIN TRY		
		BEGIN TRAN
			SET NOCOUNT ON;
			DECLARE @paymentStatusTypeId INT = (
				SELECT Payment_Status_Type_ID
				FROM TB_PAYMENT_STATUS_TYPE(NOLOCK)
				WHERE Code = 'UNASSIGNED'
				)

			-- insert status into payment status history table
			INSERT INTO [dbo].[TB_PAYMENT_STATUS] (
				[Payment_Record_ID]
				,[Payment_Status_Type_ID]
				,[Status_Date]
				,[Status_Note]
				,[CreatedBy]
				)
			SELECT PR.[Payment_Record_ID]
				,@paymentStatusTypeId
				,getdate()
				,NULL
				,'system'
			FROM TB_PAYMENT_RECORD PR(NOLOCK)
			WHERE PR.Transaction_ID = @TransactionID

			-- update payment DN status table
			UPDATE [TB_PAYMENT_DN_STATUS]
			SET CurrentPaymentStatusID = PS.Payment_Status_ID
				,LatestPaymentStatusID = PS.Payment_Status_ID
			FROM TB_PAYMENT_RECORD PR(NOLOCK)
			INNER JOIN TB_PAYMENT_STATUS PS(NOLOCK) ON PS.Payment_Record_ID = PR.Payment_Record_ID
				AND PS.Payment_Status_Type_ID = @paymentStatusTypeId
			INNER JOIN TB_PAYMENT_DN_STATUS PDS(NOLOCK) ON PDS.Payment_Record_ID = PR.Payment_Record_ID
			WHERE PR.Transaction_ID = @TransactionID


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
