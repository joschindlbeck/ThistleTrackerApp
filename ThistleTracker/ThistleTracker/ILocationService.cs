using System;
using System.Collections.Generic;
using System.Text;

namespace ThistleTracker
{
    public interface ILocationService
    {
        
        Xamarin.Essentials.Location getLocation();
        event EventHandler LocationChanged;
        
    }
}
