USE [EAMI-MC]
GO
/****** Object:  StoredProcedure [dbo].[AddFundingSource]    Script Date: 1/15/2024 10:24:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


/******************************************************************************
* PROCEDURE:  [dbo].[AddFundingSource]
* PURPOSE: inserts a new funding source in the system. Checks for duplicate Funding Source code and returns an error.
* NOTES:
* CREATED: 12/08/2023  Gopal P.
* MODIFIED
* DATE		AUTHOR      DESCRIPTION
*------------------------------------------------------------------------------

*****************************************************************************/
ALTER PROCEDURE [dbo].[AddFundingSource]	
	@Code			varchar(50),	
	@Description	varchar(100),	
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
					
					Set @Sort_value = (Select max(Sort_Value) From dbo.TB_FUNDING_SOURCE Where System_ID =@System_ID);

					Set @Status = 'ERROR';
					Set @Message = '';
					IF (Select Count(1) From dbo.TB_FUNDING_SOURCE Where ltrim(rtrim(lower(Code))) = ltrim(rtrim(lower(@Code))) 
						AND IsActive = 1 
						AND System_ID = @System_ID) > 0
					BEGIN
								SET @Status = 'ERROR';	
								Set @Message = 'Duplicate Funding Source Name found';		
								RETURN;
					END;
					ELSE
					BEGIN
							INSERT INTO [dbo].[TB_FUNDING_SOURCE]
								   ([Code]
								   ,[Description]								   
								   ,[System_ID]
								   ,[IsActive]
								   ,[Sort_Value]
								   ,[CreateDate]
								   ,[CreatedBy]
								   ,[UpdateDate]
								   ,[UpdatedBy]
								   ,[DeactivatedBy]
								   ,[DeactivatedDate])
							 VALUES
								   (@Code
								   ,@Description								   
								   ,@System_ID
								   ,@IsActive
								   ,@Sort_value
								   ,GETDATE()
								   ,@CreatedBy
								   ,NULL
								   ,NULL
								   ,NULL
								   ,NULL)

							--SET @Fund_ID = SCOPE_IDENTITY();
									
							SET @Status = 'OK';
							SET @Message = 'Funding Source created successfully';		
							RETURN;
					END
	END TRY
	BEGIN CATCH
	SET @Message = ERROR_MESSAGE();
				SELECT
				ERROR_NUMBER() AS ErrorNumber
				--,ERROR_SEVERITY() AS ErrorSeverity
				--,ERROR_STATE() AS ErrorState
				--,ERROR_PROCEDURE() AS ErrorProcedure
				--,ERROR_LINE() AS ErrorLine
				,ERROR_MESSAGE() AS ErrorMessage;
				--IF @@TRANCOUNT > 0
				--	ROLLBACK TRANSACTION;
	END CATCH
END
