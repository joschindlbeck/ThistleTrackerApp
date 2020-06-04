using SharpKml.Base;
using SharpKml.Dom;
using SharpKml.Engine;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using Xamarin.Essentials;

namespace ThistleTracker
{
    public class ThistleTracker
    {
        // List of recorded spots
        public ObservableCollection<WeedSpot> _spots = new ObservableCollection<WeedSpot>();

        public async System.Threading.Tasks.Task<Xamarin.Essentials.Location> getLatLongAsync()
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
                    return new Xamarin.Essentials.Location(0, 0);
                }


            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Handle not supported on device exception
                Debug.WriteLine(fnsEx.Message);
                return new Xamarin.Essentials.Location(0, 0);
            }
            catch (FeatureNotEnabledException fneEx)
            {
                // Handle not enabled on device exception
                Debug.WriteLine(fneEx.Message);
                return new Xamarin.Essentials.Location(0, 0);
            }
            catch (PermissionException pEx)
            {
                // Handle permission exception
                Debug.WriteLine(pEx.Message);
                return new Xamarin.Essentials.Location(0, 0);
            }
            catch (Exception ex)
            {
                // Unable to get location
                Debug.WriteLine(ex.Message);
                return new Xamarin.Essentials.Location(0, 0);
            }
        }

        public async System.Threading.Tasks.Task SaveAsKmlAsync()
        {

            Kml kml = new Kml();
            Document document = new Document();
            document.Name = "Weeds from ThistleTracker";
            kml.Feature = document;
            // Loop & add weedspots
            foreach(WeedSpot spot in _spots)
            {
                Point point = new Point();
                point.Coordinate = new Vector(spot.Latitude, spot.Longitude);
                SharpKml.Dom.Placemark place = new SharpKml.Dom.Placemark();
                place.Name = spot.Name;
                place.Geometry = point;
                document.AddFeature(place);
            }
            string fileName = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Weeds.kml");
            // delete if existing
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }

            // Create KML file
            KmlFile kmlFile = KmlFile.Create(kml, true);
            using (FileStream stream = File.OpenWrite(fileName))
            {
                kmlFile.Save(stream);
            }

            // Share with other apps
            await Share.RequestAsync(new ShareFileRequest
            {
                Title = "ThistleTracker KML Export",
                File = new ShareFile(fileName)
            });
        }
    }

}

public class WeedSpot
    {
        public string Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime DateTimeAdded { get; set; }

        public string LatLongDisplay { get
        {
            return Math.Round(Latitude, 4).ToString() + " / " + Math.Round(Longitude, 4).ToString();
        } }

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
            DateTimeAdded = DateTime.Now;
        }
}
