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
    public class AdvScans : Android.Support.V4.App.Fragment
    {
        static int iEnableAds = 1;
        string _databasePath = Xamarin.Essentials.FileSystem.AppDataDirectory + "/SqliteDatabase.db3";
        string app_unique_id;
        int account_id;
        int is_sub_active;
        int db_state;

        RelativeLayout rlCHANNELUP;
        RelativeLayout rlCHANNELDO;
        RelativeLayout rlFLAG;
        RelativeLayout rlFLAGINV;
        RelativeLayout rlCCIHIGHBOT;
        RelativeLayout rlCCILOWTOP;
        RelativeLayout rlDOUBLEBOT;
        RelativeLayout rlDOUBLETOP;
        //RelativeLayout rlSMCAFBEAR;
        //RelativeLayout rlSMCAFBULL;
        RelativeLayout rlBOTREVRSI;
        RelativeLayout rlTOPREVRSI;

        TextView txtCHANNELUP;
        TextView txtCHANNELDO;
        TextView txtFLAG;
        TextView txtFLAGINV;
        TextView txtCCIHIGHBOT;
        TextView txtCCILOWTOP;
        TextView txtDOUBLEBOT;
        TextView txtDOUBLETOP;
        //TextView txtSMCAFBEAR;
        //TextView txtSMCAFBULL;
        TextView txtBOTREVRSI;
        TextView txtTOPREVRSI;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);

            var view = inflater.Inflate(Resource.Layout.activity_adv, container, false);

            rlCHANNELUP = view.FindViewById<RelativeLayout>(Resource.Id.rlCHANNELUP);
            rlCHANNELDO = view.FindViewById<RelativeLayout>(Resource.Id.rlCHANNELDO);
            rlFLAG = view.FindViewById<RelativeLayout>(Resource.Id.rlFLAG);
            rlFLAGINV = view.FindViewById<RelativeLayout>(Resource.Id.rlFLAGINV);
            rlCCIHIGHBOT = view.FindViewById<RelativeLayout>(Resource.Id.rlCCIHIGHBOT);
            rlCCILOWTOP = view.FindViewById<RelativeLayout>(Resource.Id.rlCCILOWTOP);
            rlDOUBLEBOT = view.FindViewById<RelativeLayout>(Resource.Id.rlDOUBLEBOT);
            rlDOUBLETOP = view.FindViewById<RelativeLayout>(Resource.Id.rlDOUBLETOP);
            //rlSMCAFBEAR = view.FindViewById<RelativeLayout>(Resource.Id.rlSMCAFBEAR);
            //rlSMCAFBULL = view.FindViewById<RelativeLayout>(Resource.Id.rlSMCAFBULL);
            rlBOTREVRSI = view.FindViewById<RelativeLayout>(Resource.Id.rlBOTREVRSI);
            rlTOPREVRSI = view.FindViewById<RelativeLayout>(Resource.Id.rlTOPREVRSI);

            txtCHANNELUP = view.FindViewById<TextView>(Resource.Id.txtCHANNELUP);
            txtCHANNELDO = view.FindViewById<TextView>(Resource.Id.txtCHANNELDO);
            txtFLAG = view.FindViewById<TextView>(Resource.Id.txtFLAG);
            txtFLAGINV = view.FindViewById<TextView>(Resource.Id.txtFLAGINV);
            txtCCIHIGHBOT = view.FindViewById<TextView>(Resource.Id.txtCCIHIGHBOT);
            txtCCILOWTOP = view.FindViewById<TextView>(Resource.Id.txtCCILOWTOP);
            txtDOUBLEBOT = view.FindViewById<TextView>(Resource.Id.txtDOUBLEBOT);
            txtDOUBLETOP = view.FindViewById<TextView>(Resource.Id.txtDOUBLETOP);
            //txtSMCAFBEAR = view.FindViewById<TextView>(Resource.Id.txtSMCAFBEAR);
            //txtSMCAFBULL = view.FindViewById<TextView>(Resource.Id.txtSMCAFBULL);
            txtBOTREVRSI = view.FindViewById<TextView>(Resource.Id.txtBOTREVRSI);
            txtTOPREVRSI = view.FindViewById<TextView>(Resource.Id.txtTOPREVRSI);

            if (iEnableAds == 1)
            {
                AdView adView = view.FindViewById<AdView>(Resource.Id.adView);
                var adRequest = new AdRequest.Builder().Build();
                adView.LoadAd(adRequest);
            }

            string sDesc = "";
            rlCHANNELUP.Click += delegate {
                sDesc = "This pattern shows a channel formation by the moving average lines followed by the price movement upwards, showing a possible bullish breakout.";
                PopulateStockItems("CHANNELUP", "Up From Narrow Channel", sDesc);
            };
            rlCHANNELDO.Click += delegate {
                sDesc = "This pattern shows a channel formation by the moving average lines followed by the price movement downwards, showing a possible bearish breakout.";
                PopulateStockItems("CHANNELDO", "Down From Narrow Channel", sDesc);
            };
            rlFLAG.Click += delegate {
                sDesc = "In this pattern a large bullish day is followed by smaller price movements near the initial bullish closing price, showing a possible bullish breakout.";
                PopulateStockItems("FLAG", "Flag", sDesc);
            };
            rlFLAGINV.Click += delegate {
                sDesc = "In this pattern a large bearish day is followed by smaller price movements near the initial bearish closing price, showing a possible bearish breakout.";
                PopulateStockItems("FLAGINV", "Inverted Flag", sDesc);
            };
            rlCCIHIGHBOT.Click += delegate {
                sDesc = "In this CCI line pattern, the CCI line makes rising bottoms with a bullish final day, showing a possible bullish breakout.";
                PopulateStockItems("CCIHIGHBOT", "CCI Higher Bottoms", sDesc);
            };
            rlCCILOWTOP.Click += delegate {
                sDesc = "In this CCI line pattern, the CCI line makes falling topss with a bearish final day, showing a possible bearish breakout.";
                PopulateStockItems("CCILOWTOP", "CCI Lower Tops", sDesc);
            };
            rlDOUBLEBOT.Click += delegate {
                sDesc = "In this pattern, the prices make double bottoms, with the current day closing above the maximum closing price between the bottoms, showing a possible bullish breakout.";
                PopulateStockItems("DOUBLEBOT", "Double Bottoms", sDesc);
            };
            rlDOUBLETOP.Click += delegate {
                sDesc = "In this pattern, the prices make double tops, with the current day closing below the minimum closing price between the tops, showing a possible bearish breakout.";
                PopulateStockItems("DOUBLETOP", "Double Tops", sDesc);
            };
            /*rlSMCAFBEAR.Click += delegate {
                PopulateStockItems("SMCAFBEAR", "SMA Crossover After Long Bear", "9 days moving average crosses over 21 day average after a long bear.");
            };
            rlSMCAFBULL.Click += delegate {
                PopulateStockItems("SMCAFBULL", "SMA Crossover After Long Bull", "9 days moving average crosses below 21 day average after a long bull.");
            };*/
            rlBOTREVRSI.Click += delegate {
                sDesc = "In this pattern, the prices make falling bottoms, but the RSI rises during the same period, showing a possible bullish breakout.";
                PopulateStockItems("BOTREVRSI", "Falling Bottoms Rising RSI", sDesc);
            };
            rlTOPREVRSI.Click += delegate {
                sDesc = "In this pattern, the prices make rising tops, but the RSI falls during the same period, showing a possible bearish breakout.";
                PopulateStockItems("TOPREVRSI", "Rising Tops Falling RSI", sDesc);
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
            txtCHANNELUP.Text = "0";
            txtCHANNELDO.Text = "0";
            txtFLAG.Text = "0";
            txtFLAGINV.Text = "0";
            txtCCIHIGHBOT.Text = "0";
            txtCCILOWTOP.Text = "0";
            txtDOUBLEBOT.Text = "0";
            txtDOUBLETOP.Text = "0";
            //txtSMCAFBEAR.Text = "0";
            //txtSMCAFBULL.Text = "0";
            txtBOTREVRSI.Text = "0";
            txtTOPREVRSI.Text = "0";

            var table = db.Query<ScanData>("SELECT [scan_id], COUNT(*) [sec_id] FROM [ScanData] GROUP BY [scan_id] ");
            if (table.Count > 0)
                foreach (var s in table)
                {
                    if (s.scan_id == "CHANNELUP")
                        txtCHANNELUP.Text = s.sec_id.ToString();
                    if (s.scan_id == "CHANNELDO")
                        txtCHANNELDO.Text = s.sec_id.ToString();
                    if (s.scan_id == "FLAG")
                        txtFLAG.Text = s.sec_id.ToString();
                    if (s.scan_id == "FLAGINV")
                        txtFLAGINV.Text = s.sec_id.ToString();
                    if (s.scan_id == "CCIHIGHBOT")
                        txtCCIHIGHBOT.Text = s.sec_id.ToString();
                    if (s.scan_id == "CCILOWTOP")
                        txtCCILOWTOP.Text = s.sec_id.ToString();
                    if (s.scan_id == "DOUBLEBOT")
                        txtDOUBLEBOT.Text = s.sec_id.ToString();
                    if (s.scan_id == "DOUBLETOP")
                        txtDOUBLETOP.Text = s.sec_id.ToString();
                    //if (s.scan_id == "SMCAFBEAR")
                    //    txtSMCAFBEAR.Text = s.sec_id.ToString();
                    //if (s.scan_id == "SMCAFBULL")
                    //    txtSMCAFBULL.Text = s.sec_id.ToString();
                    if (s.scan_id == "BOTREVRSI")
                        txtBOTREVRSI.Text = s.sec_id.ToString();
                    if (s.scan_id == "TOPREVRSI")
                        txtTOPREVRSI.Text = s.sec_id.ToString();
                }
        }
        
        private void PopulateStockItems(string sScanType, string sScanName, string sScanDesc)
        {
            var intent = new Intent(this.Context, typeof(ScanStockList));
            intent.AddFlags(ActivityFlags.ClearTop);
            intent.PutExtra("scantype", sScanType);
            intent.PutExtra("scanname", sScanName);
            intent.PutExtra("scandesc", sScanDesc);

            this.Context.StartActivity(intent);            
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