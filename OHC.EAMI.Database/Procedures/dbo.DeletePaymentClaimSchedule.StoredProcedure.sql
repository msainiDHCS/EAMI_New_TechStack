USE [EAMI-PRX]
GO
/****** Object:  StoredProcedure [dbo].[DeletePaymentClaimSchedule]    Script Date: 5/28/2019 10:08:03 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DeletePaymentClaimSchedule]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[DeletePaymentClaimSchedule]
GO
/****** Object:  StoredProcedure [dbo].[DeletePaymentClaimSchedule]    Script Date: 5/28/2019 10:08:03 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/******************************************************************************
* PROCEDURE:  [dbo].[DeletePaymentClaimSchedule]
* PURPOSE: Delete Payment Claim Schedule Record 
* NOTES:
* CREATED: 01/17/2018  Genady G.
* MODIFIED
* DATE		AUTHOR      DESCRIPTION
*5/30/2018	ggidenko	Allow to delete using a multiple claim schedule IDs
*6/26/2018  ggidenko	Fix bug in parsing the @ClaimScheduleIDList
*9/10/2018  ggidenko	Rename to PaymentSet_Number
*------------------------------------------------------------------------------
*****************************************************************************/
CREATE PROCEDURE [dbo].[DeletePaymentClaimSchedule] 
	@PaymentSet_NumberList VARCHAR(max),
	@ClaimScheduleIDList VARCHAR(max)
AS
BEGIN
	BEGIN TRY

	-- convert list to xml
	DECLARE @xml XML
						
	--PARSE provided PaymentSet_Number List into temp table
	SET @xml = N'<root><r>' + replace(@PaymentSet_NumberList, ',', '</r><r>') + '</r></root>'
	CREATE TABLE #prSet ([PaymentSet_Number] varchar (15));
	INSERT INTO #prSet
	SELECT t.value('.', 'varchar(15)') FROM @xml.nodes('//root/r') AS a(t)
				
	--PARSE provided Claim Schedule IDs into temp table	
	SET @xml = N'<root><r>' + replace(@ClaimScheduleIDList, ',', '</r><r>') + '</r></root>'
	CREATE TABLE #csID ([Claim_Schedule_ID] int);
	INSERT INTO #csID
	SELECT t.value('.', 'int') FROM @xml.nodes('//root/r') AS a(t)

	--DELETE
	DELETE PCS 
	FROM [dbo].[TB_PAYMENT_CLAIM_SCHEDULE] PCS
		INNER JOIN TB_PAYMENT_RECORD PR ON PCS.Payment_Record_ID = PR.Payment_Record_ID
	WHERE 
		PR.PaymentSet_Number in (SELECT [PaymentSet_Number] FROM #prSet) 
		AND PCS.Claim_Schedule_ID IN (SELECT [Claim_Schedule_ID] FROM #csID) 
	
	DROP TABLE #prSet
	DROP TABLE #csID

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
