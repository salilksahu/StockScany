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

namespace StockScany.Classes
{
    class StockItem
    {
        public int sec_id { get; set; }
        public string symbol { get; set; }
        public float stock_price { get; set; }
        public float stock_price_per { get; set; }
        public float trade_value { get; set; }
        public float open_p { get; set; }
        public float close_p { get; set; }
        public float high_p { get; set; }
        public float low_p { get; set; }
        public DateTime trade_date { get; set; }
    }

    class WatchListItem
    {
        public int sec_id { get; set; }
        public string symbol { get; set; }        
        public List<string> event_desc { get; set; }
        public float stock_price { get; set; }
        public float stock_price_per { get; set; }
        public float price_change { get; set; }
        public float percentage_change { get; set; }
        public List<string> event_type { get; set; }
    }

    class TransItem
    {
        public string transDate { get; set; }
        public string orderId { get; set; }
        public string refId { get; set; }
        public string orderAmount { get; set; }
        public string status { get; set; }
        public string details { get; set; }
    }

    class IndexItem
    {
        public string indice { get; set; }
        public string price { get; set; }
        public string change { get; set; }
        public string per_change { get; set; }
    }

    class OTPModel
    { 
        public string sender { get; set; }
        public string route { get; set; }
        public string country { get; set; }
        public List<Sms> sms { get; set; } = new List<Sms> { };
    }

    class Sms
    {
        public string message { get; set; }
        public List<string> to { get; set; }
    }

    class OTPResponse
    {
        public string message { get; set; }
        public string type { get; set; }
    }
}