using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Support.V4.Widget;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using StockScany.Classes;
using System.Collections;
using Xamarin.Essentials;
using SQLite;
using System.Data;
using Android.Support.V4.View;
using Android.Support.V7.App;
using System.Threading.Tasks;
using System.IO;
using Android.Gms.Ads;

namespace StockScany.Fragments
{    
    public class WatchlistActivity : Android.Support.V4.App.Fragment, MethodCaller
    {
        static int iEnableAds = 1;
        string _databasePath = Xamarin.Essentials.FileSystem.AppDataDirectory + "/SqliteDatabase.db3";
        ListView listView;
        MethodCaller methodCaller;
        Button btnAdd;
        AutoCompleteTextView txtSearch;
        string app_unique_id;
        int account_id;
        int is_sub_active;
        int db_state = 0;
        string account_type;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);
            var view = inflater.Inflate(Resource.Layout.activity_watchlist, container, false);
            btnAdd = view.FindViewById<Button>(Resource.Id.btnAdd);
            listView = view.FindViewById<ListView>(Resource.Id.listView1);
            txtSearch = view.FindViewById<AutoCompleteTextView>(Resource.Id.txtSearch);

            if (iEnableAds == 1)
            {
                AdView adView = view.FindViewById<AdView>(Resource.Id.adView);
                var adRequest = new AdRequest.Builder().Build();
                adView.LoadAd(adRequest);
            }

            GetAccountDetails();

            Dialog progressDialog = CustomProgressDialog.Show(this.Context, "");
            Task upSec = UpdateNewSecurities();
            upSec.Wait();
            Task applySec = ApplySecurityToList();
            applySec.Wait();
            progressDialog.Dismiss();

            UpdateWatchList();
            ApplyWatchList();

            btnAdd.Click += (sender, args) => {
                if (txtSearch.Text.Length < 2)
                    return;
                btnAdd.Enabled = false;
                txtSearch.Enabled = false;
                //if (account_type == "P" || account_type == "R")
                {
                    int iSecId = AddToWatchlist(txtSearch.Text);
                    //Toast.MakeText(Application.Context, "pos: " + ((Button)sender).Tag.ToString(), ToastLength.Short).Show();
                    if (iSecId > 0)
                    {
                        UpdateSingleWL(txtSearch.Text, iSecId);
                        ApplyWatchList();
                    }
                }
                //else {
                //    DisplayAlert("Adding to Watchlist","You need to have a registered account to use the watchlist.");
                //}
                txtSearch.Enabled = true; ;
                btnAdd.Enabled = true;
            };

            return view;
        }        

        private void ReadCSVData()
        {
            var db = new SQLiteConnection(_databasePath);
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var resourceName = "StockScany.securities_csv.csv";
            Stream stream = assembly.GetManifestResourceStream(resourceName);
            StreamReader reader = new StreamReader(stream);
            if (reader != null)
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] svals = line.Split(",");
                    if (svals[0] != "id" && svals[0].Length > 0)
                    {
                        var newSec = new Securities();
                        newSec.sec_id = Int32.Parse(svals[0]);
                        newSec.symbol = svals[1];
                        newSec.db_state = Int32.Parse(svals[2]);
                        db.Insert(newSec);
                    }
                }
            }
        }

        private async Task UpdateNewSecurities()
        {
            var service = new net.azurewebsites.stockscanyweb.SSWebService();
            int sSecState = 0;

            var db = new SQLiteConnection(_databasePath);
            db.CreateTable<Securities>();
            //int c = db.Table<Securities>().Count();
            //if (db.Table<Securities>().Count() == 0)
            //{
            //    while(db.Table<Securities>().Count() == 0)
            //        System.Threading.Thread.Sleep(2000);
            //}

            var table = db.Query<Securities>("SELECT * FROM [Securities] WHERE db_state = (SELECT MAX(db_state) FROM [Securities]) ");
            if (table.Count > 0)
            {
                sSecState = table[0].db_state;
            }

            if (sSecState < db_state)
            {
                try
                {
                    DataTable dtSecs = service.GetSecurities(app_unique_id, account_id, sSecState);
                    foreach (DataRow dr in dtSecs.Rows)
                    {
                        var newSec = new Securities();
                        if ((int)dr["sec_id"] > 0)
                        {
                            newSec.sec_id = (int)dr["sec_id"];
                            newSec.symbol = (string)dr["symbol"];
                            newSec.db_state = (int)dr["state_id"];
                            db.Insert(newSec);
                        }
                    }
                }
                catch (Exception ex){}
            }
            db.Close();
        }

        private void GetAccountDetails()
        {
            try
            {
                var db = new SQLiteConnection(_databasePath);
                var table = db.Query<AccountInfo>("SELECT * FROM [AccountInfo] ");
                if (table.Count > 0)
                {
                    foreach (var s in table)
                    {
                        app_unique_id = s.unique_id;
                        account_id = s.account_id;
                        is_sub_active = s.is_active;
                        db_state = s.db_state;
                        account_type = s.account_type;
                    }
                }
                db.Close();
            }
            catch (Exception ex)
            { }            
        }

        private async Task<int> ApplySecurityToList()
        {
            var db = new SQLiteConnection(_databasePath);
            //add securities to list
            if (txtSearch.Adapter == null || txtSearch.Adapter.Count == 0)
            {
                var secs = db.Query<Securities>("SELECT distinct symbol FROM [Securities]");

                if (secs.Count > 0)
                {
                    List<string> sSymbols = new List<string>();
                    foreach (var s in secs)
                        sSymbols.Add(s.symbol);

                    ArrayAdapter adapter = new ArrayAdapter<string>(this.Context, Android.Resource.Layout.SimpleSpinnerItem, sSymbols);
                    txtSearch.Adapter = adapter;
                }
            }
            return 1;
        }

        public void UpdateWatchList()
        {
            var service = new net.azurewebsites.stockscanyweb.SSWebService();
            int appWatchlistState = Preferences.Get("watchlist_state", 0);            
            
            if (db_state == 0)
                GetAccountDetails();

            if (appWatchlistState == 0 || appWatchlistState < db_state)
            {
                var db = new SQLiteConnection(_databasePath);
                db.DropTable<WatchList>();
                db.CreateTable<WLIDS>();

                string sSec_ids = "";
                string sDb_state_ids = "";

                try
                {
                    if (db.Table<WLIDS>().Count() == 0)
                    {
                        //fetch from db 
                        if (account_type == "P" || account_type == "R" || account_type == "F")
                        {
                            DataTable dWL = service.GetSetWL(app_unique_id, account_id, 2, "0", "0");
                            if (dWL.Rows.Count > 0)
                            {
                                foreach (DataRow dr in dWL.Rows)
                                {
                                    var wl = new WLIDS();
                                    wl.sec_id = (int)dr["sec_id"];
                                    wl.db_state_id = (int)dr["state_id"];
                                    db.Insert(wl);
                                }
                            }
                        }
                    }

                    var sWLIDS = db.Query<WLIDS>("SELECT sec_id, db_state_id FROM [WLIDS]");
                    foreach (var s in sWLIDS)
                    {
                        sSec_ids = sSec_ids + "_" + s.sec_id.ToString();
                        sDb_state_ids = sDb_state_ids + "_" + s.db_state_id.ToString();
                    }

                    db.CreateTable<WatchList>();
                    if (sSec_ids.Length > 1 && (account_type == "P" || account_type == "R" || account_type == "F"))
                    {
                        if (account_id == 0)
                            GetAccountDetails();
                        DataTable dtWL = service.GetWLDelta(app_unique_id, account_id, sSec_ids, sDb_state_ids);
                        foreach (DataRow dr in dtWL.Rows)
                        {
                            var wlData = new WatchList();

                            wlData.sec_id = (int)dr["sec_id"];
                            wlData.symbol = (string)dr["symbol"];
                            wlData.event_date = (DateTime)dr["event_date"];
                            wlData.event_desc = (string)dr["event_desc"];
                            wlData.stock_price = float.Parse(((decimal)dr["last_p"]).ToString());
                            wlData.stock_price_per = float.Parse(((decimal)dr["last_p_chg"]).ToString());
                            wlData.event_type = (string)dr["event_type"];

                            db.Insert(wlData);
                        }
                    }
                    Preferences.Set("watchlist_state", db_state);
                }
                catch (Exception ex)
                {
                    DisplayAlert("WatchList", "There was some issue in the connection. Please try after some time.");
                    Preferences.Set("watchlist_state", 0);
                }
            }
        }

        public void UpdateSingleWL(string sSymbol, int iSecId)
        {
            var service = new net.azurewebsites.stockscanyweb.SSWebService();
            var db = new SQLiteConnection(_databasePath);

            if (db_state == 0)
                GetAccountDetails();

            try
            {
                DataTable dtWL = service.GetWLDelta(app_unique_id, account_id, "_" + iSecId.ToString(), "_" + db_state.ToString());
                foreach (DataRow dr in dtWL.Rows)
                {
                    var wlData = new WatchList();

                    wlData.sec_id = (int)dr["sec_id"];
                    wlData.symbol = (string)dr["symbol"];
                    wlData.event_date = (DateTime)dr["event_date"];
                    wlData.event_desc = (string)dr["event_desc"];
                    wlData.stock_price = float.Parse(((decimal)dr["last_p"]).ToString());
                    wlData.stock_price_per = float.Parse(((decimal)dr["last_p_chg"]).ToString());
                    wlData.event_type = (string)dr["event_type"];

                    db.Insert(wlData);
                }
            }
            catch (Exception e)
            {
                DisplayAlert("Adding to Watchlist", "There was some issue adding the security to watchlist.");
                Preferences.Set("watchlist_state", 0);
            }
            db.Close();
            Preferences.Set("watchlist_state", db_state);
        }

        public void ApplyWatchList()
        {
            var db = new SQLiteConnection(_databasePath);

            List<WatchListItem> sItemList = new List<WatchListItem>();
            WatchListItem sItem;

            float first_stock_price = 0;
            float last_stock_price = 0;
            float stock_price_per = 0;
            string sSymbol = "";

            db.CreateTable<WatchList>();
            if (db.Table<WatchList>().Count() > 0)
            {
                var sWLData = db.Query<WatchList>("SELECT DISTINCT sec_id FROM [WatchList]");
                string sWLString = "";
                List<string> event_desc;
                List<string> event_types;
                foreach (var ss in sWLData)
                {
                    event_desc = new List<string>();
                    event_types = new List<string>();
                    first_stock_price = 0;
                    last_stock_price = 0;

                    var table = db.Query<WatchList>("SELECT * FROM [WatchList] WHERE sec_id = ?", ss.sec_id);
                    foreach (var s in table)
                    {
                        if (first_stock_price == 0)
                            first_stock_price = s.stock_price;

                        if (s.event_desc == "Current Price")
                        {
                            stock_price_per = s.stock_price_per;
                            last_stock_price = s.stock_price;
                        }
                        else
                        {
                            event_desc.Add("> " + sWLString + s.event_date.ToString("dd/MM/yyyy") + "   " + s.stock_price.ToString() + "   " + s.event_desc);
                            event_types.Add(s.event_type);
                        }

                        sSymbol = s.symbol;
                    }
                    sItem = new WatchListItem();
                    sItem.sec_id = ss.sec_id;
                    sItem.symbol = sSymbol;
                    sItem.stock_price = last_stock_price;
                    sItem.stock_price_per = stock_price_per;
                    sItem.price_change = float.Parse(Decimal.Round(Decimal.Parse((last_stock_price - first_stock_price).ToString()),2).ToString());
                    stock_price_per = float.Parse(Decimal.Round(Decimal.Parse((100 * (last_stock_price - first_stock_price) / first_stock_price).ToString()), 2).ToString());  
                    sItem.percentage_change = float.Parse((Decimal.Round(Decimal.Parse(stock_price_per.ToString()), 2)).ToString());
                    sItem.event_desc = event_desc;
                    sItem.event_type = event_types;

                    sItemList.Add(sItem);
                }

                if (sItemList.Count > 0)
                {
                    WatchListAdapter sAdapter = new WatchListAdapter(this.Context, sItemList, methodCaller);
                    listView.Adapter = sAdapter;
                }
            }            
        }

        public int AddToWatchlist(string symbol)
        {
            var db = new SQLiteConnection(_databasePath);
            db.CreateTable<WLIDS>();
            if (db.Table<WLIDS>().Count() >= 30)
            {
                DisplayAlert("Adding to Watchlist", "Sorry, we currently have a limit of 30 stocks for the watchlist.");
                return 0;
            }

            if (db_state == 0)
                GetAccountDetails();

            if (string.IsNullOrEmpty(symbol))
                return 0;
            
            var table1 = db.Query<Securities>("SELECT sec_id FROM [Securities] WHERE symbol = ? ", symbol);
            int sec_id = 0;

            if (table1.Count > 0)
                foreach (var s in table1)
                    sec_id = s.sec_id;            
            
            var table = db.Query<WLIDS>("SELECT * FROM [WLIDS] WHERE sec_id = ? ", sec_id);
            if (table.Count > 0)
            {
                DisplayAlert("Adding to Watchlist", "The security is already in your watchlist");
                return -1;
            }
            else
            {
                var wlid = new WLIDS();
                wlid.sec_id = sec_id;
                wlid.db_state_id = db_state;
                db.Insert(wlid);
            }

            Preferences.Set("watchlist_state", 0);
            return sec_id;
        }         

        public void RemoveFromWatchlist(int sec_id, int position)
        {
            var db = new SQLiteConnection(_databasePath);

            //db.CreateTable<WatchList>();                       
            int iRows = db.Execute("DELETE FROM [WatchList] WHERE [sec_id] = ?", sec_id);

            //db.CreateTable<WLIDS>();
            iRows = db.Execute("DELETE FROM [WLIDS] WHERE [sec_id] = ?", sec_id);


        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            //update on WL list
            if (account_type == "P" || account_type == "R" || account_type == "F")
            {
                try
                {
                    var service = new net.azurewebsites.stockscanyweb.SSWebService();
                    var db = new SQLiteConnection(_databasePath);
                    string sSec_ids = "";
                    string sDb_state_ids = "";
                    var sWLIDS = db.Query<WLIDS>("SELECT sec_id, db_state_id FROM [WLIDS]");
                    foreach (var s in sWLIDS)
                    {
                        sSec_ids = sSec_ids + "_" + s.sec_id.ToString();
                        sDb_state_ids = sDb_state_ids + "_" + s.db_state_id.ToString();
                    }
                    DataTable dWL = service.GetSetWL(app_unique_id, account_id, 1, sSec_ids, sDb_state_ids);
                }
                catch (Exception ee) { }
            }
        }

        private void DisplayAlert(string sTitle, string sMessage)
        {
            Android.App.AlertDialog.Builder dialog = new Android.App.AlertDialog.Builder(this.Context);
            Android.App.AlertDialog alert = dialog.Create();
            alert.SetTitle(sTitle);
            alert.SetMessage(sMessage);
            alert.SetButton("OK", (c, ev) =>
            {
                alert.Cancel();
            });
            alert.Show();
        }
    }
}