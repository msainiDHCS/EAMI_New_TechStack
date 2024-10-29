--EAMI SCRIPT Release.1.2.0.0 

/******************************************************************************
* PURPOSE: Script modifies schema AND executes data converssion
* NOTES:   Release.1.2.0.0
	There are 3 parts of scripts that are interdependant.
		1. EAMI DB SCHEMA CHANGES
		2. DATA CONVERSION
		3. POST DATA CONVERSION SCRIPT
		4. OTHER SCRIPTS

* CREATED: 8/10/2020  Genady G.
* MODIFIED
* DATE     AUTHOR      DESCRIPTION
*------------------------------------------------------------------------------
*****************************************************************************/

USE [EAMI-PRX]
GO

BEGIN TRY

SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON

/******************************************************
	1. EAMI DB SCHEMA CHANGES
*******************************************************/

-- CREATE TABLE TB_PEE_SYSTEM
IF NOT EXISTS(SELECT NULL FROM sys.tables 
          WHERE Object_ID = Object_ID(N'TB_PEE_SYSTEM'))
BEGIN
	CREATE TABLE [dbo].[TB_PEE_SYSTEM](
		[PEE_System_ID] [int] IDENTITY(1,1) NOT NULL,
		[Payment_Exchange_Entity_ID] [int] NOT NULL,
		[SOR_ID] [int] NOT NULL,
	 CONSTRAINT [PK_TB_PEE_SYSTEM] PRIMARY KEY CLUSTERED 
	(
		[PEE_System_ID] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]
	ALTER TABLE [dbo].[TB_PEE_SYSTEM]  WITH CHECK ADD  CONSTRAINT [FK_TB_PEE_SYSTEM_TB_PAYMENT_EXCHANGE_ENTITY] FOREIGN KEY([Payment_Exchange_Entity_ID])
	REFERENCES [dbo].[TB_PAYMENT_EXCHANGE_ENTITY] ([Payment_Exchange_Entity_ID])
	ALTER TABLE [dbo].[TB_PEE_SYSTEM] CHECK CONSTRAINT [FK_TB_PEE_SYSTEM_TB_PAYMENT_EXCHANGE_ENTITY]
	ALTER TABLE [dbo].[TB_PEE_SYSTEM]  WITH CHECK ADD  CONSTRAINT [FK_TB_PEE_SYSTEM_TB_SYSTEM_OF_RECORD] FOREIGN KEY([SOR_ID])
	REFERENCES [dbo].[TB_SYSTEM_OF_RECORD] ([SOR_ID])
	ALTER TABLE [dbo].[TB_PEE_SYSTEM] CHECK CONSTRAINT [FK_TB_PEE_SYSTEM_TB_SYSTEM_OF_RECORD]
END

-- CREATE TABLE TB_PEE_ADDRESS
IF NOT EXISTS(SELECT NULL FROM sys.tables 
          WHERE Object_ID = Object_ID(N'TB_PEE_ADDRESS'))
BEGIN
	CREATE TABLE [dbo].[TB_PEE_ADDRESS](
		[PEE_Address_ID] [int] IDENTITY(1,1) NOT NULL,
		[PEE_System_ID] [int] NOT NULL,
		[Address_Line1] [varchar](30) NOT NULL,
		[Address_Line2] [varchar](30) NULL,
		[Address_Line3] [varchar](30) NULL,
		[City] [varchar](27) NOT NULL,
		[State] [varchar](2) NOT NULL,
		[Zip] [varchar](10) NOT NULL,
		[ContractNumber] [varchar](20) NULL,
		[CreateDate] [datetime] NOT NULL,
		[Entity_Name] [varchar](100) NOT NULL,
	 CONSTRAINT [PK_TB_PEE_ADDRESS] PRIMARY KEY CLUSTERED 
	(
		[PEE_Address_ID] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]
	ALTER TABLE [dbo].[TB_PEE_ADDRESS] ADD  DEFAULT ('') FOR [Entity_Name]
	ALTER TABLE [dbo].[TB_PEE_ADDRESS]  WITH CHECK ADD  CONSTRAINT [FK_TB_PEE_ADDRESS_TB_PEE_SYSTEM] FOREIGN KEY([PEE_System_ID])
	REFERENCES [dbo].[TB_PEE_SYSTEM] ([PEE_System_ID])
	ALTER TABLE [dbo].[TB_PEE_ADDRESS] CHECK CONSTRAINT [FK_TB_PEE_ADDRESS_TB_PEE_SYSTEM]
END

-- CREATE TABLE [TB_PEE_EFT_INFO]
IF NOT EXISTS(SELECT NULL FROM sys.tables 
          WHERE Object_ID = Object_ID(N'TB_PEE_EFT_INFO'))
BEGIN
	CREATE TABLE [dbo].[TB_PEE_EFT_INFO](
		[PEE_EFT_Info_ID] [int] IDENTITY(1,1) NOT NULL,
		[PEE_System_ID] [int] NOT NULL,
		[FIRoutingNumber] [varchar](20) NOT NULL,
		[FIAccountType] [varchar](20) NOT NULL,
		[PrvAccountNo] [varchar](20) NOT NULL,
		[DatePrenoted] [datetime] NOT NULL,
		[CreateDate] [datetime] NOT NULL,
	 CONSTRAINT [PK_TB_PEE_EFT_INFO] PRIMARY KEY CLUSTERED 
	(
		[PEE_EFT_Info_ID] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]
	ALTER TABLE [dbo].[TB_PEE_EFT_INFO]  WITH CHECK ADD  CONSTRAINT [FK_TB_PEE_EFT_INFO_TB_PEE_SYSTEM] FOREIGN KEY([PEE_System_ID])
	REFERENCES [dbo].[TB_PEE_SYSTEM] ([PEE_System_ID])
	ALTER TABLE [dbo].[TB_PEE_EFT_INFO] CHECK CONSTRAINT [FK_TB_PEE_EFT_INFO_TB_PEE_SYSTEM]
END

--ADD FIELD [PEE_Address_ID] to [TB_CLAIM_SCHEDULE]
IF NOT EXISTS(SELECT NULL FROM sys.columns 
          WHERE Name = N'PEE_Address_ID'
          AND Object_ID = Object_ID(N'TB_CLAIM_SCHEDULE'))
BEGIN
	ALTER TABLE [dbo].[TB_CLAIM_SCHEDULE] ADD [PEE_Address_ID] INT NOT NULL DEFAULT 0; 	
END

--ADD FIELD [PEE_Address_ID] to [TB_PAYMENT_RECORD]
IF NOT EXISTS(SELECT NULL FROM sys.columns 
          WHERE Name = N'PEE_Address_ID'
          AND Object_ID = Object_ID(N'TB_PAYMENT_RECORD'))
BEGIN
	ALTER TABLE [dbo].[TB_PAYMENT_RECORD] ADD [PEE_Address_ID] INT NOT NULL DEFAULT 0; 	
END

--ADD FIELD [PEE_EFT_Info_ID] to [TB_PAYMENT_RECORD]
IF NOT EXISTS(SELECT NULL FROM sys.columns 
          WHERE Name = N'PEE_EFT_Info_ID'
          AND Object_ID = Object_ID(N'TB_PAYMENT_RECORD'))
BEGIN
	ALTER TABLE [dbo].[TB_PAYMENT_RECORD] ADD [PEE_EFT_Info_ID] INT NULL; 
	ALTER TABLE [dbo].[TB_PAYMENT_RECORD]  WITH CHECK ADD  CONSTRAINT [FK_TB_PAYMENT_RECORD_TB_PEE_EFT_Info] FOREIGN KEY([PEE_EFT_Info_ID])
	REFERENCES [dbo].[TB_PEE_EFT_INFO] ([PEE_EFT_Info_ID])			
	ALTER TABLE [dbo].[TB_PAYMENT_RECORD] CHECK CONSTRAINT [FK_TB_PAYMENT_RECORD_TB_PEE_EFT_Info]	
END

--ADD FIELD [PEE_EFT_Info_ID] to [TB_CLAIM_SCHEDULE]
IF NOT EXISTS(SELECT NULL FROM sys.columns 
          WHERE Name = N'PEE_EFT_Info_ID'
          AND Object_ID = Object_ID(N'TB_CLAIM_SCHEDULE'))
BEGIN
	ALTER TABLE [dbo].[TB_CLAIM_SCHEDULE] ADD [PEE_EFT_Info_ID] INT NULL; 	
	ALTER TABLE [dbo].[TB_CLAIM_SCHEDULE]  WITH CHECK ADD  CONSTRAINT [FK_TB_CLAIM_SCHEDULE_TB_PEE_EFT_Info] FOREIGN KEY([PEE_EFT_Info_ID])
	REFERENCES [dbo].[TB_PEE_EFT_INFO] ([PEE_EFT_Info_ID])			
	ALTER TABLE [dbo].[TB_CLAIM_SCHEDULE] CHECK CONSTRAINT [FK_TB_CLAIM_SCHEDULE_TB_PEE_EFT_Info]
END

--ADD FIELD [Payment_Exchange_Entity_Info_ID] to [TB_PEE_ADDRESS]
--This field is for data conversion purpose.
IF NOT EXISTS(SELECT NULL FROM sys.columns 
          WHERE Name = N'Payment_Exchange_Entity_Info_ID'
          AND Object_ID = Object_ID(N'TB_PEE_ADDRESS'))
BEGIN
	ALTER TABLE [dbo].[TB_PEE_ADDRESS] ADD [Payment_Exchange_Entity_Info_ID] INT  NULL; 	
END

 
	/******************************************************
		2. DATA CONVERSION
	*******************************************************/
	WAITFOR DELAY '00:00:30'
	DECLARE @system_code varchar (100) = 'CAPMAN'
	DECLARE @system_of_record_id int
	DECLARE @run_data_converssion BIT

	--Determine system code
	SET @system_of_record_id = (SELECT TOP 1 [SOR_ID] FROM dbo.[TB_SYSTEM_OF_RECORD] WHERE [Code] = @system_code)

	IF   (@system_of_record_id IS NOT NULL) 
	BEGIN	
		--SAFETY CHECK
		--Determine if data has already been migrated 
		SET @run_data_converssion = IIF (EXISTS(
		SELECT NULL FROM TB_PEE_ADDRESS PEEA
			INNER JOIN TB_PEE_SYSTEM PEES ON PEEA.PEE_System_ID = PEES.PEE_System_ID
		WHERE PEES.SOR_ID = @system_of_record_id
		),0,1)
				
		IF (@run_data_converssion = 1)
		BEGIN	
			--POPULATE TB_PEE_SYSTEM
			INSERT INTO dbo.[TB_PEE_SYSTEM]
			SELECT 
				Payment_Exchange_Entity_ID
				, @System_of_Record_ID 
			FROM dbo.[TB_PAYMENT_EXCHANGE_ENTITY]

			--POPULATE TB_PEE_ADDRESS
			INSERT INTO dbo.[TB_PEE_ADDRESS]
			SELECT 
				PEES.PEE_System_ID
				,PEEI.Entity_Address_Line1
				,PEEI.Entity_Address_Line2
				,PEEI.Entity_Address_Line3
				,PEEI.Entity_City
				,PEEI.Entity_State
				,PEEI.Entity_Zip
				,PEEI.Entity_ContractNumber
				,PEEI.CreateDate
				,PEEI.[Entity_Name]
				,PEEI.[Payment_Exchange_Entity_Info_ID]
			FROM dbo.[TB_PAYMENT_EXCHANGE_ENTITY_INFO] PEEI
				INNER JOIN dbo.[TB_PAYMENT_EXCHANGE_ENTITY] PEI ON PEEI.Payment_Exchange_Entity_ID = PEI.Payment_Exchange_Entity_ID
				INNER JOIN dbo.[TB_PEE_SYSTEM] PEES ON PEEI.Payment_Exchange_Entity_ID = PEES.Payment_Exchange_Entity_ID
			
			--UPDATE PEE_Address_ID field in TB_PAYMENT_RECORD table
			UPDATE PR 
			SET 
				PR.PEE_Address_ID = PEEA.PEE_Address_ID
			FROM dbo.[TB_PAYMENT_RECORD] AS PR
				INNER JOIN  dbo.[TB_PEE_ADDRESS] PEEA ON PR.[Payee_Entity_Info_ID] = PEEA.[Payment_Exchange_Entity_Info_ID]

			--UPDATE PEE_Address_ID field in TB_CLAIM_SCHEDULE table
			UPDATE CS 
			SET 
				CS.[PEE_Address_ID] = PEEA.[PEE_Address_ID]
			FROM dbo.[TB_CLAIM_SCHEDULE] AS CS
				INNER JOIN  dbo.[TB_PEE_ADDRESS] PEEA ON CS.Payee_Entity_Info_ID = PEEA.[Payment_Exchange_Entity_Info_ID]
		
			--DROP TEMP FIELD IN TB_PEE_ADDRESS table
			IF EXISTS(SELECT NULL FROM sys.columns 
			  WHERE Name = N'Payment_Exchange_Entity_Info_ID' AND Object_ID = Object_ID(N'TB_PEE_ADDRESS'))
			BEGIN
				ALTER TABLE [dbo].[TB_PEE_ADDRESS] DROP COLUMN [Payment_Exchange_Entity_Info_ID]; 	
			END 

			/******************************************************
				3. POST DATA CONVERSION SCRIPT
			******************************************************/
			
			--ADD PEE_Address_ID FOREIGN KEY TO [TB_CLAIM_SCHEDULE]
			IF NOT EXISTS(SELECT NULL FROM sys.foreign_keys WHERE Name = N'FK_TB_CLAIM_SCHEDULE_TB_PEE_ADDRESS')
			BEGIN
				ALTER TABLE [dbo].[TB_CLAIM_SCHEDULE]  WITH CHECK ADD  CONSTRAINT [FK_TB_CLAIM_SCHEDULE_TB_PEE_ADDRESS] FOREIGN KEY([PEE_Address_ID])
				REFERENCES [dbo].[TB_PEE_ADDRESS] ([PEE_Address_ID])			
				ALTER TABLE [dbo].[TB_CLAIM_SCHEDULE] CHECK CONSTRAINT [FK_TB_CLAIM_SCHEDULE_TB_PEE_ADDRESS]
			END
			
			--ADD PEE_Address_ID FOREIGN KEY TO TB_PAYMENT_RECORD
			IF NOT EXISTS(SELECT NULL FROM sys.foreign_keys WHERE Name = N'FK_TB_PAYMENT_RECORD_TB_PEE_ADDRESS')
			BEGIN
				ALTER TABLE [dbo].[TB_PAYMENT_RECORD]  WITH CHECK ADD  CONSTRAINT [FK_TB_PAYMENT_RECORD_TB_PEE_ADDRESS] FOREIGN KEY([PEE_Address_ID])
				REFERENCES [dbo].[TB_PEE_ADDRESS] ([PEE_Address_ID])			
				ALTER TABLE [dbo].[TB_PAYMENT_RECORD] CHECK CONSTRAINT [FK_TB_PAYMENT_RECORD_TB_PEE_ADDRESS]
			END


			/******************************************************
				4. OTHER SCRIPTS
			******************************************************/
			
			--TB_CLAIM_SCHEDULE MAKE Payee_Entity_Info_ID NULLABLE
			IF EXISTS(SELECT NULL FROM sys.columns 
				WHERE Name = N'Payee_Entity_Info_ID' AND Object_ID = Object_ID(N'TB_CLAIM_SCHEDULE'))
			BEGIN
				ALTER TABLE TB_CLAIM_SCHEDULE ALTER COLUMN Payee_Entity_Info_ID INT NULL 	
			END 
			
			--TB_PAYMENT_RECORD MAKE Payee_Entity_Info_ID NULLABLE
			IF EXISTS(SELECT NULL FROM sys.columns 
				WHERE Name = N'Payee_Entity_Info_ID' AND Object_ID = Object_ID(N'TB_PAYMENT_RECORD'))
			BEGIN
				ALTER TABLE TB_PAYMENT_RECORD ALTER COLUMN Payee_Entity_Info_ID INT NULL
			END

			--ADD EXCLUSIVE PAYMENT TYPES
			
			DECLARE @dt DATETIME = GETDATE()
			IF NOT EXISTS(SELECT NULL FROM [dbo].[TB_EXCLUSIVE_PAYMENT_TYPE] WHERE Code = 'PCFH')
			BEGIN
				INSERT INTO [dbo].[TB_EXCLUSIVE_PAYMENT_TYPE] VALUES  ('PCFH', 'Payment Type associated to fund 0001', 1, 9, @dt, 'SYSTEM', NULL, NULL)
			END

			IF NOT EXISTS(SELECT NULL FROM [dbo].[TB_EXCLUSIVE_PAYMENT_TYPE] WHERE Code = 'EWC')
			BEGIN
				INSERT INTO [dbo].[TB_EXCLUSIVE_PAYMENT_TYPE] VALUES  ('EWC', 'Payment Type associated to fund 0236', 1, 10, @dt, 'SYSTEM', NULL, NULL)
			END
   
			--ADD CLAIM FUND TO [TB_DB_SETTING]
			
			IF NOT EXISTS(SELECT NULL FROM [dbo].[TB_DB_SETTING] WHERE [DB_Setting_Key] = 'SCO_CLAIM_ID_FUND_0912_EFT')
			BEGIN
					INSERT INTO [dbo].[TB_DB_SETTING] VALUES ('SCO_CLAIM_ID_FUND_0912_EFT', 'DHCSMEDRXE', 1, 'SCO Claim Identifier for fund 0912 EFT', 1, 0, getdate(), 'system', null, null)
			END
			
			IF NOT EXISTS(SELECT NULL FROM [dbo].[TB_DB_SETTING] WHERE [DB_Setting_Key] = 'SCO_CLAIM_ID_FUND_0555_EFT')
			BEGIN
					INSERT INTO [dbo].[TB_DB_SETTING] VALUES ('SCO_CLAIM_ID_FUND_0555_EFT', 'DHCSMEDRXH', 1, 'SCO Claim Identifier for fund 0555 EFT', 1, 0, getdate(), 'system', null, null)
			END

			IF NOT EXISTS(SELECT NULL FROM [dbo].[TB_DB_SETTING] WHERE [DB_Setting_Key] = 'SCO_CLAIM_ID_FUND_0236_EFT')
			BEGIN
					INSERT INTO [dbo].[TB_DB_SETTING] VALUES ('SCO_CLAIM_ID_FUND_0236_EFT', 'DHCSMEDRXT', 1, 'SCO Claim Identifier for fund 0236 EFT', 1, 0, getdate(), 'system', null, null)
			END

			IF NOT EXISTS(SELECT NULL FROM [dbo].[TB_DB_SETTING] WHERE [DB_Setting_Key] = 'SCO_CLAIM_ID_FUND_0001_EFT')
			BEGIN 
					INSERT INTO [dbo].[TB_DB_SETTING] VALUES ('SCO_CLAIM_ID_FUND_0001_EFT', 'DHCSMEDRXG', 1, 'SCO Claim Identifier for fund 0001 EFT', 1, 0, getdate(), 'system', null, null)
			END

			--ADD FILE NAME PER FUND TO [TB_DB_SETTING]
			
			--PROD
			--PROD EFT 0912
			IF NOT EXISTS(SELECT NULL FROM [dbo].[TB_DB_SETTING] WHERE [DB_Setting_Key] = 'EFT_FILE_NAME_FUND_0912_PROD')
			BEGIN
					INSERT INTO [dbo].[TB_DB_SETTING] VALUES ('EFT_FILE_NAME_FUND_0912_PROD', 'D{0}.MRXE.INPUT', 1, 'EFT Prod file name for fund 0912', 1, 0, getdate(), 'system', null, null)
			END

			--PROD EFT 0555
			IF NOT EXISTS(SELECT NULL FROM [dbo].[TB_DB_SETTING] WHERE [DB_Setting_Key] = 'EFT_FILE_NAME_FUND_0555_PROD')
			BEGIN
					INSERT INTO [dbo].[TB_DB_SETTING] VALUES ('EFT_FILE_NAME_FUND_0555_PROD', 'D{0}.MRXH.INPUT', 1, 'EFT Prod file name for fund 0555', 1, 0, getdate(), 'system', null, null)
			END

			--PROD EFT 0236
			IF NOT EXISTS(SELECT NULL FROM [dbo].[TB_DB_SETTING] WHERE [DB_Setting_Key] = 'EFT_FILE_NAME_FUND_0236_PROD')
			BEGIN
					INSERT INTO [dbo].[TB_DB_SETTING] VALUES ('EFT_FILE_NAME_FUND_0236_PROD', 'D{0}.MRXT.INPUT', 1, 'EFT Prod file name for fund 0236', 1, 0, getdate(), 'system', null, null)
			END

			--PROD EFT 0001
			IF NOT EXISTS(SELECT NULL FROM [dbo].[TB_DB_SETTING] WHERE [DB_Setting_Key] = 'EFT_FILE_NAME_FUND_0001_PROD')
			BEGIN
					INSERT INTO [dbo].[TB_DB_SETTING] VALUES ('EFT_FILE_NAME_FUND_0001_PROD', 'DHCS.D{0}.MRXG.INPUT', 1, 'EFT Prod file name for fund 0001', 1, 0, getdate(), 'system', null, null)
			END

			--PROD DEX 0236
			IF NOT EXISTS(SELECT NULL FROM [dbo].[TB_DB_SETTING] WHERE [DB_Setting_Key] = 'DEX_FILE_NAME_FUND_0236_PROD')
			BEGIN
					INSERT INTO [dbo].[TB_DB_SETTING] VALUES ('DEX_FILE_NAME_FUND_0236_PROD', 'PD.CFIS.FTP.DHCS.D{0}.MRXT.DEX', 1, 'DEX Prod file name for fund 0236', 1, 0, getdate(), 'system', null, null)
			END

			--PROD DEX 0001
			IF NOT EXISTS(SELECT NULL FROM [dbo].[TB_DB_SETTING] WHERE [DB_Setting_Key] = 'DEX_FILE_NAME_FUND_0001_PROD')
			BEGIN
					INSERT INTO [dbo].[TB_DB_SETTING] VALUES ('DEX_FILE_NAME_FUND_0001_PROD', 'PD.CFIS.FTP.DHCS.D{0}.MRXG.DEX', 1, 'DEX Prod file name for fund 0001', 1, 0, getdate(), 'system', null, null)
			END

			--PROD DEX 0912
			IF NOT EXISTS(SELECT NULL FROM [dbo].[TB_DB_SETTING] WHERE [DB_Setting_Key] = 'DEX_FILE_NAME_FUND_0912_PROD')
			BEGIN
					INSERT INTO [dbo].[TB_DB_SETTING] VALUES ('DEX_FILE_NAME_FUND_0912_PROD', 'PD.CFIS.FTP.DHCS.D{0}.MRXE.DEX', 1, 'DEX Prod file name for fund 0912', 1, 0, getdate(), 'system', null, null)
			END
			ELSE
			BEGIN
					UPDATE [dbo].[TB_DB_SETTING] SET 
						[DB_Setting_Value] = 'PD.CFIS.FTP.DHCS.D{0}.MRXE.DEX'
						, [UpdateDate] = getdate() 
						, [UpdatedBy] = 'system'
					WHERE [DB_Setting_Key] = 'DEX_FILE_NAME_FUND_0912_PROD'
			END

			--PROD DEX 0555
			IF NOT EXISTS(SELECT NULL FROM [dbo].[TB_DB_SETTING] WHERE [DB_Setting_Key] = 'DEX_FILE_NAME_FUND_0555_PROD')
			BEGIN
					INSERT INTO [dbo].[TB_DB_SETTING] VALUES ('DEX_FILE_NAME_FUND_0555_PROD', 'PD.CFIS.FTP.DHCS.D{0}.MRXH.DEX', 1, 'DEX Prod file name for fund 0555', 1, 0, getdate(), 'system', null, null)
			END
			ELSE
			BEGIN
					UPDATE [dbo].[TB_DB_SETTING] SET 
						[DB_Setting_Value] = 'PD.CFIS.FTP.DHCS.D{0}.MRXH.DEX'
						, [UpdateDate] = getdate() 
						, [UpdatedBy] = 'system'
					WHERE [DB_Setting_Key] = 'DEX_FILE_NAME_FUND_0555_PROD'
			END

			--TEST
			--TEST EFT 0912
			IF NOT EXISTS(SELECT NULL FROM [dbo].[TB_DB_SETTING] WHERE [DB_Setting_Key] = 'EFT_FILE_NAME_FUND_0912_TEST')
			BEGIN
					INSERT INTO [dbo].[TB_DB_SETTING] VALUES ('EFT_FILE_NAME_FUND_0912_TEST', 'D{0}.MRXE.EFT', 1, 'EFT Test file name for fund 0912', 1, 0, getdate(), 'system', null, null)
			END

			--TEST EFT 0555
			IF NOT EXISTS(SELECT NULL FROM [dbo].[TB_DB_SETTING] WHERE [DB_Setting_Key] = 'EFT_FILE_NAME_FUND_0555_TEST')
			BEGIN
					INSERT INTO [dbo].[TB_DB_SETTING] VALUES ('EFT_FILE_NAME_FUND_0555_TEST', 'D{0}.MRXH.EFT', 1, 'EFT Test file name for fund 0555', 1, 0, getdate(), 'system', null, null)
			END

			--TEST EFT 0236
			IF NOT EXISTS(SELECT NULL FROM [dbo].[TB_DB_SETTING] WHERE [DB_Setting_Key] = 'EFT_FILE_NAME_FUND_0236_TEST')
			BEGIN
					INSERT INTO [dbo].[TB_DB_SETTING] VALUES ('EFT_FILE_NAME_FUND_0236_TEST', 'D{0}.MRXT.EFT', 1, 'EFT Test file name for fund 0236', 1, 0, getdate(), 'system', null, null)
			END

			--TEST EFT 0001
			IF NOT EXISTS(SELECT NULL FROM [dbo].[TB_DB_SETTING] WHERE [DB_Setting_Key] = 'EFT_FILE_NAME_FUND_0001_TEST')
			BEGIN
					INSERT INTO [dbo].[TB_DB_SETTING] VALUES ('EFT_FILE_NAME_FUND_0001_TEST', 'D{0}.MRXG.EFT', 1, 'EFT Test file name for fund 0001', 1, 0, getdate(), 'system', null, null)
			END

			--TEST DEX 0236
			IF NOT EXISTS(SELECT NULL FROM [dbo].[TB_DB_SETTING] WHERE [DB_Setting_Key] = 'DEX_FILE_NAME_FUND_0236_TEST')
			BEGIN
					INSERT INTO [dbo].[TB_DB_SETTING] VALUES ('DEX_FILE_NAME_FUND_0236_TEST', 'CO.ELECCLMS.FTP.D{0}.MRXT.DEX', 1, 'DEX Test file name for fund 0236', 1, 0, getdate(), 'system', null, null)
			END

			--TEST DEX 0001
			IF NOT EXISTS(SELECT NULL FROM [dbo].[TB_DB_SETTING] WHERE [DB_Setting_Key] = 'DEX_FILE_NAME_FUND_0001_TEST')
			BEGIN
					INSERT INTO [dbo].[TB_DB_SETTING] VALUES ('DEX_FILE_NAME_FUND_0001_TEST', 'CO.ELECCLMS.FTP.D{0}.MRXG.DEX', 1, 'DEX Test file name for fund 0001', 1, 0, getdate(), 'system', null, null)
			END	
			
			--TEST DEX 0912
			IF NOT EXISTS(SELECT NULL FROM [dbo].[TB_DB_SETTING] WHERE [DB_Setting_Key] = 'DEX_FILE_NAME_FUND_0912_TEST')
			BEGIN
					INSERT INTO [dbo].[TB_DB_SETTING] VALUES ('DEX_FILE_NAME_FUND_0912_TEST', 'CO.ELECCLMS.FTP.D{0}.MRXE.DEX', 1, 'DEX Test file name for fund 0912', 1, 0, getdate(), 'system', null, null)
			END
			ELSE
			BEGIN
					UPDATE [dbo].[TB_DB_SETTING] SET 
						[DB_Setting_Value] = 'CO.ELECCLMS.FTP.D{0}.MRXE.DEX'
						, [UpdateDate] = getdate() 
						, [UpdatedBy] = 'system'
					WHERE [DB_Setting_Key] = 'DEX_FILE_NAME_FUND_0912_TEST'
			END

			--TEST DEX 0555
			IF NOT EXISTS(SELECT NULL FROM [dbo].[TB_DB_SETTING] WHERE [DB_Setting_Key] = 'DEX_FILE_NAME_FUND_0555_TEST')
			BEGIN
					INSERT INTO [dbo].[TB_DB_SETTING] VALUES ('DEX_FILE_NAME_FUND_0555_TEST', 'CO.ELECCLMS.FTP.D{0}.MRXH.DEX', 1, 'DEX Test file name for fund 0555', 1, 0, getdate(), 'system', null, null)
			END
			ELSE
			BEGIN
					UPDATE [dbo].[TB_DB_SETTING] SET 
						[DB_Setting_Value] = 'CO.ELECCLMS.FTP.D{0}.MRXH.DEX'
						, [UpdateDate] = getdate() 
						, [UpdatedBy] = 'system'
					WHERE [DB_Setting_Key] = 'DEX_FILE_NAME_FUND_0555_TEST'
			END
					
		END
	END
END TRY

BEGIN CATCH
	DECLARE @ErrorMessage NVARCHAR(4000)
	DECLARE @ErrorSeverity INT
	DECLARE @ErrorState INT

	SET @ErrorMessage = error_message()
	SET @ErrorSeverity = error_severity()
	SET @ErrorState = error_state()

	RAISERROR (
			@ErrorMessage
			,@ErrorSeverity
			,@ErrorState
			)
END CATCH