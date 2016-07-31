using Android.Database;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using Common;
using Finance.Droid.Activities;
using Finance.Droid.Adapters;
using System;
using System.Collections.Generic;

namespace Finance.Droid.Fragments
{
    public class ExportDatabaseFragment : Fragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public static ExportDatabaseFragment NewInstance()
        {
            var frag = new ExportDatabaseFragment { Arguments = new Bundle() };
            return frag;
        }


        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var ignored = base.OnCreateView(inflater, container, savedInstanceState);
            View tmp = inflater.Inflate(Resource.Layout.ExportDatabaseFragmentLayout, null);

            Button exportDB = tmp.FindViewById<Button>(Resource.Id.exportDBButton);
            exportDB.Click += ExportDB_Click;

            TextView dataBasePath = tmp.FindViewById<TextView>(Resource.Id.textViewDB);
            dataBasePath.Text = "База данных будет сохранена в " + App._dir.ToString() + "/" + DatabaseHelper.DATABASE_NAME;

            Button importDBFromOldVersion = tmp.FindViewById<Button>(Resource.Id.importDBFromOldVersion);
            importDBFromOldVersion.Click += ImportDBFromOldVersion_Click;

            return tmp;
        }

        private void ImportDBFromOldVersion_Click(object sender, EventArgs e)
        {
            Android.App.AlertDialog.Builder alert = new Android.App.AlertDialog.Builder(Activity);
            alert.SetTitle("Импорт");
            alert.SetMessage("Внимание, данные будут перезаписаны. Вы действительно хотите импортировать базу данных?");

            alert.SetPositiveButton("ОК", (dialog, whichButton) =>
            {
                try
                {
                    string storage = Android.OS.Environment.ExternalStorageDirectory.ToString();
                    string db = DatabaseHelper.database.DatabasePath;
                    System.IO.File.Copy(storage + "/" + DatabaseHelper.DATABASE_NAME, db, true);
                    Snackbar.Make(MainActivity.drawerLayout, "База данных импортирована", Snackbar.LengthLong).Show();
                }
                catch (Exception exp)
                {
                    Snackbar.Make(MainActivity.drawerLayout, "Ошибка импорта", Snackbar.LengthLong).Show();
                }
            });

            alert.SetNegativeButton("Отмена", (dialog, whichButton) =>
            {
                // what ever you want to do with No option.
            });

            alert.Show();

        }


        private void ExportDB_Click(object sender, EventArgs e)
        {
            try
            {
                System.IO.File.Copy(DatabaseHelper.database.DatabasePath, App._dir.ToString() + "/" + DatabaseHelper.DATABASE_NAME, true);
                Snackbar.Make(MainActivity.drawerLayout, "База данных сохранена", Snackbar.LengthLong).Show();
            }
            catch
            {
                Snackbar.Make(MainActivity.drawerLayout, "Ошибка сохранения", Snackbar.LengthLong).Show();
            }
        }
    }
}