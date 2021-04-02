using BAT_Class_Library;
using Dapper;
using Dapper.Contrib;
using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace DataAccess
{
    public class AsrsData
    {
        private readonly IConfiguration _configuration;
        private string conStr;
        public AsrsData()
        {

            _configuration =BaseDataAccess.configuration;
            conStr = _configuration.GetConnectionString("WHSimulation");
        }

        public IEnumerable<Asrs> GetAllAsrs()
        {
            IEnumerable<Asrs> list;
            using (var conn = new SqlConnection(conStr))
            {
                conn.Open();
                list = conn.GetAll<Asrs>();
                conn.Close();

            }
            return list;
        }

        public void UpdateLastAddress(Asrs asrs)
        {
            using (var conn = new SqlConnection(conStr))
            {
                conn.Open();
                conn.Update<Asrs>(asrs);
                conn.Close();

            }
        }
    }
}
