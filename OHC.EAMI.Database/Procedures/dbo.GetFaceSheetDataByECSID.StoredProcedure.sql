USE [EAMI-MC]
GO
/****** Object:  StoredProcedure [dbo].[GetFaceSheetDataByECSID]    Script Date: 5/28/2019 10:08:03 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetFaceSheetDataByECSID]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetFaceSheetDataByECSID]
GO
/****** Object:  StoredProcedure [dbo].[GetFaceSheetDataByECSID]    Script Date: 5/28/2019 10:08:03 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/******************************************************************************
* PROCEDURE:  [dbo].[GetFaceSheetDataByECSID]
* PURPOSE: Gets Data Elements for FaceSheet by ECSID
* NOTES:
* CREATED: 6/8/2018  Genady G.
* MODIFIED
* DATE     AUTHOR      DESCRIPTION
*--------------------------------------------------------------------------------------------------------------------------------------
*12/19/18	Eugene S.	Added condition to set certain valued based on SCHIP type
*02/28/19	Genady G.	Remove Currency formatting on [Amount] and [RPI_Amount]
*03/05/19	Genady G.	Calculate FFY (Fiscal Year), based on the CS PayDate
*03/05/19	Genady G.	Get Record Count from the [TB_ECS][SCO_File_Line_Count] field
*03/07/19	Genady G.	Bug fix in RPI Amount calculation
*06/24/19	Genady G.	Determine and return full SCO file name
*03/30/20	Genady G.	Add Fund_Name assignment for SCHIP at line #72
*10/28/20	Genady G.	Refactor for EAMI Rx and use of four fixed funds: 0912, 0555, 0001 and 0236
*02/09/21	Joe S.		Added table for Line No, Payee, and Amount
*02/22/23	Gopal P.	Removed EWC Exclusive Payment Type
*02/24/23   Sujitha K.  Added [Payment_Method_Type_ID] column from [TB_ECS] 
*08/25/23   Gopal P.    Modified Chapter value for Fund 0001
*01/01/24   Meetu S.    Removed the EPT hardcoding to pick data directly from TB_FUND and TB_FACESHEET. Jira#: EAMI-1140
*03/07/24 Meetu S.      Removed hardcoding and pulled the values from TB_SYSTEM for Agency Name and Code  

*****************************************************************************************************************************************/
ALTER PROCEDURE [dbo].[GetFaceSheetDataByECSID]
	@ECS_ID int
	, @SystemID int = 1
AS
BEGIN
	BEGIN TRY

		DECLARE @Fund_Name varchar(100) 
		DECLARE @Fund_Number varchar(25) 
		DECLARE @Agency_Name varchar(100) 
		DECLARE @Agency_Number varchar(25) 
		DECLARE @Stat_Year varchar(25) 
		DECLARE @FS_Reference_Item varchar(10) 
		DECLARE @Chapter varchar(10) 
		DECLARE @RPI_Amount money = 0
		DECLARE @Current_FFY varchar(10) 
		DECLARE @Program varchar(10) 
		DECLARE @Element varchar(10) 

		
		--DETERMINE FILE NAME
		DECLARE @File_Name varchar(100) = (SELECT 
			CASE
				WHEN c.ECS_File_Name LIKE '%.WARRANT' THEN 'PS.DEVL.DISB.FTP.' + c.ECS_File_Name
				WHEN c.ECS_File_Name LIKE '%.EFT' THEN 'PS.DEVL.DISB.FTP.' + c.ECS_File_Name
				WHEN c.ECS_File_Name LIKE '%.INPUT' THEN 'PD.EFTCLMS.FTP.DHCS.' + c.ECS_File_Name
			END
		FROM TB_ECS c WHERE c.ECS_ID = @ECS_ID)

		--Determine FundID

	   DECLARE @Fund_ID int = (SELECT TOP 1 EPT.Fund_ID  
							   FROM [dbo].[TB_ECS] ECS  
							   INNER JOIN [dbo].[TB_EXCLUSIVE_PAYMENT_TYPE] EPT ON EPT.[Exclusive_Payment_Type_ID] = ECS.[Exclusive_Payment_Type_ID]  
							   WHERE ECS.[ECS_ID] = @ECS_ID )
		
		--DETERMINE CURRENT FISCAL YEAR
		SET @Current_FFY = (SELECT DISTINCT TOP 1 RIGHT(CONVERT(VARCHAR(8), DATEADD(month, -6, PDCAL.Paydate), 1), 2)
				FROM TB_CLAIM_SCHEDULE_ECS CSECS
					INNER JOIN TB_CLAIM_SCHEDULE CS ON CSECS.Claim_Schedule_ID = CS.Claim_Schedule_ID
					INNER JOIN TB_PAYDATE_CALENDAR PDCAL ON CS.Paydate_Calendar_ID = PDCAL.Paydate_Calendar_ID
				WHERE CSECS.ECS_ID = @ECS_ID)
		
		--SET CUSTOM PROPERTIES FOR EACH FUND			
	   SET @Agency_Name = (SELECT RA_DEPARTMENT_NAME FROM TB_SYSTEM where System_ID = @SystemID and IsActive = 1) 
	   SET @Agency_Number = (SELECT RA_ORGANIZATION_CODE FROM TB_SYSTEM where System_ID = @SystemID and IsActive = 1)  
	   SET @Fund_Number = (SELECT Fund_Code FROM TB_FUND where Fund_ID = @Fund_ID)
	   SET @Fund_Name = (SELECT Fund_Name FROM TB_FUND where Fund_ID = @Fund_ID) 
	   SET @Stat_Year = (SELECT Stat_Year FROM TB_FUND where Fund_ID = @Fund_ID)  
	   SET @Chapter =  (Select Chapter from TB_FACESHEET where Fund_ID = @Fund_ID)  
	   SET @FS_Reference_Item =  (Select Reference_Item from TB_FACESHEET where Fund_ID = @Fund_ID)
	   SET @Program = (Select Program from TB_FACESHEET where Fund_ID = @Fund_ID)
	   SET @Element =  (Select Element from TB_FACESHEET where Fund_ID = @Fund_ID)

		-- MAIN SELET
		SELECT 
			@Fund_Name as [Fund_Name],
			@Agency_Name as [Department_Name],
			@Fund_Number as [Fund_Number],
			@Agency_Number as [Agency_Number],
			@Stat_Year as [Stat_Year],
			@FS_Reference_Item as [Reference_Item],
			@Current_FFY AS [FFY],
			@Chapter as [Chapter],
			@Program as [Program],
			@Element as [Element],
			ECS.Amount as [Amount],
			ECS.[Payment_Method_Type_ID] as [Payment_Method_Type_ID],
			--WARRANT COUNT
			(SELECT COUNT(CSECS.Claim_Schedule_ID) as CS_COUNT
					FROM TB_CLAIM_SCHEDULE_ECS CSECS
					WHERE CSECS.ECS_ID = @ECS_ID
					GROUP BY CSECS.ECS_ID) as [Warrant_Count],								
					
			--RECORD COUNT
			ECS.SCO_File_Line_Count as [Record_Count],	

			--RPI AMOUNT
			(SELECT ISNULL(SUM(PR.Amount),0)
				FROM TB_CLAIM_SCHEDULE_ECS CSECS
					INNER JOIN TB_CLAIM_SCHEDULE CS ON CSECS.Claim_Schedule_ID = CS.Claim_Schedule_ID
					INNER JOIN TB_PAYMENT_CLAIM_SCHEDULE PMTCS ON CS.Claim_Schedule_ID = PMTCS.Claim_Schedule_ID
					INNER JOIN TB_PAYMENT_RECORD PR ON PMTCS.Payment_Record_ID = PR.Payment_Record_ID
				WHERE CSECS.ECS_ID = @ECS_ID and pr.IsReportableRPI = 1) AS [RPI_Amount],
			
			--RPI COUNT
			(SELECT COUNT(*) FROM TB_CLAIM_SCHEDULE where Claim_Schedule_ID IN (
				SELECT PMTCS.Claim_Schedule_ID
				FROM TB_CLAIM_SCHEDULE_ECS CSECS
					INNER JOIN TB_PAYMENT_CLAIM_SCHEDULE PMTCS ON CSECS.Claim_Schedule_ID = PMTCS.Claim_Schedule_ID
					INNER JOIN TB_PAYMENT_RECORD PR ON PMTCS.Payment_Record_ID = PR.Payment_Record_ID
				WHERE CSECS.ECS_ID = @ECS_ID and pr.IsReportableRPI = 1)) AS [RPI_Count],
			ECS.[ECS_Number] as  [ECS_Number],
			@File_Name as  [ECS_File_Name],			
			Current_ECS_Status_Type_ID
		FROM TB_ECS ECS
		WHERE  
			ECS.ECS_ID = @ECS_ID

			-- TABLE SELECT
			SELECT 
				CS.SeqNumber AS [LINE NO.],
				'US BANK' AS [PAYEE],
				CS.Amount AS [AMOUNT]
			FROM TB_CLAIM_SCHEDULE CS
				INNER JOIN TB_CLAIM_SCHEDULE_ECS CSECS ON CS.Claim_Schedule_ID = CSECS.Claim_Schedule_ID
			WHERE 
				CSECS.ECS_ID = @ECS_ID
			ORDER BY CS.SeqNumber

				
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
