/****** Object:  StoredProcedure [dbo].[sp_MachineTasks_ups]    Script Date: Aug 14 2020 11:43AM  ******/
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

CREATE OR ALTER PROCEDURE [dbo].[sp_MachineTasks_ups]
  (@ID int=NULL,@TaskBatch int=NULL,@MachineCode nvarchar(50)=NULL,@SourceType nvarchar(50)=NULL,@SourceAddress nvarchar(50)=NULL,@LoadInfo nvarchar(50)=NULL,@TargetType nvarchar(50)=NULL,@TargetAddress nvarchar(50)=NULL,@AssignUser nvarchar(50)=NULL,@AssignReason nvarchar(50)=NULL,@AssignTime datetime=NULL,@StartTime datetime=NULL,@EndTime datetime=NULL,@IsCompleted bit=NULL,@ErrorCode nvarchar(50)=NULL)
AS
BEGIN
  IF @ID IS NULL OR @ID = 0

    BEGIN
      INSERT INTO [dbo].[MachineTasks]
        ([TaskBatch],[MachineCode],[SourceType],[SourceAddress],[LoadInfo],[TargetType],[TargetAddress],[AssignUser],[AssignReason],[AssignTime],[StartTime],[EndTime],[IsCompleted],[ErrorCode])
      VALUES
        (@TaskBatch,@MachineCode,@SourceType,@SourceAddress,@LoadInfo,@TargetType,@TargetAddress,@AssignUser,@AssignReason,@AssignTime,@StartTime,@EndTime,@IsCompleted,@ErrorCode);
      SELECT * FROM [dbo].[MachineTasks] WHERE [ID] = SCOPE_IDENTITY();
    END
  ELSE
    BEGIN
      UPDATE [dbo].[MachineTasks]
        SET [TaskBatch]=@TaskBatch,[MachineCode]=@MachineCode,[SourceType]=@SourceType,[SourceAddress]=@SourceAddress,[LoadInfo]=@LoadInfo,[TargetType]=@TargetType,[TargetAddress]=@TargetAddress,[AssignUser]=@AssignUser,[AssignReason]=@AssignReason,[AssignTime]=@AssignTime,[StartTime]=@StartTime,[EndTime]=@EndTime,[IsCompleted]=@IsCompleted,[ErrorCode]=@ErrorCode
        WHERE ([Id] = @ID);
      SELECT * FROM [dbo].[MachineTasks] WHERE [ID] = @ID;
    END
END
GO

/****** Object:  StoredProcedure [dbo].[sp_MachineTasks_sel]    Script Date: Aug 14 2020 11:43AM  ******/
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

CREATE OR ALTER PROCEDURE [dbo].[sp_MachineTasks_sel]
  (@ID int=NULL)
AS
BEGIN
  IF @ID IS NULL OR @ID = 0
    SELECT * FROM [dbo].[MachineTasks] ORDER BY [Id] DESC;
  ELSE
    SELECT * FROM [dbo].[MachineTasks] WHERE [ID] = @ID;
END
GO

/****** Object:  StoredProcedure [dbo].[sp_MachineTasks_del]    Script Date: Aug 14 2020 11:43AM  ******/
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

CREATE OR ALTER PROCEDURE [dbo].[sp_MachineTasks_del]
  (@ID int)
AS
BEGIN
  SET NOCOUNT ON;
  DELETE FROM [dbo].[MachineTasks] WHERE [Id]=@ID;
  SELECT @@ROWCOUNT as [Rows Affected];
END
GO

USE [WarehouseSimulation];
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- ===================================================================
-- Author      : scelik
-- Create date : 08/19/2020
-- Revised date: 
-- Description : Tamamlanmamış Taskları döndürür 
-- ===================================================================

CREATE OR ALTER PROCEDURE [dbo].[sp_SelectMachineTasksByIsCompleted_sel]
AS
BEGIN
    set nocount on;
    SELECT * FROM [dbo].[MachineTasks] WHERE IsCompleted = 0;
    set nocount off;
END
GO


USE [WarehouseSimulation];
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- ===================================================================
-- Author      : scelik
-- Create date : 09/15/2020
-- Revised date: 
-- Description : assign task işleri 
-- ===================================================================

CREATE OR ALTER PROCEDURE [dbo].[sp_GetTaskDataForEntrySide_sel]
AS
BEGIN
    set nocount on;
    

        
        --assign task for entrance
        SELECT TOP 1
               mt.[Id] taskId
	          ,mt.[TaskType] taskType
	          ,IIF(mt.TargetType = 'ADDRESS', a.X, b.X ) as X
	          ,IIF(mt.TargetType = 'ADDRESS', a.Z1, b.Z1 ) as Z1
	          ,IIF(mt.TargetType = 'ADDRESS', a.Z2, b.Z2 ) as Z2
	          ,IIF(mt.TargetType = 'ADDRESS', a.G, b.G ) as G
          FROM [MachineTasks] mt(NOLOCK)
          LEFT OUTER JOIN Addresses a(NOLOCK) ON a.Code = mt.TargetAddress AND a.Direction = 'WH_IN'
          LEFT OUTER JOIN Buffers b(NOLOCK) ON b.Code = mt.TargetAddress
          WHERE SentFlag = 0 AND IsCompleted = 0
          AND MachineCode = 'ASRS_WH_IN'
          ORDER BY AssignTime 



    
END
GO






USE [WarehouseSimulation];
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- ===================================================================
-- Author      : scelik
-- Create date : 09/15/2020
-- Revised date: 
-- Description : assign task işleri 
-- ===================================================================

CREATE OR ALTER PROCEDURE [dbo].[sp_GetTaskDataForExitSide_sel]
AS
BEGIN
    set nocount on;
    
        --assign task for exit
        SELECT TOP 1
               mt.[Id] taskId
	          ,mt.[TaskType] taskType
	          ,a.X
	          ,a.Z1
	          ,a.Z2
	          ,a.G
          FROM [MachineTasks] mt(NOLOCK)
          LEFT OUTER JOIN Addresses a(NOLOCK) ON a.Code = mt.SourceAddress AND a.Direction = 'WH_OUT'
          WHERE SentFlag = 0 AND IsCompleted = 0
          AND MachineCode = 'ASRS_WH_OUT'
          ORDER BY AssignTime 

END
GO








USE [WarehouseSimulation];
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- ===================================================================
-- Author      : scelik
-- Create date : 09/21/2020
-- Revised date: 
-- Description : assign task shuttle işleri 
-- ===================================================================

CREATE OR ALTER PROCEDURE [dbo].[sp_GetTaskDataForShuttle_sel]
(
    @Code nvarchar(50)
)
AS
BEGIN
    set nocount on;
        --assign task for shuttle
     
	SELECT TOP 1
               mt.[Id] taskId
	          ,mt.[TaskType] taskType
	          ,a.X as X
	          ,a.Z1 as Z1
	          ,a.Z2 as Z2
	          ,a.G as G
          FROM [MachineTasks] mt(NOLOCK)
		  LEFT OUTER JOIN Addresses a(NOLOCK) ON a.Code = mt.SourceAddress AND a.Direction = 'WH_OUT'
          WHERE SentFlag = 0 
			AND IsCompleted = 0
			AND ((TaskType = 10 AND [Sequence] = 1)--ShATA
			  OR TaskType = 11 --ShCHRG
			  OR (TaskType = 12 AND TargetAddress IN (SELECT s.LastAddress FROM Shuttles s(NOLOCK) WHERE s.[Status] = 'READY' AND s.Assignment = 'OPTIMIZATION' and s.Code = @Code ) )--ShOPT
			  OR (TaskType = 13 AND TargetAddress IN (SELECT s.LastAddress FROM Shuttles s(NOLOCK) WHERE s.[Status] = 'READY' AND s.Assignment = 'IO' and s.Code = @Code ) )--ShINSERT
			  OR (TaskType = 14 AND SourceAddress IN (SELECT s.LastAddress FROM Shuttles s(NOLOCK) WHERE s.[Status] = 'READY' AND s.Assignment = 'IO' and s.Code = @Code ) )--ShTAKEOUT
				)
		ORDER BY TaskType, AssignTime 


    
END
GO








USE [WarehouseSimulation];
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- ===================================================================
-- Author      : scelik
-- Create date : 09/21/2020
-- Revised date: 
-- Description : assign task shuttle işleri 
-- ===================================================================

CREATE OR ALTER PROCEDURE [dbo].[sp_UpdateMachineTaskWithMachineCodeById_upd]
(
    @Id int,
    @Code nvarchar(50)
)
AS
BEGIN
    set nocount on;
        --assign task for shuttle
     
    UPDATE MachineTasks SET MachineCode = @Code WHERE Id = @Id

    
END
GO
