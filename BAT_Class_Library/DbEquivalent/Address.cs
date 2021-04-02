using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace BAT_Class_Library
{
    [Table("Addresses")]
    public class Address : BaseEntity
    {
        public string PCode { get; set; }
        public string Code { get; set; }
        public int LineNumber { get; set; }
        public string ShelfLetter { get; set; }
        public string Direction { get; set; }
        public bool IsActive { get; set; }
        public string BlockReason { get; set; }
        public bool FirstRowIsEmpty { get; set; }
        public DateTime LastLoadTime { get; set; }
        public int X { get; set; }
        public int Z1 { get; set; }
        public int Z2 { get; set; }
        public int G { get; set; }
        public string DistanceType { get; set; }
        public decimal DistanceToRefPoint { get; set; }
        public int DependedProductId { get; set; }
        public decimal CurrentDensity { get; set; }
        public string DensityMeasurementType { get; set; }
        public decimal MaxDensityValue { get; set; }
        public string Affiliation { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateUser { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public string LastUptadeUser { get; set; }
        public bool IsDeleted { get; set; }
    }
}
