using System;
using System.Collections.Generic;
using System.Text;
using SQLite.Net.Attributes;

namespace Common
{
    [Table("Bill")]
    public class Bill
    {
        [PrimaryKey, AutoIncrement, Column("_id")]
        public long id { get; set; }
        public string name { get; set; }
        public double price { get; set; }
        public long date { get; set; }
        public string imgPath { get; set; }
        public string note { get; set; }
        public long id_category { get; set; }
    }
}
