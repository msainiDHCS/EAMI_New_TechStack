SET NOCOUNT ON;
USE [EAMI-Dental]
GO
/****** Object:  StoredProcedure [dbo].[usp_DeactivateEAMIUser]    Script Date: 10/28/2022 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_DeactivateEAMIUser]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_DeactivateEAMIUser]
GO
/****** Object:  StoredProcedure [dbo].[usp_DeactivateEAMIUser]    Script Date: 10/28/2022 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author								:Sujitha Kodavati
-- Create date							:10/28/2022
-- Last Modified Date                   :
---Last Modified By                     :
-- Description                          :This procedure adds/updates EAMI user.
-- Special Notes						:
-- =============================================
CREATE  PROCEDURE [dbo].[usp_DeactivateEAMIUser]
(
	@UserID			int =  NULL,	
	@IsActive		bit,	
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

					
					Update dbo.TB_USER
						   Set 
							IsActive = @IsActive,
							UpdatedBy = @LoginUserName, UpdateDate = getdate()
						From dbo.TB_USER a
					Inner Join dbo.TB_USER_TYPE b On a.User_Type_ID = b.User_Type_ID
					Where a.User_ID = @UserID And a.User_Type_ID = 1 --and b.IsActive = 1;

							Set @Status = 'OK';
									Set @Message = 'User deactivated successfully';		
									return;

				
					
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


