USE [EAMI-PRX]
GO
/****** Object:  StoredProcedure [dbo].[GetPymtOnHoldNotification]    Script Date: 5/28/2019 10:08:03 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetPymtOnHoldNotification]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetPymtOnHoldNotification]
GO
/****** Object:  StoredProcedure [dbo].[GetPymtOnHoldNotification]    Script Date: 7/16/2020 2:07:04 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/******************************************************************************
* PROCEDURE:  [dbo].[GetPymtOnHoldNotification]
* PURPOSE: Gets payment submission transaction for user notification 
* NOTES:
* CREATED: 01/14/2019  Eugene S.
* MODIFIED
* DATE			AUTHOR			DESCRIPTION
* 5/31/19		Alex Hoang		Added @DaysPassed param and Entity_ContractNumber
* 7/11/2020		Genady G		Refactor to use new tables [TB_PEE_ADDRESS] and [TB_PEE_SYSTEM]
*------------------------------------------------------------------------------
*****************************************************************************/
CREATE PROCEDURE [dbo].[GetPymtOnHoldNotification]
	@DaysPassed int
AS
BEGIN
	BEGIN TRY
					
		DECLARE @paymentSetsOnHold TABLE (
			PaymentSet_Number VARCHAR(30)
			,Status_Note VARCHAR(200)
			,CreatedBy VARCHAR(20)
			,Entity_ContractNumber VARCHAR(20)
			)

		INSERT INTO @paymentSetsOnHold
		SELECT PR.PaymentSet_Number
			,PS.Status_Note
			,PS.CreatedBy
			,PEEA.[ContractNumber]
		FROM TB_PAYMENT_RECORD (NOLOCK) PR
		INNER JOIN TB_PAYMENT_DN_STATUS (NOLOCK) PSDN ON PSDN.Payment_Record_ID = PR.Payment_Record_ID
		INNER JOIN TB_PAYMENT_STATUS (NOLOCK) PS ON PS.Payment_Record_ID = PR.Payment_Record_ID
			AND PS.Payment_Status_ID = PSDN.CurrentHoldStatusID
			
		INNER JOIN TB_PEE_ADDRESS(NOLOCK) PEEA ON PEEA.PEE_Address_ID = PR.PEE_Address_ID	
		WHERE 1 = 1
			AND PS.Status_Date < DATEADD(DAY, -1*@DaysPassed, getdate())
			-- for testing
			--AND PS.Status_Date < DATEADD(DAY, - 1, '2019-01-22')
		GROUP BY PR.PaymentSet_Number
			,PS.Status_Note
			,PS.CreatedBy
			,PEEA.[ContractNumber]

		SELECT PSH.PaymentSet_Number
			,PS.Status_Date
			,PSH.Status_Note
			,PSH.CreatedBy
			,PSH.Entity_ContractNumber
		FROM @paymentSetsOnHold PSH
		INNER JOIN (
			SELECT DISTINCT PRR.PaymentSet_Number
				,MIN(PRR.Payment_Record_ID) AS Payment_Record_ID
			FROM TB_PAYMENT_RECORD PRR
			GROUP BY PRR.PaymentSet_Number
			) PR ON PR.PaymentSet_Number = PSH.PaymentSet_Number
		INNER JOIN TB_PAYMENT_DN_STATUS PSDN ON PSDN.Payment_Record_ID = PR.Payment_Record_ID
		INNER JOIN TB_PAYMENT_STATUS PS ON PS.Payment_Record_ID = PR.Payment_Record_ID
			AND PS.Payment_Status_ID = PSDN.CurrentHoldStatusID
		ORDER BY PS.Status_Date

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
