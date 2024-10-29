SET NOCOUNT ON;
USE [EAMI-Dental]
GO
/****** Object:  StoredProcedure [dbo].[usp_GetEAMIUserActiveInfo]    Script Date: 11/2/2022 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_GetEAMIUserActiveInfo]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_GetEAMIUserActiveInfo]
GO
/****** Object:  StoredProcedure [dbo].[usp_GetEAMIUserActiveInfo]   Script Date: 11/2/2022 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author								:Sujitha Kodavati
-- Create date							:11/2/2022
-- Last Modified Date                   :
---Last Modified By                     :
-- Description                          :This procedure to get EAMI user active Information.
-- Special Notes						:
-- =============================================
CREATE  PROCEDURE [dbo].[usp_GetEAMIUserActiveInfo]  
(
	@UserID		int =  NULL	,
	@Status   bit OUTPUT  
)	
AS
BEGIN
		SET NOCOUNT ON;

		BEGIN TRY		
		
		 Set @Status = 'false' 		 
		  
					select	@Status = a.IsActive							 
						From dbo.TB_USER a
					Inner Join dbo.TB_USER_TYPE b On a.User_Type_ID = b.User_Type_ID
					Where a.User_ID = @UserID And a.User_Type_ID = 1 --and b.IsActive = 1;	
				
				select @status
					
		END TRY
		BEGIN CATCH
				
				--Set @Message = ERROR_MESSAGE();

				SELECT
				ERROR_NUMBER() AS ErrorNumber
				--,ERROR_SEVERITY() AS ErrorSeverity
				--,ERROR_STATE() AS ErrorState
				--,ERROR_PROCEDURE() AS ErrorProcedure
				--,ERROR_LINE() AS ErrorLine
				 -- ,ERROR_MESSAGE() AS ErrorMessage;
				--IF @@TRANCOUNT > 0
				--	ROLLBACK TRANSACTION;
		END CATCH	
END

GO


