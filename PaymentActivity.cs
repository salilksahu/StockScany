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
using Com.Gocashfree.Cashfreesdk;
using StockScany.Fragments;
using Android.Preferences;

namespace StockScany
{
    [Activity(Label = "PaymentActivity")]
    public class PaymentActivity : Activity
    {
        string orderId = "";
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            string cftoken = Intent.GetStringExtra("cftoken");
            orderId = Intent.GetStringExtra("orderId");
            string orderAmount = Intent.GetStringExtra("orderAmount");
            string appID = Intent.GetStringExtra("appID");
            string customerName = Intent.GetStringExtra("customerName");
            string customerPhone = Intent.GetStringExtra("customerPhone");
            string customerEmail = Intent.GetStringExtra("customerEmail");

            // Create your application here
            CFPaymentService cfPaymentService = CFPaymentService.CFPaymentServiceInstance;
            cfPaymentService.Orientation = 0;

            IDictionary<String, String> postData = new Dictionary<String, String>();

            postData.Add("appId", appID);
            postData.Add("orderId", orderId);
            postData.Add("orderCurrency", "INR");
            postData.Add("orderAmount", orderAmount);
            postData.Add("orderNote", "Sunscription charges");
            postData.Add("customerName", customerName);
            postData.Add("customerPhone", customerPhone);
            postData.Add("customerEmail", customerEmail);

            cfPaymentService.DoPayment(this, postData, cftoken, "TEST");

        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (data != null)
            {
                ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this.BaseContext);
                ISharedPreferencesEditor editor = prefs.Edit();
                editor.PutString("orderId", data.GetStringExtra("orderId"));
                editor.PutString("referenceId", data.GetStringExtra("referenceId"));
                editor.PutString("txStatus", data.GetStringExtra("txStatus"));
                editor.PutString("txMsg", data.GetStringExtra("txMsg"));
                editor.PutString("paymentMode", data.GetStringExtra("paymentMode"));
                editor.PutString("signature", data.GetStringExtra("signature"));
                //editor.Commit();    // applies changes synchronously on older APIs
                editor.Apply();
            }
            Finish();
        }
    }
}