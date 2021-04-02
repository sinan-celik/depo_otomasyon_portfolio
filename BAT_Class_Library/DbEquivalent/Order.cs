using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace BAT_Class_Library
{
    [Table("Orders")]
    public class Order : BaseEntity
    {
        //public int Id { get; set; }
        public int PlaceId { get; set; }
        [System.ComponentModel.DisplayName("Order No")]
        public string OrderNo { get; set; }
        [System.ComponentModel.DisplayName("Is Completed")]
        public bool IsCompleted { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateUser { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public string LastUptadeUser { get; set; }
        public bool IsDeleted { get; set; }
    }
}
