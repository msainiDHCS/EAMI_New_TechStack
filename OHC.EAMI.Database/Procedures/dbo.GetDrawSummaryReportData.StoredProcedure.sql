USE [EAMI-MC]
GO
/****** Object:  StoredProcedure [dbo].[GetDrawSummaryReportData]    Script Date: 5/28/2019 10:08:03 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetDrawSummaryReportData]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetDrawSummaryReportData]
GO
/****** Object:  StoredProcedure [dbo].[GetDrawSummaryReportData]    Script Date: 5/28/2019 10:08:03 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/******************************************************************************
* PROCEDURE:  [dbo].[GetDrawSummaryReportData]
* PURPOSE: Gets Draw Summary Report Data
* NOTES:
* CREATED: 8/10/2018  Genady G.
* MODIFIED
* DATE     AUTHOR      DESCRIPTION
*------------------------------------------------------------------------------
*****************************************************************************/
CREATE PROCEDURE [dbo].[GetDrawSummaryReportData]
	@PAY_DATE VARCHAR (50) 
AS
BEGIN
	BEGIN TRY

		DECLARE @FUND_SECTION_STATE varchar (50) = 'State Share PFA''s'
		DECLARE @FUND_SECTION_FEDERAL varchar (50) = 'Federal Share'
		DECLARE @FUND_SECTION_FEDERAL_CASH varchar (50) = 'Federal Cash (Including Cash Receipts)**'
		DECLARE @OTHER varchar (50) = 'Other'


		--TRANSFER LETTER DETAIL
		CREATE TABLE #fundingDetail (
			[Funding_Source_Name] varchar (50),
			[DebitAmount] money NULL,
			[CreditAmount] money NULL,
			[FundingCategory] varchar (50)
		);

		--STATE
		INSERT INTO #fundingDetail VALUES('MC170778', NULL, -26873158.49, @FUND_SECTION_STATE)
		INSERT INTO #fundingDetail VALUES('MC170805', NULL, -1516078817.93, @FUND_SECTION_STATE)
		INSERT INTO #fundingDetail VALUES('MC170806', NULL, -82155.37, @FUND_SECTION_STATE)		
		INSERT INTO #fundingDetail VALUES('MM17-487', NULL, -277950.27, @FUND_SECTION_STATE)
		INSERT INTO #fundingDetail VALUES('MM17-488', 589530000.00, NULL, @FUND_SECTION_STATE)
		
		--FEDERAL
		INSERT INTO #fundingDetail VALUES('MC170778', 26873158.49, NULL, @FUND_SECTION_FEDERAL)
		INSERT INTO #fundingDetail VALUES('MC170801', 891523.67, NULL, @FUND_SECTION_FEDERAL)
		INSERT INTO #fundingDetail VALUES('MC170804', NULL, -2224710889.76, @FUND_SECTION_FEDERAL)		
		INSERT INTO #fundingDetail VALUES('MM17-487', NULL, -3424195.73, @FUND_SECTION_FEDERAL)
		
		--FEDERAL CASH
		INSERT INTO #fundingDetail VALUES('MC170802', 285425.00, NULL, @FUND_SECTION_FEDERAL_CASH)
				
		--FEDERAL OTHER
		INSERT INTO #fundingDetail VALUES('MC170801', NULL, 891523.67, @OTHER)
		INSERT INTO #fundingDetail VALUES('MM17-488', NULL, 589530000.00, @OTHER)

		SELECT * FROM #fundingDetail

		SELECT ECS_Number, Amount FROM TB_ECS WHERE PayDate = @PAY_DATE

	DROP TABLE #fundingDetail
				
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
