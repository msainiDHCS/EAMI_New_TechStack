USE [EAMI-MC]
GO
/****** Object:  StoredProcedure [dbo].[GetEAMICounts]    Script Date: 5/28/2019 10:08:03 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetEAMICounts]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetEAMICounts]
GO
/****** Object:  StoredProcedure [dbo].[GetEAMICounts]    Script Date: 5/28/2019 10:08:03 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/******************************************************************************
* PROCEDURE:  [dbo].[GetEAMICounts]
* PURPOSE: Gets EAMI record counts for categories such as Assigned/Unassigned Payment Records, Pending Claim Schedule
* NOTES:
* CREATED: 4/5/2018  Genady G.
* MODIFIED
* DATE     AUTHOR      DESCRIPTION
*------------------------------------------------------------------------------
*****************************************************************************/
CREATE PROCEDURE [dbo].[GetEAMICounts]
AS
BEGIN
	BEGIN TRY
		
	--PAYMENT RECORD STATUS TYPES
	DECLARE @PaymentStatusTypeID_UNASSIGNED int = (SELECT [Payment_Status_Type_ID] from [dbo].[TB_PAYMENT_STATUS_TYPE] WHERE [Code] = 'UNASSIGNED')
	DECLARE @PaymentStatusTypeID_ASSIGNED int = (SELECT [Payment_Status_Type_ID] from [dbo].[TB_PAYMENT_STATUS_TYPE] WHERE [Code] = 'ASSIGNED')
	DECLARE @PaymentStatusTypeID_RETURNED_TO_SUP int = (SELECT [Payment_Status_Type_ID] from [dbo].[TB_PAYMENT_STATUS_TYPE] WHERE [Code] = 'RETURNED_TO_SUP')

	--CLAIM SCHEDULE STATUS TYPES
	DECLARE @CSStatusTypeID_CREATED int = (SELECT [Claim_Schedule_Status_Type_ID] from [dbo].[TB_CLAIM_SCHEDULE_STATUS_TYPE] WHERE [Code] = 'CREATED')
	DECLARE @CSStatusTypeID_ASSIGNED int = (SELECT [Claim_Schedule_Status_Type_ID] from [dbo].[TB_CLAIM_SCHEDULE_STATUS_TYPE] WHERE [Code] = 'ASSIGNED')
	DECLARE @CSStatusTypeID_SUBMIT_FOR_APPROVAL int = (SELECT [Claim_Schedule_Status_Type_ID] from [dbo].[TB_CLAIM_SCHEDULE_STATUS_TYPE] WHERE [Code] = 'SUBMIT_FOR_APPROVAL')
	DECLARE @CSStatusTypeID_RETURN_TO_PROCESSOR int = (SELECT [Claim_Schedule_Status_Type_ID] from [dbo].[TB_CLAIM_SCHEDULE_STATUS_TYPE] WHERE [Code] = 'RETURN_TO_PROCESSOR')
	
	--ECS STATUS TYPES
	DECLARE @ECSStatusTypeID_PENDING int = (SELECT [ECS_Status_Type_ID] from [dbo].[TB_ECS_STATUS_TYPE] WHERE [Code] = 'PENDING')

	--RESULT CATEGORIES
	DECLARE @Category_PR_UNASSIGNED varchar (50) = 'Unassigned Payment Sets'
	DECLARE @Category_PR_ASSIGNED varchar (50) = 'Assigned Payment Sets'
	DECLARE @Category_CS_PENDING varchar (50) = 'Processor Queue Pending Claim Schedules'
	DECLARE @Category_CS_AWAITING_APPROVAL varchar (50) = 'Claim Schedules Awaiting Approval'
	DECLARE @Category_ECS_PENDING varchar (50) = 'Pending E-Claim Schedules'
	DECLARE @Category_PR_PENDING_RETURNS varchar (50) = 'Pending Returns'
	DECLARE @Category_PR_ON_HOLD varchar (50) = 'On Hold'

	CREATE TABLE #kvResult
	(
		[Category] varchar (50),
		[Count] int,
		[Sort] int
	)

	--UNASSIGNED CATEGORY
	INSERT INTO #kvResult
	SELECT 
		@Category_PR_UNASSIGNED, 
		COUNT(PaymentSet_Number),
		1
	FROM 
		(SELECT DISTINCT
			pr.PaymentSet_Number
		FROM TB_PAYMENT_RECORD pr 
			inner join TB_PAYMENT_DN_STATUS dnst ON pr.Payment_Record_ID = dnst.Payment_Record_ID
			inner join TB_PAYMENT_STATUS ps ON dnst.CurrentPaymentStatusID = ps.Payment_Status_ID
		WHERE 
			ps.Payment_Status_Type_ID = @PaymentStatusTypeID_UNASSIGNED) a

	--ASSIGNED CATEGORY
	INSERT INTO #kvResult
	SELECT   
		@Category_PR_ASSIGNED,
		COUNT(PaymentSet_Number),
		2
	FROM 
		(SELECT DISTINCT
			pr.PaymentSet_Number
		FROM TB_PAYMENT_RECORD pr 
			inner join TB_PAYMENT_DN_STATUS dnst ON pr.Payment_Record_ID = dnst.Payment_Record_ID
			inner join TB_PAYMENT_STATUS ps ON dnst.CurrentPaymentStatusID = ps.Payment_Status_ID
		WHERE 
			ps.Payment_Status_Type_ID = @PaymentStatusTypeID_ASSIGNED) a

	--PENDING CLAIM SCHEDULES CATEGORY
	INSERT INTO #kvResult
	SELECT   
		@Category_CS_PENDING,
		COUNT(Claim_Schedule_ID),
		3
	FROM 
		(SELECT DISTINCT
			csst.Claim_Schedule_ID
		FROM TB_CLAIM_SCHEDULE_DN_STATUS cs 
			inner join TB_CLAIM_SCHEDULE_STATUS csst ON cs.CurrentCSStatusID = csst.Claim_Schedule_Status_ID
		WHERE 
			csst.Claim_Schedule_Status_Type_ID in 
				(@CSStatusTypeID_CREATED
				, @CSStatusTypeID_ASSIGNED
				, @CSStatusTypeID_RETURN_TO_PROCESSOR)) a

	--AWAITING APPROVAL CLAIM SCHEDULES CATEGORY
	INSERT INTO #kvResult
	SELECT   
		@Category_CS_AWAITING_APPROVAL,
		COUNT(Claim_Schedule_ID),
		4
	FROM 
		(SELECT DISTINCT
			csst.Claim_Schedule_ID
		FROM TB_CLAIM_SCHEDULE_DN_STATUS cs 
			inner join TB_CLAIM_SCHEDULE_STATUS csst ON cs.CurrentCSStatusID = csst.Claim_Schedule_Status_ID
		WHERE 
			csst.Claim_Schedule_Status_Type_ID = @CSStatusTypeID_SUBMIT_FOR_APPROVAL) a
			
	--PENDING ECS CATEGORY
	INSERT INTO #kvResult
	SELECT   
		@Category_ECS_PENDING,
		COUNT(ECS_ID),
		5
	FROM 
		(SELECT DISTINCT ecs.ECS_ID
			FROM TB_ECS ecs 
			WHERE ecs.Current_ECS_Status_Type_ID = @ECSStatusTypeID_PENDING) a

	--PENDING RETURNS CATEGORY
	INSERT INTO #kvResult
	SELECT   
		@Category_PR_PENDING_RETURNS,
		COUNT(PaymentSet_Number),
		6
	FROM 
		(SELECT DISTINCT
			pr.PaymentSet_Number
		FROM TB_PAYMENT_RECORD pr 
			inner join TB_PAYMENT_DN_STATUS dnst ON pr.Payment_Record_ID = dnst.Payment_Record_ID
			inner join TB_PAYMENT_STATUS ps ON dnst.CurrentPaymentStatusID = ps.Payment_Status_ID
		WHERE 
			ps.Payment_Status_Type_ID = @PaymentStatusTypeID_RETURNED_TO_SUP) a

	--ON HOLD CATEGORY
	INSERT INTO #kvResult
	SELECT   
		@Category_PR_ON_HOLD,
		COUNT(PaymentSet_Number),
		7
	FROM 
		(SELECT DISTINCT
			pr.PaymentSet_Number
		FROM TB_PAYMENT_RECORD pr 
			inner join TB_PAYMENT_DN_STATUS dnst ON pr.Payment_Record_ID = dnst.Payment_Record_ID
			inner join TB_PAYMENT_STATUS ps ON dnst.CurrentPaymentStatusID = ps.Payment_Status_ID
		WHERE 
			dnst.CurrentHoldStatusID IS NOT NULL) a

	--SELET RESULT
	SELECT [Category],[Count] FROM #kvResult ORDER BY [Sort]

	DROP TABLE #kvResult

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
