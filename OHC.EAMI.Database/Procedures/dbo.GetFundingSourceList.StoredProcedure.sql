USE [EAMI-MC]
GO
/****** Object:  StoredProcedure [dbo].[GetFundingSourceList]    Script Date: 1/15/2024 10:27:22 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




/******************************************************************************
* PROCEDURE:  [dbo].[GetFundingSourceList]
* PURPOSE: Fetches the list of Funding Source
* NOTES:
* CREATED: 12/27/2023  Gopal P.
* MODIFIED
* DATE		AUTHOR      DESCRIPTION
*------------------------------------------------------------------------------
-- exec GetFundList 0, 1
*****************************************************************************/
ALTER PROCEDURE [dbo].[GetFundingSourceList]
	-- Add the parameters for the stored procedure here	
	@includeInactive bit,
	@systemID int
AS
BEGIN
	IF (@includeInactive = 1) --in case of reports where we need all active + inactive funding source
	BEGIN
			SELECT * FROM TB_FUNDING_SOURCE 		
			return;
	END;
	ELSE -- in case of Fund UI, where we need to show only Active Funding Source
	BEGIN
		SELECT F.*
				--, S.System_Code
				--,S.System_Name
		FROM TB_FUNDING_SOURCE as F
		--JOIN TB_SYSTEM AS S ON F.System_ID = S.System_ID
		WHERE F.IsActive = 1
		AND F.System_ID = @systemID
			return;
	END;
	SET NOCOUNT ON;
	SELECT * FROM TB_FUNDING_SOURCE 
	WHERE IsActive = @includeInactive
END
