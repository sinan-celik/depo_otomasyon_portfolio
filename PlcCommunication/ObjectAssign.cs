using BAT_Class_Library;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlcCommunication
{
    public class ObjectAssign
    {


        public static int[] ObjectToIntArray<T>(T target)
        {
            List<int> values = new List<int>();
            foreach (var pi in typeof(T).GetProperties())
            {

                if (pi.GetValue(target, null).GetType() == typeof(int) && pi.Name != "Id")
                {
                    values.Add((int)pi.GetValue(target, null));
                }

            }

            return values.ToArray();
        }

        public static PlcCommunicationWritedData IntArrayToObject_WD(int[] array, MachinesDTO machinesDTO)
        {
            return new PlcCommunicationWritedData
            {
                //Id
                MW100 = array[0],
                MW101 = array[1],
                MW102 = array[2],
                MW103 = array[3],
                MW104 = array[4],
                MW105 = array[5],
                MW106 = array[6],
                MW107 = array[7],
                MW108 = array[8],
                MW109 = array[9],
                MW110 = array[10],
                MW111 = array[11],
                MW112 = array[12],
                MW113 = array[13],
                MW114 = array[14],
                MW115 = array[15],
                MW116 = array[16],
                MW117 = array[17],
                MW118 = array[18],
                MW119 = array[19],
                MW120 = array[20],
                MW121 = array[21],
                MW122 = array[22],
                MW123 = array[23],
                MW124 = array[24],
                MW125 = array[25],
                MW126 = array[26],
                MW127 = array[27],
                MW128 = array[28],
                MW129 = array[29],
                MW130 = array[30],
                MW131 = array[31],
                MW132 = array[32],
                MW133 = array[33],
                MW134 = array[34],
                MW135 = array[35],
                MW136 = array[36],
                MW137 = array[37],
                MW138 = array[38],
                MW139 = array[39],
                MW140 = array[40],
                MW141 = array[41],
                MW142 = array[42],
                MW143 = array[43],
                MW144 = array[44],
                MW145 = array[45],
                MW146 = array[46],
                MW147 = array[47],
                MW148 = array[48],
                MW149 = array[49],
                MachineCode = machinesDTO.Code,
                RecordDate = DateTime.Now
            };
        }

        public static PlcCommunicationReadedData IntArrayToObject_RD(int[] array, MachinesDTO machinesDTO)
        {
            return new PlcCommunicationReadedData
            {
                //Id
                MW200 = array[0],
                MW201 = array[1],
                MW202 = array[2],
                MW203 = array[3],
                MW204 = array[4],
                MW205 = array[5],
                MW206 = array[6],
                MW207 = array[7],
                MW208 = array[8],
                MW209 = array[9],
                MW210 = array[10],
                MW211 = array[11],
                MW212 = array[12],
                MW213 = array[13],
                MW214 = array[14],
                MW215 = array[15],
                MW216 = array[16],
                MW217 = array[17],
                MW218 = array[18],
                MW219 = array[19],
                MW220 = array[20],
                MW221 = array[21],
                MW222 = array[22],
                MW223 = array[23],
                MW224 = array[24],
                MW225 = array[25],
                MW226 = array[26],
                MW227 = array[27],
                MW228 = array[28],
                MW229 = array[29],
                MW230 = array[30],
                MW231 = array[31],
                MW232 = array[32],
                MW233 = array[33],
                MW234 = array[34],
                MW235 = array[35],
                MW236 = array[36],
                MW237 = array[37],
                MW238 = array[38],
                MW239 = array[39],//TODO: plc registerları düzeltildiğinde değiştir
                MW240 = array[39],//array[40], 
                MW241 = array[39],//array[41],
                MW242 = array[39],//array[42],
                MW243 = array[39],//array[43],
                MW244 = array[39],//array[44],
                MW245 = array[39],//array[45],
                MW246 = array[39],//array[46],
                MW247 = array[39],//array[47],
                MW248 = array[39],//array[48],
                MW249 = array[39],//array[49],
                MachineCode = machinesDTO.Code,
                RecordDate = DateTime.Now
            };
        }




    }
}
