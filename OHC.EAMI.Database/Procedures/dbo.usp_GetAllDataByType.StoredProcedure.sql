USE [EAMI-MC]
GO
/****** Object:  StoredProcedure [dbo].[usp_GetAllDataByType]    Script Date: 5/28/2019 10:08:03 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_GetAllDataByType]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_GetAllDataByType]
GO
/****** Object:  StoredProcedure [dbo].[usp_GetAllDataByType]    Script Date: 5/28/2019 10:08:03 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/******************************************************************************  
* PROCEDURE:  [dbo].[usp_GetAllDataByType]  
* PURPOSE: This procedure gets data fpr roles, permissions and systems for grid display.
* NOTES:  
* CREATED: 10/24/2017  Ram Dongre  
* MODIFIED  
* DATE     AUTHOR      DESCRIPTION  
*------------------------------------------------------------------------------  
*02/22/2024	Meetu S.	Added 2 additional fields for pulling DepartmentName and OrganizationCode

-----------------------------------------------------------------------------------
-- exec [usp_GetAllDataByType] 'system'
*****************************************************************************/  

CREATE  PROCEDURE [dbo].[usp_GetAllDataByType]  
(  
 @DataType varchar(10)--possible values can be ROLE/PERMISSION/SYSTEM  
)  
AS  
BEGIN  
  SET NOCOUNT ON;  
  
  BEGIN TRY   
       
     Declare @tb TABLE  
     (  
      ID int,  
      Code varchar(200),  
      Name varchar(250),  
	  DepartmentName varchar (100),
	  OrganizationCode varchar (100),
      CreatedBy varchar(500),  
      CreateDate Datetime,  
      UpdatedBy varchar(500),  
      UpdateDate Datetime,  
      IsActive bit,  
      AssociatedData varchar(2000)  
     );  
  
     If(@DataType = 'ROLE')  
     Begin  
       Insert into @tb  
       Select Role_ID 'ID', Role_Code 'Code', Role_Name 'Name'
			, '' as 'DepartmentName'
			,'' as 'OrganizationCode'
			, CreatedBy, CreateDate, UpdatedBy, UpdateDate, IsActive,  
         [dbo].[fn_GetConcatenatedRolePermissions](Role_ID) 'AssociatedData'  
       From dbo.TB_ROLE;  
     End;  
     Else If(@DataType = 'PERMISSION')  
     Begin  
       Insert into @tb  
       Select Permission_ID 'ID', Permission_Code 'Code', Permission_Name 'Name'
			, '' as 'DepartmentName'
			,'' as 'OrganizationCode'
			, CreatedBy, CreateDate, UpdatedBy, UpdateDate, IsActive,  
         '' 'AssociatedData'  
       From dbo.TB_PERMISSION;  
     End;  
     Else If(@DataType = 'SYSTEM')  
     Begin  
       Insert into @tb  
       Select System_ID 'ID'
			, System_Code 'Code'
			, System_Name 'Name'
			, RA_DEPARTMENT_NAME 'DepartmentName'
			, RA_ORGANIZATION_CODE 'OrganizationCode'
			, CreatedBy
			, CreateDate
			, UpdatedBy
			, UpdateDate
			, IsActive,  
         '' 'AssociatedData'  
       From dbo.TB_SYSTEM;  
     End;  
  
     Select ID, Code, Name, CreatedBy, DepartmentName, OrganizationCode, CreateDate, UpdatedBy, UpdateDate, IsActive, AssociatedData From @tb;  
       
  END TRY  
  BEGIN CATCH  
    --SELECT  
    --ERROR_NUMBER() AS ErrorNumber  
    --,ERROR_SEVERITY() AS ErrorSeverity  
    --,ERROR_STATE() AS ErrorState  
    --,ERROR_PROCEDURE() AS ErrorProcedure  
    --,ERROR_LINE() AS ErrorLine  
    --,ERROR_MESSAGE() AS ErrorMessage;  
    --IF @@TRANCOUNT > 0  
    -- ROLLBACK TRANSACTION;  
  END CATCH   
END  
  
