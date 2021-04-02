/****** Object:  StoredProcedure [dbo].[sp_Buffers_ups]    Script Date: Aug 24 2020 11:36AM  ******/
USE [WarehouseSimulation];
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- ===================================================================
-- Author      : scelik
-- Create date : 08/24/2020
-- Revised date: 
-- Description : Upsert Addresses
-- ===================================================================

CREATE OR ALTER PROCEDURE [dbo].[sp_Buffers_ups]
  (@ID int=NULL,@Code nvarchar(10)=NULL,@LineNumber int=NULL,@BufferLetter nvarchar(2)=NULL,@IsActive bit=NULL,@IsEmpty bit=NULL,@DistanceType nvarchar(50)=NULL,@DistanceToRefPoint decimal=NULL,@LastPaletteInfo nvarchar(50)=NULL,@IsDeleted bit=NULL)
AS
BEGIN
  IF @ID IS NULL OR @ID = 0
    BEGIN
      INSERT INTO [dbo].[Buffers]
        ([Code],[LineNumber],[BufferLetter],[IsActive],[IsEmpty],[DistanceType],[DistanceToRefPoint],[LastPaletteInfo],[IsDeleted])
      VALUES
        (@Code,@LineNumber,@BufferLetter,@IsActive,@IsEmpty,@DistanceType,@DistanceToRefPoint,@LastPaletteInfo,@IsDeleted);
      SELECT * FROM [dbo].[Buffers] WHERE [ID] = SCOPE_IDENTITY();
    END
  ELSE
    BEGIN
      UPDATE [dbo].[Buffers]
        SET [Code]=@Code,[LineNumber]=@LineNumber,[BufferLetter]=@BufferLetter,[IsActive]=@IsActive,[IsEmpty]=@IsEmpty,[DistanceType]=@DistanceType,[DistanceToRefPoint]=@DistanceToRefPoint,[LastPaletteInfo]=@LastPaletteInfo,[IsDeleted]=@IsDeleted
        WHERE ([Id] = @ID);
      SELECT * FROM [dbo].[Buffers] WHERE [ID] = @ID;
    END
END
GO

/****** Object:  StoredProcedure [dbo].[sp_Buffers_sel]    Script Date: Aug 24 2020 11:36AM  ******/
USE [WarehouseSimulation];
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- ===================================================================
-- Author      : scelik
-- Create date : 08/24/2020
-- Revised date: 
-- Description : Select Addresses
-- ===================================================================

CREATE OR ALTER PROCEDURE [dbo].[sp_Buffers_sel]
  (@ID int=NULL)
AS
BEGIN
  IF @ID IS NULL OR @ID = 0
    SELECT * FROM [dbo].[Buffers] ORDER BY [Id] DESC;
  ELSE
    SELECT * FROM [dbo].[Buffers] WHERE [ID] = @ID;
END
GO

/****** Object:  StoredProcedure [dbo].[sp_Buffers_del]    Script Date: Aug 24 2020 11:36AM  ******/
USE [WarehouseSimulation];
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- ===================================================================
-- Author      : scelik
-- Create date : 08/24/2020
-- Revised date: 
-- Description : Delete Addresses
-- ===================================================================

CREATE OR ALTER PROCEDURE [dbo].[sp_Buffers_del]
  (@ID int)
AS
BEGIN
  SET NOCOUNT ON;
  DELETE FROM [dbo].[Buffers] WHERE [Id]=@ID;
  SELECT @@ROWCOUNT as [Rows Affected];
END
GO


/****** Object:  StoredProcedure [dbo].[sp_Buffers_del]    Script Date: Aug 24 2020 11:36AM  ******/
USE [WarehouseSimulation];
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- ===================================================================
-- Author      : scelik
-- Create date : 08/24/2020
-- Revised date: 
-- Description : Delete Addresses
-- ===================================================================

CREATE OR ALTER PROCEDURE [dbo].[sp_SelectAppropiriateBuffer_sel]
AS
BEGIN
  SET NOCOUNT ON;


  SELECT TOP 1 *
  FROM [WarehouseSimulation].[dbo].[Buffers]
  WHERE IsActive = 1 AND IsEmpty = 1 AND IsDeleted = 0
  ORDER BY DistanceToRefPoint ASC


END
GO


/****** Object:  StoredProcedure [dbo].[sp_Buffers_del]    Script Date: Aug 24 2020 11:36AM  ******/
USE [WarehouseSimulation];
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- ===================================================================
-- Author      : scelik
-- Create date : 08/24/2020
-- Revised date: 
-- Description : Delete Addresses
-- ===================================================================

CREATE OR ALTER PROCEDURE [dbo].[sp_UpdateBufferLoadInfo_upd]
(
    @Code nvarchar(10),
    @IsEmpty bit,
    @LastPaletteInfo nvarchar(50)
)
AS
BEGIN
  SET NOCOUNT ON;


    UPDATE Buffers 
    SET LastPaletteInfo = @LastPaletteInfo, 
        IsEmpty = @IsEmpty 
    WHERE Code = @Code


END
GO