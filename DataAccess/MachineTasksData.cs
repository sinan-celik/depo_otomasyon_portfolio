using BAT_Class_Library;
using Dapper;
using Dapper.Contrib.Extensions;
using log4net;
using log4net.Core;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DataAccess
{
    public class MachineTasksData
    {

        private static readonly ILog logToDB = LogManager.GetLogger(LoggerManager.GetRepository(Assembly.GetExecutingAssembly()).Name, "AdoNetLogAppender");

        //private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IConfiguration _configuration;
        private string conStr;
        public MachineTasksData()
        {
            _configuration = BaseDataAccess.configuration;
            conStr = _configuration.GetConnectionString("WHSimulation");
        }

        public void InsertMachineTaskBatch(List<MachineTask> taskBatchList)
        {

            using (var conn = new SqlConnection(conStr))
            {
                conn.Open();
                var x = conn.Insert(taskBatchList);
                conn.Close();
                //logToDB.Info(x);
            }
            //return identity;
        }

        public long InsertMachineTask(MachineTask task)
        {
            long identity;
            using (var conn = new SqlConnection(conStr))
            {
                conn.Open();
                identity = conn.Insert(task);
                conn.Close();

            }
            return identity;
        }

        public List<MachineTask> GetWaitingTasks()
        {
            List<MachineTask> list;
            using (var conn = new SqlConnection(conStr))
            {
                conn.Open();
                list = conn.Query<MachineTask>("sp_SelectMachineTasksByIsCompleted_sel", commandType: System.Data.CommandType.StoredProcedure).ToList();
                conn.Close();

            }
            return list;
        }

        public void UpdateTask(MachineTask task)
        {
            using (var conn = new SqlConnection(conStr))
            {
                conn.Open();
                conn.Update<MachineTask>(task);
                conn.Close();

            }
        }


        public MachineTask GetMachineTaskById(int id)
        {
            MachineTask machineTask;
            using (var conn = new SqlConnection(conStr))
            {
                conn.Open();
                machineTask = conn.QueryFirstOrDefault<MachineTask>("SELECT * FROM MachineTasks mt(NOLOCK) WHERE Id = @Id", new { Id = id }, commandType: System.Data.CommandType.Text);
                conn.Close();

            }
            return machineTask;
        }

        public void CreateTaskForFirstRowPalette(MachineTask completedMachineTask)
        {
            MachineTask machineTask = new MachineTask
            {
                OrderDetailPalletId = 0,//boş kalamalı
                ProductNotificationId = completedMachineTask.ProductNotificationId,
                TaskType = (int)TaskType.ShINSERT,
                TaskBatch = 0,
                Sequence = 1,
                MachineCode = null,
                SourceType = AddressType.ADDRESS,
                SourceAddress = completedMachineTask.TargetAddress,
                LoadInfo = completedMachineTask.LoadInfo,
                TargetType = AddressType.ADDRESS,
                TargetAddress = completedMachineTask.TargetAddress,//aynı tüpteki içeri kaydırma işlemi
                AssignUser = "SYSTEM",
                AssignReason = "STANDART",
                AssignTime = DateTime.Now,
                StartTime = null,
                EndTime = null,
                SentFlag = false,
                IsCompleted = false,
                ErrorCode = null,

            };

            InsertMachineTask(machineTask);
        }

        public void CreateTaskForBufferPalette(MachineTask completedMachineTask)
        {
            MachineTask machineTask = new MachineTask
            {
                OrderDetailPalletId = 0,//boş kalamalı
                ProductNotificationId = completedMachineTask.ProductNotificationId,
                TaskType = (int)TaskType.BTA,
                TaskBatch = 0,
                Sequence = 1,
                MachineCode = null,
                SourceType = AddressType.BUFFER,
                SourceAddress = completedMachineTask.TargetAddress,
                LoadInfo = completedMachineTask.LoadInfo,
                TargetType = AddressType.ADDRESS,
                TargetAddress = "",//TODO:adres seç
                AssignUser = "SYSTEM",
                AssignReason = "STANDART",
                AssignTime = DateTime.Now,
                StartTime = null,
                EndTime = null,
                SentFlag = false,
                IsCompleted = false,
                ErrorCode = null,

            };

            InsertMachineTask(machineTask);
        }

        public TaskDTO GetTaskDataForShuttle(MachinesDTO communicationMachine)
        {
            TaskDTO obj;
            using (var conn = new SqlConnection(conStr))
            {
                conn.Open();
                //TODO: task seçimi optimizasyon için sequence ekle
                obj = conn.QuerySingleOrDefault<TaskDTO>("sp_GetTaskDataForShuttle_sel", new { Code = communicationMachine.Code }, commandType: System.Data.CommandType.StoredProcedure);
                conn.Close();

            }
            return obj;
        }

        public bool UpdateMachineTaskWithPlcSentById(int id)
        {
            var machineTask = GetMachineTaskById(id);
            machineTask.SentFlag = true;
            machineTask.StartTime = DateTime.Now;
            bool done;
            using (var conn = new SqlConnection(conStr))
            {
                conn.Open();
                done = conn.Update<MachineTask>(machineTask);
                conn.Close();

            }

            return done;
        }

        public bool UpdateMachineTaskWithPlcCompletedById(int id)
        {
            var machineTask = GetMachineTaskById(id);
            machineTask.IsCompleted = true;
            machineTask.EndTime = DateTime.Now;
            bool done;
            using (var conn = new SqlConnection(conStr))
            {
                conn.Open();
                done = conn.Update<MachineTask>(machineTask);
                conn.Close();

            }

            return done;
        }

        public int UpdateMachineTaskWithMachineCodeById(int taskId, string code)
        {
            int rowsEffected;
            using (var conn = new SqlConnection(conStr))
            {
                conn.Open();
                rowsEffected = conn.Execute("sp_UpdateMachineTaskWithMachineCodeById_upd", new { Id = taskId, Code = code }, commandType: System.Data.CommandType.StoredProcedure);
                conn.Close();

            }
            return rowsEffected;
        }

        public TaskDTO GetTaskDataForEntrySide()
        {
            TaskDTO obj;
            using (var conn = new SqlConnection(conStr))
            {
                conn.Open();
                obj = conn.QuerySingleOrDefault<TaskDTO>("sp_GetTaskDataForEntrySide_sel", commandType: System.Data.CommandType.StoredProcedure);
                conn.Close();

            }
            return obj;
        }

        public TaskDTO GetTaskDataForExitSide()
        {
            TaskDTO obj;
            using (var conn = new SqlConnection(conStr))
            {
                conn.Open();
                obj = conn.QuerySingleOrDefault<TaskDTO>("sp_GetTaskDataForExitSide_sel", commandType: System.Data.CommandType.StoredProcedure);
                conn.Close();

            }
            return obj;
        }


        public int GetNewTaskBatch()
        {
            //Oluşturulan yeni çoklu tasklara TaskBatch ataması yapmak için yazıldı. takip eden işleri görmeyi kolaylaştırmayı amaçlar.
            int newTaskBatch;
            using (var conn = new SqlConnection(conStr))
            {
                conn.Open();
                newTaskBatch = conn.QuerySingleOrDefault<int>("SELECT ISNULL(MAX(TaskBatch), 0) + 1 FROM MachineTasks (NOLOCK)", commandType: System.Data.CommandType.Text);
                conn.Close();

            }
            return newTaskBatch;
        }


    }
}
