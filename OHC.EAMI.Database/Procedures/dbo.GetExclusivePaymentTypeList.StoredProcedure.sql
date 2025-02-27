USE [EAMI-MC]
GO

/****** Object:  StoredProcedure [dbo].[GetExclusivePaymentTypeList]    Script Date: 12/1/2023 1:20:42 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



/******************************************************************************
* PROCEDURE:  [dbo].[GetExclusivePaymentTypeList]
* PURPOSE: Fetches the list of funds
* NOTES:
* CREATED: 10/27/2022  Gopal P.
* MODIFIED
* DATE		AUTHOR      DESCRIPTION
*------------------------------------------------------------------------------
-- exec GetFundList 0
*****************************************************************************/
CREATE PROCEDURE [dbo].[GetExclusivePaymentTypeList]
	
	@includeInactive bit,
	@systemID int
AS
BEGIN
	

	SELECT TB_FUND.Fund_ID,TB_FUND.Fund_Code, TB_EXCLUSIVE_PAYMENT_TYPE.*
FROM TB_FUND 
INNER JOIN TB_EXCLUSIVE_PAYMENT_TYPE ON TB_FUND.Fund_Id=TB_EXCLUSIVE_PAYMENT_TYPE.Fund_ID;
return;
END
GO


