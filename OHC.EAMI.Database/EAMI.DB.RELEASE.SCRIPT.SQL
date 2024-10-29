USE [EAMI-RX]
GO

SET NOCOUNT ON  
EXEC master.dbo.sp_configure 'show advanced options', 1 
RECONFIGURE 
EXEC master.dbo.sp_configure 'xp_cmdshell', 1 
RECONFIGURE 

DECLARE @Release_Number VARCHAR (50) = '1.3.2.0'
DECLARE @Applied_Date_Time DATETIME = GetDate()
DECLARE @Script_Name VARCHAR (200) = 'EAMI Dental Release'
DECLARE @Script_Description VARCHAR (2000) = 'EAMI Dental Release'

DECLARE @DBServerName   VARCHAR(50) = 'localhost' 
DECLARE @DBName VARCHAR(50) = 'EAMI-Dental'  
DECLARE @ReleaseFolder   VARCHAR(200) = 'D:\EAMI\Release.1.3.2.0\Dental\'
DECLARE @UpdateScriptFileName   VARCHAR(100) = 'Release.1.3.2.0.sql' 


IF NOT EXISTS(SELECT NULL FROM [dbo].[TB_DB_UPDATE_SCRIPT] WHERE [Release_Number] = @Release_Number)
BEGIN
	BEGIN TRANSACTION

	BEGIN TRY		

		--RESULT MESSAGE
		CREATE TABLE #update_result_message (
			[Update_Type] VARCHAR(25),
			[Status] VARCHAR(25),
			[Script_Name] VARCHAR(500),
			[Message] VARCHAR(500)
		   )
		   
		DECLARE @update_type VARCHAR(25)

	
		-------------------------------------------------------------------------
		-- SCRIPTS
		-------------------------------------------------------------------------		
		IF LEN(@UpdateScriptFileName) > 0
		BEGIN
			SET @update_type = 'Scripts'
			DECLARE @UpdateScriptFolder   VARCHAR(300) = CONCAT(@ReleaseFolder, 'Scripts\') 
			DECLARE @UpdateScriptFilePath   VARCHAR(300) = CONCAT(@UpdateScriptFolder, @UpdateScriptFileName) 
			DECLARE @fileExists INT EXEC master.dbo.xp_fileexist  @UpdateScriptFilePath, @fileExists OUTPUT  
			
			IF @fileExists = 1
			BEGIN
				DECLARE @command  VARCHAR(600)  = 'sqlcmd -S ' + @DBServerName + ' -d  ' + @DBName + ' -i "' + @UpdateScriptFilePath +'"'  
				SET NOCOUNT ON
				EXEC xp_cmdshell  @command   , NO_OUTPUT
			END
			ELSE
			BEGIN
				DECLARE @error_msg  VARCHAR(300) = 'Release script file ' + @UpdateScriptFileName + ' does not exist at ' + @UpdateScriptFolder + ' release folder.'
				RAISERROR(@error_msg, 16,16)				
			END
		END
						
		
		-------------------------------------------------------------------------
		-- STORED PROCEDURES
		-------------------------------------------------------------------------
		SET @update_type = 'Procedures'
		DECLARE @Procedure_FileList TABLE (Files VARCHAR(100))  
		DECLARE @Procedure_Path VARCHAR(250) = @ReleaseFolder + 'Procedures\'

		 CREATE TABLE #pr_Files (
			Id int identity(1,1),
			[FileName] nvarchar(500),
			Depth smallint,
			FileFlag bit,
		   );

		INSERT INTO #pr_Files ([FileName], Depth, FileFlag) EXEC xp_dirtree @Procedure_Path, 1, 1

		WHILE (SELECT COUNT([FileName]) FROM #pr_Files ) > 0  
		BEGIN  
			--PREPARE EXECUTE COMMAND
			DECLARE @pr_cmd_result TABLE ([Message] NVARCHAR(512))
			DECLARE @pr_FileName VARCHAR(500) = (SELECT TOP(1) [FileName] FROM #pr_Files)  
			DECLARE @sp_command  VARCHAR(500)  = 'sqlcmd -S ' + @DBServerName + ' -d  ' + @DBName + ' -i "' + @Procedure_Path + @pr_FileName +'"'  
			SET NOCOUNT ON
			
			--EXECUTE COMMAND
			INSERT INTO @pr_cmd_result EXEC xp_cmdshell  @sp_command 

			--DETERMINE RESULT 
			DECLARE @pr_result_message varchar (500) =  (SELECT stuff((select  ',' + [Message] from @pr_cmd_result WHERE [Message] is NOT NULL and [Message] NOT LIKE 'Changed database context to%' for xml path('')),1,1,'') Concats)
			DECLARE @pr_Update_Status varchar (25) = IIF(@pr_result_message IS NULL,'SUCCESS', 'FAIL')
			
			--CAPTURE RESULT
			INSERT INTO #update_result_message VALUES (
				@update_type,
				@pr_Update_Status,
				@pr_FileName,
				@pr_result_message
				)
				
			--CLEAN UP
			DELETE FROM #pr_Files WHERE [FileName] = @pr_FileName 
			DELETE FROM @pr_cmd_result
		END 

		-------------------------------------------------------------------------
		-- FUNCTIONS 
		-------------------------------------------------------------------------
		SET @update_type = 'Functions'
		DECLARE @Function_FileList TABLE (Files VARCHAR(100))  
		DECLARE @Functions_Path VARCHAR(250) = @ReleaseFolder + 'Functions\'

		 CREATE TABLE #fn_Files (
			Id int identity(1,1),
			[FileName] nvarchar(100),
			Depth smallint,
			FileFlag bit,
		   );
		   
		INSERT INTO #fn_Files ([FileName], Depth, FileFlag) EXEC xp_dirtree @Functions_Path, 1, 1

		WHILE (SELECT COUNT([FileName]) FROM #fn_Files ) > 0  
		BEGIN  
			--PREPARE EXECUTE COMMAND
			DECLARE @fn_cmd_result TABLE ([Message] NVARCHAR(512))
			DECLARE @fn_fileName VARCHAR(500) = (SELECT TOP(1) [FileName] FROM #fn_Files)  
			DECLARE @fn_command  VARCHAR(500)  = 'sqlcmd -S ' + @DBServerName + ' -d  ' + @DBName + ' -i "' + @Functions_Path + @fn_fileName +'"'  
			SET NOCOUNT ON
			
			--EXECUTE COMMAND
			INSERT INTO @fn_cmd_result EXEC xp_cmdshell  @fn_command  
			
			--DETERMINE RESULT 
			DECLARE @fn_result_message varchar (500) =  (SELECT stuff((select  ',' + [Message] from @fn_cmd_result WHERE [Message] is NOT NULL and [Message] NOT LIKE 'Changed database context to%' for xml path('')),1,1,'') Concats)
			DECLARE @fn_Update_Status varchar (25) = IIF(@fn_result_message IS NULL,'SUCCESS', 'FAIL')
			
			--CAPTURE RESULT
			INSERT INTO #update_result_message VALUES (
				@update_type,
				@fn_Update_Status,
				@fn_FileName,
				@fn_result_message
				)

			DELETE FROM #fn_Files WHERE [FileName] = @fn_fileName  
			DELETE FROM @fn_cmd_result
		END 

		-------------------------------------------------------------------------
		--SHOW RESULT
		-------------------------------------------------------------------------

		DECLARE @update_Status varchar (25) = IIF(EXISTS(SELECT NULL FROM #update_result_message WHERE  [Status] = 'FAIL'),'FAIL','SUCCESS')

		DROP TABLE #pr_Files
		DROP TABLE #fn_Files

		SELECT
			@Release_Number as [Release_Number],
			@Script_Name as [Script_Name],
			@Applied_Date_Time as [Execution_Date_Time],
			@update_Status as [Update_Status]



		-------------------------------------------------------------------------
		-- FINALIZE 
		-------------------------------------------------------------------------
		IF (@update_Status = 'FAIL' )
		BEGIN
			SELECT * FROM #update_result_message WHERE [Status] = 'FAIL'
			DROP TABLE #update_result_message

			ROLLBACK TRANSACTION;
		END
		ELSE
		BEGIN
			--SUCCESS. CAPTURE RELEASE 
			INSERT INTO [dbo].[TB_DB_UPDATE_SCRIPT] ([Release_Number], [Applied_Date_Time], [Script_Name], [Script_Description]) VALUES (@Release_Number, @Applied_Date_Time, @Script_Name, @Script_Description)
			
			DROP TABLE #update_result_message
			
			COMMIT TRAN
		END

	END TRY
	BEGIN CATCH

		print 'EAMI - Error occured, rollback changes';
		ROLLBACK TRANSACTION;
		THROW;

	END CATCH	

 
END
ELSE
BEGIN
	PRINT 'EAMI - Release script [' + @Release_Number + '] has already been applied';
END


EXEC master.dbo.sp_configure 'xp_cmdshell', 0 
RECONFIGURE 
EXEC master.dbo.sp_configure 'show advanced options', 0 
RECONFIGURE  
SET NOCOUNT OFF 