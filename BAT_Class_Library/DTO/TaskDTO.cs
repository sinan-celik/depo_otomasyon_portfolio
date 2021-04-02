using System;
using System.Collections.Generic;
using System.Text;

namespace BAT_Class_Library
{
    public class TaskDTO
    {
        //Plc ile task detayını yazmayı amaçlar, writed data nesnesini buradan set etmeyi planlıyorum
        public int taskId { get; set; }
        public TaskType taskType { get; set; }
        public int X { get; set; }
        public int Z1 { get; set; }
        public int Z2 { get; set; }
        public int G { get; set; }
        public Plc_Asrs plc_Asrs { get; set; }
    }
}
