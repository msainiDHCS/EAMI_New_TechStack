USE [EAMI-MC]
GO
/****** Object:  StoredProcedure [dbo].[GetECSFileNameIncrement]    Script Date: 5/28/2019 10:08:03 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetECSFileNameIncrement]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetECSFileNameIncrement]
GO
/****** Object:  StoredProcedure [dbo].[GetECSFileNameIncrement]    Script Date: 5/28/2019 10:08:03 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/******************************************************************************
* PROCEDURE:  [dbo].[GetECSFileNameIncrement]
* PURPOSE: Get ECS File Name Increment from the provied FileName mask and Date
* NOTES:
* CREATED: 10/26/2018  Genady G.
* MODIFIED
* DATE			AUTHOR      DESCRIPTION
* 02/11/2019	Genady G	Remove @file_Name_Part_2 from determening the Incerement value
* 01/14/2021	Genady G	Add back @file_Name_Part_2 from determening the Incerement value
*------------------------------------------------------------------------------
*****************************************************************************/

CREATE PROCEDURE [dbo].[GetECSFileNameIncrement]
	@File_Name VARCHAR (50) = null,
	@Date_Stamp DATETIME
AS
BEGIN
	BEGIN TRY
		
		DECLARE @result_Increment VARCHAR (1)
		DECLARE @ecs_Date_Value VARCHAR (6) = FORMAT(@Date_Stamp, 'MMddyy')
		DECLARE @file_Name_Separator VARCHAR (3) = '{0}'
		DECLARE @file_Name_Separator_Index int = CHARINDEX(@file_Name_Separator, @File_Name)
		DECLARE @file_Name_Part_1 VARCHAR (50)  = SUBSTRING(@File_Name, 1, @file_Name_Separator_Index - 1)
		DECLARE @file_Name_Part_2 VARCHAR (50)  = SUBSTRING(@File_Name, @file_Name_Separator_Index + 3, LEN(@File_Name)) 
		
		--INCREMENT TABLE
		CREATE TABLE #Increment (
			[Increment_Value] CHAR
		);

		CREATE TABLE #ECS_FileName_Match (
			[File_Name] VARCHAR (50),
			[File_Name_Part_1] VARCHAR (50),
			[File_Name_Part_2] VARCHAR (50),
			[File_Name_Incrementor] CHAR
		);

		--GET ALL POSIIBLE INCREMENT VALUES (A-Z ALPHABET)
		WITH cte_tally as (SELECT row_number() OVER (ORDER BY (SELECT 1)) AS n FROM sys.all_columns)
		INSERT INTO #Increment SELECT CHAR(n) AS alpha FROM cte_tally WHERE (n > 64 AND n < 91) 

		--GET EXISTING MATCHING FILE NAMES IF ANY
		INSERT INTO #ECS_FileName_Match 
		SELECT 
			ECS_File_Name,
			@file_Name_Part_1 + @ecs_Date_Value,
			@file_Name_Part_2,
			SUBSTRING(ECS_File_Name, LEN(@file_Name_Part_1 + @ecs_Date_Value) + 1, 1)
		FROM 
			TB_ECS 
		WHERE 
			ECS_File_Name LIKE(@file_Name_Part_1 + @ecs_Date_Value + '%')
			AND ECS_File_Name LIKE('%' + @file_Name_Part_2) 

		--DETERMINE RESULT INCREMENT
		IF NOT EXISTS(SELECT NULL FROM #ECS_FileName_Match)
		BEGIN			
			SET @result_Increment = (SELECT TOP 1 [Increment_Value] FROM #Increment ORDER BY [Increment_Value] ASC)
		END
		ELSE
		BEGIN
			SET @result_Increment = (SELECT TOP 1 [Increment_Value] 
			FROM #Increment 
			WHERE  [Increment_Value] NOT IN (SELECT [File_Name_Incrementor] FROM #ECS_FileName_Match)
			ORDER BY [Increment_Value] ASC)
		END

		--SELECT RESULT INCREMENTOR
		SELECT @result_Increment AS [NEXT_INCREMENT_VALUE]
				
		--DROP TEMP TABLE
		drop table #Increment
		drop table #ECS_FileName_Match

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
