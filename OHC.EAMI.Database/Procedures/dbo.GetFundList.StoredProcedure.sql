USE [EAMI-MC]
GO
/****** Object:  StoredProcedure [dbo].[GetFundList]    Script Date: 10/27/2023 12:51:42 PM ******/

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetFundList]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetFundList]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/******************************************************************************
* PROCEDURE:  [dbo].[GetFundList]
* PURPOSE: Fetches the list of funds
* NOTES:
* CREATED: 10/27/2022  Meetu S.
* MODIFIED
* DATE		AUTHOR      DESCRIPTION
*------------------------------------------------------------------------------
-- exec GetFundList 0
*****************************************************************************/
CREATE PROCEDURE [dbo].[GetFundList]
	-- Add the parameters for the stored procedure here	
	@includeInactive bit,
	@systemID int
AS
BEGIN
	IF (@includeInactive = 1) --in case of reports where we need all active + inactive funds
	BEGIN
			SELECT * FROM TB_FUND 		
			return;
	END;
	ELSE -- in case of Fund UI, where we need to show only Active Funds
	BEGIN
		SELECT F.*
				--, S.System_Code
				--,S.System_Name
		FROM TB_FUND as F
		--JOIN TB_SYSTEM AS S ON F.System_ID = S.System_ID
		WHERE F.IsActive = 1
		AND F.System_ID = @systemID
			return;
	END;
	SET NOCOUNT ON;
	SELECT * FROM TB_FUND 
	WHERE IsActive = @includeInactive
END
