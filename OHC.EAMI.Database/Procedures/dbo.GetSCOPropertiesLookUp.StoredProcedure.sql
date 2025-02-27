USE [EAMI-MC]
GO
/****** Object:  StoredProcedure [dbo].[GetSCOPropertiesLookUp]    Script Date: 10/27/2023 12:51:42 PM ******/

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetSCOPropertiesLookUp]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetSCOPropertiesLookUp]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/******************************************************************************
* PROCEDURE:  [dbo].[GetSCOPropertiesLookUp]
* PURPOSE: Fetches the list of SCO PROPERTY TYPES and Property Enums
* NOTES:
* CREATED: 01/29/2024  Meetu S.
* MODIFIED
* DATE		AUTHOR      DESCRIPTION
*------------------------------------------------------------------------------
-- exec [GetSCOPropertiesLookUp]
*****************************************************************************/
ALTER PROCEDURE [dbo].[GetSCOPropertiesLookUp]
	-- Add the parameters for the stored procedure here	
	--@systemID int
AS
BEGIN

		SELECT *
		FROM TB_SCO_PROPERTY_TYPE as SPT
		WHERE SPT.IsActive = 1

		SELECT *
		FROM TB_SCO_PROPERTY_ENUM as SPE
		WHERE SPE.IsActive = 1
			
END;
