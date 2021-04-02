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
    public class OrderDetailsData
    {
        private static readonly ILog logToDB = LogManager.GetLogger(LoggerManager.GetRepository(Assembly.GetExecutingAssembly()).Name, "AdoNetLogAppender");

        //private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IConfiguration _configuration;
        private string conStr;
        public OrderDetailsData()
        {
            _configuration = BaseDataAccess.configuration;
            conStr = _configuration.GetConnectionString("WHSimulation");
        }

        public OrderDetail GetOrderDetailById(int id)
        {
            OrderDetail orderDetail;
            using (var conn = new SqlConnection(conStr))
            {
                conn.Open();
                orderDetail = conn.QueryFirstOrDefault<OrderDetail>("SELECT * FROM OrderDetails od(NOLOCK) WHERE Id = @Id", new { Id = id }, commandType: System.Data.CommandType.Text);
                conn.Close();

            }
            return orderDetail;
        }

        public void UpdateOrderDetailsWithTaken(int orderDetailId)
        {
            var od = GetOrderDetailById(orderDetailId);
            od.LastUpdateDate = DateTime.Now;
            od.AllPalletsTaken = true;

            UpdateOrderDetail(od);

        }

        public void UpdateOrderDetail(OrderDetail orderDetail)
        {
            using (var conn = new SqlConnection(conStr))
            {
                conn.Open();
                conn.Update<OrderDetail>(orderDetail);
                conn.Close();

            }
        }

        public List<OrderDetail> GetOrderDetailsByOrderId(int id)
        {
            List<OrderDetail> orderDetailList;
            using (var conn = new SqlConnection(conStr))
            {
                conn.Open();
                orderDetailList = conn.Query<OrderDetail>("SELECT * FROM OrderDetails od(NOLOCK) WHERE OrderId = @OrderId", new { OrderId = id }, commandType: System.Data.CommandType.Text).ToList();
                conn.Close();

            }
            return orderDetailList;
        }
    }
}
