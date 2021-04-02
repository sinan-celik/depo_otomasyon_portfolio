using Dapper.Contrib.Extensions;
using System;

namespace BAT_Class_Library
{
    [Table("Asrs")]
    public class Asrs : BaseEntity
    {
        public string Code { get; set; }
        public string IpAddress { get; set; }
        public int Port { get; set; }
        public string LastAddress { get; set; }
        public string Location { get; set; }
        public string Status { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateUser { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public string LastUptadeUser { get; set; }
        public bool IsDeleted { get; set; }
    }
}
