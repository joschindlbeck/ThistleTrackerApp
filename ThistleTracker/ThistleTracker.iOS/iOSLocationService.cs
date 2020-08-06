using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using Xamarin.Essentials;

namespace ThistleTracker.iOS
{
    class iOSLocationService : ILocationService
    {
        public event EventHandler LocationChanged;

        public Location getLocation()
        {
            throw new NotImplementedException();
        }

        public iOSLocationService()
        {
            throw new NotImplementedException();
        }
    }
}