USE [EAMI-MC]
GO
/****** Object:  StoredProcedure [dbo].[GetDexFileNameList]    Script Date: 5/28/2019 10:08:03 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetDexFileNameList]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetDexFileNameList]
GO
/****** Object:  StoredProcedure [dbo].[GetDexFileNameList]    Script Date: 5/28/2019 10:08:03 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/******************************************************************************
* PROCEDURE:  [dbo].[GetDexFileNameList]
* PURPOSE: Get Dex File Name List
* NOTES:
* CREATED: 02/21/2019  Genady G.
* MODIFIED
* DATE     AUTHOR      DESCRIPTION
*------------------------------------------------------------------------------
*****************************************************************************/
CREATE PROCEDURE [dbo].[GetDexFileNameList]  
	@DexFileNameList VARCHAR(max)
AS
BEGIN
	BEGIN TRY	
	
		-- convert list to xml
		DECLARE @xml XML
		SET @xml = N'<root><r>' + replace(@DexFileNameList, ',', '</r><r>') + '</r></root>'
		
		--DEX FILE NAME LIST FROM PARAM
		CREATE TABLE #dexFileNameList ([DexFileName] VARCHAR (200), [DoesExist] BIT);
		
		INSERT INTO #dexFileNameList
		SELECT t.value('.', 'VARCHAR(200)'), 0 FROM @xml.nodes('//root/r') AS a(t)

		IF ((SELECT COUNT(*) FROM #dexFileNameList) > 0)
		BEGIN
			--DETERMINE IF DEX FILE NAME ALREADY EXISTS
			UPDATE DEX_FILE 
			SET DEX_FILE.[DoesExist] = 1 
			FROM #dexFileNameList AS DEX_FILE
			INNER JOIN [dbo].TB_ECS AS ECS ON DEX_FILE.DexFileName = ECS.DexFileName 
		END

		--SELECT DEX FILES THAT DO NOT EXIST YET
		SELECT [DexFileName] AS [DexFileName] FROM #dexFileNameList WHERE [DoesExist] = 0

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
