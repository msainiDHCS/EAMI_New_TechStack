USE [EAMI-MC]
GO
/****** Object:  StoredProcedure [dbo].[usp_AddUpdateDataByType]    Script Date: 5/28/2019 10:08:03 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_AddUpdateDataByType]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_AddUpdateDataByType]
GO
/****** Object:  StoredProcedure [dbo].[usp_AddUpdateDataByType]    Script Date: 5/28/2019 10:08:03 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author								:Ram Dongre
-- Create date							:9/27/2017
-- Last Modified Date                   :
---Last Modified By                     :
-- Description                          :This procedure adds/updates EAMI master data for role, permission and system.
-- Special Notes						:
-- =============================================
CREATE  PROCEDURE [dbo].[usp_AddUpdateDataByType]
(
	@Type			varchar(10),--values can be ROLE/PERMISSION/SYSTEM
	@ID				int =  NULL,
	@Code			varchar(250),
	@Name			varchar(250),		
	@IsActive		bit,
	@AssociatedData	EAMILongArrayTableType ReadOnly,
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

					If(@Type != 'ROLE' And @Type != 'PERMISSION' And @Type !=  'SYSTEM')
					Begin
							Set @Message = 'Invalid data';
							return ;
					End;

					If(len(ltrim(rtrim(isnull(@Code,''))))=0 or len(ltrim(rtrim(isnull(@Name,''))))=0)
					Begin
							Set @Message = 'Code or Name cannot be emtpy';
							return;
					End;


					If(@ID Is Not NULL)--Edit use case
					Begin
							If(@Type = 'ROLE')
							Begin
									Update TB_ROLE
									Set Role_Code = @Code, Role_Name = @Name, IsActive = @IsActive, UpdatedBy = @LoginUserName, UpdateDate = Getdate()
									Where Role_ID = @ID;

									Update dbo.TB_ROLE_PERMISSION
									Set IsActive = 0, UpdatedBy = @LoginUserName, UpdateDate = getdate()
									Where Permission_ID Not In (Select ID From @AssociatedData) And Role_ID = @ID;

									Update dbo.TB_ROLE_PERMISSION
									Set IsActive = 1, UpdatedBy = @LoginUserName, UpdateDate = getdate()
									Where Permission_ID In (Select ID From @AssociatedData) And Role_ID = @ID;

									Insert Into dbo.TB_ROLE_PERMISSION(Permission_ID, Role_ID, CreatedBy, CreateDate, Sort_Order, IsActive)
									Select ID, @ID, @LoginUserName, getdate(), (Isnull((Select max(Sort_Order) From dbo.TB_ROLE_PERMISSION),0) + 1), 1 
									From @AssociatedData
									Where ID Not In (Select Permission_ID From dbo.TB_ROLE_PERMISSION Where Role_ID = @ID);

									Set @Status = 'OK';
									Set @Message = 'Role updated successfully';		
							End;
							Else If(@Type = 'PERMISSION')
							Begin
									Update TB_PERMISSION
									Set Permission_Code = @Code, Permission_Name = @Name, IsActive = @IsActive, UpdatedBy = @LoginUserName, UpdateDate = Getdate()
									Where Permission_ID = @ID;

									Set @Status = 'OK';
									Set @Message = 'Permission updated successfully';		
							End;
							Else If(@Type = 'SYSTEM')
							Begin
									Update TB_SYSTEM
									Set System_Code = @Code, System_Name = @Name, IsActive = @IsActive, UpdatedBy = @LoginUserName, UpdateDate = Getdate()
									Where System_ID = @ID;

									Set @Status = 'OK';
									Set @Message = 'System updated successfully';		
							End;
							
							return;

					End;
					Else--add use case
					Begin	
							Declare @NewID int;

							If(@Type = 'ROLE')
							Begin
									If (Select Count(1) From dbo.TB_ROLE Where ltrim(rtrim(lower(Role_Code))) = ltrim(rtrim(lower(@Code))))>0
									Begin
											Set @Message = 'Duplicate role code found';		
											return;
									End;

									Insert Into dbo.TB_ROLE(Role_Code, Role_Name,Sort_Order, CreatedBy, CreateDate, IsActive)
									Select @Code, @Name, Isnull((Select max(Sort_Order) From dbo.TB_USER),0) + 1, @LoginUserName, getdate(), 1;

									Set @NewID = SCOPE_IDENTITY();

									Insert Into dbo.TB_ROLE_PERMISSION(Role_ID, Permission_ID, CreatedBy, CreateDate, IsActive, Sort_Order)
									Select @NewID, ID, @LoginUserName, getdate(), 1, (Isnull((Select max(Sort_Order) From dbo.TB_ROLE_PERMISSION),0) + 1)
									From @AssociatedData;

									Set @Status = 'OK';
									Set @Message = 'Role added successfully';

							End;							
							Else If(@Type = 'PERMISSION')
							Begin
									
									If (Select Count(1) From dbo.TB_PERMISSION Where ltrim(rtrim(lower(Permission_Code))) = ltrim(rtrim(lower(@Code))))>0
									Begin
											Set @Message = 'Duplicate permission code found';		
											return;
									End;

									Insert Into dbo.TB_PERMISSION(Permission_Code, Permission_Name,Sort_Order, CreatedBy, CreateDate, IsActive)
									Select @Code, @Name, Isnull((Select max(Sort_Order) From dbo.TB_PERMISSION),0) + 1, @LoginUserName, getdate(), 1;


									Set @Status = 'OK';
									Set @Message = 'Permission added successfully';		

							End;
							Else If(@Type = 'SYSTEM')
							Begin
									If (Select Count(1) From dbo.TB_SYSTEM Where ltrim(rtrim(lower(System_Code))) = ltrim(rtrim(lower(@Code))))>0
									Begin
											Set @Message = 'Duplicate system code found';		
											return;
									End;

									Insert Into dbo.TB_SYSTEM(System_Code, System_Name,Sort_Order, CreatedBy, CreateDate, IsActive)
									Select @Code, @Name, Isnull((Select max(Sort_Order) From dbo.TB_SYSTEM),0) + 1, @LoginUserName, getdate(), 1;


									Set @Status = 'OK';
									Set @Message = 'System added successfully';	
									
							End;

							Return;
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
