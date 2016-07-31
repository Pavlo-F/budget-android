using Android.App;
using Android.Content.PM;
using Android.Content.Res;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Views;
using Android.Widget;

using Finance.Droid.Fragments;
using Android.Support.V7.App;
using Android.Support.V4.View;
using Android.Support.Design.Widget;
using System;
using Android.Content;

namespace Finance.Droid.Activities
{
    [Activity(Label = "Финансы", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, Icon = "@drawable/Icon")]
    public class MainActivity : BaseActivity
    {
        
        public static DrawerLayout drawerLayout;
        NavigationView navigationView;

        protected override int LayoutResource
        {
            get
            {
                return Resource.Layout.main;
            }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);



            drawerLayout = this.FindViewById<DrawerLayout>(Resource.Id.drawer_layout);

            //Set hamburger items menu
            SupportActionBar.SetHomeAsUpIndicator(Resource.Drawable.ic_menu);

            //setup navigation view
            navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            Title = "Добавление";
            //handle navigation
            navigationView.NavigationItemSelected += (sender, e) =>
            {
                e.MenuItem.SetChecked(true);
                
                switch (e.MenuItem.ItemId)
                {
                    case Resource.Id.nav_add:
                        ListItemClicked(0);
                        break;
                    case Resource.Id.nav_category:
                        ListItemClicked(1);
                        break;
                    case Resource.Id.nav_billList:
                        ListItemClicked(2);
                        break;
                    case Resource.Id.nav_statistic:
                        ListItemClicked(3);
                        break;
                    case Resource.Id.nav_exportDB:
                        ListItemClicked(4);
                        break;
                }

                Title = e.MenuItem.TitleFormatted.ToString();
                //Snackbar.Make(drawerLayout, "You selected: " + e.MenuItem.TitleFormatted, Snackbar.LengthLong).Show();

                drawerLayout.CloseDrawers();
            };


            //if first time you will want to go ahead and click first item.
            if (savedInstanceState == null)
            {
                ListItemClicked(0);
            }


        }

        int oldPosition = -1;
        private void ListItemClicked(int position)
        {
            //this way we don't load twice, but you might want to modify this a bit.
            if (position == oldPosition)
                return;

            oldPosition = position;

            Android.Support.V4.App.Fragment fragment = null;
            switch (position)
            {
                case 0:
                    fragment = AddFragment.NewInstance();
                    break;
                case 1:
                    fragment = CategoryFragment.NewInstance();
                    break;
                case 2:
                    fragment = BillListFragment.NewInstance();
                    break;
                case 3:
                    fragment = StatisticFragment.NewInstance();
                    break;
                case 4:
                    fragment = ExportDatabaseFragment.NewInstance();
                    break;
            }

            SupportFragmentManager.BeginTransaction()
                .Replace(Resource.Id.content_frame, fragment)
                .Commit();
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    drawerLayout.OpenDrawer(Android.Support.V4.View.GravityCompat.Start);
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }

    }
}

