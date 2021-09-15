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
using System.Data;
using StockScany.Classes;
using System.Collections;
using Xamarin.Essentials;
using SQLite;
using Android.Gms.Ads;

namespace StockScany.Fragments
{

    public class BasicScans : Android.Support.V4.App.Fragment
    {
        string _databasePath = Xamarin.Essentials.FileSystem.AppDataDirectory + "/SqliteDatabase.db3";
        static int iEnableAds = 1;

        string app_unique_id;
        int account_id;
        int is_sub_active;
        int db_state;

        RelativeLayout rlBULLENG;
        RelativeLayout rlBEARENG;
        RelativeLayout rl3WHITESOL;
        RelativeLayout rl3BLACKCRO;
        RelativeLayout rlMORNSTAR;
        RelativeLayout rlEVENSTAR;
        RelativeLayout rlPIERCING;
        RelativeLayout rlDARKCLOUD;
        RelativeLayout rl3INSIDEUP;
        RelativeLayout rl3INSIDEDO;
        RelativeLayout rl3OUTSIDEUP;
        RelativeLayout rl3OUTSIDEDO;
        RelativeLayout rlHARAMIBULL;
        RelativeLayout rlHARAMIBEAR;

        TextView txtBULLENG;
        TextView txtBEARENG;
        TextView txt3WHITESOL;
        TextView txt3BLACKCRO;
        TextView txtMORNSTAR;
        TextView txtEVENSTAR;
        TextView txtPIERCING;
        TextView txtDARKCLOUD;
        TextView txt3INSIDEUP;
        TextView txt3INSIDEDO;
        TextView txt3OUTSIDEUP;
        TextView txt3OUTSIDEDO;
        TextView txtHARAMIBULL;
        TextView txtHARAMIBEAR;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);            
            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            var view = inflater.Inflate(Resource.Layout.activity_basic, container, false);

            rlBULLENG = view.FindViewById<RelativeLayout>(Resource.Id.rlBULLENG);
            rlBEARENG = view.FindViewById<RelativeLayout>(Resource.Id.rlBEARENG);
            rl3WHITESOL = view.FindViewById<RelativeLayout>(Resource.Id.rl3WHITESOL);
            rl3BLACKCRO = view.FindViewById<RelativeLayout>(Resource.Id.rl3BLACKCRO);
            rlMORNSTAR = view.FindViewById<RelativeLayout>(Resource.Id.rlMORNSTAR);
            rlEVENSTAR = view.FindViewById<RelativeLayout>(Resource.Id.rlEVENSTAR);
            rlPIERCING = view.FindViewById<RelativeLayout>(Resource.Id.rlPIERCING);
            rlDARKCLOUD = view.FindViewById<RelativeLayout>(Resource.Id.rlDARKCLOUD);
            rl3INSIDEUP = view.FindViewById<RelativeLayout>(Resource.Id.rl3INSIDEUP);
            rl3INSIDEDO = view.FindViewById<RelativeLayout>(Resource.Id.rl3INSIDEDO);
            rl3OUTSIDEUP = view.FindViewById<RelativeLayout>(Resource.Id.rl3OUTSIDEUP);
            rl3OUTSIDEDO = view.FindViewById<RelativeLayout>(Resource.Id.rl3OUTSIDEDO);
            rlHARAMIBULL = view.FindViewById<RelativeLayout>(Resource.Id.rlHARAMIBULL);
            rlHARAMIBEAR = view.FindViewById<RelativeLayout>(Resource.Id.rlHARAMIBEAR);

            txtBULLENG = view.FindViewById<TextView>(Resource.Id.txtBULLENG);
            txtBEARENG = view.FindViewById<TextView>(Resource.Id.txtBEARENG);
            txt3WHITESOL = view.FindViewById<TextView>(Resource.Id.txt3WHITESOL);
            txt3BLACKCRO = view.FindViewById<TextView>(Resource.Id.txt3BLACKCRO);
            txtMORNSTAR = view.FindViewById<TextView>(Resource.Id.txtMORNSTAR);
            txtEVENSTAR = view.FindViewById<TextView>(Resource.Id.txtEVENSTAR);
            txtPIERCING = view.FindViewById<TextView>(Resource.Id.txtPIERCING);
            txtDARKCLOUD = view.FindViewById<TextView>(Resource.Id.txtDARKCLOUD);
            txt3INSIDEUP = view.FindViewById<TextView>(Resource.Id.txt3INSIDEUP);
            txt3INSIDEDO = view.FindViewById<TextView>(Resource.Id.txt3INSIDEDO);
            txt3OUTSIDEUP = view.FindViewById<TextView>(Resource.Id.txt3OUTSIDEUP);
            txt3OUTSIDEDO = view.FindViewById<TextView>(Resource.Id.txt3OUTSIDEDO);
            txtHARAMIBULL = view.FindViewById<TextView>(Resource.Id.txtHARAMIBULL);
            txtHARAMIBEAR = view.FindViewById<TextView>(Resource.Id.txtHARAMIBEAR);

            if (iEnableAds == 1)
            {
                AdView adView = view.FindViewById<AdView>(Resource.Id.adView);
                var adRequest = new AdRequest.Builder().Build();
                adView.LoadAd(adRequest);
            }

            string sDesc = "";
            rlBULLENG.Click += delegate {
                sDesc = "The pattern consists of a down candlestick followed by a large up candlestick that eclipses or engulfs the smaller down candle.";
                sDesc = sDesc + "We are showing the ones on a downward price trend to show a possible reversal.";
                PopulateStockItems("BULLENG", "Bullish Engulf", sDesc);                
            };
            rlBEARENG.Click += delegate {
                sDesc = "The pattern consists of an up candlestick followed by a large down candlestick that eclipses or engulfs the smaller up candle.";
                sDesc = sDesc + "We are showing the ones on a upward price trend to show a possible reversal.";
                PopulateStockItems("BEARENG", "Bearish Engulf", sDesc);
            };
            rl3WHITESOL.Click += delegate {
                sDesc = "The pattern consists of three consecutive long-bodied candlesticks that open within the previous candle's real body and a close that exceeds or nears the previous candle's high.";
                sDesc = sDesc + "We are showing the ones on a downward price trend to show a possible reversal.";
                PopulateStockItems("3WHITESOL", "Three White Soldiers", sDesc);
            };
            rl3BLACKCRO.Click += delegate {
                sDesc = "The pattern consists of three consecutive long-bodied candlesticks that open within the previous candle's real body and a close that exceeds or nears the previous candle's low.";
                sDesc = sDesc + "We are showing the ones on a upward price trend to show a possible reversal.";
                PopulateStockItems("3BLACKCRO", "Three Black Crows", sDesc);
            };
            rlMORNSTAR.Click += delegate {
                sDesc = "This pattern is made up of a tall down candlestick, a smaller up or down candlestick with a short body and above the previous candle, and a third tall up candlestick.";
                sDesc = sDesc + "We are showing the ones on a downward price trend to show a possible reversal.";
                PopulateStockItems("MORNSTAR", "Morning Star", sDesc);
            };
            rlEVENSTAR.Click += delegate {
                sDesc = "This pattern is made up of a tall up candlestick, a smaller up or down candlestick with a short body and above the previous candle, and a third tall down candlestick.";
                sDesc = sDesc + "We are showing the ones on a upward price trend to show a possible reversal.";
                PopulateStockItems("EVENSTAR", "Evening Star", sDesc);
            };
            rlPIERCING.Click += delegate {
                sDesc = "This pattern is made by an up candle that opens below the close of the prior down candle, and then closes above the midpoint of the down candle.";
                sDesc = sDesc + "We are showing the ones on a downward price trend to show a possible reversal.";
                PopulateStockItems("PIERCING", "Piercing Line", sDesc);
            };
            rlDARKCLOUD.Click += delegate {
                sDesc = "This pattern is made by a down candle that opens above the close of the prior up candle, and then closes below the midpoint of the up candle.";
                sDesc = sDesc + "We are showing the ones on a upward price trend to show a possible reversal.";
                PopulateStockItems("DARKCLOUD", "Dark Cloud Cover", sDesc);
            };
            rl3INSIDEUP.Click += delegate {
                sDesc = "This pattern is made by a large down candle, a smaller up candle contained within the prior candle, and then another up candle that closes above the close of the second candle.";
                sDesc = sDesc + "We are showing the ones on a downward price trend to show a possible reversal.";
                PopulateStockItems("3INSIDEUP", "Three Inside Up", sDesc);
            };
            rl3INSIDEDO.Click += delegate {
                sDesc = "This pattern is made by a large up candle, a smaller down candle contained within the prior candle, then another down candle that closes below the close of the second candle.";
                sDesc = sDesc + "We are showing the ones on a upward price trend to show a possible reversal.";
                PopulateStockItems("3INSIDEDO", "Three Inside Down", sDesc);
            };
            rl3OUTSIDEUP.Click += delegate {
                sDesc = "This pattern is made by a down candle, then an up candle that fully contains the first candle, and then another up candle with a higher close than the second candle.";
                sDesc = sDesc + "We are showing the ones on a downward price trend to show a possible reversal.";               
                PopulateStockItems("3OUTSIDEUP", "Three Outside Up", sDesc);
            };
            rl3OUTSIDEDO.Click += delegate {
                sDesc = "This pattern is made by an up candle, then a down candle that fully contains the first candle, and then another down candle with a lower close than the second candle.";
                sDesc = sDesc + "We are showing the ones on a upward price trend to show a possible reversal.";
                PopulateStockItems("3OUTSIDEDO", "Three Outside Down", sDesc);
            };
            rlHARAMIBULL.Click += delegate {
                sDesc = "This pattern occurs when there is a large down candle on first day followed by a smaller up candle within the body of the first candle.";
                sDesc = sDesc + "We are showing the ones on a downward price trend to show a possible reversal.";
                PopulateStockItems("HARAMIBULL", "Harami Bullish", sDesc);
            };
            rlHARAMIBEAR.Click += delegate {
                sDesc = "This pattern occurs when there is a large up candle on first day followed by a smaller down candle within the body of the first candle.";
                sDesc = sDesc + "We are showing the ones on a upward price trend to show a possible reversal.";
                PopulateStockItems("HARAMIBEAR", "Harami Bearish", sDesc);
            };

            CheckScanState();
            UpdateScanStockCounts();
            
            return view;
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
                    is_sub_active = s.is_active;
                    db_state = s.db_state;
                }
            }
            db.Close();
        }

        private void CheckScanState()
        {
            int appScanState = (int)Preferences.Get("scans_state", 0);

            if (db_state == 0)
                GetAccountDetails();

            if (appScanState == 0 || appScanState < db_state)
            {
                //truncate all records or delete tables
                string _databasePath = Xamarin.Essentials.FileSystem.AppDataDirectory + "/SqliteDatabase.db3";
                var db = new SQLiteConnection(_databasePath);
                db.DropTable<ScanData>();
                db.DropTable<StockData>();
                GetScansInfo();
                Preferences.Set("scans_state", db_state);
            }            
        }

        private void GetScansInfo()
        {
            var service = new net.azurewebsites.stockscanyweb.SSWebService();            
            var db = new SQLiteConnection(_databasePath);
            db.CreateTable<ScanData>();
            if (db.Table<ScanData>().Count() == 0)
            {
                if (account_id == 0)
                    GetAccountDetails();
                try
                {
                    DataTable dtScans = service.GetAllScans(app_unique_id, account_id, is_sub_active);

                    foreach (DataRow dr in dtScans.Rows)
                    {
                        var scanData = new ScanData();

                        if ((int)dr["id"] > 0)
                        {
                            scanData.scan_id = (string)dr["scan_id"];
                            scanData.sec_id = (int)dr["sec_id"];
                            scanData.symbol = (string)dr["symbol"];
                            scanData.high_p = float.Parse(((decimal)dr["high_p"]).ToString());
                            scanData.low_p = float.Parse(((decimal)dr["low_p"]).ToString());
                            scanData.open_p = float.Parse(((decimal)dr["open_p"]).ToString());
                            scanData.close_p = float.Parse(((decimal)dr["close_p"]).ToString());
                            scanData.stock_price = float.Parse(((decimal)dr["last_p"]).ToString());
                            scanData.stock_price_per = float.Parse(((decimal)dr["price_chg"]).ToString());
                            scanData.trade_value = float.Parse(((decimal)dr["tottrdval"]).ToString());
                            scanData.scan_date = (DateTime)dr["scan_date"];
                            scanData.event_desc = (string)dr["event_desc"];
                            db.Insert(scanData);
                        }
                    }
                }
                catch (Exception ee)
                { DisplayAlert("Scans", "There was some issue in the connection. Please try after some time."); }
            }
        }

        private void UpdateScanStockCounts()
        {
            var db = new SQLiteConnection(_databasePath);
            txtBULLENG.Text = "0";
            txtBEARENG.Text = "0";
            txt3WHITESOL.Text = "0";
            txt3BLACKCRO.Text = "0";
            txtMORNSTAR.Text = "0";
            txtEVENSTAR.Text = "0";
            txtPIERCING.Text = "0";
            txtDARKCLOUD.Text = "0";
            txt3INSIDEUP.Text = "0";
            txt3INSIDEDO.Text = "0";
            txt3OUTSIDEUP.Text = "0";
            txt3OUTSIDEDO.Text = "0";
            txtHARAMIBULL.Text = "0";
            txtHARAMIBEAR.Text = "0";

            var table = db.Query<ScanData>("SELECT [scan_id], COUNT(*) [sec_id] FROM [ScanData] GROUP BY [scan_id] ");
            if (table.Count > 0)
                foreach (var s in table)
                {
                    if (s.scan_id == "BULLENG")
                        txtBULLENG.Text = s.sec_id.ToString();
                    if (s.scan_id == "BEARENG")
                        txtBEARENG.Text = s.sec_id.ToString();
                    if (s.scan_id == "3WHITESOL")
                        txt3WHITESOL.Text = s.sec_id.ToString();
                    if (s.scan_id == "3BLACKCRO")
                        txt3BLACKCRO.Text = s.sec_id.ToString();
                    if (s.scan_id == "MORNSTAR")
                        txtMORNSTAR.Text = s.sec_id.ToString();
                    if (s.scan_id == "EVENSTAR")
                        txtEVENSTAR.Text = s.sec_id.ToString();
                    if (s.scan_id == "PIERCING")
                        txtPIERCING.Text = s.sec_id.ToString();
                    if (s.scan_id == "DARKCLOUD")
                        txtDARKCLOUD.Text = s.sec_id.ToString();
                    if (s.scan_id == "3INSIDEUP")
                        txt3INSIDEUP.Text = s.sec_id.ToString();
                    if (s.scan_id == "3INSIDEDO")
                        txt3INSIDEDO.Text = s.sec_id.ToString();
                    if (s.scan_id == "3OUTSIDEUP")
                        txt3OUTSIDEUP.Text = s.sec_id.ToString();
                    if (s.scan_id == "3OUTSIDEDO")
                        txt3OUTSIDEDO.Text = s.sec_id.ToString();
                    if (s.scan_id == "HARAMIBULL")
                        txtHARAMIBULL.Text = s.sec_id.ToString();
                    if (s.scan_id == "HARAMIBEAR")
                        txtHARAMIBEAR.Text = s.sec_id.ToString();
                }
        }

        //private void PopulateStockItems(string sScanType, ListView listView, TextView txtView)
        private void PopulateStockItems(string sScanType, string sScanName, string sScanDesc)
        {
            var intent = new Intent(this.Context, typeof(ScanStockList));
            intent.AddFlags(ActivityFlags.ClearTop);
            intent.PutExtra("scantype", sScanType);
            intent.PutExtra("scanname", sScanName);
            intent.PutExtra("scandesc", sScanDesc);

            this.Context.StartActivity(intent);            
        }

        private void SetListViewHeight(ListView listView)
        {
            if (listView.Adapter != null)
            {
                int totalHeight = 0;
                for (int i = 0; i < listView.Adapter.Count; i++)
                {
                    View listItem = listView.Adapter.GetView(i, null, listView);
                    //istItem.measure(0, 0);
                    //listItem.M
                    totalHeight += listItem.MeasuredHeight;
                }
                ViewGroup.LayoutParams par = listView.LayoutParameters;
                par.Height = totalHeight + (listView.DividerHeight * (listView.Adapter.Count - 1));
                listView.LayoutParameters = par;
                listView.RequestLayout();
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