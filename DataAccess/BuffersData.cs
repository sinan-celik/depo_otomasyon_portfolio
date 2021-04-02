using BAT_Class_Library;
using Dapper;
using Dapper.Contrib.Extensions;
using log4net;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;
using Buffer = BAT_Class_Library.Buffer;

namespace DataAccess
{
    public class BuffersData
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IConfiguration _configuration;
        private string conStr;
        public BuffersData()
        {
            _configuration = BaseDataAccess.configuration;
            conStr = _configuration.GetConnectionString("WHSimulation");
        }

        public Buffer GetBufferByCode(string code)
        {
            Buffer buffer;
            using (var conn = new SqlConnection(conStr))
            {
                conn.Open();
                buffer = conn.QuerySingleOrDefault<Buffer>("SELECT * FROM Buffers b(NOLOCK) WHERE Code = @Code", new { Code = code }, commandType: System.Data.CommandType.Text);
                conn.Close();

            }
            return buffer;
        }

        public BAT_Class_Library.Buffer SelectAppropiriateBuffer()
        {
            BAT_Class_Library.Buffer bfr;
            using (var conn = new SqlConnection(conStr))
            {
                conn.Open();
                bfr = conn.QuerySingleOrDefault<BAT_Class_Library.Buffer>(
                    @"sp_SelectAppropiriateBuffer_sel",
                    //new { Code = sourceAddress },
                    commandType: System.Data.CommandType.StoredProcedure);
                conn.Close();

            }
            return bfr;
        }

        public void ChangeBufferLoadInfo(string targetAddress, bool v, string loadInfo)
        {
            using var conn = new SqlConnection(conStr);
            conn.Open();
            conn.Execute(
                "sp_UpdateBufferLoadInfo_upd",
                new
                {
                    Code = targetAddress,
                    IsEmpty = v,
                    LastPaletteInfo = loadInfo
                },
                commandType: System.Data.CommandType.StoredProcedure);
            conn.Close();
        }


        public BAT_Class_Library.Buffer GetPaletteInfoByProductId(int productID)
        {
            BAT_Class_Library.Buffer buffer;

            using (var conn = new SqlConnection(conStr))
            {
                conn.Open();
                buffer = conn.QuerySingleOrDefault<BAT_Class_Library.Buffer>(
                    @"SELECT * 
	                    FROM Buffers a(NOLOCK)
	                    WHERE LastPaletteInfo = @ProductId;",
                    new { ProductId = productID },
                    commandType: System.Data.CommandType.Text);
                conn.Close();

            }

            return buffer;
        }

        public bool UpdateBufferWithPalette(MachineTask completedMachineTask)
        {
            log.Debug($"UpdateBufferWithPalette, completedMachineTask.TargetAddress: {completedMachineTask.TargetAddress}");

            Buffer buffer = GetBufferByCode(completedMachineTask.TargetAddress); //load
            buffer.IsEmpty = false;
            buffer.LastPaletteInfo = completedMachineTask.LoadInfo;

            bool done;
            using (var conn = new SqlConnection(conStr))
            {
                conn.Open();
                done = conn.Update(buffer);
                conn.Close();

            }
            return done;
        }

        public bool UpdateBufferWithOutPalette(MachineTask completedMachineTask)
        {
            log.Debug($"UpdateBufferWithOutPalette, completedMachineTask.SourceAddress: {completedMachineTask.SourceAddress}");

            Buffer buffer = GetBufferByCode(completedMachineTask.SourceAddress);//unload
            buffer.IsEmpty = true;
            buffer.LastPaletteInfo = null;

            bool done;
            using (var conn = new SqlConnection(conStr))
            {
                conn.Open();
                done = conn.Update(buffer);
                conn.Close();

            }
            return done;
        }
    }
}
