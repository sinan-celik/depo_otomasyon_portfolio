using BAT_Class_Library;
using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace DataAccess
{
    public class ShuttlesData
    {
        private readonly IConfiguration _configuration;
        private string conStr;
        public ShuttlesData()
        {

            _configuration = BaseDataAccess.configuration;
            conStr = _configuration.GetConnectionString("WHSimulation");
        }

        public IEnumerable<Shuttle> GetAllShuttles()
        {
            IEnumerable<Shuttle> list;
            using (var conn = new SqlConnection(conStr))
            {
                conn.Open();
                list = conn.GetAll<Shuttle>();
                conn.Close();

            }
            return list;
        }

        public Shuttle GetMoveableShuttleByAddress(string code)
        {
            Shuttle shuttle;


            //TODO: sp ye çevir, adrese göre ayarla
            using (var conn = new SqlConnection(conStr))
            {
                conn.Open();
                shuttle = conn.QuerySingleOrDefault<Shuttle>(
                    @"  SELECT TOP 1 s.* 
                        FROM Shuttles s(NOLOCK)
                        LEFT OUTER JOIN Addresses a(NOLOCK) ON s.LastAddress = a.Code
                        WHERE a.Direction = 'WH_OUT'
                          AND s.IsActive = 1
                          and s.Status = 'WAITING'
                          and s.Id NOT IN (SELECT ShuttleId FROM AddressDedicatedShuttles)",
                    new { Code = code }, //code parametresi mesafeye göre seçim için eklenecek
                    commandType: System.Data.CommandType.Text);
                conn.Close();

            }

            return shuttle;
        }

        public Shuttle GetShuttleById(int id)
        {
            Shuttle shuttle;
            using (var conn = new SqlConnection(conStr))
            {
                conn.Open();
                shuttle = conn.QueryFirstOrDefault<Shuttle>("SELECT * FROM Shuttles s(NOLOCK) WHERE Id = @Id", new { Id = id }, commandType: System.Data.CommandType.Text);
                conn.Close();

            }
            return shuttle;
        }
        public Shuttle GetShuttleByCode(string code)
        {
            Shuttle shuttle;
            using (var conn = new SqlConnection(conStr))
            {
                conn.Open();
                shuttle = conn.QueryFirstOrDefault<Shuttle>("SELECT * FROM Shuttles s(NOLOCK) WHERE Code = @Code", new { Code = code }, commandType: System.Data.CommandType.Text);
                conn.Close();

            }
            return shuttle;
        }

        public void UpdateShuttle(Shuttle shuttle)
        {
            using (var conn = new SqlConnection(conStr))
            {
                conn.Open();
                conn.Update<Shuttle>(shuttle);
                conn.Close();

            }
        }

        public void UpdateShuttleLastAddressAndStatus(MachineTask completedMachineTask)
        {
            var shuttle = GetShuttleByCode(completedMachineTask.LoadInfo); //taşınan mekik code u burda yazmalı
            shuttle.LastUpdateDate = DateTime.Now;
            shuttle.LastUptadeUser = "SYSTEM";
            shuttle.LastAddress = completedMachineTask.TargetAddress;
            shuttle.Status = SysStatus.READY.ToString(); //TODO:kontrollü yazılmalı

            //shuttle.Assignment = DetermineAssigment();
            //shuttle.Assignment = completedMachineTask.TaskType == (int)TaskType.ShCHRG ? ShuttleAssign.CHARGE :
            //    completedMachineTask.TaskType == (int)TaskType.ShATA ? ShuttleAssign.IO : ShuttleAssign.OPTIMIZATION; //TODO:kontrollü yazılmalı


            UpdateShuttle(shuttle);
        }

        public Shuttle SelectNewOptimizationShuttle()
        {
            Shuttle shuttle;
            using (var conn = new SqlConnection(conStr))
            {
                conn.Open();
                shuttle = conn.QueryFirstOrDefault<Shuttle>("sp_SelectNewOptimizationShuttle_upd", commandType: System.Data.CommandType.StoredProcedure);
                conn.Close();

            }
            return shuttle;
        }
    }
}
