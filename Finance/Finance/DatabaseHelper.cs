using SQLite.Net;
using System;
using System.Collections.Generic;

namespace Common
{
    public class DatabaseHelper
    {
        // Database Info
        public static string DATABASE_NAME = "BillDatabase";
        private static int DATABASE_VERSION = 5;

            // Table Names
        private static string TABLE_BILL = "Bill";
        private static string TABLE_CATEGORY = "Category";

        // Bill Table Columns
        public static string KEY_BILL_ID = "_id";
        private static string KEY_BILL_CATEGORY_ID = "id_category";
        private static string KEY_BILL_NAME = "name";
        private static string KEY_BILL_PRICE = "price";
        private static string KEY_BILL_DATE = "date";
        private static string KEY_BILL_IMG_PATH = "imgPath";
        private static string KEY_BILL_NOTE = "note";

        // Category Table Columns
        public static string KEY_CATEGORY_ID = "_id";
        public static string KEY_CATEGORY_NAME = "name";

        public static SQLiteConnection database;
        private static DatabaseHelper sInstance;

        public DatabaseHelper(object DB)
        {
            database = (DB as ISQLite).GetConnection(DATABASE_NAME);
            database.CreateTable<Category>();
            database.CreateTable<Bill>();
            database.Commit();

            sInstance = this;
        }

        public static DatabaseHelper Instance
        {
           get { return sInstance; }
        }


        public bool addCategory(Category category)
        {
            database.BeginTransaction();
            try
            {            
                database.Insert(category);
                database.Commit();
                return true;
            }
            catch (Exception e)
            {
                database.Rollback();
            }
            return false;
        }


        public List<Category> getAllCategories()
        {
            List<Category> categories = new List<Category>();
            IEnumerator<Category> list = database.Table<Category>().GetEnumerator();

            try
            {
                if (list.MoveNext())
                {
                    do
                    {
                        categories.Add(list.Current);
                    }
                    while (list.MoveNext());
                }
            }
            catch (Exception e)
            {
            }

            return categories;
        }

        public IEnumerator<Category> getAllCategoriesEnumerator()
        {
            return database.Table<Category>().GetEnumerator();
        }


        public bool deleteCategory(long itemId)
        {
            database.BeginTransaction();
            try
            {
                database.Delete<Category>(itemId);

                var tmp = database.Table<Bill>().Where(i => i.id_category == itemId);
                for(int i = 0; i < tmp.Count(); i++)
                {
                    tmp.ElementAt(i).id_category = 0;
                }

                database.UpdateAll(tmp);
                database.Commit();
                return true;
            }
            catch (Exception e)
            {
                database.Rollback();
                return false;
            }
        }

        public bool editCategory(long Id, string Name)
        {
            database.BeginTransaction();
            try
            {
                database.Update(new Category {id = Id, name = Name });
                database.Commit();
                return true;
            }
            catch (Exception e)
            {
                database.Rollback();
            }
            return false;
        }

        public IEnumerator<StatisticByCategory> getStatisticsEnumerator(string order, long beginDate, long endDate)
        {
            string str = "SELECT " + TABLE_BILL + "." + KEY_BILL_ID + ", SUM(" + KEY_BILL_PRICE + ") as Total, " + TABLE_CATEGORY + "." + KEY_CATEGORY_NAME +
                    " FROM " + TABLE_BILL + ", " + TABLE_CATEGORY +
                    " WHERE " + TABLE_BILL + "." + KEY_BILL_CATEGORY_ID + " = " + TABLE_CATEGORY + "." + KEY_CATEGORY_ID +
                    " AND (" + TABLE_BILL + "." + KEY_BILL_DATE + " BETWEEN " + beginDate.ToString() + " AND " + endDate.ToString() + ")" +
                    " AND " + TABLE_BILL + "." + KEY_BILL_PRICE + " " + order + " 0" +
                    " GROUP BY " + TABLE_BILL + "." + KEY_BILL_CATEGORY_ID +
                    " ORDER BY Total";

            IEnumerator<StatisticByCategory> list = database.Query<StatisticByCategory>(str).GetEnumerator();

            return list;
        }

        public double getTotal(string order, long beginDate, long endDate)
        {
            string str = "SELECT " + TABLE_BILL + "." + KEY_BILL_ID + ", SUM(" + KEY_BILL_PRICE + ") as Total" +
                    " FROM " + TABLE_BILL +
                    " WHERE (" + TABLE_BILL + "." + KEY_BILL_DATE + " BETWEEN " + beginDate.ToString() + " AND " + endDate.ToString() + ")" +
                    " AND " + TABLE_BILL + "." + KEY_BILL_PRICE + " " + order + " 0";

            double value = database.Query<StatisticByCategory>(str)[0].Total;

            return value;
        }

        public bool addBill(Bill bill)
        {
            database.BeginTransaction();
            try
            {
                database.Insert(bill);
                database.Commit();
                return true;
            }
            catch (Exception e)
            {
                database.Rollback();
                return false;
            }
   
        }


        public List<string> getBillsName()
        {
            List<string> names = new List<string>();
            List<Bill> list = database.Query<Bill>("SELECT DISTINCT "+ KEY_BILL_NAME + " FROM " + TABLE_BILL);

            foreach(Bill item in list)
            {
                names.Add(item.name);
            }

            return names;
        }

        public List<string> getBillsGroups()
        {
            List<string> groups = new List<string>();

            // note используется для того чтобы получить дату в текстовом виде
            string BILL_SELECT_QUERY = string.Format("SELECT {0}, date FROM {1} GROUP BY {2} ORDER BY date DESC",
                    new string[] { "strftime('%d.%m.%Y', date / 1000, 'unixepoch') as note", TABLE_BILL, "note" });

            List<Bill> list = database.Query<Bill>(BILL_SELECT_QUERY);
            try
            {
                foreach (Bill item in list)
                {
                    groups.Add(item.note);
                }
            }
            catch (Exception e)
            {
            }

            return groups;
        }

        public List<Bill> getAllBills(string date)
        {
            string BILL_SELECT_QUERY = string.Format("SELECT *, {0} FROM {1} WHERE {2} = '{3}' ORDER BY date DESC",
            new string[] {"strftime('%d.%m.%Y', date / 1000, 'unixepoch') as dateBill", TABLE_BILL, "dateBill", date });

            List<Bill> bills = database.Query<Bill>(BILL_SELECT_QUERY);

            return bills;
        }


        public bool deleteBill(Bill bill)
        {
            database.BeginTransaction();
            try
            {
                database.Delete(bill);
                database.Commit();
                return true;
            }
            catch (Exception e)
            {
                database.Rollback();
                return false;
            }

        }

    }
}
