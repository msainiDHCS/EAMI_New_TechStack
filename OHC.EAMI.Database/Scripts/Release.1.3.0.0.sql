--EAMI SCRIPT Release.1.3.0.0 

/******************************************************************************
* PURPOSE: Script modifies schema AND executes data converssion
* NOTES:   Release.1.3.0.0
	There are 3 parts of scripts that are interdependant.
		1. EAMI DB SCHEMA CHANGES
		2. DATA CONVERSION
		3. POST DATA CONVERSION SCRIPT
		4. OTHER SCRIPTS

* CREATED: 3/03/2022  Genady G.
* MODIFIED
* DATE     AUTHOR      DESCRIPTION
*------------------------------------------------------------------------------
*****************************************************************************/

USE [EAMI-RX]
GO

BEGIN TRY

SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON


			DECLARE @dt DATETIME = GETDATE()
			
			--UPDATE	
			UPDATE [dbo].[TB_EXCLUSIVE_PAYMENT_TYPE] SET [IsActive] = 1, [Sort_Value] = 1, UpdateDate = @dt, UpdatedBy = 'SYSTEM' WHERE [Code] = 'NONE'
			UPDATE [dbo].[TB_EXCLUSIVE_PAYMENT_TYPE] SET [IsActive] = 1, [Sort_Value] = 2, UpdateDate = @dt, UpdatedBy = 'SYSTEM' WHERE [Code] = 'SCHIP'
			UPDATE [dbo].[TB_EXCLUSIVE_PAYMENT_TYPE] SET [IsActive] = 1, [Sort_Value] = 3, UpdateDate = @dt, UpdatedBy = 'SYSTEM' WHERE [Code] = 'PCFH'						
			UPDATE [dbo].[TB_EXCLUSIVE_PAYMENT_TYPE] SET [IsActive] = 0, [Sort_Value] = 4, UpdateDate = @dt, UpdatedBy = 'SYSTEM' WHERE [Code] = 'EWC'
			UPDATE [dbo].[TB_EXCLUSIVE_PAYMENT_TYPE] SET [IsActive] = 0, [Sort_Value] = 5, UpdateDate = @dt, UpdatedBy = 'SYSTEM' WHERE [Code] = 'GF'
			
					
			
			IF NOT EXISTS(SELECT NULL FROM [dbo].[TB_EXCLUSIVE_PAYMENT_TYPE] WHERE Code = 'GF')
			BEGIN
				
				INSERT INTO [dbo].[TB_EXCLUSIVE_PAYMENT_TYPE]
				VALUES
					('GF', 'Payment Type associated with fund 0001', 0, 5, @dt, 'SYSTEM', NULL, NULL)
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