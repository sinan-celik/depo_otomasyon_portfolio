using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace BAT_Class_Library.DbEquivalent
{
    [Table("ProductQuantitiesPlaces")]
    public class ProductQuantityPlace : BaseEntity
    {
        //public int Id { get; set; }
        public int ProductId { get; set; }
        public int PlaceId { get; set; }
        public int PalletCount { get; set; }
        public int BoxCount { get; set; }
    }
}
