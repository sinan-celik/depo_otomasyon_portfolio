using System;
using System.Collections.Generic;
using System.Text;

namespace BAT_Class_Library
{
    public enum TaskType
    {
        CTA = 1, //conveyor to address
        CTB = 2, //conveyor to buffer
        BTA = 3, //buffer to address
        ATA = 4, //address to address
        ATC = 5, //address to conveyor
        EMERGENCY = 6, //emergency tasks
        MAINTENANCE = 7, //maintenance tasks
        QC = 8, //quality control tasks
        COUNTING = 9, //warehouse counting tasks
        ShATA = 10, //shuttle address to address, applying both asrs and shuttle
        ShCHRG = 11, //shuttle charging, applying both asrs and shuttle
        ShOPT = 12, //shuttle optimization tube task
        ShINSERT = 13, //shuttle insert pallet to tube 
        ShTAKEOUT = 14 //shuttle take out pallet from tube
    }

    public enum Plc_Asrs
    {

        GO = 140,
        TAKE = 150,
        ShTAKE = 151,
        GIVE = 160,
        ShGIVE = 161,
        /**
            140: Pozisyona git. (ASRS)
            150: Palet Al (ASRS), Palet getir (Mekik)
            151: Mekik Al (ASRS)
            160: Palet Ver (ASRS), Palet yükle (Mekik)
            161: Mekik Ver (ASRS
            190: Optimizasyon (Mekik)

            Asrs için 140 görevi varsa, TASK2 atlanır.
            Mekik için TASK2 her zaman atlanır.

            */
    }

    public enum Plc_Shuttle
    {

        TAKE = 150,
        LOAD = 160,
        OPT = 190
    }

    public enum SysStatus  //Makinelerin plc de yazan durumları
    {
        NOTREADY = 0,
        READY = 1,
        BUSY = 2,
        ALARM = 3
    }
    //public sealed class SysStatusString
    //{
    //    private SysStatusString() { }

    //    public static readonly string NOTREADY = "NOTREADY";
    //    public static readonly string READY = "READY";
    //    public static readonly string BUSY = "BUSY";
    //    public static readonly string ALARM = "ALARM";
    //}

    public enum Plc_Task  //Görev yapılma durumu
    {
        /**
            Görev yapılma durumu. 10:Paleti alıyor. 11:Pozisyona gidiyor.
             29:Gripper çıkıyor. vs. 100:başarıyla bitti, 101:uyarı ile bitti. 
            Alarma  geçti ise son kaldığı id (29 silinmez.) Bu bilgi ile
            Yarım görev gönderilebilir.)
            Yeni göreve başlanırken 0’a çekilir. 
            */

        TAKINGPALETTE = 10,
        GOINGADDRESS = 11,
        GRIPPEROUT = 29,
        DONESUCCESSFULLY = 100,
        DONEWITHERROR = 101
    }


    public sealed class AddressesBlockReason
    {
        private AddressesBlockReason() { }

        public static readonly string USERBLOCKED = "USERBLOCKED";
        public static readonly string QUALITYCONTROL = "QUALITYCONTROL";
        public static readonly string RESERVED = "RESERVED";
    }
    public sealed class AddressType
    {
        private AddressType() { }

        public static readonly string ADDRESS = "ADDRESS";
        public static readonly string BUFFER = "BUFFER";
        public static readonly string CONVEYOR = "CONVEYOR";
    }
    public sealed class ShuttleAssign
    {
        private ShuttleAssign() { }

        public static readonly string OPTIMIZATION = "OPTIMIZATION"; //shuttle sadece optimizasyon işi yapsın 1 tane ayrılmış kesin olmalı
        public static readonly string EXPORT = "EXPORT"; //sadece export tüpleri için çalışsın 1 tane ayrılmış kesin olmalı
        public static readonly string CHARGE = "CHARGE"; //şarjda olan shuttle ın assignment i şarj durumlarına göre 
        public static readonly string IO = "IO"; //giriş ve çıkış işlerini yapan taşınmada öncelikli olanlar
    }

    public sealed class Location
    {
        private Location() { }

        public static readonly string WH_IN = "WH_IN";
        public static readonly string WH_OUT = "WH_OUT";
    }

    public sealed class Affiliation
    {
        private Affiliation() { }

        public static readonly string LOCAL = "LOCAL";
        public static readonly string EXPORT = "EXPORT";
    }

    class ProjectEnums
    {
        enum MyEnum
        {

        }
    }
}
