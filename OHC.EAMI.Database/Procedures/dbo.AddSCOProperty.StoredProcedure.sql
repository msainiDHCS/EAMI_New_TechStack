USE [EAMI-MC]
GO

/****** Object:  StoredProcedure [dbo].[AddSCOProperty]    Script Date: 2/12/2024 10:51:38 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

/**********************************************************************************************************************************************************************************
* PROCEDURE:  [dbo].[AddSCOProperty]
* PURPOSE: inserts a new SCO Property in the system. Checks for duplicate SCO Property and returns an error.
* NOTES:
* CREATED: 2/01/2024  Gopal P.
* MODIFIED
* DATE		AUTHOR      DESCRIPTION
 3/16/2024  Meetu S.	Jira# EAMI-1354 - validation based on System, SCO Property Name, Fund, Payment Type and Environment so that there is no duplication for Property type “File”. 
*----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
-- exec AddSCOProperty
***********************************************************************************************************************************************************************************/
ALTER PROCEDURE [dbo].[AddSCOProperty]
	-- parameters for the stored procedure...	
	@SCO_Property_Name		varchar(50),	
	@SCO_Property_Value	varchar(50) ,
	@SCO_Property_Type_ID	Int	,
	@Description	varchar(100) = null,
	@Fund_ID			int,
	@System_ID			int,	
	@SCO_Property_Enum_ID			int,
	@Payment_Type			varchar(20),
	@Environment		varchar(20),
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
					Declare @Sort_value as tinyint = 0;
					--*********************NON-File properties section Begins **********************************
					if(@SCO_Property_Type_ID != 3) 
					Begin
						Set @Sort_value = (Select Isnull(max(Sort_Value),0) From dbo.TB_SCO_PROPERTY Where System_ID =@System_ID);

						IF (Select Count(1) From dbo.TB_SCO_PROPERTY Where ltrim(rtrim(lower(SCO_Property_Name))) = ltrim(rtrim(lower(@SCO_Property_Name))) 
							AND IsActive = 1 
							AND System_ID = @System_ID) > 0
						BEGIN
									SET @Status = 'ERROR';	
									Set @Message = 'Duplicate SCO Property  found';		
									RETURN;
						END;
					ELSE
					BEGIN
							INSERT INTO [dbo].[TB_SCO_PROPERTY]
								([SCO_Property_Name],
								[SCO_Property_Value],
								[SCO_Property_Type_ID],
								[Description],
								[System_ID],	
								[SCO_Property_Enum_ID],	
								[IsActive], 
								[Sort_Value], 
								[CreateDate],
								[CreatedBy], 
								[UpdateDate],
								[UpdatedBy])
							 VALUES
								   (@SCO_Property_Name
								   ,@SCO_Property_Value
								   ,@SCO_Property_Type_ID								   
								   ,@Description
								   ,@System_ID
								   ,@SCO_Property_Enum_ID
								   ,@IsActive
								   ,(select @Sort_value +1)
								   ,GETDATE()
								   ,@CreatedBy
								   ,NULL
								   ,NULL)
								   
								   
							--SET @Fund_ID = SCOPE_IDENTITY();
									
							SET @Status = 'OK';
							SET @Message = 'SCO Property created successfully';		
							RETURN;
					END
					End
					--*********************NON-File properties section Ends **********************************
					--********************* File properties section Begins **********************************
				else 
				BEGIN
					Set @Sort_value = (Select Isnull(max(Sort_Value),0) From dbo.TB_SCO_FILE_PROPERTY Where System_ID =@System_ID);

						IF (Select Count(1) From dbo.TB_SCO_FILE_PROPERTY Where ltrim(rtrim(lower(SCO_File_Property_Name))) = ltrim(rtrim(lower(@SCO_Property_Name))) 
							AND Environment = @Environment
							AND Payment_Type = @Payment_Type
							AND Fund_ID = @Fund_ID
							AND IsActive = 1 
							AND System_ID = @System_ID) > 0
						BEGIN
									SET @Status = 'ERROR';	
									Set @Message = 'Duplicate SCO File Property found';		
									RETURN;
						END;
						ELSE
						BEGIN
								INSERT INTO [dbo].[TB_SCO_FILE_PROPERTY]
									([SCO_File_Property_Name],
									[SCO_File_Property_Value],
									[SCO_Property_Type_ID],
									[Description],
									[System_ID],
									[Fund_ID],
									[SCO_Property_Enum_ID],	
									[Payment_Type],
									[Environment],
									[IsActive], 
									[Sort_Value], 
									[CreateDate],
									[CreatedBy], 
									[UpdateDate],
									[UpdatedBy])
								 VALUES
									   (@SCO_Property_Name
									   ,@SCO_Property_Value
									   ,@SCO_Property_Type_ID								   
									   ,@Description
									   ,@System_ID
									   ,@Fund_ID
									   ,@SCO_Property_Enum_ID
									   ,@Payment_Type
									   ,@Environment
									   ,@IsActive
									   ,(select @Sort_value +1)
									   ,GETDATE()
									   ,@CreatedBy
									   ,NULL
									   ,NULL)
								   
								   
								--SET @Fund_ID = SCOPE_IDENTITY();
									
								SET @Status = 'OK';
								SET @Message = 'SCO File Property created successfully';		
								RETURN;
						END
					END
					--********************* File properties section Ends **********************************
					
	END TRY
	BEGIN CATCH
	SET @Message = ERROR_MESSAGE();
				SELECT
				ERROR_NUMBER() AS ErrorNumber
				,ERROR_MESSAGE() AS ErrorMessage;
	END CATCH
END


