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
            // clear results (repete logic of program)
            if (isResultShown)
                button6_Click(sender, e);
            // get x-y point
            var pos = e.GetPosition(myMap);
            // Get Place from x-y
            try
            {
                getPlaceFromCoordinates(pos);
            }
            catch 
            {
                MessageDialog dialog = new MessageDialog("Мы не смогли обнаружить ничего рядом с данной меткой. Однако вы все равно можете попробовать построить маршрут, используя её.", "Ничего не найдено!");
            }
            contextPerform();
        }


        // perform our route
        private void getResult()
        {
            isResultShown = true;
            omniBoxAnimation();
            button.Visibility = Visibility.Collapsed;
            button5.Visibility = Visibility.Collapsed;
            tipLable.Text = "Цены на такси:";
        }
        

        // clear button
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


        // Entered in textBox == clon of search button
        private void enteredInTextBox(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                // get OmniBox
                button5_Click(sender, new RoutedEventArgs());
            }
        }
        
        
        // goto info page
        private void infoButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(info));
        }


        // GPS (find me button)
        private async void button_Click_1(object sender, RoutedEventArgs e)
        {
            // Bad case
            if (isSourseTapped)
            {
                MessageDialog dialog = new MessageDialog("Ваше местоположение может быть только исходной точкой маршрута. Очистите карту и попробуйте построить маршрут снова.", "Ошибка построения маршрута");
                await dialog.ShowAsync();
                return;
            }
            // Find sourse location
            findMe();
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