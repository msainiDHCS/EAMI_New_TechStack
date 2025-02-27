USE [EAMI-PRX]
GO
/****** Object:  StoredProcedure [dbo].[GetPaymentRecordIDsByPaymentSetNumbers]    Script Date: 5/28/2019 10:08:03 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetPaymentRecordIDsByPaymentSetNumbers]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetPaymentRecordIDsByPaymentSetNumbers]
GO
/****** Object:  StoredProcedure [dbo].[GetPaymentRecordIDsByPaymentSetNumbers]    Script Date: 5/28/2019 10:08:03 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/******************************************************************************
* PROCEDURE:  [dbo].[GetPaymentRecordIDsByPaymentSetNumbers]
* PURPOSE: Gets Payment Record IDs by PaymentSetNumber values
* NOTES:
* CREATED: 1/5/2018  Genady G.
* MODIFIED
* DATE     AUTHOR      DESCRIPTION
*------------------------------------------------------------------------------
*****************************************************************************/
CREATE PROCEDURE [dbo].[GetPaymentRecordIDsByPaymentSetNumbers]
	@PaymentSetNumberList VARCHAR(max)	
AS
BEGIN
	BEGIN TRY
			
		-- convert list to xml
		DECLARE @xml XML
		SET @xml = N'<root><r>' + replace(@PaymentSetNumberList, ',', '</r><r>') + '</r></root>'

		SELECT [Payment_Record_ID] FROM dbo.[TB_PAYMENT_RECORD] (NOLOCK) 
		WHERE [PaymentSet_Number]  IN (SELECT t.value('.', 'VARCHAR(15)') FROM @xml.nodes('//root/r') AS a(t))	

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
