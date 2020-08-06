using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;


namespace ThistleTracker
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        // Application class
        ThistleTracker ThistleTracker = new ThistleTracker();
        public ObservableCollection<WeedSpot> WeedSpots { get { return ThistleTracker._spots; } }
        
        private string _lat;
        public String lat { get { return _lat; } set { _lat = value; OnPropertyChanged(); } }
        private string _lon;
        public String lon { get { return _lon; } set { _lon = value; OnPropertyChanged(); } }

        public MainPage()
        {
            InitializeComponent();
            
            //Item source for list view
            SpotsListView.ItemsSource = ThistleTracker._spots;

            // label binding
            lblLat.BindingContext = this;
            lblLat.SetBinding(Label.TextProperty, "lat");
            lblLong.BindingContext = this;
            lblLong.SetBinding(Label.TextProperty, "lon");

            // add button profile
            this.ThistleTracker.buttonProfiles.getCurrent().applyProfile(flButtons, this.WeedButton_Clicked);

        }

        

        public void LocationChanged(object sender, EventArgs e)
        {
            var location = App.locationService.getLocation();
            lat = location.Latitude.ToString();
            lon = location.Longitude.ToString();
        }


        private void WeedButton_Clicked(object sender, EventArgs e)
        {

            // Get Geolocation
            var location = App.locationService.getLocation();

            // add spot
            WeedSpot spot = new WeedSpot(((Button)sender).Text, location.Latitude, location.Longitude);
            ThistleTracker._spots.Insert(0,spot);

        }

        private async void ToolbarItemSave_Clicked(object sender, EventArgs e)
        {
            //save
            string fileName = await ThistleTracker.SaveAsKmlAsync();
            //display message
            bool doClear = await DisplayAlert("File Saved", "File saved to " + fileName + ". Do you want to clear the list?", "Yes", "No");
            if (doClear)
            {
                ThistleTracker._spots.Clear();
            }
            
        }
        private async void ToolbarItemClear_ClickedAsync(object sender, EventArgs e)
        {
            // clear list
            bool doClear = await DisplayAlert("Clear List", "Do you really want to clear the list?", "Yes", "No");
            if (doClear)
            {
                ThistleTracker._spots.Clear();
            }
        }

        private async void ToolbarItemExport_Clicked(object sender, EventArgs e)
        {
            await ThistleTracker.ExportAsKmlAsync();
        }
    }
}
