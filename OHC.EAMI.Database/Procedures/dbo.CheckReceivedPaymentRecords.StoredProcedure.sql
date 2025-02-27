USE [EAMI-PRX]
GO
/****** Object:  StoredProcedure [dbo].[CheckReceivedPaymentRecords]    Script Date: 5/28/2019 10:08:03 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CheckReceivedPaymentRecords]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[CheckReceivedPaymentRecords]
GO
/****** Object:  StoredProcedure [dbo].[CheckReceivedPaymentRecords]    Script Date: 5/28/2019 10:08:03 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/******************************************************************************
* PROCEDURE:  [dbo].[CheckReceivedPaymentRecords]
* PURPOSE: Gets/checks received payment records by SOR
* NOTES:
* CREATED: 10/27/2020  Eugene S.
* MODIFIED
* DATE			AUTHOR      DESCRIPTION
*------------------------------------------------------------------------------
*****************************************************************************/
CREATE PROCEDURE [dbo].[CheckReceivedPaymentRecords]
	@SOR_ID INT
AS
BEGIN
	BEGIN TRY

		select PR.Payment_Record_ID
		from TB_PAYMENT_RECORD PR
		INNER JOIN TB_TRANSACTION T on T.Transaction_ID = PR.Transaction_ID
		INNER JOIN TB_SYSTEM_OF_RECORD SOR on SOR.SOR_ID = T.SOR_ID
		INNER JOIN TB_PAYMENT_DN_STATUS PDNS on PDNS.Payment_Record_ID = PR.Payment_Record_ID
		INNER JOIN TB_PAYMENT_STATUS PS on PS.Payment_Status_ID = PDNS.CurrentPaymentStatusID
		INNER JOIN TB_PAYMENT_STATUS_TYPE PST on PST.Payment_Status_Type_ID = PS.Payment_Status_Type_ID
		WHERE PST.Code = 'RECEIVED' AND SOR.SOR_ID = @SOR_ID --SOR.Code = 'MEDICAL_RX'		

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