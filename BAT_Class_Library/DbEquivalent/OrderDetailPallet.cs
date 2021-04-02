using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace BAT_Class_Library
{
    [Table("OrderDetailPallets")]
    public class OrderDetailPallet : BaseEntity
    {
        //public int Id { get; set; }
        public int Seq { get; set; }
        public int OrderId { get; set; }
        public int OrderDetailId { get; set; }
        public int ProductId { get; set; }
        public string PalletBarcode { get; set; }
        public int BoxQuantity { get; set; }
        public int UsedQuantity { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateUser { get; set; }
        public DateTime? TakenDate { get; set; }
        public bool IsTaskCreated { get; set; }
        public bool IsTaken { get; set; }
    }
}
