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
    public class ProductsData
    {
        private readonly IConfiguration _configuration;
        private string conStr;
        public ProductsData()
        {

            _configuration = BaseDataAccess.configuration;
            conStr = _configuration.GetConnectionString("WHSimulation");
        }




        public Product GetProductById(int id)
        {
            Product p;
            using (var conn = new SqlConnection(conStr))
            {
                conn.Open();
                p = conn.QuerySingleOrDefault<Product>("SELECT * FROM Products p(NOLOCK) WHERE Id = @Id; ",
                    new { Id = id },
                    commandType: System.Data.CommandType.Text);
                conn.Close();

            }
            return p;
        }
        public Product GetProductByBarcode(string barcode)
        {
            Product p;
            using (var conn = new SqlConnection(conStr))
            {
                conn.Open();
                p = conn.QuerySingleOrDefault<Product>("SELECT * FROM Products p(NOLOCK) WHERE LabelCode = @Barcode; ",
                    new { Barcode = barcode },
                    commandType: System.Data.CommandType.Text);
                conn.Close();

            }
            return p;
        }

    }
}
