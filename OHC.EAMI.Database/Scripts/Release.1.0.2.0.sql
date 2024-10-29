USE EAMI-PRX
GO


--CHANGE SCRIPT GOES BELOW

--Add TB_DB_UPDATE_SCRIPT table to keep track of furure database changes
	BEGIN TRAN

	BEGIN TRY	
		IF (NOT EXISTS (SELECT * 
						 FROM INFORMATION_SCHEMA.TABLES 
						 WHERE TABLE_SCHEMA = 'dbo' 
						 AND  TABLE_NAME = 'TB_DB_UPDATE_SCRIPT'))
		BEGIN
			CREATE TABLE [dbo].[TB_DB_UPDATE_SCRIPT](
				[DB_Update_Script_ID] [int] IDENTITY(1,1) NOT NULL,
				[Release_Number] [varchar](50) NOT NULL,
				[Applied_Date_Time] [datetime] NOT NULL,
				[Script_Name] [varchar](200) NOT NULL,
				[Script_Description] [varchar](2000) NOT NULL
			) ON [PRIMARY]
		END

		
		COMMIT TRAN

	END TRY
	BEGIN CATCH

		print 'EAMI - Error occured, rollback changes';
		ROLLBACK TRANSACTION;
		THROW;

	END CATCH	