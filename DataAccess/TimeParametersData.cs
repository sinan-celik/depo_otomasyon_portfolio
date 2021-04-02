using BAT_Class_Library;
using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace DataAccess
{
    public class TimeParametersData
    {
        private readonly IConfiguration _configuration;
        private string conStr;
        public TimeParametersData()
        {
            _configuration =BaseDataAccess.configuration;
            conStr = _configuration.GetConnectionString("WHSimulation");

        }


        public IEnumerable<TimeParameter> GetTimeParameters()
        {
            IEnumerable<TimeParameter> param;
            using (var conn = new SqlConnection(conStr))
            {
                conn.Open();
                param = conn.Query<TimeParameter>("sp_TimeParameters_sel", commandType: System.Data.CommandType.StoredProcedure);
                conn.Close();

            }
            return param;
        }

    }
}
