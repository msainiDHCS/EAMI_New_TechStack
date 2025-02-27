USE [EAMI-Dental]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetAvailableAuditFilePayDates]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetAvailableAuditFilePayDates]
GO


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/******************************************************************************
* PROCEDURE:  [dbo].[GetAvailableAuditFilePayDates]
* PURPOSE: Gets ECS PayDate Values that are in SENT_TO_SCO status and have no such PayDate value in TB_AUDIT_FILE table
* NOTES:
* CREATED: 06/29/2022  Genady G.
* MODIFIED
* DATE			AUTHOR      DESCRIPTION
*------------------------------------------------------------------------------
*****************************************************************************/

CREATE PROCEDURE [dbo].[GetAvailableAuditFilePayDates]
AS
BEGIN
	BEGIN TRY

		DECLARE @SENT_TO_SCO_ECS_StatusTypeID int = (SELECT[ECS_Status_Type_ID] from [dbo].[TB_ECS_STATUS_TYPE] WHERE [Code] = 'SENT_TO_SCO')

		SELECT 
			ECS.[PayDate]
		FROM [dbo].[TB_ECS] ECS
			LEFT JOIN TB_AUDIT_FILE ADT ON ECS.PayDate = ADT.PayDate
		WHERE ECS.[Current_ECS_Status_Type_ID] = @SENT_TO_SCO_ECS_StatusTypeID
		GROUP BY  ECS.[PayDate], ADT.[PayDate]
		HAVING ADT.[PayDate] IS NULL
		ORDER BY ADT.[PayDate]
		
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