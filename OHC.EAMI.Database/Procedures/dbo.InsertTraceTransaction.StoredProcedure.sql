USE [EAMI-MC]
GO
/****** Object:  StoredProcedure [dbo].[InsertTraceTransaction]    Script Date: 5/28/2019 10:08:03 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertTraceTransaction]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[InsertTraceTransaction]
GO
/****** Object:  StoredProcedure [dbo].[InsertTraceTransaction]    Script Date: 5/28/2019 10:08:03 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/******************************************************************************
* PROCEDURE:  [dbo].[InsertTraceTransaction]
* PURPOSE: Inserts Trace Transaction and resturns identity
* NOTES:
* CREATED: 09/19/2017  Eugene S.
* MODIFIED
* DATE     AUTHOR      DESCRIPTION
*------------------------------------------------------------------------------
*****************************************************************************/
CREATE PROCEDURE [dbo].[InsertTraceTransaction] 
	@TraceTransactionId INT
	--,@PayerEntityId VARCHAR(10)
	--,@PayerEntityIdType VARCHAR(30)
	--,@PayerEntityName VARCHAR(100)
	--,@PayerEntityIdSfx VARCHAR(10)
	--,@PayerAddressLine1 VARCHAR(80)
	--,@PayerAddressLine2 VARCHAR(80)
	--,@PayerAddressLine3 VARCHAR(80)
	--,@PayerCity VARCHAR(40)
	--,@PayerState VARCHAR(2)
	--,@PayerZip VARCHAR(10)
	,@TotalPymtAmount varchar(20)
	,@TotalPymtRecCount varchar(10)
	,@RejectedPaymentDateFrom datetime
	,@RejectedPaymentDateTo datetime
AS
BEGIN
	BEGIN TRY
		INSERT INTO [dbo].[TB_TRACE_TRANSACTION] (
			[Trace_Transaction_ID]
			,[Payer_Entity_ID]
			,[Payer_Entity_ID_Type]
			,[Payer_Entity_Name]
			,[Payer_Entity_ID_Suffix]
			,[Payer_Address_Line1]
			,[Payer_Address_Line2]
			,[Payer_Address_Line3]
			,[Payer_City]
			,[Payer_State]
			,[Payer_Zip]
			,[TotalPymtAmount]
			,[TotalPymtRecCount]
			,[RejectedPaymentDateFrom]
			,[RejectedPaymentDateTo]
			)
		VALUES (
			@TraceTransactionId
			,null --,@PayerEntityId
			,null --,@PayerEntityIdType
			,null --,@PayerEntityName
			,null --,@PayerEntityIdSfx
			,null --,@PayerAddressLine1
			,null --,@PayerAddressLine2
			,null --,@PayerAddressLine3
			,null --,@PayerCity
			,null --,@PayerState
			,null --,@PayerZip
			,@TotalPymtAmount
			,@TotalPymtRecCount
			,@RejectedPaymentDateFrom
			,@RejectedPaymentDateTo
			)

		SELECT SCOPE_IDENTITY() AS [SCOPE_IDENTITY];
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
