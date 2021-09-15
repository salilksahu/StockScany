using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Gms.Ads;
using Android.Support.V7.App;

namespace StockScany.Classes
{
    [Activity(Label = "Stocks List", Theme = "@style/AppTheme")]
    public class ScanStockList : AppCompatActivity
    {
        static int iEnableAds = 1;
        ListView lvScanStocks;
        TextView txtScanName;
        TextView txtScanDesc;
        string _databasePath = Xamarin.Essentials.FileSystem.AppDataDirectory + "/SqliteDatabase.db3";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            string scantype = Intent.GetStringExtra("scantype");
            string scanname = Intent.GetStringExtra("scanname");
            string scandesc = Intent.GetStringExtra("scandesc");
            // Create your application here
            SetContentView(Resource.Layout.activity_stockList);
            lvScanStocks = FindViewById<ListView>(Resource.Id.lvScanStocks);
            txtScanName = FindViewById<TextView>(Resource.Id.txtScanName);
            txtScanDesc = FindViewById<TextView>(Resource.Id.txtScanDesc);

            if (iEnableAds == 1)
            {
                AdView adView = FindViewById<AdView>(Resource.Id.adView);
                var adRequest = new AdRequest.Builder().Build();
                adView.LoadAd(adRequest);
            }

            txtScanName.Text = scanname;
            txtScanDesc.Text = scandesc;

            try
            {
                if (lvScanStocks.Adapter != null && lvScanStocks.Adapter.Count > 0)
                {
                    List<StockItem> sEmptyItemList = new List<StockItem>();
                    lvScanStocks.Adapter = new StocksListAdapter(this, sEmptyItemList);
                    return;
                }
            }
            catch (Exception ex) { }

            var db = new SQLiteConnection(_databasePath);
            //var table = db.Table<ScanData>();

            var table = db.Query<ScanData>("SELECT DISTINCT * FROM [ScanData] WHERE scan_id = ? ", scantype);

            List<StockItem> sItemList = new List<StockItem>();
            StockItem sItem;

            foreach (var s in table)
            {
                sItem = new StockItem();
                sItem.sec_id = s.sec_id;
                sItem.symbol = s.symbol;
                sItem.stock_price = s.stock_price;
                sItem.stock_price_per = s.stock_price_per;
                sItem.open_p = s.open_p;
                sItem.high_p = s.high_p;
                sItem.low_p = s.low_p;
                sItem.close_p = s.close_p;
                sItem.trade_value = s.trade_value;
                sItem.trade_date = s.scan_date;
                sItemList.Add(sItem);
            }

            StocksListAdapter sAdapter = new StocksListAdapter(this, sItemList);
            lvScanStocks.Adapter = sAdapter;            
        }
    }
}