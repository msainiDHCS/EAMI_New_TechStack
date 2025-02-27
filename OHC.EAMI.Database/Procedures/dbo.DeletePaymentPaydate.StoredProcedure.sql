USE [EAMI-MC]
GO
/****** Object:  StoredProcedure [dbo].[DeletePaymentPaydate]    Script Date: 5/28/2019 10:08:03 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DeletePaymentPaydate]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[DeletePaymentPaydate]
GO
/****** Object:  StoredProcedure [dbo].[DeletePaymentPaydate]    Script Date: 5/28/2019 10:08:03 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/******************************************************************************
* PROCEDURE:  [dbo].[DeletePaymentPaydate]
* PURPOSE: Delete Payment Paydate 
* NOTES:
* CREATED: 12/6/2017  Genady G.
* MODIFIED
* DATE     AUTHOR      DESCRIPTION
*------------------------------------------------------------------------------
*****************************************************************************/
CREATE PROCEDURE [dbo].[DeletePaymentPaydate]
	@PaymentRecordIDList VARCHAR(max)	
AS
BEGIN
	BEGIN TRY

	-- convert list to xml
	DECLARE @xml XML
	SET @xml = N'<root><r>' + replace(@PaymentRecordIDList, ',', '</r><r>') + '</r></root>'

	DELETE FROM [dbo].[TB_PAYMENT_PAYDATE_CALENDAR] WHERE [Payment_Record_ID] IN ( 
			SELECT t.value('.', 'int') 
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
