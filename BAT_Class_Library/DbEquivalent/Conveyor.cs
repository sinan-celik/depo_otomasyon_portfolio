using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace BAT_Class_Library
{
    [Table("Conveyors")]
    public class Conveyor : BaseEntity
    {
        public string PCode { get; set; }
        public string Code { get; set; }
        public string IpAddress { get; set; }
        public int Port { get; set; }
        public string Location { get; set; }
        public string Status { get; set; }
        public int X { get; set; }
        public int Z1 { get; set; }
        public int Z2 { get; set; }
        public int G { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateUser { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public string LastUptadeUser { get; set; }
        public bool IsDeleted { get; set; }
    }
}
