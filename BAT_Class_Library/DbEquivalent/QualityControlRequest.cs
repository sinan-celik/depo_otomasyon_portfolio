using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace BAT_Class_Library
{
    [Table("QualityControlRequests")]
    public class QualityControlRequest : BaseEntity
    {
        public int ProductId { get; set; }
        public string AddressCode { get; set; }
        public string PaletteBarcode { get; set; }
        public string RequestOwner { get; set; }
        public string ControllerUser { get; set; }
        public decimal BeforeBoxQuantity { get; set; }
        public decimal AfterBoxQuantity { get; set; }
        public string Status { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateUser { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public string LastUptadeUser { get; set; }
        public bool IsDeleted { get; set; }
    }
}
