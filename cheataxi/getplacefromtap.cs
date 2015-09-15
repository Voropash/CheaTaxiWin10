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
        private async void getPlaceFromCoordinates(Windows.Foundation.Point pos)
        {
            // Get world coordinates
            myMap.TryPixelToLocation(pos, out location);
            myMap.SetView(location);
            // Form request
            string tmpSourseLoc = Convert.ToString(location.Latitude).Substring(0, Convert.ToString(location.Latitude).IndexOf(',')) + '.' + Convert.ToString(location.Latitude).Substring(Convert.ToString(location.Latitude).IndexOf(',') + 1, Convert.ToString(location.Latitude).Length - Convert.ToString(location.Latitude).IndexOf(',') - 1);
            tmpSourseLoc += "," + Convert.ToString(location.Longitude).Substring(0, Convert.ToString(location.Longitude).IndexOf(',')) + '.' + Convert.ToString(location.Longitude).Substring(Convert.ToString(location.Longitude).IndexOf(',') + 1, Convert.ToString(location.Longitude).Length - Convert.ToString(location.Longitude).IndexOf(',') - 1);
            string address = "http://geocode-maps.yandex.ru/1.x/?geocode=" + tmpSourseLoc + "&sco=latlong";
                //log1.Text += address;
            StorageFile myDB;
            try
            {
                // Send request
                StorageFile tempFile2 = await Windows.Storage.ApplicationData.Current.LocalFolder.CreateFileAsync("multi", CreationCollisionOption.ReplaceExisting);
                BackgroundDownloader manager2 = new BackgroundDownloader();
                var operation = manager2.CreateDownload(new Uri(address), tempFile2);
                IProgress<DownloadOperation> progressH = new Progress<DownloadOperation>((p) =>
                { /* Debug.WriteLine("Transferred: {0}, Total: {1}", p.Progress.BytesReceived, p.Progress.TotalBytesToReceive); */ });
                await operation.StartAsync().AsTask(progressH);
                 //Debug.WriteLine("BacgroundTransfer created");

                // Write answer
                myDB = await Windows.Storage.ApplicationData.Current.LocalFolder.GetFileAsync("multi");
                var alldata = await Windows.Storage.FileIO.ReadLinesAsync(myDB);
                datalines = new string[alldata.Count];
                int ko = 0; int start;
                // Analise answer
                foreach (var line in alldata)
                {
                    datalines[ko] = line.ToString();
                    ko++;
                    if ((start = datalines[ko - 1].IndexOf("<text>")) != -1)
                    {
                        // Print adress
                        int nameLeng = datalines[ko - 1].Length;
                        if (!isSourseTapped) sourseLable.Text = datalines[ko - 1].Substring(start + 6, nameLeng - 6 - start - 7);
                        else aimLable.Text = datalines[ko - 1].Substring(start + 6, nameLeng - 6 - start - 7);
                        break;
                    }
                }
                await myDB.DeleteAsync();
            }
            catch
            {
                MessageDialog dialog = new MessageDialog("Мы не смогли обнаружить ничего рядом с данной меткой. Однако вы все равно можете попробовать построить маршрут, используя её.", "Ничего не найдено!");
            }
        }
    }
}