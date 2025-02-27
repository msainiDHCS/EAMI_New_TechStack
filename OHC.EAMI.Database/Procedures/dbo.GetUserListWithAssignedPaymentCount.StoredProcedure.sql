USE [EAMI-MC]
GO
/****** Object:  StoredProcedure [dbo].[GetUserListWithAssignedPaymentCount]    Script Date: 5/28/2019 10:08:03 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetUserListWithAssignedPaymentCount]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetUserListWithAssignedPaymentCount]
GO
/****** Object:  StoredProcedure [dbo].[GetUserListWithAssignedPaymentCount]    Script Date: 5/28/2019 10:08:03 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/******************************************************************************
* PROCEDURE:  [dbo].[GetUserListWithAssignedPaymentCount]
* PURPOSE: Gets user list and roles with assigned payment count for specified ROLE(s)
* NOTES:
* CREATED: 2/5/2018  Genady G.
* MODIFIED
* DATE				AUTHOR					DESCRIPTION
* 2/1/2019			Genady G.				Add UserID parameter. Filter result if user does not have ADMIN role
* 6/4/2019			Genady G.				Eliminate Inactive users from result list
*------------------------------------------------------------------------------
*****************************************************************************/
CREATE PROCEDURE [dbo].[GetUserListWithAssignedPaymentCount]
	@UserID INT
AS
BEGIN
	BEGIN TRY
		
		DECLARE @AssignedPaymentStatusTypeID int = (SELECT [Payment_Status_Type_ID] from [dbo].[TB_PAYMENT_STATUS_TYPE] WHERE [Code] = 'ASSIGNED')
		DECLARE @RoleID_PROCESSOR int = (SELECT [Role_ID] from [dbo].[TB_ROLE] WHERE [Role_Code] = 'PROCESSOR')
		DECLARE @RoleID_ADMIN int = (SELECT [Role_ID] from [dbo].[TB_ROLE] WHERE [Role_Code] = 'ADMIN')
		DECLARE @Is_User_ADMIN bit = (SELECT IIF (EXISTS (SELECT NULL from [dbo].[TB_USER_ROLE] WHERE [User_ID] = @UserID AND Role_ID = @RoleID_ADMIN AND IsActive = 1), 1 , 0))
				
		--GET USER LIST BY ROLE
		CREATE TABLE #userList
		(
			[User_ID] int,
			[User_Name] varchar (200),
			[Display_Name] varchar (500)
		)
		
		INSERT INTO #userList
		SELECT DISTINCT
			usr.[User_ID] 
			,usr.[User_Name]
			,usr.[Display_Name]
		FROM TB_USER usr
			INNER JOIN TB_USER_ROLE url on usr.[User_ID] = url.[User_ID]
		WHERE 
			url.[Role_ID] = @RoleID_PROCESSOR
			AND usr.IsActive = 1
			AND url.IsActive = 1
		
		--REMOVE ADMINS USERS FROM LIST, IF REQUESTER IS NOT AN 'ADMIN'
		IF(@Is_User_ADMIN = 0)
		BEGIN
			DELETE FROM #userList WHERE [User_ID] in
			(SELECT [User_ID] FROM TB_USER_ROLE WHERE Role_ID = @RoleID_ADMIN AND IsActive = 1)
		END

		--DETERMINE USER ASSIGNMENT COUNTS 
		CREATE TABLE #userPaymentGroups
		(
			[User_ID] int
		)

		INSERT INTO #userPaymentGroups
		SELECT 
			UA.[User_ID]
		FROM TB_PAYMENT_DN_STATUS PDN
			INNER JOIN TB_PAYMENT_RECORD PR ON PDN.Payment_Record_ID = PR.Payment_Record_ID
			INNER JOIN TB_PAYMENT_STATUS PS ON pdn.CurrentPaymentStatusID = ps.Payment_Status_ID
			INNER JOIN TB_PAYMENT_USER_ASSIGNMENT UA ON PDN.CurrentUserAssignmentID = UA.Payment_User_Assignment_ID
		WHERE 
			PS.Payment_Status_Type_ID = @AssignedPaymentStatusTypeID
			AND UA.[User_ID] IN (SELECT [User_ID] FROM #userList)
		GROUP BY [User_ID], PR.PaymentSet_Number

		--RETURN DATASET 1
		--SELECT USER LIST, EACH USER WITH ITS ASSIGNMENT COUNTS
		SELECT
			USR.[User_ID],
			USR.[User_Name],
			USR.[Display_Name],
			COUNT(AUSR.[User_ID]) AS Assignment_Count
		FROM #userList USR
			LEFT JOIN #userPaymentGroups AUSR ON USR.[User_ID] = AUSR.[User_ID]
		GROUP BY 
		USR.[User_ID],
			USR.[User_Name],
			USR.[Display_Name]
		
		--RETURN DATASET 2
		--SELECT ROLES FOR EACH USER
		SELECT 
			C.Role_ID
			,a.[User_ID]
			,c.Role_Code
			,c.Role_Name
		FROM dbo.TB_USER_ROLE a
			INNER JOIN TB_USER b ON a.User_ID = b.User_ID
			INNER JOIN TB_ROLE c ON a.Role_ID = c.Role_ID
		WHERE b.User_ID in (SELECT [User_ID] FROM #userList)
			AND a.IsActive = 1
			AND b.IsActive = 1
			AND c.IsActive = 1;

		DROP TABLE #userList
		DROP TABLE #userPaymentGroups
	END TRY

	BEGIN CATCH
		DECLARE @ErrorMessage NVARCHAR(4000)
		DECLARE @ErrorSeverity INT
		DECLARE @ErrorState INT

		SET @ErrorMessage = error_message()
		SET @ErrorSeverity = error_severity()
		SET @ErrorState = error_state()

		RAISERROR (
				@ErrorMessage
				,@ErrorSeverity
				,@ErrorState
				)
	END CATCH
END
GO
