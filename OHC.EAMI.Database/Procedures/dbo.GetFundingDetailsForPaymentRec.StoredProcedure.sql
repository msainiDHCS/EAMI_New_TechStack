USE [EAMI-MC]
GO
/****** Object:  StoredProcedure [dbo].[GetFundingDetailsForPaymentRec]    Script Date: 5/28/2019 10:08:03 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetFundingDetailsForPaymentRec]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetFundingDetailsForPaymentRec]
GO
/****** Object:  StoredProcedure [dbo].[GetFundingDetailsForPaymentRec]    Script Date: 5/28/2019 10:08:03 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/******************************************************************************
* PROCEDURE:  [dbo].[GetFundingDetailsForPaymentRec]
* PURPOSE: Gets unassigned/unprocessed (received) payment records 
* NOTES:
* CREATED: 11/3/2017  Eugene S.
* MODIFIED
* DATE     AUTHOR      DESCRIPTION
*------------------------------------------------------------------------------
* 02/21/2019 Eugene S.  Removed Waiver Name and Waiver Category fields
*****************************************************************************/
CREATE PROCEDURE [dbo].[GetFundingDetailsForPaymentRec]
	@paymentRecId int
AS
BEGIN
	BEGIN TRY
		
		SELECT [Funding_Detail_ID]
			,[Payment_Record_ID]
			,[Funding_Source_Name]
			,[FFPAmount]
			,[SGFAmount]
			,[TotalAmount]
			,[FiscalYear]
			,[FiscalQuarter]
			,[Title]
		FROM [dbo].[TB_FUNDING_DETAIL] FD
		WHERE FD.Payment_Record_ID = @paymentRecId

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
