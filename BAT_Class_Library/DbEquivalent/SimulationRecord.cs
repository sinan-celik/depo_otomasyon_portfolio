using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BAT_Class_Library
{
    [Table("SimulationRecords")]
    public class SimulationRecord : BaseEntity
    {

        public int ProductNotificationId { get; set; }
        public int DependedTaskBatchNo { get; set; }
        public string MachineCode { get; set; }
        public int WaitingPaletteBufferCount { get; set; }
        public int WaitingPaletteConveyorQuee { get; set; }
        public int ExecutionDurationInSeconds { get; set; }
        public DateTime ExecutionCompleteDateTime { get; set; }
    }
}
