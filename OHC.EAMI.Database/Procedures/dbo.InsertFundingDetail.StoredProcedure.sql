USE [EAMI-MC]
GO
/****** Object:  StoredProcedure [dbo].[InsertFundingDetail]    Script Date: 5/28/2019 10:08:03 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertFundingDetail]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[InsertFundingDetail]
GO
/****** Object:  StoredProcedure [dbo].[InsertFundingDetail]    Script Date: 5/28/2019 10:08:03 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/******************************************************************************
* PROCEDURE:  [dbo].[InsertFundingDetail]
* PURPOSE: Inserts Funding Detail and returns identity
* NOTES:
* CREATED: 09/19/2017  Eugene S.
* MODIFIED
* DATE     AUTHOR      DESCRIPTION
*------------------------------------------------------------------------------
* 02/11/2019 Eugene S. Removed Waiver Name and Waiver Category insert parameters 
*****************************************************************************/
CREATE PROCEDURE [dbo].[InsertFundingDetail] 
	@PaymentRecId INT
	,@FundingSourceName VARCHAR(50)
	,@FFPAmount MONEY
	,@SGFAmount MONEY
	,@FiscalYear VARCHAR(5)
	,@FiscalQuarter VARCHAR(5)
	,@Title VARCHAR(10)
AS
BEGIN
	BEGIN TRY
		INSERT INTO [dbo].[TB_FUNDING_DETAIL] (
			[Payment_Record_ID]
			,[Funding_Source_Name]
			,[FFPAmount]
			,[SGFAmount]
			,[FiscalYear]
			,[FiscalQuarter]
			,[Title]
			)
		VALUES (
			@PaymentRecId
			,@FundingSourceName
			,@FFPAmount
			,@SGFAmount
			,@FiscalYear
			,@FiscalQuarter
			,@Title
			)

		SELECT SCOPE_IDENTITY() AS [SCOPE_IDENTITY];
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
