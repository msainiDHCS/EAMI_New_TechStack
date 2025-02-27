USE [EAMI-MC]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UpdateElectronicClaimScheduleTaskNumber]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[UpdateElectronicClaimScheduleTaskNumber]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/******************************************************************************
* PROCEDURE:  [dbo].[UpdateElectronicClaimScheduleTaskNumber]
* PURPOSE: Updates Electronic Claim Schedule TaskNumber by Ecs ID
* NOTES:
* CREATED: 08/03/2018  Genady G.
* MODIFIED
* DATE     AUTHOR      DESCRIPTION
*------------------------------------------------------------------------------
*****************************************************************************/
CREATE PROCEDURE [dbo].[UpdateElectronicClaimScheduleTaskNumber] 
	@EcsIDList VARCHAR(MAX),
	@SentToScoTaskNumber VARCHAR(30),
	@WarrantReceivedTaskNumber VARCHAR(30)
AS
BEGIN
	BEGIN TRY	
		-- convert list to xml
		DECLARE @xml XML
		SET @xml = N'<root><r>' + replace(@EcsIDList, ',', '</r><r>') + '</r></root>'

		UPDATE [dbo].[TB_ECS] 
		SET 
			[SentToScoTaskNumber] = ISNULL(@SentToScoTaskNumber, [SentToScoTaskNumber]),
			[WarrantReceivedTaskNumber] = ISNULL(@WarrantReceivedTaskNumber, [WarrantReceivedTaskNumber])
		WHERE [ECS_ID] IN (SELECT t.value('.', 'INT') FROM @xml.nodes('//root/r') AS a(t))

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
