using Android.OS;
using Android.Support.V4.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Provider;
using Java.IO;
using Android.Graphics;
using Android.Net;
using Java.Lang;
using System;
using Android.Database;
using Common;
using Finance.Droid.Database;
using Android.Support.V7.App;
using System.Collections.Generic;
using Java.Util;
using Android.Support.Design.Widget;
using Finance.Droid.Activities;

namespace Finance.Droid.Fragments
{
    public static class App
    {
        public static File _file;
        public static File _dir;
        public static Bitmap bitmap;
    }

    public class AddFragment : Fragment
    {
        const int CAPTURE_IMAGE_ACTIVITY_REQUEST_CODE = 1034;
        ImageView photoView;
        Spinner operationSpinner;
        Spinner categorySpinner;
        EditText sumEdit = null;
        AutoCompleteTextView nameEdit = null;
        Button dateButton = null;
        DatePickerFragment dateDialog = null;

        DatabaseHelper dataBase = null;
        SQLite_Android db = null;

        string[] operationItems = { "Расход", "Доход" };

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Create your fragment here

            CreateDirectoryForPictures();

            db = new SQLite_Android();
            dataBase = new DatabaseHelper(db);
        }

        public static AddFragment NewInstance()
        {
            var frag1 = new AddFragment { Arguments = new Bundle() };
            return frag1;
        }


        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var ignored = base.OnCreateView(inflater, container, savedInstanceState);
            View tmp = inflater.Inflate(Resource.Layout.AddFragmentLayout, null);

            ImageButton LaunchCameraButton = tmp.FindViewById<ImageButton>(Resource.Id.btnTakePhoto);
            LaunchCameraButton.Click += onLaunchCamera;

            photoView = tmp.FindViewById<ImageView>(Resource.Id.photoView);

            operationSpinner = tmp.FindViewById<Spinner>(Resource.Id.spinnerOperation);
            ArrayAdapter<string> operationAdapter = new ArrayAdapter<string>(Activity, Resource.Layout.support_simple_spinner_dropdown_item, operationItems);
            operationAdapter.SetDropDownViewResource(Resource.Layout.support_simple_spinner_dropdown_item);
            operationSpinner.Adapter = operationAdapter;

            ImageButton AddCategoryButton = tmp.FindViewById<ImageButton>(Resource.Id.btnAddCategory);
            AddCategoryButton.Click += onAddCatButton;

            List<Category> tmpCatList = dataBase.getAllCategories();
            ArrayAdapter<Category> categoryAdapter = new ArrayAdapter<Category>(Activity, Resource.Layout.support_simple_spinner_dropdown_item, tmpCatList);
            categoryAdapter.SetDropDownViewResource(Resource.Layout.support_simple_spinner_dropdown_item);
            categorySpinner = tmp.FindViewById<Spinner>(Resource.Id.spinnerCategory);
            categorySpinner.Adapter = categoryAdapter;

            sumEdit = tmp.FindViewById<EditText>(Resource.Id.textTotalValue);
            nameEdit = tmp.FindViewById<AutoCompleteTextView>(Resource.Id.autoCompleteTextBill);

            ImageButton SaveButton = tmp.FindViewById<ImageButton>(Resource.Id.btnSaveBill);
            SaveButton.Click += onSaveButton;

            Java.Text.DateFormat dateFormat = Android.Text.Format.DateFormat.GetDateFormat(Activity);
            dateButton = tmp.FindViewById<Button>(Resource.Id.btnDate);
            dateButton.Text = dateFormat.Format(Calendar.Instance.Time).ToString();
            dateButton.Click += showDatePickerDialog;

            dateDialog = new DatePickerFragment(dateButton);
            dateDialog.DateBill = Calendar.Instance;

            ArrayAdapter<string> adapterBill = new ArrayAdapter<string>
                (Activity, Resource.Layout.support_simple_spinner_dropdown_item, dataBase.getBillsName());
            nameEdit.Adapter = adapterBill;

            return tmp;
        }

        private void onLaunchCamera(object sender, EventArgs args)
        {
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            long UnixDate = (long)(DateTime.Now.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            App._file = new File(App._dir, string.Format("Photo_{0}.jpg", UnixDate));
            intent.PutExtra(MediaStore.ExtraOutput, Android.Net.Uri.FromFile(App._file));

            if (intent.ResolveActivity(Activity.PackageManager) != null)
                {
                    StartActivityForResult(intent, CAPTURE_IMAGE_ACTIVITY_REQUEST_CODE);
                }
        }

        private void CreateDirectoryForPictures()
        {
            App._dir = new File(
                Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDcim), "Сosts");


            if (!App._dir.Exists())
            {
                App._dir.Mkdirs();
            }
        }

        public override void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            switch (requestCode)
            {

                case CAPTURE_IMAGE_ACTIVITY_REQUEST_CODE:
                    {
                        if (resultCode == -1)
                        {
                            // Display in ImageView. We will resize the bitmap to fit the display.
                            // Loading the full sized image will consume to much memory
                            // and cause the application to crash.

                            int height = photoView.Height; // Resources.DisplayMetrics.HeightPixels;
                            //int width = photoView.Width;

                            App.bitmap = BitmapHelpers.scaleToFitHeight(
                                BitmapHelpers.rotateBitmapOrientation(App._file.AbsolutePath), height);

                            if (App.bitmap != null)
                            {
                                photoView.SetImageBitmap(App.bitmap);
                                App.bitmap = null;
                            }

                            // Dispose of the Java side bitmap.
                            GC.Collect();
                        }

                        break;
                    }
            }
        }


        public void onAddCatButton(object sender, EventArgs args)
        {
            AlertDialog.Builder alert = new AlertDialog.Builder(Activity);
            EditText edittext = new EditText(Activity);
            alert.SetTitle("Добавление");
            alert.SetMessage("Введите название категории:");
            alert.SetView(edittext);


            alert.SetPositiveButton("ОК", (dialog, whichButton) => 
            {
                string textName = edittext.Text;
                if (string.IsNullOrEmpty(textName))
                {
                    Snackbar.Make(MainActivity.drawerLayout, "Название не может быть пустым", Snackbar.LengthLong).Show();
                    return;
                }
                Category tmp = new Category { name = textName };

                if(!dataBase.addCategory(tmp))
                {
                    Snackbar.Make(MainActivity.drawerLayout, "Ошибка при добавлении", Snackbar.LengthLong).Show();
                    return;
                }

                List<Category> tmpCatList = dataBase.getAllCategories();
                ArrayAdapter<Category> categoryAdapter = new ArrayAdapter<Category>(Activity, Resource.Layout.support_simple_spinner_dropdown_item, tmpCatList);
                categoryAdapter.SetDropDownViewResource(Resource.Layout.support_simple_spinner_dropdown_item);
                categorySpinner.Adapter = categoryAdapter;
                categorySpinner.SetSelection(tmpCatList.Count - 1);

            });

            alert.SetNegativeButton("Отмена", (dialog, whichButton) =>
            {
                // what ever you want to do with No option.
    
            });

            alert.Show();
        }


        public void onSaveButton(object sender, EventArgs args)
        {
            string textPrice = sumEdit.Text;
            string textName = nameEdit.Text;
            string textDate = dateButton.Text;

            if (string.IsNullOrEmpty(textName))
            {
                Snackbar.Make(MainActivity.drawerLayout, "Заполните поле \"Платёж / Покупка\"", Snackbar.LengthLong ).Show();
                return;
            }

            if (string.IsNullOrEmpty(textPrice))
            {
                Snackbar.Make(MainActivity.drawerLayout, "Заполните поле \"Сумма\"", Snackbar.LengthLong).Show();
                return;
            }

            if (string.IsNullOrEmpty(textDate))
            {
                Snackbar.Make(MainActivity.drawerLayout, "Заполните поле \"Дата внесения\"", Snackbar.LengthLong).Show();
                return;
            }
            
            double price = double.Parse(textPrice.Replace('.', ','));
            Category category = ObjectHelper.Cast<Category>(categorySpinner.SelectedItem);
            Bill bill = new Bill();
            bill.name = textName;
            bill.price = (operationSpinner.SelectedItemId == 1) ? price : (-1 * price);
            bill.date = dateDialog.DateBill.TimeInMillis;
            bill.imgPath = (App._file == null) ? null : App._file.Name;
            bill.note = "privet";
            bill.id_category = category.id;

            if (dataBase.addBill(bill))
            {
                nameEdit.Text = "";
                App._file = null;
                sumEdit.Text = "";
                photoView.SetImageBitmap(null);
                dateButton.Text = "";
                Snackbar.Make(MainActivity.drawerLayout, "Запись сохранена", Snackbar.LengthLong).Show();
            }
            else
            {
                Snackbar.Make(MainActivity.drawerLayout, "Сохранить запись не удалось", Snackbar.LengthLong).Show();
            }

        }


        public void showDatePickerDialog(object sender, EventArgs args)
        {
            dateDialog.Show(Activity.FragmentManager, "datePicker");
        }


    }


    public static class ObjectHelper
    {
        public static T Cast<T>(this Java.Lang.Object obj) where T : class
        {
            var propertyInfo = obj.GetType().GetProperty("Instance");
            return propertyInfo == null ? null : propertyInfo.GetValue(obj, null) as T;
        }
    }

    public static class BitmapHelpers
    {
        public static Bitmap rotateBitmapOrientation(string photoFilePath)
        {
            // Create and configure BitmapFactory
            BitmapFactory.Options bounds = new BitmapFactory.Options();
            bounds.InJustDecodeBounds = true;
            BitmapFactory.DecodeFile(photoFilePath, bounds);
            BitmapFactory.Options opts = new BitmapFactory.Options();
            Bitmap bm = BitmapFactory.DecodeFile(photoFilePath, opts);

            int rotationAngle = 90;

            // Rotate Bitmap
            Matrix matrix = new Matrix();
            matrix.SetRotate(rotationAngle, (float)bm.Width / 2, (float)bm.Height / 2);
            Bitmap rotatedBitmap = Bitmap.CreateBitmap(bm, 0, 0, bounds.OutWidth, bounds.OutHeight, matrix, true);
            // Return result
            return rotatedBitmap;
        }

        // Scale and maintain aspect ratio given a desired width
        public static Bitmap scaleToFitWidth(Bitmap b, int width)
        {
            float factor = width / (float)b.Width;
            return Bitmap.CreateScaledBitmap(b, width, (int)(b.Height * factor), true);
        }

        // Scale and maintain aspect ratio given a desired height
        public static Bitmap scaleToFitHeight(Bitmap b, int height)
        {
            if (height < 1) return null;

            float factor = height / (float)b.Height;
            return Bitmap.CreateScaledBitmap(b, (int)(b.Width * factor), height, true);
        }

    }
}