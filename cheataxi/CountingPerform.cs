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
        private async Task countingPerform()
        {
            string tmpSourseLoc = Convert.ToString(sourseLocation.Latitude).Substring(0, Convert.ToString(sourseLocation.Latitude).IndexOf(',')) + '.' + Convert.ToString(sourseLocation.Latitude).Substring(Convert.ToString(sourseLocation.Latitude).IndexOf(',') + 1, Convert.ToString(sourseLocation.Latitude).Length - Convert.ToString(sourseLocation.Latitude).IndexOf(',') - 1);
            tmpSourseLoc += "," + Convert.ToString(sourseLocation.Longitude).Substring(0, Convert.ToString(sourseLocation.Longitude).IndexOf(',')) + '.' + Convert.ToString(sourseLocation.Longitude).Substring(Convert.ToString(sourseLocation.Longitude).IndexOf(',') + 1, Convert.ToString(sourseLocation.Longitude).Length - Convert.ToString(sourseLocation.Longitude).IndexOf(',') - 1);
            string tmpAimLoc = Convert.ToString(AimLocation.Latitude).Substring(0, Convert.ToString(AimLocation.Latitude).IndexOf(',')) + '.' + Convert.ToString(AimLocation.Latitude).Substring(Convert.ToString(AimLocation.Latitude).IndexOf(',') + 1, Convert.ToString(AimLocation.Latitude).Length - Convert.ToString(AimLocation.Latitude).IndexOf(',') - 1);
            tmpAimLoc += "," + Convert.ToString(AimLocation.Longitude).Substring(0, Convert.ToString(AimLocation.Longitude).IndexOf(',')) + '.' + Convert.ToString(AimLocation.Longitude).Substring(Convert.ToString(AimLocation.Longitude).IndexOf(',') + 1, Convert.ToString(AimLocation.Longitude).Length - Convert.ToString(AimLocation.Longitude).IndexOf(',') - 1);
            string address = "https://maps.googleapis.com/maps/api/directions/xml?origin=(" + tmpSourseLoc + "&destination=" + tmpAimLoc + "&KEY=AIzaSyBrweENi7gpNQd23mLEWr9g3OuvXBq0LBA";
            log1.Text += address;
            StorageFile myDB;


            try
            {

                StorageFile tempFile2 = await Windows.Storage.ApplicationData.Current.LocalFolder.CreateFileAsync("route", CreationCollisionOption.ReplaceExisting);
                BackgroundDownloader manager2 = new BackgroundDownloader();
                var operation = manager2.CreateDownload(new Uri(address), tempFile2);
                IProgress<DownloadOperation> progressH = new Progress<DownloadOperation>((p) =>
                { Debug.WriteLine("Transferred: {0}, Total: {1}", p.Progress.BytesReceived, p.Progress.TotalBytesToReceive); });
                await operation.StartAsync().AsTask(progressH);
                Debug.WriteLine("BacgroundTransfer created");



                //StorageFolder folder = Windows.Storage.ApplicationData.Current.LocalFolder;
                myDB = await Windows.Storage.ApplicationData.Current.LocalFolder.GetFileAsync("route");


                // Read the data
                var alldata = await Windows.Storage.FileIO.ReadLinesAsync(myDB);
                string[] datalines = new string[alldata.Count];
                int ko = 0; int start; string distance = ""; bool flagYet = false; bool flagend = true; bool flagDuration = false;
                foreach (var line in alldata)
                {
                    datalines[ko] = line.ToString();
                    //log1.Text += datalines[ko] + "\r\n";



                    if (flagDuration)
                    {
                        if ((start = datalines[ko].IndexOf("<value>")) == -1)
                        {
                            MessageDialog dialog = new MessageDialog("Не удалось построить маршрут. Попробуйте снова. \r\nЕсли ошибка повторится вновь, просим уведомить нас о ней через форму обратной связи.", "Ошибка #007002");
                            await dialog.ShowAsync();
                            await myDB.DeleteAsync();
                            break;
                        }
                        int durationStringLenght = datalines[ko].Length;
                        distance = datalines[ko].Substring(start + 7, durationStringLenght - 7 - start - 8);
                        timeResult.Text = "Предположительное время в дороге " + distance + " секунд\t";
                        ko++;
                        flagDuration = false;
                        continue;
                    }
                    if (flagYet)
                    {
                        if ((start = datalines[ko].IndexOf("<value>")) == -1)
                        {
                            MessageDialog dialog = new MessageDialog("Маршрут построить не удалось. Попробуйте снова. \r\nЕсли ошибка повторится вновь, просим уведомить нас о ней через форму обратной связи.", "Ошибка #007003");
                            await dialog.ShowAsync();
                            await myDB.DeleteAsync();
                            break;
                        }
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

                    if ((start = datalines[ko - 1].IndexOf("<status>ZERO")) != -1)
                    {
                        MessageDialog dialog = new MessageDialog("Маршрут построить не удалось. Попробуйте снова. \r\nЕсли ошибка повторится вновь, просим уведомить нас о ней через форму обратной связи.", "Ошибка #007004");
                        await dialog.ShowAsync();
                        await myDB.DeleteAsync();
                        break;
                    }
                    if ((start = datalines[ko - 1].IndexOf("<status>NOT")) != -1)
                    {
                        textBox.Text = "Маршрут не найден!";
                        break;
                    }
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
                    //log1.Text += line.ToString() + "\r\n";
                }

                await myDB.DeleteAsync();

            }
            catch (Exception ex)
            {
                textBlock1.Text = "Info Error";
            }
        }
    }
}