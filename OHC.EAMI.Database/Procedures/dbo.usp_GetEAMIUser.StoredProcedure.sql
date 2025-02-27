USE [EAMI-MC]
GO
/****** Object:  StoredProcedure [dbo].[usp_GetEAMIUser]    Script Date: 5/28/2019 10:08:03 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_GetEAMIUser]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_GetEAMIUser]
GO
/****** Object:  StoredProcedure [dbo].[usp_GetEAMIUser]    Script Date: 5/28/2019 10:08:03 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author					:Ram Dongre
-- Create date				:9/27/2017
-- Last Modified Date       :
---Last Modified By         :
-- Description              :This procedure gets a active EAMI user details who could be either domain or a id-password based user.
-- Special Notes			:Password stored contains salt with it, when null we assume passed username to be from Active Directory   
-- =============================================
CREATE PROCEDURE [dbo].[usp_GetEAMIUser] (
	@UserName VARCHAR(200)
	,@Password VARCHAR(1000) = NULL
	,@DomainName VARCHAR(200) = NULL
	)
AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRY
		DECLARE @UserID INT = 0;

		SET @UserName = dbo.fn_FitData(@UserName);
		SET @Password = dbo.fn_FitData(@Password);
		SET @DomainName = dbo.fn_FitData(@DomainName);

		IF (len(@Password) = 0)
			SELECT @UserID = User_ID
			FROM dbo.TB_USER
			WHERE User_Name = @UserName
				AND Domain_Name = @DomainName;
		ELSE
			SELECT @UserID = User_ID
			FROM dbo.TB_USER
			WHERE User_Name = @UserName
				AND Domain_Name IS NULL;

		SELECT a.User_ID
			,User_Name
			,Display_Name
			,User_EmailAddr
			,User_Password
			,Domain_Name
			,a.User_Type_ID
			,b.User_Type_Code
			,b.User_Type_Name
		FROM dbo.TB_USER a
		INNER JOIN dbo.TB_USER_TYPE b ON a.User_Type_ID = b.User_Type_ID
		WHERE a.User_ID = @UserID
			AND a.IsActive = 1
			AND b.IsActive = 1;

		SELECT C.Role_ID
			,c.Role_Code
			,c.Role_Name
		FROM dbo.TB_USER_ROLE a
		INNER JOIN TB_USER b ON a.User_ID = b.User_ID
		INNER JOIN TB_ROLE c ON a.Role_ID = c.Role_ID
		WHERE b.User_ID = @UserID
			AND a.IsActive = 1
			AND b.IsActive = 1
			AND c.IsActive = 1;

		SELECT DISTINCT e.Permission_ID
			,e.Permission_Code
			,e.Permission_Name
		FROM dbo.TB_USER_ROLE a
		INNER JOIN TB_USER b ON a.User_ID = b.User_ID
		INNER JOIN TB_ROLE c ON a.Role_ID = c.Role_ID
		INNER JOIN TB_ROLE_PERMISSION d ON c.Role_ID = d.Role_ID
		INNER JOIN TB_PERMISSION e ON d.Permission_ID = e.Permission_ID
		WHERE a.User_ID = @UserID
			AND a.IsActive = 1
			AND b.IsActive = 1
			AND c.IsActive = 1
			AND d.IsActive = 1;

		SELECT C.System_ID
			,c.System_Code
			,c.System_Name
		FROM dbo.TB_SYSTEM_USER a
		INNER JOIN TB_USER b ON a.User_ID = b.User_ID
		INNER JOIN TB_SYSTEM c ON a.System_ID = c.System_ID
		WHERE a.User_ID = @UserID
			AND a.IsActive = 1
			AND b.IsActive = 1
			AND c.IsActive = 1;
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
