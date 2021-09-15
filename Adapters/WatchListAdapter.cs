using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StockScany.Classes;
using MikePhil.Charting.Charts;
using System.Drawing;
using System.Data;
using SQLite;
using Xamarin.Essentials;
using Android.Content.Res;
using Android.Util;
using StockScany.Fragments;
using Android.Support.V4.Text;
using Android.Webkit;
using Android.Graphics;
using System.Collections;
using MikePhil.Charting.Data;

namespace StockScany
{
    class WatchListAdapter : BaseAdapter
    {
        List<WatchListItem> items;
        Context context;
        Dialog popupChart;
        private MethodCaller methodCaller;
        ListView lvEvents;
        RelativeLayout layoutStockItem;
        int iDeleted = 0;
        string app_unique_id;
        int account_id;
        int is_sub_active;
        public WatchListAdapter(Context context, List<WatchListItem> items, MethodCaller methodCaller)
        {
            this.items = items;
            this.context = context;
            this.methodCaller = methodCaller;            
        }


        public override Java.Lang.Object GetItem(int position)
        {
            return position;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var item = items[position];
            var view = convertView;
            WatchListAdapterViewHolder holder = null;

            if (view != null)
            {
                string sSymbol = view.FindViewById<TextView>(Resource.Id.txtSymbol).Text;
                if (sSymbol == item.symbol)
                    holder = view.Tag as WatchListAdapterViewHolder;
                else
                    holder = null;

            }

            if (holder == null)// || iDeleted == 1)
            {
                //iDeleted = 0;
                holder = new WatchListAdapterViewHolder();
                var inflater = context.GetSystemService(Context.LayoutInflaterService).JavaCast<LayoutInflater>();
                //replace with your item and your holder items
                //comment back in
                view = inflater.Inflate(Resource.Layout.watchlist_item, parent, false);
                //holder.Title = view.FindViewById<TextView>(Resource.Id.text);

                view.FindViewById<TextView>(Resource.Id.txtSymbol).Text = item.symbol;
                view.FindViewById<TextView>(Resource.Id.txtStockPrice).Text = item.stock_price.ToString();
                view.FindViewById<TextView>(Resource.Id.txtStockPricePer).Text = " [" + item.stock_price_per.ToString() + "%]";
                if(item.stock_price_per >= 0)
                    view.FindViewById<TextView>(Resource.Id.txtStockPricePer).SetTextColor(Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.DarkGreen));
                else
                    view.FindViewById<TextView>(Resource.Id.txtStockPricePer).SetTextColor(Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.DarkRed));
                view.FindViewById<TextView>(Resource.Id.txtStockPriceChange).Text = item.price_change.ToString();
                view.FindViewById<TextView>(Resource.Id.txtStockPerChange).Text = " [" + item.percentage_change.ToString() + "%]";
                if (item.percentage_change >= 0)
                    view.FindViewById<TextView>(Resource.Id.txtStockPerChange).SetTextColor(Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.DarkGreen));
                else
                    view.FindViewById<TextView>(Resource.Id.txtStockPerChange).SetTextColor(Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.DarkRed));

                LinearLayout llEventDesc = view.FindViewById<LinearLayout>(Resource.Id.llEventDesc);
                for (int i = 0; i < item.event_desc.Count; i++)
                {
                    TextView tt = new TextView(this.context);
                    tt.Text = item.event_desc[i];
                    tt.SetTextSize(ComplexUnitType.Dip, 15);
                    Typeface tf = Typeface.CreateFromAsset(context.Assets, "barlow.ttf");
                    tt.Typeface = tf;

                    if (i == 0)
                        tt.SetTextColor(Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.DarkGray));
                    else
                    {
                        if (item.event_type[i] == "BULL")
                            tt.SetTextColor(Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.DarkGreen));
                        else
                            tt.SetTextColor(Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.DarkRed));
                    }
                    llEventDesc.AddView(tt);
                }                

                ImageButton btnShowChart = view.FindViewById<ImageButton>(Resource.Id.btnShowChart);
                ImageButton btnRemoveWatch = view.FindViewById<ImageButton>(Resource.Id.btnShowChartR);
                //ImageButton btnExtChart = view.FindViewById<ImageButton>(Resource.Id.btnExtChart);

                btnShowChart.Tag = position;
                btnShowChart.Click += (sender, args) =>
                {
                    Dialog progressDialog = CustomProgressDialog.Show(this.context);
                    LoadStockChart(item.sec_id, item.symbol);
                    progressDialog.Dismiss();
                    //Toast.MakeText(Application.Context, "pos: " + ((Button)sender).Tag.ToString(), ToastLength.Short).Show();
                };

                btnRemoveWatch.Tag = position;
                btnRemoveWatch.Click += (sender, args) =>
                {
                    RemoveWLItem((int)btnRemoveWatch.Tag, item.sec_id);                    
                };

                view.Tag = holder;
            }

            //fill in your items
            //holder.Title.Text = "new text here";

            return view;
        }

        //Fill in cound here, currently 0
        public override int Count
        {
            get
            {
                return items.Count;
            }
        }
        private void GetAccountDetails()
        {
            string _databasePath = Xamarin.Essentials.FileSystem.AppDataDirectory + "/SqliteDatabase.db3";
            var db = new SQLiteConnection(_databasePath);
            var table = db.Query<AccountInfo>("SELECT * FROM [AccountInfo] ");
            if (table.Count > 0)
            {
                foreach (var s in table)
                {
                    app_unique_id = s.unique_id;
                    account_id = s.account_id;
                    is_sub_active = s.is_active;
                }
            }
            db.Close();
        }

        public void RemoveWLItem(int iPos, int sec_id)
        {
            Android.App.AlertDialog.Builder dialog = new Android.App.AlertDialog.Builder(this.context);
            Android.App.AlertDialog alert = dialog.Create();
            alert.SetTitle("Remove Stock");
            alert.SetMessage("Are you sure you want to remove the stock from the watchlist.");
            alert.SetButton("OK", (c, ev) =>
            {
                //iDeleted = 1;
                methodCaller = new WatchlistActivity();
                methodCaller.RemoveFromWatchlist(sec_id, iPos);

                WatchListItem wlItem = items[iPos];
                items.Remove(wlItem);
                this.NotifyDataSetChanged();                
                alert.Cancel();
            });
            alert.SetButton2("Cancel", (c, ev) =>
            {                
                alert.Cancel();
            });
            alert.Show();
        }

        public void LoadExternalChart(string symbol)
        {
            try
            {
                Uri uri = new Uri("https://in.tradingview.com/chart/?symbol=NSE%3A" + symbol);
                Browser.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
            }
            catch (Exception ex)
            {
                return;
            }
        }

        public void LoadStockChart(int sec_id, string sSymbol)
        {
            //candleStickChart.Visibility = ViewStates.Visible;
            popupChart = new Dialog(this.context);
            popupChart.SetContentView(Resource.Layout.chart_layout);
            popupChart.Window.SetSoftInputMode(SoftInput.AdjustResize);
            popupChart.Show();

            var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;

            // Access Popup layout fields like below
            CandleStickChart candleStickChart = popupChart.FindViewById<CandleStickChart>(Resource.Id.canStickStockChart);
            candleStickChart.LayoutParameters.Width = (int)mainDisplayInfo.Width - (int)(mainDisplayInfo.Width / 6);
            candleStickChart.LayoutParameters.Height = (int)mainDisplayInfo.Height - (int)(3.1 * mainDisplayInfo.Height / 4);

            MikePhil.Charting.Components.Description desc = new MikePhil.Charting.Components.Description();
            desc.Typeface = Typeface.CreateFromAsset(this.context.Assets, "barlow.ttf");
            desc.Text = sSymbol + " last 20 trading days";
            desc.TextSize = 12;
            candleStickChart.Description = desc;
            candleStickChart.SetDrawBorders(true);
            candleStickChart.SetBorderColor(Android.Graphics.Color.LightGray);
            candleStickChart.SetBorderWidth(0.8f);
            MikePhil.Charting.Components.YAxis yAxis = candleStickChart.AxisLeft;
            MikePhil.Charting.Components.YAxis rightAxis = candleStickChart.AxisRight;
            yAxis.SetDrawGridLines(false);
            yAxis.SetDrawLabels(false);
            rightAxis.SetDrawGridLines(true);
            rightAxis.TextColor = Android.Graphics.Color.Black.ToArgb();
            MikePhil.Charting.Components.XAxis xAxis = candleStickChart.XAxis;
            xAxis.SetDrawGridLines(false);// disable x axis grid lines
            xAxis.SetDrawLabels(false);
            xAxis.SetDrawAxisLine(true);
            xAxis.Granularity = 1.5f;
            xAxis.GranularityEnabled = true;
            xAxis.SetAvoidFirstLastClipping(true);
            MikePhil.Charting.Components.Legend l = candleStickChart.Legend;
            l.Enabled = false;
            IList<MikePhil.Charting.Data.CandleEntry> stockCandleStick = new List<MikePhil.Charting.Data.CandleEntry>();

            string _databasePath = Xamarin.Essentials.FileSystem.AppDataDirectory + "/SqliteDatabase.db3";
            var service = new net.azurewebsites.stockscanyweb.SSWebService();
            var db = new SQLiteConnection(_databasePath);
            db.CreateTable<StockData>();
            string sFlag = "N";

            if (db.Table<StockData>().Count() == 0)
                sFlag = "Y";
            else
            {
                var stockDataCheck = db.Query<StockData>("SELECT * FROM [StockData] WHERE [sec_id] = ?", sec_id);
                if (stockDataCheck.Count == 0)
                    sFlag = "Y";
            }
            if (sFlag == "Y")
            {
                if (account_id == 0)
                    GetAccountDetails();
                try
                {
                    DataTable dtStockData = service.GetScanStocksData(app_unique_id, account_id, sec_id);
                    foreach (DataRow dr in dtStockData.Rows)
                    {
                        var stockDataDB = new StockData();
                        stockDataDB.sec_id = (int)dr["sec_id"];
                        stockDataDB.symbol = (string)dr["symbol"];
                        stockDataDB.high_p = float.Parse(((decimal)dr["high_p"]).ToString());
                        stockDataDB.low_p = float.Parse(((decimal)dr["low_p"]).ToString());
                        stockDataDB.open_p = float.Parse(((decimal)dr["open_p"]).ToString());
                        stockDataDB.close_p = float.Parse(((decimal)dr["close_p"]).ToString());
                        stockDataDB.trade_date = (DateTime)dr["trade_date"];
                        stockDataDB.seq = (int)dr["seq"];
                        stockDataDB.sma5 = float.Parse(((decimal)dr["sma_5"]).ToString());
                        stockDataDB.sma9 = float.Parse(((decimal)dr["sma_9"]).ToString());
                        stockDataDB.sma21 = float.Parse(((decimal)dr["sma_21"]).ToString());
                        stockDataDB.sma50 = float.Parse(((decimal)dr["sma_50"]).ToString());
                        stockDataDB.sma100 = float.Parse(((decimal)dr["sma_100"]).ToString());
                        stockDataDB.rsi = float.Parse(((decimal)dr["rsi"]).ToString());
                        stockDataDB.qty = Int32.Parse(((decimal)dr["trdqty"]).ToString());
                        db.Insert(stockDataDB);
                    }
                }
                catch (Exception ex)
                {
                    Android.App.AlertDialog.Builder dialog = new AlertDialog.Builder(this.context);
                    AlertDialog alert = dialog.Create();
                    alert.SetTitle("Loading Chart");
                    alert.SetMessage("There was some issue loading the stock chart. Please try after some time.");
                    alert.SetButton("OK", (c, ev) =>
                    {
                        alert.Cancel();
                    });
                    alert.Show();
                }
            }

            var stockData = db.Query<StockData>("SELECT * FROM [StockData] WHERE [sec_id] = ?", sec_id);
            foreach (var s in stockData)
                stockCandleStick.Add(new MikePhil.Charting.Data.CandleEntry(s.seq, s.high_p, s.low_p, s.open_p, s.close_p));

            MikePhil.Charting.Data.CandleDataSet set1 = new MikePhil.Charting.Data.CandleDataSet(stockCandleStick, "DataSet 1");
            set1.SetColor(Android.Graphics.Color.Gray.ToArgb(), 0);
            set1.ShadowColor = Android.Graphics.Color.Gray.ToArgb();
            set1.ShadowWidth = 1.7f;
            set1.DecreasingColor = Android.Graphics.Color.DarkRed.ToArgb();
            set1.DecreasingPaintStyle = Android.Graphics.Paint.Style.Stroke;
            set1.IncreasingColor = Android.Graphics.Color.DarkGreen.ToArgb();
            set1.IncreasingPaintStyle = Android.Graphics.Paint.Style.Stroke;
            set1.NeutralColor = Android.Graphics.Color.Gray.ToArgb();
            set1.SetDrawValues(false);
            // create a data object with the datasets
            MikePhil.Charting.Data.CandleData data = new MikePhil.Charting.Data.CandleData(set1);
            // set data
            candleStickChart.Data = data;
            candleStickChart.Invalidate();

            //Price SMA chart
            LineChart lineChart = popupChart.FindViewById<LineChart>(Resource.Id.lineChart);
            lineChart.LayoutParameters.Width = (int)mainDisplayInfo.Width - (int)(mainDisplayInfo.Width / 6);
            lineChart.LayoutParameters.Height = (int)mainDisplayInfo.Height - (int)(3.1 * mainDisplayInfo.Height / 4);
            lineChart.SetDrawBorders(true);
            lineChart.SetBorderColor(Android.Graphics.Color.LightGray);
            MikePhil.Charting.Components.Description desc1 = new MikePhil.Charting.Components.Description();
            desc1.Text = "Price & SMA's";
            desc1.TextSize = 12;
            lineChart.Description = desc1;
            MikePhil.Charting.Components.YAxis leftAxis = lineChart.AxisLeft;
            rightAxis = lineChart.AxisRight;
            leftAxis.SetDrawGridLines(false);
            leftAxis.SetDrawLabels(false);
            rightAxis.SetDrawGridLines(true);
            rightAxis.TextColor = Android.Graphics.Color.Black.ToArgb();
            xAxis = lineChart.XAxis;
            xAxis.SetDrawGridLines(false);// disable x axis grid lines
            xAxis.SetDrawLabels(false);
            xAxis.SetDrawAxisLine(true);
            xAxis.Granularity = 1.5f;

            IList<MikePhil.Charting.Data.Entry> linePriceEntry = new List<MikePhil.Charting.Data.Entry>();
            IList<MikePhil.Charting.Data.Entry> lineSMA5Entry = new List<MikePhil.Charting.Data.Entry>();
            IList<MikePhil.Charting.Data.Entry> lineSMA9Entry = new List<MikePhil.Charting.Data.Entry>();
            IList<MikePhil.Charting.Data.Entry> lineSMA21Entry = new List<MikePhil.Charting.Data.Entry>();
            IList<MikePhil.Charting.Data.Entry> lineSMA50Entry = new List<MikePhil.Charting.Data.Entry>();
            IList<MikePhil.Charting.Data.Entry> lineSMA100Entry = new List<MikePhil.Charting.Data.Entry>();
            
            foreach (var s in stockData)
            {
                linePriceEntry.Add(new Entry(s.seq, s.close_p));
                lineSMA5Entry.Add(new Entry(s.seq, s.sma5));
                lineSMA9Entry.Add(new Entry(s.seq, s.sma9));
                lineSMA21Entry.Add(new Entry(s.seq, s.sma21));
                lineSMA50Entry.Add(new Entry(s.seq, s.sma50));
                lineSMA100Entry.Add(new Entry(s.seq, s.sma100));
            }
            LineDataSet priceSet = new LineDataSet(linePriceEntry, "Price");
            LineDataSet sma5Set = new LineDataSet(lineSMA5Entry, "5 SMA");
            LineDataSet sma9Set = new LineDataSet(lineSMA9Entry, "9 SMA");
            LineDataSet sma21Set = new LineDataSet(lineSMA21Entry, "21 SMA");
            LineDataSet sma50Set = new LineDataSet(lineSMA50Entry, "50 SMA");
            LineDataSet sma100Set = new LineDataSet(lineSMA100Entry, "100 SMA");

            priceSet.Color = Android.Graphics.Color.Green.ToArgb();
            priceSet.SetDrawCircles(false);
            priceSet.SetDrawValues(false);
            sma5Set.Color = Android.Graphics.Color.Red.ToArgb();
            sma5Set.SetDrawCircles(false);
            sma5Set.SetDrawValues(false);
            sma5Set.EnableDashedLine(10f, 5f, 0f);            
            sma9Set.Color = Android.Graphics.Color.Blue.ToArgb();
            sma9Set.SetDrawCircles(false);
            sma9Set.EnableDashedLine(10f, 5f, 0f);
            sma9Set.SetDrawValues(false);
            sma21Set.Color = Android.Graphics.Color.Coral.ToArgb();
            sma21Set.SetDrawCircles(false);
            sma21Set.EnableDashedLine(10f, 5f, 0f);
            sma21Set.SetDrawValues(false);
            sma50Set.Color = Android.Graphics.Color.Brown.ToArgb();
            sma50Set.SetDrawCircles(false);
            sma50Set.EnableDashedLine(10f, 5f, 0f);
            sma50Set.SetDrawValues(false);
            sma100Set.Color = Android.Graphics.Color.Gold.ToArgb();
            sma100Set.SetDrawCircles(false);
            sma100Set.EnableDashedLine(10f, 5f, 0f);
            sma100Set.SetDrawValues(false);

            LineData ldata = new LineData();
            ldata.AddDataSet(priceSet);
            ldata.AddDataSet(sma5Set);
            ldata.AddDataSet(sma9Set);
            ldata.AddDataSet(sma21Set);
            ldata.AddDataSet(sma50Set);
            ldata.AddDataSet(sma100Set);
            
            // set data
            lineChart.Data = ldata;            
            lineChart.Invalidate();

            //volume chart            
            BarChart volChart = popupChart.FindViewById<BarChart>(Resource.Id.volChart);
            volChart.LayoutParameters.Width = (int)mainDisplayInfo.Width - (int)(1.2*mainDisplayInfo.Width / 6);
            volChart.LayoutParameters.Height = (int)mainDisplayInfo.Height - (int)(3.7 * mainDisplayInfo.Height / 4);            
            MikePhil.Charting.Components.Description desc2 = new MikePhil.Charting.Components.Description();
            desc2.Text = "";
            desc2.TextSize = 11;
            volChart.Description = desc2;
            volChart.SetDrawBorders(false);
            leftAxis = volChart.AxisLeft;
            rightAxis = volChart.AxisRight;
            leftAxis.SetDrawGridLines(false);
            leftAxis.SetDrawLabels(false);
            leftAxis.SetDrawAxisLine(false);
            rightAxis.SetDrawGridLines(false);
            rightAxis.SetDrawLabels(false);
            rightAxis.SetDrawAxisLine(false);
            xAxis = volChart.XAxis;
            xAxis.SetDrawGridLines(false);// disable x axis grid lines
            xAxis.SetDrawLabels(false);
            xAxis.SetDrawAxisLine(false);
            xAxis.Granularity = 1.5f;
            MikePhil.Charting.Components.Legend l1 = volChart.Legend;
            l1.Enabled = false;

            IList<MikePhil.Charting.Data.BarEntry> volEntry = new List<MikePhil.Charting.Data.BarEntry>();
            
            foreach (var s in stockData)
            {
                volEntry.Add(new BarEntry(s.seq, s.qty));
            }
            BarDataSet vSet = new BarDataSet(volEntry, "qty");
            vSet.Color = Android.Graphics.Color.GreenYellow.ToArgb();
            vSet.SetDrawValues(false);
            BarData vdata = new BarData();
            vdata.AddDataSet(vSet);
            volChart.Data = vdata;
            volChart.Invalidate();

            //rsi chart
            LineChart rsiChart = popupChart.FindViewById<LineChart>(Resource.Id.rsiChart);
            rsiChart.LayoutParameters.Width = (int)mainDisplayInfo.Width - (int)(mainDisplayInfo.Width / 6);
            rsiChart.LayoutParameters.Height = (int)mainDisplayInfo.Height - (int)(3.4 * mainDisplayInfo.Height / 4);
            MikePhil.Charting.Components.Description desc4 = new MikePhil.Charting.Components.Description();
            desc4.Text = "RSI";
            desc4.TextSize = 12;
            rsiChart.Description = desc4;
            leftAxis = rsiChart.AxisLeft;
            rightAxis = rsiChart.AxisRight;
            leftAxis.SetDrawGridLines(false);
            leftAxis.SetDrawLabels(false);
            rightAxis.SetDrawGridLines(false);
            rightAxis.TextColor = Android.Graphics.Color.Black.ToArgb();
            xAxis = rsiChart.XAxis;
            xAxis.SetDrawGridLines(false);// disable x axis grid lines
            xAxis.SetDrawLabels(false);
            xAxis.SetDrawAxisLine(true);
            xAxis.Granularity = 1.5f;
            MikePhil.Charting.Components.Legend l2 = rsiChart.Legend;
            l2.Enabled = false;

            IList<MikePhil.Charting.Data.Entry> rsiEntry = new List<MikePhil.Charting.Data.Entry>();
            IList<MikePhil.Charting.Data.Entry> rsiZeroEntry = new List<MikePhil.Charting.Data.Entry>();
            IList<MikePhil.Charting.Data.Entry> rsiHighEntry = new List<MikePhil.Charting.Data.Entry>();
            IList<MikePhil.Charting.Data.Entry> rsiLowEntry = new List<MikePhil.Charting.Data.Entry>();
             
            foreach (var s in stockData)
            {
                rsiEntry.Add(new Entry(s.seq, s.rsi));
                rsiLowEntry.Add(new Entry(s.seq, 20));
                rsiHighEntry.Add(new Entry(s.seq, 80));
            }
            LineDataSet rsiSet = new LineDataSet(rsiEntry, "rsi");
            LineDataSet rsiHighSet = new LineDataSet(rsiHighEntry, "rsi80");
            LineDataSet rsiLowSet = new LineDataSet(rsiLowEntry, "rsim20");

            rsiSet.Color = Android.Graphics.Color.Cyan.ToArgb();
            rsiSet.SetDrawValues(false);
            rsiSet.SetDrawFilled(true);
            rsiSet.SetDrawCircles(false);
            rsiHighSet.SetDrawCircles(false);
            rsiHighSet.Color = Android.Graphics.Color.Gray.ToArgb();
            rsiHighSet.SetDrawValues(false);
            rsiHighSet.SetDrawCircles(false);
            rsiHighSet.EnableDashedLine(10f, 5f, 0f);
            rsiLowSet.Color = Android.Graphics.Color.Gray.ToArgb();
            rsiLowSet.SetDrawValues(false);
            rsiLowSet.SetDrawCircles(false);
            rsiLowSet.EnableDashedLine(10f, 5f, 0f);

            LineData lRSIdata = new LineData();
            lRSIdata.AddDataSet(rsiSet);
            lRSIdata.AddDataSet(rsiHighSet);
            lRSIdata.AddDataSet(rsiLowSet);
            // set data
            rsiChart.Data = lRSIdata;
            rsiChart.Invalidate();
        }
    }

    class WatchListAdapterViewHolder : Java.Lang.Object
    {
        //Your adapter views to re-use
        //public TextView Title { get; set; }
    }
}