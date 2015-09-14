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
        // global page variables
        private static Boolean isAppBarOpen = false;
            //private static Boolean isAppBarHBOpened = false; 
        // Semaphore Lite for some functions
        private static Boolean OwnSemaphoreBtn2 = true;
        private static Boolean OwnSemaphoreBtn3 = true;
        // global Map variables
        Bing.Maps.Pushpin pin = new Bing.Maps.Pushpin();
        Bing.Maps.Pushpin pinAim = new Bing.Maps.Pushpin();
        Bing.Maps.Location location , sourseLocation, AimLocation;
        private static bool isSourseTapped = false;
        Geolocator geolocator;
        private string[] datalines;
        private bool isResultShown = false;
        private bool flagdistance = true;

        // Const in kilometers, when route will not be drown
        Int64 MAX_LENGTH_ROUTE = 150000;

        public MainPage()
        {
            this.InitializeComponent();
            // Page animations 
            fadeInStoryboardOpacity.Begin();
            fadeInStoryboard.Begin();
            fadeInGridOpacity.Begin();
            Hamburger.Begin();
            AppBarOpacityAnimationIn.Begin();
            AppBarOpacityAnimationInBackground.Begin();

            // Map and Pin init
            pin.Background = new SolidColorBrush(Windows.UI.Colors.Red);
            myMap.Children.Add(pin);
            pinAim.Background = new SolidColorBrush(Windows.UI.Colors.Blue);
            myMap.Children.Add(pinAim);
                //this.DataContext = dataContext;
            // Hide some Items at Page
            listBox.Visibility = Visibility.Collapsed;
            loading.Visibility = Visibility.Collapsed;
        }
        
        
        // If Hamburger menu Lost focus
        private void notfocus(object sender, RoutedEventArgs e)
        {
            if (isAppBarOpen)
            {
                // Close AppBar with animations 
                isAppBarOpen = false;
                closeAppBarAnimation();
                textBlock1.Text = "Гамбургер закрыт"; // viewDebug
            }
        }


        // Hamburger Button Handler
        private void button_Click(object sender, RoutedEventArgs e)
        {
                //isAppBarHBOpened = true;
            if (!isAppBarOpen)
            {
                isAppBarOpen = true;
                openAppBarAnimation();
                textBlock1.Text = "Гамбургер открыт";
            }
            else
            {
                isAppBarOpen = false;
                closeAppBarAnimation();
                textBlock1.Text = "Гамбургер закрыт"; // viewDebug
            }
        }


        private async void MapTapped(object sender, TappedRoutedEventArgs e)
        {
            if (isResultShown)
                button6_Click(sender, e);

            var pos = e.GetPosition(myMap);
            myMap.TryPixelToLocation(pos, out location);
            myMap.SetView(location);

            string tmpSourseLoc = Convert.ToString(location.Latitude).Substring(0, Convert.ToString(location.Latitude).IndexOf(',')) + '.' + Convert.ToString(location.Latitude).Substring(Convert.ToString(location.Latitude).IndexOf(',') + 1, Convert.ToString(location.Latitude).Length - Convert.ToString(location.Latitude).IndexOf(',') - 1);
            tmpSourseLoc += "," + Convert.ToString(location.Longitude).Substring(0, Convert.ToString(location.Longitude).IndexOf(',')) + '.' + Convert.ToString(location.Longitude).Substring(Convert.ToString(location.Longitude).IndexOf(',') + 1, Convert.ToString(location.Longitude).Length - Convert.ToString(location.Longitude).IndexOf(',') - 1);
            string address = "http://geocode-maps.yandex.ru/1.x/?geocode=" + tmpSourseLoc+ "&sco=latlong";
            log1.Text += address;
            StorageFile myDB;
            try
            {
               StorageFile tempFile2 = await Windows.Storage.ApplicationData.Current.LocalFolder.CreateFileAsync("multi", CreationCollisionOption.ReplaceExisting);
                BackgroundDownloader manager2 = new BackgroundDownloader();
                var operation = manager2.CreateDownload(new Uri(address), tempFile2);
                IProgress<DownloadOperation> progressH = new Progress<DownloadOperation>((p) =>
                { Debug.WriteLine("Transferred: {0}, Total: {1}", p.Progress.BytesReceived, p.Progress.TotalBytesToReceive); });
                await operation.StartAsync().AsTask(progressH);
                Debug.WriteLine("BacgroundTransfer created");

                myDB = await Windows.Storage.ApplicationData.Current.LocalFolder.GetFileAsync("multi");

                var alldata = await Windows.Storage.FileIO.ReadLinesAsync(myDB);
                datalines = new string[alldata.Count];
                int ko = 0; int start;
                foreach (var line in alldata)
                {
                    datalines[ko] = line.ToString();
                    ko++;
                    if ((start = datalines[ko - 1].IndexOf("<text>")) != -1)
                    {
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
                //isSourseTapped = true;
                await countingPerform();
                if (flagdistance)
                    await DrawPath();
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

        private void getResult()
        {
            isResultShown = true;
            omniBoxAnimation();
            button.Visibility = Visibility.Collapsed;
            button5.Visibility = Visibility.Collapsed;
            tipLable.Text = "Цены на такси:";
        }

        


        private void button6_Click(object sender, RoutedEventArgs e)
        {
            isSourseTapped = false;
            sourseLable.Visibility = Visibility.Collapsed;
            sourseLableBG.Visibility = Visibility.Collapsed;
            sourseLableHead.Visibility = Visibility.Collapsed;
            aimLable.Visibility = Visibility.Collapsed;
            aimLableBG.Visibility = Visibility.Collapsed;
            aimLableHead.Visibility = Visibility.Collapsed;
            Bing.Maps.MapLayer.SetPosition(pin, null);
            Bing.Maps.MapLayer.SetPosition(pinAim, null);
            myMap.Children.Clear();
            myMap.ShapeLayers.Clear();
            tipLable.Text = "Введите или выберите на карте исходную точку маршрута:";

            // If results are open
            if (isResultShown)
            {
                clearButtonAnimation();
                isResultShown = false;
                
                button.Visibility = Visibility.Visible;
                button5.Visibility = Visibility.Visible;
            }
        }

        private void enteredInTextBox(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                // get OmniBox
                button5_Click(sender, new RoutedEventArgs());
            }
        }
        
        

        private void infoButton_Click(object sender, RoutedEventArgs e)
        {
            loading.Visibility = Visibility.Visible;
            this.Frame.Navigate(typeof(info));
            loading.Visibility = Visibility.Collapsed;
        }

        private async void button_Click_1(object sender, RoutedEventArgs e)
        {
            if (isSourseTapped)
            {
                MessageDialog dialog = new MessageDialog("Ваше местоположение может быть только исходной точкой маршрута. Очистите карту и попробуйте построить маршрут снова.", "Ошибка построения маршрута");
                await dialog.ShowAsync();
                return;
            }
            

            Windows.Devices.Geolocation.Geolocator geolocator = new Windows.Devices.Geolocation.Geolocator();
            Windows.Devices.Geolocation.Geoposition geoposition = await geolocator.GetGeopositionAsync();
            myMap.SetView(new Bing.Maps.Location() { Latitude = geoposition.Coordinate.Latitude, Longitude = geoposition.Coordinate.Longitude }, 13);
            //button.Content = geoposition.Coordinate.Accuracy+ " "+ geoposition.CivicAddress.Country+ " "+ geoposition.CivicAddress.City + " " + geoposition.CivicAddress.State;

            
            Bing.Maps.MapLayer.SetPosition(pin, new Bing.Maps.Location() { Latitude = geoposition.Coordinate.Latitude, Longitude = geoposition.Coordinate.Longitude });
            myMap.SetView(new Bing.Maps.Location() { Latitude = geoposition.Coordinate.Latitude, Longitude = geoposition.Coordinate.Longitude });


            myMap.Children.Clear();
            myMap.ShapeLayers.Clear();
            Bing.Maps.MapLayer.SetPosition(pin, new Bing.Maps.Location() { Latitude = geoposition.Coordinate.Latitude, Longitude = geoposition.Coordinate.Longitude });
            myMap.Children.Add(pin);
            sourseLocation = new Bing.Maps.Location() { Latitude = geoposition.Coordinate.Latitude, Longitude = geoposition.Coordinate.Longitude };
            isSourseTapped = true;
            tipLable.Text = "Введите или выберите на карте конечную точку маршрута:";

            sourseLable.Visibility = Visibility.Visible;
            sourseLableBG.Visibility = Visibility.Visible;
            sourseLableHead.Visibility = Visibility.Visible;
            aimLable.Visibility = Visibility.Collapsed;
            aimLableBG.Visibility = Visibility.Collapsed;
            aimLableHead.Visibility = Visibility.Collapsed;


        }

        
    }
}