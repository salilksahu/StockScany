using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using MikePhil.Charting.Charts;
using System.Drawing;
using System.Collections;
using Xamarin.Essentials;
using System.Data;
using SQLite;
using StockScany.Classes;
using StockScany.Adapters;
using Android.Graphics;
using Android.Gms.Ads;
using System.Threading.Tasks;
using System.IO;
using MikePhil.Charting.Data;

namespace StockScany.Fragments
{
    public class WelcomeFragment : Android.Support.V4.App.Fragment
    {
        static int iEnableAds = 1;
        const int iMandatoryVerUpdate = 2000;
        TextView txtSensex;
        TextView txtSensexPer;
        TextView txtNifty;
        TextView txtNiftyPer;
        TextView txtBullScan;
        TextView txtBearScan;
        LineChart niftyChart;
        HorizontalBarChart indiceChart;
        ListView lvIndex;
        TextView txtLastUpdate;

        TextView lblSensex;
        TextView lblNifty;

        string sNewInstall = "Y";
        string app_unique_id;
        int account_id;
        string _databasePath = Xamarin.Essentials.FileSystem.AppDataDirectory + "/SqliteDatabase.db3";
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here            
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);

            var view = inflater.Inflate(Resource.Layout.activity_welcome, container, false);

            txtLastUpdate = view.FindViewById<TextView>(Resource.Id.txtLastUpdate);
            CheckDataState();
            
            txtSensex = view.FindViewById<TextView>(Resource.Id.txtSensex);
            txtSensexPer = view.FindViewById<TextView>(Resource.Id.txtSensexPer);
            txtNifty = view.FindViewById<TextView>(Resource.Id.txtNifty);
            txtNiftyPer = view.FindViewById<TextView>(Resource.Id.txtNiftyPer);
            txtBullScan = view.FindViewById<TextView>(Resource.Id.txtBullScan);
            txtBearScan = view.FindViewById<TextView>(Resource.Id.txtBearScan);
            niftyChart = view.FindViewById<LineChart>(Resource.Id.niftyChart);
            //lvIndex = view.FindViewById<ListView>(Resource.Id.lvIndex);
            lblSensex = view.FindViewById<TextView>(Resource.Id.lblSensex);
            lblNifty = view.FindViewById<TextView>(Resource.Id.lblNifty);
            indiceChart = view.FindViewById<HorizontalBarChart>(Resource.Id.indiceChart);

            Typeface tf = Typeface.CreateFromAsset(this.Context.Assets, "orbitron.ttf");
            Typeface tf2 = Typeface.CreateFromAsset(this.Context.Assets, "barlow.ttf");            
            lblSensex.Typeface = tf;
            lblNifty.Typeface = tf;
            txtSensex.Typeface = tf2;
            txtNifty.Typeface = tf2;

            if (iEnableAds == 1)
            {
                AdView adView = view.FindViewById<AdView>(Resource.Id.adView);
                var adRequest = new AdRequest.Builder().Build();
                adView.LoadAd(adRequest);

                AdView adView2 = view.FindViewById<AdView>(Resource.Id.adView2);
                var adRequest2 = new AdRequest.Builder().Build();
                adView2.LoadAd(adRequest2);
            }

            GetWelcomeInfo();
            //LoadIndexList();

            //var db = new SQLiteConnection(_databasePath);
            //db.DropTable<Securities>();

            Task.Run(async () =>
            {
                //can use param from command if needed
                GetOnetimeSecs();                
            }).ConfigureAwait(false);

            if (sNewInstall == "Y")
            {
                Dialog popupWelcome = new Dialog(this.Context);
                popupWelcome.SetContentView(Resource.Layout.activity_firstload);
                popupWelcome.Window.SetSoftInputMode(SoftInput.AdjustResize);
                popupWelcome.Show();
                popupWelcome.FindViewById<TextView>(Resource.Id.txtHeader).Text = "Welcome to StockScany";
                popupWelcome.FindViewById<TextView>(Resource.Id.txtBody).Text = GetString(Resource.String.welcome);
            }

            LogInfo();
            return view;
        }

        private async Task<int> GetOnetimeSecs()
        {
            var db = new SQLiteConnection(_databasePath);
            db.CreateTable<Securities>();
            if (db.Table<Securities>().Count() == 0)
            {
                var assembly = System.Reflection.Assembly.GetExecutingAssembly();
                var resourceName = "StockScany.securities_csv.csv";
                Stream stream = assembly.GetManifestResourceStream(resourceName);                
                await ReadCSVDataAsync(stream, _databasePath);                
                return 1;
            }
            return 0;
        }

        static async Task ReadCSVDataAsync(Stream stream, string _dbPath)
        {
            Char[] buffer;

            using (var sr = new StreamReader(stream))
            {
                buffer = new Char[(int)sr.BaseStream.Length];
                await sr.ReadAsync(buffer, 0, (int)sr.BaseStream.Length);
            }

            String sVal = new String(buffer);
            String[] sValues = sVal.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            var db = new SQLiteConnection(_dbPath);
            foreach (string s in sValues)
            {
                string[] sSec = s.Split(",", StringSplitOptions.RemoveEmptyEntries);
                if (sSec[0] != "id" && sSec[0].Length > 0)
                {
                    var newSec = new Securities();
                    newSec.sec_id = Int32.Parse(sSec[0]);
                    newSec.symbol = sSec[1];
                    newSec.db_state = Int32.Parse(sSec[2]);
                    db.Insert(newSec);
                }
            }
            db.Close();
        }

        private void LogInfo()
        {
            var service = new net.azurewebsites.stockscanyweb.SSWebService();
            if (account_id == 0)
                GetAccountDetails();
            service.LoggedUpdateAsync(app_unique_id, account_id, DateTime.Now);
        }

        private void GetAccountDetails()
        {
            var db = new SQLiteConnection(_databasePath);
            var table = db.Query<AccountInfo>("SELECT * FROM [AccountInfo] ");
            if (table.Count > 0)
            {                
                foreach (var s in table)
                {
                    app_unique_id = s.unique_id;
                    account_id = s.account_id;
                }
            }
            db.Close();
        }        

        private void CheckDataState()
        {
            try
            {
                var service = new net.azurewebsites.stockscanyweb.SSWebService();
                int dbDataState = 0;
                DataTable dtGenInfo;

                //Check account info
                var db = new SQLiteConnection(_databasePath);
                db.CreateTable<AccountInfo>();                

                var table = db.Query<AccountInfo>("SELECT * FROM [AccountInfo] ");
                if (table.Count == 0)
                {
                    sNewInstall = "Y";
                    app_unique_id = Preferences.Get("app_unique_id", string.Empty);
                    if (string.IsNullOrEmpty(app_unique_id))
                    {
                        app_unique_id = System.Guid.NewGuid().ToString();
                        Preferences.Set("app_unique_id", app_unique_id);
                    }
                    //get the current data state and account information
                    dtGenInfo = service.GetGeneralInfo(app_unique_id, 0);
                    var account = new AccountInfo();
                    account.account_id = (int)dtGenInfo.Rows[0]["ID"];
                    account.unique_id = app_unique_id;
                    account.name = (string)dtGenInfo.Rows[0]["NAME"];
                    if (account.name == "*")
                        account.name = String.Empty;
                    account.email = (string)dtGenInfo.Rows[0]["EMAIL"];
                    if (account.email == "*")
                        account.email = String.Empty;
                    account.account_type = (string)dtGenInfo.Rows[0]["ACCOUNT_TYPE"];
                    account.sub_end_date = (string)dtGenInfo.Rows[0]["SUB_END_DATE"];
                    account.is_active = (int)dtGenInfo.Rows[0]["SUB_ACTIVE"];
                    account.db_state = (int)dtGenInfo.Rows[0]["DB_STATE"];
                    account.update_date = DateTime.Now;

                    //set update date
                    DateTime zone1 = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 17, 00, 00);
                    DateTime zone2 = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 20, 00, 00);
                    if(account.update_date >= zone1 && account.update_date <= zone2)
                        account.update_date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 09, 00, 00);

                    db.Insert(account);
                    Preferences.Set("account_id", account.account_id);
                    Preferences.Set("sub_active", account.is_active);
                    dbDataState = account.db_state;
                    if(account.db_state >= iMandatoryVerUpdate)
                    {
                        txtLastUpdate.SetTextColor(Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Red));
                        txtLastUpdate.Text = "Please update the app from play store. The current version is not supported anymore and won't get the stock updates.";
                    }
                    else
                        txtLastUpdate.Text = "Last Updated: " + account.update_date.ToShortDateString() + " " + account.update_date.ToShortTimeString();
                }
                else
                {
                    sNewInstall = "N";
                    foreach (var s in table)
                    {
                        //check the datastate update time
                        int prevDbState = 0;
                        DateTime dUpdatedate = DateTime.Now;
                        var dtable = db.Query<AccountInfo>("SELECT * FROM [AccountInfo] ");
                        foreach (var ss in dtable)
                        {
                            dUpdatedate = ss.update_date;
                            prevDbState = ss.db_state;
                        }

                        DateTime currentUpdate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 17, 00, 00);
                        DateTime prevUpdate = currentUpdate.AddDays(-1);
                        if (currentUpdate.DayOfWeek == DayOfWeek.Saturday)
                        {
                            currentUpdate = currentUpdate.AddDays(-1);
                            prevUpdate = prevUpdate.AddDays(-1);
                        }
                        if (currentUpdate.DayOfWeek == DayOfWeek.Sunday)
                        {
                            currentUpdate = currentUpdate.AddDays(-2);
                            prevUpdate = prevUpdate.AddDays(-2);
                        }

                        if (dUpdatedate < prevUpdate || (DateTime.Now > currentUpdate && dUpdatedate < currentUpdate))
                        {
                            dtGenInfo = service.GetGeneralInfo(s.unique_id, s.account_id);
                            Preferences.Set("app_unique_id", s.unique_id);
                            Preferences.Set("account_id", s.account_id);
                            Preferences.Set("sub_active", (int)dtGenInfo.Rows[0]["SUB_ACTIVE"]);
                            dbDataState = (int)dtGenInfo.Rows[0]["DB_STATE"];

                            if (dbDataState < iMandatoryVerUpdate)
                            {
                                DateTime dUpdDateCheck = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 09, 00, 00);
                                if (prevDbState < dbDataState || (prevDbState == dbDataState && dUpdatedate == dUpdDateCheck))
                                {
                                    SQLiteCommand cmd = new SQLiteCommand(new SQLiteConnection(_databasePath));
                                    cmd.CommandText = "UPDATE [AccountInfo] SET account_type = @a, sub_end_date = @s, db_state = @d, update_date = @u ";
                                    cmd.Bind("@a", (string)dtGenInfo.Rows[0]["ACCOUNT_TYPE"]);
                                    cmd.Bind("@s", (string)dtGenInfo.Rows[0]["SUB_END_DATE"]);
                                    cmd.Bind("@id", (int)dtGenInfo.Rows[0]["ID"]);
                                    cmd.Bind("@d", (int)dtGenInfo.Rows[0]["DB_STATE"]);
                                    cmd.Bind("@u", DateTime.Now);
                                    cmd.ExecuteNonQuery();
                                    txtLastUpdate.Text = "Last Updated: " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString();
                                }
                                else
                                    txtLastUpdate.Text = "Last Updated: " + dUpdatedate.ToShortDateString() + " " + dUpdatedate.ToShortTimeString();
                            }
                            else
                            {
                                txtLastUpdate.SetTextColor(Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Red));
                                txtLastUpdate.Text = "Please update the app from play store. The current version is not supported anymore and won't get the stock updates.";
                            }
                        }
                        else
                        {
                            if (prevDbState >= iMandatoryVerUpdate)
                            {
                                txtLastUpdate.SetTextColor(Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Red));
                                txtLastUpdate.Text = "Please update the app from play store. The current version is not supported anymore and won't get the stock updates.";
                            }
                            else
                                txtLastUpdate.Text = "Last Updated: " + dUpdatedate.ToShortDateString() + " " + dUpdatedate.ToShortTimeString();
                        }
                    }
                }

                int appWelcomeState = (int)Preferences.Get("welcome_state", 0);
                if (appWelcomeState == 0 || appWelcomeState < dbDataState)
                {
                    //truncate all records or delete tables                    
                    db.DropTable<Indices>();
                    Preferences.Set("welcome_state", dbDataState);
                }
                db.Close();
            }
            catch (Exception e) {
                Preferences.Set("welcome_state", 0);
            }
        }

        private void GetWelcomeInfo()
        {
            var service = new net.azurewebsites.stockscanyweb.SSWebService();
            var db = new SQLiteConnection(_databasePath);
            //db.CreateTable<WelcomeInfo>();
            db.CreateTable<Indices>();
            if (db.Table<Indices>().Count() == 0)
            {
                if (account_id == 0)
                    GetAccountDetails();
                try
                {
                    DataTable dtScans = service.GetWelcomeInfo(app_unique_id, account_id);
                    int iSeq = 0;
                    foreach (DataRow dr in dtScans.Rows)
                    {
                        string sIndex = (string)dr["indice"];
                        if (sIndex != "*" && sIndex.Length > 2)
                        {
                            var ind = new Indices();
                            ind.indice = sIndex;
                            ind.price = (decimal)dr["price"];
                            ind.change = (decimal)dr["change"];
                            ind.per_change = (decimal)dr["per_change"];
                            ind.seq = (int)dr["seq"];
                            db.Insert(ind);
                        }
                    }
                }
                catch (Exception ex)
                {
                    DisplayAlert("Getting Information", "There was some issue in the connection. Please check your internet connection.");
                }
            }

            var sSensexData = db.Query<Indices>("SELECT * FROM [Indices] WHERE indice = ? and seq = -1 ", "SENSEX");
            foreach (var s in sSensexData)
            {
                txtSensex.Text = s.price.ToString();
                if (s.per_change < 0)
                    txtSensexPer.SetTextColor(Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Red));
                else
                    txtSensexPer.SetTextColor(Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.DarkGreen));
                txtSensexPer.Text = "[" + s.per_change.ToString() + " %" + "]";
            }
            var sNiftyData = db.Query<Indices>("SELECT * FROM [Indices] WHERE indice = ? and seq = -1 ", "NIFTY");
            foreach (var s in sNiftyData)
            {
                txtNifty.Text = s.price.ToString();
                if (s.per_change < 0)
                    txtNiftyPer.SetTextColor(Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Red));
                else
                    txtNiftyPer.SetTextColor(Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.DarkGreen));
                txtNiftyPer.Text = "[" + s.per_change.ToString() + " %" + "]";
            }
            var sScanInfo = db.Query<Indices>("SELECT * FROM [Indices] WHERE indice = ? and seq = -1 ", "SCAN");
            foreach (var s in sScanInfo)
            {
                txtBullScan.Text = " " + Int32.Parse(s.change.ToString()).ToString() + " ";
                txtBearScan.Text = " " + Int32.Parse(s.per_change.ToString()).ToString() + " ";
            }

            //nse chart
            MikePhil.Charting.Components.Description desc4 = new MikePhil.Charting.Components.Description();
            desc4.Text = "Nifty";
            desc4.TextSize = 12;
            niftyChart.Description = desc4;
            MikePhil.Charting.Components.YAxis leftAxis = niftyChart.AxisLeft;
            MikePhil.Charting.Components.YAxis rightAxis = niftyChart.AxisRight;
            leftAxis.SetDrawGridLines(false);
            leftAxis.SetDrawLabels(false);
            rightAxis.SetDrawGridLines(false);
            rightAxis.TextColor = Android.Graphics.Color.Black.ToArgb();
            MikePhil.Charting.Components.XAxis xAxis = niftyChart.XAxis;
            xAxis.SetDrawGridLines(false);// disable x axis grid lines
            xAxis.SetDrawLabels(false);
            xAxis.SetDrawAxisLine(true);
            xAxis.Granularity = 1.5f;
            MikePhil.Charting.Components.Legend l2 = niftyChart.Legend;
            l2.Enabled = false;

            IList<MikePhil.Charting.Data.Entry> priceEntry = new List<MikePhil.Charting.Data.Entry>();
            var niftyData = db.Query<Indices>("SELECT * FROM [Indices] WHERE indice = ? and seq != -1 ", "NIFTY");

            foreach (var s in niftyData)
            {
                priceEntry.Add(new Entry(s.seq, float.Parse(s.price.ToString())));                
            }

            LineDataSet priceSet = new LineDataSet(priceEntry, "price");

            priceSet.Color = Android.Graphics.Color.Honeydew.ToArgb();
            priceSet.SetDrawValues(false);
            priceSet.SetDrawFilled(true);
            priceSet.SetDrawCircles(true);
            priceSet.CircleRadius = 1f;

            LineData lPricedata = new LineData();
            lPricedata.AddDataSet(priceSet);
            // set data
            niftyChart.Data = lPricedata;
            niftyChart.Invalidate();


            //indice chart
            MikePhil.Charting.Components.Description desc5 = new MikePhil.Charting.Components.Description();
            desc5.Text = "";
            desc5.TextSize = 12;
            indiceChart.Description = desc5;
            MikePhil.Charting.Components.YAxis leftAxis1 = indiceChart.AxisLeft;
            leftAxis1.SetDrawGridLines(false);
            leftAxis1.SetDrawLabels(false);
            leftAxis1.SetDrawAxisLine(false);
            MikePhil.Charting.Components.YAxis rightAxis1 = indiceChart.AxisRight;            
            rightAxis1.SetDrawGridLines(false);
            rightAxis1.SetDrawLabels(false);
            rightAxis1.SetDrawAxisLine(false);
            MikePhil.Charting.Components.XAxis xAxis1 = indiceChart.XAxis;
            xAxis1.SetDrawGridLines(false);// disable x axis grid lines
            xAxis1.SetDrawLabels(true);
            xAxis1.SetDrawAxisLine(false);
            xAxis1.Position = MikePhil.Charting.Components.XAxis.XAxisPosition.Bottom;
            xAxis1.Enabled = true;
            
            MikePhil.Charting.Components.Legend l3 = indiceChart.Legend;
            l3.Enabled = false;

            IList<MikePhil.Charting.Data.BarEntry> bEntryList = new List<MikePhil.Charting.Data.BarEntry>();
            var iData = db.Query<Indices>("SELECT * FROM [Indices]");

            int i = 0;
            int[] iColors = new int[8];
            string[] sLabels = new string[8];
            
            foreach (var s in iData)
            {
                if (s.indice != "NIFTY" && s.indice != "SENSEX" && s.indice != "SCAN" && i <= 7)
                {
                    BarEntry barEntry = new BarEntry(i, float.Parse(s.per_change.ToString()));                    
                    bEntryList.Add(barEntry);
                    sLabels[i] = s.indice;                    
                    if (float.Parse(s.per_change.ToString()) >= 0)
                        iColors[i] = Android.Graphics.Color.Green.ToArgb();
                    else
                        iColors[i] = Android.Graphics.Color.Red.ToArgb();
                    i += 1;
                }
            }
            BarDataSet barSet = new BarDataSet(bEntryList, "price");
            barSet.SetColors(iColors);
            barSet.SetStackLabels(sLabels);
            MikePhil.Charting.Formatter.IndexAxisValueFormatter ivals = new MikePhil.Charting.Formatter.IndexAxisValueFormatter(sLabels);
            xAxis1.ValueFormatter = ivals;

            BarData barData = new BarData();
            barData.AddDataSet(barSet);
            // set data
            indiceChart.Data = barData;
            indiceChart.Invalidate();
        }

        private void LoadIndexList()
        {
            List<IndexItem> items = new List<IndexItem>();
            IndexItem item;
            var db = new SQLiteConnection(_databasePath);
            var sIndexData = db.Query<Indices>("SELECT * FROM [Indices] ");
            item = new IndexItem();
            item.indice = "Index";
            item.price = "Price";
            item.change = "Change";
            item.per_change = "% Change";
            items.Add(item);
            foreach (var s in sIndexData)
            {                
                if (s.indice != "NIFTY" && s.indice != "SENSEX" && s.indice != "SCAN")
                {
                    item = new IndexItem();
                    item.indice = s.indice;
                    item.price = s.price.ToString();
                    item.change = s.change.ToString();
                    item.per_change = s.per_change.ToString();
                    items.Add(item);
                }
            }
            if (items.Count > 1)
            {
                IndexAdapter indexAdapter = new IndexAdapter(this.Context, items);
                lvIndex.Adapter = indexAdapter;
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            Dialog progressDialog = CustomProgressDialog.Show(this.Context, "Running one time securities load..");
            string sMsg = "database is locked";
            while (sMsg == "database is locked")
            {
                //check db locaked
                try
                {
                    var db = new SQLiteConnection(_databasePath);
                    if (db.Table<Securities>().Count() > 0)
                        sMsg = "*";
                    else
                    {
                        System.Threading.Thread.Sleep(1500);
                        sMsg = "database is locked";
                    }

                }
                catch (Exception ex)
                {
                    sMsg = ex.Message;
                    System.Threading.Thread.Sleep(1500);
                }
            }
            progressDialog.Dismiss();
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