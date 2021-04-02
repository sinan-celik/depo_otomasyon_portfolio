using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace BAT_Class_Library
{
    [Table("ProductionNotifications")]
    public class ProductionNotification : BaseEntity
    {
        public int ProductId { get; set; }
        public int ProductionLineId { get; set; }
        public string PaletteBarcode { get; set; }
        public string BatchNo { get; set; }
        public DateTime NotificationTime { get; set; }
        public string WeightUnit { get; set; }
        public decimal Weight { get; set; }
        public int BoxQuantity { get; set; }
        public string Lot { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateUser { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public string LastUptadeUser { get; set; }
    }
}
