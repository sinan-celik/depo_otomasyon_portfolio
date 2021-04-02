/****** Object:  StoredProcedure [dbo].[sp_Addresses_ups]    Script Date: Aug 18 2020  2:58PM  ******/
USE [WarehouseSimulation];
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- ===================================================================
-- Author      : scelik
-- Create date : 08/18/2020
-- Revised date: 
-- Description : Upsert Addresses
-- ===================================================================

CREATE OR ALTER PROCEDURE [dbo].[sp_Addresses_ups]
  (@ID int=NULL,@Code nvarchar(50)=NULL,@LineNumber int=NULL,@ShelfLetter nvarchar(10)=NULL,@Direction nvarchar(10)=NULL,@IsActive bit=NULL,@BlockReason nvarchar(50)=NULL,@FirstRowIsEmpty bit=NULL,@LastLoadTime datetime=NULL,@DistanceType nvarchar(50)=NULL,@DistanceToRefPoint decimal=NULL,@DependedProductId int=NULL,@CurrentDensity decimal=NULL,@DensityMeasurementType nvarchar(50)=NULL,@MaxDensityValue decimal=NULL,@IsDeleted bit=NULL)
AS
BEGIN
  IF @ID IS NULL OR @ID = 0
    BEGIN
      INSERT INTO [dbo].[Addresses]
        ([Code],[LineNumber],[ShelfLetter],[Direction],[IsActive],[BlockReason],[FirstRowIsEmpty],[LastLoadTime],[DistanceType],[DistanceToRefPoint],[DependedProductId],[CurrentDensity],[DensityMeasurementType],[MaxDensityValue],[IsDeleted])
      VALUES
        (@Code,@LineNumber,@ShelfLetter,@Direction,@IsActive,@BlockReason,@FirstRowIsEmpty,@LastLoadTime,@DistanceType,@DistanceToRefPoint,@DependedProductId,@CurrentDensity,@DensityMeasurementType,@MaxDensityValue,@IsDeleted);
      SELECT * FROM [dbo].[Addresses] WHERE [ID] = SCOPE_IDENTITY();
    END
  ELSE
    BEGIN
      UPDATE [dbo].[Addresses]
        SET [Code]=@Code,[LineNumber]=@LineNumber,[ShelfLetter]=@ShelfLetter,[Direction]=@Direction,[IsActive]=@IsActive,[BlockReason]=@BlockReason,[FirstRowIsEmpty]=@FirstRowIsEmpty,[LastLoadTime]=@LastLoadTime,[DistanceType]=@DistanceType,[DistanceToRefPoint]=@DistanceToRefPoint,[DependedProductId]=@DependedProductId,[CurrentDensity]=@CurrentDensity,[DensityMeasurementType]=@DensityMeasurementType,[MaxDensityValue]=@MaxDensityValue,[IsDeleted]=@IsDeleted
        WHERE ([Id] = @ID);
      SELECT * FROM [dbo].[Addresses] WHERE [ID] = @ID;
    END
END
GO

/****** Object:  StoredProcedure [dbo].[sp_Addresses_sel]    Script Date: Aug 18 2020  2:58PM  ******/
USE [WarehouseSimulation];
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- ===================================================================
-- Author      : scelik
-- Create date : 08/18/2020
-- Revised date: 
-- Description : Select Addresses
-- ===================================================================

CREATE OR ALTER PROCEDURE [dbo].[sp_Addresses_sel]
  (@ID int=NULL)
AS
BEGIN
  IF @ID IS NULL OR @ID = 0
    SELECT * FROM [dbo].[Addresses] ORDER BY [Id] DESC;
  ELSE
    SELECT * FROM [dbo].[Addresses] WHERE [ID] = @ID;
END
GO

/****** Object:  StoredProcedure [dbo].[sp_Addresses_del]    Script Date: Aug 18 2020  2:58PM  ******/
USE [WarehouseSimulation];
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- ===================================================================
-- Author      : scelik
-- Create date : 08/18/2020
-- Revised date: 
-- Description : Delete Addresses
-- ===================================================================

CREATE OR ALTER PROCEDURE [dbo].[sp_Addresses_del]
  (@ID int)
AS
BEGIN
  SET NOCOUNT ON;
  DELETE FROM [dbo].[Addresses] WHERE [Id]=@ID;
  SELECT @@ROWCOUNT as [Rows Affected];
END
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- ===================================================================
-- Author      : scelik
-- Create date : 08/18/2020
-- Revised date: 
-- Description : Select Addresses
-- ===================================================================

CREATE OR ALTER PROCEDURE [dbo].[sp_SelectRelatedAppropiriateAddress_sel]
  (@ProductID int=NULL)
AS
BEGIN
	SET NOCOUNT ON;

DECLARE @Address nvarchar(50) = '';
 
 SELECT TOP 1 @Address = [Code]
  FROM [WarehouseSimulation].[dbo].[Addresses] 
  WHERE DependedProductId = @ProductID 
	AND Direction = 'WH_IN'
	AND IsActive = 1
	AND FirstRowIsEmpty = 1
	AND [IsDeleted] = 0
	--AND [BlockReason] is null -- daha sonra aktıf hale getirilmeli 
	--mesafe ile ilgili kriterler eklenebilir.

	SELECT @Address as [Address]
END
GO


/****** Object:  StoredProcedure [dbo].[sp_Addresses_del]      ******/
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
-- Description : 
-- ===================================================================

CREATE OR ALTER PROCEDURE [dbo].[sp_AddressesChangeFirstRowInfo_upd]
(
 @Code nvarchar(50),
 @FirstRowIsEmpty bit,
 @LastLoadTime datetime,
 @Direction nvarchar(50)
)
AS
BEGIN
  SET NOCOUNT ON;
 
  IF @FirstRowIsEmpty = 0
	UPDATE Addresses SET LastLoadTime = @LastLoadTime, FirstRowIsEmpty =  @FirstRowIsEmpty WHERE Code = @Code AND Direction = @Direction 
  ELSE IF @FirstRowIsEmpty = 1
    UPDATE Addresses SET LastLoadTime = null, FirstRowIsEmpty =  @FirstRowIsEmpty WHERE Code = @Code AND Direction = @Direction

END
GO



/****** Object:  StoredProcedure [dbo].[]      ******/
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
-- Description : 
-- ===================================================================

CREATE OR ALTER PROCEDURE [dbo].[sp_AddressesUpdateFirstRowIsEmpty_upd]
(
 @Code nvarchar(50),
 @FirstRowIsEmpty bit,
 @Direction nvarchar(50)
)
AS
BEGIN
  SET NOCOUNT ON;
 
	UPDATE Addresses 
    SET LastLoadTime = IIF(@FirstRowIsEmpty = 0,GETDATE(),null ), 
        FirstRowIsEmpty =  @FirstRowIsEmpty 
    WHERE Code = @Code 
      AND Direction = @Direction 


END
GO









/****** Object:  StoredProcedure [dbo].[]      ******/
USE [WarehouseSimulation];
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- ===================================================================
-- Author      : scelik
-- Create date : 9/22/2020
-- Revised date: 
-- Description : Shuttle ihtiyacı olan adresleri order ile getirir
-- ===================================================================

CREATE OR ALTER PROCEDURE [dbo].[sp_GetNeedShuttleAddresses_sel]
AS
BEGIN
  SET NOCOUNT ON;
 
     
        SELECT * 
        FROM Addresses 
        WHERE LineNumber <> 0 -- sabit adresleri ayırır
          AND BlockReason IS NULL
          AND Code NOT IN (select TargetAddress from MachineTasks where TaskType = 10 and Sequence = 2 and (SentFlag = 0 or IsCompleted = 0 ))
          AND Code NOT IN (SELECT LastAddress 
					        FROM Shuttles 
					        WHERE Assignment = 'IO' 
					          AND IsActive = 1
					          AND IsDeleted = 0)
          AND (
		        (FirstRowIsEmpty = 0 AND Direction = 'WH_IN')
		        OR 
		        ( DependedProductId IN (SELECT ProductId 
								        FROM OrderDetailPallets
								        WHERE IsTaken = 0) AND Direction = 'WH_OUT')
              )
        ORDER BY Case When FirstRowIsEmpty = 0 Then 0 Else 1 End, 
		         Case When DependedProductId IN (SELECT ProductId FROM OrderDetailPallets WHERE IsTaken = 0) Then 0 Else 1 End


END
GO




/****** Object:  StoredProcedure [dbo].[]      ******/
USE [WarehouseSimulation];
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- ===================================================================
-- Author      : scelik
-- Create date : 9/28/2020
-- Revised date: 
-- Description : şarj adresi seç
-- ===================================================================

CREATE OR ALTER PROCEDURE [dbo].[sp_GetAvailableChargeAddress_sel]
AS
BEGIN
  SET NOCOUNT ON;
 
     
     select top 1 Code from Addresses (NOLOCK) where Code like 'CHRG%' and FirstRowIsEmpty = 1


END
GO