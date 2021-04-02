using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace BAT_Class_Library
{
    [Table("PalletsAtAddresses")]
    public class PalletsAtAddress : BaseEntity
    {
        //public int Id { get; set; }
        public int AddressId { get; set; }
        public int ProductionNotificationId { get; set; }
        public int ProductId { get; set; }
        public int DistanceToExit { get; set; }
        public string AddressCode { get; set; }
        public string PaletteBarcode { get; set; }
        public int BoxQuantity { get; set; }
        public string Lot { get; set; }
        public DateTime? EntryDate { get; set; }
        public string EntryReason { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public string ReleaseReason { get; set; }
        public bool IsInside { get; set; }
    }
}
