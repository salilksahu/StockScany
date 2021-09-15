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
using SQLite;

namespace StockScany.Classes
{
    [Table("AccountInfo")]
    public class AccountInfo
    {
        public int account_id { get; set; }
        public string unique_id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string account_type { get; set; }
        public string sub_end_date { get; set; }
        public int is_active { get; set; }
        public int db_state { get; set; }
        public DateTime update_date { get; set; }
    }

    [Table("Transaction")]
    public class Transaction
    {
        public int account_id { get; set; }
        public string unique_id { get; set; }        
        public string sub_type { get; set; }
        public string trans_date { get; set; }
        public string order_id { get; set; }
        public string ref_id { get; set; }
        public string status { get; set; }
        public int order_amount { get; set; }
        public string msg { get; set; }
        public int is_pushed { get; set; }

    }
    [Table("DataState")]
    public class DataState
    {
        public float app_state { get; set; }
        public float scans_state { get; set; }
        public float watchlist_state { get; set; }
        public DateTime update_date { get; set; }
    }

    [Table("Indices")]
    public class Indices
    {
        public string indice  { get; set; }
        public decimal price { get; set; }
        public decimal change { get; set; }
        public decimal per_change { get; set; }
        public int seq { get; set; }
    }

    [Table("StockData")]
    public class StockData
    {        
        public int sec_id { get; set; }
        public string symbol { get; set; }
        public float open_p { get; set; }
        public float close_p { get; set; }
        public float high_p { get; set; }
        public float low_p { get; set; }
        public int seq { get; set; }
        public DateTime trade_date { get; set; }
        public float sma5 { get; set; }
        public float sma9 { get; set; }
        public float sma21 { get; set; }
        public float sma50 { get; set; }
        public float sma100 { get; set; }
        public float rsi { get; set; }
        public int qty { get; set; }
    }

    [Table("ScanData")]
    public class ScanData
    {
        public string scan_id { get; set; }
        public int sec_id { get; set; }
        public string symbol { get; set; }
        public float stock_price { get; set; }
        public float stock_price_per { get; set; }
        public float trade_value { get; set; }
        public float open_p { get; set; }
        public float close_p { get; set; }
        public float high_p { get; set; }
        public float low_p { get; set; }
        public DateTime scan_date { get; set; }
        public string event_desc { get; set; }
    }

    [Table("WatchList")]
    public class WatchList
    {
        public int sec_id { get; set; }
        public string symbol { get; set; }        
        public DateTime event_date { get; set; }
        public string event_desc { get; set; }
        public float stock_price { get; set; }
        public float stock_price_per { get; set; }
        public int qty { get; set; }
        public string event_type { get; set; }
    }

    [Table("WLIDS")]
    public class WLIDS
    {
        public int sec_id { get; set; }
        public int db_state_id { get; set; }
    }

    [Table("Securities")]
    public class Securities
    {
        public int sec_id { get; set; }
        public string symbol { get; set; }
        public int db_state { get; set; }
    }

    [Table("FeedbacksData")]
    public class FeedbacksData
    {
        public int complaintCount { get; set; }
        public int suggestionCount { get; set; }
        public int db_state { get; set; }
    }
}