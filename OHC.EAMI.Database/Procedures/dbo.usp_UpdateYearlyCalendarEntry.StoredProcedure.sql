USE [EAMI-MC]
GO
/****** Object:  StoredProcedure [dbo].[usp_UpdateYearlyCalendarEntry]    Script Date: 5/28/2019 10:08:03 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_UpdateYearlyCalendarEntry]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_UpdateYearlyCalendarEntry]
GO
/****** Object:  StoredProcedure [dbo].[usp_UpdateYearlyCalendarEntry]    Script Date: 5/28/2019 10:08:03 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author								:Ram Dongre
-- Create date							:11/21/2017
-- Last Modified Date                   :
---Last Modified By                     :
-- Description                          :This procedure saves calendar date entries.
-- Special Notes						:
-- =============================================
CREATE  PROCEDURE [dbo].[usp_UpdateYearlyCalendarEntry]
(	
	@Dates			EAMIDateTypeTableType ReadOnly,
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

					If Exists(Select 1 From dbo.TB_PAYMENT_PAYDATE_CALENDAR a Inner Join dbo.TB_PAYDATE_CALENDAR b On a.Paydate_Calendar_ID = b.Paydate_Calendar_ID
							Inner Join (select EAMIDate DateForDeletion From @Dates Where EAMIDateType = 'P' And EAMIDateActionType = 'D') c ON
									b.Paydate = c.DateForDeletion)
					Begin	
							Set @Message = 'Active assignments found for the pay dates choses for deletion. Cannot continue.';					
							return;
					End;
					Else
					Begin
							Insert Into dbo.TB_DRAWDATE_CALENDAR(Drawdate, IsActive, CreatedBy, CreateDate, Note)
							Select EAMIDate, 1, @LoginUserName, getdate(), Comment
							From @Dates
							Where EAMIDateType = 'D' And EAMIDateActionType = 'A';

							Update dbo.TB_DRAWDATE_CALENDAR 
							Set Note = a.Comment, UpdatedBy = @LoginUserName, UpdateDate = GETDATE()
							From @Dates a
							Where cast(a.EAMIDate as date) = cast(Drawdate as date) And a.EAMIDateType ='D' And a.EAMIDateActionType ='U';

							Delete 
							From dbo.TB_DRAWDATE_CALENDAR 
							Where Drawdate In (Select EAMIDate From @Dates Where EAMIDateType ='D' And EAMIDateActionType ='D');
					
							Insert Into dbo.TB_PAYDATE_CALENDAR(Paydate, IsActive, CreatedBy, CreateDate, Note)
							Select EAMIDate, 1, @LoginUserName, getdate(), Comment
							From @Dates
							Where EAMIDateType = 'P' And EAMIDateActionType = 'A';	

							Update dbo.TB_PAYDATE_CALENDAR 
							Set Note = a.Comment, UpdatedBy = @LoginUserName, UpdateDate = GETDATE()
							From @Dates a
							Where cast(a.EAMIDate as date) = cast(Paydate as date) And a.EAMIDateType ='P' And a.EAMIDateActionType ='U';
					
							Delete 
							From dbo.TB_PAYDATE_CALENDAR 
							Where Paydate In (Select EAMIDate From @Dates Where EAMIDateType ='P' And EAMIDateActionType ='D');
					
							Set @Status = 'OK';
							Set @Message = 'Calendar saved successfully';				
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
