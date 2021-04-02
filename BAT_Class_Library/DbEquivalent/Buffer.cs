using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace BAT_Class_Library
{
    [Table("Buffers")]
    public class Buffer : BaseEntity
    {
        public string PCode { get; set; }
        public string Code { get; set; }
        public int LineNumber { get; set; }
        public string BufferLetter { get; set; }
        public bool IsActive { get; set; }
        public bool IsEmpty { get; set; }
        public int X { get; set; }
        public int Z1 { get; set; }
        public int Z2 { get; set; }
        public int G { get; set; }
        public string DistanceType { get; set; }
        public decimal DistanceToRefPoint { get; set; }
        public string LastPaletteInfo { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateUser { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public string LastUptadeUser { get; set; }
        public bool IsDeleted { get; set; }
    }
}
