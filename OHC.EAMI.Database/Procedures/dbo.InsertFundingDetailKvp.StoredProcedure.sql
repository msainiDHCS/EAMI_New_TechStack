USE [EAMI-MC]
GO
/****** Object:  StoredProcedure [dbo].[InsertFundingDetailKvp]    Script Date: 5/28/2019 10:08:03 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertFundingDetailKvp]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[InsertFundingDetailKvp]
GO
/****** Object:  StoredProcedure [dbo].[InsertFundingDetailKvp]    Script Date: 5/28/2019 10:08:03 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/******************************************************************************
* PROCEDURE:  [dbo].[InsertFundingDetailKvp]
* PURPOSE: Inserts FundingDetail KeyValuePair 
* NOTES:
* CREATED: 09/19/2017  Eugene S.
* MODIFIED
* DATE     AUTHOR      DESCRIPTION
*------------------------------------------------------------------------------
*****************************************************************************/
CREATE PROCEDURE [dbo].[InsertFundingDetailKvp] 
	@FundingDetailId INT
	,@SorKvpKeyId INT
	,@KvpValue VARCHAR(100)
AS
BEGIN
	BEGIN TRY
		INSERT INTO [dbo].[TB_FUNDING_DETAIL_KVP] (
			[Funding_Detail_ID]
			,[SOR_Kvp_Key_ID]
			,[Kvp_Value]
			)
		VALUES (
			@FundingDetailId
			,@SorKvpKeyId
			,@KvpValue
			)
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
