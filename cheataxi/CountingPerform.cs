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
        // Drow dot, start main perform, drow path 
        private async void contextPerform()
        {
            if (!isSourseTapped)
            {
                myMap.Children.Clear();
                myMap.ShapeLayers.Clear();
                myMap.Children.Add(pin);
                Bing.Maps.MapLayer.SetPosition(pin, location);
                sourseLocation = location;
                isSourseTapped = true;
                tipLable.Text = "Введите или выберите на карте конечную точку маршрута:";
                sourseLable.Visibility = Visibility.Visible;
                sourseLableBG.Visibility = Visibility.Visible;
                sourseLableHead.Visibility = Visibility.Visible;
                aimLable.Visibility = Visibility.Collapsed;
                aimLableBG.Visibility = Visibility.Collapsed;
                aimLableHead.Visibility = Visibility.Collapsed;
            }
            else
            {
                Bing.Maps.MapLayer.SetPosition(pinAim, location);
                AimLocation = location;
                myMap.Children.Add(pinAim);
                // Main Perform
                await countingPerform();
                // If distance is not very big
                if (flagdistance)
                {
                    await DrawPath();
                }
                else
                {
                    MessageDialog dialog = new MessageDialog("Такое расстояние легче преодолеть на самолете", "Такси?");
                    await dialog.ShowAsync();
                }
                isSourseTapped = false;
                sourseLable.Visibility = Visibility.Visible;
                sourseLableBG.Visibility = Visibility.Visible;
                sourseLableHead.Visibility = Visibility.Visible;
                aimLable.Visibility = Visibility.Visible;
                aimLableBG.Visibility = Visibility.Visible;
                aimLableHead.Visibility = Visibility.Visible;
                getResult();
            }
        }

        private async Task countingPerform()
        {
            // Forming request
            string tmpSourseLoc = Convert.ToString(sourseLocation.Latitude).Substring(0, Convert.ToString(sourseLocation.Latitude).IndexOf(',')) + '.' + Convert.ToString(sourseLocation.Latitude).Substring(Convert.ToString(sourseLocation.Latitude).IndexOf(',') + 1, Convert.ToString(sourseLocation.Latitude).Length - Convert.ToString(sourseLocation.Latitude).IndexOf(',') - 1);
            tmpSourseLoc += "," + Convert.ToString(sourseLocation.Longitude).Substring(0, Convert.ToString(sourseLocation.Longitude).IndexOf(',')) + '.' + Convert.ToString(sourseLocation.Longitude).Substring(Convert.ToString(sourseLocation.Longitude).IndexOf(',') + 1, Convert.ToString(sourseLocation.Longitude).Length - Convert.ToString(sourseLocation.Longitude).IndexOf(',') - 1);
            string tmpAimLoc = Convert.ToString(AimLocation.Latitude).Substring(0, Convert.ToString(AimLocation.Latitude).IndexOf(',')) + '.' + Convert.ToString(AimLocation.Latitude).Substring(Convert.ToString(AimLocation.Latitude).IndexOf(',') + 1, Convert.ToString(AimLocation.Latitude).Length - Convert.ToString(AimLocation.Latitude).IndexOf(',') - 1);
            tmpAimLoc += "," + Convert.ToString(AimLocation.Longitude).Substring(0, Convert.ToString(AimLocation.Longitude).IndexOf(',')) + '.' + Convert.ToString(AimLocation.Longitude).Substring(Convert.ToString(AimLocation.Longitude).IndexOf(',') + 1, Convert.ToString(AimLocation.Longitude).Length - Convert.ToString(AimLocation.Longitude).IndexOf(',') - 1);
            string address = "https://maps.googleapis.com/maps/api/directions/xml?origin=(" + tmpSourseLoc + "&destination=" + tmpAimLoc + "&KEY=AIzaSyBrweENi7gpNQd23mLEWr9g3OuvXBq0LBA";
            log1.Text += address;
            StorageFile myDB;
            try
            {
                // Save answer
                StorageFile tempFile2 = await Windows.Storage.ApplicationData.Current.LocalFolder.CreateFileAsync("route", CreationCollisionOption.ReplaceExisting);
                BackgroundDownloader manager2 = new BackgroundDownloader();
                var operation = manager2.CreateDownload(new Uri(address), tempFile2);
                IProgress<DownloadOperation> progressH = new Progress<DownloadOperation>((p) =>
                    { /*Debug.WriteLine("Transferred: {0}, Total: {1}", p.Progress.BytesReceived, p.Progress.TotalBytesToReceive);*/ });
                await operation.StartAsync().AsTask(progressH);
                    //Debug.WriteLine("BacgroundTransfer created");

                // Read the data
                myDB = await Windows.Storage.ApplicationData.Current.LocalFolder.GetFileAsync("route");
                var alldata = await Windows.Storage.FileIO.ReadLinesAsync(myDB);
                string[] datalines = new string[alldata.Count];
                    int ko = 0; // counter
                    int start;
                    string distance = "";
                    bool flagYet = false;
                    bool flagend = true; // semaphor for check 1or2 hashtag. 
                    bool flagDuration = false;
                foreach (var line in alldata)
                {
                    // get new string
                    datalines[ko] = line.ToString();

                    // in <duration>
                    if (flagDuration)
                    {
                        // recheck next hashtag
                        if ((start = datalines[ko].IndexOf("<value>")) == -1)
                        {
                            MessageDialog dialog = new MessageDialog("Не удалось построить маршрут. Попробуйте снова. \r\nЕсли ошибка повторится вновь, просим уведомить нас о ней через форму обратной связи.", "Ошибка #007002");
                            await dialog.ShowAsync();
                            await myDB.DeleteAsync();
                            break;
                        }
                        // print duratuion
                        int durationStringLenght = datalines[ko].Length;
                        distance = datalines[ko].Substring(start + 7, durationStringLenght - 7 - start - 8);
                        timeResult.Text = "Предположительное время в дороге " + distance + " секунд\t";
                        ko++;
                        flagDuration = false;
                        continue;
                    }
                    // in <distance>
                    if (flagYet)
                    {
                        // recheck next hashtag
                        if ((start = datalines[ko].IndexOf("<value>")) == -1)
                        {
                            MessageDialog dialog = new MessageDialog("Маршрут построить не удалось. Попробуйте снова. \r\nЕсли ошибка повторится вновь, просим уведомить нас о ней через форму обратной связи.", "Ошибка #007003");
                            await dialog.ShowAsync();
                            await myDB.DeleteAsync();
                            break;
                        }
                        //print distance
                        int distanceStringLenght = datalines[ko].Length;
                        distance = datalines[ko].Substring(start + 7, distanceStringLenght - 7 - start - 8);
                            //Debug.WriteLine(distance);
                            //Debug.WriteLine(datalines[ko]);
                        distanceResult.Text = "Длина оптимального маршрута " + distance + " метров";
                        if (Convert.ToInt64(distance) > MAX_LENGTH_ROUTE)
                        {
                            flagdistance = false;
                        }
                        else
                        {
                            flagdistance = true;
                        }
                        ko++;
                        break;
                    }
                    
                    ko++;
                    // status zero in answer
                    if ((start = datalines[ko - 1].IndexOf("<status>ZERO")) != -1)
                    {
                        MessageDialog dialog = new MessageDialog("Маршрут построить не удалось. Попробуйте снова. \r\nЕсли ошибка повторится вновь, просим уведомить нас о ней через форму обратной связи.", "Ошибка #007004");
                        await dialog.ShowAsync();
                        await myDB.DeleteAsync();
                        break;
                    }
                    // status not in answer
                    if ((start = datalines[ko - 1].IndexOf("<status>NOT")) != -1)
                    {
                        textBox.Text = "Маршрут не найден!";
                        break;
                    }
                    // change semaphore values
                    if (flagend && (datalines[ko - 1].IndexOf("<step>")) != -1)
                    {
                        flagend = false;
                    }
                    if (!flagend && (datalines[ko - 1].IndexOf("</step>")) != -1)
                    {
                        flagend = true;
                    }
                    if (flagend && (start = datalines[ko - 1].IndexOf("<duration>")) != -1)
                    {
                        flagDuration = true;
                    }
                    if (flagend && (start = datalines[ko - 1].IndexOf("<distance>")) != -1)
                    {
                        flagYet = true;
                    }
                }
                await myDB.DeleteAsync();
            }
            catch (Exception)
            {
                textBlock1.Text = "Info Error"; // Some errors with file
            }
        }
    }
}