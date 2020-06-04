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
        public MainPage()
        {
            InitializeComponent();

            //Item source for list view
            SpotsListView.ItemsSource = ThistleTracker._spots;

            // test add spot
            /*
            WeedSpot spot = new WeedSpot("Test", 1.1, 12.3);
            ThistleTracker._spots.Add(spot);
            WeedSpot spot2 = new WeedSpot("Test2", 1.1, 12.3);
            ThistleTracker._spots.Add(spot2);
            */
        }


        private async void WeedButton_Clicked(object sender, EventArgs e)
        {
            // Weed button clicked
            // get geolocation
            var location = await ThistleTracker.getLatLongAsync();

            // add spot
            WeedSpot spot = new WeedSpot(((Button)sender).Text, location.Latitude, location.Longitude);
            ThistleTracker._spots.Add(spot);

        }

        private async void ToolbarItemSave_Clicked(object sender, EventArgs e)
        {
            await ThistleTracker.SaveAsKmlAsync();
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
    }
}
