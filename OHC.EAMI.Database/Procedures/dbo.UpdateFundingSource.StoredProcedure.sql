USE [EAMI-MC]
GO
/****** Object:  StoredProcedure [dbo].[UpdateFundingSource]    Script Date: 1/15/2024 10:26:03 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


/******************************************************************************
* PROCEDURE:  [dbo].[UpdateFundingSource]
* PURPOSE: updates an existing fund record in the system. Checks for duplicate Fund code and returns an error.
* NOTES:
* CREATED: 12/08/2023  Gopal P.
* MODIFIED
* DATE		AUTHOR      DESCRIPTION
*------------------------------------------------------------------------------
* Updated: 04/17/2024  Gopal P. Added condition for Delete update  
* Updated: 05/03/2024  Meetu S. Disabled Funding Source Code in UI and hence changing the SP (EAMI-1513).
*****************************************************************************/
ALTER PROCEDURE [dbo].[UpdateFundingSource]		
	@Funding_Source_ID			int,
	@Code			varchar(50),
	@System_ID			int,
	@Description	varchar(100),	
	@IsActive			bit,
	@UpdatedBy			varchar(20),
	@Status				varchar(10) OUTPUT,
	@Message			varchar(max) OUTPUT	
AS
BEGIN
	
	SET NOCOUNT ON;

	BEGIN TRY	
					
					Set @Status = 'ERROR';
					Set @Message = '';
					
							UPDATE [dbo].[TB_FUNDING_SOURCE]
							 SET
									--Code = @Code,
								    Description = @Description								  
								   ,IsActive = @IsActive
								   ,UpdatedBy = @UpdatedBy
								   ,UpdateDate = GETDATE()
							WHERE Funding_Source_ID = @Funding_Source_ID
							AND System_ID = @System_ID
									
							SET @Status = 'OK';
							SET @Message = 'Funding Source updated successfully';		
							RETURN;
					--END
	END TRY
	BEGIN CATCH
	SET @Message = ERROR_MESSAGE();
				SELECT
				ERROR_NUMBER() AS ErrorNumber
				,ERROR_MESSAGE() AS ErrorMessage;
	END CATCH
END
