USE [EAMI-MC]
GO
/****** Object:  UserDefinedFunction [dbo].[fn_GetConcatenatedSystems]    Script Date: 5/28/2019 4:31:32 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[fn_GetConcatenatedSystems]') AND type in (N'FN'))
DROP FUNCTION [dbo].[fn_GetConcatenatedSystems]
GO
/****** Object:  UserDefinedFunction [dbo].[fn_GetConcatenatedSystems]    Script Date: 5/28/2019 4:31:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
--Author               :Ram Dongre
--Create date		   :9/27/2017
--Last Updated by      :
--Last Updated Date    :
--Description          :This function returns concatenated systems for user grid display
-- =============================================
CREATE  FUNCTION [dbo].[fn_GetConcatenatedSystems]
(
	@UserID int
)
RETURNS varchar(max)
AS
BEGIN
	-- Declare the return variable here
	DECLARE @CommaSeperatedRoles varchar(max) ='';
					Select  @CommaSeperatedRoles = COALESCE(@CommaSeperatedRoles + ',','') + c.System_Name   
					From dbo.TB_SYSTEM_USER a
					Inner Join dbo.TB_USER b On a.User_ID = b.User_ID
					Inner Join dbo.TB_SYSTEM c On a.System_ID = c.System_ID					
					Where b.User_ID = @UserID And a.IsActive = 1 And b.IsActive = 1 And c.IsActive = 1;

	-- Return the result of the function
	RETURN stuff(@CommaSeperatedRoles,1,1,'');

END
GO
