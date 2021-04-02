using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace BAT_Class_Library
{
    [Table("Shuttles")]
    public class Shuttle : BaseEntity
    {
        public string Code { get; set; }
        public string IpAddress { get; set; }
        public int Port { get; set; }
        public string LastAddress { get; set; }
        public string Status { get; set; }
        public string Assignment { get; set; }
        public int ChargePercent { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateUser { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public string LastUptadeUser { get; set; }
        public bool IsDeleted { get; set; }
    }

}
