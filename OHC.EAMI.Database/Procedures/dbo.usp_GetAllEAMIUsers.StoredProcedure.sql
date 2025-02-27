USE [EAMI-MC]
GO
/****** Object:  StoredProcedure [dbo].[usp_GetAllEAMIUsers]    Script Date: 5/28/2019 10:08:03 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_GetAllEAMIUsers]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_GetAllEAMIUsers]
GO
/****** Object:  StoredProcedure [dbo].[usp_GetAllEAMIUsers]    Script Date: 5/28/2019 10:08:03 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author								:Ram Dongre
-- Create date							:9/27/2017
-- Last Modified Date                   :
---Last Modified By                     :
-- Description                          :This procedure gets all EAMI users for grid display.
-- Special Notes						:
-- =============================================
CREATE  PROCEDURE [dbo].[usp_GetAllEAMIUsers]
AS
BEGIN
		SET NOCOUNT ON;

		BEGIN TRY			
					Declare @Status	varchar(10), @Message varchar(max);	
					
					Select a.User_ID, a.User_Name, a.Display_Name, a.User_EmailAddr, a.Domain_Name, b.User_Type_ID, b.User_Type_Code, b.User_Type_Name, 
							dbo.fn_GetConcatenatedRoles(a.User_ID) 'Roles', dbo.fn_GetConcatenatedSystems(a.User_ID) 'Systems',
							a.IsActive 'UserStatus', a.CreateDate, a.UpdateDate 
					From dbo.TB_USER a
					Inner Join dbo.TB_USER_TYPE b On a.User_Type_ID = b.User_Type_ID;
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
