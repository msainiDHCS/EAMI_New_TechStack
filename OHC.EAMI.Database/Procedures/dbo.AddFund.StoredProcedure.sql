USE [EAMI-MC]
GO
/****** Object:  StoredProcedure [dbo].[AddFund]    Script Date: 10/27/2023 12:51:42 PM ******/

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AddFund]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[AddFund]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/******************************************************************************
* PROCEDURE:  [dbo].[AddFund]
* PURPOSE: inserts a new fund in the system. Checks for duplicate Fund code and returns an error.
* NOTES:
* CREATED: 10/27/2022  Meetu S.
* MODIFIED
* DATE		AUTHOR      DESCRIPTION
*------------------------------------------------------------------------------
-- exec AddFund
*****************************************************************************/
CREATE PROCEDURE [dbo].[AddFund]
	-- parameters for the stored procedure...	
	@Fund_Code			varchar(20),
	@Fund_Description	varchar(100) = null,
	@Fund_Name			varchar(50) = null,
	@Stat_Year			varchar(4),
	@System_ID			int,
	@IsActive			bit,
	@CreatedBy			varchar(20),
	@Status				varchar(10) OUTPUT,
	@Message			varchar(max) OUTPUT	
AS
BEGIN
	
	SET NOCOUNT ON;

	BEGIN TRY	

					Set @Status = 'ERROR';
					Set @Message = '';
					IF (Select Count(1) From dbo.TB_FUND Where ltrim(rtrim(lower(Fund_Code))) = ltrim(rtrim(lower(@Fund_Code))) 
						AND IsActive = 1 
						AND System_ID = @System_ID) > 0
					BEGIN
								SET @Status = 'ERROR';	
								Set @Message = 'Duplicate Fund Code found';		
								RETURN;
					END;
					ELSE
					BEGIN
							INSERT INTO [dbo].[TB_FUND]
								   ([Fund_Code]
								   ,[Fund_Name]
								   ,[Fund_Description]
								   ,[Stat_Year]
								   ,[System_ID]
								   ,[IsActive]
								   ,[CreateDate]
								   ,[CreatedBy]
								   ,[UpdateDate]
								   ,[UpdatedBy]
								   ,[DeactivatedBy]
								   ,[DeactivatedDate])
							 VALUES
								   (@Fund_Code
								   ,@Fund_Name
								   ,@Fund_Description
								   ,@Stat_Year
								   ,@System_ID
								   ,@IsActive
								   ,GETDATE()
								   ,@CreatedBy
								   ,NULL
								   ,NULL
								   ,NULL
								   ,NULL)

							--SET @Fund_ID = SCOPE_IDENTITY();
									
							SET @Status = 'OK';
							SET @Message = 'Fund created successfully';		
							RETURN;
					END
	END TRY
	BEGIN CATCH
	SET @Message = ERROR_MESSAGE();
				SELECT
				ERROR_NUMBER() AS ErrorNumber
				,ERROR_MESSAGE() AS ErrorMessage;
	END CATCH
END
