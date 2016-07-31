using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    public class StatisticByCategory
    {
        [PrimaryKey, AutoIncrement, Column("_id")]
        public long id { get; set; }
        public double Total { get; set; }
        public string name { get; set; }
    }
}
