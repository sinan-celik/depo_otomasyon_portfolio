using BAT_Class_Library.DbEquivalent;
using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace DataAccess
{
    public class ProductQuantitiesPlacesData
    {

        private readonly IConfiguration _configuration;
        private string conStr;
        public ProductQuantitiesPlacesData()
        {

            _configuration = BaseDataAccess.configuration;
            conStr = _configuration.GetConnectionString("WHSimulation");
        }


        public ProductQuantityPlace GetProductQuantityPlaceById(int id)
        {
            ProductQuantityPlace productQuantityPlace;
            using (var conn = new SqlConnection(conStr))
            {
                conn.Open();
                productQuantityPlace = conn.QueryFirstOrDefault<ProductQuantityPlace>("SELECT * FROM ProductQuantitiesPlaces pqp(NOLOCK) WHERE Id = @Id", new { Id = id }, commandType: System.Data.CommandType.Text);
                conn.Close();

            }
            return productQuantityPlace;
        }


        public bool UpdateProductQuantityPlace(ProductQuantityPlace pqp)
        {
            bool done;
            using (var conn = new SqlConnection(conStr))
            {
                conn.Open();
                done = conn.Update(pqp);
                conn.Close();

            }
            return done;
        }
    }
}
