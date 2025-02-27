USE [EAMI-PRX]
GO
/****** Object:  StoredProcedure [dbo].[GetPaymentRecStatusList]    Script Date: 5/28/2019 10:08:03 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetPaymentRecStatusList]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetPaymentRecStatusList]
GO
/****** Object:  StoredProcedure [dbo].[GetPaymentRecStatusList]    Script Date: 5/28/2019 10:08:03 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/******************************************************************************
* PROCEDURE:  [dbo].[GetPaymentRecStatusList]
* PURPOSE: Gets payment rec status per given list of payment rec number list 
* NOTES:
* CREATED: 11/8/2017  Eugene S.
* MODIFIED
* DATE     AUTHOR      DESCRIPTION
*------------------------------------------------------------------------------
3/23/2023 Gidenko		Add [Payment_Method_Type_ID] field to selection criteria

*****************************************************************************/
CREATE PROCEDURE [dbo].[GetPaymentRecStatusList]
	@PaymentRecNumList VARCHAR(max)
AS
BEGIN
	BEGIN TRY
		
		-- convert list to xml
		DECLARE @xml XML
		SET @xml = N'<root><r>' + replace(@PaymentRecNumList, ',', '</r><r>') + '</r></root>'		

		-- select records using xml
		SELECT PS.[Payment_Status_ID] as [Current_Payment_Status_ID]
			,PR.[Payment_Record_ID]
			,PR.[PaymentRec_Number]
			,PR.[PaymentRec_NumberExt]
			,PR.[PaymentSet_Number]
			,PR.[PaymentSet_NumberExt]
			,PR.[Payment_Method_Type_ID]
			,PST.[Payment_Status_Type_ID] as [Current_Payment_Status_Type_ID]
			,PS.[CreatedBy] as [Current_Status_CreatedBy]
			,PSTE.Payment_Status_Type_Ext_ID
			,PST.[Code]
			,PS.[Status_Date] as [Current_Status_Date]
			,PS.[Status_Note] as [Current_Status_Note]
			,CS.[Claim_Schedule_Number]
			,CS.[Claim_Schedule_Date]
			,W.[Warrant_Number]
			,W.[Warrant_Date]
			,W.[Amount] as WARRANT_AMOUNT
			,IIF(PDS.CurrentHoldStatusID IS NULL, 0, 1) as [Is_On_Hold]
		FROM TB_PAYMENT_DN_STATUS(NOLOCK) PDS
		INNER JOIN TB_PAYMENT_RECORD(NOLOCK) PR ON PR.Payment_Record_ID = PDS.Payment_Record_ID
		INNER JOIN TB_PAYMENT_STATUS(NOLOCK) PS ON PS.Payment_Status_ID = PDS.CurrentPaymentStatusID
		INNER JOIN TB_PAYMENT_STATUS_TYPE(NOLOCK) PST ON PST.Payment_Status_Type_ID = PS.Payment_Status_Type_ID 
		INNER JOIN TB_PAYMENT_STATUS_TYPE_INTERNAL_TO_EXTERNAL (NOLOCK) PSTIE ON PSTIE.Payment_Status_Type_ID = PST.Payment_Status_Type_ID
		INNER JOIN TB_PAYMENT_STATUS_TYPE_EXTERNAL (NOLOCK) PSTE ON PSTE.Payment_Status_Type_Ext_ID = PSTIE.Payment_Status_Type_Ext_ID
		LEFT JOIN TB_PAYMENT_CLAIM_SCHEDULE(NOLOCK) PCS ON PCS.Payment_Record_ID = PDS.Payment_Record_ID
		LEFT JOIN TB_CLAIM_SCHEDULE(NOLOCK) CS ON CS.Claim_Schedule_ID = PCS.Claim_Schedule_ID
		LEFT JOIN TB_CLAIM_SCHEDULE_WARRANT(NOLOCK) CSW ON CSW.Claim_Schedule_ID = CS.Claim_Schedule_ID
		LEFT JOIN TB_WARRANT(NOLOCK) W ON W.Warrant_ID = CSW.Warrant_ID
		WHERE PR.PaymentRec_Number IN (
			SELECT t.value('.', 'varchar(max)') AS [PaymentRec_Number]
			FROM @xml.nodes('//root/r') AS a(t) 
			)

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
