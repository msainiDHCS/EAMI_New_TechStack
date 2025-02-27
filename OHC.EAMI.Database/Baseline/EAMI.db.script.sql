USE [EAMI]
GO
/****** Object:  User [DHSINTRA\EAMI Support]    Script Date: 5/29/2019 2:51:33 PM ******/
CREATE USER [DHSINTRA\EAMI Support] FOR LOGIN [DHSINTRA\EAMI Support]
GO
/****** Object:  User [DHSINTRA\EAMI System Admin]    Script Date: 5/29/2019 2:51:33 PM ******/
CREATE USER [DHSINTRA\EAMI System Admin] FOR LOGIN [DHSINTRA\EAMI System Admin]
GO
/****** Object:  User [DHSINTRA\OTyrellss01]    Script Date: 5/29/2019 2:51:33 PM ******/
CREATE USER [DHSINTRA\OTyrellss01] FOR LOGIN [DHSINTRA\OTyrellss01] WITH DEFAULT_SCHEMA=[dbo]
GO
ALTER ROLE [db_datareader] ADD MEMBER [DHSINTRA\EAMI Support]
GO
ALTER ROLE [db_owner] ADD MEMBER [DHSINTRA\EAMI System Admin]
GO
ALTER ROLE [db_owner] ADD MEMBER [DHSINTRA\OTyrellss01]
GO
/****** Object:  UserDefinedTableType [dbo].[EAMIDateTypeTableType]    Script Date: 5/29/2019 2:51:33 PM ******/
CREATE TYPE [dbo].[EAMIDateTypeTableType] AS TABLE(
	[EAMIDate] [datetime] NULL,
	[EAMIDateType] [varchar](10) NULL,
	[EAMIDateActionType] [varchar](10) NULL,
	[Comment] [varchar](500) NULL
)
GO
/****** Object:  UserDefinedTableType [dbo].[EAMILongArrayTableType]    Script Date: 5/29/2019 2:51:33 PM ******/
CREATE TYPE [dbo].[EAMILongArrayTableType] AS TABLE(
	[ID] [bigint] NULL
)
GO
/****** Object:  Table [dbo].[TB_CLAIM_SCHEDULE]    Script Date: 5/29/2019 2:51:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TB_CLAIM_SCHEDULE](
	[Claim_Schedule_ID] [int] IDENTITY(1,1) NOT NULL,
	[Claim_Schedule_Number] [varchar](20) NOT NULL,
	[Amount] [money] NULL,
	[Claim_Schedule_Date] [datetime] NULL,
	[FiscalYear] [varchar](10) NOT NULL,
	[Payment_Type] [varchar](50) NOT NULL,
	[ContractNumber] [varchar](20) NOT NULL,
	[Exclusive_Payment_Type_ID] [int] NOT NULL,
	[Paydate_Calendar_ID] [int] NOT NULL,
	[Payee_Entity_Info_ID] [int] NOT NULL,
	[IsLinked] [bit] NOT NULL,
	[LinkedByPGNumber] [varchar](15) NULL,
	[SeqNumber] [int] NULL,
 CONSTRAINT [PK_TB_CLAIM_SCHEDULE] PRIMARY KEY CLUSTERED 
(
	[Claim_Schedule_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [IX_TB_CLAIM_SCHEDULE] UNIQUE NONCLUSTERED 
(
	[Claim_Schedule_Number] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TB_CLAIM_SCHEDULE_DN_STATUS]    Script Date: 5/29/2019 2:51:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TB_CLAIM_SCHEDULE_DN_STATUS](
	[Claim_Schedule_ID] [int] NOT NULL,
	[CurrentCSStatusID] [int] NOT NULL,
	[LatestCSStatusID] [int] NOT NULL,
	[CurrentUserAssignmentID] [int] NULL,
 CONSTRAINT [PK_TB_CLAIM_SCHEDULE_DN_STATUS] PRIMARY KEY CLUSTERED 
(
	[Claim_Schedule_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TB_CLAIM_SCHEDULE_ECS]    Script Date: 5/29/2019 2:51:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TB_CLAIM_SCHEDULE_ECS](
	[Claim_Schedule_ECS_ID] [int] IDENTITY(1,1) NOT NULL,
	[Claim_Schedule_ID] [int] NOT NULL,
	[ECS_ID] [int] NOT NULL,
 CONSTRAINT [PK_TB_CLAIM_SCHEDULE_ECS] PRIMARY KEY CLUSTERED 
(
	[Claim_Schedule_ECS_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TB_CLAIM_SCHEDULE_REMITTANCE_ADVICE_NOTE]    Script Date: 5/29/2019 2:51:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TB_CLAIM_SCHEDULE_REMITTANCE_ADVICE_NOTE](
	[Claim_Schedule_ID] [int] NOT NULL,
	[Note] [varchar](500) NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](20) NOT NULL,
	[UpdateDate] [datetime] NULL,
	[UpdatedBy] [varchar](20) NULL,
 CONSTRAINT [PK_TB_CLAIM_SCHEDULE_REMITTANCE_ADVICE_NOTE] PRIMARY KEY CLUSTERED 
(
	[Claim_Schedule_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TB_CLAIM_SCHEDULE_STATUS]    Script Date: 5/29/2019 2:51:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TB_CLAIM_SCHEDULE_STATUS](
	[Claim_Schedule_Status_ID] [int] IDENTITY(1,1) NOT NULL,
	[Claim_Schedule_ID] [int] NOT NULL,
	[Claim_Schedule_Status_Type_ID] [int] NOT NULL,
	[Status_Date] [datetime] NOT NULL,
	[Status_Note] [varchar](200) NULL,
	[CreatedBy] [varchar](20) NOT NULL,
 CONSTRAINT [PK_TB_CLAIM_SCHEDULE_STATUS] PRIMARY KEY CLUSTERED 
(
	[Claim_Schedule_Status_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TB_CLAIM_SCHEDULE_STATUS_TYPE]    Script Date: 5/29/2019 2:51:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TB_CLAIM_SCHEDULE_STATUS_TYPE](
	[Claim_Schedule_Status_Type_ID] [int] IDENTITY(1,1) NOT NULL,
	[Code] [varchar](20) NOT NULL,
	[Description] [varchar](100) NULL,
	[IsActive] [bit] NOT NULL,
	[Sort_Value] [tinyint] NULL,
	[CreateDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](20) NOT NULL,
	[UpdateDate] [datetime] NULL,
	[UpdatedBy] [varchar](20) NULL,
 CONSTRAINT [PK_TB_CLAIM_SCHEDULE_STATUS_TYPE] PRIMARY KEY CLUSTERED 
(
	[Claim_Schedule_Status_Type_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [IX_TB_CLAIM_SCHEDULE_STATUS_TYPE] UNIQUE NONCLUSTERED 
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TB_CLAIM_SCHEDULE_USER_ASSIGNMENT]    Script Date: 5/29/2019 2:51:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TB_CLAIM_SCHEDULE_USER_ASSIGNMENT](
	[Claim_Schedule_User_Assignment_ID] [int] IDENTITY(1,1) NOT NULL,
	[Claim_Schedule_ID] [int] NOT NULL,
	[User_ID] [int] NOT NULL,
	[Assigned_CS_Status_ID] [int] NOT NULL,
 CONSTRAINT [PK_TB_CLAIM_SCHEDULE_USER_ASSIGNMENT] PRIMARY KEY CLUSTERED 
(
	[Claim_Schedule_User_Assignment_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TB_CLAIM_SCHEDULE_WARRANT]    Script Date: 5/29/2019 2:51:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TB_CLAIM_SCHEDULE_WARRANT](
	[Claim_Schedule_ID] [int] NOT NULL,
	[Warrant_ID] [int] NOT NULL,
 CONSTRAINT [PK_TB_CLAIM_SCHEDULE_WARRANT] PRIMARY KEY CLUSTERED 
(
	[Claim_Schedule_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TB_DB_SETTING]    Script Date: 5/29/2019 2:51:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TB_DB_SETTING](
	[DB_Setting_ID] [int] IDENTITY(1,1) NOT NULL,
	[DB_Setting_Key] [varchar](30) NOT NULL,
	[DB_Setting_Value] [varchar](100) NOT NULL,
	[DB_Setting_Type_ID] [int] NOT NULL,
	[Description] [varchar](50) NULL,
	[IsActive] [bit] NOT NULL,
	[Sort_Value] [tinyint] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](20) NOT NULL,
	[UpdateDate] [datetime] NULL,
	[UpdatedBy] [varchar](20) NULL,
 CONSTRAINT [PK_TB_DB_SETTING] PRIMARY KEY CLUSTERED 
(
	[DB_Setting_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [IX_TB_DB_SETTING] UNIQUE NONCLUSTERED 
(
	[DB_Setting_Key] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TB_DB_SETTING_TYPE]    Script Date: 5/29/2019 2:51:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TB_DB_SETTING_TYPE](
	[DB_Setting_Type_ID] [int] IDENTITY(1,1) NOT NULL,
	[Code] [varchar](30) NOT NULL,
	[Description] [varchar](50) NULL,
	[IsActive] [bit] NOT NULL,
	[Sort_Value] [tinyint] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](20) NOT NULL,
	[UpdateDate] [datetime] NULL,
	[UpdatedBy] [varchar](20) NULL,
 CONSTRAINT [PK_TB_DB_SETTING_TYPE] PRIMARY KEY CLUSTERED 
(
	[DB_Setting_Type_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [IX_TB_DB_SETTING_TYPE] UNIQUE NONCLUSTERED 
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TB_DRAWDATE_CALENDAR]    Script Date: 5/29/2019 2:51:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TB_DRAWDATE_CALENDAR](
	[Drawdate_Calendar_Id] [int] IDENTITY(1,1) NOT NULL,
	[Drawdate] [date] NOT NULL,
	[Note] [varchar](50) NULL,
	[IsActive] [bit] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](20) NOT NULL,
	[UpdateDate] [datetime] NULL,
	[UpdatedBy] [varchar](20) NULL,
 CONSTRAINT [PK_TB_DRAWDATE_CALENDAR] PRIMARY KEY CLUSTERED 
(
	[Drawdate_Calendar_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [IX_TB_DRAWDATE_CALENDAR] UNIQUE NONCLUSTERED 
(
	[Drawdate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TB_ECS]    Script Date: 5/29/2019 2:51:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TB_ECS](
	[ECS_ID] [int] IDENTITY(1,1) NOT NULL,
	[ECS_Number] [varchar](20) NOT NULL,
	[ECS_File_Name] [varchar](50) NULL,
	[Exclusive_Payment_Type_ID] [int] NOT NULL,
	[PayDate] [datetime] NOT NULL,
	[Amount] [money] NOT NULL,
	[SentToScoTaskNumber] [varchar](30) NULL,
	[WarrantReceivedTaskNumber] [varchar](30) NULL,
	[CreateDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](20) NOT NULL,
	[ApproveDate] [datetime] NULL,
	[ApprovedBy] [varchar](20) NULL,
	[SentToScoDate] [datetime] NULL,
	[WarrantReceivedDate] [datetime] NULL,
	[Current_ECS_Status_Type_ID] [int] NOT NULL,
	[CurrentStatusDate] [datetime] NOT NULL,
	[CurrentStatusNote] [varchar](200) NULL,
	[DexFileName] [varchar](200) NULL,
	[SCO_File_Line_Count] [int] NOT NULL,
 CONSTRAINT [PK_TB_ECS] PRIMARY KEY CLUSTERED 
(
	[ECS_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [IX_TB_ECS] UNIQUE NONCLUSTERED 
(
	[ECS_Number] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TB_ECS_SEQUENCE]    Script Date: 5/29/2019 2:51:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TB_ECS_SEQUENCE](
	[ECS_Sequence] [bigint] IDENTITY(10000000,1) NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
 CONSTRAINT [PK_TB_ECS_SEQUENCE] PRIMARY KEY CLUSTERED 
(
	[ECS_Sequence] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TB_ECS_STATUS_TYPE]    Script Date: 5/29/2019 2:51:35 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TB_ECS_STATUS_TYPE](
	[ECS_Status_Type_ID] [int] IDENTITY(1,1) NOT NULL,
	[CODE] [varchar](20) NOT NULL,
	[Description] [varchar](50) NULL,
	[IsActive] [bit] NOT NULL,
	[Sort_Value] [tinyint] NULL,
	[CreateDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](20) NOT NULL,
	[UpdateDate] [datetime] NULL,
	[UpdatedBy] [varchar](20) NULL,
 CONSTRAINT [PK_TB_ECS_STATUS_TYPE] PRIMARY KEY CLUSTERED 
(
	[ECS_Status_Type_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [IX_TB_ECS_STATUS_TYPE] UNIQUE NONCLUSTERED 
(
	[CODE] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TB_EXCLUSIVE_PAYMENT_TYPE]    Script Date: 5/29/2019 2:51:35 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TB_EXCLUSIVE_PAYMENT_TYPE](
	[Exclusive_Payment_Type_ID] [int] IDENTITY(1,1) NOT NULL,
	[Code] [varchar](20) NOT NULL,
	[Description] [varchar](100) NULL,
	[IsActive] [bit] NOT NULL,
	[Sort_Value] [tinyint] NULL,
	[CreateDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](20) NOT NULL,
	[UpdateDate] [datetime] NULL,
	[UpdatedBy] [varchar](20) NULL,
 CONSTRAINT [PK_TB_EXCLUSIVE_PAYMENT_TYPE] PRIMARY KEY CLUSTERED 
(
	[Exclusive_Payment_Type_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [IX_TB_EXCLUSIVE_PAYMENT_TYPE] UNIQUE NONCLUSTERED 
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TB_FUNDING_DETAIL]    Script Date: 5/29/2019 2:51:35 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TB_FUNDING_DETAIL](
	[Funding_Detail_ID] [int] IDENTITY(1,1) NOT NULL,
	[Payment_Record_ID] [int] NOT NULL,
	[Funding_Source_Name] [varchar](50) NOT NULL,
	[FFPAmount] [money] NOT NULL,
	[SGFAmount] [money] NOT NULL,
	[TotalAmount]  AS ([FFPAmount]+[SGFAmount]),
	[FiscalYear] [varchar](5) NOT NULL,
	[FiscalQuarter] [varchar](5) NOT NULL,
	[Title] [varchar](10) NOT NULL,
 CONSTRAINT [PK_TB_FUNDING_DETAIL] PRIMARY KEY CLUSTERED 
(
	[Funding_Detail_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TB_FUNDING_DETAIL_EXT_CAPMAN]    Script Date: 5/29/2019 2:51:35 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TB_FUNDING_DETAIL_EXT_CAPMAN](
	[Funding_Detail_ID] [int] NOT NULL,
 CONSTRAINT [PK_TB_FUNDING_DETAIL_EXT_CAPMAN] PRIMARY KEY CLUSTERED 
(
	[Funding_Detail_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TB_FUNDING_DETAIL_KVP]    Script Date: 5/29/2019 2:51:35 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TB_FUNDING_DETAIL_KVP](
	[Funding_Detail_Kvp_ID] [int] IDENTITY(1,1) NOT NULL,
	[Funding_Detail_ID] [int] NOT NULL,
	[SOR_Kvp_Key_ID] [int] NOT NULL,
	[Kvp_Value] [varchar](100) NOT NULL,
 CONSTRAINT [PK_TB_FUNDING_DETAIL_KVP] PRIMARY KEY CLUSTERED 
(
	[Funding_Detail_Kvp_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TB_PAYDATE_CALENDAR]    Script Date: 5/29/2019 2:51:35 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TB_PAYDATE_CALENDAR](
	[Paydate_Calendar_ID] [int] IDENTITY(1,1) NOT NULL,
	[Paydate] [date] NOT NULL,
	[Note] [varchar](50) NULL,
	[IsActive] [bit] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](20) NOT NULL,
	[UpdateDate] [datetime] NULL,
	[UpdatedBy] [varchar](20) NULL,
 CONSTRAINT [PK_TB_PAYDATE_CALENDAR] PRIMARY KEY CLUSTERED 
(
	[Paydate_Calendar_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [IX_TB_PAYDATE_CALENDAR] UNIQUE NONCLUSTERED 
(
	[Paydate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TB_PAYMENT_CLAIM_SCHEDULE]    Script Date: 5/29/2019 2:51:35 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TB_PAYMENT_CLAIM_SCHEDULE](
	[Payment_Record_ID] [int] NOT NULL,
	[Claim_Schedule_ID] [int] NOT NULL,
 CONSTRAINT [PK_TB_PAYMENT_CLAIM_SCHEDULE] PRIMARY KEY CLUSTERED 
(
	[Payment_Record_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TB_PAYMENT_DN_STATUS]    Script Date: 5/29/2019 2:51:35 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TB_PAYMENT_DN_STATUS](
	[Payment_Record_ID] [int] NOT NULL,
	[CurrentPaymentStatusID] [int] NOT NULL,
	[LatestPaymentStatusID] [int] NOT NULL,
	[CurrentUserAssignmentID] [int] NULL,
	[CurrentHoldStatusID] [int] NULL,
	[CurrentUnHoldStatusID] [int] NULL,
	[CurrentReleaseFromSupStatusID] [int] NULL,
 CONSTRAINT [PK_TB_PAYMENT_DN_STATUS] PRIMARY KEY CLUSTERED 
(
	[Payment_Record_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TB_PAYMENT_EXCHANGE_ENTITY]    Script Date: 5/29/2019 2:51:35 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TB_PAYMENT_EXCHANGE_ENTITY](
	[Payment_Exchange_Entity_ID] [int] IDENTITY(1,1) NOT NULL,
	[Entity_ID] [varchar](10) NOT NULL,
	[Entity_ID_Type] [varchar](30) NOT NULL,
	[Entity_Name] [varchar](100) NOT NULL,
	[Entity_EIN] [varchar](9) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[Sort_Value] [tinyint] NULL,
	[CreateDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](20) NOT NULL,
	[UpdateDate] [datetime] NULL,
	[UpdatedBy] [varchar](20) NULL,
 CONSTRAINT [PK_TB_PAYMENT_EXCHANGE_ENTITY] PRIMARY KEY CLUSTERED 
(
	[Payment_Exchange_Entity_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [IX_TB_PAYMENT_EXCHANGE_ENTITY] UNIQUE NONCLUSTERED 
(
	[Entity_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TB_PAYMENT_EXCHANGE_ENTITY_INFO]    Script Date: 5/29/2019 2:51:35 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TB_PAYMENT_EXCHANGE_ENTITY_INFO](
	[Payment_Exchange_Entity_Info_ID] [int] IDENTITY(1,1) NOT NULL,
	[Payment_Exchange_Entity_ID] [int] NOT NULL,
	[Entity_Name] [varchar](100) NOT NULL,
	[Entity_Code_Suffix] [varchar](2) NOT NULL,
	[Entity_Address_Line1] [varchar](80) NOT NULL,
	[Entity_Address_Line2] [varchar](80) NULL,
	[Entity_Address_Line3] [varchar](80) NULL,
	[Entity_City] [varchar](40) NOT NULL,
	[Entity_State] [varchar](2) NOT NULL,
	[Entity_Zip] [varchar](10) NOT NULL,
	[Entity_VendorTypeCode] [varchar](1) NOT NULL,
	[Entity_ContractNumber] [varchar](20) NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[UpdateDate] [datetime] NULL,
 CONSTRAINT [PK_TB_PAYMENT_EXCHANGE_ENTITY_INFO] PRIMARY KEY CLUSTERED 
(
	[Payment_Exchange_Entity_Info_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TB_PAYMENT_KVP]    Script Date: 5/29/2019 2:51:35 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TB_PAYMENT_KVP](
	[Payment_Kvp_ID] [int] IDENTITY(1,1) NOT NULL,
	[Payment_Record_ID] [int] NOT NULL,
	[SOR_Kvp_Key_ID] [int] NOT NULL,
	[Kvp_Value] [varchar](100) NOT NULL,
 CONSTRAINT [PK_TB_PAYMENT_KVP] PRIMARY KEY CLUSTERED 
(
	[Payment_Kvp_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TB_PAYMENT_PAYDATE_CALENDAR]    Script Date: 5/29/2019 2:51:35 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TB_PAYMENT_PAYDATE_CALENDAR](
	[Payment_Record_ID] [int] NOT NULL,
	[Paydate_Calendar_ID] [int] NOT NULL,
 CONSTRAINT [PK_TB_PAYMENT_PAYDATE_CALENDAR] PRIMARY KEY CLUSTERED 
(
	[Payment_Record_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TB_PAYMENT_RECORD]    Script Date: 5/29/2019 2:51:35 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TB_PAYMENT_RECORD](
	[Payment_Record_ID] [int] IDENTITY(1,1) NOT NULL,
	[Transaction_ID] [int] NOT NULL,
	[PaymentRec_Number] [varchar](30) NOT NULL,
	[PaymentRec_NumberExt] [varchar](30) NOT NULL,
	[Payment_Type] [varchar](50) NOT NULL,
	[Payment_Date] [datetime] NOT NULL,
	[Amount] [money] NOT NULL,
	[FiscalYear] [varchar](10) NOT NULL,
	[IndexCode] [varchar](10) NOT NULL,
	[ObjectDetailCode] [varchar](10) NOT NULL,
	[ObjectAgencyCode] [varchar](10) NULL,
	[PCACode] [varchar](10) NOT NULL,
	[ApprovedBy] [varchar](30) NOT NULL,
	[PaymentSet_Number] [varchar](30) NOT NULL,
	[PaymentSet_NumberExt] [varchar](30) NOT NULL,
	[Payee_Entity_Info_ID] [int] NOT NULL,
	[RPICode] [varchar](2) NOT NULL,
	[IsReportableRPI] [bit] NOT NULL,
 CONSTRAINT [PK_TB_PAYMENT_RECORD] PRIMARY KEY CLUSTERED 
(
	[Payment_Record_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [IX_TB_PAYMENT_RECORD] UNIQUE NONCLUSTERED 
(
	[PaymentRec_Number] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TB_PAYMENT_RECORD_EXT_CAPMAN]    Script Date: 5/29/2019 2:51:36 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TB_PAYMENT_RECORD_EXT_CAPMAN](
	[Payment_Record_ID] [int] NOT NULL,
	[ContractNumber] [varchar](20) NOT NULL,
	[ContractDateFrom] [datetime] NOT NULL,
	[ContractDateTo] [datetime] NOT NULL,
	[Exclusive_Payment_Type_ID] [int] NOT NULL,
 CONSTRAINT [PK_TB_PAYMENT_RECORD_EXT_CAPMAN] PRIMARY KEY CLUSTERED 
(
	[Payment_Record_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TB_PAYMENT_STATUS]    Script Date: 5/29/2019 2:51:36 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TB_PAYMENT_STATUS](
	[Payment_Status_ID] [int] IDENTITY(1,1) NOT NULL,
	[Payment_Record_ID] [int] NOT NULL,
	[Payment_Status_Type_ID] [int] NOT NULL,
	[Status_Date] [datetime] NOT NULL,
	[Status_Note] [varchar](200) NULL,
	[CreatedBy] [varchar](20) NOT NULL,
 CONSTRAINT [PK_TB_PAYMENT_STATUS] PRIMARY KEY CLUSTERED 
(
	[Payment_Status_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TB_PAYMENT_STATUS_TYPE]    Script Date: 5/29/2019 2:51:36 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TB_PAYMENT_STATUS_TYPE](
	[Payment_Status_Type_ID] [int] IDENTITY(1,1) NOT NULL,
	[Code] [varchar](20) NOT NULL,
	[Description] [varchar](100) NULL,
	[IsActive] [bit] NOT NULL,
	[Sort_Value] [tinyint] NULL,
	[CreateDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](20) NOT NULL,
	[UpdateDate] [datetime] NULL,
	[UpdatedBy] [varchar](20) NULL,
 CONSTRAINT [PK_TB_PAYMENT_STATUS_TYPE] PRIMARY KEY CLUSTERED 
(
	[Payment_Status_Type_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [IX_TB_PAYMENT_STATUS_TYPE] UNIQUE NONCLUSTERED 
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TB_PAYMENT_STATUS_TYPE_EXTERNAL]    Script Date: 5/29/2019 2:51:36 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TB_PAYMENT_STATUS_TYPE_EXTERNAL](
	[Payment_Status_Type_Ext_ID] [int] IDENTITY(1,1) NOT NULL,
	[Code] [varchar](20) NOT NULL,
	[Description] [varchar](50) NULL,
	[IsActive] [bit] NOT NULL,
	[Sort_Value] [tinyint] NULL,
	[CreateDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](20) NOT NULL,
	[UpdateDate] [datetime] NULL,
	[UpdatedBy] [varchar](20) NULL,
 CONSTRAINT [PK_TB_PAYMENT_STATUS_TYPE_EXTERNAL] PRIMARY KEY CLUSTERED 
(
	[Payment_Status_Type_Ext_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [IX_TB_PAYMENT_STATUS_TYPE_EXTERNAL] UNIQUE NONCLUSTERED 
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TB_PAYMENT_STATUS_TYPE_INTERNAL_TO_EXTERNAL]    Script Date: 5/29/2019 2:51:36 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TB_PAYMENT_STATUS_TYPE_INTERNAL_TO_EXTERNAL](
	[Payment_Status_Type_ID] [int] NOT NULL,
	[Payment_Status_Type_Ext_ID] [int] NOT NULL,
 CONSTRAINT [PK_TB_PAYMENT_STATUS_TYPE_INTERNAL_TO_EXTERNAL] PRIMARY KEY CLUSTERED 
(
	[Payment_Status_Type_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TB_PAYMENT_USER_ASSIGNMENT]    Script Date: 5/29/2019 2:51:36 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TB_PAYMENT_USER_ASSIGNMENT](
	[Payment_User_Assignment_ID] [int] IDENTITY(1,1) NOT NULL,
	[Payment_Record_ID] [int] NOT NULL,
	[User_ID] [int] NOT NULL,
	[Assigned_Payment_Status_ID] [int] NOT NULL,
 CONSTRAINT [PK_TB_PAYMENT_USER_ASSIGNMENT] PRIMARY KEY CLUSTERED 
(
	[Payment_User_Assignment_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TB_PERMISSION]    Script Date: 5/29/2019 2:51:36 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TB_PERMISSION](
	[Permission_ID] [int] IDENTITY(1,1) NOT NULL,
	[Permission_Code] [varchar](50) NOT NULL,
	[Permission_Name] [varchar](200) NOT NULL,
	[Sort_Order] [smallint] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](20) NOT NULL,
	[UpdateDate] [datetime] NULL,
	[UpdatedBy] [varchar](20) NULL,
 CONSTRAINT [PK__TB_PERMI__89B744E559F39196] PRIMARY KEY CLUSTERED 
(
	[Permission_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UC_Permission] UNIQUE NONCLUSTERED 
(
	[Permission_Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TB_REQUEST]    Script Date: 5/29/2019 2:51:36 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TB_REQUEST](
	[Request_ID] [int] IDENTITY(1,1) NOT NULL,
	[Request_TimeStamp] [datetime] NOT NULL,
	[Msg_TimeStamp] [datetime] NULL,
	[Msg_Sender_ID] [varchar](20) NULL,
	[Msg_Transaction_ID] [varchar](50) NULL,
	[Msg_Transaction_Type] [varchar](50) NULL,
	[Msg_Transaction_Version] [varchar](5) NULL,
 CONSTRAINT [PK_TB_REQUEST] PRIMARY KEY CLUSTERED 
(
	[Request_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TB_RESPONSE]    Script Date: 5/29/2019 2:51:36 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TB_RESPONSE](
	[Response_ID] [int] NOT NULL,
	[Msg_Transaction_ID] [varchar](50) NOT NULL,
	[Msg_Transaction_Type] [varchar](50) NOT NULL,
	[Response_TimeStamp] [datetime] NOT NULL,
	[Response_Status_Type_ID] [int] NOT NULL,
	[Response_Message] [varchar](200) NULL,
 CONSTRAINT [PK_TB_RESPONSE] PRIMARY KEY CLUSTERED 
(
	[Response_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TB_RESPONSE_STATUS_TYPE]    Script Date: 5/29/2019 2:51:36 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TB_RESPONSE_STATUS_TYPE](
	[Response_Status_Type_ID] [int] IDENTITY(1,1) NOT NULL,
	[Code] [varchar](20) NOT NULL,
	[Description] [varchar](50) NULL,
	[IsActive] [bit] NOT NULL,
	[Sort_Value] [tinyint] NULL,
	[CreateDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](20) NOT NULL,
	[UpdateDate] [datetime] NULL,
	[UpdatedBy] [varchar](20) NULL,
 CONSTRAINT [PK_TB_TRANSACTION_STATUS_TYPE] PRIMARY KEY CLUSTERED 
(
	[Response_Status_Type_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [IX_TB_RESPONSE_STATUS_TYPE] UNIQUE NONCLUSTERED 
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TB_ROLE]    Script Date: 5/29/2019 2:51:36 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TB_ROLE](
	[Role_ID] [int] IDENTITY(1,1) NOT NULL,
	[Role_Code] [varchar](50) NOT NULL,
	[Role_Name] [varchar](200) NOT NULL,
	[Sort_Order] [smallint] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](20) NOT NULL,
	[UpdateDate] [datetime] NULL,
	[UpdatedBy] [varchar](20) NULL,
 CONSTRAINT [PK__TB_ROLE__D80AB49BCA471749] PRIMARY KEY CLUSTERED 
(
	[Role_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UC_Role] UNIQUE NONCLUSTERED 
(
	[Role_Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TB_ROLE_PERMISSION]    Script Date: 5/29/2019 2:51:36 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TB_ROLE_PERMISSION](
	[Role_Permission_ID] [int] IDENTITY(1,1) NOT NULL,
	[Role_ID] [int] NOT NULL,
	[Permission_ID] [int] NOT NULL,
	[Sort_Order] [smallint] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](20) NOT NULL,
	[UpdateDate] [datetime] NULL,
	[UpdatedBy] [varchar](20) NULL,
 CONSTRAINT [PK__TB_ROLE___71179B8C68D7DB06] PRIMARY KEY CLUSTERED 
(
	[Role_Permission_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UC_Role_Permission] UNIQUE NONCLUSTERED 
(
	[Role_ID] ASC,
	[Permission_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TB_SOR_KVP_KEY]    Script Date: 5/29/2019 2:51:36 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TB_SOR_KVP_KEY](
	[SOR_Kvp_Key_ID] [int] IDENTITY(1,1) NOT NULL,
	[SOR_ID] [int] NOT NULL,
	[Kvp_Key_Name] [varchar](20) NOT NULL,
	[Kvp_Value_DataType] [varchar](10) NOT NULL,
	[Kvp_Value_Length] [int] NOT NULL,
	[Kvp_Description] [varchar](100) NULL,
	[OwnerEntity] [varchar](15) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[Sort_Value] [tinyint] NULL,
	[CreateDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](20) NOT NULL,
	[UpdateDate] [datetime] NULL,
	[UpdatedBy] [varchar](20) NULL,
 CONSTRAINT [PK_TB_SOR_KVP_KEY] PRIMARY KEY CLUSTERED 
(
	[SOR_Kvp_Key_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [IX_TB_SOR_KVP_KEY] UNIQUE NONCLUSTERED 
(
	[Kvp_Key_Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TB_SYSTEM]    Script Date: 5/29/2019 2:51:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TB_SYSTEM](
	[System_ID] [int] IDENTITY(1,1) NOT NULL,
	[System_Code] [varchar](50) NOT NULL,
	[System_Name] [varchar](50) NOT NULL,
	[Sort_Order] [smallint] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](20) NOT NULL,
	[UpdateDate] [datetime] NULL,
	[UpdatedBy] [varchar](20) NULL,
 CONSTRAINT [PK__TB_SYSTE__C7178F128ACD8DD1] PRIMARY KEY CLUSTERED 
(
	[System_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UC_System] UNIQUE NONCLUSTERED 
(
	[System_Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TB_SYSTEM_OF_RECORD]    Script Date: 5/29/2019 2:51:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TB_SYSTEM_OF_RECORD](
	[SOR_ID] [int] IDENTITY(1,1) NOT NULL,
	[Code] [varchar](20) NOT NULL,
	[Description] [varchar](50) NULL,
	[IsActive] [bit] NOT NULL,
	[Sort_Value] [tinyint] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[CreatedBy] [nchar](20) NOT NULL,
	[UpdateDate] [datetime] NULL,
	[UpdatedBy] [varchar](20) NULL,
 CONSTRAINT [PK_TB_SYSTEM_OF_RECORD] PRIMARY KEY CLUSTERED 
(
	[SOR_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [IX_TB_SYSTEM_OF_RECORD] UNIQUE NONCLUSTERED 
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TB_SYSTEM_USER]    Script Date: 5/29/2019 2:51:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TB_SYSTEM_USER](
	[System_User_ID] [int] IDENTITY(1,1) NOT NULL,
	[System_ID] [int] NOT NULL,
	[User_ID] [int] NOT NULL,
	[Sort_Order] [smallint] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](20) NOT NULL,
	[UpdateDate] [datetime] NULL,
	[UpdatedBy] [varchar](20) NULL,
 CONSTRAINT [PK__TB_SYSTE__4F8A61AAB04A68A1] PRIMARY KEY CLUSTERED 
(
	[System_User_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UC_System_User] UNIQUE NONCLUSTERED 
(
	[System_ID] ASC,
	[User_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TB_TRACE_PAYMENT]    Script Date: 5/29/2019 2:51:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TB_TRACE_PAYMENT](
	[Trace_Payment_ID] [int] IDENTITY(1,1) NOT NULL,
	[Trace_Transaction_ID] [int] NOT NULL,
	[Payment_Status_Type_ID] [int] NULL,
	[Payment_Status_Date] [datetime] NULL,
	[Payment_Status_Message] [varchar](300) NULL,
	[ClaimScheduleNumber] [varchar](20) NULL,
	[ClaimScheduleDate] [datetime] NULL,
	[WarrantNumber] [varchar](20) NULL,
	[WarrantDate] [datetime] NULL,
	[WarrantAmount] [money] NULL,
	[PaymentRec_Number] [varchar](30) NULL,
	[PaymentRec_NumberExt] [varchar](30) NULL,
	[Payment_Type] [varchar](50) NULL,
	[Payment_Date] [datetime] NULL,
	[Amount] [money] NULL,
	[FiscalYear] [varchar](10) NULL,
	[IndexCode] [varchar](10) NULL,
	[ObjectDetailCode] [varchar](10) NULL,
	[ObjectAgencyCode] [varchar](10) NULL,
	[PCACode] [varchar](10) NULL,
	[ApprovedBy] [varchar](30) NULL,
	[PaymentSet_Number] [varchar](30) NULL,
	[PaymentSet_NumberExt] [varchar](30) NULL,
	[Payee_Entity_ID] [varchar](10) NULL,
	[Payee_Entity_ID_Type] [varchar](30) NULL,
	[Payee_Entity_Name] [varchar](100) NULL,
	[Payee_Entity_ID_Suffix] [varchar](2) NULL,
	[Payee_Address_Line1] [varchar](80) NULL,
	[Payee_Address_Line2] [varchar](80) NULL,
	[Payee_Address_Line3] [varchar](80) NULL,
	[Payee_City] [varchar](40) NULL,
	[Payee_State] [varchar](2) NULL,
	[Payee_Zip] [varchar](10) NULL,
	[Payee_EIN] [varchar](10) NULL,
	[Payee_VendorTypeCode] [varchar](1) NULL,
	[Payment_Kvp_Xml] [xml] NULL,
	[Payment_Funding_Details_Xml] [xml] NULL,
 CONSTRAINT [PK_TB_REJECTED_INVOICE] PRIMARY KEY CLUSTERED 
(
	[Trace_Payment_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TB_TRACE_TRANSACTION]    Script Date: 5/29/2019 2:51:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TB_TRACE_TRANSACTION](
	[Trace_Transaction_ID] [int] NOT NULL,
	[Payer_Entity_ID] [varchar](10) NULL,
	[Payer_Entity_ID_Type] [varchar](30) NULL,
	[Payer_Entity_Name] [varchar](100) NULL,
	[Payer_Entity_ID_Suffix] [varchar](2) NULL,
	[Payer_Address_Line1] [varchar](80) NULL,
	[Payer_Address_Line2] [varchar](80) NULL,
	[Payer_Address_Line3] [varchar](80) NULL,
	[Payer_City] [varchar](40) NULL,
	[Payer_State] [varchar](2) NULL,
	[Payer_Zip] [varchar](10) NULL,
	[TotalPymtAmount] [varchar](20) NULL,
	[TotalPymtRecCount] [varchar](10) NULL,
	[RejectedPaymentDateFrom] [datetime] NULL,
	[RejectedPaymentDateTo] [datetime] NULL,
 CONSTRAINT [PK_TB_TRACE_TRANSACTION] PRIMARY KEY CLUSTERED 
(
	[Trace_Transaction_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TB_TRANSACTION]    Script Date: 5/29/2019 2:51:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TB_TRANSACTION](
	[Transaction_ID] [int] NOT NULL,
	[Msg_Transaction_ID] [varchar](50) NOT NULL,
	[SOR_ID] [int] NOT NULL,
	[Transaction_Type_ID] [int] NOT NULL,
	[TransactionVersion] [varchar](5) NOT NULL,
	[UsersNotified] [bit] NOT NULL,
 CONSTRAINT [PK_TB_TRANSACTION] PRIMARY KEY CLUSTERED 
(
	[Transaction_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [IX_TB_TRANSACTION] UNIQUE NONCLUSTERED 
(
	[Msg_Transaction_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TB_TRANSACTION_PAYER_ENTITY]    Script Date: 5/29/2019 2:51:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TB_TRANSACTION_PAYER_ENTITY](
	[Transaction_Payer_Entity_ID] [int] IDENTITY(1,1) NOT NULL,
	[Transactioin_ID] [int] NOT NULL,
	[Payment_Exchange_Entity_ID] [int] NOT NULL,
 CONSTRAINT [PK_TB_TRANSACTION_PAYER_ENTITY] PRIMARY KEY CLUSTERED 
(
	[Transaction_Payer_Entity_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TB_TRANSACTION_TYPE]    Script Date: 5/29/2019 2:51:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TB_TRANSACTION_TYPE](
	[Transaction_Type_ID] [int] IDENTITY(1,1) NOT NULL,
	[Code] [varchar](30) NOT NULL,
	[Description] [varchar](50) NULL,
	[IsActive] [bit] NOT NULL,
	[Sort_Value] [tinyint] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](20) NOT NULL,
	[UpdateDate] [datetime] NULL,
	[UpdatedBy] [varchar](20) NULL,
 CONSTRAINT [PK_TB_TRANSACTION_TYPE] PRIMARY KEY CLUSTERED 
(
	[Transaction_Type_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [IX_TB_TRANSACTION_TYPE] UNIQUE NONCLUSTERED 
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TB_USER]    Script Date: 5/29/2019 2:51:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TB_USER](
	[User_ID] [int] IDENTITY(1,1) NOT NULL,
	[User_Name] [varchar](20) NOT NULL,
	[Display_Name] [varchar](50) NULL,
	[User_EmailAddr] [varchar](50) NULL,
	[User_Password] [varchar](200) NULL,
	[User_Type_ID] [int] NOT NULL,
	[Domain_Name] [varchar](100) NULL,
	[Sort_Order] [smallint] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](20) NOT NULL,
	[UpdateDate] [datetime] NULL,
	[UpdatedBy] [varchar](20) NULL,
 CONSTRAINT [PK__TB_USER__206D91900933C820] PRIMARY KEY CLUSTERED 
(
	[User_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UC_User] UNIQUE NONCLUSTERED 
(
	[User_Type_ID] ASC,
	[User_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TB_USER_ROLE]    Script Date: 5/29/2019 2:51:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TB_USER_ROLE](
	[User_Role_ID] [int] IDENTITY(1,1) NOT NULL,
	[User_ID] [int] NOT NULL,
	[Role_ID] [int] NOT NULL,
	[Sort_Order] [smallint] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](20) NOT NULL,
	[UpdateDate] [datetime] NULL,
	[UpdatedBy] [varchar](20) NULL,
 CONSTRAINT [PK__TB_USER___134E48ECB83FCDD5] PRIMARY KEY CLUSTERED 
(
	[User_Role_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UC_User_Role] UNIQUE NONCLUSTERED 
(
	[User_ID] ASC,
	[Role_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TB_USER_TYPE]    Script Date: 5/29/2019 2:51:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TB_USER_TYPE](
	[User_Type_ID] [int] IDENTITY(1,1) NOT NULL,
	[User_Type_Code] [varchar](50) NOT NULL,
	[User_Type_Name] [varchar](50) NOT NULL,
	[Sort_Order] [smallint] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](20) NOT NULL,
	[UpdateDate] [datetime] NULL,
	[UpdatedBy] [varchar](20) NULL,
 CONSTRAINT [PK__TB_USER___D3A592DC465EA32C] PRIMARY KEY CLUSTERED 
(
	[User_Type_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UC_User_Type] UNIQUE NONCLUSTERED 
(
	[User_Type_Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TB_WARRANT]    Script Date: 5/29/2019 2:51:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TB_WARRANT](
	[Warrant_ID] [int] IDENTITY(1,1) NOT NULL,
	[Warrant_Number] [varchar](20) NOT NULL,
	[Amount] [money] NOT NULL,
	[Warrant_Date] [datetime] NOT NULL,
	[Date_Info_Received] [datetime] NULL,
 CONSTRAINT [PK_TB_WARRANT] PRIMARY KEY CLUSTERED 
(
	[Warrant_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [IX_TB_WARRANT] UNIQUE NONCLUSTERED 
(
	[Warrant_Number] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[VendorTypeVendor]    Script Date: 5/29/2019 2:51:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[VendorTypeVendor](
	[F1] [nvarchar](255) NULL,
	[F2] [nvarchar](255) NULL,
	[VendorCode] [nvarchar](255) NULL,
	[VendorCodeSuffix] [nvarchar](255) NULL,
	[VendorName] [nvarchar](255) NULL,
	[VendorTypeCode] [nvarchar](255) NULL,
	[VendorTypeDescription] [nvarchar](255) NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[TB_CLAIM_SCHEDULE] ADD  CONSTRAINT [DF_TB_CLAIM_SCHEDULE_Claim_Schedule_Date]  DEFAULT (getdate()) FOR [Claim_Schedule_Date]
GO
ALTER TABLE [dbo].[TB_CLAIM_SCHEDULE] ADD  CONSTRAINT [DF_TB_CLAIM_SCHEDULE_IsLinked]  DEFAULT ((0)) FOR [IsLinked]
GO
ALTER TABLE [dbo].[TB_CLAIM_SCHEDULE_STATUS] ADD  CONSTRAINT [DF_TB_CLAIM_SCHEDULE_STATUS_Status_Date]  DEFAULT (getdate()) FOR [Status_Date]
GO
ALTER TABLE [dbo].[TB_CLAIM_SCHEDULE_STATUS_TYPE] ADD  CONSTRAINT [DF_TB_CLAIM_SCHEDULE_STATUS_TYPE_CreateDate]  DEFAULT (getdate()) FOR [CreateDate]
GO
ALTER TABLE [dbo].[TB_CLAIM_SCHEDULE_STATUS_TYPE] ADD  CONSTRAINT [DF_TB_CLAIM_SCHEDULE_STATUS_TYPE_UpdateDate]  DEFAULT (getdate()) FOR [UpdateDate]
GO
ALTER TABLE [dbo].[TB_DB_SETTING_TYPE] ADD  CONSTRAINT [DF_TB_DB_SETTING_TYPE_CreateDate]  DEFAULT (getdate()) FOR [CreateDate]
GO
ALTER TABLE [dbo].[TB_DB_SETTING_TYPE] ADD  CONSTRAINT [DF_TB_DB_SETTING_TYPE_UpdateDate]  DEFAULT (getdate()) FOR [UpdateDate]
GO
ALTER TABLE [dbo].[TB_DRAWDATE_CALENDAR] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[TB_DRAWDATE_CALENDAR] ADD  DEFAULT (getdate()) FOR [CreateDate]
GO
ALTER TABLE [dbo].[TB_DRAWDATE_CALENDAR] ADD  DEFAULT ('system') FOR [CreatedBy]
GO
ALTER TABLE [dbo].[TB_ECS] ADD  CONSTRAINT [DF_TB_ECS_CreateDate]  DEFAULT (getdate()) FOR [CreateDate]
GO
ALTER TABLE [dbo].[TB_ECS] ADD  CONSTRAINT [DF_TB_ECS_SCO_File_Line_Count]  DEFAULT ((0)) FOR [SCO_File_Line_Count]
GO
ALTER TABLE [dbo].[TB_ECS_SEQUENCE] ADD  CONSTRAINT [DF_TB_ECS_SEQUENCE_UpdateDate]  DEFAULT (getdate()) FOR [UpdateDate]
GO
ALTER TABLE [dbo].[TB_EXCLUSIVE_PAYMENT_TYPE] ADD  CONSTRAINT [DF_TB_EXCLUSIVE_PAYMENT_TYPE_CreateDate]  DEFAULT (getdate()) FOR [CreateDate]
GO
ALTER TABLE [dbo].[TB_EXCLUSIVE_PAYMENT_TYPE] ADD  CONSTRAINT [DF_TB_EXCLUSIVE_PAYMENT_TYPE_UpdateDate]  DEFAULT (getdate()) FOR [UpdateDate]
GO
ALTER TABLE [dbo].[TB_PAYMENT_STATUS] ADD  CONSTRAINT [DF_TB_PAYMENT_STATUS_Status_Date]  DEFAULT (getdate()) FOR [Status_Date]
GO
ALTER TABLE [dbo].[TB_PAYMENT_STATUS_TYPE] ADD  CONSTRAINT [DF_TB_PAYMENT_STATUS_TYPE_CreateDate]  DEFAULT (getdate()) FOR [CreateDate]
GO
ALTER TABLE [dbo].[TB_PAYMENT_STATUS_TYPE] ADD  CONSTRAINT [DF_TB_PAYMENT_STATUS_TYPE_UpdateDate]  DEFAULT (getdate()) FOR [UpdateDate]
GO
ALTER TABLE [dbo].[TB_PAYMENT_STATUS_TYPE_EXTERNAL] ADD  CONSTRAINT [DF_TB_PAYMENT_STATUS_TYPE_EXTERNAL_CreateDate]  DEFAULT (getdate()) FOR [CreateDate]
GO
ALTER TABLE [dbo].[TB_PAYMENT_STATUS_TYPE_EXTERNAL] ADD  CONSTRAINT [DF_TB_PAYMENT_STATUS_TYPE_EXTERNAL_UpdateDate]  DEFAULT (getdate()) FOR [UpdateDate]
GO
ALTER TABLE [dbo].[TB_REQUEST] ADD  CONSTRAINT [DF_TB_REQUEST_Request_TimeStamp]  DEFAULT (getdate()) FOR [Request_TimeStamp]
GO
ALTER TABLE [dbo].[TB_RESPONSE] ADD  CONSTRAINT [DF_TB_RESPONSE_Response_TimeStamp]  DEFAULT (getdate()) FOR [Response_TimeStamp]
GO
ALTER TABLE [dbo].[TB_RESPONSE_STATUS_TYPE] ADD  CONSTRAINT [DF_TB_RESPONSE_STATUS_TYPE_CreateDate]  DEFAULT (getdate()) FOR [CreateDate]
GO
ALTER TABLE [dbo].[TB_RESPONSE_STATUS_TYPE] ADD  CONSTRAINT [DF_TB_RESPONSE_STATUS_TYPE_UpdateDate]  DEFAULT (getdate()) FOR [UpdateDate]
GO
ALTER TABLE [dbo].[TB_SYSTEM_OF_RECORD] ADD  CONSTRAINT [DF_TB_SYSTEM_OF_RECORD_CreateDate]  DEFAULT (getdate()) FOR [CreateDate]
GO
ALTER TABLE [dbo].[TB_SYSTEM_OF_RECORD] ADD  CONSTRAINT [DF_TB_SYSTEM_OF_RECORD_UpdatDate]  DEFAULT (getdate()) FOR [UpdateDate]
GO
ALTER TABLE [dbo].[TB_TRANSACTION_TYPE] ADD  CONSTRAINT [DF_TB_TRANSACTION_TYPE_CreateDate]  DEFAULT (getdate()) FOR [CreateDate]
GO
ALTER TABLE [dbo].[TB_TRANSACTION_TYPE] ADD  CONSTRAINT [DF_TB_TRANSACTION_TYPE_UpdateDate]  DEFAULT (getdate()) FOR [UpdateDate]
GO
ALTER TABLE [dbo].[TB_CLAIM_SCHEDULE]  WITH CHECK ADD  CONSTRAINT [FK_TB_CLAIM_SCHEDULE_TB_EXCLUSIVE_PAYMENT_TYPE] FOREIGN KEY([Exclusive_Payment_Type_ID])
REFERENCES [dbo].[TB_EXCLUSIVE_PAYMENT_TYPE] ([Exclusive_Payment_Type_ID])
GO
ALTER TABLE [dbo].[TB_CLAIM_SCHEDULE] CHECK CONSTRAINT [FK_TB_CLAIM_SCHEDULE_TB_EXCLUSIVE_PAYMENT_TYPE]
GO
ALTER TABLE [dbo].[TB_CLAIM_SCHEDULE]  WITH CHECK ADD  CONSTRAINT [FK_TB_CLAIM_SCHEDULE_TB_PAYDATE_CALENDAR] FOREIGN KEY([Paydate_Calendar_ID])
REFERENCES [dbo].[TB_PAYDATE_CALENDAR] ([Paydate_Calendar_ID])
GO
ALTER TABLE [dbo].[TB_CLAIM_SCHEDULE] CHECK CONSTRAINT [FK_TB_CLAIM_SCHEDULE_TB_PAYDATE_CALENDAR]
GO
ALTER TABLE [dbo].[TB_CLAIM_SCHEDULE]  WITH CHECK ADD  CONSTRAINT [FK_TB_CLAIM_SCHEDULE_TB_PAYMENT_EXCHANGE_ENTITY_INFO] FOREIGN KEY([Payee_Entity_Info_ID])
REFERENCES [dbo].[TB_PAYMENT_EXCHANGE_ENTITY_INFO] ([Payment_Exchange_Entity_Info_ID])
GO
ALTER TABLE [dbo].[TB_CLAIM_SCHEDULE] CHECK CONSTRAINT [FK_TB_CLAIM_SCHEDULE_TB_PAYMENT_EXCHANGE_ENTITY_INFO]
GO
ALTER TABLE [dbo].[TB_CLAIM_SCHEDULE_DN_STATUS]  WITH CHECK ADD  CONSTRAINT [FK_TB_CLAIM_SCHEDULE_DN_STATUS_TB_CLAIM_SCHEDULE] FOREIGN KEY([Claim_Schedule_ID])
REFERENCES [dbo].[TB_CLAIM_SCHEDULE] ([Claim_Schedule_ID])
GO
ALTER TABLE [dbo].[TB_CLAIM_SCHEDULE_DN_STATUS] CHECK CONSTRAINT [FK_TB_CLAIM_SCHEDULE_DN_STATUS_TB_CLAIM_SCHEDULE]
GO
ALTER TABLE [dbo].[TB_CLAIM_SCHEDULE_DN_STATUS]  WITH CHECK ADD  CONSTRAINT [FK_TB_CLAIM_SCHEDULE_DN_STATUS_TB_CLAIM_SCHEDULE_STATUS] FOREIGN KEY([CurrentCSStatusID])
REFERENCES [dbo].[TB_CLAIM_SCHEDULE_STATUS] ([Claim_Schedule_Status_ID])
GO
ALTER TABLE [dbo].[TB_CLAIM_SCHEDULE_DN_STATUS] CHECK CONSTRAINT [FK_TB_CLAIM_SCHEDULE_DN_STATUS_TB_CLAIM_SCHEDULE_STATUS]
GO
ALTER TABLE [dbo].[TB_CLAIM_SCHEDULE_DN_STATUS]  WITH CHECK ADD  CONSTRAINT [FK_TB_CLAIM_SCHEDULE_DN_STATUS_TB_CLAIM_SCHEDULE_STATUS1] FOREIGN KEY([LatestCSStatusID])
REFERENCES [dbo].[TB_CLAIM_SCHEDULE_STATUS] ([Claim_Schedule_Status_ID])
GO
ALTER TABLE [dbo].[TB_CLAIM_SCHEDULE_DN_STATUS] CHECK CONSTRAINT [FK_TB_CLAIM_SCHEDULE_DN_STATUS_TB_CLAIM_SCHEDULE_STATUS1]
GO
ALTER TABLE [dbo].[TB_CLAIM_SCHEDULE_ECS]  WITH CHECK ADD  CONSTRAINT [FK_TB_CLAIM_SCHEDULE_ECS_TB_CLAIM_SCHEDULE] FOREIGN KEY([Claim_Schedule_ID])
REFERENCES [dbo].[TB_CLAIM_SCHEDULE] ([Claim_Schedule_ID])
GO
ALTER TABLE [dbo].[TB_CLAIM_SCHEDULE_ECS] CHECK CONSTRAINT [FK_TB_CLAIM_SCHEDULE_ECS_TB_CLAIM_SCHEDULE]
GO
ALTER TABLE [dbo].[TB_CLAIM_SCHEDULE_ECS]  WITH CHECK ADD  CONSTRAINT [FK_TB_CLAIM_SCHEDULE_ECS_TB_ECS] FOREIGN KEY([ECS_ID])
REFERENCES [dbo].[TB_ECS] ([ECS_ID])
GO
ALTER TABLE [dbo].[TB_CLAIM_SCHEDULE_ECS] CHECK CONSTRAINT [FK_TB_CLAIM_SCHEDULE_ECS_TB_ECS]
GO
ALTER TABLE [dbo].[TB_CLAIM_SCHEDULE_REMITTANCE_ADVICE_NOTE]  WITH CHECK ADD  CONSTRAINT [FK_TB_CLAIM_SCHEDULE_REMITTANCE_ADVICE_NOTE_TB_CLAIM_SCHEDULE] FOREIGN KEY([Claim_Schedule_ID])
REFERENCES [dbo].[TB_CLAIM_SCHEDULE] ([Claim_Schedule_ID])
GO
ALTER TABLE [dbo].[TB_CLAIM_SCHEDULE_REMITTANCE_ADVICE_NOTE] CHECK CONSTRAINT [FK_TB_CLAIM_SCHEDULE_REMITTANCE_ADVICE_NOTE_TB_CLAIM_SCHEDULE]
GO
ALTER TABLE [dbo].[TB_CLAIM_SCHEDULE_STATUS]  WITH CHECK ADD  CONSTRAINT [FK_TB_CLAIM_SCHEDULE_STATUS_TB_CLAIM_SCHEDULE] FOREIGN KEY([Claim_Schedule_ID])
REFERENCES [dbo].[TB_CLAIM_SCHEDULE] ([Claim_Schedule_ID])
GO
ALTER TABLE [dbo].[TB_CLAIM_SCHEDULE_STATUS] CHECK CONSTRAINT [FK_TB_CLAIM_SCHEDULE_STATUS_TB_CLAIM_SCHEDULE]
GO
ALTER TABLE [dbo].[TB_CLAIM_SCHEDULE_STATUS]  WITH CHECK ADD  CONSTRAINT [FK_TB_CLAIM_SCHEDULE_STATUS_TB_CLAIM_SCHEDULE_STATUS_TYPE] FOREIGN KEY([Claim_Schedule_Status_Type_ID])
REFERENCES [dbo].[TB_CLAIM_SCHEDULE_STATUS_TYPE] ([Claim_Schedule_Status_Type_ID])
GO
ALTER TABLE [dbo].[TB_CLAIM_SCHEDULE_STATUS] CHECK CONSTRAINT [FK_TB_CLAIM_SCHEDULE_STATUS_TB_CLAIM_SCHEDULE_STATUS_TYPE]
GO
ALTER TABLE [dbo].[TB_CLAIM_SCHEDULE_USER_ASSIGNMENT]  WITH CHECK ADD  CONSTRAINT [FK_TB_CLAIM_SCHEDULE_USER_ASSIGNMENT_TB_CLAIM_SCHEDULE] FOREIGN KEY([Claim_Schedule_ID])
REFERENCES [dbo].[TB_CLAIM_SCHEDULE] ([Claim_Schedule_ID])
GO
ALTER TABLE [dbo].[TB_CLAIM_SCHEDULE_USER_ASSIGNMENT] CHECK CONSTRAINT [FK_TB_CLAIM_SCHEDULE_USER_ASSIGNMENT_TB_CLAIM_SCHEDULE]
GO
ALTER TABLE [dbo].[TB_CLAIM_SCHEDULE_USER_ASSIGNMENT]  WITH CHECK ADD  CONSTRAINT [FK_TB_CLAIM_SCHEDULE_USER_ASSIGNMENT_TB_CLAIM_SCHEDULE_STATUS] FOREIGN KEY([Assigned_CS_Status_ID])
REFERENCES [dbo].[TB_CLAIM_SCHEDULE_STATUS] ([Claim_Schedule_Status_ID])
GO
ALTER TABLE [dbo].[TB_CLAIM_SCHEDULE_USER_ASSIGNMENT] CHECK CONSTRAINT [FK_TB_CLAIM_SCHEDULE_USER_ASSIGNMENT_TB_CLAIM_SCHEDULE_STATUS]
GO
ALTER TABLE [dbo].[TB_CLAIM_SCHEDULE_USER_ASSIGNMENT]  WITH CHECK ADD  CONSTRAINT [FK_TB_CLAIM_SCHEDULE_USER_ASSIGNMENT_TB_USER] FOREIGN KEY([User_ID])
REFERENCES [dbo].[TB_USER] ([User_ID])
GO
ALTER TABLE [dbo].[TB_CLAIM_SCHEDULE_USER_ASSIGNMENT] CHECK CONSTRAINT [FK_TB_CLAIM_SCHEDULE_USER_ASSIGNMENT_TB_USER]
GO
ALTER TABLE [dbo].[TB_CLAIM_SCHEDULE_WARRANT]  WITH CHECK ADD  CONSTRAINT [FK_TB_CLAIM_SCHEDULE_WARRANT_TB_CLAIM_SCHEDULE] FOREIGN KEY([Claim_Schedule_ID])
REFERENCES [dbo].[TB_CLAIM_SCHEDULE] ([Claim_Schedule_ID])
GO
ALTER TABLE [dbo].[TB_CLAIM_SCHEDULE_WARRANT] CHECK CONSTRAINT [FK_TB_CLAIM_SCHEDULE_WARRANT_TB_CLAIM_SCHEDULE]
GO
ALTER TABLE [dbo].[TB_CLAIM_SCHEDULE_WARRANT]  WITH CHECK ADD  CONSTRAINT [FK_TB_CLAIM_SCHEDULE_WARRANT_TB_WARRANT] FOREIGN KEY([Warrant_ID])
REFERENCES [dbo].[TB_WARRANT] ([Warrant_ID])
GO
ALTER TABLE [dbo].[TB_CLAIM_SCHEDULE_WARRANT] CHECK CONSTRAINT [FK_TB_CLAIM_SCHEDULE_WARRANT_TB_WARRANT]
GO
ALTER TABLE [dbo].[TB_DB_SETTING]  WITH CHECK ADD  CONSTRAINT [FK_TB_DB_SETTING_TB_DB_SETTING_TYPE] FOREIGN KEY([DB_Setting_Type_ID])
REFERENCES [dbo].[TB_DB_SETTING_TYPE] ([DB_Setting_Type_ID])
GO
ALTER TABLE [dbo].[TB_DB_SETTING] CHECK CONSTRAINT [FK_TB_DB_SETTING_TB_DB_SETTING_TYPE]
GO
ALTER TABLE [dbo].[TB_ECS]  WITH CHECK ADD  CONSTRAINT [FK_TB_ECS_TB_ECS_STATUS_TYPE1] FOREIGN KEY([Current_ECS_Status_Type_ID])
REFERENCES [dbo].[TB_ECS_STATUS_TYPE] ([ECS_Status_Type_ID])
GO
ALTER TABLE [dbo].[TB_ECS] CHECK CONSTRAINT [FK_TB_ECS_TB_ECS_STATUS_TYPE1]
GO
ALTER TABLE [dbo].[TB_ECS]  WITH CHECK ADD  CONSTRAINT [FK_TB_ECS_TB_EXCLUSIVE_PAYMENT_TYPE] FOREIGN KEY([Exclusive_Payment_Type_ID])
REFERENCES [dbo].[TB_EXCLUSIVE_PAYMENT_TYPE] ([Exclusive_Payment_Type_ID])
GO
ALTER TABLE [dbo].[TB_ECS] CHECK CONSTRAINT [FK_TB_ECS_TB_EXCLUSIVE_PAYMENT_TYPE]
GO
ALTER TABLE [dbo].[TB_FUNDING_DETAIL]  WITH CHECK ADD  CONSTRAINT [FK_TB_FUNDING_DETAIL_TB_PAYMENT_RECORD] FOREIGN KEY([Payment_Record_ID])
REFERENCES [dbo].[TB_PAYMENT_RECORD] ([Payment_Record_ID])
GO
ALTER TABLE [dbo].[TB_FUNDING_DETAIL] CHECK CONSTRAINT [FK_TB_FUNDING_DETAIL_TB_PAYMENT_RECORD]
GO
ALTER TABLE [dbo].[TB_FUNDING_DETAIL_EXT_CAPMAN]  WITH CHECK ADD  CONSTRAINT [FK_TB_FUNDING_DETAIL_EXT_CAPMAN_TB_FUNDING_DETAIL] FOREIGN KEY([Funding_Detail_ID])
REFERENCES [dbo].[TB_FUNDING_DETAIL] ([Funding_Detail_ID])
GO
ALTER TABLE [dbo].[TB_FUNDING_DETAIL_EXT_CAPMAN] CHECK CONSTRAINT [FK_TB_FUNDING_DETAIL_EXT_CAPMAN_TB_FUNDING_DETAIL]
GO
ALTER TABLE [dbo].[TB_FUNDING_DETAIL_KVP]  WITH CHECK ADD  CONSTRAINT [FK_TB_FUNDING_DETAIL_KVP_TB_FUNDING_DETAIL] FOREIGN KEY([Funding_Detail_ID])
REFERENCES [dbo].[TB_FUNDING_DETAIL] ([Funding_Detail_ID])
GO
ALTER TABLE [dbo].[TB_FUNDING_DETAIL_KVP] CHECK CONSTRAINT [FK_TB_FUNDING_DETAIL_KVP_TB_FUNDING_DETAIL]
GO
ALTER TABLE [dbo].[TB_FUNDING_DETAIL_KVP]  WITH CHECK ADD  CONSTRAINT [FK_TB_FUNDING_DETAIL_KVP_TB_SOR_KVP_KEY] FOREIGN KEY([SOR_Kvp_Key_ID])
REFERENCES [dbo].[TB_SOR_KVP_KEY] ([SOR_Kvp_Key_ID])
GO
ALTER TABLE [dbo].[TB_FUNDING_DETAIL_KVP] CHECK CONSTRAINT [FK_TB_FUNDING_DETAIL_KVP_TB_SOR_KVP_KEY]
GO
ALTER TABLE [dbo].[TB_PAYMENT_CLAIM_SCHEDULE]  WITH CHECK ADD  CONSTRAINT [FK_TB_PAYMENT_CLAIM_SCHEDULE_TB_CLAIM_SCHEDULE] FOREIGN KEY([Claim_Schedule_ID])
REFERENCES [dbo].[TB_CLAIM_SCHEDULE] ([Claim_Schedule_ID])
GO
ALTER TABLE [dbo].[TB_PAYMENT_CLAIM_SCHEDULE] CHECK CONSTRAINT [FK_TB_PAYMENT_CLAIM_SCHEDULE_TB_CLAIM_SCHEDULE]
GO
ALTER TABLE [dbo].[TB_PAYMENT_CLAIM_SCHEDULE]  WITH CHECK ADD  CONSTRAINT [FK_TB_PAYMENT_CLAIM_SCHEDULE_TB_PAYMENT_RECORD] FOREIGN KEY([Payment_Record_ID])
REFERENCES [dbo].[TB_PAYMENT_RECORD] ([Payment_Record_ID])
GO
ALTER TABLE [dbo].[TB_PAYMENT_CLAIM_SCHEDULE] CHECK CONSTRAINT [FK_TB_PAYMENT_CLAIM_SCHEDULE_TB_PAYMENT_RECORD]
GO
ALTER TABLE [dbo].[TB_PAYMENT_DN_STATUS]  WITH CHECK ADD  CONSTRAINT [FK_TB_PAYMENT_DN_STATUS_TB_PAYMENT_RECORD] FOREIGN KEY([Payment_Record_ID])
REFERENCES [dbo].[TB_PAYMENT_RECORD] ([Payment_Record_ID])
GO
ALTER TABLE [dbo].[TB_PAYMENT_DN_STATUS] CHECK CONSTRAINT [FK_TB_PAYMENT_DN_STATUS_TB_PAYMENT_RECORD]
GO
ALTER TABLE [dbo].[TB_PAYMENT_DN_STATUS]  WITH CHECK ADD  CONSTRAINT [FK_TB_PAYMENT_DN_STATUS_TB_PAYMENT_STATUS] FOREIGN KEY([CurrentPaymentStatusID])
REFERENCES [dbo].[TB_PAYMENT_STATUS] ([Payment_Status_ID])
GO
ALTER TABLE [dbo].[TB_PAYMENT_DN_STATUS] CHECK CONSTRAINT [FK_TB_PAYMENT_DN_STATUS_TB_PAYMENT_STATUS]
GO
ALTER TABLE [dbo].[TB_PAYMENT_DN_STATUS]  WITH CHECK ADD  CONSTRAINT [FK_TB_PAYMENT_DN_STATUS_TB_PAYMENT_STATUS1] FOREIGN KEY([CurrentHoldStatusID])
REFERENCES [dbo].[TB_PAYMENT_STATUS] ([Payment_Status_ID])
GO
ALTER TABLE [dbo].[TB_PAYMENT_DN_STATUS] CHECK CONSTRAINT [FK_TB_PAYMENT_DN_STATUS_TB_PAYMENT_STATUS1]
GO
ALTER TABLE [dbo].[TB_PAYMENT_DN_STATUS]  WITH CHECK ADD  CONSTRAINT [FK_TB_PAYMENT_DN_STATUS_TB_PAYMENT_STATUS4] FOREIGN KEY([CurrentUnHoldStatusID])
REFERENCES [dbo].[TB_PAYMENT_STATUS] ([Payment_Status_ID])
GO
ALTER TABLE [dbo].[TB_PAYMENT_DN_STATUS] CHECK CONSTRAINT [FK_TB_PAYMENT_DN_STATUS_TB_PAYMENT_STATUS4]
GO
ALTER TABLE [dbo].[TB_PAYMENT_DN_STATUS]  WITH CHECK ADD  CONSTRAINT [FK_TB_PAYMENT_DN_STATUS_TB_PAYMENT_STATUS5] FOREIGN KEY([CurrentReleaseFromSupStatusID])
REFERENCES [dbo].[TB_PAYMENT_STATUS] ([Payment_Status_ID])
GO
ALTER TABLE [dbo].[TB_PAYMENT_DN_STATUS] CHECK CONSTRAINT [FK_TB_PAYMENT_DN_STATUS_TB_PAYMENT_STATUS5]
GO
ALTER TABLE [dbo].[TB_PAYMENT_DN_STATUS]  WITH CHECK ADD  CONSTRAINT [FK_TB_PAYMENT_DN_STATUS_TB_PAYMENT_USER_ASSIGNMENT] FOREIGN KEY([CurrentUserAssignmentID])
REFERENCES [dbo].[TB_PAYMENT_USER_ASSIGNMENT] ([Payment_User_Assignment_ID])
GO
ALTER TABLE [dbo].[TB_PAYMENT_DN_STATUS] CHECK CONSTRAINT [FK_TB_PAYMENT_DN_STATUS_TB_PAYMENT_USER_ASSIGNMENT]
GO
ALTER TABLE [dbo].[TB_PAYMENT_EXCHANGE_ENTITY_INFO]  WITH CHECK ADD  CONSTRAINT [FK_TB_PAYMENT_EXCHANGE_ENTITY_INFO_TB_PAYMENT_EXCHANGE_ENTITY] FOREIGN KEY([Payment_Exchange_Entity_ID])
REFERENCES [dbo].[TB_PAYMENT_EXCHANGE_ENTITY] ([Payment_Exchange_Entity_ID])
GO
ALTER TABLE [dbo].[TB_PAYMENT_EXCHANGE_ENTITY_INFO] CHECK CONSTRAINT [FK_TB_PAYMENT_EXCHANGE_ENTITY_INFO_TB_PAYMENT_EXCHANGE_ENTITY]
GO
ALTER TABLE [dbo].[TB_PAYMENT_KVP]  WITH CHECK ADD  CONSTRAINT [FK_TB_PAYMENT_KVP_TB_PAYMENT_RECORD] FOREIGN KEY([Payment_Record_ID])
REFERENCES [dbo].[TB_PAYMENT_RECORD] ([Payment_Record_ID])
GO
ALTER TABLE [dbo].[TB_PAYMENT_KVP] CHECK CONSTRAINT [FK_TB_PAYMENT_KVP_TB_PAYMENT_RECORD]
GO
ALTER TABLE [dbo].[TB_PAYMENT_KVP]  WITH CHECK ADD  CONSTRAINT [FK_TB_PAYMENT_KVP_TB_SOR_KVP_KEY] FOREIGN KEY([SOR_Kvp_Key_ID])
REFERENCES [dbo].[TB_SOR_KVP_KEY] ([SOR_Kvp_Key_ID])
GO
ALTER TABLE [dbo].[TB_PAYMENT_KVP] CHECK CONSTRAINT [FK_TB_PAYMENT_KVP_TB_SOR_KVP_KEY]
GO
ALTER TABLE [dbo].[TB_PAYMENT_PAYDATE_CALENDAR]  WITH CHECK ADD  CONSTRAINT [FK_TB_PAYMENT_PAYDATE_CALENDAR_TB_PAYDATE_CALENDAR] FOREIGN KEY([Paydate_Calendar_ID])
REFERENCES [dbo].[TB_PAYDATE_CALENDAR] ([Paydate_Calendar_ID])
GO
ALTER TABLE [dbo].[TB_PAYMENT_PAYDATE_CALENDAR] CHECK CONSTRAINT [FK_TB_PAYMENT_PAYDATE_CALENDAR_TB_PAYDATE_CALENDAR]
GO
ALTER TABLE [dbo].[TB_PAYMENT_PAYDATE_CALENDAR]  WITH CHECK ADD  CONSTRAINT [FK_TB_PAYMENT_PAYDATE_CALENDAR_TB_PAYMENT_RECORD] FOREIGN KEY([Payment_Record_ID])
REFERENCES [dbo].[TB_PAYMENT_RECORD] ([Payment_Record_ID])
GO
ALTER TABLE [dbo].[TB_PAYMENT_PAYDATE_CALENDAR] CHECK CONSTRAINT [FK_TB_PAYMENT_PAYDATE_CALENDAR_TB_PAYMENT_RECORD]
GO
ALTER TABLE [dbo].[TB_PAYMENT_RECORD]  WITH CHECK ADD  CONSTRAINT [FK_TB_PAYMENT_RECORD_TB_PAYMENT_EXCHANGE_ENTITY_INFO] FOREIGN KEY([Payee_Entity_Info_ID])
REFERENCES [dbo].[TB_PAYMENT_EXCHANGE_ENTITY_INFO] ([Payment_Exchange_Entity_Info_ID])
GO
ALTER TABLE [dbo].[TB_PAYMENT_RECORD] CHECK CONSTRAINT [FK_TB_PAYMENT_RECORD_TB_PAYMENT_EXCHANGE_ENTITY_INFO]
GO
ALTER TABLE [dbo].[TB_PAYMENT_RECORD]  WITH CHECK ADD  CONSTRAINT [FK_TB_PAYMENT_RECORD_TB_TRANSACTION] FOREIGN KEY([Transaction_ID])
REFERENCES [dbo].[TB_TRANSACTION] ([Transaction_ID])
GO
ALTER TABLE [dbo].[TB_PAYMENT_RECORD] CHECK CONSTRAINT [FK_TB_PAYMENT_RECORD_TB_TRANSACTION]
GO
ALTER TABLE [dbo].[TB_PAYMENT_RECORD_EXT_CAPMAN]  WITH CHECK ADD  CONSTRAINT [FK_TB_PAYMENT_RECORD_EXT_CAPMAN_TB_EXCLUSIVE_PAYMENT_TYPE] FOREIGN KEY([Exclusive_Payment_Type_ID])
REFERENCES [dbo].[TB_EXCLUSIVE_PAYMENT_TYPE] ([Exclusive_Payment_Type_ID])
GO
ALTER TABLE [dbo].[TB_PAYMENT_RECORD_EXT_CAPMAN] CHECK CONSTRAINT [FK_TB_PAYMENT_RECORD_EXT_CAPMAN_TB_EXCLUSIVE_PAYMENT_TYPE]
GO
ALTER TABLE [dbo].[TB_PAYMENT_RECORD_EXT_CAPMAN]  WITH CHECK ADD  CONSTRAINT [FK_TB_PAYMENT_RECORD_EXT_CAPMAN_TB_PAYMENT_RECORD] FOREIGN KEY([Payment_Record_ID])
REFERENCES [dbo].[TB_PAYMENT_RECORD] ([Payment_Record_ID])
GO
ALTER TABLE [dbo].[TB_PAYMENT_RECORD_EXT_CAPMAN] CHECK CONSTRAINT [FK_TB_PAYMENT_RECORD_EXT_CAPMAN_TB_PAYMENT_RECORD]
GO
ALTER TABLE [dbo].[TB_PAYMENT_STATUS]  WITH CHECK ADD  CONSTRAINT [FK_TB_PAYMENT_STATUS_TB_PAYMENT_RECORD] FOREIGN KEY([Payment_Record_ID])
REFERENCES [dbo].[TB_PAYMENT_RECORD] ([Payment_Record_ID])
GO
ALTER TABLE [dbo].[TB_PAYMENT_STATUS] CHECK CONSTRAINT [FK_TB_PAYMENT_STATUS_TB_PAYMENT_RECORD]
GO
ALTER TABLE [dbo].[TB_PAYMENT_STATUS]  WITH CHECK ADD  CONSTRAINT [FK_TB_PAYMENT_STATUS_TB_PAYMENT_STATUS_TYPE] FOREIGN KEY([Payment_Status_Type_ID])
REFERENCES [dbo].[TB_PAYMENT_STATUS_TYPE] ([Payment_Status_Type_ID])
GO
ALTER TABLE [dbo].[TB_PAYMENT_STATUS] CHECK CONSTRAINT [FK_TB_PAYMENT_STATUS_TB_PAYMENT_STATUS_TYPE]
GO
ALTER TABLE [dbo].[TB_PAYMENT_STATUS_TYPE_INTERNAL_TO_EXTERNAL]  WITH CHECK ADD  CONSTRAINT [FK_TB_PAYMENT_STATUS_TYPE_INTERNAL_TO_EXTERNAL_TB_PAYMENT_STATUS_TYPE] FOREIGN KEY([Payment_Status_Type_ID])
REFERENCES [dbo].[TB_PAYMENT_STATUS_TYPE] ([Payment_Status_Type_ID])
GO
ALTER TABLE [dbo].[TB_PAYMENT_STATUS_TYPE_INTERNAL_TO_EXTERNAL] CHECK CONSTRAINT [FK_TB_PAYMENT_STATUS_TYPE_INTERNAL_TO_EXTERNAL_TB_PAYMENT_STATUS_TYPE]
GO
ALTER TABLE [dbo].[TB_PAYMENT_STATUS_TYPE_INTERNAL_TO_EXTERNAL]  WITH CHECK ADD  CONSTRAINT [FK_TB_PAYMENT_STATUS_TYPE_INTERNAL_TO_EXTERNAL_TB_PAYMENT_STATUS_TYPE_EXTERNAL] FOREIGN KEY([Payment_Status_Type_Ext_ID])
REFERENCES [dbo].[TB_PAYMENT_STATUS_TYPE_EXTERNAL] ([Payment_Status_Type_Ext_ID])
GO
ALTER TABLE [dbo].[TB_PAYMENT_STATUS_TYPE_INTERNAL_TO_EXTERNAL] CHECK CONSTRAINT [FK_TB_PAYMENT_STATUS_TYPE_INTERNAL_TO_EXTERNAL_TB_PAYMENT_STATUS_TYPE_EXTERNAL]
GO
ALTER TABLE [dbo].[TB_PAYMENT_USER_ASSIGNMENT]  WITH CHECK ADD  CONSTRAINT [FK_TB_PAYMENT_USER_ASSIGNMENT_TB_PAYMENT_RECORD] FOREIGN KEY([Payment_Record_ID])
REFERENCES [dbo].[TB_PAYMENT_RECORD] ([Payment_Record_ID])
GO
ALTER TABLE [dbo].[TB_PAYMENT_USER_ASSIGNMENT] CHECK CONSTRAINT [FK_TB_PAYMENT_USER_ASSIGNMENT_TB_PAYMENT_RECORD]
GO
ALTER TABLE [dbo].[TB_PAYMENT_USER_ASSIGNMENT]  WITH CHECK ADD  CONSTRAINT [FK_TB_PAYMENT_USER_ASSIGNMENT_TB_PAYMENT_STATUS] FOREIGN KEY([Assigned_Payment_Status_ID])
REFERENCES [dbo].[TB_PAYMENT_STATUS] ([Payment_Status_ID])
GO
ALTER TABLE [dbo].[TB_PAYMENT_USER_ASSIGNMENT] CHECK CONSTRAINT [FK_TB_PAYMENT_USER_ASSIGNMENT_TB_PAYMENT_STATUS]
GO
ALTER TABLE [dbo].[TB_PAYMENT_USER_ASSIGNMENT]  WITH CHECK ADD  CONSTRAINT [FK_TB_PAYMENT_USER_ASSIGNMENT_TB_USER] FOREIGN KEY([User_ID])
REFERENCES [dbo].[TB_USER] ([User_ID])
GO
ALTER TABLE [dbo].[TB_PAYMENT_USER_ASSIGNMENT] CHECK CONSTRAINT [FK_TB_PAYMENT_USER_ASSIGNMENT_TB_USER]
GO
ALTER TABLE [dbo].[TB_RESPONSE]  WITH CHECK ADD  CONSTRAINT [FK_TB_RESPONSE_TB_REQUEST1] FOREIGN KEY([Response_ID])
REFERENCES [dbo].[TB_REQUEST] ([Request_ID])
GO
ALTER TABLE [dbo].[TB_RESPONSE] CHECK CONSTRAINT [FK_TB_RESPONSE_TB_REQUEST1]
GO
ALTER TABLE [dbo].[TB_RESPONSE]  WITH CHECK ADD  CONSTRAINT [FK_TB_RESPONSE_TB_RESPONSE_STATUS_TYPE] FOREIGN KEY([Response_Status_Type_ID])
REFERENCES [dbo].[TB_RESPONSE_STATUS_TYPE] ([Response_Status_Type_ID])
GO
ALTER TABLE [dbo].[TB_RESPONSE] CHECK CONSTRAINT [FK_TB_RESPONSE_TB_RESPONSE_STATUS_TYPE]
GO
ALTER TABLE [dbo].[TB_ROLE_PERMISSION]  WITH CHECK ADD  CONSTRAINT [FK_Role_Permission_Permission] FOREIGN KEY([Permission_ID])
REFERENCES [dbo].[TB_PERMISSION] ([Permission_ID])
GO
ALTER TABLE [dbo].[TB_ROLE_PERMISSION] CHECK CONSTRAINT [FK_Role_Permission_Permission]
GO
ALTER TABLE [dbo].[TB_ROLE_PERMISSION]  WITH CHECK ADD  CONSTRAINT [FK_Role_Permission_Role] FOREIGN KEY([Role_ID])
REFERENCES [dbo].[TB_ROLE] ([Role_ID])
GO
ALTER TABLE [dbo].[TB_ROLE_PERMISSION] CHECK CONSTRAINT [FK_Role_Permission_Role]
GO
ALTER TABLE [dbo].[TB_SOR_KVP_KEY]  WITH CHECK ADD  CONSTRAINT [FK_TB_SOR_KVP_KEY_TB_SYSTEM_OF_RECORD] FOREIGN KEY([SOR_ID])
REFERENCES [dbo].[TB_SYSTEM_OF_RECORD] ([SOR_ID])
GO
ALTER TABLE [dbo].[TB_SOR_KVP_KEY] CHECK CONSTRAINT [FK_TB_SOR_KVP_KEY_TB_SYSTEM_OF_RECORD]
GO
ALTER TABLE [dbo].[TB_SYSTEM_USER]  WITH CHECK ADD  CONSTRAINT [FK_System_User_System] FOREIGN KEY([System_ID])
REFERENCES [dbo].[TB_SYSTEM] ([System_ID])
GO
ALTER TABLE [dbo].[TB_SYSTEM_USER] CHECK CONSTRAINT [FK_System_User_System]
GO
ALTER TABLE [dbo].[TB_SYSTEM_USER]  WITH CHECK ADD  CONSTRAINT [FK_System_User_User] FOREIGN KEY([User_ID])
REFERENCES [dbo].[TB_USER] ([User_ID])
GO
ALTER TABLE [dbo].[TB_SYSTEM_USER] CHECK CONSTRAINT [FK_System_User_User]
GO
ALTER TABLE [dbo].[TB_TRACE_PAYMENT]  WITH CHECK ADD  CONSTRAINT [FK_TB_TRACE_INVOICE_TB_INVOICE_STATUS_TYPE] FOREIGN KEY([Payment_Status_Type_ID])
REFERENCES [dbo].[TB_PAYMENT_STATUS_TYPE] ([Payment_Status_Type_ID])
GO
ALTER TABLE [dbo].[TB_TRACE_PAYMENT] CHECK CONSTRAINT [FK_TB_TRACE_INVOICE_TB_INVOICE_STATUS_TYPE]
GO
ALTER TABLE [dbo].[TB_TRACE_PAYMENT]  WITH CHECK ADD  CONSTRAINT [FK_TB_TRACE_INVOICE_TB_TRACE_TRANSACTION] FOREIGN KEY([Trace_Transaction_ID])
REFERENCES [dbo].[TB_TRACE_TRANSACTION] ([Trace_Transaction_ID])
GO
ALTER TABLE [dbo].[TB_TRACE_PAYMENT] CHECK CONSTRAINT [FK_TB_TRACE_INVOICE_TB_TRACE_TRANSACTION]
GO
ALTER TABLE [dbo].[TB_TRACE_TRANSACTION]  WITH CHECK ADD  CONSTRAINT [FK_TB_TRACE_TRANSACTION_TB_REQUEST1] FOREIGN KEY([Trace_Transaction_ID])
REFERENCES [dbo].[TB_REQUEST] ([Request_ID])
GO
ALTER TABLE [dbo].[TB_TRACE_TRANSACTION] CHECK CONSTRAINT [FK_TB_TRACE_TRANSACTION_TB_REQUEST1]
GO
ALTER TABLE [dbo].[TB_TRANSACTION]  WITH CHECK ADD  CONSTRAINT [FK_TB_TRANSACTION_TB_REQUEST] FOREIGN KEY([Transaction_ID])
REFERENCES [dbo].[TB_REQUEST] ([Request_ID])
GO
ALTER TABLE [dbo].[TB_TRANSACTION] CHECK CONSTRAINT [FK_TB_TRANSACTION_TB_REQUEST]
GO
ALTER TABLE [dbo].[TB_TRANSACTION]  WITH CHECK ADD  CONSTRAINT [FK_TB_TRANSACTION_TB_SYSTEM_OF_RECORD] FOREIGN KEY([SOR_ID])
REFERENCES [dbo].[TB_SYSTEM_OF_RECORD] ([SOR_ID])
GO
ALTER TABLE [dbo].[TB_TRANSACTION] CHECK CONSTRAINT [FK_TB_TRANSACTION_TB_SYSTEM_OF_RECORD]
GO
ALTER TABLE [dbo].[TB_TRANSACTION]  WITH CHECK ADD  CONSTRAINT [FK_TB_TRANSACTION_TB_TRANSACTION_TYPE] FOREIGN KEY([Transaction_Type_ID])
REFERENCES [dbo].[TB_TRANSACTION_TYPE] ([Transaction_Type_ID])
GO
ALTER TABLE [dbo].[TB_TRANSACTION] CHECK CONSTRAINT [FK_TB_TRANSACTION_TB_TRANSACTION_TYPE]
GO
ALTER TABLE [dbo].[TB_TRANSACTION_PAYER_ENTITY]  WITH CHECK ADD  CONSTRAINT [FK_TB_TRANSACTION_PAYER_ENTITY_TB_PAYMENT_EXCHANGE_ENTITY] FOREIGN KEY([Payment_Exchange_Entity_ID])
REFERENCES [dbo].[TB_PAYMENT_EXCHANGE_ENTITY] ([Payment_Exchange_Entity_ID])
GO
ALTER TABLE [dbo].[TB_TRANSACTION_PAYER_ENTITY] CHECK CONSTRAINT [FK_TB_TRANSACTION_PAYER_ENTITY_TB_PAYMENT_EXCHANGE_ENTITY]
GO
ALTER TABLE [dbo].[TB_USER]  WITH CHECK ADD  CONSTRAINT [FK_User_User_Type] FOREIGN KEY([User_Type_ID])
REFERENCES [dbo].[TB_USER_TYPE] ([User_Type_ID])
GO
ALTER TABLE [dbo].[TB_USER] CHECK CONSTRAINT [FK_User_User_Type]
GO
ALTER TABLE [dbo].[TB_USER_ROLE]  WITH CHECK ADD  CONSTRAINT [FK_User_Role_Role] FOREIGN KEY([Role_ID])
REFERENCES [dbo].[TB_ROLE] ([Role_ID])
GO
ALTER TABLE [dbo].[TB_USER_ROLE] CHECK CONSTRAINT [FK_User_Role_Role]
GO
ALTER TABLE [dbo].[TB_USER_ROLE]  WITH CHECK ADD  CONSTRAINT [FK_User_Role_User] FOREIGN KEY([User_ID])
REFERENCES [dbo].[TB_USER] ([User_ID])
GO
ALTER TABLE [dbo].[TB_USER_ROLE] CHECK CONSTRAINT [FK_User_Role_User]
GO
