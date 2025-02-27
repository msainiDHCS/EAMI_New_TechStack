USE [EAMI-MC]
GO
/****** Object:  StoredProcedure [dbo].[GetFundingSummaryForMultipleCS]    Script Date: 5/28/2019 10:08:03 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetFundingSummaryForMultipleCS]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetFundingSummaryForMultipleCS]
GO
/****** Object:  StoredProcedure [dbo].[GetFundingSummaryForMultipleCS]    Script Date: 5/28/2019 10:08:03 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/******************************************************************************
* PROCEDURE:  [dbo].[GetFundingSummaryForMultipleCS]
* PURPOSE: Gets aggregated funding summary for provided list of CS ids  
* NOTES:
* CREATED: 06/29/2018  Eugene S.
* MODIFIED
* DATE     AUTHOR      DESCRIPTION
*------------------------------------------------------------------------------
* 10/08/2018, Eugene S., changed script to only include aggregate values Name and ammounts	
*****************************************************************************/
CREATE PROCEDURE [dbo].[GetFundingSummaryForMultipleCS]
	@ClaimScheduleIDList VARCHAR(max)
AS
BEGIN
	BEGIN TRY
		DECLARE @xml XML
		SET @xml = N'<root><r>' + replace(@ClaimScheduleIDList, ',', '</r><r>') + '</r></root>'

		SELECT		
			FD.Funding_Source_Name			
			,SUM(FD.SGFAmount) AS SGFAmount
			,SUM(FD.FFPAmount) AS FFPAmount
			,SUM(FD.TotalAmount) AS TotalAmount			
			--,count(FD.Funding_Detail_ID) as FDCount			
		FROM TB_FUNDING_DETAIL FD
		INNER JOIN TB_PAYMENT_RECORD PR ON PR.Payment_Record_ID = FD.Payment_Record_ID
		INNER JOIN TB_PAYMENT_CLAIM_SCHEDULE PCS ON PCS.Payment_Record_ID = PR.Payment_Record_ID
		INNER JOIN TB_CLAIM_SCHEDULE CS ON CS.Claim_Schedule_ID = PCS.Claim_Schedule_ID
		WHERE CS.Claim_Schedule_ID IN (
					SELECT t.value('.', 'int') AS [Claim_Schedule_ID]
					FROM @xml.nodes('//root/r') AS a(t)
					)
		GROUP BY FD.Funding_Source_Name			
		ORDER BY FD.Funding_Source_Name ASC

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
