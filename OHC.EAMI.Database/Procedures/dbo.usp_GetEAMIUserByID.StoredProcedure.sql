USE [EAMI-MC]
GO
/****** Object:  StoredProcedure [dbo].[usp_GetEAMIUserByID]    Script Date: 5/28/2019 10:08:03 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_GetEAMIUserByID]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_GetEAMIUserByID]
GO
/****** Object:  StoredProcedure [dbo].[usp_GetEAMIUserByID]    Script Date: 5/28/2019 10:08:03 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author								:Ram Dongre
-- Create date							:10/17/2017
-- Last Modified Date                   :
---Last Modified By                     :
-- Description                          :This procedure gets a active EAMI user details by User-ID.
-- Special Notes						:
-- =============================================
CREATE  PROCEDURE [dbo].[usp_GetEAMIUserByID]
(
	@UserID int
)	
AS
BEGIN
		SET NOCOUNT ON;

		BEGIN TRY						
						
					Select User_Name, Display_Name, User_EmailAddr, User_Password, Domain_Name, a.User_Type_ID, b.User_Type_Code, b.User_Type_Name, a.IsActive 'UserStatus'
					From dbo.TB_USER a
					Inner Join dbo.TB_USER_TYPE b On a.User_Type_ID = b.User_Type_ID
					Where a.User_ID = @UserID And b.IsActive = 1;

					Select C.Role_ID, c.Role_Code, c.Role_Name
					From dbo.TB_USER_ROLE a
					Inner Join TB_USER b On a.User_ID = b.User_ID
					Inner Join TB_ROLE c On a.Role_ID = c.Role_ID
					Where b.User_ID = @UserID And c.IsActive = 1;

					Select Distinct e.Permission_ID, e.Permission_Code, e.Permission_Name
					From dbo.TB_USER_ROLE a
					Inner Join TB_USER b On a.User_ID = b.User_ID
					Inner Join TB_ROLE c On a.Role_ID = c.Role_ID
					Inner Join TB_ROLE_PERMISSION d On c.Role_ID = d.Role_ID
					Inner Join TB_PERMISSION e On d.Permission_ID = e.Permission_ID
					Where a.User_ID = @UserID And e.IsActive = 1 And c.IsActive = 1 And d.IsActive = 1;

					Select C.System_ID, c.System_Code, c.System_Name
					From dbo.TB_SYSTEM_USER a
					Inner Join TB_USER b On a.User_ID = b.User_ID
					Inner Join TB_SYSTEM c On a.System_ID = c.System_ID
					Where a.User_ID = @UserID And c.IsActive = 1;

					
		END TRY
		BEGIN CATCH
				--SELECT
				--ERROR_NUMBER() AS ErrorNumber
				--,ERROR_SEVERITY() AS ErrorSeverity
				--,ERROR_STATE() AS ErrorState
				--,ERROR_PROCEDURE() AS ErrorProcedure
				--,ERROR_LINE() AS ErrorLine
				--,ERROR_MESSAGE() AS ErrorMessage;
				--IF @@TRANCOUNT > 0
				--	ROLLBACK TRANSACTION;
		END CATCH	
END

GO
