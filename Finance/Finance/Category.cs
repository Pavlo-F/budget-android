using System;
using System.Collections.Generic;
using System.Text;
using SQLite.Net.Attributes;

namespace Common
{
    [Table("Category")]
    public class Category
    {
        [PrimaryKey, AutoIncrement, Column("_id")]
        public long id { get; set; }
        public string name { get; set; }

        public override string ToString()
        {
            return name;
        }

        public Category()
        {

        }

    }
}
