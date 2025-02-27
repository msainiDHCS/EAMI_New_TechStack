SET NOCOUNT ON;
USE [EAMI-MC]
GO
/****** Object:  StoredProcedure [dbo].[CheckDuplicatePaymentRecords]    Script Date: 5/28/2019 10:08:03 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CheckDuplicatePaymentRecords]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[CheckDuplicatePaymentRecords]
GO
/****** Object:  StoredProcedure [dbo].[CheckDuplicatePaymentRecords]    Script Date: 5/28/2019 10:08:03 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/******************************************************************************
* PROCEDURE:  [dbo].[CheckDuplicatePaymentRecords]
* PURPOSE: Searches existing payment records based on provided list of new payment record numbers
* NOTES:
* CREATED: 10/25/2017  Eugene S.
* MODIFIED
* DATE     AUTHOR      DESCRIPTION 
* TEST 1111
*------------------------------------------------------------------------------
*****************************************************************************/
CREATE PROCEDURE [dbo].[CheckDuplicatePaymentRecords]
	@PaymentRecNumList VARCHAR(max)
AS
BEGIN
	BEGIN TRY
		
		-- convert list to xml
		DECLARE @xml XML
		SET @xml = N'<root><r>' + replace(@PaymentRecNumList, ',', '</r><r>') + '</r></root>'
				
		-- select recs using xml
		SELECT distinct PaymentRec_Number 
		FROM TB_PAYMENT_RECORD		
		WHERE PaymentRec_Number in ( 
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
