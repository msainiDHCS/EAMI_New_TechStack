USE [EAMI-MC]
GO
/****** Object:  StoredProcedure [dbo].[AddFacesheetValues]    Script Date: 10/27/2023 12:51:42 PM ******/

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AddFacesheetValues]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[AddFacesheetValues]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/******************************************************************************
* PROCEDURE:  [dbo].[AddFacesheetValues]
* PURPOSE: inserts a new FS value in the system.
* NOTES:
* CREATED: 12/12/2023  Meetu S.
* MODIFIED
* DATE		AUTHOR      DESCRIPTION
*------------------------------------------------------------------------------
*****************************************************************************/
CREATE PROCEDURE [dbo].[AddFacesheetValues]
	-- parameters for the stored procedure...	
	@Fund_ID				int,
	@System_ID				int,
	@Chapter				nvarchar(50),
	@Reference_Item			nvarchar(100),
	@Program				nvarchar(50) = null,
	@Element				nvarchar(50) = null,
	@Description			nvarchar(100),
	@IsActive				bit,
	@CreatedBy				varchar(20),
	@Status					varchar(10) OUTPUT,
	@Message				varchar(max) OUTPUT	
AS
BEGIN
	
	SET NOCOUNT ON;

	BEGIN TRY	

					Set @Status = 'ERROR';
					Set @Message = '';
					
						BEGIN
							INSERT INTO [dbo].[TB_FACESHEET]
										   ([Fund_ID]
										   ,[Chapter]
										   ,[Reference_Item]
										   ,[Program]
										   ,[Element]
										   ,[Description]
										   ,[CreateDate]
										   ,[CreatedBy]
										   ,[UpdateDate]
										   ,[UpdatedBy]
										   ,[IsActive])
									 VALUES
										   (@Fund_ID
										   ,@Chapter
										   ,@Reference_Item	
										   ,@Program
										   ,@Element
										   ,@Description
										   ,GETDATE()
										   ,@CreatedBy
										   ,GetDate()
										   ,NULL
										   ,1)	

								SET @Status = 'OK';
								SET @Message = 'Facesheet created successfully';		
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
