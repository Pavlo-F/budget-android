using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Common;
using System.IO;
using Finance.Droid.Database;
//using Xamarin.Forms;

//[assembly: Dependency(typeof(SQLite_Android))]
namespace Finance.Droid.Database
{
    public class SQLite_Android : ISQLite
    {
        public SQLite_Android() { }
        public SQLite.Net.SQLiteConnection GetConnection(string sqliteFilename)
        {
            string documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            var path = Path.Combine(documentsPath, sqliteFilename);
            // создаем подключение
            var plat = new SQLite.Net.Platform.XamarinAndroid.SQLitePlatformAndroid();
            var conn = new SQLite.Net.SQLiteConnection(plat, path);

            return conn;
        }
    }
}