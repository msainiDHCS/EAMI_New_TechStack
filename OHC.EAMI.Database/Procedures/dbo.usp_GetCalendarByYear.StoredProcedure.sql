USE [EAMI-MC]
GO
/****** Object:  StoredProcedure [dbo].[usp_GetCalendarByYear]    Script Date: 5/28/2019 10:08:03 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_GetCalendarByYear]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_GetCalendarByYear]
GO
/****** Object:  StoredProcedure [dbo].[usp_GetCalendarByYear]    Script Date: 5/28/2019 10:08:03 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author								:Ram Dongre
-- Create date							:11/21/2017
-- Last Modified Date                   :
---Last Modified By                     :
-- Description                          :This procedure gets caledar dates for a specific year
-- Special Notes						:
-- =============================================
CREATE  PROCEDURE [dbo].[usp_GetCalendarByYear]
(
	@ActiveYear int
)
AS
BEGIN
		SET NOCOUNT ON;

		BEGIN TRY	
					
					Select Paydate 'Date', Convert(varchar(1),'P') As 'Type', Note
					From dbo.TB_PAYDATE_CALENDAR 
					Where DATEPART(YEAR,Paydate) = @ActiveYear  UNION ALL
					Select Drawdate 'Date', Convert(varchar(1),'D') As 'Type', Note
					From dbo.TB_DRAWDATE_CALENDAR
					Where DATEPART(YEAR,Drawdate) = @ActiveYear
					Order By Type, Date;
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
