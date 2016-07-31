using Android.Database;
using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using Common;
using Finance.Droid.Adapters;
using System;
using System.Collections.Generic;

namespace Finance.Droid.Fragments
{
    public class BillListFragment : Fragment
    {
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

            List<List<Bill>> groups = new List<List<Bill>>();

            List<string> groupList = DatabaseHelper.Instance.getBillsGroups();

            List<Bill> children = null;
            foreach (string item in groupList)
            {
                children = DatabaseHelper.Instance.getAllBills(item);
                groups.Add(children);
            }

            ExpandableListView listView = (ExpandableListView)tmp.FindViewById(Resource.Id.expListView);

            ExpListAdapter adapterExp = new ExpListAdapter(Activity, groups);
            listView.SetAdapter(adapterExp);

            return tmp;
        }
    }
}