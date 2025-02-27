
  
/******************************************************************************  
* PROCEDURE:  [dbo].[GetPaymentRecordsByUser]  
* PURPOSE: Gets all the Payment Records associated with a given User.
* NOTES:  JIRA: EAMI-741
* CREATED: 07/07/2023  Meetu S.  
* MODIFIED  
* DATE    AUTHOR     DESCRIPTION  
*------------------------------------------------------------------------------  
....Test...
exec [GetPaymentRecordsByUser] 64
*****************************************************************************/  
ALTER PROCEDURE [dbo].[GetPaymentRecordsByUser]  
   @UserID INT
AS  
BEGIN  
 BEGIN TRY 
 --PAYMENT RECORD STATUS TYPES
	--DECLARE @PaymentStatusCode_WARRANT_RECEIVED  INT = (SELECT Payment_Status_Type_ID from [dbo].[TB_PAYMENT_STATUS_TYPE] WHERE [Code] = 'WARRANT_RECEIVED')
	--DECLARE @PaymentStatusCode_RETURNED_TO_SOR INT = (SELECT Payment_Status_Type_ID from [dbo].[TB_PAYMENT_STATUS_TYPE] WHERE [Code] = 'RETURNED_TO_SOR')
	DECLARE @PaymentStatusCode_ASSIGNED INT = (SELECT Payment_Status_Type_ID from [dbo].[TB_PAYMENT_STATUS_TYPE] WHERE [Code] = 'ASSIGNED')
	DECLARE @PaymentStatusCode_RECEIVED INT = (SELECT Payment_Status_Type_ID from [dbo].[TB_PAYMENT_STATUS_TYPE] WHERE [Code] = 'RECEIVED')
	DECLARE @PaymentStatusCode_UNASSIGNED INT = (SELECT Payment_Status_Type_ID from [dbo].[TB_PAYMENT_STATUS_TYPE] WHERE [Code] = 'UNASSIGNED')
	DECLARE @PaymentStatusCode_ADDED_TO_CS INT = (SELECT Payment_Status_Type_ID from [dbo].[TB_PAYMENT_STATUS_TYPE] WHERE [Code] = 'ADDED_TO_CS')
	DECLARE @PaymentStatusCode_SENT_TO_SCO INT = (SELECT Payment_Status_Type_ID from [dbo].[TB_PAYMENT_STATUS_TYPE] WHERE [Code] = 'SENT_TO_SCO')
	DECLARE @PaymentStatusCode_SENT_TO_CALSTARS INT = (SELECT Payment_Status_Type_ID from [dbo].[TB_PAYMENT_STATUS_TYPE] WHERE [Code] = 'SENT_TO_CALSTARS')
	DECLARE @PaymentStatusCode_RETURNED_TO_SUP INT = (SELECT Payment_Status_Type_ID from [dbo].[TB_PAYMENT_STATUS_TYPE] WHERE [Code] = 'RETURNED_TO_SUP')
	DECLARE @PaymentStatusCode_RELEASED_FROM_SUP INT = (SELECT Payment_Status_Type_ID from [dbo].[TB_PAYMENT_STATUS_TYPE] WHERE [Code] = 'RELEASED_FROM_SUP')
	DECLARE @PaymentStatusCode_HOLD INT = (SELECT Payment_Status_Type_ID from [dbo].[TB_PAYMENT_STATUS_TYPE] WHERE [Code] = 'HOLD')
	DECLARE @PaymentStatusCode_UNHOLD INT = (SELECT Payment_Status_Type_ID from [dbo].[TB_PAYMENT_STATUS_TYPE] WHERE [Code] = 'UNHOLD')

	CREATE TABLE #paymentRecordCountForUserPerStatus
	(
		UserID INT,
		Code varchar(50),
		Payment_Record_ID INT,
		--[Count] int
	)

  INSERT INTO #paymentRecordCountForUserPerStatus
  SELECT 
		 @UserID
		, Code
		, Payment_Record_ID
		--,COUNT(Payment_Record_ID)
	FROM 
	  (
		  SELECT distinct PR.Payment_Record_ID
		  , PST.Code
		  FROM TB_PAYMENT_DN_STATUS (NOLOCK) PDN  
		   INNER JOIN TB_PAYMENT_RECORD (NOLOCK) PR ON PDN.Payment_Record_ID = PR.Payment_Record_ID  
		   INNER JOIN TB_PAYMENT_STATUS (NOLOCK) PS ON pdn.CurrentPaymentStatusID = ps.Payment_Status_ID  
		   LEFT JOIN TB_PAYMENT_STATUS_TYPE (NOLOCK) PST ON PS.Payment_Status_Type_ID = PST.Payment_Status_Type_ID
		   LEFT JOIN TB_PAYMENT_USER_ASSIGNMENT (NOLOCK) UA ON UA.Payment_User_Assignment_ID = PDN.CurrentUserAssignmentID
		   LEFT JOIN TB_USER USR (NOLOCK) ON UA.[User_ID] = USR.[User_ID]
		  WHERE   
		   PST.Payment_Status_Type_ID IN (	 
											 @PaymentStatusCode_ASSIGNED
											--, @PaymentStatusCode_WARRANT_RECEIVED
											--, @PaymentStatusCode_RETURNED_TO_SOR
											, @PaymentStatusCode_RECEIVED
											, @PaymentStatusCode_UNASSIGNED 
											, @PaymentStatusCode_ADDED_TO_CS
											, @PaymentStatusCode_SENT_TO_SCO
											, @PaymentStatusCode_SENT_TO_CALSTARS 
											, @PaymentStatusCode_RETURNED_TO_SUP
											, @PaymentStatusCode_RELEASED_FROM_SUP
											, @PaymentStatusCode_HOLD
											, @PaymentStatusCode_UNHOLD
										 )
		   AND UA.[User_ID] IN (@UserID)  
		   AND USR.IsActive = 1
	   )  a
	  -- GROUP BY Payment_Record_ID, Code

 SELECT * FROM #paymentRecordCountForUserPerStatus

 DROP TABLE #paymentRecordCountForUserPerStatus

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