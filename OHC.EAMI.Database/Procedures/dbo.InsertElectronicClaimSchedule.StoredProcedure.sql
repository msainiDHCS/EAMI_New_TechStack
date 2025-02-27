USE [EAMI-PRX]
GO
/****** Object:  StoredProcedure [dbo].[InsertElectronicClaimSchedule]    Script Date: 5/28/2019 10:08:03 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertElectronicClaimSchedule]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[InsertElectronicClaimSchedule]
GO
/****** Object:  StoredProcedure [dbo].[InsertElectronicClaimSchedule]    Script Date: 5/28/2019 10:08:03 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/******************************************************************************
* PROCEDURE:  [dbo].[InsertElectronicClaimSchedule]
* PURPOSE: Insert ECS (Electronic Claim Schedule) record with Pending Status and associated CS ids
* NOTES:
* CREATED: 03/01/2018  Eugene S.
* MODIFIED
* DATE     AUTHOR      DESCRIPTION
*------------------------------------------------------------------------------
* 11/13/2018, Eugene S, added auto-generation of ESC number via separate sequence table
* 02/05/2019, Genady G, add @StatusDate parameter
*****************************************************************************/
CREATE PROCEDURE [dbo].[InsertElectronicClaimSchedule] 
	@ClaimScheduleIDList VARCHAR(max)
	,@ExclusivePaymentTypeID INT
	,@PayDate DATETIME	
	,@Amount MONEY
	,@userName varchar(20)
	,@CurrentStatusNote varchar(200) = null
	,@StatusDate DATETIME
	,@PaymentMethodTypeID INT
AS
BEGIN
	BEGIN TRY		
		--NOTE: the transaction is managed from the application code calling this SP

		--1. insert into ECS sequence table and use scope_identity as ECS number
		INSERT INTO [dbo].[TB_ECS_Sequence] ([UpdateDate])
		VALUES (@StatusDate)
		DECLARE @EcsNumber VARCHAR(20)
		SET @EcsNumber = Cast(SCOPE_IDENTITY() as VARCHAR(20));
		
		--2. insert ecs record
		INSERT INTO [dbo].[TB_ECS] (
			[ECS_Number]
			,[Exclusive_Payment_Type_ID]
			,[PayDate]
			,[Amount]
			,[CreateDate]
			,[CreatedBy]
			,[Current_ECS_Status_Type_ID]
			,[CurrentStatusDate]
			,[CurrentStatusNote]
			,[Payment_Method_Type_ID]
			)
		VALUES (
			@EcsNumber
			,@ExclusivePaymentTypeID
			,@PayDate
			,@Amount
			,@StatusDate
			,@userName
			,(select ECS_STATUS_TYPE_ID from TB_ECS_STATUS_TYPE where CODE = 'PENDING')
			,@StatusDate
			,@CurrentStatusNote
			,@PaymentMethodTypeID
			)

		DECLARE @ecsId INT
		SET @ecsId = SCOPE_IDENTITY()

		--3. insert ecs to cs mapping
		DECLARE @xml XML
		SET @xml = N'<root><r>' + replace(@ClaimScheduleIDList, ',', '</r><r>') + '</r></root>'

		INSERT INTO [dbo].[TB_CLAIM_SCHEDULE_ECS] (
			[Claim_Schedule_ID]
			,[ECS_ID]
			)
		SELECT CS.Claim_Schedule_ID
			,@ecsId
		FROM TB_CLAIM_SCHEDULE CS(NOLOCK)
		WHERE CS.Claim_Schedule_ID IN (
				SELECT t.value('.', 'int') AS [Claim_Schedule_ID]
				FROM @xml.nodes('//root/r') AS a(t)
				)

		SELECT @ecsId as [SCOPE_IDENTITY]

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
