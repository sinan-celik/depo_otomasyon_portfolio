using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace BAT_Class_Library
{
    [Table("PalletTransactions")]
    public class PalletTransaction : BaseEntity
    {
        public int TaskBatch { get; set; }
        public int TaskId { get; set; }
        public string PaletteBarcode { get; set; }
        public string AddressCode { get; set; }
        public string MachineCode { get; set; }
        public string TransactionType { get; set; }
        public string ReferanceOrder { get; set; }
        public DateTime TransactionTime { get; set; }
    }
}
