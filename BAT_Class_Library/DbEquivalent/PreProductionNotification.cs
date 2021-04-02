using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace BAT_Class_Library
{
    [Table("PreProductionNotifications")]
    public class PreProductionNotification : BaseEntity
    {
        //public int Id { get; set; }
        public int ProductId { get; set; }
        public string PaletteBarcode { get; set; }
        public string BatchNo { get; set; }
        public DateTime PreNotificationTime { get; set; }
        public string Lot { get; set; }
        public bool ReadOnTheConveyor { get; set; }
    }
}
