using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace BAT_Class_Library
{
    [Table("ProductionLines")]
    public class ProductionLine : BaseEntity
    {
        public string Code { get; set; }
        public string LastProducedProduct { get; set; }
        public DateTime LastProductionStart { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateUser { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public string LastUptadeUser { get; set; }
    }
}
