USE [EAMI-MC]
GO
/****** Object:  StoredProcedure [dbo].[InsertPaymentClaimSchedule]    Script Date: 5/28/2019 10:08:03 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertPaymentClaimSchedule]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[InsertPaymentClaimSchedule]
GO
/****** Object:  StoredProcedure [dbo].[InsertPaymentClaimSchedule]    Script Date: 5/28/2019 10:08:03 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/******************************************************************************
* PROCEDURE:  [dbo].[InsertPaymentClaimSchedule]
* PURPOSE: Inserts PaymentRec Claim Schedule 
* NOTES:
* CREATED: 12/6/2017  Genady G.
* MODIFIED
* DATE     AUTHOR      DESCRIPTION
*------------------------------------------------------------------------------
*****************************************************************************/
CREATE PROCEDURE [dbo].[InsertPaymentClaimSchedule] 
	@PaymenRecordIDList VARCHAR(max),
	@claim_Schedule_ID INT
AS
BEGIN
	BEGIN TRY
	-- convert list to xml
		DECLARE @xml XML
		SET @xml = N'<root><r>' + replace(@PaymenRecordIDList, ',', '</r><r>') + '</r></root>'

		INSERT INTO [dbo].[TB_PAYMENT_CLAIM_SCHEDULE] (
				[Payment_Record_ID]
				,[Claim_Schedule_ID]
				)
			SELECT 
				t.value('.', 'int') AS [Payment_Record_ID],
				@claim_Schedule_ID AS [Claim_Schedule_ID]
			FROM @xml.nodes('//root/r') AS a(t) 
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
