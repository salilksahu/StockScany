
using StockScany.Classes;
using StockScany.Fragments;


namespace StockScany
{
    /*
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : AppCompatActivity, NavigationView.IOnNavigationItemSelectedListener
    {
        Android.Support.V4.App.Fragment frag = null;
        string _databasePath = Xamarin.Essentials.FileSystem.AppDataDirectory + "/SqliteDatabase.db3";
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

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
            var db = new SQLiteConnection(_databasePath);
            db.CreateTable<AccountInfo>();
            var table = db.Query<AccountInfo>("SELECT * FROM [AccountInfo] ");
            if (table.Count > 0)
            {
                foreach (var s in table)
                {
                    txtHeaderName.Text = "Welcome " + s.name;
                    txtHeaderMail.Text = "" + s.email;
                }
            }
            else
            {
                txtHeaderName.Text = "Welcome Guest";
                txtHeaderMail.Text = "";
            }           

            frag = new Welcome();
            var welcomeTransaction = SupportFragmentManager.BeginTransaction();
            welcomeTransaction.Add(Resource.Id.fragment_container, frag, "Welcome");
            welcomeTransaction.Commit();
        }

        public override void OnBackPressed()
        {
            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            if(drawer.IsDrawerOpen(GravityCompat.Start))
            {
                drawer.CloseDrawer(GravityCompat.Start);
            }
            else
            {
                Android.Support.V4.App.Fragment myFragment = SupportFragmentManager.FindFragmentByTag("Welcome");
                if (myFragment != null && myFragment.IsVisible)
                {
                    base.OnBackPressed();
                    // add your code here
                }
                else
                {
                    Android.Support.V4.App.Fragment frag = new Welcome();
                    var welcomeTransaction = SupportFragmentManager.BeginTransaction();
                    welcomeTransaction.Replace(Resource.Id.fragment_container, frag, "Welcome");                    
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
            if (id == Resource.Id.action_settings)
            {
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        private void FabOnClick(object sender, EventArgs eventArgs)
        {
            View view = (View) sender;
            Snackbar.Make(view, "Replace with your own action", Snackbar.LengthLong)
                .SetAction("Action", (Android.Views.View.IOnClickListener)null).Show();
        }

        public bool OnNavigationItemSelected(IMenuItem item)
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

            }
            else if (id == Resource.Id.nav_send)
            {

            }

            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            drawer.CloseDrawer(GravityCompat.Start);
            return true;
        }

        private void ListItemClicked(int position)
        {
            
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
            }
            if (frag != null)
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
    } */
}

