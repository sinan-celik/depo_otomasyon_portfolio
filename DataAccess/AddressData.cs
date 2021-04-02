using BAT_Class_Library;
using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace DataAccess
{
    public class AddressData
    {
        private readonly IConfiguration _configuration;
        private string conStr;
        public AddressData()
        {

            _configuration = BaseDataAccess.configuration;
            conStr = _configuration.GetConnectionString("WHSimulation");
        }


        public IEnumerable<Address> GetAddresses()
        {
            IEnumerable<Address> addresses;
            using (var conn = new SqlConnection(conStr))
            {
                conn.Open();
                addresses = conn.Query<Address>("select * from Addresses", commandType: System.Data.CommandType.Text);
                conn.Close();

            }
            return addresses;
        }

        public string SelectRelatedAppropiriateAddress(int productId)
        {
            string str = "";
            using (var conn = new SqlConnection(conStr))
            {
                conn.Open();
                str = conn.QuerySingleOrDefault<string>("sp_SelectRelatedAppropiriateAddress_sel", new { ProductID = productId }, commandType: System.Data.CommandType.StoredProcedure);
                conn.Close();

            }
            return str;
        }

        public double[] GetDistanceToRefPoint(string sourceAddress, string targetAddress)
        {
            double sourceDist, targetDist;

            using (var conn = new SqlConnection(conStr))
            {
                conn.Open();
                sourceDist = conn.QuerySingleOrDefault<double>(
                    @"SELECT DistanceToRefPoint 
	                    FROM Addresses a(NOLOCK)
	                    WHERE Direction ='WH_IN' 
	                    AND Code = @Code;",
                    new { Code = sourceAddress },
                    commandType: System.Data.CommandType.Text);


                targetDist = conn.QuerySingleOrDefault<double>(
                        @"SELECT DistanceToRefPoint 
	                    FROM Addresses a(NOLOCK)
	                    WHERE Direction ='WH_IN' 
	                    AND Code = @Code;",
                        new { Code = targetAddress },
                        commandType: System.Data.CommandType.Text);
                conn.Close();

            }

            double[] dist = { sourceDist, targetDist };
            return dist;
        }

        public Address GetFirstRowIsEmpty(string addressCode)
        {
            Address address;

            using (var conn = new SqlConnection(conStr))
            {
                conn.Open();
                address = conn.QuerySingleOrDefault<Address>(
                    @"SELECT * 
	                    FROM Addresses a(NOLOCK)
	                    WHERE Direction ='WH_IN' 
	                    AND Code = @Code;",
                    new { Code = addressCode },
                    commandType: System.Data.CommandType.Text);
                conn.Close();

            }

            return address;
        }



        public Address GetFirstRowIsEmptyByProductId(int productID)
        {
            Address address;

            using (var conn = new SqlConnection(conStr))
            {
                conn.Open();
                address = conn.QuerySingleOrDefault<Address>(
                    @"SELECT * 
	                    FROM Addresses a(NOLOCK)
	                    WHERE Direction ='WH_IN' 
	                    AND DependedProductId = @ProductId;",
                    new { ProductId = productID },
                    commandType: System.Data.CommandType.Text);
                conn.Close();

            }

            return address;
        }

        public void AddressesChangeFirstRowInfo(string code, bool firstRowIsEmpty, DateTime? loadTime, string direction)
        {
            using var conn = new SqlConnection(conStr);
            conn.Open();
            conn.Execute(
                "sp_AddressesChangeFirstRowInfo_upd",
                new
                {
                    Code = code,
                    FirstRowIsEmpty = firstRowIsEmpty,
                    LastLoadTime = loadTime,
                    Direction = direction
                },
                commandType: System.Data.CommandType.StoredProcedure);
            conn.Close();

        }


        //adres ağzı boş dolu bilgisini güncellemek için krıtik önem
        public void AddressesUpdateFirstRowIsEmpty(string code, bool firstRowIsEmpty, string direction)
        {
            using var conn = new SqlConnection(conStr);
            conn.Open();
            conn.Execute(
                "sp_AddressesUpdateFirstRowIsEmpty_upd",
                new
                {
                    Code = code,
                    FirstRowIsEmpty = firstRowIsEmpty,
                    Direction = direction
                },
                commandType: System.Data.CommandType.StoredProcedure);
            conn.Close();

        }


        public bool GetFirstRowIsEmptyByAddressCode(string code, string location)
        {
            //task başlamadan(assign etmeden) önce ki kontrolleri yapmayı amaçlar
            bool IsEmpty;
            using (var conn = new SqlConnection(conStr))
            {
                conn.Open();
                IsEmpty = conn.QuerySingleOrDefault<bool>(
                    @"SELECT FirstRowIsEmpty FROM Addresses a(NOLOCK)
	                    WHERE Direction = @Direction 
	                    AND Code = @Code;",
                    new { Direction = location, Code = code },
                    commandType: System.Data.CommandType.Text);
                conn.Close();

            }

            return IsEmpty;
        }

        public List<Address> GetNeedShuttleAddresses()
        {
            //sp_GetNeedShuttleAddresses_sel
            List<Address> addresses;
            using (var conn = new SqlConnection(conStr))
            {
                conn.Open();
                addresses = conn.Query<Address>("sp_GetNeedShuttleAddresses_sel", commandType: System.Data.CommandType.StoredProcedure).ToList();
                conn.Close();

            }
            return addresses;

        }

        public string GetAvailableChargeAddress()
        {
            string address;
            using (var conn = new SqlConnection(conStr))
            {
                conn.Open();
                address = conn.QuerySingleOrDefault("sp_GetAvailableChargeAddress_sel", commandType: System.Data.CommandType.StoredProcedure);
                conn.Close();

            }
            return address;
        }
    }
}
