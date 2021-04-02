using BAT_Class_Library;
using BAT_Class_Library.DbEquivalent;
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
    public class PlacesData
    {
        private static readonly ILog logToDB = LogManager.GetLogger(LoggerManager.GetRepository(Assembly.GetExecutingAssembly()).Name, "AdoNetLogAppender");

        //private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IConfiguration _configuration;
        private string conStr;
        public PlacesData()
        {
            _configuration = BaseDataAccess.configuration;
            conStr = _configuration.GetConnectionString("WHSimulation");
        }


        public List<Place> GetAllPlaces()
        {
            List<Place> list;
            using (var conn = new SqlConnection(conStr))
            {
                conn.Open();
                list = conn.GetAll<Place>().ToList();
                conn.Close();

            }

            return list;
        }


    }
}
