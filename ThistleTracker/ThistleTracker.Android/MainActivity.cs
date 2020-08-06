using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using ThistleTracker;

namespace ThistleTracker.Droid
{
    [Activity(Label = "ThistleTracker", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());

            // set location service
            try
            {
                App.init(new AndroidLocationService(this));
            }catch(Exception e)
            {
                // exception occurred!
                string msg = "Exception during Location Service initilization: \"" + e.Message + "\" Using fallback LocationService!";
                Toast toast = Toast.MakeText(this, msg, ToastLength.Short);
                toast.Show();
                App.init(locationServiceImpl: new LocationServiceXamEss());
            }

            // set Path to external storage directory
            App.externalStorageDirectoryPath = this.GetExternalFilesDir(null).AbsolutePath;
            App.externalStorageDirectoryDocumentsPath = this.GetExternalFilesDir(Android.OS.Environment.DirectoryDocuments).AbsolutePath;
            
        }
            
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}