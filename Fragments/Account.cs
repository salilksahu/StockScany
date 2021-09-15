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
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Globalization;
using Android.Preferences;
using StockScany.Classes;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using Xamarin.Essentials;
using SQLite;
using System.Data;
using Android.Support.V7.Widget;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Android.Gms.Ads;

namespace StockScany.Fragments
{
    public class Account : Android.Support.V4.App.Fragment
    {
        static int iEnableAds = 1;
        string _databasePath = Xamarin.Essentials.FileSystem.AppDataDirectory + "/SqliteDatabase.db3";
        string app_unique_id = "";
        int account_id = 0;
        string sAccountType = "F";       

        Button btnReset;
        TextView txtReset;
        LinearLayout llAccountMainReset;
        TextView txtPrivacyP;
        TextView txtAboutUs;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here         
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);
            var view = inflater.Inflate(Resource.Layout.activity_account, container, false);

            llAccountMainReset = view.FindViewById<LinearLayout>(Resource.Id.llAccountMainReset);
            txtReset = view.FindViewById<TextView>(Resource.Id.txtReset);
            btnReset = view.FindViewById<Button>(Resource.Id.btnReset);
            txtPrivacyP = view.FindViewById<TextView>(Resource.Id.txtPrivacyP);
            txtAboutUs = view.FindViewById<TextView>(Resource.Id.txtAboutUs);
            llAccountMainReset.Visibility = ViewStates.Gone;

            if (iEnableAds == 1)
            {
                AdView adViewAcc = view.FindViewById<AdView>(Resource.Id.adView);
                var adRequest = new AdRequest.Builder().Build();
                adViewAcc.LoadAd(adRequest);
            }

            txtReset.Click += (sender, args) => {
                //hide show
                ViewStates vs = llAccountMainReset.Visibility;
                if (vs.Equals(ViewStates.Gone))
                    llAccountMainReset.Visibility = ViewStates.Visible;
                else
                    llAccountMainReset.Visibility = ViewStates.Gone;
            };

            btnReset.Click += (sender, args) => {
                ResetAll();
            };

            txtPrivacyP.Click += (sender, args) => {                
                ShowPrivacy();
            };

            txtAboutUs.Click += (sender, args) => {
                ShowAboutUs();
                
            };

            return view;
        }

        private void ShowPrivacy()
        {
            Dialog popupPrivacy = new Dialog(this.Context);
            popupPrivacy.SetContentView(Resource.Layout.activity_firstload);
            popupPrivacy.Window.SetSoftInputMode(SoftInput.AdjustResize);
            popupPrivacy.Show();
            popupPrivacy.FindViewById<TextView>(Resource.Id.txtHeader).Text = "App Privacy Policy";
            popupPrivacy.FindViewById<TextView>(Resource.Id.txtBody).Text = GetString(Resource.String.privacy);
        }

        private void ShowAboutUs()
        {
            Dialog popupPrivacy = new Dialog(this.Context);
            popupPrivacy.SetContentView(Resource.Layout.activity_firstload);
            popupPrivacy.Window.SetSoftInputMode(SoftInput.AdjustResize);
            popupPrivacy.Show();
            popupPrivacy.FindViewById<TextView>(Resource.Id.txtHeader).Text = "About StockScany";
            popupPrivacy.FindViewById<TextView>(Resource.Id.txtBody).Text = GetString(Resource.String.about);
        }

        private void ResetAll()
        {
            Android.App.AlertDialog.Builder dialog = new Android.App.AlertDialog.Builder(this.Context);
            Android.App.AlertDialog alert = dialog.Create();
            alert.SetTitle("Reset Account");
            alert.SetMessage("This will reset all your account information and locally stored information. Are you sure you want to proceed.");
            alert.SetButton("OK", (c, ev) =>
            {
                var db = new SQLiteConnection(_databasePath);
                db.DropTable<AccountInfo>();
                db.DropTable<Transaction>();
                db.DropTable<StockData>();
                db.DropTable<ScanData>();
                db.DropTable<WatchList>();
                db.DropTable<WLIDS>();
                db.DropTable<FeedbacksData>();
                db.CreateTable<AccountInfo>();
                db.CreateTable<Transaction>();
                db.CreateTable<StockData>();
                db.CreateTable<ScanData>();
                db.CreateTable<WatchList>();
                db.CreateTable<WLIDS>();
                db.CreateTable<FeedbacksData>();

                app_unique_id = System.Guid.NewGuid().ToString();
                Preferences.Set("app_unique_id", app_unique_id);
                
                try
                {
                    var service = new net.azurewebsites.stockscanyweb.SSWebService();
                    DataTable dtGenInfo = service.GetGeneralInfo(app_unique_id, 0);

                    db.CreateTable<AccountInfo>();
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
                    db.Insert(account);

                    account_id = (int)dtGenInfo.Rows[0]["ID"];
                    Preferences.Set("account_id", (int)dtGenInfo.Rows[0]["ID"]);
                    Preferences.Set("sub_active", (int)dtGenInfo.Rows[0]["SUB_ACTIVE"]);
                    Preferences.Set("welcome_state", (int)dtGenInfo.Rows[0]["DB_STATE"]);
                    Preferences.Set("scans_state", 0);
                    Preferences.Get("watchlist_state", 0);
                    
                    sAccountType = "F";
                    DisplayAlert("Reset", "The reset was done sucessfully.");
                }
                catch (Exception ex)
                {
                    DisplayAlert("Reset", "There was some issue in resetting your account. Please try after some time. ");
                }                

                alert.Cancel();
            
            });
            alert.SetButton2("Cancel", (c, ev) =>
            {
                alert.Cancel();
            });
            alert.Show();
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