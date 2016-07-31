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
    public class CategoryFragment : Fragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public static CategoryFragment NewInstance()
        {
            var frag2 = new CategoryFragment { Arguments = new Bundle() };
            return frag2;
        }


        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var ignored = base.OnCreateView(inflater, container, savedInstanceState);
            View tmp = inflater.Inflate(Resource.Layout.CategoryFragmentLayout, null);

            MatrixCursor matrixCursor = new MatrixCursor(new string[] { DatabaseHelper.KEY_CATEGORY_ID, DatabaseHelper.KEY_CATEGORY_NAME });
            IEnumerator<Category> list = DatabaseHelper.Instance.getAllCategoriesEnumerator();

            try
            {
                if (list.MoveNext())
                {
                    do
                    {
                        matrixCursor.AddRow(new Java.Lang.Object[] { list.Current.id, list.Current.name });
                    }
                    while (list.MoveNext());
                }
            }
            catch (Exception e)
            {
                Snackbar.Make(MainActivity.drawerLayout, "Ошибка при отображении", Snackbar.LengthLong).Show();
            }

            ListView lvItems = tmp.FindViewById<ListView>(Resource.Id.categoryListView);
            CategoryCursorAdapter categoryAdapter = new CategoryCursorAdapter(Activity, matrixCursor, true);
            lvItems.Adapter = categoryAdapter;

            return tmp;
        }
    }
}