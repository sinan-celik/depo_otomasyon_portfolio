using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace BAT_Class_Library
{
    [Table("SystemParameters")]
    public class SystemParameter : BaseEntity
    {
        //public int Id { get; set; }
        public string RelatedScenario { get; set; }
        public string RelatedMachineType { get; set; }
        public string RelatedTaskType { get; set; }
        public string ParameterName { get; set; }
        public string ParameterValue { get; set; }
        public string ValueType { get; set; }
        public string Description { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateUser { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public string LastUptadeUser { get; set; }
        public bool IsDeleted { get; set; }
    }
}
