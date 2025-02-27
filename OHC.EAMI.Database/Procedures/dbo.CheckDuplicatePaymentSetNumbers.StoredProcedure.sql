USE [EAMI-PRX]
GO
/****** Object:  StoredProcedure [dbo].[CheckDuplicatePaymentSetNumbers]    Script Date: 5/28/2019 10:08:03 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CheckDuplicatePaymentSetNumbers]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[CheckDuplicatePaymentSetNumbers]
GO
/****** Object:  StoredProcedure [dbo].[CheckDuplicatePaymentSetNumbers]    Script Date: 5/28/2019 10:08:03 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/******************************************************************************
* PROCEDURE:  [dbo].[CheckDuplicatePaymentSetNumbers]
* PURPOSE: Searches existing payment records based on provided list of payment set number list
* NOTES:
* CREATED: 12/20/2017  Eugene S.
* MODIFIED
* DATE     AUTHOR      DESCRIPTION
*------------------------------------------------------------------------------
*****************************************************************************/
CREATE PROCEDURE [dbo].[CheckDuplicatePaymentSetNumbers]
	@PaymentSetNumberList VARCHAR(max)
AS
BEGIN
	BEGIN TRY
		
		-- convert list to xml
		DECLARE @xml XML
		SET @xml = N'<root><r>' + replace(@PaymentSetNumberList, ',', '</r><r>') + '</r></root>'
				
		-- select recs using xml
		SELECT distinct PaymentSet_Number 
		FROM TB_PAYMENT_RECORD		
		WHERE PaymentSet_Number in ( 
			SELECT t.value('.', 'varchar(max)') AS [PaymentSet_Number]
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
