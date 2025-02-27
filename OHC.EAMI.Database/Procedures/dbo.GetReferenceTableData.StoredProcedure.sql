USE [EAMI-MC]
GO

/****** Object:  StoredProcedure [dbo].[GetReferenceTableData]    Script Date: 3/22/2024 12:53:45 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

  
/******************************************************************************  
* PROCEDURE:  [dbo].[GetReferenceTableData]  
* PURPOSE: Multiple selects for each reference table  
* NOTES:  
* CREATED: 09/25/2017  Eugene S.  
* MODIFIED  
* DATE     AUTHOR      DESCRIPTION  
* Updated:Gopal P. Updated Exclusive Payment Type Table 
* 03/13/2024:Gopal P. Added TB_SCO_PROPERTY, TB_SCO_FILE_PROPERTY, TB_SYSTEM Table 
*------------------------------------------------------------------------------  
*****************************************************************************/  
CREATE PROCEDURE [dbo].[GetReferenceTableData]  
AS  
BEGIN  
 BEGIN TRY  
    
  --1. TB_TRANSACTION_TYPE  
  SELECT  
    'TB_TRANSACTION_TYPE' as TABLE_NAME  
   ,[Transaction_Type_ID] as ID  
   ,[Code]  
   ,[Description]  
   ,[IsActive]  
   ,[Sort_Value]  
   ,[CreateDate]  
   ,[CreatedBy]  
   ,[UpdateDate]  
   ,[UpdatedBy]  
  FROM [dbo].[TB_TRANSACTION_TYPE]  
  
  --2. TB_SYSTEM_OF_RECORD  
  SELECT  
   'TB_SYSTEM_OF_RECORD' as TABLE_NAME   
   ,[SOR_ID] as ID  
   ,[Code]  
   ,[Description]  
   ,[IsActive]  
   ,[Sort_Value]  
   ,[CreateDate]  
   ,[CreatedBy]  
   ,[UpdateDate]  
   ,[UpdatedBy]  
  FROM [dbo].[TB_SYSTEM_OF_RECORD]  
  
  --3. TB_SOR_KVP_KEY  
  SELECT  
   'TB_SOR_KVP_KEY' as TABLE_NAME    
   ,[SOR_Kvp_Key_ID] as ID  
   ,[SOR_ID]  
   ,[Kvp_Key_Name] as Code  
   ,[Kvp_Value_DataType]  
   ,[Kvp_Value_Length]  
   ,[Kvp_Description] as [Description]  
   ,[OwnerEntity]  
   ,[IsActive]  
   ,[Sort_Value]  
   ,[CreateDate]  
   ,[CreatedBy]  
   ,[UpdateDate]  
   ,[UpdatedBy]  
  FROM [dbo].[TB_SOR_KVP_KEY]  
    
  --4. TB_RESPONSE_STATUS_TYPE  
  SELECT  
   'TB_RESPONSE_STATUS_TYPE' as TABLE_NAME   
   ,[Response_Status_Type_ID] as ID  
   ,[Code]  
   ,[Description]  
   ,[IsActive]  
   ,[Sort_Value]  
   ,[CreateDate]  
   ,[CreatedBy]  
   ,[UpdateDate]  
   ,[UpdatedBy]  
  FROM [dbo].[TB_RESPONSE_STATUS_TYPE]  
  
  --5. TB_PAYMENT_STATUS_TYPE  
  SELECT  
   'TB_PAYMENT_STATUS_TYPE' as TABLE_NAME  
   ,[Payment_Status_Type_ID] as ID  
   ,[Code]  
   ,[Description]  
   ,[IsActive]  
   ,[Sort_Value]  
   ,[CreateDate]  
   ,[CreatedBy]  
   ,[UpdateDate]  
   ,[UpdatedBy]  
  FROM [dbo].[TB_PAYMENT_STATUS_TYPE]  
  
  -- 6. TB_DB_SETTING_TYPE  
  --SELECT   
  -- 'TB_DB_SETTING_TYPE' as TABLE_NAME  
  -- ,[DB_Setting_Type_ID] as ID  
  -- ,[Code]  
  -- ,[Description]  
  -- ,[IsActive]  
  -- ,[Sort_Value]  
  -- ,[CreateDate]  
  -- ,[CreatedBy]  
  -- ,[UpdateDate]  
  -- ,[UpdatedBy]  
  --FROM [dbo].[TB_DB_SETTING_TYPE]  
  
  --7. TB_DB_SETTING  
  --SELECT   
  -- 'TB_DB_SETTING' as TABLE_NAME  
  -- ,ds.[DB_Setting_ID] as ID  
  -- ,ds.[DB_Setting_Key] as Code  
  -- ,ds.[DB_Setting_Value]     
  -- ,ds.[DB_Setting_Type_ID]  
  -- ,dst.Code as [DB_Setting_Type_Name]  
  -- ,ds.[Description]  
  -- ,ds.[IsActive]  
  -- ,ds.[Sort_Value]  
  -- ,ds.[CreateDate]  
  -- ,ds.[CreatedBy]  
  -- ,ds.[UpdateDate]  
  -- ,ds.[UpdatedBy]  
  --FROM [dbo].[TB_DB_SETTING] ds  
  --INNER JOIN TB_DB_SETTING_TYPE dst on dst.DB_Setting_Type_ID = ds.DB_Setting_Type_ID  
  
  --8. TB_PAYMENT_EXCHANGE_ENTITY  
  SELECT  
   'TB_PAYMENT_EXCHANGE_ENTITY' as TABLE_NAME   
   ,[Payment_Exchange_Entity_ID] as ID  
   ,[Entity_ID] as Code  
   ,[Entity_Name] as [Description]  
   ,[Entity_ID_Type]  
   ,[Entity_EIN]  
   ,[IsActive]  
   ,[Sort_Value]  
   ,[CreateDate]  
   ,[CreatedBy]  
   ,[UpdateDate]  
   ,[UpdatedBy]  
  FROM [dbo].[TB_PAYMENT_EXCHANGE_ENTITY]  
  
  --9. TB_PAYDATE_CALENDAR  
  SELECT  
   'TB_PAYDATE_CALENDAR' as TABLE_NAME   
   ,[Paydate_Calendar_ID] as ID  
   ,Convert(varchar(10), [Paydate], 120) as Code  
   ,[Note] as [Description]  
   ,[IsActive]  
   ,0 as Sort_Value  
   ,[CreateDate]  
   ,[CreatedBy]  
   ,[UpdateDate]  
   ,[UpdatedBy]  
  FROM [dbo].[TB_PAYDATE_CALENDAR]  
  
  --10. TB_DRAWDATE_CALENDAR  
  SELECT  
   'TB_DRAWDATE_CALENDAR' as TABLE_NAME   
   ,[Drawdate_Calendar_ID] as ID  
   ,Convert(varchar(10), [Drawdate], 120) as Code  
   ,[Note] as [Description]  
   ,[IsActive]  
   ,0 as Sort_Value  
   ,[CreateDate]  
   ,[CreatedBy]  
   ,[UpdateDate]  
   ,[UpdatedBy]  
  FROM [dbo].[TB_DRAWDATE_CALENDAR]  
  
  --11. TB_CLAIM_SCHEDULE_STATUS_TYPE  
  SELECT  
   'TB_CLAIM_SCHEDULE_STATUS_TYPE' as TABLE_NAME   
   ,[Claim_Schedule_Status_Type_ID] as ID  
   ,[Code]  
   ,[Description]  
   ,[IsActive]  
   ,[Sort_Value]  
   ,[CreateDate]  
   ,[CreatedBy]  
   ,[UpdateDate]  
   ,[UpdatedBy]  
  FROM [dbo].[TB_CLAIM_SCHEDULE_STATUS_TYPE]  
  
  -- 12. TB_PAYMENT_STATUS_TYPE_EXTERNAL  
  SELECT  
   'TB_PAYMENT_STATUS_TYPE_EXTERNAL' as TABLE_NAME   
   ,[Payment_Status_Type_Ext_ID] as ID  
   ,[Code]  
   ,[Description]  
   ,[IsActive]  
   ,[Sort_Value]  
   ,[CreateDate]  
   ,[CreatedBy]  
   ,[UpdateDate]  
   ,[UpdatedBy]  
  FROM [dbo].[TB_PAYMENT_STATUS_TYPE_EXTERNAL]  
  
  -- 13. TB_ECS_STATUS_TYPE  
  SELECT   
   'TB_ECS_STATUS_TYPE' AS TABLE_NAME  
   ,[ECS_Status_Type_ID] as ID  
   ,[CODE]  
   ,[Description]  
   ,[IsActive]  
   ,[Sort_Value]  
   ,[CreateDate]  
   ,[CreatedBy]  
   ,[UpdateDate]  
   ,[UpdatedBy]  
  FROM [dbo].[TB_ECS_STATUS_TYPE]  
  
  -- 14. TB_EXCLUSIVE_PAYMENT_TYPE  
    
  SELECT  
   'TB_EXCLUSIVE_PAYMENT_TYPE' as TABLE_NAME   
   ,[Exclusive_Payment_Type_ID] as ID  
   ,[Code]   
   ,[Description]        
   ,[IsActive]   
   ,[System_ID]  
   ,[Fund_ID]  
   ,0 as [Sort_Value]  
   ,[CreateDate]  
   ,[CreatedBy]  
   ,[UpdateDate]  
   ,[UpdatedBy]     
  FROM [dbo].[TB_EXCLUSIVE_PAYMENT_TYPE]  
  
  -- 15. TB_USER  
  SELECT  
   'TB_USER' as TABLE_NAME   
   ,U.[User_ID] as ID  
   ,U.[User_Name] as Code  
   ,U.[Display_Name] as [Description]  
   ,U.[User_EmailAddr]    
   ,U.[Domain_Name]  
   ,UT.[User_Type_Code]  
   ,UT.[User_Type_Name]   
   ,U.[IsActive]  
   ,1 as [Sort_Value]  
   ,U.[CreateDate]  
   ,U.[CreatedBy]  
   ,U.[UpdateDate]  
   ,U.[UpdatedBy]  
  FROM [dbo].[TB_USER] U  
  INNER JOIN TB_USER_TYPE UT on UT.User_Type_ID = U.User_Type_ID    
  
  -- 16. TB_PAYMENT_METHOD_TYPE  
  SELECT   
   'TB_PAYMENT_METHOD_TYPE' AS TABLE_NAME  
   ,[Payment_Method_Type_ID] as ID  
   ,[Code]  
   ,[Description]  
   ,[IsActive]  
   ,1 as [Sort_Value]  
   ,[CreateDate]  
   ,[CreatedBy]  
   ,[UpdateDate]  
   ,[UpdatedBy]  
  FROM [dbo].[TB_PAYMENT_METHOD_TYPE]  
  
  -- 17. TB_FUNDING_SOURCE  
  SELECT   
   'TB_FUNDING_SOURCE' AS TABLE_NAME  
   ,[FUNDING_SOURCE_ID] as ID  
   ,[Code]  
   ,[Description]  
   ,[IsActive]  
   ,1 as [Sort_Value]  
   ,[CreateDate]  
   ,[CreatedBy]  
   ,[UpdateDate]  
   ,[UpdatedBy]  
  FROM [dbo].[TB_FUNDING_SOURCE]  
  
  -- 18. TB_SCO_PROPERTY  
    
  SELECT   
   'TB_SCO_PROPERTY' as TABLE_NAME  
   ,ds.[SCO_Property_ID] as ID  
   ,ds.[SCO_Property_Name] as Code  
   ,ds.[SCO_Property_Value]     
   ,ds.[SCO_Property_Type_ID]  
   ,dst.Code as [SCO_Property_Type_Name]  
   ,ds.[Description]  
   ,ds.[IsActive]  
   ,ds.[Sort_Value]  
   ,ds.[CreateDate]  
   ,ds.[CreatedBy]  
   ,ds.[UpdateDate]  
   ,ds.[UpdatedBy]  
  FROM [dbo].[TB_SCO_PROPERTY] ds  
  INNER JOIN TB_SCO_PROPERTY_TYPE dst on dst.SCO_Property_Type_ID = ds.SCO_Property_Type_ID  
  
  
  ------ 19. TB_SCO_FILE_PROPERTY  
    
  SELECT   
   'TB_SCO_FILE_PROPERTY' as TABLE_NAME  
   ,ds.[SCO_File_Property_ID] as ID  
   ,ds.[SCO_File_Property_Name] as Code  
   ,ds.[SCO_File_Property_Value]     
   ,ds.[SCO_Property_Type_ID]  
   ,dst.Code as [SCO_Property_Type_Name]  
   ,ds.[Fund_ID]  
   ,ds.[Payment_Type]  
   ,ds.[Environment]  
   ,ds.[System_ID]  
   ,ds.[Description]  
   ,ds.[SCO_Property_Enum_ID]  
   ,ds.[IsActive]  
   ,ds.[Sort_Value]  
   ,ds.[CreateDate]  
   ,ds.[CreatedBy]  
   ,ds.[UpdateDate]  
   ,ds.[UpdatedBy]  
  FROM [dbo].[TB_SCO_FILE_PROPERTY] ds  
  INNER JOIN TB_SCO_PROPERTY_TYPE dst on dst.SCO_Property_Type_ID = ds.SCO_Property_Type_ID  
  
   ------ 20. TB_SYSTEM  
    
  SELECT   
    'TB_SYSTEM' as TABLE_NAME  
    ,ds.[System_ID] as ID  
    ,ds.[System_Code] as Code  
    ,ds.[System_Name] as Description    
    ,ds.[RA_DEPARTMENT_NAME]  
    ,ds.[RA_DEPARTMENT_ADDR_LINE]  
    ,ds.[RA_DEPARTMENT_ADDR_CSZ]  
    ,ds.[RA_ORGANIZATION_CODE]
    ,ds.[RA_INQUIRIES_PHONE_NUMBER]  
    ,ds.[FEIN_Number]
    ,ds.[MAX_PYMT_REC_AMOUNT] 
    ,ds.[MAX_PYMT_REC_PER_TRAN]
    ,ds.[MAX_FUNDING_DTL_PER_PYMT_REC]
    ,ds.[TRACE_INCOMING_PAYMENT_DATA]
    ,ds.[IsActive]  
    ,ds.[Sort_Order] as Sort_Value 
    ,ds.[CreateDate]  
    ,ds.[CreatedBy]  
    ,ds.[UpdateDate]  
    ,ds.[UpdatedBy]  
  FROM [dbo].[TB_System] ds 

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


