using Android.Database;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using Common;
using Finance.Droid.Activities;
using Finance.Droid.Adapters;
using Java.Text;
using Java.Util;
using System;
using System.Collections.Generic;

namespace Finance.Droid.Fragments
{
    public class StatisticFragment : Fragment
    {
        DatePickerFragment beginDateDialog = null;
        DatePickerFragment endDateDialog = null;
        ListView statisticItems = null;
        Button btnBeginDate;
        Button btnEndDate;
        Switch statSwitch;
        TextView dohodView;
        TextView rashodView;
        TextView totalView;
        bool switchChecked = false;
        bool isOnCreate = false;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public static StatisticFragment NewInstance()
        {
            var frag = new StatisticFragment { Arguments = new Bundle() };
            return frag;
        }


        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var ignored = base.OnCreateView(inflater, container, savedInstanceState);
            View tmp = inflater.Inflate(Resource.Layout.StatisticFragmentLayout, null);

            statisticItems = tmp.FindViewById<ListView>(Resource.Id.statisticListView);

            dohodView = tmp.FindViewById<TextView>(Resource.Id.textViewPositiv);
            rashodView = tmp.FindViewById<TextView>(Resource.Id.textViewNegativ);
            totalView = tmp.FindViewById<TextView>(Resource.Id.textViewStatTotal);

            btnBeginDate = tmp.FindViewById<Button>(Resource.Id.btnBeginDate);
            if (!btnBeginDate.HasOnClickListeners)
            {
                btnBeginDate.Click += showBeginDateDialog;
                btnBeginDate.TextChanged += onDateChanged;
            }

            btnEndDate = tmp.FindViewById<Button>(Resource.Id.btnEndDate);
            if (!btnEndDate.HasOnClickListeners)
            {
                btnEndDate.Click += showEndDateDialog;
                btnEndDate.TextChanged += onDateChanged;
            }

            statSwitch = tmp.FindViewById<Switch>(Resource.Id.statSwitch);
            if (!statSwitch.HasOnClickListeners)
            {
                statSwitch.Click += onSwitchChanged;
            }


            beginDateDialog = new DatePickerFragment(btnBeginDate);
            endDateDialog = new DatePickerFragment(btnEndDate);

            // определяем текущую дату
            DateFormat dateFormat = Android.Text.Format.DateFormat.GetDateFormat(Activity);
            btnEndDate.Text = dateFormat.Format(Calendar.Instance.Time);
            endDateDialog.DateBill = Calendar.Instance;

            Calendar c = Calendar.Instance;
            c.Add(CalendarField.Month, -1);
            btnBeginDate.Text = dateFormat.Format(c.Time);
            beginDateDialog.DateBill = c;

            showStatistic("<", beginDateDialog.DateBill, endDateDialog.DateBill);
            isOnCreate = true;

            return tmp;
        }

        private void showStatistic(string order, Calendar beginDate, Calendar endDate)
        {
            beginDate.Set(CalendarField.HourOfDay, 0); // чтобы весь текущий день был включён в поиск
            beginDate.Set(CalendarField.Minute, 0);
            beginDate.Set(CalendarField.Second, 0);

            endDate.Set(CalendarField.HourOfDay, 23); // чтобы весь крайний день был включён в поиск
            endDate.Set(CalendarField.Minute, 59);
            endDate.Set(CalendarField.Second, 59);

            double Total = 0;
            MatrixCursor matrixCursor = new MatrixCursor(new string[] { DatabaseHelper.KEY_BILL_ID, "Total", DatabaseHelper.KEY_CATEGORY_NAME });
            IEnumerator<StatisticByCategory> list = DatabaseHelper.Instance.getStatisticsEnumerator(order, beginDate.TimeInMillis, endDate.TimeInMillis);

            try
            {
                if (list.MoveNext())
                {
                    do
                    {
                        matrixCursor.AddRow(new Java.Lang.Object[] { list.Current.id, list.Current.Total, list.Current.name });
                        Total += list.Current.Total;
                    }
                    while (list.MoveNext());
                }
            }
            catch (Exception e)
            {
                Snackbar.Make(MainActivity.drawerLayout, "Ошибка при отображении", Snackbar.LengthLong).Show();
            }

            DisplayMetrics display = Activity.Resources.DisplayMetrics;
            StatisticCursorAdapter statisticAdapter = new StatisticCursorAdapter(Activity, matrixCursor, true, display.WidthPixels - 30, (Total == 0) ? 1 : Total); // -30 учёт утступа слева и справа по 7dp
            statisticItems.Adapter = statisticAdapter;

            double dohod = DatabaseHelper.Instance.getTotal(">", beginDate.TimeInMillis, endDate.TimeInMillis);
            double rashod = DatabaseHelper.Instance.getTotal("<", beginDate.TimeInMillis, endDate.TimeInMillis);
            double total = dohod + rashod;

            dohodView.Text = "Доход: " + dohod.ToString("F") + " р.";
            rashodView.Text = "Расход: " + rashod.ToString("F") + " р.";
            totalView.Text = "Итог: " + total.ToString("F") + " р.";
            if (total >= 0)
            {
                totalView.SetTextColor(new Android.Graphics.Color(0, 180, 12, 100)); // green
            }
            else
            {
                totalView.SetTextColor(new Android.Graphics.Color(180, 0, 0, 100)); // red
            }
        }

        public void onSwitchChanged(object sender, EventArgs args)
        {
            switchChecked = !switchChecked;
            onDateChanged(null, null);
        }

        public void showBeginDateDialog(object sender, EventArgs args)
        {
            beginDateDialog.Show(Activity.FragmentManager, "datePicker");
        }

        public void showEndDateDialog(object sender, EventArgs args)
        {
            endDateDialog.Show(Activity.FragmentManager, "datePicker");
        }

        public void onDateChanged(object sender, EventArgs args)
        {
            Calendar beginDate = beginDateDialog.DateBill;
            Calendar endDate = endDateDialog.DateBill;
            string order = "=";

            if (switchChecked)
            {
                order = ">";
            }
            else
            {
                order = "<";
            }

            if (isOnCreate)
            {
                showStatistic(order, beginDate, endDate);
            }
        }

    }
}