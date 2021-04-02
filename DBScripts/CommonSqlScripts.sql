

USE [WarehouseSimulation];
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- ===================================================================
-- Author      : scelik
-- Create date : 9/2/2020
-- Revised date: 
-- Description : Common Selects
-- ===================================================================

CREATE OR ALTER PROCEDURE [dbo].[sp_GetAllMachinesIpAddress_sel]
AS
BEGIN

SELECT 'ASRS' as [Type]
	  ,[Code]
      ,[IpAddress]
      --,[Port]
      --,[LastAddress]
      ,[Location]
      --,[Status]
      --,[CreateDate]
      --,[CreateUser]
      --,[LastUpdateDate]
      --,[LastUptadeUser]
      --,[IsDeleted]
  FROM [WarehouseSimulation].[dbo].[Asrs]
UNION ALL
SELECT 'SHUTTLE' as [Type]
	  ,[Code]
      ,[IpAddress]
      ,'' as [Location] 
      --,[Port]
      --,[LastAddress]
      --,[Status]
      --,[IsActive]
      --,[CreateDate]
      --,[CreateUser]
      --,[LastUpdateDate]
      --,[LastUptadeUser]
      --,[IsDeleted]
  FROM [WarehouseSimulation].[dbo].[Shuttles]
UNION ALL
SELECT 'CONVEYOR' as [Type]
	  ,[Code]
      ,[IpAddress]
      --,[Port]
      ,[Location]
      --,[Status]
      --,[CreateDate]
      --,[CreateUser]
      --,[LastUpdateDate]
      --,[LastUptadeUser]
      --,[IsDeleted]
  FROM [WarehouseSimulation].[dbo].[Conveyors]



END
GO