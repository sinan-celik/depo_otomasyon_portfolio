using BAT_Class_Library;
using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace DataAccess
{
    public class PlcCommunicationReadedDataAcces
    {
        private readonly IConfiguration _configuration;
        private string conStr;

        public PlcCommunicationReadedDataAcces()
        {
            _configuration = BaseDataAccess.configuration;
            conStr = _configuration.GetConnectionString("WHSimulation");
        }


        public long InsertReadedData(PlcCommunicationReadedData rd)
        {
            long identity;
            using (var conn = new SqlConnection(conStr))
            {
                conn.Open();
                identity = conn.Insert(rd);
                conn.Close();

            }
            return identity;
        }
    }
}
