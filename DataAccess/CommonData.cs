using BAT_Class_Library;
using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace DataAccess
{
    public class CommonData
    {
        private readonly IConfiguration _configuration;
        private string conStr;
        public CommonData()
        {

            _configuration = BaseDataAccess.configuration;
            conStr = _configuration.GetConnectionString("WHSimulation");
        }

        public List<MachinesDTO> GetAllMachinesIpAddress()
        {
            List<MachinesDTO> list;
            using (var conn = new SqlConnection(conStr))
            {
                conn.Open();
                list = conn.Query<MachinesDTO>("sp_GetAllMachinesIpAddress_sel",/*new { },*/ commandType: System.Data.CommandType.StoredProcedure).ToList();
                conn.Close();

            }
            return list;
        }

    }
}
