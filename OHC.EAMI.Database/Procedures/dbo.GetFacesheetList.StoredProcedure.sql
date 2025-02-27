USE [EAMI-MC]
GO
/****** Object:  StoredProcedure [dbo].[GetFacesheetList]    Script Date: 12/12/2023 12:51:42 PM ******/

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetFacesheetList]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetFacesheetList]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/******************************************************************************
* PROCEDURE:  [dbo].[GetFacesheetList]
* PURPOSE: Fetches the list of Facesheet values
* NOTES:
* CREATED: 12/12/2023  Meetu S.
* MODIFIED
* DATE		AUTHOR      DESCRIPTION
*------------------------------------------------------------------------------
-- exec GetFacesheetList 0, 1
*****************************************************************************/
CREATE PROCEDURE [dbo].[GetFacesheetList]  
 -- Add the parameters for the stored procedure here   
 @includeInactive bit,  
 @systemID int  
AS  
BEGIN  
 IF (@includeInactive = 1) --in case of reports where we need all active + inactive funds  
 BEGIN  
   SELECT Agency_Number = '4260' --remove hardcoding later system properties screen is created  
   , Agency_Name = 'DEPT. OF HEALTH CARE SERVICES' --remove hardcoding later system properties screen is created  
   , F.Stat_Year  
   , Fiscal_Year = '2015' --remove hardcoding later system properties screen is created  
   , F.Fund_Code  
   , F.Fund_Name  
   , F.System_ID  
   , FS.*   
  FROM TB_FACESHEET as FS  
  JOIN TB_FUND AS F ON FS.Fund_ID = F.Fund_ID  
  JOIN TB_SYSTEM AS S ON F.System_ID = S.System_ID    
 END;  
 ELSE -- in case of Fund UI, where we need to show only Active Funds  
 BEGIN  
  SELECT Agency_Number = '4260' --remove hardcoding later system properties screen is created  
   , Agency_Name = 'DEPT. OF HEALTH CARE SERVICES' --remove hardcoding later system properties screen is created  
   , F.Stat_Year  
   , Fiscal_Year = '2015' --remove hardcoding later system properties screen is created  
   , F.Fund_Code  
   , F.Fund_Name  
   , F.System_ID  
   , FS.*   
  FROM TB_FACESHEET as FS  
  JOIN TB_FUND AS F ON FS.Fund_ID = F.Fund_ID  
  JOIN TB_SYSTEM AS S ON F.System_ID = S.System_ID    
  WHERE FS.IsActive = 1  
  AND F.IsActive = 1  
  AND F.System_ID = @systemID  
    
  return;  
 END;  
 SET NOCOUNT ON;  
 SELECT * FROM TB_FACESHEET   
 WHERE IsActive = @includeInactive  
END 
