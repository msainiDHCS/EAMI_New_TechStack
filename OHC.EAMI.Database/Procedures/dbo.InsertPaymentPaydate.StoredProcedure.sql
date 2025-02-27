USE [EAMI-MC]
GO
/****** Object:  StoredProcedure [dbo].[InsertPaymentPaydate]    Script Date: 5/28/2019 10:08:03 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertPaymentPaydate]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[InsertPaymentPaydate]
GO
/****** Object:  StoredProcedure [dbo].[InsertPaymentPaydate]    Script Date: 5/28/2019 10:08:03 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/******************************************************************************
* PROCEDURE:  [dbo].[InsertPaymentPaydate]
* PURPOSE: Inserts PaymentRec Paydate 
* NOTES:
* CREATED: 12/6/2017  Genady G.
* MODIFIED
* DATE     AUTHOR      DESCRIPTION
*------------------------------------------------------------------------------
*****************************************************************************/
CREATE PROCEDURE [dbo].[InsertPaymentPaydate] 
	@paymentRecordIDList VARCHAR(max)
	,@paydate_Calendar_ID INT
AS
BEGIN
	BEGIN TRY
	-- convert list to xml
	DECLARE @xml XML
	SET @xml = N'<root><r>' + replace(@paymentRecordIDList, ',', '</r><r>') + '</r></root>'
		
	-- INSERT if entry does NOT exist
	if NOT EXISTS (SELECT NULL 
			FROM [dbo].[TB_PAYMENT_PAYDATE_CALENDAR] (nolock)
			WHERE [Payment_Record_ID] in (SELECT t.value('.', 'int') FROM @xml.nodes('//root/r') AS a(t)))
	BEGIN
		INSERT INTO [dbo].[TB_PAYMENT_PAYDATE_CALENDAR] (
			[Payment_Record_ID]
			,[Paydate_Calendar_ID]
			)
		SELECT t.value('.', 'int'), @paydate_Calendar_ID  FROM @xml.nodes('//root/r') AS a(t)
	END
	ELSE
	BEGIN
		--UPDATE if entry exists
		UPDATE [dbo].[TB_PAYMENT_PAYDATE_CALENDAR]
		SET [Paydate_Calendar_ID] = @paydate_Calendar_ID
		WHERE [Payment_Record_ID] IN (SELECT t.value('.', 'int') FROM @xml.nodes('//root/r') AS a(t))
	END
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
