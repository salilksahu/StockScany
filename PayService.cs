using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using StockScany.Fragments;
using Plugin.CurrentActivity;

//[assembly:Xamarin.Forms.Dependency(typeof(PayService))]
namespace StockScany
{
    class PayService: IPaymentService
    {
        public string PhonePE(Context context)
        {
            var intent = new Intent(context, typeof(PhonepeActivity));
            intent.AddFlags(ActivityFlags.ClearTop);
            context.StartActivity(intent);
            return "";
        }

        public string CashfreePayment(Context context, string cftoken, string orderId, string orderAmount, string appID,
            string customerName, string customerPhone, string customerEmail)
        {
            var intent = new Intent(context, typeof(PaymentActivity));
            intent.AddFlags(ActivityFlags.ClearTop);
            intent.PutExtra("cftoken", cftoken);
            intent.PutExtra("orderId", orderId);
            intent.PutExtra("orderAmount", orderAmount);
            intent.PutExtra("appID", appID);
            intent.PutExtra("customerName", customerName);
            intent.PutExtra("customerPhone", customerPhone);
            intent.PutExtra("customerEmail", customerEmail);
            context.StartActivity(intent);
            return "";
        }
    }
}