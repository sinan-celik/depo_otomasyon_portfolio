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
    public class PalletsAtAddressesData
    {
        private readonly IConfiguration _configuration;
        private string conStr;
        public PalletsAtAddressesData()
        {

            _configuration = BaseDataAccess.configuration;
            conStr = _configuration.GetConnectionString("WHSimulation");
        }


        public PalletsAtAddress GetPalletsAtAddressById(int id)
        {
            PalletsAtAddress palletsAtAddress;
            using (var conn = new SqlConnection(conStr))
            {
                conn.Open();
                palletsAtAddress = conn.QueryFirstOrDefault<PalletsAtAddress>("SELECT * FROM PalletsAtAddresses paa(NOLOCK) WHERE Id = @Id", new { Id = id }, commandType: System.Data.CommandType.Text);
                conn.Close();

            }
            return palletsAtAddress;
        }

        public PalletsAtAddress GetPalletsAtAddressByProductNotificationId(int prodnotifid)
        {
            PalletsAtAddress palletsAtAddress;
            using (var conn = new SqlConnection(conStr))
            {
                conn.Open();
                palletsAtAddress = conn.QueryFirstOrDefault<PalletsAtAddress>("SELECT * FROM PalletsAtAddresses paa(NOLOCK) WHERE ProductionNotificationId = @ProductionNotificationId",
                    new { ProductionNotificationId = prodnotifid },
                    commandType: System.Data.CommandType.Text);
                conn.Close();

            }
            return palletsAtAddress;
        }

        public PalletsAtAddress GetPalletsAtAddressByPaletteBarcode(string barcode)
        {
            PalletsAtAddress palletsAtAddress;
            using (var conn = new SqlConnection(conStr))
            {
                conn.Open();
                palletsAtAddress = conn.QueryFirstOrDefault<PalletsAtAddress>("SELECT * FROM PalletsAtAddresses paa(NOLOCK) WHERE PaletteBarcode = @PaletteBarcode",
                    new { PaletteBarcode = barcode },
                    commandType: System.Data.CommandType.Text);
                conn.Close();

            }
            return palletsAtAddress;
        }

        public long InsertPalletsAtAddress(PalletsAtAddress palletsAtAddress)
        {
            long identity;
            using (var conn = new SqlConnection(conStr))
            {
                conn.Open();
                identity = conn.Insert(palletsAtAddress);
                conn.Close();

            }
            return identity;
        }

        public bool UpdatePalletsAtAddress(PalletsAtAddress palletsAtAddress)
        {
            bool done;
            using (var conn = new SqlConnection(conStr))
            {
                conn.Open();
                done = conn.Update(palletsAtAddress);
                conn.Close();

            }
            return done;
        }

        public bool UpdatePalletsAtAddressById(int id)
        {
            PalletsAtAddress palletsAtAddress = GetPalletsAtAddressById(id);

            bool done;
            using (var conn = new SqlConnection(conStr))
            {
                conn.Open();
                done = conn.Update(palletsAtAddress);
                conn.Close();

            }
            return done;
        }


        public void UpdatePalletsAtAddressWithShInsert(MachineTask completedMachineTask, int distance)
        {
            var paa = GetPalletsAtAddressByProductNotificationId(completedMachineTask.ProductNotificationId);
            paa.DistanceToExit = distance;
            UpdatePalletsAtAddress(paa);

        }
        public void UpdatePalletsAtAddressWithShTakeOut(MachineTask completedMachineTask)
        {

            //completedMachineTask.loadinfo e göre seçip distance 0 olarak güncellenebilir
            var paa = GetPalletsAtAddressByPaletteBarcode(completedMachineTask.LoadInfo);
            paa.DistanceToExit = 0;
            UpdatePalletsAtAddress(paa);

        }

        public long InsertPalettesAtAddresses(MachineTask completedMachineTask, ProductionNotification prodNotif)
        {
            PalletsAtAddress palletsAtAddress = new PalletsAtAddress
            {
                AddressId = 0,//
                ProductionNotificationId = completedMachineTask.ProductNotificationId,
                ProductId = prodNotif.ProductId,
                DistanceToExit = 80000,
                AddressCode = completedMachineTask.TargetAddress,
                PaletteBarcode = completedMachineTask.LoadInfo,
                BoxQuantity = prodNotif.BoxQuantity,
                Lot = prodNotif.Lot,
                EntryDate = DateTime.Now,
                EntryReason = "ProductionNotification",
                ReleaseDate = null,
                ReleaseReason = null,
                IsInside = true
            };


            long identity;
            using (var conn = new SqlConnection(conStr))
            {
                conn.Open();
                identity = conn.Insert(palletsAtAddress);
                conn.Close();

            }
            return identity;
        }

        public bool UpdatePalletsAtAddressWithRelease(string barcode)
        {
            var palletataddress = GetPalletsAtAddressByPaletteBarcode(barcode);
            palletataddress.ReleaseDate = DateTime.Now;
            palletataddress.ReleaseReason = "ORDER";//TODO:set reason with enum by task data
            palletataddress.IsInside = false;

            bool done;
            using (var conn = new SqlConnection(conStr))
            {
                conn.Open();
                done = conn.Update(palletataddress);
                conn.Close();

            }
            return done;
        }


        public int OptimizationUpdatePalletsAtAddresses(string addressCode)
        {
            int count;
            using (var conn = new SqlConnection(conStr))
            {
                conn.Open();
                count = conn.Execute(
                    "sp_OptimizationUpdatePalletsAtAddresses_upd",
                    new { AddressCode = addressCode, StartDistance = 100, increment = 100 },//TODO: start distance, increment
                    commandType: System.Data.CommandType.StoredProcedure
                    );
                conn.Close();

            }
            return count;
        }

        public List<string> GetNextOptimizationNeededAddresses()
        {
            List<string> addresscodes = new List<string>();
            using (var conn = new SqlConnection(conStr))
            {
                conn.Open();
                var list = conn.Query(
                    "[sp_GetNextOptimizationNeededAddresses_sel]",
                    commandType: System.Data.CommandType.StoredProcedure
                    );
                conn.Close();

                foreach (var rows in list)
                {
                    var fields = rows as IDictionary<string, object>;
                    var code = fields["AddressCode"];
                    addresscodes.Add(code.ToString());

                }

            }
            return addresscodes;
        }
    }
}
