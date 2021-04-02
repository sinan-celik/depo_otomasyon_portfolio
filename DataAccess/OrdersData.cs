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
    public class OrdersData
    {
        private static readonly ILog logToDB = LogManager.GetLogger(LoggerManager.GetRepository(Assembly.GetExecutingAssembly()).Name, "AdoNetLogAppender");

        //private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IConfiguration _configuration;
        private string conStr;

        public OrdersData()
        {
            _configuration = BaseDataAccess.configuration;
            conStr = _configuration.GetConnectionString("WHSimulation");
        }

        public long InsertOrder(Order order)
        {
            long identity;
            using (var conn = new SqlConnection(conStr))
            {
                conn.Open();
                identity = conn.Insert(order);
                conn.Close();

            }
            return identity;
        }

        public List<Order> GetAllOrders()
        {
            List<Order> list;
            using (var conn = new SqlConnection(conStr))
            {
                conn.Open();
                list = conn.GetAll<Order>().ToList();
                conn.Close();

            }


            var newList = list.OrderByDescending(x => x.Id).ToList();
            
            return newList;
        }


    }
}
