USE [EAMI-MC]
GO
/****** Object:  StoredProcedure [dbo].[usp_AddUpdateEAMIUser]    Script Date: 5/28/2019 10:08:03 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_AddUpdateEAMIUser]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_AddUpdateEAMIUser]
GO
/****** Object:  StoredProcedure [dbo].[usp_AddUpdateEAMIUser]    Script Date: 5/28/2019 10:08:03 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author								:Ram Dongre
-- Create date							:9/27/2017
-- Last Modified Date                   :
---Last Modified By                     :
-- Description                          :This procedure adds/updates EAMI user.
-- Special Notes						:
-- =============================================
CREATE  PROCEDURE [dbo].[usp_AddUpdateEAMIUser]
(
	@UserID			int =  NULL,
	@UserName		varchar(20),
	@DisplayName	varchar(50) = null,
	@UserEmailAddr  varchar(50) = null,
	@UserPassword	varchar(200) = NULL,--this cannot be plain text but rather hashed, salted password
	@DomainName     varchar(100) = NULL,
	@UserTypeID		int,
	@IsActive		bit,
	@Systems		EAMILongArrayTableType ReadOnly,
	@Roles			EAMILongArrayTableType ReadOnly,
	@LoginUserName	varchar(200),
	@Status			varchar(10) OUTPUT,
	@Message		varchar(max) OUTPUT	
)	
AS
BEGIN
		SET NOCOUNT ON;

		BEGIN TRY	

					Set @Status = 'ERROR';
					Set @Message = '';

					Declare @UserTypeCode varchar(50)=''; 

					Select @UserTypeCode = User_Type_Code
					From dbo.TB_USER_TYPE
					Where User_Type_ID = @UserTypeID And IsActive = 1;

					If(@UserID Is Not NULL)--Edit use case
					Begin
							If(len(ltrim(rtrim(isnull(@UserPassword,''))))=0 And @UserTypeCode = 'UNPD')
							Begin
									Set @Message = 'Password cannot be emtpy';
									return;
							End;

							If(len(ltrim(rtrim(isnull(@DomainName,''))))=0 And @UserTypeCode = 'AD')
							Begin
									Set @Message = 'Domain name cannot be emtpy';
									return;
							End

							Update dbo.TB_USER
							Set User_Password = Case When @UserPassword Is Null Then User_Password Else @UserPassword End, 
									Domain_Name = @DomainName, --User_Type_ID =  @UserTypeID, 
									IsActive = @IsActive,
									UpdatedBy = @LoginUserName, UpdateDate = getdate(), Display_Name = @DisplayName
							Where User_ID = @UserID;

							----------
							Update dbo.TB_USER_ROLE
							Set IsActive = 0, UpdatedBy = @LoginUserName, UpdateDate = getdate()
							Where Role_ID Not In (Select ID From @Roles) And User_ID = @UserID;

							Update dbo.TB_USER_ROLE
							Set IsActive = 1, UpdatedBy = @LoginUserName, UpdateDate = getdate()
							Where Role_ID In (Select ID From @Roles) And User_ID = @UserID;

							Insert Into dbo.TB_USER_ROLE(Role_ID, User_ID, CreatedBy, CreateDate, Sort_Order, IsActive)
							Select ID, @UserID, @LoginUserName,getdate(), (Isnull((Select max(Sort_Order) From dbo.TB_USER_ROLE),0) + 1) ,1 
							From @Roles
							Where ID Not In (Select Role_ID From dbo.TB_USER_ROLE Where User_ID = @UserID);
							-------------

							Update dbo.TB_SYSTEM_USER
							Set IsActive = 0, UpdatedBy = @LoginUserName, UpdateDate = getdate()
							Where System_ID Not In (Select ID From @Systems) And User_ID = @UserID;

							Update dbo.TB_SYSTEM_USER
							Set IsActive = 1, UpdatedBy = @LoginUserName, UpdateDate = getdate()
							Where System_ID In (Select ID From @Systems) And User_ID = @UserID;

							Insert Into dbo.TB_SYSTEM_USER(System_ID, User_ID, CreatedBy, CreateDate, Sort_Order, IsActive)
							Select ID, @UserID, @LoginUserName,getdate(),  (Isnull((Select max(Sort_Order) From dbo.TB_SYSTEM_USER),0) + 1), 1 
							From @Systems
							Where ID Not In (Select System_ID From dbo.TB_SYSTEM_USER Where User_ID = @UserID);
							------------

							Set @Status = 'OK';
							Set @Message = 'User updated successfully';		
							return;

					End;
					Else--add use case
					Begin	
							
							If (Select Count(1) From dbo.TB_USER Where ltrim(rtrim(lower(User_Name))) = ltrim(rtrim(lower(@UserName)))
									And User_Type_ID = @UserTypeID)>0
							Begin
									Set @Message = 'Duplicate user-name found';		
									return;
							End;
							Else
							Begin --print 'inside'
									Insert Into dbo.TB_USER(User_Name, User_EmailAddr, User_Password, Domain_Name, User_Type_ID, Sort_Order, CreatedBy, CreateDate, IsActive, Display_Name)
									Select @UserName, @UserEmailAddr, @UserPassword, @DomainName, @UserTypeID, Isnull((Select max(Sort_Order) From dbo.TB_USER),0) + 1, 
										@LoginUserName, getdate(), 1, @DisplayName;

									Set @UserID = SCOPE_IDENTITY();

									Insert Into dbo.TB_USER_ROLE(User_ID, Role_ID, CreatedBy, CreateDate, IsActive, Sort_Order)
									Select @UserID, ID, @LoginUserName, getdate(), 1, (Isnull((Select max(Sort_Order) From dbo.TB_USER_ROLE),0) + 1)
									From @Roles;

									Insert Into dbo.TB_SYSTEM_USER(User_ID, System_ID, CreatedBy, CreateDate, IsActive, Sort_Order)
									Select @UserID, ID, @LoginUserName, getdate(), 1, (Isnull((Select max(Sort_Order) From dbo.TB_SYSTEM_USER),0) + 1)
									From @Systems;

									Set @Status = 'OK';
									Set @Message = 'User created successfully';		
									return;
							End;
					End;
					
		END TRY
		BEGIN CATCH
				
				Set @Message = ERROR_MESSAGE();

				SELECT
				ERROR_NUMBER() AS ErrorNumber
				--,ERROR_SEVERITY() AS ErrorSeverity
				--,ERROR_STATE() AS ErrorState
				--,ERROR_PROCEDURE() AS ErrorProcedure
				--,ERROR_LINE() AS ErrorLine
				,ERROR_MESSAGE() AS ErrorMessage;
				--IF @@TRANCOUNT > 0
				--	ROLLBACK TRANSACTION;
		END CATCH	
END

GO
