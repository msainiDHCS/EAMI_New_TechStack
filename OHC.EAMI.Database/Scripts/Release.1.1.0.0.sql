USE EAMI-PRX
GO


--CHANGE SCRIPT GOES BELOW

--Add new values to TB_DB_SETTING table
	BEGIN TRAN

	BEGIN TRY	

		--Claim Identifier for SCO_CLAIM_ID_FUND_0912_WARRANT
		IF NOT EXISTS(SELECT NULL FROM TB_DB_SETTING WHERE DB_Setting_Key = 'SCO_CLAIM_ID_FUND_0912_WARRANT')
		BEGIN
			INSERT INTO TB_DB_SETTING 
			VALUES (
				'SCO_CLAIM_ID_FUND_0912_WARRANT'
				,'DHCSMCMCW'
				,1
				,'SCO Claim Identifier for fund 0912 WARRANT'
				,1
				,0
				,getdate()
				,'system'
				,null
				,null)
		END

		--Claim Identifier for SCO_CLAIM_ID_FUND_0555_WARRANT
		IF NOT EXISTS(SELECT NULL FROM TB_DB_SETTING WHERE DB_Setting_Key = 'SCO_CLAIM_ID_FUND_0555_WARRANT')
		BEGIN
			INSERT INTO TB_DB_SETTING 
			VALUES (
				'SCO_CLAIM_ID_FUND_0555_WARRANT'
				,'DHCSMCMCHW'
				,1
				,'SCO Claim Identifier for fund 0555 WARRANT'
				,1
				,0
				,getdate()
				,'system'
				,null
				,null)
		END


		--Claim Identifier for SCO_CLAIM_ID_FUND_0912_EFT
		IF NOT EXISTS(SELECT NULL FROM TB_DB_SETTING WHERE DB_Setting_Key = 'SCO_CLAIM_ID_FUND_0912_EFT')
		BEGIN
			INSERT INTO TB_DB_SETTING 
			VALUES (
				'SCO_CLAIM_ID_FUND_0912_EFT'
				,'DHCSMCMCE'
				,1
				,'SCO Claim Identifier for fund 0912 EFT'
				,1
				,0
				,getdate()
				,'system'
				,null
				,null)
		END

		--Claim Identifier for SCO_CLAIM_ID_FUND_0555_EFT
		IF NOT EXISTS(SELECT NULL FROM TB_DB_SETTING WHERE DB_Setting_Key = 'SCO_CLAIM_ID_FUND_0555_EFT')
		BEGIN
			INSERT INTO TB_DB_SETTING 
			VALUES (
				'SCO_CLAIM_ID_FUND_0555_EFT'
				,'DHCSMCMCHE'
				,1
				,'SCO Claim Identifier for fund 0555 EFT'
				,1
				,0
				,getdate()
				,'system'
				,null
				,null)
		END
		
		COMMIT TRAN

	END TRY
	BEGIN CATCH

		print 'EAMI - Error occured, rollback changes';
		ROLLBACK TRANSACTION;
		THROW;

	END CATCH	