USE [EAMI-RX2]
GO

-- 1. insert into TB_TRANSACTION_TYPE
-- 2. insert into TB_SYSTEM_OF_RECORD
-- 3. insert into TB_SOR_KVP_KEY
-- 4. insert into TB_RESPONSE_STATUS_TYPE
-- 5. insert into TB_PAYMENT_STATUS_TYPE
-- 6. insert into TB_DB_SETTING_TYPE
-- 7. insert into TB_DB_SETTING
-- 8. insert into TB_PAYDATE_CALENDAR
-- 9. insert into TB_DRAWDATE_CALENDAR
-- 10. insert into TB_PAYMENT_EXCHANGE_ENTITY
-- 11. insert into TB_CLAIM_SCHEDULE_TYPE
-- 12. insert into TB_PAYMENT_STATUS_TYPE_EXTERNAL
-- 13. insert into TB_PAYMENT_STATUS_TYPE_INTERNAL_TO_EXTERNAL
-- 14. insert into TB_ECS_STATUS_TYPE
-- 15. insert into TB_EXCLUSIVE_PAYMENT_TYPE

-- 16. insert into TB_ROLE
-- 17. insert into TB_PERMISSION
-- 18. insert into TB_ROLE_PERMISSION
-- 19. insert into TB_SYSTEM
-- 20. insert into TB_USER_TYPE
-- 21. insert into TB_USER
-- 22. insert into TB_USER_ROLE
-- 23. insert into TB_SYSTEM_USER


-- special (program specific) handling of this table (has to start with 10050001 value for RX)
DELETE FROM TB_ECS_SEQUENCE
DBCC CHECKIDENT ('TB_ECS_SEQUENCE',RESEED, 10050001)
GO

-- 1. insert into TB_TRANSACTION_TYPE
INSERT INTO [dbo].[TB_TRANSACTION_TYPE] (
	[Code]
	,[Description]
	,[IsActive]
	,[Sort_Value]
	,[CreateDate]
	,[CreatedBy]
	,[UpdateDate]
	,[UpdatedBy]
	)
VALUES
	('PaymentSubmissionRequest', null, 1, 1, getdate(), 'system', null, null),
	('PaymentSubmissionResponse', null, 1, 2, getdate(), 'system', null, null),
	('StatusInquiryRequest', null, 1, 3, getdate(), 'system', null, null),
	('StatusInquiryResponse', null, 1, 4, getdate(), 'system', null, null),
	('RejectedPaymentInquiryRequest', null, 1, 5, getdate(), 'system', null, null),
	('RejectedPaymentInquiryResponse', null, 1, 6, getdate(), 'system', null, null)           
GO


-- 2. insert into TB_SYSTEM_OF_RECORD
INSERT INTO [dbo].[TB_SYSTEM_OF_RECORD] (
	[Code]
	,[Description]
	,[IsActive]
	,[Sort_Value]
	,[CreateDate]
	,[CreatedBy]
	,[UpdateDate]
	,[UpdatedBy]
	)
VALUES
	('MEDICAL_RX', null, 1, 1, getdate(), 'system', null, null)     
GO


-- 3. insert into TB_SOR_KVP_KEY
INSERT INTO [dbo].[TB_SOR_KVP_KEY] (
	[SOR_ID]
	,[Kvp_Key_Name]
	,[Kvp_Value_DataType]
	,[Kvp_Value_Length]
	,[Kvp_Description]
	,[OwnerEntity]
	,[IsActive]
	,[Sort_Value]
	,[CreateDate]
	,[CreatedBy]
	,[UpdateDate]
	,[UpdatedBy]
	)
VALUES
	(1, 'CONTRACT_NUMBER', 'TEXT', 20, 'Contract Number', 'PaymentRec', 1, 1, getdate(), 'system', null, null),
	(1, 'CONTRACT_DATE_FROM', 'DATE', 22, 'Contract effective date from', 'PaymentRec', 1, 2, getdate(), 'system', null, null),
	(1, 'CONTRACT_DATE_TO', 'DATE', 22, 'Contract effective date to', 'PaymentRec', 1, 3, getdate(), 'system', null, null),
	(1, 'EXCLUSIVE_PYMT_CODE', 'TEXT', 5, 'Exclusive payment code', 'PaymentRec', 1, 4, getdate(), 'system', null, null)
GO


-- 4. insert into TB_RESPONSE_STATUS_TYPE
INSERT INTO [dbo].[TB_RESPONSE_STATUS_TYPE] (
	[Code]
	,[Description]
	,[IsActive]
	,[Sort_Value]
	,[CreateDate]
	,[CreatedBy]
	,[UpdateDate]
	,[UpdatedBy]
	)
VALUES
	('REJECTED', null, 1, 1, getdate(), 'system', null, null),
	('ACCEPTED', null, 1, 2, getdate(), 'system', null, null),
	('ACCEPTED-PARTIALLY', null, 1, 3, getdate(), 'system', null, null)
GO


-- 5. insert into TB_PAYMENT_STATUS_TYPE
INSERT INTO [dbo].[TB_PAYMENT_STATUS_TYPE] (
	[Code]
	,[Description]
	,[IsActive]
	,[Sort_Value]
	,[CreateDate]
	,[CreatedBy]
	,[UpdateDate]
	,[UpdatedBy]
	)
VALUES 
    ('RECEIVED', 'Valid payment record received', 1, 1, getdate(), 'system', null, null),
	('UNASSIGNED', 'Payment ready for assignment', 1, 2, getdate(), 'system', null, null),
	('ASSIGNED', 'Payment (with paydate) assigned to processor', 1, 3, getdate(), 'system', null, null),
	('ADDED_TO_CS', 'Payment is added to Claim Schedule', 1, 4, getdate(), 'system', null, null),
	('SENT_TO_SCO', 'PaymentRec/ClaimSchedule/ESC sent to SCO', 1, 5, getdate(), 'system', null, null),
	('WARRANT_RECEIVED', 'Warrant info received from SCO', 1, 6, getdate(), 'system', null, null),
	('SENT_TO_CALSTARS', 'PaymentRec/ClaimSchedule info sent to CalStars', 1, 7, getdate(), 'system', null, null),
	('RETURNED_TO_SOR', 'Payment rejected by Accounting or SCO', 1, 8, getdate(), 'system', null, null),
	('RETURNED_TO_SUP', 'Payment sent to sup for approval to be rejected back to SOR', 1, 9, getdate(), 'system', null, null),
	('RELEASED_FROM_SUP', 'Payment declined by sup and routed back to processor', 1, 10, getdate(), 'system', null, null),
	('HOLD', 'Payment placed on Hold by sup or processor', 1, 11, getdate(), 'system', null, null),
	('UNHOLD', 'Payment released from Hold', 1, 12, getdate(), 'system', null, null)

GO


-- 6. insert into TB_DB_SETTING_TYPE
INSERT INTO [dbo].[TB_DB_SETTING_TYPE] (
	[Code]
	,[Description]
	,[IsActive]
	,[Sort_Value]
	,[CreateDate]
	,[CreatedBy]
	,[UpdateDate]
	,[UpdatedBy]
	)
VALUES
	('GENERAL_SINGLE_KVP', 'Generic key value pair setting', 1, 1, getdate(), 'system', null, null)
GO


-- 7. insert into TB_DB_SETTING
INSERT INTO [dbo].[TB_DB_SETTING] (
	[DB_Setting_Key]
	,[DB_Setting_Value]
	,[DB_Setting_Type_ID]
	,[Description]
	,[IsActive]
	,[Sort_Value]
	,[CreateDate]
	,[CreatedBy]
	,[UpdateDate]
	,[UpdatedBy]
	)
VALUES
	('MAX_PYMT_REC_AMOUNT', '99999999.99', 1, 'max amount per pymt rec', 1, 0, getdate(), 'system', null, null),
	('MAX_PYMT_REC_PER_TRAN', '2000', 1, 'max pymt rec lines per submit transaaction', 1, 0, getdate(), 'system', null, null),
	('MAX_FUNDING_DTL_PER_PYMT_REC', '200', 1, 'max funding detail lines per payment rec', 1, 0, getdate(), 'system', null, null),
	('TRACE_INCOMING_PAYMENT_DATA', 'false', 1, 'Switch to enable persistence of all incoming data', 1, 0, getdate(), 'system', null, null),	
	('RA_DEPARTMENT_NAME', 'DEPT. OF HEALTH CARE SERVICES', 1, 'Department Name on Remittance Advice', 1, 0, getdate(), 'system', null, null),
	('RA_DEPARTMENT_ADDR_LINE', 'PO BOX 997415 MS 1101', 1, 'Department Address line 1 on Remittance Advice', 1, 0, getdate(), 'system', null, null),
	('RA_DEPARTMENT_ADDR_CSZ', 'SACRAMENTO CA 95899-7415', 1, 'Department Address CSZ on Remittance Advice', 1, 0, getdate(), 'system', null, null),
	('RA_ORGANIZATION_CODE', '4260', 1, 'Organization code for Remittance Advice', 1, 0, getdate(), 'system', null, null),
	('ECS_MAX_TOTAL_AMOUNT', '999999999999', 1, 'Electronic claim schedule max total allowed amount', 1, 0, getdate(), 'system', null, null),
	('ECS_MAX_RECORD_COUNT', '100', 1, 'Electronic claim schedulek,.m ,m  max record count', 1, 0, getdate(), 'system', null, null),
	('CS_MAX_TOTAL_AMOUNT', '9999999999', 1, 'Claim schedule max total allowed amount', 1, 0, getdate(), 'system', null, null),
	('FS_FUND_NAME', 'HEALTH CARE DEPOSIT FUND', 1, 'Face Sheet Fund Name', 1, 0, getdate(), 'system', null, null),
	('FS_FUND_NUMBER', '0912', 1, 'Face sheet fund number', 1, 0, getdate(), 'system', null, null),
	('FS_YEAR_OF_START', '1965', 1, 'Face sheet year of start', 1, 0, getdate(), 'system', null, null),
	('FS_REF_ITEM_NUMBER', '601', 1, 'Face sheet Reference Item Number', 1, 0, getdate(), 'system', null, null),
	('RA_INQUIRIES_PHONE_NUMBER', '(916) 552-8400', 1, 'Payment Inquiries Phone Number', 1, 0, getdate(), 'system', null, null),

	-- SCO Claim ID
	('SCO_CLAIM_ID_FUND_0912_EFT', 'DHCSMEDRXE', 1, 'SCO Claim Identifier for fund 0912 EFT', 1, 0, getdate(), 'system', null, null),
	('SCO_CLAIM_ID_FUND_0555_EFT', 'DHCSMEDRXH', 1, 'SCO Claim Identifier for fund 0555 EFT', 1, 0, getdate(), 'system', null, null),
	('SCO_CLAIM_ID_FUND_0236_EFT', 'DHCSMEDRXT', 1, 'SCO Claim Identifier for fund 0236 EFT', 1, 0, getdate(), 'system', null, null),
	('SCO_CLAIM_ID_FUND_0001_EFT', 'DHCSMEDRXG', 1, 'SCO Claim Identifier for fund 0001 EFT', 1, 0, getdate(), 'system', null, null),

	-- ECS File Name
	('ECS_FILE_NAME_FUND_0912_PROD', 'D{0}.MRXE.INPUT', 1, 'ECS Prod file name for fund 0912', 1, 0, getdate(), 'system', null, null),
	('ECS_FILE_NAME_FUND_0555_PROD', 'D{0}.MRXH.INPUT', 1, 'ECS Prod file name for fund 0555', 1, 0, getdate(), 'system', null, null),
	('ECS_FILE_NAME_FUND_0912_TEST', 'D{0}.MRXE.WARRANT', 1, 'ECS Test file name for fund 0912', 1, 0, getdate(), 'system', null, null),
	('ECS_FILE_NAME_FUND_0555_TEST', 'D{0}.MRXH.WARRANT', 1, 'ECS Test file name for fund 0555', 1, 0, getdate(), 'system', null, null),
		
	-- EFT File Name
	('EFT_FILE_NAME_FUND_0912_PROD', 'D{0}.MRXE.INPUT', 1, 'EFT Prod file name for fund 0912', 1, 0, getdate(), 'system', null, null),
	('EFT_FILE_NAME_FUND_0555_PROD', 'D{0}.MRXH.INPUT', 1, 'EFT Prod file name for fund 0555', 1, 0, getdate(), 'system', null, null),
	('EFT_FILE_NAME_FUND_0236_PROD', 'D{0}.MRXT.INPUT', 1, 'EFT Prod file name for fund 0236', 1, 0, getdate(), 'system', null, null),
	('EFT_FILE_NAME_FUND_0001_PROD', 'D{0}.MRXG.INPUT', 1, 'EFT Prod file name for fund 0001', 1, 0, getdate(), 'system', null, null),
	('EFT_FILE_NAME_FUND_0912_TEST', 'D{0}.MRXE.EFT', 1, 'EFT Test file name for fund 0912', 1, 0, getdate(), 'system', null, null),
	('EFT_FILE_NAME_FUND_0555_TEST', 'D{0}.MRXH.EFT', 1, 'EFT Test file name for fund 0555', 1, 0, getdate(), 'system', null, null),
	('EFT_FILE_NAME_FUND_0236_TEST', 'D{0}.MRXT.EFT', 1, 'EFT Test file name for fund 0236', 1, 0, getdate(), 'system', null, null),
	('EFT_FILE_NAME_FUND_0001_TEST', 'D{0}.MRXG.EFT', 1, 'EFT Test file name for fund 0001', 1, 0, getdate(), 'system', null, null)
	

GO


-- 8. insert into TB_PAYDATE_CALENDAR
INSERT INTO [dbo].[TB_PAYDATE_CALENDAR] (
	[Paydate]
	,[Note]
	,[IsActive]
	,[CreateDate]
	,[CreatedBy]
	,[UpdateDate]
	,[UpdatedBy]
	)
VALUES
	(CONVERT(date, DATEADD(day, 1, getdate())), null, 1, getdate(), 'system', null, null)
	--(CONVERT(date, DATEADD(day, 3, getdate())), null, 1, getdate(), 'system', null, null),
	--(CONVERT(date, DATEADD(day, 5, getdate())), null, 1, getdate(), 'system', null, null),
	--(CONVERT(date, DATEADD(day, 8, getdate())), null, 1, getdate(), 'system', null, null),
	--(CONVERT(date, DATEADD(day, 11, getdate())), null, 1, getdate(), 'system', null, null),
	--(CONVERT(date, DATEADD(day, 14, getdate())), null, 1, getdate(), 'system', null, null),
	--(CONVERT(date, DATEADD(day, 16, getdate())), null, 1, getdate(), 'system', null, null),
	--(CONVERT(date, DATEADD(day, 19, getdate())), null, 1, getdate(), 'system', null, null)
GO

-- 9. insert into TB_DRAWDATE_CALENDAR
INSERT INTO [dbo].[TB_DRAWDATE_CALENDAR] (
	[Drawdate]
	,[Note]
	)
VALUES 
	(CONVERT(date, getdate()), NULL)
	--(CONVERT(date, DATEADD(day, 1, getdate())), NULL),
	--(CONVERT(date, DATEADD(day, 2, getdate())), NULL),
	--(CONVERT(date, DATEADD(day, 6, getdate())), NULL),
	--(CONVERT(date, DATEADD(day, 9, getdate())), NULL),
	--(CONVERT(date, DATEADD(day, 12, getdate())), NULL)
GO

-- 10. insert into TB_PAYMENT_EXCHANGE_ENTITY
INSERT INTO [dbo].[TB_PAYMENT_EXCHANGE_ENTITY]
    ([Entity_ID]
    ,[Entity_ID_Type]
    ,[Entity_Name]
    ,[Entity_EIN]
    ,[IsActive]
    ,[Sort_Value]
    ,[CreateDate]
    ,[CreatedBy]
    ,[UpdateDate]
    ,[UpdatedBy])
VALUES
	('VC99999999', 'VENDOR_CODE', 'Vendor Name', '999999999', 1, 0, getdate(), 'system', null, null)
GO

-- 11. insert into TB_CLAIM_SCHEDULE_TYPE
INSERT INTO [dbo].[TB_CLAIM_SCHEDULE_STATUS_TYPE] (
	[Code]
	,[Description]
	,[IsActive]
	,[Sort_Value]
	,[CreateDate]
	,[CreatedBy]
	,[UpdateDate]
	,[UpdatedBy]
	)
VALUES
	('CREATED', 'Claim Schedule is created', 1, 1, getdate(), 'system', null, null),
	('ASSIGNED', 'CS assigned to processor', 1, 2, getdate(), 'system', null, null),
	('SUBMIT_FOR_APPROVAL', 'CS submitted for approval', 1, 3, getdate(), 'system', null, null),
	('RETURN_TO_PROCESSOR', 'CS returned to processor', 1, 4, getdate(), 'system', null, null),
	('APPROVED', 'CS approved by sup and ready to be sent to sco', 1, 5, getdate(), 'system', null, null),	
	('SENT_TO_SCO', 'CS sent to sco', 1, 6, getdate(), 'system', null, null),	
	('WARRANT_RECEIVED', 'CS Warrant info received', 1, 7, getdate(), 'system', null, null),
	('SENT_TO_CALSTARS', 'CS sent to CalStars', 1, 8, getdate(), 'system', null, null)
GO

-- 12. insert into TB_PAYMENT_STATUS_TYPE_EXTERNAL
INSERT INTO [dbo].[TB_PAYMENT_STATUS_TYPE_EXTERNAL] (
	[Code]
	,[Description]
	,[IsActive]
	,[Sort_Value]
	,[CreateDate]
	,[CreatedBy]
	,[UpdateDate]
	,[UpdatedBy]
	)
VALUES
	('RECEIVED', 'Valid payment record received', 1, 1, getdate(), 'system', null, null),
	('PROCESSING', 'Payment is being processed by Accounting', 1, 2, getdate(), 'system', null, null),
	('SENT_TO_SCO', 'Payment/ClaimSchedule sent to SCO', 1, 3, getdate(), 'system', null, null),	
	('WARRANT_RECEIVED', 'Payment/CS is paid and warrant info received', 1, 4, getdate(), 'system', null, null),
	('SENT_TO_CALSTARS', 'Payment/ClaimSchedule sent to CalStars', 1, 5, getdate(), 'system', null, null),
	('REJECTED', 'Payment rejected by accounting or SCO', 1, 6, getdate(), 'system', null, null)
GO


-- 13. inert into TB_PAYMENT_STATUS_TYPE_INTERNAL_TO_EXTERNAL
INSERT INTO [dbo].[TB_PAYMENT_STATUS_TYPE_INTERNAL_TO_EXTERNAL]
SELECT PST.Payment_Status_Type_ID
	,CASE
		WHEN PST.Code IN ('RECEIVED')
			THEN (SELECT Payment_Status_Type_Ext_ID FROM TB_PAYMENT_STATUS_TYPE_EXTERNAL WHERE Code = 'RECEIVED')		
		WHEN PST.Code in ('SENT_TO_SCO')
			THEN (SELECT Payment_Status_Type_Ext_ID FROM TB_PAYMENT_STATUS_TYPE_EXTERNAL WHERE Code = 'SENT_TO_SCO')
		WHEN PST.Code in ('WARRANT_RECEIVED', 'SENT_TO_CALSTARS')
			THEN (SELECT Payment_Status_Type_Ext_ID FROM TB_PAYMENT_STATUS_TYPE_EXTERNAL WHERE Code = 'WARRANT_RECEIVED')		
		WHEN PST.Code = 'RETURNED_TO_SOR'
			THEN (SELECT Payment_Status_Type_Ext_ID FROM TB_PAYMENT_STATUS_TYPE_EXTERNAL WHERE Code = 'REJECTED')
		ELSE (SELECT Payment_Status_Type_Ext_ID FROM TB_PAYMENT_STATUS_TYPE_EXTERNAL WHERE Code = 'PROCESSING')
		END AS Payment_Status_Type_Ext_ID
FROM TB_PAYMENT_STATUS_TYPE PST
order by 1 asc
GO

-- 14. insert into TB_ECS_STATUS_TYPE_ID	
INSERT INTO [dbo].[TB_ECS_STATUS_TYPE] 
	([CODE]
	,[Description]
	,[IsActive]
	,[Sort_Value]
	,[CreateDate]
	,[CreatedBy]
	,[UpdateDate]
	,[UpdatedBy]
	)
VALUES
	('PENDING', 'ESC is defined pending approval and upload', 1, 0, getdate(), 'system', null, null),
	('APPROVED', 'ESC payment package is approved', 1, 0, getdate(), 'system', null, null),
	('SENT_TO_SCO', 'ESC file is created and sent to SCO ', 1, 0, getdate(), 'system', null, null),
	('WARRANT_RECEIVED', 'ECS payments are paid and warrant info received', 1, 0, getdate(), 'system', null, null),
	('FAIL', 'Error while generating or uploading ESC file', 1, 0, getdate(), 'system', null, null),
	('REJECTED', 'SCO rejected the ECS file', 1, 0, getdate(), 'system', null, null)
GO


-- 15. insert into TB_EXCLUSIVE_PAYMENT_TYPE
INSERT INTO [dbo].[TB_EXCLUSIVE_PAYMENT_TYPE]
    ([Code]
    ,[Description]
    ,[IsActive]
    ,[Sort_Value]
    ,[CreateDate]
    ,[CreatedBy]
    ,[UpdateDate]
    ,[UpdatedBy])
VALUES
	('NONE', 'Payment Type associated with fund 0912', 1, 1, getdate(), 'SYSTEM', null, null),
	('SCHIP', 'Payment Type associated with fund 0555', 1, 2, getdate(), 'SYSTEM', null, null),
	('PCFH', 'Payment Type associated with fund 0001', 1, 3, getdate(), 'SYSTEM', NULL, NULL),
	('EWC', 'Payment Type associated with fund 0236', 0, 4, getdate(), 'SYSTEM', NULL, NULL),
	('GF', 'Payment Type associated with fund 0001', 0, 5, getdate(), 'SYSTEM', NULL, NULL)
GO


-- 16. insert into TB_ROLE
INSERT INTO [dbo].[TB_ROLE]
    ([Role_Code]
    ,[Role_Name]
    ,[Sort_Order]
    ,[IsActive]
    ,[CreateDate]
    ,[CreatedBy]
    ,[UpdateDate]
    ,[UpdatedBy])
VALUES
	('ADMIN', 'Administrator', 1, 1, getdate(), 'SYSTEM', NULL, NULL),
	('SUPERVISOR', 'Supervisor', 2, 1, getdate(), 'SYSTEM', NULL, NULL),
	('PROCESSOR', 'Processor', 3, 1, getdate(), 'SYSTEM', NULL, NULL)
GO

-- 17. insert into TB_PERMISSION
INSERT INTO [dbo].[TB_PERMISSION]
    ([Permission_Code]
    ,[Permission_Name]
    ,[Sort_Order]
    ,[IsActive]
    ,[CreateDate]
    ,[CreatedBy]
    ,[UpdateDate]
    ,[UpdatedBy])
VALUES
	('APP_ADMIN', 'Application and User Access Administration', 1, 1, getdate(), 'SYSTEM', NULL, NULL),
	('CS_CREATE', 'Create Claim Schedule', 2, 1, getdate(), 'SYSTEM', NULL, NULL),
	('CS_ASSIGN', 'Assign Claim Schedule', 3, 1, getdate(), 'SYSTEM', NULL, NULL)
GO

-- 18. insert into TB_ROLE_PERMISSION
INSERT INTO [dbo].[TB_ROLE_PERMISSION]
	([Role_ID]
	,[Permission_ID]
	,[Sort_Order]
	,[IsActive]
	,[CreateDate]
	,[CreatedBy]
	,[UpdateDate]
	,[UpdatedBy])
VALUES
	((SELECT Role_ID from TB_ROLE where Role_Code = 'SUPERVISOR'), 
		(SELECT Permission_ID from TB_PERMISSION where [Permission_Code] = 'CS_CREATE'), 1, 1, getdate(), 'SYSTEM', NULL, NULL),
	((SELECT Role_ID from TB_ROLE where Role_Code = 'SUPERVISOR'), 
		(SELECT Permission_ID from TB_PERMISSION where [Permission_Code] = 'CS_ASSIGN'), 1, 1, getdate(), 'SYSTEM', NULL, NULL),
	((SELECT Role_ID from TB_ROLE where Role_Code = 'PROCESSOR'), 
		(SELECT Permission_ID from TB_PERMISSION where [Permission_Code] = 'CS_CREATE'), 1, 1, getdate(), 'SYSTEM', NULL, NULL),
	((SELECT Role_ID from TB_ROLE where Role_Code = 'ADMIN'), 
		(SELECT Permission_ID from TB_PERMISSION where [Permission_Code] = 'CS_ASSIGN'), 1, 1, getdate(), 'SYSTEM', NULL, NULL),
	((SELECT Role_ID from TB_ROLE where Role_Code = 'ADMIN'), 
		(SELECT Permission_ID from TB_PERMISSION where [Permission_Code] = 'CS_CREATE'), 1, 1, getdate(), 'SYSTEM', NULL, NULL),
	((SELECT Role_ID from TB_ROLE where Role_Code = 'ADMIN'), 
		(SELECT Permission_ID from TB_PERMISSION where [Permission_Code] = 'APP_ADMIN'), 1, 1, getdate(), 'SYSTEM', NULL, NULL)
GO


-- 19. insert into TB_SYSTEM
INSERT INTO [dbo].[TB_SYSTEM]
    ([System_Code]
    ,[System_Name]
    ,[Sort_Order]
    ,[IsActive]
    ,[CreateDate]
    ,[CreatedBy]
    ,[UpdateDate]
    ,[UpdatedBy])
VALUES
	('MEDICAL_RX', 'MEDICAL_RX', 1, 1, getdate(), 'SYSTEM', NULL, NULL)
GO

-- 20. insert into TB_USER_TYPE
INSERT INTO [dbo].[TB_USER_TYPE]
    ([User_Type_Code]
    ,[User_Type_Name]
    ,[Sort_Order]
    ,[IsActive]
    ,[CreateDate]
    ,[CreatedBy]
    ,[UpdateDate]
    ,[UpdatedBy])
VALUES
	('AD', 'Active Directory', 1, 1, getdate(), 'SYSTEM', NULL, NULL),
	('UNPD', 'User Name and Password', 2, 1, getdate(), 'SYSTEM', NULL, NULL)
GO

-- 21. insert into TB_USER
INSERT INTO [dbo].[TB_USER]
    ([User_Name]
    ,[Display_Name]
    ,[User_EmailAddr]
    ,[User_Password]
    ,[User_Type_ID]
    ,[Domain_Name]
    ,[Sort_Order]
    ,[IsActive]
    ,[CreateDate]
    ,[CreatedBy]
    ,[UpdateDate]
    ,[UpdatedBy])
VALUES
	--('administrator', 'administrator', NULL, 'notapassword', 2, NULL, 1, 1, getdate(), 'SYSTEM', NULL, NULL),
	('ahoang2', 'ahoang2', 'Alex.Hoang@dhcs.ca.gov', NULL, 1, 'DHSINTRA', 2, 1, getdate(), 'SYSTEM', NULL, NULL),
	('esamoylo', 'esamoylo', 'Eugene.Samoylovich@dhcs.ca.gov', NULL, 1, 'DHSINTRA', 3, 1, getdate(), 'SYSTEM', NULL, NULL),
	('gGidenko', 'gGidenko', 'Genady.Gidenko@dhcs.ca.gov', NULL, 1, 'DHSINTRA', 4, 1, getdate(), 'SYSTEM', NULL, NULL),	
	('lfong', 'lfong', 'Leslie.Fong@dhcs.ca.gov', NULL, 1, 'DHSINTRA', 6, 1, getdate(), 'SYSTEM', NULL, NULL),	
	('rwoolf', 'rwoolf', 'Robert.Woolf@dhcs.ca.gov', NULL, 1, 'DHSINTRA', 7, 1, getdate(), 'SYSTEM', NULL, NULL),
	('wvien', 'wvien', 'William.Vien@dhcs.ca.gov', NULL, 1, 'DHSINTRA', 8, 1, getdate(), 'SYSTEM', NULL, NULL),
	('RSarpal', 'RSarpal', 'Ravi.Sarpal@dhcs.ca.gov', NULL, 1, 'DHSINTRA', 9, 1, getdate(), 'SYSTEM', NULL, NULL),
	('jwang', 'jwang', 'Joanna.Wang@dhcs.ca.gov', NULL, 1, 'DHSINTRA', 10, 1, getdate(), 'SYSTEM', NULL, NULL),

	-- accounting users	
	('JMowrer', 'JMowrer', 'James.Mowrer@dhcs.ca.gov', NULL, 1, 'DHSINTRA', 20, 1, getdate(), 'SYSTEM', NULL, NULL),
	('VLy', 'VLy', 'Vivian.Ly@dhcs.ca.gov', NULL, 1, 'DHSINTRA', 21, 1, getdate(), 'SYSTEM', NULL, NULL)		
GO

-- 22. insert into TB_USER_ROLE
-- insert users with max (Admin/supervisors/processor) role 
INSERT INTO [dbo].[TB_USER_ROLE]
    ([User_ID]
    ,[Role_ID]
    ,[Sort_Order]
    ,[IsActive]
    ,[CreateDate]
    ,[CreatedBy]
    ,[UpdateDate]
    ,[UpdatedBy])
SELECT USR.[User_ID], RL.Role_ID, USR.[User_ID] as Sort_Order, 1, getdate(), 'SYSTEM', NULL, NULL
FROM TB_USER USR, TB_ROLE RL
WHERE USR.[User_Name] in ('ahoang2', 'esamoylo', 'gGidenko',  
			'lfong', 'rwoolf', 'wvien', 'RSarpal',
			'jwang')
ORDER BY USR.[User_ID]
GO

-- insert users with only supervisor role
INSERT INTO [dbo].[TB_USER_ROLE]
    ([User_ID]
    ,[Role_ID]
    ,[Sort_Order]
    ,[IsActive]
    ,[CreateDate]
    ,[CreatedBy]
    ,[UpdateDate]
    ,[UpdatedBy])
SELECT USR.[User_ID], RL.Role_ID, USR.[User_ID] as Sort_Order, 1, getdate(), 'SYSTEM', NULL, NULL
FROM TB_USER USR
INNER JOIN TB_ROLE RL on RL.Role_Code = 'SUPERVISOR'
WHERE USR.[User_Name] in ('JMowrer')
ORDER BY USR.[User_ID]
GO

-- insert users with only processor role
INSERT INTO [dbo].[TB_USER_ROLE]
    ([User_ID]
    ,[Role_ID]
    ,[Sort_Order]
    ,[IsActive]
    ,[CreateDate]
    ,[CreatedBy]
    ,[UpdateDate]
    ,[UpdatedBy])
SELECT USR.[User_ID], RL.Role_ID, USR.[User_ID] as Sort_Order, 1, getdate(), 'SYSTEM', NULL, NULL
FROM TB_USER USR
INNER JOIN TB_ROLE RL on RL.Role_Code = 'PROCESSOR'
WHERE USR.[User_Name] in ('VLy')
ORDER BY USR.[User_ID]
GO

-- 23. insert into TB_SYSTEM_USER
INSERT INTO [dbo].[TB_SYSTEM_USER]
    ([System_ID]
    ,[User_ID]
    ,[Sort_Order]
    ,[IsActive]
    ,[CreateDate]
    ,[CreatedBy]
    ,[UpdateDate]
    ,[UpdatedBy])
SELECT SYSTM.[System_ID], USR.[User_ID], 1, 1, getdate(), 'SYSTEM', NULL, NULL
FROM TB_SYSTEM SYSTM, TB_USER USR
ORDER BY SYSTM.[System_ID]
GO




-- select all ref tables
--exec [dbo].[GetReferenceTableData]

select * from [TB_DRAWDATE_CALENDAR]
select * from [TB_PAYDATE_CALENDAR]
select * from [TB_TRANSACTION_TYPE]
select * from [TB_SYSTEM_OF_RECORD]
select * from [TB_SOR_KVP_KEY]
select * from [TB_RESPONSE_STATUS_TYPE]
select * from [TB_PAYMENT_STATUS_TYPE]
select * from [TB_DB_SETTING]
select * from [TB_DB_SETTING_TYPE]
select * from [TB_PAYMENT_EXCHANGE_ENTITY]
select * from [TB_CLAIM_SCHEDULE_STATUS_TYPE]
select * from [TB_PAYMENT_STATUS_TYPE_EXTERNAL]
select * from [TB_PAYMENT_STATUS_TYPE_INTERNAL_TO_EXTERNAL]
select * from TB_ECS_STATUS_TYPE

select * from TB_ROLE
select * from TB_PERMISSION
select * from TB_ROLE_PERMISSION
select * from TB_USER_ROLE
select * from TB_USER_TYPE
select * from TB_USER
select * from TB_SYSTEM_USER
select * from TB_SYSTEM

select * from TB_EXCLUSIVE_PAYMENT_TYPE



-- clear all refference data tables
CREATE TABLE #tmp
(
	ID INT IDENTITY(1,1),
	Table_Name VARCHAR (100)
)

--CAPTURE TABLE NAMES
INSERT INTO #tmp 
VALUES 
	('TB_SYSTEM_USER'),
	('TB_USER_ROLE'),
	('TB_USER'),
	('TB_USER_TYPE'),
	('TB_SYSTEM'),
	('TB_ROLE_PERMISSION'),
	('TB_PERMISSION'),
	('TB_ROLE'),
	('TB_EXCLUSIVE_PAYMENT_TYPE'),
	('TB_ECS_STATUS_TYPE'),
	('TB_PAYMENT_STATUS_TYPE_INTERNAL_TO_EXTERNAL'),
	('TB_PAYMENT_STATUS_TYPE_EXTERNAL'),
	('TB_CLAIM_SCHEDULE_STATUS_TYPE'),
	('TB_PAYMENT_EXCHANGE_ENTITY'),
	('TB_DRAWDATE_CALENDAR'),
	('TB_PAYDATE_CALENDAR'),
	('TB_DB_SETTING'),
	('TB_DB_SETTING_TYPE'),
	('TB_PAYMENT_STATUS_TYPE'),
	('TB_RESPONSE_STATUS_TYPE'),
	('TB_SOR_KVP_KEY'),
	('TB_SYSTEM_OF_RECORD'),
	('TB_TRANSACTION_TYPE')


-- CURSOR, GO THROUGH EACH TABLE
-- 1. DELETE TABLE RECORDS
-- 2. DETERMINE IF TABLE NEEDS RESEED 
-- 3. DETERMINE HOW TO RESEED
-- 4. RESEED

DECLARE @table_Name VARCHAR(100)
DECLARE cur CURSOR 
FOR SELECT Table_Name FROM #tmp ORDER BY ID ASC

OPEN cur
FETCH NEXT FROM cur INTO @table_Name
WHILE @@FETCH_STATUS = 0
BEGIN		
	--DELETE RECORDS
	EXEC ('DELETE FROM ' + @table_Name)	
	
	--CHECK TABLE HAS IDENTITY
	DECLARE @has_identity BIT
	DECLARE @has_identity_cmd NVARCHAR(500) = 'SELECT @has_ident=OBJECTPROPERTY( OBJECT_ID(@tbl_Name), ''TableHasIdentity'')'	
	EXECUTE sp_executesql @has_identity_cmd, N'@tbl_Name VARCHAR(100),@has_ident BIT OUTPUT', @tbl_Name = @table_Name, @has_ident=@has_identity OUTPUT

	IF(@has_identity = 1)
	BEGIN
		--DTERMINE HOW TO RESEED THE TABLE by getting the Last _Value
		DECLARE @last_value INT 
		DECLARE @get_last_value_cmd NVARCHAR(500) = 'SELECT @lst_value=CONVERT(INT,last_value) FROM sys.identity_columns WHERE OBJECT_NAME(OBJECT_ID) = @tbl_Name'	
		EXECUTE sp_executesql @get_last_value_cmd, N'@tbl_Name VARCHAR(100),@lst_value INT OUTPUT', @tbl_Name = @table_Name, @lst_value=@last_value OUTPUT

		IF @last_value IS NULL
		BEGIN
			-- RESEED TO 0 - Table is NEW
			DBCC CHECKIDENT (@table_Name, RESEED, 1);
		END
		ELSE
		BEGIN
			-- RESEED TO 1 - Table is NOT NEW
			DBCC CHECKIDENT (@table_Name, RESEED, 0);
		END
	END
	FETCH NEXT FROM cur INTO @table_Name
END

CLOSE cur
DEALLOCATE cur
DROP TABLE #tmp




