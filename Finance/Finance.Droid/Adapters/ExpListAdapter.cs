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
using System.Globalization;
using Finance.Droid.Fragments;
using Android.Support.Design.Widget;
using Finance.Droid.Activities;

namespace Finance.Droid.Adapters
{
    class Pair: Java.Lang.Object
    {
        public int group;
        public int child;
    }

    class ExpListAdapter : BaseExpandableListAdapter
    {
        private List<List<Bill>> mGroups;
        private Context mContext;
        //private ExpListAdapter mAdapter = null;
        EventHandler menuDelegat = null;
        //EventHandler showPhotoDelegat = null;
        //private Activity mActivity = null;

        public ExpListAdapter(Context context, List<List<Bill>> groups)
        {
            mContext = context;
            mGroups = groups;
        }

        public List<List<Bill>> Groups
        {
            get
            {
                return mGroups;
            }
            set
            {
                mGroups = value;
            }
        }

        public override int GroupCount
        {
            get
            {
                return mGroups.Count;
            }
        }

        public override bool HasStableIds
        {
            get
            {
                return true;
            }
        }

        public override Java.Lang.Object GetChild(int groupPosition, int childPosition)
        {
            return null;
        }


        public override long GetChildId(int groupPosition, int childPosition)
        {
            return childPosition;
        }

        public override int GetChildrenCount(int groupPosition)
        {
            return mGroups[groupPosition].Count;
        }

        public override Java.Lang.Object GetGroup(int groupPosition)
        {
            return null;
        }


        public override long GetGroupId(int groupPosition)
        {
            return groupPosition;
        }

        public override bool IsChildSelectable(int groupPosition, int childPosition)
        {
            return true;
        }

        public override View GetGroupView(int groupPosition, bool isExpanded, View convertView, ViewGroup parent)
        {
            if (convertView == null)
            {
                LayoutInflater inflater = (LayoutInflater)mContext.GetSystemService(Context.LayoutInflaterService);
                convertView = inflater.Inflate(Resource.Layout.groupBillView, null);
            }

            //if (isExpanded)
            //{

            //}
            //else
            //{

            //}

            TextView textGroup = (TextView)convertView.FindViewById(Resource.Id.textGroup);
            TextView textTotal = (TextView)convertView.FindViewById(Resource.Id.textTotal);

            double totalPrice = 0;
            List<Bill> billGroupList = mGroups[groupPosition];
            var isProfitExist = false;


            foreach (Bill item in billGroupList)
            {
                totalPrice += item.price;

                if(item.price > 0)
                {
                    isProfitExist = true;
                }
            }

            long date = billGroupList[0].date * 10000 + (new DateTime(1970, 1, 1).Ticks);
            DateTime groupDate = new DateTime(date);
            string vv = groupDate.ToString("ddd dd MMMM yyyy", CultureInfo.CreateSpecificCulture("ru-RU"));
            textTotal.Text = totalPrice.ToString("F") + " р.";
            textGroup.Text = vv;

            if (totalPrice > 0)
            {
                textTotal.SetTextColor(new Android.Graphics.Color(0, 180, 12, 200)); // green
            }
            else if (isProfitExist)
            {
                textTotal.SetTextColor(new Android.Graphics.Color(255, 165, 0, 200)); // orange
            }
            else
            {
                textTotal.SetTextColor(Android.Graphics.Color.Black);
            }

            return convertView;
        }

        public override View GetChildView(int groupPosition, int childPosition, bool isLastChild, View convertView, ViewGroup parent)
        {
            if (convertView == null)
            {
                LayoutInflater inflater = (LayoutInflater)mContext.GetSystemService(Context.LayoutInflaterService);
                convertView = inflater.Inflate(Resource.Layout.childBillView, null);
            }
            
            TextView textViewBill = (TextView)convertView.FindViewById(Resource.Id.textViewBill);
            TextView textViewCatigory = (TextView)convertView.FindViewById(Resource.Id.textViewCatigory);
            TextView textChildPrice = (TextView)convertView.FindViewById(Resource.Id.textViewPrice);
            Bill tmpBill = mGroups[groupPosition][childPosition];
            textViewBill.Text = tmpBill.name;
            textViewBill.Tag = tmpBill.id;
            textChildPrice.Text = tmpBill.price.ToString("F") + " р.";
            if (tmpBill.id_category > 0)
            {
                try
                {
                    textViewCatigory.Text = DatabaseHelper.database.Table<Category>().First(s => s.id == tmpBill.id_category).name;
                }
                catch
                {
                    textViewCatigory.Text = "Без категории";
                }
            }

            ImageButton button = (ImageButton)convertView.FindViewById(Resource.Id.btnAction);
            button.Tag = new Pair { group = groupPosition, child = childPosition }; // исправление ошибки позиционирования в обработчике. В Java решается final

            if (!button.HasOnClickListeners)
            {
                menuDelegat = delegate
                {

                    PopupMenu popupMenu = new PopupMenu(mContext, button);
                    Pair positions = (Pair)button.Tag;
                    Bill innerTmpBill = mGroups[positions.group][positions.child];

                    popupMenu.Inflate(Resource.Menu.popupmenu);
                    if (innerTmpBill.imgPath == null)
                    {
                        popupMenu.Menu.GetItem(0).SetEnabled(false);
                    }
                    else
                    {
                        popupMenu.Menu.GetItem(0).SetEnabled(true);
                    }
       
                    EventHandler<PopupMenu.MenuItemClickEventArgs> actionDelegat = (handled, item) =>
                    {
                        DatabaseHelper db = DatabaseHelper.Instance;
                        
                        switch (item.Item.ItemId)
                        {
                            case Resource.Id.showPhoto:
                                Intent intent = new Intent();
                                intent.SetAction(Intent.ActionView);
                                var tmp = new Java.IO.File(App._dir.ToString() + "//" + innerTmpBill.imgPath);
                                intent.SetDataAndType(Android.Net.Uri.FromFile(tmp), "image/*");
                                mContext.StartActivity(intent);
                                break;

                            case Resource.Id.deleteItem:
                                AlertDialog.Builder alert = new AlertDialog.Builder(mContext);
                                alert.SetTitle("Удаление");
                                alert.SetMessage("Вы действитель хотите удалить запись \"" + innerTmpBill.name + "\" ?");


                                alert.SetPositiveButton("ОК", (dialog, whichButton) =>
                                {
                                    try
                                    {
                                        if (db.deleteBill(innerTmpBill))
                                        {                                          
                                            mGroups[positions.group].RemoveAt(positions.child);

                                            // удаляем узел с нулевым количеством элементов чтобы не появлялась пустая строка в интерфейсе
                                            if (mGroups[positions.group].Count == 0)
                                            {
                                                mGroups.RemoveAt(positions.group);
                                            }

                                            try
                                            {
                                                if (innerTmpBill.imgPath != null)
                                                {
                                                    string delFile = App._dir.ToString() + "//" + innerTmpBill.imgPath;
                                                    System.IO.File.Delete(delFile);
                                                }
                                            }
                                            catch (Exception e)
                                            {
                                                Snackbar.Make(MainActivity.drawerLayout, "Ошибка удаления фото", Snackbar.LengthLong).Show();
                                            }
                                            this.NotifyDataSetChanged();
                                            Snackbar.Make(MainActivity.drawerLayout, "Запись \"" + innerTmpBill.name + "\" удалена", Snackbar.LengthLong).Show();
                                        }
                                        else
                                        {
                                            Snackbar.Make(MainActivity.drawerLayout, "Ошибка удаления", Snackbar.LengthLong).Show();
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        Snackbar.Make(MainActivity.drawerLayout, "Ошибка удаления записи \"" + innerTmpBill.name + "\"", Snackbar.LengthLong).Show();
                                    }

                                    button.Click -= menuDelegat;
                                });

                                alert.SetNegativeButton("Отмена", (dialog, whichButton) =>
                                {
                                    // what ever you want to do with No option.
                                });

                                alert.Show();

                                break;
                        }
                    };

                    popupMenu.MenuItemClick -= actionDelegat;
                    popupMenu.MenuItemClick += actionDelegat;
                    popupMenu.Show();
                };

                button.Click += menuDelegat;
            }
            
            return convertView;
        }


    }
}