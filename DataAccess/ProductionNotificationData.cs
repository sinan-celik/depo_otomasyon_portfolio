using BAT_Class_Library;
using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace DataAccess
{
    public class ProductionNotificationData
    {

        private readonly IConfiguration _configuration;
        private string conStr;
        public ProductionNotificationData()
        {
            _configuration = BaseDataAccess.configuration;
            conStr = _configuration.GetConnectionString("WHSimulation");

        }



        public long InsertProductionNotification(ProductionNotification productionNotification)
        {
            long identity;
            using (var conn = new SqlConnection(conStr))
            {
                conn.Open();
                identity = conn.Insert(productionNotification);
                conn.Close();

            }
            return identity;
        }


        public IEnumerable<ProductionNotification> GetProductionNotifications()
        {
            IEnumerable<ProductionNotification> addresses;
            using (var conn = new SqlConnection(conStr))
            {
                conn.Open();
                addresses = conn.Query<ProductionNotification>("select * from ProductionNotifications pn(NOLOCK)", commandType: System.Data.CommandType.Text);
                conn.Close();

            }
            return addresses;
        }

        public int SelectWaitingPaletteCount(DateTime start)
        {
            int count;
            using (var conn = new SqlConnection(conStr))
            {
                conn.Open();
                count = conn.QuerySingleOrDefault<int>("sp_SelectWaitingPaletteCount", new { StartTime = start }, commandType: System.Data.CommandType.StoredProcedure);
                conn.Close();

            }

            return count;
        }

        public ProductionNotification GetProductionNotificationById(int productNotificationId)
        {
            ProductionNotification productionNotification;
            using (var conn = new SqlConnection(conStr))
            {
                conn.Open();
                productionNotification = conn.QueryFirstOrDefault<ProductionNotification>("SELECT * FROM ProductionNotifications pn(NOLOCK) WHERE Id = @Id", new { Id = productNotificationId }, commandType: System.Data.CommandType.Text);
                conn.Close();

            }
            return productionNotification;
        }
    }
}
