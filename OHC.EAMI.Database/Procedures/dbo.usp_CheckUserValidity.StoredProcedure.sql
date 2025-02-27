USE [EAMI-MC]
GO
/****** Object:  StoredProcedure [dbo].[usp_CheckUserValidity]    Script Date: 5/28/2019 10:08:03 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_CheckUserValidity]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_CheckUserValidity]
GO
/****** Object:  StoredProcedure [dbo].[usp_CheckUserValidity]    Script Date: 5/28/2019 10:08:03 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author								:Ram Dongre
-- Create date							:9/27/2017
-- Last Modified Date                   :
---Last Modified By                     :
-- Description                          :This procedure checks if EAMI user exists, even if inactive.
-- Special Notes						:
-- =============================================
CREATE  PROCEDURE [dbo].[usp_CheckUserValidity]
(	
	@UserName		varchar(200),	
	@UserTypeID		int,
	@Status			bit OUTPUT	
)	
AS
BEGIN
		SET NOCOUNT ON;

		BEGIN TRY	

					Set @Status=0;

					if exists(
						Select User_ID
						From dbo.TB_USER
						Where ltrim(rtrim(User_Name)) = ltrim(rtrim(@UserName)) COLLATE SQL_Latin1_General_CP1_CI_AS 
								And User_Type_ID = @UserTypeID)
						Set @Status=1;
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
