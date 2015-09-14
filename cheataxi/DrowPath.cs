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
        private async Task<BingMapHelper.Response> GetResponse(Uri uri)
        {
            System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
            var response = await client.GetAsync(uri);
            using (var stream = await response.Content.ReadAsStreamAsync())
            {
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(BingMapHelper.Response));
                return ser.ReadObject(stream) as BingMapHelper.Response;
            }
        }

        async private Task DrawPath()
        {
            loading.Visibility = Visibility.Visible;


            string tmpSourseLoc = Convert.ToString(sourseLocation.Latitude).Substring(0, Convert.ToString(sourseLocation.Latitude).IndexOf(',')) + '.' + Convert.ToString(sourseLocation.Latitude).Substring(Convert.ToString(sourseLocation.Latitude).IndexOf(',') + 1, Convert.ToString(sourseLocation.Latitude).Length - Convert.ToString(sourseLocation.Latitude).IndexOf(',') - 1);
            tmpSourseLoc += "," + Convert.ToString(sourseLocation.Longitude).Substring(0, Convert.ToString(sourseLocation.Longitude).IndexOf(',')) + '.' + Convert.ToString(sourseLocation.Longitude).Substring(Convert.ToString(sourseLocation.Longitude).IndexOf(',') + 1, Convert.ToString(sourseLocation.Longitude).Length - Convert.ToString(sourseLocation.Longitude).IndexOf(',') - 1);
            string tmpAimLoc = Convert.ToString(AimLocation.Latitude).Substring(0, Convert.ToString(AimLocation.Latitude).IndexOf(',')) + '.' + Convert.ToString(AimLocation.Latitude).Substring(Convert.ToString(AimLocation.Latitude).IndexOf(',') + 1, Convert.ToString(AimLocation.Latitude).Length - Convert.ToString(AimLocation.Latitude).IndexOf(',') - 1);
            tmpAimLoc += "," + Convert.ToString(AimLocation.Longitude).Substring(0, Convert.ToString(AimLocation.Longitude).IndexOf(',')) + '.' + Convert.ToString(AimLocation.Longitude).Substring(Convert.ToString(AimLocation.Longitude).IndexOf(',') + 1, Convert.ToString(AimLocation.Longitude).Length - Convert.ToString(AimLocation.Longitude).IndexOf(',') - 1);

            var url = "http://dev.virtualearth.net/REST/V1/Routes/Driving?o=json&wp.0=" +
                        tmpSourseLoc +
                        "&wp.1=" +
                        tmpAimLoc +
                        "&optmz=distance&rpo=Points&key=" + "2YNtFFMLLBJhNArw7AGF~J1bD5V8AXTm26ao0o20rRw~AqSRI8WDH0Mx820MLWME4SM-ye-FewEHmO7vj5O4vSDrNdVWsnRXo3I-LUhllJrB";

            Uri geocodeRequest = new Uri(url);
            BingMapHelper.Response r = await GetResponse(geocodeRequest);

            if (r.StatusCode != 404)
            {
                myMap.Children.Clear();
                myMap.ShapeLayers.Clear();

                geolocator = new Geolocator();

                MapPolyline routeLine = new MapPolyline();
                routeLine.Locations = new LocationCollection();
                routeLine.Color = Windows.UI.Colors.RoyalBlue;
                routeLine.Width = 6.0;

                int bound = ((BingMapHelper.Route)(r.ResourceSets[0].Resources[0])).
                    RoutePath.Line.Coordinates.GetUpperBound(0);

                sourseLocation.Latitude = ((BingMapHelper.Route)(r.ResourceSets[0].Resources[0])).
                    RoutePath.Line.Coordinates[0][0];
                sourseLocation.Longitude = ((BingMapHelper.Route)(r.ResourceSets[0].Resources[0])).
                    RoutePath.Line.Coordinates[0][1];

                AimLocation.Latitude = ((BingMapHelper.Route)(r.ResourceSets[0].Resources[0])).
                    RoutePath.Line.Coordinates[bound][0];
                AimLocation.Longitude = ((BingMapHelper.Route)(r.ResourceSets[0].Resources[0])).
                    RoutePath.Line.Coordinates[bound][1];

                //var sourcePin = new Pushpin();
                var sourceLocation = new Bing.Maps.Location(sourseLocation.Latitude, sourseLocation.Longitude);
                // MapLayer.SetPosition(sourcePin, sourceLocation);
                myMap.Children.Add(pin);
                myMap.Children.Add(pinAim);

                var destinationLocation = new Bing.Maps.Location(AimLocation.Latitude, AimLocation.Longitude);
                //MapLayer.SetPosition(pin, destinationLocation);
                //myMap.Children.Add(pin);

                myMap.SetView(sourceLocation, myMap.ZoomLevel);

                for (int i = 0; i < bound; i++)
                {
                    routeLine.Locations.Add(new Location
                    {
                        Latitude = ((BingMapHelper.Route)(r.ResourceSets[0].Resources[0])).
                        RoutePath.Line.Coordinates[i][0],
                        Longitude = ((BingMapHelper.Route)(r.ResourceSets[0].Resources[0])).
                        RoutePath.Line.Coordinates[i][1]
                    });
                }

                MapShapeLayer shapeLayer = new MapShapeLayer();
                shapeLayer.Shapes.Add(routeLine);
                myMap.ShapeLayers.Add(shapeLayer);
            }
            else
            {
                MessageDialog dialog = new MessageDialog("Маршрут построить не удалось. Попробуйте снова. \r\nЕсли ошибка повторится вновь, просим уведомить нас о ней через форму обратной связи.", "Ошибка построения маршрута");
                await dialog.ShowAsync();
            }

            loading.Visibility = Visibility.Collapsed;
        }
    }
}