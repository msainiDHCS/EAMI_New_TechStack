USE [EAMI-PRX]
GO
/****** Object:  StoredProcedure [dbo].[GetPymtSubmissionTransForNotification]    Script Date: 5/28/2019 10:08:03 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetPymtSubmissionTransForNotification]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetPymtSubmissionTransForNotification]
GO
/****** Object:  StoredProcedure [dbo].[GetPymtSubmissionTransForNotification]    Script Date: 5/28/2019 10:08:03 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/******************************************************************************
* PROCEDURE:  [dbo].[GetPymtSubmissionTransForNotification]
* PURPOSE: Gets payment submission transaction for user notification 
* NOTES:
* CREATED: 10/25/2018  Eugene S.
* MODIFIED
* DATE     AUTHOR      DESCRIPTION
*------------------------------------------------------------------------------
*****************************************************************************/
CREATE PROCEDURE [dbo].[GetPymtSubmissionTransForNotification]
AS
BEGIN
	BEGIN TRY
					
		-- select records using xml
		SELECT 
			T.Transaction_ID
			,T.Msg_Transaction_ID
			,RSP.Response_TimeStamp as SubmissionDateTime
			,RST.Code as StatusCode
			,SOR.Code as SystemCode
			,TT.Code as TransactionType
			,count(P.Payment_Record_ID) PymtRecCount
			,count(DISTINCT P.PaymentSet_Number) PymtSetCount
		FROM TB_REQUEST REQ
		INNER JOIN TB_RESPONSE RSP ON RSP.Response_ID = REQ.Request_ID
		INNER JOIN TB_RESPONSE_STATUS_TYPE RST ON RST.Response_Status_Type_ID = RSP.Response_Status_Type_ID
		INNER JOIN TB_TRANSACTION T ON T.Transaction_ID = REQ.Request_ID
		INNER JOIN TB_SYSTEM_OF_RECORD SOR ON SOR.SOR_ID = T.SOR_ID
		INNER JOIN TB_TRANSACTION_TYPE TT ON TT.Transaction_Type_ID = T.Transaction_Type_ID
		INNER JOIN TB_PAYMENT_RECORD P ON P.Transaction_ID = T.Transaction_ID
		WHERE TT.Code = 'PaymentSubmissionRequest'
			AND ( RST.Code = 'ACCEPTED' OR RST.Code = 'ACCEPTED-PARTIALLY' )
			AND T.UsersNotified = 0
		GROUP BY T.Transaction_ID
			,T.Msg_Transaction_ID
			,RSP.Response_TimeStamp
			,RST.Code
			,SOR.Code
			,TT.Code
		ORDER BY RSP.Response_TimeStamp ASC

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
