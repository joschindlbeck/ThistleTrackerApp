using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Gms.Common;
using Android.Gms.Location;
using Android.Gms.Maps;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace ThistleTracker.Droid
{
    public class AndroidLocationService: LocationCallback, ILocationService
    {
        MainActivity mainActivity;
        FusedLocationProviderClient fusedLocationProviderClient;

        //Last reported coords
        Xamarin.Essentials.Location lastLocation = new Xamarin.Essentials.Location(0, 0);

        public event EventHandler LocationChanged;

        protected virtual void OnLocationChanged(EventArgs e)
        {
            LocationChanged?.Invoke(this, e);
        }

        public AndroidLocationService(MainActivity mainActivity)
        {
            this.mainActivity = mainActivity;
            // check if google play is active
            if (!this.IsGooglePlayServicesInstalled())
            {
                // not installed, return exception
                throw new Exception("Google Play Services not installed!");
            }
            fusedLocationProviderClient = LocationServices.GetFusedLocationProviderClient(mainActivity);
            subscribeToLocationUpdates();
        }

        
        public void subscribeToLocationUpdates()
        {
            LocationRequest locationRequest = new LocationRequest()
                          .SetPriority(LocationRequest.PriorityHighAccuracy)
                          .SetInterval(5000); // 5 seconds update
                          //.SetFastestInterval(1000 * 30); // not sooner than 10 seconds

            //async? mit await?
            fusedLocationProviderClient.RequestLocationUpdatesAsync(locationRequest, this);
       

        }


        public bool IsGooglePlayServicesInstalled()
        {
            var queryResult = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(this.mainActivity);
            if (queryResult == ConnectionResult.Success)
            {
                Log.Info("MainActivity", "Google Play Services is installed on this device.");
                return true;
            }

            if (GoogleApiAvailability.Instance.IsUserResolvableError(queryResult))
            {
                // Check if there is a way the user can resolve the issue
                var errorString = GoogleApiAvailability.Instance.GetErrorString(queryResult);
                Log.Error("MainActivity", "There is a problem with Google Play Services on this device: {0} - {1}",
                          queryResult, errorString);
            }

            return false;
        }

        public Xamarin.Essentials.Location getLocation()
        {
            return lastLocation;
        }

        public override void OnLocationAvailability(LocationAvailability locationAvailability)
        {
            Log.Debug("FusedLocationProviderSample", "IsLocationAvailable: {0}", locationAvailability.IsLocationAvailable);
        }

        public override void OnLocationResult(LocationResult result)
        {
            if (result.Locations.Any())
            {
                var location = result.Locations.First();
                Log.Debug("Sample", "The latitude is :" + location.Latitude);
                Log.Debug("Sample", "The longitude is:" + location.Longitude);
                this.lastLocation.Latitude = location.Latitude;
                this.lastLocation.Longitude = location.Longitude;
                this.lastLocation.Altitude = location.Altitude;

                // raise event
                OnLocationChanged(EventArgs.Empty);
            }
            else
            {
                // No locations to work with.
            }
        }
               
    }
}