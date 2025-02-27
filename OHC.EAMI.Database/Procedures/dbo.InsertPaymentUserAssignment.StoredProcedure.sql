USE [EAMI-MC]
GO
/****** Object:  StoredProcedure [dbo].[InsertPaymentUserAssignment]    Script Date: 5/28/2019 10:08:03 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertPaymentUserAssignment]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[InsertPaymentUserAssignment]
GO
/****** Object:  StoredProcedure [dbo].[InsertPaymentUserAssignment]    Script Date: 5/28/2019 10:08:03 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/******************************************************************************
* PROCEDURE:  [dbo].[InsertPaymentUserAssignment]
* PURPOSE: Inserts Payment User Assignment 
* NOTES:
* CREATED: 12/6/2017  Genady G.
* MODIFIED
* DATE     AUTHOR      DESCRIPTION
*------------------------------------------------------------------------------
*****************************************************************************/
CREATE PROCEDURE [dbo].[InsertPaymentUserAssignment] 
	@PaymentRecordID INT
	,@UserID INT
AS
BEGIN
	BEGIN TRY
		
		DECLARE @AssignedPaymentStatusID INT = (SELECT CurrentPaymentStatusID FROM TB_PAYMENT_DN_STATUS WHERE Payment_Record_ID = @PaymentRecordID)	

		--INSERT PAYMENT USER ASSIGNMENT
		INSERT INTO [dbo].[TB_PAYMENT_USER_ASSIGNMENT] (
			[Payment_Record_ID]
			,[User_ID]
			,[Assigned_Payment_Status_ID]
			)
		VALUES (
			@PaymentRecordID
			,@UserID
			,@AssignedPaymentStatusID
			)
						
		-- get newly inserted Payment User Assignment identity
		DECLARE @paymentUserAssignmentID INT = (SELECT SCOPE_IDENTITY() AS [SCOPE_IDENTITY])	
		
		--UPDATE PAYMENT DN STATUS
		UPDATE [dbo].[TB_PAYMENT_DN_STATUS]
		SET [CurrentUserAssignmentID] = @paymentUserAssignmentID
		WHERE [Payment_Record_ID] = @PaymentRecordID

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
