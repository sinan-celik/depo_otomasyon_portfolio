
/****** Object:  StoredProcedure [dbo].[sp_Shuttles_sel]    Script Date: Sep 29 2020 11:44AM  ******/
USE [WarehouseSimulation];
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- ===================================================================
-- Author      : scelik
-- Create date : 09/29/2020
-- Revised date: 
-- Description : Select Addresses
-- ===================================================================
CREATE OR ALTER PROCEDURE [dbo].[sp_Shuttles_sel]
  (@ID int=NULL)
AS
BEGIN
  IF @ID IS NULL OR @ID = 0
    SELECT * FROM [dbo].[Shuttles] ORDER BY [Id] DESC;
  ELSE
    SELECT * FROM [dbo].[Shuttles] WHERE [ID] = @ID;
END
GO



/****** Object:  StoredProcedure [dbo].[sp_Shuttles_del]    Script Date: Sep 29 2020 11:44AM  ******/
USE [WarehouseSimulation];
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- ===================================================================
-- Author      : scelik
-- Create date : 09/29/2020
-- Revised date: 
-- Description : Delete Addresses
-- ===================================================================
CREATE OR ALTER PROCEDURE [dbo].[sp_Shuttles_del]
  (@ID int)
AS
BEGIN
  SET NOCOUNT ON;
  DELETE FROM [dbo].[Shuttles] WHERE [Id]=@ID;
  SELECT @@ROWCOUNT as [Rows Affected];
END
GO




/****** Object:  StoredProcedure [dbo].[]    Script Date: Sep 29 2020 11:44AM  ******/
USE [WarehouseSimulation];
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- ===================================================================
-- Author      : scelik
-- Create date : 09/29/2020
-- Revised date: 
-- Description : update Addresses
-- ===================================================================
CREATE OR ALTER PROCEDURE [dbo].[sp_SelectNewOptimizationShuttle_upd]
AS
BEGIN
  SET NOCOUNT ON;

		DECLARE @ShCode nvarchar(50)

		SELECT TOP 1 @ShCode = Code 
		FROM Shuttles
		WHERE Assignment = 'IO' 
		  AND [Status] = 'READY'

		UPDATE Shuttles SET Assignment = 'OPTIMIZATION' WHERE Code = @ShCode

		SELECT * FROM Shuttles WHERE Code = @ShCode
END
GO
