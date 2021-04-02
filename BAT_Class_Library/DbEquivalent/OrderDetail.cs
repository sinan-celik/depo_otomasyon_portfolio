using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace BAT_Class_Library
{
    [Table("OrderDetails")]
    public class OrderDetail : BaseEntity
    {

        //public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int BoxQuantity { get; set; }
        public string Status { get; set; }
        public bool AllPalletsTaken { get; set; }
        public DateTime? CreateDate { get; set; }
        public string CreateUser { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public string LastUptadeUser { get; set; }
        public bool IsDeleted { get; set; }
    }
}
