USE [EAMI-MC]
GO

/****** Object:  StoredProcedure [dbo].[GetSCOPropertiesList]    Script Date: 2/12/2024 10:53:03 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



  
/******************************************************************************  
* PROCEDURE:  [dbo].[GetSCOPropertiesList]  
* PURPOSE: Gets all the SCO Properties including SCO File Properties.
* NOTES:  JIRA: 
* CREATED: 01/29/2024  Gopal P.
* MODIFIED  
* DATE    AUTHOR     DESCRIPTION  
*------------------------------------------------------------------------------  
....Test...
exec [GetSCOPropertiesList]  1,1
*****************************************************************************/  
CREATE PROCEDURE [dbo].[GetSCOPropertiesList]  
    @includeInactive bit,
	@systemID int
	
AS 

BEGIN  
 BEGIN TRY 


	CREATE TABLE #scoProperties
	(	
		ID INT,
		SCO_Property_Name varchar(50),
		SCO_Property_Value varchar(50),
		Description varchar(100),
		Fund_ID INT,
		Fund_Code varchar(20),
		Property_Type_ID INT,
		Payment_Type varchar(20),
		Environment varchar(20),
		CreatedBy varchar(20),
		UpdatedBy varchar(20),
		CreateDate datetime,
		UpdateDate datetime,
		IsActive bit,
		System_ID INT,
		SCO_Property_Enum_ID int
		
	)
	CREATE TABLE #fileProperties
	(	
	ID INT,
		SCO_Property_Name varchar(50),
		SCO_Property_Value varchar(50),
		Description varchar(100),
		Fund_ID INT,
		Fund_Code varchar(20),
		Property_Type_ID INT,
		Payment_Type varchar(20),
		Environment varchar(20),
		CreatedBy varchar(20),
		UpdatedBy varchar(20),
		CreateDate datetime,
		UpdateDate datetime,
		IsActive bit,
		System_ID INT,
		SCO_Property_Enum_ID int
	)


	INSERT INTO #scoProperties
	SELECT 		
		 SCO_Property_ID
		 ,SCO_Property_Name
		,SCO_Property_Value
		,Description
		,0 as Fund_Id
		,'' as Fund_Code
		,SCO_Property_Type_ID
		,'' as Payment_Type
		,'' as Environment
		,CreatedBy
		,UpdatedBy
		,CreateDate
		,UpdateDate
		,IsActive
		,System_ID
		,SCO_Property_Enum_ID
	FROM 
	  TB_SCO_PROPERTY SP
	Where SP.System_ID = @systemID
		AND SP.IsActive = 1;
	   

	INSERT INTO #fileProperties
	SELECT
		 SCO_File_Property_ID
		,SCO_File_Property_Name
		,SCO_File_Property_Value
		,Description
		,SFP.Fund_ID
		,TB_Fund.Fund_Code
		,SCO_Property_Type_ID
		,Payment_Type
		,Environment
		,SFP.CreatedBy
		,SFP.UpdatedBy
		,SFP.CreateDate
		,SFP.UpdateDate
		,SFP.IsActive
		,SFP.System_ID
		,SFP.SCO_Property_Enum_ID
	FROM 
	  TB_SCO_FILE_PROPERTY SFP join TB_FUnd on SFP.Fund_ID = TB_FUND.Fund_ID
	Where SFP.System_ID =@systemID
		AND SFP.IsActive = 1
	  

	SELECT * INTO #temp FROM #scoProperties
	UNION ALL
	SELECT * FROM #fileProperties

	SELECT *
	FROM #temp

	DROP TABLE #scoProperties
	DROP TABLE #fileProperties
	DROP TABLE #temp


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


