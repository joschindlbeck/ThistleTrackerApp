using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Xamarin.Essentials;

namespace ThistleTracker
{
    public class ThistleTracker
    {
        // List of recorded spots
        public ObservableCollection<WeedSpot> _spots = new ObservableCollection<WeedSpot>();

        public async System.Threading.Tasks.Task<Location> getLatLongAsync()
        {
            try
            {
                // Geo Request
                var request = new GeolocationRequest(GeolocationAccuracy.Best);
                var location = await Geolocation.GetLocationAsync(request);

                // Return                  
                if (location != null)
                {
                    return location;
                }
                else
                {
                    return new Location(0, 0);
                }


            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Handle not supported on device exception
                Debug.WriteLine(fnsEx.Message);
                return new Location(0, 0);
            }
            catch (FeatureNotEnabledException fneEx)
            {
                // Handle not enabled on device exception
                Debug.WriteLine(fneEx.Message);
                return new Location(0, 0);
            }
            catch (PermissionException pEx)
            {
                // Handle permission exception
                Debug.WriteLine(pEx.Message);
                return new Location(0, 0);
            }
            catch (Exception ex)
            {
                // Unable to get location
                Debug.WriteLine(ex.Message);
                return new Location(0, 0);
            }
        }
    }

}

public class WeedSpot
    {
        public string Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        
        public string DisplayName
    {
        get
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Name);
            sb.Append(": ");
            sb.Append(Latitude);
            sb.Append(", ");
            sb.Append(Longitude);
            return sb.ToString();
        }
    }

        public WeedSpot(string name, double lat, double lon)
        {
            Name = name;
            Latitude = lat;
            Longitude = lon;
        }
}
