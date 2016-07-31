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
using Java.Util;
using Java.Text;

namespace Finance.Droid.Fragments
{
    class DatePickerFragment : DialogFragment, DatePickerDialog.IOnDateSetListener
    {
        Calendar dateBill = Calendar.Instance;
        TextView viewWidget = null;

        public void OnDateSet(DatePicker view, int year, int monthOfYear, int dayOfMonth)
        {
            dateBill.Set(year, monthOfYear, dayOfMonth);
            DateFormat dateFormat = Android.Text.Format.DateFormat.GetDateFormat(Activity);
            viewWidget.Text = dateFormat.Format(dateBill.Time).ToString();
        }

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            int year = dateBill.Get(Calendar.Year);
            int month = dateBill.Get(Calendar.Month);
            int day = dateBill.Get(Calendar.DayOfMonth);

            // создаем DatePickerDialog и возвращаем его
            Dialog picker = new DatePickerDialog(Activity, this, year, month, day);

            picker.SetTitle("Выбор даты");

            return picker;
        }

        public DatePickerFragment(TextView v)
        {
            viewWidget = v;
        }

        public Calendar DateBill
        {
            get { return dateBill; }
            set { dateBill = value; }
        }



    }
}