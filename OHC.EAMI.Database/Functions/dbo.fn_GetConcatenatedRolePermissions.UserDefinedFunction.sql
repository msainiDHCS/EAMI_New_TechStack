USE [EAMI-MC]
GO
/****** Object:  UserDefinedFunction [dbo].[fn_GetConcatenatedRolePermissions]    Script Date: 5/28/2019 4:31:32 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[fn_GetConcatenatedRolePermissions]') AND type in (N'FN'))
DROP FUNCTION [dbo].[fn_GetConcatenatedRolePermissions]
GO
/****** Object:  UserDefinedFunction [dbo].[fn_GetConcatenatedRolePermissions]    Script Date: 5/28/2019 4:31:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
--Author               :Ram Dongre
--Create date		   :9/27/2017
--Last Updated by      :6
--Last Updated Date    :
--Description          :This function returns concatenated role-permissions for role grid display
-- =============================================
CREATE  FUNCTION [dbo].[fn_GetConcatenatedRolePermissions]
(
	@RoleID int
)
RETURNS varchar(max)
AS
BEGIN
	-- Declare the return variable here
	DECLARE @CommaSeperatedRoles varchar(max) ='';
					Select  @CommaSeperatedRoles = COALESCE(@CommaSeperatedRoles + ', ','') + b.Permission_Code   
					From dbo.TB_ROLE_PERMISSION a 
					Inner Join dbo.TB_PERMISSION b On a.Permission_ID = b.Permission_ID					
					Where a.Role_ID = @RoleID And a.IsActive = 1 And b.IsActive = 1;

	-- Return the result of the function
	RETURN stuff(@CommaSeperatedRoles,1,1,'');

END
GO
