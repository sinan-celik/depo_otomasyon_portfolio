using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace BAT_Class_Library
{
    [Table("MachineTasks")]
    public class MachineTask : BaseEntity
    {
        public int OrderDetailPalletId { get; set; }
        public int ProductNotificationId { get; set; }
        public int TaskType { get; set; }
        public int TaskBatch { get; set; }
        public int Sequence { get; set; }
        public string MachineCode { get; set; }
        public string SourceType { get; set; }
        public string SourceAddress { get; set; }
        public string LoadInfo { get; set; }
        public string TargetType { get; set; }
        public string TargetAddress { get; set; }
        public string AssignUser { get; set; }
        public string AssignReason { get; set; }
        public DateTime AssignTime { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public bool SentFlag { get; set; }
        public bool IsCompleted { get; set; }
        public string ErrorCode { get; set; }
    }
}
