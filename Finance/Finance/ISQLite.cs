using SQLite.Net;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    interface ISQLite
    {
        SQLiteConnection GetConnection(string filename);
    }
}
