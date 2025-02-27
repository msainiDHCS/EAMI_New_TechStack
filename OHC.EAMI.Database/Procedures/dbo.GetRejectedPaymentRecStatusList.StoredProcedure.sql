USE [EAMI-PRX]
GO
/****** Object:  StoredProcedure [dbo].[GetRejectedPaymentRecStatusList]    Script Date: 5/28/2019 10:08:03 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetRejectedPaymentRecStatusList]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetRejectedPaymentRecStatusList]
GO
/****** Object:  StoredProcedure [dbo].[GetRejectedPaymentRecStatusList]    Script Date: 5/28/2019 10:08:03 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/******************************************************************************
* PROCEDURE:  [dbo].[GetRejectedPaymentRecStatusList]
* PURPOSE: Gets rejected payment rec status per given date range 
* NOTES:
* CREATED: 11/8/2017  Eugene S.
* MODIFIED
* DATE     AUTHOR      DESCRIPTION
*------------------------------------------------------------------------------
*****************************************************************************/
CREATE PROCEDURE [dbo].[GetRejectedPaymentRecStatusList]
	@rejectedDateFrom datetime,
	@rejectedDateTo datetime
AS
BEGIN
	BEGIN TRY		

		-- select records using xml
		SELECT 
			PS.Payment_Status_ID as [Current_Payment_Status_ID]
			,PS.Payment_Status_Type_ID as [Current_Payment_Status_Type_ID]
			,PS.Status_Date as [Current_Status_Date]
			,PS.Status_Note as [Current_Status_Note]
			,PS.CreatedBy as [Current_Status_CreatedBy]	
			,PR.[Payment_Record_ID]
			,PR.[PaymentRec_Number]
			,PR.[PaymentRec_NumberExt]
			,PR.[PaymentSet_Number]
			,PR.[PaymentSet_NumberExt]
			,PSTE.[Payment_Status_Type_Ext_ID]
		FROM TB_PAYMENT_DN_STATUS(NOLOCK) PDS
		INNER JOIN TB_PAYMENT_RECORD(NOLOCK) PR ON PR.Payment_Record_ID = PDS.Payment_Record_ID
		INNER JOIN TB_PAYMENT_STATUS(NOLOCK) PS ON PS.Payment_Status_ID = PDS.CurrentPaymentStatusID
		INNER JOIN TB_PAYMENT_STATUS_TYPE(NOLOCK) PST ON PST.Payment_Status_Type_ID = PS.Payment_Status_Type_ID
		INNER JOIN TB_PAYMENT_STATUS_TYPE_INTERNAL_TO_EXTERNAL (NOLOCK) PSTIE on PSTIE.Payment_Status_Type_ID = PST.Payment_Status_Type_ID
		INNER JOIN TB_PAYMENT_STATUS_TYPE_EXTERNAL (NOLOCK) PSTE on PSTE.Payment_Status_Type_Ext_ID = PSTIE.Payment_Status_Type_Ext_ID		
		WHERE 1=1
			AND PST.Code = 'RETURNED_TO_SOR' 
			AND PS.Status_Date >= @rejectedDateFrom 
			AND PS.Status_Date <= @rejectedDateTo			

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
