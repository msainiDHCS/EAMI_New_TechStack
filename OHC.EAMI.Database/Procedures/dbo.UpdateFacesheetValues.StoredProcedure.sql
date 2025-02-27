USE [EAMI-MC]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UpdateFacesheetValues]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[UpdateFacesheetValues]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/******************************************************************************
* PROCEDURE:  [dbo].[UpdateFacesheetValues]
* PURPOSE: updates an existing Facesheet record in the system.
* NOTES:
* CREATED: 12/11/2023  Meetu S.
* MODIFIED
* DATE		AUTHOR      DESCRIPTION
*------------------------------------------------------------------------------
*****************************************************************************/
CREATE PROCEDURE [dbo].[UpdateFacesheetValues]
	-- parameters for the stored procedure...	
	@Facesheet_ID			int,
	@Fund_ID				int,
	--@Fiscal_Year			varchar (50),
	@Chapter				nvarchar(50),
	@Reference_Item			nvarchar(100),
	@Program				nvarchar(50) = null,
	@Element				nvarchar(50) = null,
	@Description			nvarchar(100),
	@IsActive				bit,
	@UpdatedBy				varchar(20),
	@Status					varchar(10) OUTPUT,
	@Message				varchar(max) OUTPUT	
AS
BEGIN
	
	SET NOCOUNT ON;

	BEGIN TRY	
					Set @Status = 'ERROR';
					Set @Message = '';					
							UPDATE [dbo].[TB_FACESHEET]
							 SET
									--,Fiscal_Year = @Fiscal_Year	
									Chapter = @Chapter		
									,Reference_Item = @Reference_Item	
									,Program = @Program		
									,Element = @Element		
									,[Description] = @Description	
									,IsActive = @IsActive		
									,UpdatedBy = @UpdatedBy
								   ,UpdateDate = GETDATE()
							WHERE Facesheet_ID = @Facesheet_ID
							AND Fund_ID = @Fund_ID
									
							SET @Status = 'OK';
							SET @Message = 'Facesheet updated successfully';		
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
