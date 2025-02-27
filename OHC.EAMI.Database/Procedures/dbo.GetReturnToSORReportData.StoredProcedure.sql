USE [EAMI-PRX]
GO
/****** Object:  StoredProcedure [dbo].[GetReturnToSORReportData]    Script Date: 5/28/2019 10:08:03 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetReturnToSORReportData]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetReturnToSORReportData]
GO
/****** Object:  StoredProcedure [dbo].[GetReturnToSORReportData]    Script Date: 7/21/2020 2:19:58 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/******************************************************************************
* PROCEDURE:  [dbo].[GetReturnToSORReportData]
* PURPOSE: Gets Hold Report Data
* NOTES:
* CREATED: 9/05/2018  Genady G.
* MODIFIED
* DATE			AUTHOR      DESCRIPTION
* 7/11/2020		Genady G	Refactor to use new tables [TB_PEE_ADDRESS] and [TB_PEE_SYSTEM]
* 11/4/2021		Alex H		Made it so that System is retrieved from TB_SYSTEM_OF_RECORD
*------------------------------------------------------------------------------
*****************************************************************************/
CREATE PROCEDURE [dbo].[GetReturnToSORReportData]
	@DateFrom VARCHAR(100),
	@DateTo VARCHAR(100)
AS
BEGIN
	BEGIN TRY
		
	DECLARE @DateFrom_local datetime = CONVERT(date, @DateFrom)
	DECLARE @DateTo_local datetime = DATEADD(day,1,CONVERT(date, @DateTo))
	DECLARE @PaymentStatusTypeID_RETURNED_TO_SOR int = (SELECT [Payment_Status_Type_ID] from [dbo].[TB_PAYMENT_STATUS_TYPE] WHERE [Code] = 'RETURNED_TO_SOR')

	SELECT 
		CONVERT(VARCHAR(10), PST.Status_Date, 101)AS Date_Returned
		,(select code from TB_SYSTEM_OF_RECORD) as [System]
		,PR.[PaymentSet_Number] as PaymentSet_Number
		,PST.Status_Note AS Returned_Notes 
		,PST.CreatedBy AS [User]
		,PEEA.[Entity_Name] AS Vendor_Name
		,PR.Payment_Type AS Model
		,PEE.[Entity_ID] AS Vendor_Number
		,PEXT.ContractNumber AS Contract_Number
		,PR.FiscalYear
		,SUM(PR.Amount) AS Amount
	FROM [dbo].[TB_PAYMENT_RECORD] PR
		INNER JOIN TB_PAYMENT_DN_STATUS PRDN ON PR.Payment_Record_ID = PRDN.Payment_Record_ID
		INNER JOIN TB_PAYMENT_STATUS PST ON PRDN.CurrentPaymentStatusID = PST.Payment_Status_ID
		INNER JOIN TB_PEE_ADDRESS(NOLOCK) PEEA ON PEEA.PEE_Address_ID = PR.PEE_Address_ID			
		INNER JOIN TB_PEE_SYSTEM(NOLOCK) PEES ON PEES.PEE_System_ID = PEEA.PEE_System_ID
		INNER JOIN TB_PAYMENT_EXCHANGE_ENTITY(NOLOCK) PEE ON PEE.Payment_Exchange_Entity_ID = PEES.Payment_Exchange_Entity_ID
		INNER JOIN TB_PAYMENT_RECORD_EXT_CAPMAN(NOLOCK) PEXT ON PR.Payment_Record_ID = PEXT.Payment_Record_ID
	WHERE 		
		PST.Payment_Status_Type_ID = @PaymentStatusTypeID_RETURNED_TO_SOR
		AND PST.Status_Date between @DateFrom_local AND @DateTo_local
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


