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
using Common;
using Android.Support.Design.Widget;
using Finance.Droid.Activities;

namespace Finance.Droid.Adapters 
{
    class CategoryCursorAdapter : CursorAdapter
    {

        EventHandler deleteDelegat = null;
        EventHandler editDelegat = null;

        public CategoryCursorAdapter(Context context, ICursor c, bool autoRequery) : base(context, c, autoRequery)
        {  }

        public override void BindView(View view, Context context, ICursor cursor)
        {
            TextView tvCategory = (TextView)view.FindViewById(Resource.Id.tvCategory);

            string catText = cursor.GetString(1);
            long catId = cursor.GetInt(0);

            tvCategory.Text = catText;
            tvCategory.Tag = catId;

            ImageButton editCatBtn = (ImageButton)view.FindViewById(Resource.Id.editCatBtn);
            ImageButton delCatBtn = (ImageButton)view.FindViewById(Resource.Id.delCatBtn);

            if (!delCatBtn.HasOnClickListeners)
            { 
                deleteDelegat = delegate
                {
                    AlertDialog.Builder alert = new AlertDialog.Builder(context);
                    alert.SetTitle("Удаление");
                    alert.SetMessage("Вы действитель хотите удалить запись \"" + tvCategory.Text + "\" ?");

                    alert.SetPositiveButton("ОК", (dialog, whichButton) =>
                    {
                        try
                        {
                            DatabaseHelper tmpDB = DatabaseHelper.Instance;
                            if (tmpDB.deleteCategory((long)tvCategory.Tag))
                            {
                                MatrixCursor matrixCursor = new MatrixCursor(new string[] { DatabaseHelper.KEY_CATEGORY_ID, DatabaseHelper.KEY_CATEGORY_NAME });
                                IEnumerator<Category> list = DatabaseHelper.Instance.getAllCategoriesEnumerator();

                                if (list.MoveNext())
                                {
                                    do
                                    {
                                        matrixCursor.AddRow(new Java.Lang.Object[] { list.Current.id, list.Current.name });
                                    }
                                    while (list.MoveNext());
                                }

                                this.ChangeCursor(matrixCursor);
                                Snackbar.Make(MainActivity.drawerLayout, "Запись \"" + tvCategory.Text + "\" удалена", Snackbar.LengthLong).Show();

                                delCatBtn.Click -= deleteDelegat;
                                editCatBtn.Click -= editDelegat;
                            }
                        }
                        catch (Exception e)
                        {
                            Snackbar.Make(MainActivity.drawerLayout, "Ошибка удаления записи \"" + tvCategory.Text + "\"", Snackbar.LengthLong).Show();
                        }

                    });

                    alert.SetNegativeButton("Отмена", (dialog, whichButton) =>
                    {
                    // what ever you want to do with No option.
                });

                    alert.Show();

                };

                delCatBtn.Click += deleteDelegat;
            }
            

            if (!editCatBtn.HasOnClickListeners)
            {
                editDelegat = delegate
                {
                    AlertDialog.Builder alert = new AlertDialog.Builder(context);
                    EditText edittext = new EditText(context);
                    edittext.Text = tvCategory.Text;
                    alert.SetTitle("Редактирование");
                    alert.SetMessage("Введите название категории:");
                    alert.SetView(edittext);

                    alert.SetPositiveButton("ОК", (dialog, whichButton) =>
                    {
                        try
                        {
                            if (string.IsNullOrEmpty(edittext.Text))
                            {
                                Snackbar.Make(MainActivity.drawerLayout, "Название не может быть пустым", Snackbar.LengthLong).Show();
                                return;
                            }

                            DatabaseHelper tmpDB = DatabaseHelper.Instance;
                            if (tmpDB.editCategory((long)tvCategory.Tag, edittext.Text))
                            {
                                MatrixCursor matrixCursor = new MatrixCursor(new string[] { DatabaseHelper.KEY_CATEGORY_ID, DatabaseHelper.KEY_CATEGORY_NAME });
                                IEnumerator<Category> list = DatabaseHelper.Instance.getAllCategoriesEnumerator();

                                if (list.MoveNext())
                                {
                                    do
                                    {
                                        matrixCursor.AddRow(new Java.Lang.Object[] { list.Current.id, list.Current.name });
                                    }
                                    while (list.MoveNext());
                                }

                                this.ChangeCursor(matrixCursor);
                            }
                        }
                        catch (Exception e)
                        {
                            Snackbar.Make(MainActivity.drawerLayout, "Ошибка редактирования", Snackbar.LengthLong).Show();
                        }

                    });

                    alert.SetNegativeButton("Отмена", (dialog, whichButton) =>
                    {
                        // what ever you want to do with No option.
                    });

                    alert.Show();

                };

                editCatBtn.Click += editDelegat;
            }

        }



        public override View NewView(Context context, ICursor cursor, ViewGroup parent)
        {
            return LayoutInflater.From(context).Inflate(Resource.Layout.category_item_view, parent, false);
        }
    }
}