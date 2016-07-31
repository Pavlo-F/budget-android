using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Database;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Finance.Droid.Adapters
{
    class StatisticCursorAdapter : CursorAdapter
    {
        int layoutWidth = 0;
        double MinTotal = 0;

        public StatisticCursorAdapter(Context context, ICursor c, bool autoRequery) : base(context, c, autoRequery)
        { }

        public StatisticCursorAdapter(Context context, ICursor c, bool autoRequery, int width, double minTotal) : base(context, c, autoRequery)
        {
            layoutWidth = width;
            MinTotal = minTotal;
        }

        public override void BindView(View view, Context context, ICursor cursor)
        {
            TextView nameView = (TextView)view.FindViewById(Resource.Id.nameView);
            TextView percentView = (TextView)view.FindViewById(Resource.Id.percentView);

            double total = cursor.GetDouble(1);
            string category = cursor.GetString(2);
            double percentTotal = total / MinTotal;

            nameView.Text = category + ": " + total + "ð. (" + (percentTotal * 100).ToString("F") + "%)";

            RelativeLayout.LayoutParams params1 = new RelativeLayout.LayoutParams((int)(layoutWidth * percentTotal), 30);
            params1.AddRule(LayoutRules.Below, nameView.Id);
            percentView.LayoutParameters = params1;
            if (total < 0)
            {
                percentView.SetBackgroundResource(Resource.Color.statRed);
            }
            else
            {
                percentView.SetBackgroundResource(Resource.Color.statGreen);
            }
        }

        public override View NewView(Context context, ICursor cursor, ViewGroup parent)
        {
            return LayoutInflater.From(context).Inflate(Resource.Layout.statistic_item_view, parent, false);
        }
    }
}