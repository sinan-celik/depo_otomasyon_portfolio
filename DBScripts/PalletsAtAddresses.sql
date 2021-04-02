/****** Object:  StoredProcedure [dbo].[sp_PalletsAtAddresses_ups]    Script Date: Sep 14 2020 11:25AM  ******/
USE [WarehouseSimulation];
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- ===================================================================
-- Author      : scelik
-- Create date : 09/14/2020
-- Revised date: 
-- Description : Upsert palletatAddresses
-- ===================================================================

CREATE OR ALTER PROCEDURE [dbo].[sp_PalletsAtAddresses_ups]
  (@ID int=NULL,@AddressId int=NULL,@ProductionNotificationId int=NULL,@ProductId int=NULL,@DistanceToExit int=NULL,@AddressCode nvarchar(50)=NULL,@PaletteBarcode nvarchar(50)=NULL,@BoxQuantity int=NULL,@Lot nvarchar(50)=NULL,@EntryDate datetime=NULL,@EntryReason nvarchar(50)=NULL,@ReleaseDate datetime=NULL,@ReleaseReason nvarchar(50)=NULL,@IsInside bit=NULL)
AS
BEGIN
  IF @ID IS NULL OR @ID = 0
    BEGIN
      INSERT INTO [dbo].[PalletsAtAddresses]
        ([AddressId],[ProductionNotificationId],[ProductId],[DistanceToExit],[AddressCode],[PaletteBarcode],[BoxQuantity],[Lot],[EntryDate],[EntryReason],[ReleaseDate],[ReleaseReason],[IsInside])
      VALUES
        (@AddressId,@ProductionNotificationId,@ProductId,@DistanceToExit,@AddressCode,@PaletteBarcode,@BoxQuantity,@Lot,@EntryDate,@EntryReason,@ReleaseDate,@ReleaseReason,@IsInside);
      SELECT * FROM [dbo].[PalletsAtAddresses] WHERE [ID] = SCOPE_IDENTITY();
    END
  ELSE
    BEGIN
      UPDATE [dbo].[PalletsAtAddresses]
        SET [AddressId]=@AddressId,[ProductionNotificationId]=@ProductionNotificationId,[ProductId]=@ProductId,[DistanceToExit]=@DistanceToExit,[AddressCode]=@AddressCode,[PaletteBarcode]=@PaletteBarcode,[BoxQuantity]=@BoxQuantity,[Lot]=@Lot,[EntryDate]=@EntryDate,[EntryReason]=@EntryReason,[ReleaseDate]=@ReleaseDate,[ReleaseReason]=@ReleaseReason,[IsInside]=@IsInside
        WHERE ([Id] = @ID);
      SELECT * FROM [dbo].[PalletsAtAddresses] WHERE [ID] = @ID;
    END
END
GO

/****** Object:  StoredProcedure [dbo].[sp_PalletsAtAddresses_sel]    Script Date: Sep 14 2020 11:25AM  ******/
USE [WarehouseSimulation];
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- ===================================================================
-- Author      : scelik
-- Create date : 09/14/2020
-- Revised date: 
-- Description : Select palletatAddresses
-- ===================================================================

CREATE OR ALTER PROCEDURE [dbo].[sp_PalletsAtAddresses_sel]
  (@ID int=NULL)
AS
BEGIN
  IF @ID IS NULL OR @ID = 0
    SELECT * FROM [dbo].[PalletsAtAddresses] ORDER BY [Id] DESC;
  ELSE
    SELECT * FROM [dbo].[PalletsAtAddresses] WHERE [ID] = @ID;
END
GO

/****** Object:  StoredProcedure [dbo].[sp_PalletsAtAddresses_del]    Script Date: Sep 14 2020 11:25AM  ******/
USE [WarehouseSimulation];
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- ===================================================================
-- Author      : scelik
-- Create date : 09/14/2020
-- Revised date: 
-- Description : Delete palletatAddresses
-- ===================================================================

CREATE OR ALTER PROCEDURE [dbo].[sp_PalletsAtAddresses_del]
  (@ID int)
AS
BEGIN
  SET NOCOUNT ON;
  DELETE FROM [dbo].[PalletsAtAddresses] WHERE [Id]=@ID;
  SELECT @@ROWCOUNT as [Rows Affected];
END
GO






/****** Object:  StoredProcedure [dbo].[sp_PalletsAtAddresses_del]    Script Date: Sep 14 2020 11:25AM  ******/
USE [WarehouseSimulation];
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- ===================================================================
-- Author      : scelik
-- Create date : 09/14/2020
-- Revised date: 
-- Description : cursor update palletatAddresses
-- ===================================================================

CREATE OR ALTER PROCEDURE [dbo].[sp_OptimizationUpdatePalletsAtAddresses_upd]
(
  
  @AddressCode nvarchar(50),
  @StartDistance int = 100,--mm
  @increment int = 100
)
AS
BEGIN
  SET NOCOUNT ON;


	DECLARE @Id int;  
  
	DECLARE opt_cursor CURSOR FOR  
	SELECT [Id] FROM [WarehouseSimulation].[dbo].[PalletsAtAddresses]
	WHERE AddressCode = @AddressCode  
	ORDER BY EntryDate;  
  
	OPEN opt_cursor;  
  
	FETCH NEXT FROM opt_cursor  
	INTO @Id;  
  
	WHILE @@FETCH_STATUS = 0  
	BEGIN  
  
		SET @StartDistance = @StartDistance + @increment;

		UPDATE PalletsAtAddresses 
			SET DistanceToExit = @StartDistance 
		WHERE Id = @Id
	
	   FETCH NEXT FROM opt_cursor  
	   INTO @Id;  
	END  
  
	CLOSE opt_cursor;  
	DEALLOCATE opt_cursor;  



END
GO






/****** Object:  StoredProcedure [dbo].[sp_GetNextOptimizationNeededAddresses_sel]    Script Date: Sep 25 2020 11:25AM  ******/
USE [WarehouseSimulation];
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- ===================================================================
-- Author      : scelik
-- Create date : 09/25/2020
-- Revised date: 
-- Description : Optimizasyon yapılması gerekli adresleri seçmeyi amaçlar
-- ===================================================================

CREATE OR ALTER PROCEDURE [dbo].[sp_GetNextOptimizationNeededAddresses_sel]
AS
BEGIN
  SET NOCOUNT ON;
 

	 DECLARE @MaxEmptyOUT int = (SELECT ParameterValue FROM SystemParameters WHERE ParameterName = 'MAX_EMPTY_SPACE_OUT')
		   ,@MaxEmptyIN int = (SELECT ParameterValue FROM SystemParameters WHERE ParameterName = 'MAX_EMPTY_SPACE_IN')
		   ,@MaxDIFF int = (SELECT ParameterValue FROM SystemParameters WHERE ParameterName = 'MAX_DIFF_PALETTE_COUNT_FOOTPRINT')
		   ,@TubeHasUNLOAD bit = (SELECT ParameterValue FROM SystemParameters WHERE ParameterName = 'TUBE_UNLOAD')
		   ,@TubeHasLOAD bit = (SELECT ParameterValue FROM SystemParameters WHERE ParameterName = 'TUBE_LOAD')
		   ,@PalletSize int = (SELECT ParameterValue FROM SystemParameters WHERE ParameterName = 'PALLET_SIZE')



	SELECT * INTO #tmp FROM (
		SELECT AddressCode 
			  ,MIN(DistanceToExit) cıkısaMesafe
			  ,MAX(DistanceToExit)+ @PalletSize giriseMesafe
			  ,(MAX(DistanceToExit) - MIN(DistanceToExit)+@PalletSize) /@PalletSize olmasıTahminEdilenPalet
			  ,COUNT(*) olanPalet
			  ,null hasUnload
			  ,null hasLoad
		FROM PalletsAtAddresses
		WHERE IsInside = 1
		GROUP BY AddressCode) t





	DECLARE @AddressCode NVARCHAR(50)
	DECLARE @productId int
	DECLARE @hasUNLOAD bit
	DECLARE @hasLOAD bit

	DECLARE CRS_1 CURSOR FOR SELECT AddressCode FROM #tmp
	OPEN CRS_1
	FETCH NEXT FROM CRS_1 INTO @AddressCode
	WHILE @@FETCH_STATUS =0
		BEGIN
		SET @productId = (SELECT DependedProductId FROM Addresses WHERE Code = @AddressCode AND Direction = 'WH_IN')

		IF EXISTS (SELECT * FROM OrderDetailPallets odp LEFT OUTER JOIN Addresses a ON odp.ProductId = a.DependedProductId AND Direction = 'WH_OUT'  WHERE ProductId = @productId)
			SET @hasUNLOAD = 1
		ELSE SET @hasUNLOAD = 0
 
					--gün içinde ilk üretim saati ve belli süre sonra hala üretilip üretilmediğini yakalayan bir sorgu yazılması gereklidir
					--productionnotification tablosundan son üretilen ürünlerin üzerinden ne kadar süre geçtiği ve hesabı
				   
		IF EXISTS (SELECT * FROM PreProductionNotifications ppn LEFT OUTER JOIN Addresses a ON ppn.ProductId = a.DependedProductId AND Direction = 'WH_IN' WHERE ProductId =@productId)
			SET @hasLOAD = 1
		ELSE SET @hasLOAD = 0
 

		UPDATE #tmp 
		SET hasUnload = @hasUNLOAD, 
			hasLoad = @hasLOAD 
		WHERE AddressCode = @AddressCode


		FETCH NEXT FROM CRS_1 INTO @AddressCode
		END
	CLOSE CRS_1 
	DEALLOCATE CRS_1 



		SELECT * FROM #tmp
		WHERE (
			   cıkısaMesafe > @MaxEmptyOUT
		  or giriseMesafe > @MaxEmptyIN
		  or (olmasıTahminEdilenPalet -olanPalet) > (@MaxDIFF/@PalletSize) 
		  )
		  and hasUnload = @hasUNLOAD
		  and hasLoad = @hasLOAD
	

END
GO
