using System;
using System.Diagnostics;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;
using System.IO;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using System.Net;
using System.Text;
using Windows.UI.Xaml.Media.Animation;
using Bing.Maps;
using System.Runtime.Serialization.Json;
using Windows.Devices.Geolocation;
using Windows.UI.Popups;

namespace cheataxi
{
    public sealed partial class MainPage : Page
    {
        private async void findMe()
        {
            Windows.Devices.Geolocation.Geolocator geolocator = new Windows.Devices.Geolocation.Geolocator();
            Windows.Devices.Geolocation.Geoposition geoposition = await geolocator.GetGeopositionAsync();
            myMap.SetView(new Bing.Maps.Location() { Latitude = geoposition.Coordinate.Latitude, Longitude = geoposition.Coordinate.Longitude }, 13);
            Bing.Maps.MapLayer.SetPosition(pin, new Bing.Maps.Location() { Latitude = geoposition.Coordinate.Latitude, Longitude = geoposition.Coordinate.Longitude });
            myMap.SetView(new Bing.Maps.Location() { Latitude = geoposition.Coordinate.Latitude, Longitude = geoposition.Coordinate.Longitude });
            myMap.Children.Clear();
            myMap.ShapeLayers.Clear();
            Bing.Maps.MapLayer.SetPosition(pin, new Bing.Maps.Location() { Latitude = geoposition.Coordinate.Latitude, Longitude = geoposition.Coordinate.Longitude });
            myMap.Children.Add(pin);
            sourseLocation = new Bing.Maps.Location() { Latitude = geoposition.Coordinate.Latitude, Longitude = geoposition.Coordinate.Longitude };
        }
    }
}