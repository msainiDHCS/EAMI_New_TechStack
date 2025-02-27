USE [EAMI-MC]
GO
/****** Object:  StoredProcedure [dbo].[AddExclusivePmtType]    Script Date: 12/1/2023 4:00:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/******************************************************************************
* PROCEDURE:  [dbo].[AddExclusivePmtType]
* PURPOSE: inserts a new Exclusive Payment Type. Checks for duplicate Exclusive Payment code and returns an error.
* NOTES:
* CREATED: 11/01/2023  Gopal P.
* MODIFIED
* DATE		AUTHOR      DESCRIPTION
*------------------------------------------------------------------------------
-- exec AddExclusivePmtType
*****************************************************************************/
ALTER PROCEDURE [dbo].[AddExclusivePmtType]
	@Exclusive_Payment_Type_Code			varchar(20),
	@Exclusive_Payment_Type_Description	varchar(100) = null,
	@Exclusive_Payment_Type_Name			varchar(50) = null,
	@Fund_ID			int ,
	@System_ID			int,
	@IsActive			bit,
	@CreatedBy			varchar(20),
	@Status				varchar(10) OUTPUT,
	@Message			varchar(max) OUTPUT	
AS
BEGIN
	
	SET NOCOUNT ON;

	BEGIN TRY	
				

					Declare @Sort_value as tinyint = 0;
					Set @Status = 'ERROR';
					Set @Message = '';
					Set @Sort_value = (Select ISNULL(max(Sort_Value), 0) From dbo.TB_EXCLUSIVE_PAYMENT_TYPE Where System_ID =@System_ID);
					IF (Select Count(1) From dbo.TB_EXCLUSIVE_PAYMENT_TYPE Where ltrim(rtrim(lower(Code))) = ltrim(rtrim(lower(@Exclusive_Payment_Type_Code))) AND IsActive = 1) > 0
					BEGIN
								SET @Status = 'ERROR';	
								Set @Message = 'Duplicate Exclusive Payment Type Code found';		
								RETURN;
					END;
					ELSE
					BEGIN
							INSERT INTO [dbo].[TB_EXCLUSIVE_PAYMENT_TYPE]
								   ([Code]
								   ,[Name]
								   ,[Description]
								   ,[Fund_ID]
								   ,[System_ID]
								   ,[IsActive]
								   ,[Sort_Value]
								   ,[CreateDate]
								   ,[CreatedBy]
								   ,[UpdateDate]
								   ,[UpdatedBy]
								   )
							 VALUES
								   (@Exclusive_Payment_Type_Code
								   ,@Exclusive_Payment_Type_Name
								   ,@Exclusive_Payment_Type_Description
								   ,@Fund_ID
								   ,@System_ID
								   ,@IsActive
								   ,(select @Sort_value +1)
								   ,GETDATE()
								   ,@CreatedBy
								   ,NULL
								   ,NULL
								   )

							
							SET @Status = 'OK';
							SET @Message = 'Exclusive Payment Type created successfully';		
							RETURN;
					END
	END TRY
	BEGIN CATCH
	SET @Message = ERROR_MESSAGE();
				SELECT
				ERROR_NUMBER() AS ErrorNumber				
				,ERROR_MESSAGE() AS ErrorMessage;
				--IF @@TRANCOUNT > 0
				--	ROLLBACK TRANSACTION;
	END CATCH
END
