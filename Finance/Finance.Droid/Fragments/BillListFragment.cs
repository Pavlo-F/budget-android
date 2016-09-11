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
    public class BillListFragment : Fragment
    {
        ImageButton searchBtn;
        AutoCompleteTextView searchText;
        EventHandler searchDelegat = null;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public static BillListFragment NewInstance()
        {
            var frag = new BillListFragment { Arguments = new Bundle() };
            return frag;
        }


        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var ignored = base.OnCreateView(inflater, container, savedInstanceState);
            View tmp = inflater.Inflate(Resource.Layout.BillListFragmentLayout, null);

            ExpandableListView listView = (ExpandableListView)tmp.FindViewById(Resource.Id.expListView);
            searchBtn = (ImageButton)tmp.FindViewById(Resource.Id.SearchBtn);
            searchText = (AutoCompleteTextView)tmp.FindViewById(Resource.Id.SearchEditText);

            List<string> names = DatabaseHelper.Instance.getBillsName();
            names.AddRange(DatabaseHelper.Instance.getCategoriesName());

            ArrayAdapter<string> adapterBill = new ArrayAdapter<string>
            (Activity, Resource.Layout.support_simple_spinner_dropdown_item, names);
            searchText.Adapter = adapterBill;

            ExpListAdapter adapterExp = new ExpListAdapter(Activity, getGroups(null));
            listView.SetAdapter(adapterExp);

            if (!searchBtn.HasOnClickListeners)
            {
                searchDelegat = delegate
                {
                    try
                    {
                        adapterExp = new ExpListAdapter(Activity, getGroups(searchText.Text));
                        listView.SetAdapter(adapterExp);
                    }
                    catch
                    {
                        Snackbar.Make(MainActivity.drawerLayout, "Возникла ошибка при поиске", Snackbar.LengthLong).Show();
                    }
                };

                searchBtn.Click += searchDelegat;
            }

            return tmp;
        }


        private List<List<Bill>> getGroups(string searchStr)
        {
            List<List<Bill>> groups = new List<List<Bill>>();
            List<string> groupList;
            List<Bill> children = null;
            groupList = DatabaseHelper.Instance.getBillsGroups();

            if (string.IsNullOrEmpty(searchStr))
            {
                foreach (string item in groupList)
                {
                    children = DatabaseHelper.Instance.getAllBills(item);
                    groups.Add(children);
                }
            }
            else
            {   // такой метод поиска был использован в связи с проблемами оператора LIKE в SQLite и кириллицей
                Category tmpCat = DatabaseHelper.database.Table<Category>().Where(c => c.name == searchStr).FirstOrDefault();
                List<Bill> tmpBill = new List<Bill>();

                if (tmpCat == null)
                {
                    foreach (string item in groupList)
                    {
                        children = DatabaseHelper.Instance.getAllBills(item);
                        tmpBill = children.FindAll((c) => c.name.ToLower() == searchStr.ToLower());
                        if (tmpBill.Count > 0)
                        {
                            groups.Add(tmpBill);
                        }

                    }
                }
                else
                {
                    foreach (string item in groupList)
                    {
                        children = DatabaseHelper.Instance.getAllBills(item);
                        tmpBill = children.FindAll((c) => c.name.ToLower() == searchStr.ToLower() || c.id_category == tmpCat.id);
                        if (tmpBill.Count > 0)
                        {
                            groups.Add(tmpBill);
                        }

                    }
                }
            }

            return groups;
        }

    }
}