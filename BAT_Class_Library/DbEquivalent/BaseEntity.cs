using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace BAT_Class_Library
{
    public abstract class BaseEntity
    {
        [Key]
        public int Id { get; set; }
    }
}
