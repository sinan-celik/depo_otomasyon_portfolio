using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace BAT_Class_Library
{
    [Table("TimeParameters")]
    public class TimeParameter : BaseEntity
    {
        public string MachineType { get; set; }
        public string MovementType { get; set; }
        public int TimeInSeconds { get; set; }
        public string MeasureUnit { get; set; }
        public decimal MeasureValue { get; set; }
    }
}
