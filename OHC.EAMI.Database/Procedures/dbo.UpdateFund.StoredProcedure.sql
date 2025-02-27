USE [EAMI-MC]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UpdateFund]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[UpdateFund]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*************************************************************************************************************************************************
* PROCEDURE:  [dbo].[UpdateFund]
* PURPOSE: updates an existing fund record in the system. This SP is called on "Update" button click and "Delete" button click
* NOTES:
* CREATED: 10/27/2022  Meetu S.
* MODIFIED
* DATE		AUTHOR      DESCRIPTION
*------------------------------------------------------------------------------------------------------------------------------------------------------
-- exec UpdateFund 915, 'Testing From Code-BANK DEPOSIT FUND', 'BANK DEPOSIT FUND', 1,'UpdatedBy'
**************************************************************************************************************************************************/
CREATE  PROCEDURE [dbo].[UpdateFund]
	-- parameters for the stored procedure...	
	@Fund_ID			int,
	@System_ID			int,
	@Fund_Description	varchar(100) = null,
	@Stat_Year			varchar(4),
	@Fund_Name			varchar(50) = null,
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
					--Below condition checks if fund is already associated with EPT, it cannot be deleted. Can only be updated.
					IF ((Select Count(1) From dbo.TB_EXCLUSIVE_PAYMENT_TYPE Where Fund_ID = @Fund_ID AND IsActive = 1) > 0 AND @IsActive = 0)
					BEGIN
									Set @Status = 'ERROR';
									Set @Message = 'Fund cannot be deleted. It is associated with Exclusive Payment Type.';		
									return;
					END;
					--Below condition checks if fund is already associated with Facesheet, it cannot be deleted. Can only be updated.
					ELSE IF ((Select Count(1) From dbo.TB_FACESHEET Where Fund_ID = @Fund_ID AND IsActive = 1) > 0 AND @IsActive = 0)
					BEGIN
									Set @Status = 'ERROR';
									Set @Message = 'Fund cannot be deleted. It is associated with Facesheet.';		
									return;
					END;
					ELSE
					BEGIN

							UPDATE [dbo].[TB_FUND]
							 SET
								    Fund_Name = @Fund_Name
								   ,Fund_Description = @Fund_Description
								   ,Stat_Year = @Stat_Year
								   ,IsActive = @IsActive
								   ,UpdatedBy = @UpdatedBy
								   ,UpdateDate = GETDATE()
							WHERE Fund_ID = @Fund_ID
							AND System_ID = @System_ID									
							SET @Status = 'OK';
							SET @Message = 'Fund updated successfully';		
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
