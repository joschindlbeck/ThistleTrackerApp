using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ThistleTracker
{
    public partial class App : Application
    {
        public static ILocationService locationService { get; private set; }
        public static string externalStorageDirectoryPath;
        public static string externalStorageDirectoryDocumentsPath;

        MainPage startPage;
        
        public static void init(ILocationService locationServiceImpl)
        {
            App.locationService = locationServiceImpl;
        }

        public App()
        {
            InitializeComponent();

            startPage = new MainPage();
           MainPage = new NavigationPage(startPage);

        }

        protected override void OnStart()
        {
            App.locationService.LocationChanged += startPage.LocationChanged;
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
