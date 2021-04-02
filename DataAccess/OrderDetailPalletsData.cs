using BAT_Class_Library;
using Dapper;
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
    public class OrderDetailPalletsData
    {
        private static readonly ILog logToDB = LogManager.GetLogger(LoggerManager.GetRepository(Assembly.GetExecutingAssembly()).Name, "AdoNetLogAppender");

        //private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IConfiguration _configuration;
        private string conStr;
        public OrderDetailPalletsData()
        {
            _configuration = BaseDataAccess.configuration;
            conStr = _configuration.GetConnectionString("WHSimulation");
        }

        public OrderDetailPallet GetOrderDetailPalletById(int id)
        {
            OrderDetailPallet orderDetailPallet;
            using (var conn = new SqlConnection(conStr))
            {
                conn.Open();
                orderDetailPallet = conn.QueryFirstOrDefault<OrderDetailPallet>("SELECT * FROM OrderDetailPallets odp(NOLOCK) WHERE Id = @Id", new { Id = id }, commandType: System.Data.CommandType.Text);
                conn.Close();

            }
            return orderDetailPallet;
        }


        public long InsertOrderDetailPallet(OrderDetailPallet orderDetailPallet)
        {
            long identity;
            using (var conn = new SqlConnection(conStr))
            {
                conn.Open();
                identity = conn.Insert(orderDetailPallet);
                conn.Close();

            }
            return identity;
        }



        public void UpdateOrderDetailPallet(OrderDetailPallet orderDetailPallet)
        {
            using (var conn = new SqlConnection(conStr))
            {
                conn.Open();
                conn.Update<OrderDetailPallet>(orderDetailPallet);
                conn.Close();

            }
        }

        public void UpdateOrderDetailPalletsWithTaken(int orderDetailPalletId)
        {
            var odp = GetOrderDetailPalletById(orderDetailPalletId);
            odp.TakenDate = DateTime.Now;
            odp.IsTaken = true;

            UpdateOrderDetailPallet(odp);

        }

        public int RemainCountByOrderDetailId(int orderDetailId)
        {
            int count;
            using (var conn = new SqlConnection(conStr))
            {
                conn.Open();
                count = conn.QueryFirstOrDefault<int>("sp_RemainCountByOrderDetailId_sel", new { OrderDetailId = orderDetailId }, commandType: System.Data.CommandType.StoredProcedure);
                conn.Close();

            }
            return count;
        }
    }
}
