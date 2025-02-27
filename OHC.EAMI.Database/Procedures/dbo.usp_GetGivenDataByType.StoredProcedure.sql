USE [EAMI-MC]
GO
/****** Object:  StoredProcedure [dbo].[usp_GetGivenDataByType]    Script Date: 5/28/2019 10:08:03 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_GetGivenDataByType]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_GetGivenDataByType]
GO
/****** Object:  StoredProcedure [dbo].[usp_GetGivenDataByType]    Script Date: 5/28/2019 10:08:03 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author								:Ram Dongre
-- Create date							:10/24/2017
-- Last Modified Date                   :
---Last Modified By                     :
-- Description                          :This procedure gets data for a given role/permission/system for add/edit.
-- Special Notes						:
-- =============================================
CREATE  PROCEDURE [dbo].[usp_GetGivenDataByType]
(
	@DataType	varchar(10),--possible values can be ROLE/PERMISSION/SYSTEM,
	@DateID	    int
)
AS
BEGIN
		SET NOCOUNT ON;

		BEGIN TRY	
					
					Declare @tb TABLE
					(
						ID int,
						Code varchar(200),
						Name varchar(250),
						CreatedBy varchar(500),
						CreateDate Datetime,
						UpdatedBy varchar(500),
						UpdateDate Datetime,
						IsActive bit,
						AssociatedData varchar(2000)
					);

					If(@DataType = 'ROLE')
					Begin
							Insert into @tb
							Select Role_ID 'ID', Role_Code 'Code', Role_Name 'Name', CreatedBy, CreateDate, UpdatedBy, UpdateDate, IsActive,
									[dbo].[fn_GetConcatenatedRolePermissionIDs](Role_ID) 'AssociatedData'
							From dbo.TB_ROLE
							Where Role_ID = @DateID;
					End;
					Else If(@DataType = 'PERMISSION')
					Begin
							Insert into @tb
							Select Permission_ID 'ID', Permission_Code 'Code', Permission_Name 'Name', CreatedBy, CreateDate, UpdatedBy, UpdateDate, IsActive,
									'' 'AssociatedData'
							From dbo.TB_PERMISSION
							Where Permission_ID = @DateID;
					End;
					Else If(@DataType = 'SYSTEM')
					Begin
							Insert into @tb
							Select System_ID 'ID', System_Code 'Code', System_Name 'Name', CreatedBy, CreateDate, UpdatedBy, UpdateDate, IsActive,
									'' 'AssociatedData'
							From dbo.TB_SYSTEM
							Where System_ID = @DateID;
					End;

					Select ID, Code, Name, CreatedBy, CreateDate, UpdatedBy, UpdateDate, IsActive, AssociatedData From @tb;
					
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
