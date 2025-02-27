USE [EAMI-MC]
GO

/****** Object:  StoredProcedure [dbo].[usp_GetEAMILookUps]    Script Date: 11/30/2023 11:49:35 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author								:Ram Dongre
-- Create date							:9/27/2017
-- Last Modified Date                   :3/13/2019
---Last Modified By                     :Alex Hoang
-- Description                          :This procedure gets EAMI look ups.
-- Special Notes						:Alex added ROLE_WITH_INACTIVE_USERROLE and SYSTEM_USER sections on 3/13/19 
-- Updated								:Gopal P. added fund table
-- =============================================
CREATE  PROCEDURE [dbo].[usp_GetEAMILookUps]
AS
BEGIN
		SET NOCOUNT ON;

		BEGIN TRY	
					Select 'FUND' As 'Type', Fund_ID As 'ID', Fund_Code 'Code', Fund_Name 'Name'
					From dbo.TB_Fund Where IsActive = 1 UNION ALL
					Select 'ROLE' As 'Type', Role_ID As 'ID', Role_Code 'Code', Role_Name 'Name'
					From dbo.TB_ROLE Where IsActive = 1 UNION ALL
					Select 'USERTYPE' As 'Type', User_Type_ID As 'ID', User_Type_Code 'Code', User_Type_Name 'Name'
					From dbo.TB_USER_TYPE Where IsActive = 1 UNION ALL
					Select 'SYSTEM' As 'Type', System_ID As 'ID', System_Code 'Code', System_Name 'Name'
					From dbo.TB_SYSTEM Where IsActive = 1 UNION ALL
					Select 'PERMISSION' As 'Type', Permission_ID As 'ID', Permission_Code 'Code', Permission_Name 'Name'
					From dbo.TB_PERMISSION  Where IsActive = 1 UNION ALL
					Select 'ROLE_WITH_INACTIVE_USERROLE' As 'Type', ur.User_ID As 'ID', r.Role_Code 'Code', r.Role_Name 'Name'
					From dbo.TB_ROLE r INNER JOIN dbo.TB_USER_ROLE ur ON r.Role_ID = ur.Role_ID
					Where ur.IsActive = 0 UNION ALL
					Select 'SYSTEM_WITH_INACTIVE_SYSTEM_USER' As 'Type', su.User_ID As 'ID', s.System_Code 'Code', s.System_Name 'Name'
					From dbo.TB_SYSTEM s INNER JOIN dbo.TB_SYSTEM_USER su ON s.System_ID = su.System_ID
					Where su.IsActive = 0;
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


