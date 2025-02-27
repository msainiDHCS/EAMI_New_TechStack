USE [EAMI-MC]
GO
/****** Object:  UserDefinedFunction [dbo].[fn_FitData]    Script Date: 5/28/2019 4:31:32 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[fn_FitData]') AND type in (N'FN'))
DROP FUNCTION [dbo].[fn_FitData]
GO
/****** Object:  UserDefinedFunction [dbo].[fn_FitData]    Script Date: 5/28/2019 4:31:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
--Author               :Ram Dongre
--Create date		   :9/28/2017
--Last Updated by      :6
--Last Updated Date    :
--Description          :This function returns input string cleared of white spaces and changes null to empty string
-- =============================================
CREATE  FUNCTION [dbo].[fn_FitData]
(
	@inputData varchar(max) = null
)
RETURNS varchar(max)
AS
BEGIN
	
		Return ltrim(rtrim(isnull(@inputData,'')));	

END
GO
