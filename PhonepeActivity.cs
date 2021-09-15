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

namespace StockScany
{
    [Activity(Label = "PhonepeActivity")]
    public class PhonepeActivity : Activity
    {
        string status = "";
        string TrnxacsnId = "";
        List<string> ResponseList = new List<string>();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            try
            {
                string sAmount = "1";
                string transaction_ref_id = (DateTime.Now).ToShortDateString() + (DateTime.Now).ToShortTimeString() + "UPI";
                transaction_ref_id = ((transaction_ref_id.Replace("/", "")).Replace(" ", "")).Replace(":", "");
                string transaction_id = Guid.NewGuid().ToString().Substring(0, 10);
                using (var uri = new Android.Net.Uri.Builder()
                    .Scheme("upi")
                    .Authority("pay")
                    .AppendQueryParameter("pa", "cc@ybl")
                    .AppendQueryParameter("pn", "StockScany")
                    .AppendQueryParameter("tn", "Pay for pro")
                    .AppendQueryParameter("tr", transaction_ref_id)
                    .AppendQueryParameter("tid", transaction_id)
                    .AppendQueryParameter("am", sAmount)
                    .AppendQueryParameter("cu", "INR")
                    .Build())
                {
                    Intent intent = new Intent(Intent.ActionView);
                    intent.SetData(uri);
                    if (IsAppInstalledInYourDevice("com.phonepe.app"))
                    {
                        intent.SetPackage("com.phonepe.app");
                        StartActivityForResult(intent, 9999);
                    }
                    else
                    {
                        var package = PackageName;
                        Toast.MakeText(Android.App.Application.Context, "Phonepe is not installed in your device", ToastLength.Long).Show();
                        this.Finish();
                    }
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(Android.App.Application.Context, "Payment Failed", ToastLength.Long).Show();
                this.Finish();
            }
        }

        private bool IsAppInstalledInYourDevice(string packageName)
        {
            Android.Content.PM.PackageManager pm = this.PackageManager;
            bool installed = false;
            try
            {
                pm.GetPackageInfo(packageName, Android.Content.PM.PackageInfoFlags.Activities);
                installed = true;
            }
            catch (Android.Content.PM.PackageManager.NameNotFoundException ex)
            {
                installed = false;
            }
            return installed;
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            try
            {
                if (requestCode == 9999)
                {
                    Console.WriteLine("Phonepe pay result", data);
                    if (resultCode == Result.Ok)
                    {
                        GetResponseFromIntent(data?.Extras);
                    }
                    else if (resultCode == Result.Canceled)
                    {
                        Toast.MakeText(Android.App.Application.Context, "Payment through Phonepe failed", ToastLength.Long).Show();
                    }

                }
            }
            catch (System.Exception ex)
            {

                //Console.WriteLine("Exception while Phonepe payment :" + ex.Message);
                //ShowToast("Payment through Phonepe failed");  //! Failed
                Toast.MakeText(Android.App.Application.Context, "Phonepe :" + ex.Message, ToastLength.Long).Show();
            }
            if (status == "success")
            {
                var tranData = new Tuple<bool, string>(true, TrnxacsnId);
                //Console.Write("Phonepe Messenging center [] status : success");
                //MessagingCenter.Send("PayStatus", "PayStatus", tranData);
                Toast.MakeText(Android.App.Application.Context, "Phonepe Payment Success", ToastLength.Long).Show();
            }
            else
            {
                var tranData = new Tuple<bool, string>(false, null);
                //Console.Write("Phonepe Messenging center [] status : failed");
                //MessagingCenter.Send("PayStatus", "PayStatus", tranData);
                Toast.MakeText(Android.App.Application.Context, "Payment through Phonepe failed", ToastLength.Long).Show();
            }

            Finish();
        }

        private void GetResponseFromIntent(Bundle extras)
        {
            Dictionary<string, string> dict;
            dict = new Dictionary<string, string>();
            if (extras != null)
            {
                foreach (var key in extras.KeySet())
                {
                    dict.Add(key, extras.Get(key).ToString());
                    ResponseList.Add(key + ":" + extras.Get(key).ToString());
                    if (key == "Status" && extras.Get(key).ToString().Contains("FAILURE"))
                    {
                        Toast.MakeText(Android.App.Application.Context, "Payment through Bhim fail", ToastLength.Long).Show();
                        status = "failed";
                    }
                    if (key == "Status" && extras.Get(key).ToString().Contains("SUCCESS"))
                    {
                        Toast.MakeText(Android.App.Application.Context, "Payment through Bhim success", ToastLength.Long).Show();
                        status = "success";
                    }
                    if (key == "responseCode")
                    {
                        Console.WriteLine("Response code [] " + extras.Get(key).ToString());
                    }
                    if (key == "txnid")
                    {
                        TrnxacsnId = extras.Get(key).ToString();
                    }
                }
            }
        }
    }
}