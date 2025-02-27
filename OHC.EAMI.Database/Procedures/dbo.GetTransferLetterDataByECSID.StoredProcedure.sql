USE [EAMI-RX]
GO
/****** Object:  StoredProcedure [dbo].[GetTransferLetterDataByECSID]    Script Date: 5/28/2019 10:08:03 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetTransferLetterDataByECSID]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetTransferLetterDataByECSID]
GO
/****** Object:  StoredProcedure [dbo].[GetTransferLetterDataByECSID]    Script Date: 5/28/2019 10:08:03 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


/******************************************************************************
* PROCEDURE:  [dbo].[GetTransferLetterByECSID]
* PURPOSE: Gets Data Elements for Transfer Letter by ECSID
* NOTES:
* CREATED: 7/20/2018  Genady G.
* MODIFIED
* DATE     AUTHOR      DESCRIPTION
*------------------------------------------------------------------------------
* 10/15/18	Eugene S. change to return aggregated funding source details instead of hard-coded detail values
* 12/11/18	Eugene S. change to group details by srcName and Title and return additional table of titles
* 04/09/20	Genady G. Add fund name and determine based if SCHIP or not
* 10/30/20	Joe S.	  Changed title to Medi-Cal RX and added functionality to determine PCFH and EWC
* 10/22/21	Alex H.	  Made @Letter_Title_1 dependent on Code in TB_SYSTEM_OF_RECORD
* 02/28/22	Genady G.	Determine Fiscal Yeasr base on the Pay Date
* 02/22/23	Gopal P.  Added Managed Care Exclusive Payment Types
* 06/26/23 Meetu S.  Added Exclusive Payment Types Description. Jira EAMI-714
*****************************************************************************/
CREATE PROCEDURE [dbo].[GetTransferLetterDataByECSID]
	@ECS_ID int,
	@userName varchar (50) 
AS
BEGIN
	BEGIN TRY
		--Determine exclusive payment type--
		DECLARE @Is_GEMT BIT = (
		SELECT CAST(
			CASE WHEN EXISTS (
					SELECT ECS.*
					FROM TB_ECS ECS
					INNER JOIN TB_EXCLUSIVE_PAYMENT_TYPE EPT ON EPT.Exclusive_Payment_Type_ID = ECS.Exclusive_Payment_Type_ID
					WHERE ECS_ID = @ECS_ID AND EPT.Code = 'GEMT'
					)
				THEN 1
			ELSE 0
			END AS BIT))
		DECLARE @Is_HFEE BIT = (
		SELECT CAST(
			CASE WHEN EXISTS (
					SELECT ECS.*
					FROM TB_ECS ECS
					INNER JOIN TB_EXCLUSIVE_PAYMENT_TYPE EPT ON EPT.Exclusive_Payment_Type_ID = ECS.Exclusive_Payment_Type_ID
					WHERE ECS_ID = @ECS_ID AND EPT.Code = 'HFEE'
					)
				THEN 1
			ELSE 0
			END AS BIT))
		DECLARE @Is_HHP BIT = (
		SELECT CAST(
			CASE WHEN EXISTS (
					SELECT ECS.*
					FROM TB_ECS ECS
					INNER JOIN TB_EXCLUSIVE_PAYMENT_TYPE EPT ON EPT.Exclusive_Payment_Type_ID = ECS.Exclusive_Payment_Type_ID
					WHERE ECS_ID = @ECS_ID AND EPT.Code = 'HHP'
					)
				THEN 1
			ELSE 0
			END AS BIT))
		DECLARE @Is_IHSS BIT = (
		SELECT CAST(
			CASE WHEN EXISTS (
					SELECT ECS.*
					FROM TB_ECS ECS
					INNER JOIN TB_EXCLUSIVE_PAYMENT_TYPE EPT ON EPT.Exclusive_Payment_Type_ID = ECS.Exclusive_Payment_Type_ID
					WHERE ECS_ID = @ECS_ID AND EPT.Code = 'IHSS'
					)
				THEN 1
			ELSE 0
			END AS BIT))
		DECLARE @Is_GF BIT = (
		SELECT CAST(
			CASE WHEN EXISTS (
					SELECT ECS.*
					FROM TB_ECS ECS
					INNER JOIN TB_EXCLUSIVE_PAYMENT_TYPE EPT ON EPT.Exclusive_Payment_Type_ID = ECS.Exclusive_Payment_Type_ID
					WHERE ECS_ID = @ECS_ID AND EPT.Code = 'GF'
					)
				THEN 1
			ELSE 0
			END AS BIT))
		DECLARE @Is_Pcfh BIT = (
		SELECT CAST(
			CASE WHEN EXISTS (
					SELECT ECS.*
					FROM TB_ECS ECS
					INNER JOIN TB_EXCLUSIVE_PAYMENT_TYPE EPT ON EPT.Exclusive_Payment_Type_ID = ECS.Exclusive_Payment_Type_ID
					WHERE ECS_ID = @ECS_ID AND EPT.Code = 'PCFH'
					)
				THEN 1
			ELSE 0
			END AS BIT))
		DECLARE @Is_SCHIP BIT = (
		SELECT CAST(
			CASE WHEN EXISTS (
					SELECT ECS.*
					FROM TB_ECS ECS
					INNER JOIN TB_EXCLUSIVE_PAYMENT_TYPE EPT ON EPT.Exclusive_Payment_Type_ID = ECS.Exclusive_Payment_Type_ID
					WHERE ECS_ID = @ECS_ID AND EPT.Code = 'SCHIP'
					)
				THEN 1
			ELSE 0
			END AS BIT))
			
		
		--Set variables for form--
		DECLARE @Letter_Title_1 varchar (25);
		IF ((select Code from TB_SYSTEM_OF_RECORD) = 'MEDICAL_RX')
		begin
			SET @Letter_Title_1 = 'Medi-Cal RX'
		end
		ELSE IF ((select Code from TB_SYSTEM_OF_RECORD) = 'MDSDFFS')
		begin
			SET @Letter_Title_1 = 'Dental FFS'
		end
		ELSE IF ((select Code from TB_SYSTEM_OF_RECORD) = 'CAPMAN')
		begin
			SET @Letter_Title_1 = 'Managed Care'
		end
		DECLARE @Letter_Title_2 varchar (25) = ''
		DECLARE @Letter_Title_3 varchar (25) = ''
		DECLARE @Fund_Name varchar (50) = 'Health Care Deposit Fund'

		--DETERMINE FISCAL YEAR VALUE BASED on PAY DATE 
		DECLARE @Paydate datetime = (SELECT TOP 1 ECS.Paydate FROM TB_ECS ECS WHERE ECS.ECS_ID = @ECS_ID)
		DECLARE @FY datetime = DATEADD(month, -6, @Paydate)
		DECLARE @fy1 varchar (4) = RIGHT(CONVERT(VARCHAR(8), YEAR(@FY), 1), 4)
		DECLARE @fy2 varchar (4) = RIGHT(CONVERT(VARCHAR(8), YEAR(DATEADD(year, 1, @FY)), 1), 4)
		DECLARE @Fiscal_Year varchar (10) = @fy1 + '/' + @fy2

		DECLARE @REGULAR_MA varchar (20) = 'REGULAR MA'
		DECLARE @ENHANCED_MA varchar (20) = 'ENHANCED MA'
		--Change funding name for exclusive payment types--
		
		 -- Pull Exclusive Payment Type Description for Transfer Letter Header...
		DECLARE @Exclusive_Payment_Type_Description varchar (100);
		DECLARE @Exclusive_Payment_Type_Code varchar (20);
 	
		SELECT @Exclusive_Payment_Type_Description = EP.Description
		,@Exclusive_Payment_Type_Code = EP.Code
		FROM TB_ECS ECS  
		INNER JOIN TB_EXCLUSIVE_PAYMENT_TYPE EPT ON EPT.Exclusive_Payment_Type_ID = ECS.Exclusive_Payment_Type_ID  
		LEFT JOIN TB_EXCLUSIVE_PAYMENT_TYPE EP ON EPT.Exclusive_Payment_Type_ID = EP.Exclusive_Payment_Type_ID
		WHERE ECS_ID = @ECS_ID
		
		IF @Is_GEMT = 1
		begin
			SET @Fund_Name = 'Health Care Deposit Fund'
		end

		ELSE IF @Is_HFEE = 1
		begin
			SET @Fund_Name = 'Health Care Deposit Fund'
		end

		ELSE IF @Is_HHP = 1
		begin
			SET @Fund_Name = 'Health Care Deposit Fund'
		end

		ELSE IF @Is_IHSS = 1
		begin
			SET @Fund_Name = 'Health Care Deposit Fund'
		end

		ELSE IF @Is_GF = 1
		begin
			SET @Fund_Name = 'General Fund'
		end

		ELSE IF @Is_Pcfh = 1
		begin
			SET @Fund_Name = 'General Fund'
		end
		
		ELSE IF @Is_SCHIP = 1
		begin
			SET @Fund_Name = 'Healthy Families Fund'
		end

		--TRANSFER LETTER HEADER
		SELECT 
			@Letter_Title_1 as Title_1,
			@Letter_Title_2 as Title_2,
			@Letter_Title_3 as Title_3,
			@Fund_Name as Fund_Name,
			@Fiscal_Year as Fiscal_Year,
			RIGHT('00' + CONVERT(NVARCHAR(2), DATEPART(MONTH, ECS.PayDate)), 2) + '/' +  CONVERT(NVARCHAR(4), DATEPART(YEAR, ECS.PayDate)) as Month_Year,
			@userName as Preparer,
			ECS.ECS_Number,
			CONVERT(varchar(10), ECS.PayDate, 101) as Pay_Date,
			Current_ECS_Status_Type_ID
			, @Exclusive_Payment_Type_Code as Exclusive_Payment_Type_Code
			, @Exclusive_Payment_Type_Description as Exclusive_Payment_Type_Description
		FROM TB_ECS ECS 
		WHERE ECS.ECS_ID = @ECS_ID

		-- TITLES
		SELECT DISTINCT FD.Title
		FROM TB_FUNDING_DETAIL FD (nolock)
		INNER JOIN TB_PAYMENT_RECORD PR (nolock) ON PR.Payment_Record_ID = FD.Payment_Record_ID
		INNER JOIN TB_PAYMENT_CLAIM_SCHEDULE PCS (nolock) ON PCS.Payment_Record_ID = PR.Payment_Record_ID
		INNER JOIN TB_CLAIM_SCHEDULE CS (nolock) ON CS.Claim_Schedule_ID = PCS.Claim_Schedule_ID
		INNER JOIN TB_CLAIM_SCHEDULE_ECS CSE (nolock) on CSE.Claim_Schedule_ID = CS.Claim_Schedule_ID
		WHERE CSE.ECS_ID = @ECS_ID		

		-- FUNDING DETAILS
		SELECT		
			FD.Funding_Source_Name
			,FD.Title
			,SUM(FD.TotalAmount) AS TotalAmount
			,SUM(FD.FFPAmount) AS FFPAmount
			,SUM(FD.SGFAmount) AS SGFAmount
			--,@REGULAR_MA as [FundingCategory]
			--,count(FD.Funding_Detail_ID) as FDCount			
		FROM TB_FUNDING_DETAIL FD (nolock)
		INNER JOIN TB_PAYMENT_RECORD PR (nolock) ON PR.Payment_Record_ID = FD.Payment_Record_ID
		INNER JOIN TB_PAYMENT_CLAIM_SCHEDULE PCS (nolock) ON PCS.Payment_Record_ID = PR.Payment_Record_ID
		INNER JOIN TB_CLAIM_SCHEDULE CS (nolock) ON CS.Claim_Schedule_ID = PCS.Claim_Schedule_ID
		INNER JOIN TB_CLAIM_SCHEDULE_ECS CSE (nolock) on CSE.Claim_Schedule_ID = CS.Claim_Schedule_ID
		WHERE CSE.ECS_ID = @ECS_ID		
		GROUP BY FD.Funding_Source_Name, FD.Title	
		HAVING NOT (
					SUM (FD.TotalAmount) = 0
					AND SUM(FD.FFPAmount) = 0
					AND SUM(FD.SGFAmount) = 0
				   )
		ORDER BY FD.Funding_Source_Name ASC		
				
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


