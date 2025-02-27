USE [EAMI-MC]
GO

/****** Object:  StoredProcedure [dbo].[UpdateExclusivePmtType]    Script Date: 12/1/2023 1:17:56 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


/******************************************************************************
* PROCEDURE:  [dbo].[UpdateExclusivePmtType]
* PURPOSE: Updates an existing Exclusive Payment Type. Checks for duplicate Exclusive Payment Type code and returns an error.
* NOTES:
* CREATED: 11/02/2023 Gopal P.
* MODIFIED
* DATE		AUTHOR      DESCRIPTION
*------------------------------------------------------------------------------

*****************************************************************************/
CREATE PROCEDURE [dbo].[UpdateExclusivePmtType]		
	@Exclusive_Payment_Type_ID			int,
	@System_ID			int,
	@Exclusive_Payment_Type_Description	varchar(100) = null,
	@Exclusive_Payment_Type_Name			varchar(50) = null,
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
					
							UPDATE [dbo].[TB_EXCLUSIVE_PAYMENT_TYPE]
							 SET
								    Name = @Exclusive_Payment_Type_Name
								   ,Description = @Exclusive_Payment_Type_Description
								   ,IsActive = @IsActive								   
								   ,UpdatedBy = @UpdatedBy
								   ,UpdateDate = GETDATE()
							WHERE Exclusive_Payment_Type_ID = @Exclusive_Payment_Type_ID
							AND System_ID = @System_ID
	
						
							SET @Status = 'OK';
							SET @Message = 'Exclusive Payment Type updated successfully';		
							RETURN;
					
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
GO


