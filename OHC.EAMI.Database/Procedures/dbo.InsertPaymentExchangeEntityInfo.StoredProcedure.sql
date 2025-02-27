USE [EAMI-MC]
GO
/****** Object:  StoredProcedure [dbo].[InsertPaymentExchangeEntityInfo]    Script Date: 5/28/2019 10:08:03 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertPaymentExchangeEntityInfo]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[InsertPaymentExchangeEntityInfo]
GO
/****** Object:  StoredProcedure [dbo].[InsertPaymentExchangeEntityInfo]    Script Date: 5/28/2019 10:08:03 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/******************************************************************************
* PROCEDURE:  [dbo].[InsertPaymentExchangeEntityInfo]
* PURPOSE: Inserts PaymeentExchangeEntity and its address info and returns PEE Info identity
* NOTES:
* CREATED: 09/19/2017  Eugene S.
* MODIFIED
* DATE     AUTHOR      DESCRIPTION
*------------------------------------------------------------------------------
* 02/19/2019 Eugene s  Changed insert update PEE/PEEI logic according to newest business requirements
* 08/04/2020  Eugene S Reworked insert/update logic based on modified table scheema (PEE_Address, PEE_System etc..)
*****************************************************************************/
CREATE PROCEDURE [dbo].[InsertPaymentExchangeEntityInfo] 
	@SOR_Id int
	,@EntityId VARCHAR(10)
	,@EntityIdType VARCHAR(30)
	,@EntityName VARCHAR(100)
	,@EntityEIN VARCHAR(9)
	,@EntityContractNumber VARCHAR(20)
	,@AddressLine1 VARCHAR(80)
	,@AddressLine2 VARCHAR(80)
	,@AddressLine3 VARCHAR(80)
	,@City	VARCHAR(40)
	,@State VARCHAR(2)
	,@Zip	VARCHAR(10)
	,@IsActive BIT
	,@SortValue TINYINT
	,@CreateDate DATETIME
	,@CreatedBy VARCHAR(20)
	,@UpdateDate DATETIME
	,@UpdatedBy VARCHAR(20)
AS
BEGIN
	BEGIN TRY

	-- NOTE: Transaction entegrity & scope is managed from code for this sp execution
		
		-- 1. check for existing pee
		DECLARE @PayeeRecId INT
		DECLARE @PayeeName VARCHAR(100)
		SELECT @PayeeRecId = Payment_Exchange_Entity_ID
			,@PayeeName = [Entity_Name]
		FROM TB_PAYMENT_EXCHANGE_ENTITY (NOLOCK) PEE		
		WHERE [Entity_ID] = @EntityId
			--and [Entity_EIN] = @EntityEIN
		
		-- 2 insert (if not found) or update if name has changed
		IF @PayeeRecId IS NULL
		BEGIN
			-- 2.1 insert new PEE
			INSERT INTO [dbo].[TB_PAYMENT_EXCHANGE_ENTITY] (
				[Entity_ID]
				,[Entity_ID_Type]
				,[Entity_Name]
				,[Entity_EIN]
				,[IsActive]
				,[Sort_Value]
				,[CreateDate]
				,[CreatedBy]
				,[UpdateDate]
				,[UpdatedBy]
				)
			VALUES (
				@EntityId
				,@EntityIdType
				,@EntityName
				,@EntityEIN
				,@IsActive
				,@SortValue
				,@CreateDate
				,@CreatedBy
				,@UpdateDate
				,@UpdatedBy
				)

			SET @PayeeRecId = SCOPE_IDENTITY()
		END
		ELSE
		BEGIN
			IF @PayeeName <> @EntityName
				-- 2.2 update the name when entity is found but name does not match 
				UPDATE [dbo].[TB_PAYMENT_EXCHANGE_ENTITY]
				SET [Entity_Name] = @EntityName
					,UpdatedBy = 'system'
					,UpdateDate = getdate()
				WHERE Payment_Exchange_Entity_ID = @PayeeRecId
		END

		
		-- 3. check existing pee-adddress-info		
		DECLARE @PayeeAddrId INT
			,@PEESysId INT
			,@PayeeAddrName VARCHAR(100)
			,@PayeeAddrLine1 VARCHAR(80)
			,@PayeeAddrLine2 VARCHAR(80)
			,@PayeeAddrLine3 VARCHAR(80)
			,@PayeeAddrCity VARCHAR(40)
			,@PayeeAddrState VARCHAR(2)
			,@PayeeAddrZip VARCHAR(10)
		SELECT TOP 1 @PayeeAddrId = PEEA.[PEE_Address_ID]
			,@PEESysId = PEES.PEE_System_ID
			,@PayeeAddrName = PEEA.[Entity_Name]
			,@PayeeAddrLine1 = PEEA.[Address_Line1]
			,@PayeeAddrLine2 = PEEA.[Address_Line2]
			,@PayeeAddrLine3 = PEEA.[Address_Line3]
			,@PayeeAddrCity = PEEA.[City]
			,@PayeeAddrState = PEEA.[State]
			,@PayeeAddrZip = PEEA.[Zip]
		FROM [dbo].[TB_PEE_ADDRESS] (NOLOCK) PEEA
		INNER JOIN TB_PEE_SYSTEM (NOLOCK) PEES on PEES.PEE_System_ID = PEEA.PEE_System_ID
		WHERE PEES.Payment_Exchange_Entity_ID = @PayeeRecId
			AND PEEA.ContractNumber = @EntityContractNumber
		ORDER BY PEEA.PEE_Address_ID DESC			
	
    		
		-- 4 insert if PEEA not found OR (name or address has changed)
		IF @PayeeAddrId IS NULL 
			OR (@PayeeAddrName <> @EntityName			
			OR @PayeeAddrLine1 <> @AddressLine1
			OR @PayeeAddrLine2 <> @AddressLine2
			OR @PayeeAddrLine3 <> @AddressLine3
			OR @PayeeAddrCity <> @City
			OR @PayeeAddrState <> @State
			OR @PayeeAddrZip <> @Zip
			)
		BEGIN

			-- 4.1 insert new PEESys entry
			INSERT INTO [dbo].[TB_PEE_SYSTEM] (
				[Payment_Exchange_Entity_ID]
				,[SOR_ID]
				)
			VALUES (
				@PayeeRecId
				,@SOR_Id
				)			
			Set @PEESysId = SCOPE_IDENTITY()

			
			-- 4.2 insert new PEEAddr						
			INSERT INTO [dbo].[TB_PEE_ADDRESS] (
				[PEE_System_ID]
				,[Entity_Name]
				,[Address_Line1]
				,[Address_Line2]
				,[Address_Line3]
				,[City]
				,[State]
				,[Zip]
				,[ContractNumber]
				,[CreateDate]
				)
			VALUES (
				@PEESysId
				,@EntityName
				,@AddressLine1
				,@AddressLine2
				,@AddressLine3
				,@City
				,@State
				,@Zip
				,@EntityContractNumber
				,getdate()
				)
			SET @PayeeAddrId = SCOPE_IDENTITY()
		END
				
		-- return PEEAddr id and vendor code
		SELECT @PayeeAddrId as [SCOPE_IDENTITY];
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
END
GO
