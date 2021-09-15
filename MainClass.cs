using System;
using Android;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using SQLite;
using StockScany.Classes;
using StockScany.Fragments;
using Android.Gms.Ads;
using Android.Content;

namespace StockScany
{

    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainClass : AppCompatActivity, NavigationView.IOnNavigationItemSelectedListener
    {
        static int iEnableAds = 1;
        Android.Support.V4.App.Fragment frag = null;
        string _databasePath = Xamarin.Essentials.FileSystem.AppDataDirectory + "/SqliteDatabase.db3";
        private InterstitialAd mInterstitialAd;
        string sName = "";
        string sEmail = "";
        int dbState = 0;
        const int iMandatoryVerUpdate = 2000;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            MobileAds.Initialize(ApplicationContext, "ca-app-pub-5647159582339728~8358872565");
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            if (iEnableAds == 1)
            {
                mInterstitialAd = new InterstitialAd(this.BaseContext);
                mInterstitialAd.AdUnitId = "ca-app-pub-5647159582339728/1020272746";
                mInterstitialAd.LoadAd(new AdRequest.Builder().Build());
            }
            //FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            //fab.Click += FabOnClick;

            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            ActionBarDrawerToggle toggle = new ActionBarDrawerToggle(this, drawer, toolbar, Resource.String.navigation_drawer_open, Resource.String.navigation_drawer_close);
            drawer.AddDrawerListener(toggle);
            toggle.SyncState();

            NavigationView navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            navigationView.SetNavigationItemSelectedListener(this);

            View header = navigationView.GetHeaderView(0);
            TextView txtHeaderName = (TextView)header.FindViewById<TextView>(Resource.Id.txtHeaderName);
            TextView txtHeaderMail = (TextView)header.FindViewById<TextView>(Resource.Id.txtHeaderMail);

            GetAccountDetails();
            if (sName != "*" && sName.Length > 1)
            {
                txtHeaderName.Text = "StockScany 4.0"; // + sName;
                txtHeaderMail.Text = "" + sEmail;
            }
            else
            {
                txtHeaderName.Text = "StockScany 4.0";
                txtHeaderMail.Text = "";
            }            

            frag = new WelcomeFragment();
            var welcomeTransaction = SupportFragmentManager.BeginTransaction();
            welcomeTransaction.Add(Resource.Id.fragment_container, frag, "WelcomeFragment");
            welcomeTransaction.Commit();                        
        }        

        public override void OnBackPressed()
        {
            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            if (drawer.IsDrawerOpen(GravityCompat.Start))
            {
                drawer.CloseDrawer(GravityCompat.Start);
            }
            else
            {
                Android.Support.V4.App.Fragment myFragment = SupportFragmentManager.FindFragmentByTag("WelcomeFragment");
                if (myFragment != null && myFragment.IsVisible)
                {
                    base.OnBackPressed();
                    // add your code here
                }
                else
                {
                    Android.Support.V4.App.Fragment frag = new WelcomeFragment();
                    var welcomeTransaction = SupportFragmentManager.BeginTransaction();
                    welcomeTransaction.Replace(Resource.Id.fragment_container, frag, "WelcomeFragment");
                    welcomeTransaction.Commit();
                }
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            //if (id == Resource.Id.action_settings)
            //{
                return true;
            //}

            //return base.OnOptionsItemSelected(item);
        }

        private void FabOnClick(object sender, EventArgs eventArgs)
        {
            View view = (View)sender;
            Snackbar.Make(view, "Replace with your own action", Snackbar.LengthLong)
                .SetAction("Action", (Android.Views.View.IOnClickListener)null).Show();
        }

        private void GetAccountDetails()
        {
            try
            {
                var db = new SQLiteConnection(_databasePath);
                var table = db.Query<AccountInfo>("SELECT * FROM [AccountInfo] ");
                if (table.Count > 0)
                {
                    foreach (var s in table)
                    {
                        sName = s.name;
                        sEmail = s.email;
                        dbState = s.db_state;
                    }
                }
                db.Close();
            }
            catch (Exception ex)
            {
                sName = "*";
            }
        }

        public bool OnNavigationItemSelected(IMenuItem item)
        {
            GetAccountDetails();
            if (dbState < iMandatoryVerUpdate)
            {

                int id = item.ItemId;

                if (id == Resource.Id.nav_basic)
                {
                    ListItemClicked(0);
                }
                else if (id == Resource.Id.nav_adv)
                {
                    ListItemClicked(1);
                }
                else if (id == Resource.Id.nav_complex)
                {
                    ListItemClicked(2);
                }
                else if (id == Resource.Id.nav_account)
                {
                    ListItemClicked(3);
                }
                else if (id == Resource.Id.nav_share)
                {
                    ListItemClicked(4);
                }
                else if (id == Resource.Id.nav_send)
                {
                    ListItemClicked(5);
                }

                DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
                drawer.CloseDrawer(GravityCompat.Start);
                return true;
            }
            else
            {
                Android.App.AlertDialog.Builder dialog = new Android.App.AlertDialog.Builder(this);
                Android.App.AlertDialog alert = dialog.Create();
                alert.SetTitle("App Update");
                alert.SetMessage("Please update the app from play store. The current version is not supported anymore and won't get the stock updates.");
                alert.SetButton("OK", (c, ev) =>
                {
                    alert.Cancel();
                });
                alert.Show();
                return false;
            }
        }

        private void ListItemClicked(int position)
        {
            if (iEnableAds == 1 && mInterstitialAd.IsLoaded)
            {
                mInterstitialAd.Show();
            }

            switch (position)
            {
                case 0:
                    frag = new BasicScans();
                    break;
                case 1:
                    frag = new AdvScans();
                    break;
                case 2:
                    frag = new WatchlistActivity();
                    break;
                case 3:
                    frag = new Account();
                    break;
                case 4:
                    ShareApp();
                    break;
                case 5:
                    frag = new Feedback();
                    break;
            }
            if (frag != null && position != 4)
            {
                var menuTransaction = SupportFragmentManager.BeginTransaction();
                menuTransaction.Replace(Resource.Id.fragment_container, frag, "Fragment1");
                menuTransaction.Commit();
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public void ShareApp()
        {
            String shareBody = "*";
            var service = new net.azurewebsites.stockscanyweb.SSWebService();
            var db = new SQLiteConnection(_databasePath);
            var table = db.Query<AccountInfo>("SELECT * FROM [AccountInfo] ");
            if (table.Count > 0)
            {
                string app_unique_id = "";
                int account_id = 0;
                foreach (var s in table)
                {
                    app_unique_id = s.unique_id;
                    account_id = s.account_id;
                    
                }
                shareBody = service.GetShareLink(app_unique_id, account_id);
            }
            db.Close();

            if (shareBody != "*")
            {
                Intent sharingIntent = new Intent(Intent.ActionSend);
                sharingIntent.SetType("text/plain");
                sharingIntent.PutExtra(Intent.ExtraSubject, "Share App");
                sharingIntent.PutExtra(Intent.ExtraText, shareBody);
                StartActivity(Intent.CreateChooser(sharingIntent, "Share via"));
            }
        }
    }
}

