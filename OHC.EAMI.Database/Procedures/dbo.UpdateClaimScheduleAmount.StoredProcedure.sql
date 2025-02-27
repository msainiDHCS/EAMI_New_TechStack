USE [EAMI-MC]
GO
/****** Object:  StoredProcedure [dbo].[UpdateClaimScheduleAmount]    Script Date: 5/28/2019 10:08:03 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UpdateClaimScheduleAmount]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[UpdateClaimScheduleAmount]
GO
/****** Object:  StoredProcedure [dbo].[UpdateClaimScheduleAmount]    Script Date: 5/28/2019 10:08:03 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/******************************************************************************
* PROCEDURE:  [dbo].[UpdateClaimScheduleAmount]
* PURPOSE: Updates Claim Schedule Amount by a Claim Schedule ID
* NOTES:
* CREATED: 06/14/2018  Genady G.
* MODIFIED
* DATE     AUTHOR      DESCRIPTION
*------------------------------------------------------------------------------
*****************************************************************************/
CREATE PROCEDURE [dbo].[UpdateClaimScheduleAmount] 
	@ClaimScheduleIDList VARCHAR(max)
AS
BEGIN
	BEGIN TRY	
		-- convert list to xml
		DECLARE @xml XML
		SET @xml = N'<root><r>' + replace(@ClaimScheduleIDList, ',', '</r><r>') + '</r></root>'

		UPDATE cs
		SET cs.Amount = t.Amount
		FROM [dbo].[TB_CLAIM_SCHEDULE] AS cs
		INNER JOIN
			(
				SELECT pcs.Claim_Schedule_ID, SUM(pr.Amount) as Amount
				FROM TB_PAYMENT_RECORD pr
					INNER JOIN TB_PAYMENT_CLAIM_SCHEDULE pcs ON pr.Payment_Record_ID = pcs.Payment_Record_ID
				GROUP BY pcs.Claim_Schedule_ID 
			) t
			ON t.Claim_Schedule_ID = cs.Claim_Schedule_ID
		WHERE cs.Claim_Schedule_ID IN (SELECT t.value('.', 'INT') FROM @xml.nodes('//root/r') AS a(t))

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
