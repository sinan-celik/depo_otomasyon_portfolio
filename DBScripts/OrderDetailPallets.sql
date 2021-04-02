
/****** Object:  StoredProcedure [dbo].[sp_OrderDetailPallets_sel]    Script Date: Sep 10 2020  3:00PM  ******/
USE [WarehouseSimulation];
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- ===================================================================
-- Author      : scelik
-- Create date : 09/10/2020
-- Revised date: 
-- Description : orderdetilpalletste ilgili detail için kaç paletin daha indirileceğini döndürür
-- ===================================================================

CREATE OR ALTER PROCEDURE [dbo].[sp_RemainCountByOrderDetailId_sel]
  (
	@OrderDetailId int
  )
AS
BEGIN
	SET NOCOUNT ON;

  SELECT COUNT(Id)
  FROM [WarehouseSimulation].[dbo].[OrderDetailPallets]
  WHERE IsTaken = 0
    AND OrderDetailId = @OrderDetailId

END
GO


/****** Object:  StoredProcedure [dbo].[sp_OrderDetailPallets_sel]    Script Date: Sep 10 2020  3:00PM  ******/
USE [WarehouseSimulation];
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- ===================================================================
-- Author      : scelik
-- Create date : 09/10/2020
-- Revised date: 
-- Description : Select Addresses
-- ===================================================================

CREATE OR ALTER PROCEDURE [dbo].[sp_OrderDetailPallets_sel]
  (@ID int=NULL)
AS
BEGIN
  IF @ID IS NULL OR @ID = 0
    SELECT * FROM [dbo].[OrderDetailPallets] ORDER BY [Id] DESC;
  ELSE
    SELECT * FROM [dbo].[OrderDetailPallets] WHERE [ID] = @ID;
END
GO

/****** Object:  StoredProcedure [dbo].[sp_OrderDetailPallets_del]    Script Date: Sep 10 2020  3:00PM  ******/
USE [WarehouseSimulation];
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- ===================================================================
-- Author      : scelik
-- Create date : 09/10/2020
-- Revised date: 
-- Description : Delete Addresses
-- ===================================================================

CREATE OR ALTER PROCEDURE [dbo].[sp_OrderDetailPallets_del]
  (@ID int)
AS
BEGIN
  SET NOCOUNT ON;
  DELETE FROM [dbo].[OrderDetailPallets] WHERE [Id]=@ID;
  SELECT @@ROWCOUNT as [Rows Affected];
END
GO
