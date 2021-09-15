using Android.App;
using Android.Content;
using Android.Gms.Ads;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using SQLite;
using StockScany.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StockScany.Fragments
{
    public class Feedback : Android.Support.V4.App.Fragment
    {
        static int iEnableAds = 1;
        string _databasePath = Xamarin.Essentials.FileSystem.AppDataDirectory + "/SqliteDatabase.db3";
        string app_unique_id;
        int account_id = 0;
        string sName = "";
        string sEmail = "";
        string sAccountType = "";
        int db_state;

        EditText etName;
        EditText etEmail;
        EditText etFeedback;
        RadioButton rbComplaint;
        RadioButton rbSuggestion;
        Button btnSubmit;
        TextView txtErrMsg;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);
            var view = inflater.Inflate(Resource.Layout.activity_feedback, container, false);

            etName = view.FindViewById<EditText>(Resource.Id.etName);
            etEmail = view.FindViewById<EditText>(Resource.Id.etEmail);
            etFeedback = view.FindViewById<EditText>(Resource.Id.etFeedback);
            rbComplaint = view.FindViewById<RadioButton>(Resource.Id.rbComplaint);
            rbSuggestion = view.FindViewById<RadioButton>(Resource.Id.rbSuggestion);
            btnSubmit = view.FindViewById<Button>(Resource.Id.btnSubmit);
            txtErrMsg = view.FindViewById<TextView>(Resource.Id.txtErrMsg);

            if (iEnableAds == 1)
            {
                AdView adView = view.FindViewById<AdView>(Resource.Id.adView4);
                var adRequest = new AdRequest.Builder().Build();
                adView.LoadAd(adRequest);
            }

            rbComplaint.Checked = true;
            etFeedback.Enabled = true;
            btnSubmit.Enabled = true;
            etFeedback.Text = "";

            GetAccountDetails();
            UpdateAccountInfo();

            btnSubmit.Click += delegate {
                if (String.IsNullOrWhiteSpace(etName.Text) || String.IsNullOrEmpty(etName.Text) ||
                String.IsNullOrWhiteSpace(etEmail.Text) || String.IsNullOrEmpty(etEmail.Text))
                {
                    DisplayAlert("Feedback", "Please enter your name and email Id along with the suggestion/complaint.");
                    return;
                }
                if (String.IsNullOrWhiteSpace(etFeedback.Text) || String.IsNullOrEmpty(etFeedback.Text))
                {
                    DisplayAlert("Feedback", "Please enter a suggestion/complaint and then submit");
                    return;
                }
                if (!IsValidEmail(etEmail.Text))
                {
                    DisplayAlert("Feedback", "Please enter a valid email id.");
                    return;
                }
                Dialog progressDialog = CustomProgressDialog.Show(this.Context,"");
                SubmitFeedback(etFeedback.Text);
                progressDialog.Dismiss();                
            };

            return view;
        }

        public bool IsValidEmail(string sEmail)
        {
            Boolean IsValid = false;
            if (sEmail != null)
            {
                const string emailRegex = @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
           @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$";
                IsValid = Regex.IsMatch(sEmail, emailRegex, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            return IsValid;
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
                    sName = s.name;
                    sEmail = s.email;
                    db_state = s.db_state;
                    sAccountType = s.account_type;
                }
            }
            db.Close();
        }

        private void UpdateAccountInfo()
        {
            /*if (sAccountType == "F")
            {
                txtErrMsg.Visibility = ViewStates.Visible;
                etName.Enabled = false;
                etEmail.Enabled = false;
                btnSubmit.Enabled = false;                
            }
            else*/
            {
                //etName.Text = sName;
                //etEmail.Text = sEmail;
                etName.Enabled = true;
                etEmail.Enabled = true;
                btnSubmit.Enabled = true;
                txtErrMsg.Visibility = ViewStates.Gone;
                
            }
        }

        private void SubmitFeedback(string sFeedback)
        {
            etFeedback.Enabled = false;
            btnSubmit.Enabled = false;
            string sFeedbackType = "";

            if (rbComplaint.Checked)
                sFeedbackType = "COMP";
            if (rbSuggestion.Checked)
                sFeedbackType = "SUGG";

            int iComplaints = 0;
            int iSuggestions = 0;
            if (account_id == 0)
                GetAccountDetails();
            var db = new SQLiteConnection(_databasePath);
            db.CreateTable<FeedbacksData>();
            var table = db.Query<FeedbacksData>("SELECT * FROM [FeedbacksData] WHERE db_state = ? ", db_state);
            if (table.Count > 0)
            {
                foreach (var s in table)
                {
                    iComplaints = iComplaints + s.complaintCount;
                    iSuggestions = iSuggestions + s.suggestionCount;                    
                }
                if (sFeedbackType == "COMP" && iComplaints > 0)
                {
                    DisplayAlert("Feedback", "You have already subbmitted a complaint. Give us some time to resolve the same and get back to you. Thank you for your patience.");
                    return;
                }
                if (sFeedbackType == "SUGG" && iSuggestions > 1)
                {
                    DisplayAlert("Feedback", "We have already recived two suggestions from you today. Let us work on them and get back to you. Thank you again for writing to us.");
                    return;
                }
            }

            try
            {
                var service = new net.azurewebsites.stockscanyweb.SSWebService();
                service.SubmitFeedback(app_unique_id, account_id, sFeedbackType, etName.Text, etEmail.Text, sFeedback);
                var feedback = new FeedbacksData();
                feedback.db_state = db_state;
                if (sFeedbackType == "COMP")
                {
                    feedback.complaintCount = 1;
                    DisplayAlert("Feedback", "We have recieved your complaint. We will do our best to resolve it as soon as possible.");
                }
                else
                {
                    feedback.suggestionCount = 1;
                    DisplayAlert("Feedback", "We have recieved your suggestion. Thank you for writing to us.");
                }
                db.Insert(feedback);
            }
            catch (Exception ex)
            {
                etFeedback.Enabled = true;
                btnSubmit.Enabled = true;
                DisplayAlert("Feedback", "There was some issue in the connection. Please try after some time.");
            }
            db.Close();
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