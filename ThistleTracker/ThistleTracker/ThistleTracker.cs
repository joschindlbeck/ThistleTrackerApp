using PCLStorage;
using SharpKml.Base;
using SharpKml.Dom;
using SharpKml.Dom.GX;
using SharpKml.Engine;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using ThistleTracker;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.PlatformConfiguration;
using Timestamp = SharpKml.Dom.Timestamp;

namespace ThistleTracker
{
    public class ThistleTracker
    {
        // List of recorded spots
        public ObservableCollection<WeedSpot> _spots = new ObservableCollection<WeedSpot>();

        // Button Profiles
        public ButtonProfiles buttonProfiles = new ButtonProfiles();
 
        public ThistleTracker()
        {

        }

        public async System.Threading.Tasks.Task ExportAsKmlAsync()
        {
            Kml kml = createKML();
            string fileName = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ThistleTracker.kml");
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

        public async System.Threading.Tasks.Task<string> SaveAsKmlAsync()
        {
            // create KML
            Kml kml = createKML();


            // get external / shared storage for documents
            PCLStorage.IFolder rootFolder = await PCLStorage.FileSystem.Current.GetFolderFromPathAsync(App.externalStorageDirectoryDocumentsPath);
            Debug.WriteLine("RootFolder " + rootFolder.Path);
            StringBuilder fileName = new StringBuilder("ThistleTracker_").Append(DateTime.Now.ToString("yyyyMMdd")).Append(".kml");         
            PCLStorage.IFile file = await rootFolder.CreateFileAsync(fileName.ToString(), PCLStorage.CreationCollisionOption.GenerateUniqueName);
            
            //Create KML file
            KmlFile kmlFile = KmlFile.Create(kml, true);
            using(FileStream stream = (FileStream)await file.OpenAsync(PCLStorage.FileAccess.ReadAndWrite))
            {
                kmlFile.Save(stream);
            }

            //return file string
            return file.Path;
        }

        private Kml createKML()
        {
            Kml kml = new Kml();
            Document document = new Document();
            document.Name = "Weeds from ThistleTracker";
            kml.Feature = document;
            // Loop & add weedspots
            foreach (WeedSpot spot in _spots)
            {
                SharpKml.Dom.Point point = new SharpKml.Dom.Point();
                point.Coordinate = new Vector(spot.Latitude, spot.Longitude, spot.Altitude);
                SharpKml.Dom.Placemark place = new SharpKml.Dom.Placemark();
                place.Name = spot.Name;
                var time = new Timestamp();
                time.When = spot.DateTimeAdded;
                place.Time = time;
                place.Geometry = point;
                document.AddFeature(place);
            }

            return kml;
        }
    }

    public class WeedSpot
    {
        public string Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Altitude { get; set; }
        public DateTime DateTimeAdded { get; set; }

        public string LatLongDisplay
        {
            get
            {
                return Math.Round(Latitude, 4).ToString() + " / " + Math.Round(Longitude, 4).ToString();
            }
        }

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
            Altitude = 0; // not yet supported
            DateTimeAdded = DateTime.Now;
        }
    }

    /// <summary>
    /// Implementation of the Location Service with Xamarin Essentials
    /// </summary>
    public class LocationServiceXamEss : ILocationService
    {
        public event EventHandler LocationChanged;

        public Xamarin.Essentials.Location getLocation()
        {
            // requtest LatLong Async
            Task<Xamarin.Essentials.Location> locTask = getLatLongAsync();
            // wait for completion and return
            locTask.Wait();
            return locTask.Result;
        }

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
    }

    public class ButtonProfile
    {
        // Name of profile
        protected string Name;

        // List for Button descriptions
        protected List<string> ButtonTexts = new List<string>();

        public ButtonProfile(string name)
        {
            Name = name;
        }

        public void applyProfile(FlexLayout flexLayout, EventHandler btnClickEventHandler)
        {
 
            // remove old buttons if there are some
            flexLayout.Children.Clear();

            // add buttons from current profile
            foreach(string buttonText in ButtonTexts)
            {
                Button btn = new Button();
                btn.Text = buttonText;
                btn.WidthRequest = 120;
                btn.HeightRequest = 120;
                btn.Clicked += btnClickEventHandler;
                flexLayout.Children.Add(btn);
            }
        }

        public void addButton(int index, string text)
        {
            this.ButtonTexts.Insert(index, text);
        }

        public static ButtonProfile getDefaultProfile()
        {
            ButtonProfile defProfile = new ButtonProfile("Default");
            defProfile.ButtonTexts.Add("Distel");
            defProfile.ButtonTexts.Add("Ackerwinde");
            defProfile.ButtonTexts.Add("Hundspetersilie");
            defProfile.ButtonTexts.Add("Windenknöterich");
            defProfile.ButtonTexts.Add("Schachtelhalm");
            defProfile.ButtonTexts.Add("Sonstiges");
            return defProfile;
        }

    }

    public class ButtonProfiles
    {
        // ButtonProfiles
        public List<ButtonProfile> buttonProfiles = new List<ButtonProfile>();
        int currentProfileIndex = 0;

        public ButtonProfiles()
        {
            buttonProfiles.Add(ButtonProfile.getDefaultProfile());
            currentProfileIndex = 0;
        }

        public ButtonProfile getCurrent()
        {
            return buttonProfiles[currentProfileIndex];
        }
    }
}
