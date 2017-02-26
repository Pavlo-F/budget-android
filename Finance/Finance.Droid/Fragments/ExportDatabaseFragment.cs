using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using Common;
using Finance.Droid.Activities;
using System;

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

            return tmp;
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