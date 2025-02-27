USE [EAMI-MC]
GO
/****** Object:  UserDefinedFunction [dbo].[fn_GetCSNegativeFundingIndicator]    Script Date: 5/28/2019 4:31:32 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[fn_GetCSNegativeFundingIndicator]') AND type in (N'FN'))
DROP FUNCTION [dbo].[fn_GetCSNegativeFundingIndicator]
GO
/****** Object:  UserDefinedFunction [dbo].[fn_GetCSNegativeFundingIndicator]    Script Date: 5/28/2019 4:31:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/******************************************************************************
* FUNCTION:  [dbo].[fn_GetCSNegativeFundingIndicator]
* PURPOSE: Determines if given CS has one or more funding sources with negative amount. Returns bit
* NOTES:
* CREATED: 06/29/2018  Eugene S.
* MODIFIED
* DATE     AUTHOR      DESCRIPTION
*------------------------------------------------------------------------------
* 10/08/18 Eugene S.,	changed script to only group by name when checking amount 
*****************************************************************************/
CREATE FUNCTION [dbo].[fn_GetCSNegativeFundingIndicator] 
(
	@ClaimScheduleID INT
)
RETURNS BIT
AS
BEGIN

RETURN (
	SELECT CAST(
			CASE 
			WHEN EXISTS (
					SELECT TotalAmount
					FROM (
						SELECT FD.Funding_Source_Name
							,SIGN(SUM(FD.TotalAmount)) AS TotalAmount
						FROM TB_CLAIM_SCHEDULE CS
						INNER JOIN TB_PAYMENT_CLAIM_SCHEDULE PCS ON PCS.Claim_Schedule_ID = CS.Claim_Schedule_ID
						INNER JOIN TB_PAYMENT_RECORD PR ON PR.Payment_Record_ID = PCS.Payment_Record_ID
						INNER JOIN TB_FUNDING_DETAIL FD ON FD.Payment_Record_ID = PR.Payment_Record_ID
						WHERE CS.Claim_Schedule_ID = @ClaimScheduleID
						GROUP BY FD.Funding_Source_Name
						) AS TT
					WHERE TotalAmount < 0
					)
				THEN 1
			ELSE 0
			END AS BIT)
		)

/* 
--test script
SELECT FD.Funding_Source_Name
	,SUM(FD.SGFAmount) as TotalSGFAmount
	,SUM(FD.FFPAmount) as TotalFFPAmount
	,SUM(FD.TotalAmount) as TotalAmount
	,count(FD.Funding_Detail_ID) FundingDetailCount
FROM TB_CLAIM_SCHEDULE CS
INNER JOIN TB_PAYMENT_CLAIM_SCHEDULE PCS ON PCS.Claim_Schedule_ID = CS.Claim_Schedule_ID
INNER JOIN TB_PAYMENT_RECORD PR ON PR.Payment_Record_ID = PCS.Payment_Record_ID
INNER JOIN TB_FUNDING_DETAIL FD ON FD.Payment_Record_ID = PR.Payment_Record_ID
where CS.Claim_Schedule_ID = 7
group by FD.Funding_Source_Name
*/

END

GO
