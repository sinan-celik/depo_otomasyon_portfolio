/****** Object:  StoredProcedure [dbo].[sp_TimeParameters_ups]    Script Date: Aug 14 2020  3:25PM  ******/
USE [WarehouseSimulation];
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- ===================================================================
-- Author      : scelik
-- Create date : 08/14/2020
-- Revised date: 
-- Description : Upsert Addresses
-- ===================================================================

CREATE OR ALTER PROCEDURE [dbo].[sp_TimeParameters_ups]
  (@ID int=NULL,@MachineType nvarchar(50)=NULL,@MovementType nvarchar(50)=NULL,@TimeInSeconds int=NULL,@MeasureUnit nvarchar(50)=NULL,@MeasureValue decimal=NULL)
AS
BEGIN
  IF @ID IS NULL OR @ID = 0
    BEGIN
      INSERT INTO [dbo].[TimeParameters]
        ([MachineType],[MovementType],[TimeInSeconds],[MeasureUnit],[MeasureValue])
      VALUES
        (@MachineType,@MovementType,@TimeInSeconds,@MeasureUnit,@MeasureValue);
      SELECT * FROM [dbo].[TimeParameters] WHERE [ID] = SCOPE_IDENTITY();
    END
  ELSE
    BEGIN
      UPDATE [dbo].[TimeParameters]
        SET [MachineType]=@MachineType,[MovementType]=@MovementType,[TimeInSeconds]=@TimeInSeconds,[MeasureUnit]=@MeasureUnit,[MeasureValue]=@MeasureValue
        WHERE ([Id] = @ID);
      SELECT * FROM [dbo].[TimeParameters] WHERE [ID] = @ID;
    END
END
GO

/****** Object:  StoredProcedure [dbo].[sp_TimeParameters_sel]    Script Date: Aug 14 2020  3:25PM  ******/
USE [WarehouseSimulation];
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- ===================================================================
-- Author      : scelik
-- Create date : 08/14/2020
-- Revised date: 
-- Description : Select Addresses
-- ===================================================================

CREATE OR ALTER PROCEDURE [dbo].[sp_TimeParameters_sel]
  (@ID int=NULL)
AS
BEGIN
  IF @ID IS NULL OR @ID = 0
    SELECT * FROM [dbo].[TimeParameters] ORDER BY [Id] DESC;
  ELSE
    SELECT * FROM [dbo].[TimeParameters] WHERE [ID] = @ID;
END
GO

/****** Object:  StoredProcedure [dbo].[sp_TimeParameters_del]    Script Date: Aug 14 2020  3:25PM  ******/
USE [WarehouseSimulation];
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- ===================================================================
-- Author      : scelik
-- Create date : 08/14/2020
-- Revised date: 
-- Description : Delete Addresses
-- ===================================================================

CREATE OR ALTER PROCEDURE [dbo].[sp_TimeParameters_del]
  (@ID int)
AS
BEGIN
  SET NOCOUNT ON;
  DELETE FROM [dbo].[TimeParameters] WHERE [Id]=@ID;
  SELECT @@ROWCOUNT as [Rows Affected];
END
GO


