USE [EAMI-MC]
GO
/****** Object:  StoredProcedure [dbo].[GetHoldReportData]    Script Date: 5/28/2019 10:08:03 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetHoldReportData]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetHoldReportData]
GO
/****** Object:  StoredProcedure [dbo].[GetHoldReportData]    Script Date: 7/21/2020 2:04:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/******************************************************************************
* PROCEDURE:  [dbo].[GetHoldReportData]
* PURPOSE: Gets Hold Report Data
* NOTES:
* CREATED: 8/29/2018  Genady G.
* MODIFIED
* DATE			AUTHOR      DESCRIPTION
*------------------------------------------------------------------------------
* 02/21/2019	Eugene S	changed to return Entity Name from PEEI instead of PEE
* 7/11/2020		Genady G	Refactor to use new tables [TB_PEE_ADDRESS] and [TB_PEE_SYSTEM]
*****************************************************************************/
CREATE PROCEDURE [dbo].[GetHoldReportData]
AS
BEGIN
	BEGIN TRY

	SELECT 
		CONVERT(VARCHAR(10), PST.Status_Date, 101)AS Hold_Date
		,PR.[PaymentSet_Number]
		,PST.Status_Note AS Hold_Notes 
		,PST.CreatedBy AS [User]
		,PEEA.[Entity_Name] AS Vendor_Name
		,PR.Payment_Type AS Model
		,PEE.[Entity_ID] AS Vendor_Number
		,PEXT.ContractNumber AS Contract_Number
		,PR.FiscalYear
		,SUM(PR.Amount) AS Amount
	FROM [dbo].[TB_PAYMENT_RECORD] PR
		INNER JOIN TB_PAYMENT_DN_STATUS PRDN ON PR.Payment_Record_ID = PRDN.Payment_Record_ID
		INNER JOIN TB_PAYMENT_STATUS PST ON PRDN.CurrentHoldStatusID = PST.Payment_Status_ID
		INNER JOIN TB_PEE_ADDRESS(NOLOCK) PEEA ON PEEA.PEE_Address_ID = PR.PEE_Address_ID			
		INNER JOIN TB_PEE_SYSTEM(NOLOCK) PEES ON PEES.PEE_System_ID = PEEA.PEE_System_ID
		INNER JOIN TB_PAYMENT_EXCHANGE_ENTITY(NOLOCK) PEE ON PEE.Payment_Exchange_Entity_ID = PEES.Payment_Exchange_Entity_ID
		INNER JOIN TB_PAYMENT_RECORD_EXT_CAPMAN(NOLOCK) PEXT ON PR.Payment_Record_ID = PEXT.Payment_Record_ID
	GROUP BY 
		CONVERT(VARCHAR(10), PST.Status_Date, 101)	
		,PR.[PaymentSet_Number]
		,PST.Status_Note
		,PST.CreatedBy
		,PEEA.[Entity_Name]	
		,PR.Payment_Type
		,PEE.[Entity_ID] 
		,PEXT.ContractNumber
		,PR.FiscalYear
				
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
