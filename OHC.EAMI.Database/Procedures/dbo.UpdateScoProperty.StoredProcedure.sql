USE [EAMI-MC]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UpdateScoProperty]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[UpdateScoProperty]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*************************************************************************************************************************************************
* PROCEDURE:  [dbo].[UpdateScoProperty]
* PURPOSE: updates an existing sco property in the system. This SP is called on "Update" button click and "Delete" button click.
			Record will be updated based on the Property Type id, whether it's a File type property or otherwise.
* NOTES:
* CREATED: 02/06/2024  Meetu S.
* MODIFIED
* DATE		AUTHOR      DESCRIPTION
 3/16/2024  Meetu S.	Jira# EAMI-1354 - validation based on System, SCO Property Name, Fund, Payment Type and Environment so that there is no duplication for Property type “File”. 
*------------------------------------------------------------------------------------------------------------------------------------------------------
-- exec UpdateScoProperty 
**************************************************************************************************************************************************/
CREATE PROCEDURE [dbo].[UpdateScoProperty]
	-- parameters for the stored procedure...	
   @System_ID				INT
 , @SCO_Property_ID			INT
 , @SCO_Property_Type_ID	INT
 , @SCO_Property_Enum_ID	INT
 , @SCO_Property_Name		VARCHAR(100)
 , @Fund_ID					INT
 , @SCO_Property_Value		VARCHAR(100)
 , @Description				VARCHAR(100)
 , @EnvironmentText			VARCHAR(10)
 , @PaymentTypeText			VARCHAR(10)
 , @IsActive				bit
 , @UpdatedBy				varchar(20)
 , @Status					varchar(10) OUTPUT
 , @Message					varchar(max) OUTPUT
AS
BEGIN
	
	SET NOCOUNT ON;

	BEGIN TRY	
					Set @Status = 'ERROR';
					Set @Message = '';	
					

					IF(@SCO_Property_Type_ID = 3) -- update file property table
					BEGIN
						--  Check for Duplicate entries for Edit/Update functionality only. 
						--  Do not check duplication for delete where @IsActive = 0
						IF (@IsActive = 1) -- edit/update functionality
						BEGIN
							IF ((Select Count(1) From dbo.[TB_SCO_FILE_PROPERTY] 
							Where SCO_Property_Enum_ID = @SCO_Property_Enum_ID 
							AND Environment = @EnvironmentText
							AND Payment_Type = @PaymentTypeText
							AND Fund_ID = @Fund_ID
							AND System_ID = @System_ID 
							AND IsActive = 1) > 0)
							BEGIN
									Set @Status = 'ERROR';
									Set @Message = 'Duplicate SCO Property found.';		
									return;
							END;
						END

						UPDATE [dbo].[TB_SCO_FILE_PROPERTY]
						SET SCO_File_Property_Name = @SCO_Property_Name
						, SCO_File_Property_Value = @SCO_Property_Value
						, SCO_Property_Type_ID = @SCO_Property_Type_ID
						, SCO_Property_Enum_ID = @SCO_Property_Enum_ID
						, Fund_ID = @Fund_ID
						, Description = @Description
						, Payment_Type = @PaymentTypeText
						, Environment = @EnvironmentText
						, System_ID = @System_ID
						, IsActive = @IsActive
						, UpdatedBy = @UpdatedBy
						, UpdateDate = GetDate()
						WHERE SCO_File_Property_ID = @SCO_Property_ID

						SET @Status = 'OK';
							SET @Message = 'SCO File Property updated successfully';		
							RETURN;
					END;
					ELSE -- update non-file property table
					BEGIN
						--  Check for Duplicate entries for Edit/Update functionality only. 
						--  Do not check duplication for delete where @IsActive = 0
						IF (@IsActive = 1) -- edit/update functionality
						BEGIN
							IF ((Select Count(1) From dbo.[TB_SCO_PROPERTY] 
							Where SCO_Property_Name = @SCO_Property_Name 
							AND SCO_Property_Type_ID = @SCO_Property_Type_ID
							AND System_ID = @System_ID 
							AND IsActive = 1) > 0)
							BEGIN
									Set @Status = 'ERROR';
									Set @Message = 'Duplicate SCO Property found.';		
									return;
							END;
						END

						UPDATE [dbo].[TB_SCO_PROPERTY]
						SET SCO_Property_Name = @SCO_Property_Name
						, SCO_Property_Value = @SCO_Property_Value
						, SCO_Property_Type_ID = @SCO_Property_Type_ID
						, SCO_Property_Enum_ID = @SCO_Property_Enum_ID
						, Description = @Description
						, System_ID = @System_ID
						, IsActive = @IsActive
						, UpdatedBy = @UpdatedBy
						, UpdateDate = GetDate()
						WHERE SCO_Property_ID = @SCO_Property_ID

						SET @Status = 'OK';
							SET @Message = 'SCO Property updated successfully';		
							RETURN;
					END;
	END TRY
	BEGIN CATCH
	SET @Message = ERROR_MESSAGE();
				SELECT
				ERROR_NUMBER() AS ErrorNumber
				,ERROR_MESSAGE() AS ErrorMessage;
	END CATCH
END
