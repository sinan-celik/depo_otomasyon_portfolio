using BAT_Class_Library;
using Dapper.Contrib.Extensions;
using log4net;
using log4net.Core;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;

namespace DataAccess
{
    public class SystemParametersData
    {
        private static readonly ILog logToDB = LogManager.GetLogger(LoggerManager.GetRepository(Assembly.GetExecutingAssembly()).Name, "AdoNetLogAppender");

        //private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IConfiguration _configuration;
        private string conStr;
        public SystemParametersData()
        {
            _configuration = BaseDataAccess.configuration;
            conStr = _configuration.GetConnectionString("WHSimulation");
        }


        //optimizasyonu yazarken bu satırları global değişken olarak yazmak için global class ve propertyleri set et
        public IEnumerable<SystemParameter> GetAllSystemParameters()
        {
            IEnumerable<SystemParameter> list;
            using (var conn = new SqlConnection(conStr))
            {
                conn.Open();
                list = conn.GetAll<SystemParameter>();
                conn.Close();

            }





            return list;
        }

    }
}
