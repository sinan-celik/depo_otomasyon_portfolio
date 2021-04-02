/****** Object:  StoredProcedure [dbo].[sp_ProductionNotifications_ups]    Script Date: Aug 20 2020 10:10AM  ******/
USE [WarehouseSimulation];
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- ===================================================================
-- Author      : scelik
-- Create date : 08/20/2020
-- Revised date: 
-- Description : 
-- ===================================================================

CREATE OR ALTER PROCEDURE [dbo].[sp_ProductionNotifications_ups]
  (@ID int=NULL,@ProductId int=NULL,@PaletteBarcode nvarchar(50)=NULL,@BatchNo int=NULL,@NotificationTime datetime=NULL,@WeightUnit nvarchar(50)=NULL,@Weight decimal=NULL,@BoxQuantity int=NULL,@Lot nvarchar(50)=NULL,@BoxDataReaded bit=NULL)
AS
BEGIN
  IF @ID IS NULL OR @ID = 0
    BEGIN
      INSERT INTO [dbo].[ProductionNotifications]
        ([ProductId],[PaletteBarcode],[BatchNo],[NotificationTime],[WeightUnit],[Weight],[BoxQuantity],[Lot],[BoxDataReaded])
      VALUES
        (@ProductId,@PaletteBarcode,@BatchNo,@NotificationTime,@WeightUnit,@Weight,@BoxQuantity,@Lot,@BoxDataReaded);
      SELECT * FROM [dbo].[ProductionNotifications] WHERE [ID] = SCOPE_IDENTITY();
    END
  ELSE
    BEGIN
      UPDATE [dbo].[ProductionNotifications]
        SET [ProductId]=@ProductId,[PaletteBarcode]=@PaletteBarcode,[BatchNo]=@BatchNo,[NotificationTime]=@NotificationTime,[WeightUnit]=@WeightUnit,[Weight]=@Weight,[BoxQuantity]=@BoxQuantity,[Lot]=@Lot,[BoxDataReaded]=@BoxDataReaded
        WHERE ([Id] = @ID);
      SELECT * FROM [dbo].[ProductionNotifications] WHERE [ID] = @ID;
    END
END
GO

/****** Object:  StoredProcedure [dbo].[sp_ProductionNotifications_sel]    Script Date: Aug 20 2020 10:10AM  ******/
USE [WarehouseSimulation];
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- ===================================================================
-- Author      : scelik
-- Create date : 08/20/2020
-- Revised date: 
-- Description : 
-- ===================================================================

CREATE OR ALTER PROCEDURE [dbo].[sp_ProductionNotifications_sel]
  (@ID int=NULL)
AS
BEGIN
  IF @ID IS NULL OR @ID = 0
    SELECT * FROM [dbo].[ProductionNotifications] ORDER BY [Id] DESC;
  ELSE
    SELECT * FROM [dbo].[ProductionNotifications] WHERE [ID] = @ID;
END
GO

/****** Object:  StoredProcedure [dbo].[sp_ProductionNotifications_del]    Script Date: Aug 20 2020 10:10AM  ******/
USE [WarehouseSimulation];
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- ===================================================================
-- Author      : scelik
-- Create date : 08/20/2020
-- Revised date: 
-- Description : 
-- ===================================================================

CREATE OR ALTER PROCEDURE [dbo].[sp_ProductionNotifications_del]
  (@ID int)
AS
BEGIN
  SET NOCOUNT ON;
  DELETE FROM [dbo].[ProductionNotifications] WHERE [Id]=@ID;
  SELECT @@ROWCOUNT as [Rows Affected];
END
GO



/****** Object:  StoredProcedure [dbo].[sp_ProductionNotifications_del]    Script Date: Aug 20 2020 10:10AM  ******/
USE [WarehouseSimulation];
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- ===================================================================
-- Author      : scelik
-- Create date : 08/20/2020
-- Revised date: 
-- Description : Delete Addresses
-- ===================================================================

CREATE OR ALTER PROCEDURE [dbo].[sp_SelectWaitingPaletteCount]
  (@StartTime datetime)
AS
BEGIN
  SET NOCOUNT ON;

  SELECT COUNT(pn.[Id])
   --   ,pn.[ProductId]
   --   ,pn.[PaletteBarcode]
   --   ,pn.[BatchNo]
   --   ,pn.[NotificationTime]
   --   ,pn.[WeightUnit]
   --   ,pn.[Weight]
   --   ,pn.[BoxQuantity]
   --   ,pn.[Lot]
   --   ,pn.[BoxDataReaded]
	  --,mt.*
  FROM [ProductionNotifications] pn(NOLOCK)
  LEFT OUTER JOIN MachineTasks mt(NOLOCK) ON pn.Id = mt.TaskBatch AND mt.[Sequence] = 3 --taskbatchinin son adımnın tamamlanma durumuna göre 
  WHERE pn.NotificationTime < @StartTime 
	AND mt.IsCompleted = 0


END
GO
