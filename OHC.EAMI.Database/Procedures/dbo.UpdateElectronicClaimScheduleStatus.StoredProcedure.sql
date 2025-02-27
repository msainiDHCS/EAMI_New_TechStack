USE [EAMI-MC]
GO
/****** Object:  StoredProcedure [dbo].[UpdateElectronicClaimScheduleStatus]    Script Date: 5/28/2019 10:08:03 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UpdateElectronicClaimScheduleStatus]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[UpdateElectronicClaimScheduleStatus]
GO
/****** Object:  StoredProcedure [dbo].[UpdateElectronicClaimScheduleStatus]    Script Date: 5/28/2019 10:08:03 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/******************************************************************************
* PROCEDURE:  [dbo].[UpdateElectronicClaimScheduleStatus]
* PURPOSE: Update ESC (Electronic Claim Schedule) record status
* NOTES:
* CREATED: 03/09/2018  Eugene S.
* MODIFIED
* DATE           AUTHOR      DESCRIPTION
  7/26/2018      Genady G	 Update related status fields on status update
  5/9/2024       Meetu S   EAMI-1527: Production Issue for Multiple tabs and updating the records
*------------------------------------------------------------------------------
*****************************************************************************/
CREATE PROCEDURE [dbo].[UpdateElectronicClaimScheduleStatus]   
 @EcsId int   
 ,@EcsStatusTypeId int  
 ,@StatusDate DATETIME  
 ,@StatusNote varchar(200) = null  
 ,@User varchar(20) = null
 ,@Status    varchar(10) OUTPUT
AS  
BEGIN  
 BEGIN TRY    
   
  DECLARE @ECS_Status_TypeID_APPROVED int = (SELECT [ECS_Status_Type_ID] from [dbo].[TB_ECS_STATUS_TYPE] WHERE [Code] = 'APPROVED')  
  DECLARE @ECS_Status_TypeID_SENT_TO_SCO int = (SELECT [ECS_Status_Type_ID] from [dbo].[TB_ECS_STATUS_TYPE] WHERE [Code] = 'SENT_TO_SCO')  
  DECLARE @ECS_Status_TypeID_WARRANT_RECEIVED int = (SELECT [ECS_Status_Type_ID] from [dbo].[TB_ECS_STATUS_TYPE] WHERE [Code] = 'WARRANT_RECEIVED')  
  DECLARE @ECS_Status_TypeID_PENDING int = (SELECT [ECS_Status_Type_ID] from [dbo].[TB_ECS_STATUS_TYPE] WHERE [Code] = 'PENDING')
  
--****** STARTS: Cross browser session issue handling>preventing user to update status for a different program *************
  DECLARE @Current_ECS_Status_Type_ID int = (SELECT Current_ECS_Status_Type_ID FROM TB_ECS where ECS_ID = @EcsId) 
 
  IF(@Current_ECS_Status_Type_ID = @ECS_Status_TypeID_WARRANT_RECEIVED)
  BEGIN
		Set @Status = 'ERROR';  
		return;  
  END

  IF(@Current_ECS_Status_Type_ID = @ECS_Status_TypeID_SENT_TO_SCO and @EcsStatusTypeId = @ECS_Status_TypeID_APPROVED)
  BEGIN
		Set @Status = 'ERROR';  
		return;  
  END
--****** ENDS ************ 

	UPDATE [dbo].[TB_ECS]  
	SET [Current_ECS_Status_Type_ID] = @EcsStatusTypeId  
	,[CurrentStatusDate] = @StatusDate  
	,[CurrentStatusNote] = @StatusNote  
	WHERE ECS_ID = @EcsId  
  
	--APPROVED  
	IF @EcsStatusTypeId = @ECS_Status_TypeID_APPROVED  
	BEGIN  
	UPDATE [dbo].[TB_ECS] SET ApproveDate = @StatusDate, ApprovedBy = @User WHERE ECS_ID = @EcsId  
	END  
	--SENT_TO_SCO  
	ELSE IF @EcsStatusTypeId = @ECS_Status_TypeID_SENT_TO_SCO  
	BEGIN  
	UPDATE [dbo].[TB_ECS] SET SentToScoDate = @StatusDate WHERE ECS_ID = @EcsId  
	END  
	--WARRANT_RECEIVED  
	ELSE IF @EcsStatusTypeId = @ECS_Status_TypeID_WARRANT_RECEIVED  
	BEGIN  
	UPDATE [dbo].[TB_ECS] SET WarrantReceivedDate = @StatusDate WHERE ECS_ID = @EcsId  
	END  
	--PENDING  
	ELSE IF @EcsStatusTypeId = @ECS_Status_TypeID_PENDING  
	BEGIN  
	UPDATE [dbo].[TB_ECS] SET CurrentStatusDate = @StatusDate WHERE ECS_ID = @EcsId  
	END  

	Set @Status = 'OK';  
	-- Set @Message = 'ECS Status Updated Successfully.';    
	return;  
     
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
