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
        private async void button5_Click(object sender, RoutedEventArgs e)
        {
            string address = "http://geocode-maps.yandex.ru/1.x/?geocode=" + textBox.Text;


            StorageFile myDB;

            try
            {
                listBox.Visibility = Visibility.Visible;
                controlsBG2.Visibility = Visibility.Visible;
                controlsBG.Visibility = Visibility.Collapsed;
                listBox.Items.Clear();
                StorageFile tempFile2 = await Windows.Storage.ApplicationData.Current.LocalFolder.CreateFileAsync("multi", CreationCollisionOption.ReplaceExisting);
                BackgroundDownloader manager2 = new BackgroundDownloader();
                var operation = manager2.CreateDownload(new Uri(address), tempFile2);
                IProgress<DownloadOperation> progressH = new Progress<DownloadOperation>((p) =>
                { Debug.WriteLine("Transferred: {0}, Total: {1}", p.Progress.BytesReceived, p.Progress.TotalBytesToReceive); });
                await operation.StartAsync().AsTask(progressH);
                Debug.WriteLine("BacgroundTransfer created");



                //StorageFolder folder = Windows.Storage.ApplicationData.Current.LocalFolder;
                myDB = await Windows.Storage.ApplicationData.Current.LocalFolder.GetFileAsync("multi");


                // Read the data
                var alldata = await Windows.Storage.FileIO.ReadLinesAsync(myDB);
                datalines = new string[alldata.Count];
                int ko = 0; int start; string pos = "";
                foreach (var line in alldata)
                {
                    datalines[ko] = line.ToString();
                    ko++;
                    if ((start = datalines[ko - 1].IndexOf("<text>")) != -1)
                    {
                        int nameLeng = datalines[ko - 1].Length;
                        listBox.Items.Add(datalines[ko - 1].Substring(start + 6, nameLeng - 6 - start - 7));
                    }
                }

                await myDB.DeleteAsync();

            }
            catch (Exception ex)
            {
                MessageDialog dialog = new MessageDialog("Ошибка при поиске Вашего запроса. Очистите карту и попробуйте построить маршрут снова.", "Ошибка поиска");
                //await myDB.DeleteAsync();
            }
        }



        private async void listBoxHandler(object sender, TappedRoutedEventArgs e)
        {
            int selectedIndex = listBox.SelectedIndex;
            listBox.Visibility = Visibility.Collapsed;
            controlsBG.Visibility = Visibility.Visible;
            controlsBG2.Visibility = Visibility.Collapsed;
            try
            {
                int ko = 0; int start; string pos = ""; int counter = -1;
                while (ko < datalines.Length)
                {
                    ko++;
                    if (((start = datalines[ko - 1].IndexOf("<text>")) != -1))
                    {
                        counter++;
                        if (counter != selectedIndex) continue;
                        int nameLeng = datalines[ko - 1].Length;
                        textBox.Text = "";
                        if (!isSourseTapped)
                            sourseLable.Text = datalines[ko - 1].Substring(start + 6, nameLeng - 6 - start - 7);
                        else
                            aimLable.Text = datalines[ko - 1].Substring(start + 6, nameLeng - 6 - start - 7);
                    }
                    if (((start = datalines[ko - 1].IndexOf("<pos>")) != -1) && (counter == selectedIndex))
                    {
                        int posLenght = datalines[ko - 1].Length;
                        pos = datalines[ko - 1].Substring(start + 5, posLenght - 5 - start - 6);

                        double l1 = Convert.ToDouble(pos.Substring(0, pos.IndexOf('.')) + "," + pos.Substring(pos.IndexOf('.') + 1, pos.IndexOf(' ') - pos.IndexOf('.') - 1));

                        double l2 = Convert.ToDouble(pos.Substring(pos.IndexOf(' ') + 1, pos.IndexOf('.', pos.IndexOf('.') + 1) - pos.IndexOf(' ') - 1)
                                + ',' +
                                    pos.Substring(pos.IndexOf('.', pos.IndexOf('.') + 1) + 1, pos.Length - (pos.IndexOf('.', pos.IndexOf('.') + 1) + 1)));



                        myMap.SetView(location = new Bing.Maps.Location() { Latitude = l2, Longitude = l1 });
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
                            await countingPerform();
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

                        break;
                    }
                }
            }
            catch { MessageDialog dialog = new MessageDialog("Возникла непредвиденная ошибка. Очистите карту и попробуйте построить маршрут снова. Если ошибка повторится, сообщите нам о ней через форму обратной связи.", "Ошибка #007001"); }
        }
    }
}