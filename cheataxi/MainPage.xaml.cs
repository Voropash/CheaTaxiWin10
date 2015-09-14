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

// Шаблон элемента пустой страницы задокументирован по адресу http://go.microsoft.com/fwlink/?LinkId=234238

namespace cheataxi
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        private static Boolean isAppBarOpen = false;
        private static Boolean isAppBarHBOpened = false;
        private static Boolean OwnSemaphoreBtn2 = true;
        private static Boolean OwnSemaphoreBtn3 = true;
        static string GeneratedHTML = "";
        Bing.Maps.Pushpin pin = new Bing.Maps.Pushpin();
        Bing.Maps.Pushpin pinAim = new Bing.Maps.Pushpin();
        Bing.Maps.Location location , sourseLocation, AimLocation;
        private static bool isSourseTapped = false;
        Geolocator geolocator;
        private string[] datalines;
        private bool ifResultShown = false;
        private bool flagdistance = true;
        private MainPage dataContext;

        Int64 MAX_LENGTH_ROUTE = 150000;

        public MainPage()
        {
            this.InitializeComponent();
            fadeInStoryboardOpacity.Begin();
            fadeInStoryboard.Begin();
            fadeInGridOpacity.Begin();
            Hamburger.Begin();
            AppBarOpacityAnimationIn.Begin();
            AppBarOpacityAnimationInBackground.Begin();

            pin.Background = new SolidColorBrush(Windows.UI.Colors.Red);
            myMap.Children.Add(pin);
            pinAim.Background = new SolidColorBrush(Windows.UI.Colors.Blue);
            myMap.Children.Add(pinAim);
            this.DataContext = dataContext;
            listBox.Visibility = Visibility.Collapsed;
            loading.Visibility = Visibility.Collapsed;
        }

   

    private static async Task<BitmapImage> LoadImage(StorageFile file)
        {
            BitmapImage bitmapImage = new BitmapImage();
            FileRandomAccessStream stream = (FileRandomAccessStream)await file.OpenAsync(FileAccessMode.Read);

            bitmapImage.SetSource(stream);

            return bitmapImage;

        }





        public static async Task<bool> SmallUpload(string ftpURIInfo,
         string filename, string username, string password, string UploadLine)
        {
            string serverUrl;
            Uri serverUri = null;
            NetworkCredential credential;
            bool Successful = false;
            try
            {
                Uri.TryCreate(ftpURIInfo, UriKind.Absolute, out serverUri);
                serverUrl = serverUri.ToString();
                credential = new System.Net.NetworkCredential(username.Trim(),
                password.Trim());

                WebRequest request = WebRequest.Create(serverUrl + "/" + filename);
                request.Credentials = credential;
                request.Proxy = WebRequest.DefaultWebProxy;
                request.Method = "STOR";
                byte[] buffer = Encoding.UTF8.GetBytes(UploadLine);
              using (Stream requestStream = await request.GetRequestStreamAsync())
                {
                    await requestStream.WriteAsync(buffer, 0, buffer.Length);
                    await requestStream.FlushAsync();
               }
                Successful = true;
            }
            catch (Exception)
            {
                throw;
            }
            return Successful;
        }


        private async void button1_Click(object sender, RoutedEventArgs e)
        {
           textBlock1.Text = "загрузка";

            try
            {
                string address = @"http://artmordent.ru/wp-content/uploads/2015/04/ss2-650x433.png";
                StorageFile tempFile1 = await Windows.Storage.ApplicationData.Current.LocalFolder.CreateFileAsync("temp.png", CreationCollisionOption.ReplaceExisting);
                BackgroundDownloader manager1 = new BackgroundDownloader();
                var operation = manager1.CreateDownload(new Uri(address), tempFile1);
                IProgress<DownloadOperation> progressH = new Progress<DownloadOperation>((p) =>
                { Debug.WriteLine("Transferred: {0}, Total: {1}", p.Progress.BytesReceived, p.Progress.TotalBytesToReceive); });
                await operation.StartAsync().AsTask(progressH);
                Debug.WriteLine("BacgroundTransfer created");

                StorageFile x = await Windows.Storage.ApplicationData.Current.LocalFolder.GetFileAsync("temp.png");
                image1.Source = await LoadImage(x);

            }
            catch (Exception ex)
            {
                textBlock1.Text = "Download Error";
            }
            textBlock1.Text = "All OK";
        }

        private async void button2_Click(object sender, RoutedEventArgs e)
        {

            if (OwnSemaphoreBtn2 == true)
            {
                OwnSemaphoreBtn2 = false;

                textBlock1.Text = "загрузка";
                StorageFile myDB;

                try
                {
                    string address = @"http://artmordent.ru/cheataxi/database.cr";
                    StorageFile tempFile2 = await Windows.Storage.ApplicationData.Current.LocalFolder.CreateFileAsync("database", CreationCollisionOption.ReplaceExisting);
                    BackgroundDownloader manager2 = new BackgroundDownloader();
                    var operation = manager2.CreateDownload(new Uri(address), tempFile2);
                    IProgress<DownloadOperation> progressH = new Progress<DownloadOperation>((p) =>
                    { Debug.WriteLine("Transferred: {0}, Total: {1}", p.Progress.BytesReceived, p.Progress.TotalBytesToReceive); });
                    await operation.StartAsync().AsTask(progressH);
                    Debug.WriteLine("BacgroundTransfer created");



                    //StorageFolder folder = Windows.Storage.ApplicationData.Current.LocalFolder;
                    myDB = await Windows.Storage.ApplicationData.Current.LocalFolder.GetFileAsync("database");


                    //await Windows.Storage.FileIO.WriteTextAsync(myDB, "Swift as a shadow");
                    string s = await Windows.Storage.FileIO.ReadTextAsync(myDB);
                    int num;
                    if (Int32.TryParse(s, out num)) { }
                    else { /*Error*/ }

                    textBlock1.Text = (++num).ToString();

               
                    

                    var Successful = await SmallUpload("ftp://artmordent.ru/artmordent.ru/cheataxi/", "database.cr", "voropash", "dr1995dr", num.ToString());



                    await myDB.DeleteAsync();
                    

                }
                catch (Exception ex)
                {
                    textBlock1.Text = "Download Error";
                    //await myDB.DeleteAsync();
                }
                OwnSemaphoreBtn2 = true;
            }
        }

        private async void button3_Click(object sender, RoutedEventArgs e)
        {

            if (OwnSemaphoreBtn3 == true)
            {
                OwnSemaphoreBtn3 = false;

                textBlock1.Text = "загрузка";
                StorageFile myDB;

                try
                {
                    string address = @"http://artmordent.ru/cheataxi/multiplylines.cr";
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
                    string[] datalines = new string[alldata.Count];
                    Int32 ko = 0;
                    foreach (var line in alldata)
                    {
                        datalines[ko] = line.ToString();
                        ko++;
                        log1.Text += line.ToString() + "\r\n";
                    }
                    

                    

                    await myDB.DeleteAsync();


                }
                catch (Exception ex)
                {
                    textBlock1.Text = "Download Error";
                    //await myDB.DeleteAsync();
                }
                OwnSemaphoreBtn3 = true;
            }


         
            
        }

        private void button4_Click(object sender, RoutedEventArgs e)
        {
            //fadeInStoryboardOpacity.Begin();
            Storyboard storyboardscale = new Storyboard();
            ScaleTransform scale = new ScaleTransform();//{ X = 1.0, Y = 1.0 };
                                                        //appBar.RenderTransformOrigin = new Point(250.0/64, 0.0);
            backgr.RenderTransform = scale;
            DoubleAnimation scaleAnim = new DoubleAnimation();
            scaleAnim.Duration = TimeSpan.FromMilliseconds(400);
            scaleAnim.From = 2.0;
            scaleAnim.To = 1.0;
            SineEase easingFunctionscale = new SineEase();
            easingFunctionscale.EasingMode = EasingMode.EaseInOut;
            scaleAnim.EasingFunction = easingFunctionscale;
            Storyboard.SetTarget(scaleAnim, appBar);
            Storyboard.SetTargetProperty(scaleAnim, "(UIElement.RenderTransform).(ScaleTransform.ScaleX)");
            //storyboardscale.Completed += new System.EventHandler(storyboard_Completed);
            storyboardscale.Children.Add(scaleAnim);
            if (isAppBarHBOpened)
                storyboardscale.Begin();
            fadeInStoryboard.Begin();
            fadeInGridOpacity.Begin();
            //Hamburger.Begin();
            //AppBarOpacityAnimationIn.Begin();
        }


        private void notfocus(object sender, RoutedEventArgs e)
        {
            if (isAppBarOpen)
            {
                isAppBarOpen = false;
                Storyboard storyboard = new Storyboard();
                TranslateTransform trans = new TranslateTransform();// { X = 60.0, Y = 0.0 };
                                                                    //backgr.RenderTransformOrigin = new Point(250.0, 0.0);
                backgr.RenderTransform = trans;
                DoubleAnimation moveAnim = new DoubleAnimation();
                moveAnim.Duration = TimeSpan.FromMilliseconds(250);
                moveAnim.From = 250 - 64;
                moveAnim.To = 0;
                //moveAnim.BeginTime = TimeSpan.FromSeconds(0.85);
                SineEase easingFunction = new SineEase();
                easingFunction.EasingMode = EasingMode.EaseInOut;
                moveAnim.EasingFunction = easingFunction;
                Storyboard.SetTarget(moveAnim, backgr);
                Storyboard.SetTargetProperty(moveAnim, "(UIElement.RenderTransform).(TranslateTransform.X)");
                //storyboard.Completed += new System.EventHandler(storyboard_Completed);
                storyboard.Children.Add(moveAnim);
                storyboard.Begin();

                Storyboard storyboardscale = new Storyboard();
                ScaleTransform scale = new ScaleTransform();//{ X = 1.0, Y = 1.0 };
                                                            //appBar.RenderTransformOrigin = new Point(250.0/64, 0.0);
                appBar.RenderTransform = scale;
                DoubleAnimation scaleAnim = new DoubleAnimation();
                scaleAnim.Duration = TimeSpan.FromMilliseconds(250);
                scaleAnim.From = 250.0 / 64;
                scaleAnim.To = 1;
                SineEase easingFunctionscale = new SineEase();
                easingFunctionscale.EasingMode = EasingMode.EaseInOut;
                scaleAnim.EasingFunction = easingFunctionscale;
                Storyboard.SetTarget(scaleAnim, appBar);
                Storyboard.SetTargetProperty(scaleAnim, "(UIElement.RenderTransform).(ScaleTransform.ScaleX)");
                //storyboardscale.Completed += new System.EventHandler(storyboard_Completed);
                storyboardscale.Children.Add(scaleAnim);
                storyboardscale.Begin();


                textBlock1.Text = "Гамбургер закрыт";
            }
        }



        private void button_Click(object sender, RoutedEventArgs e)
        {
            isAppBarHBOpened = true;
            if (!isAppBarOpen)
            {
                isAppBarOpen = true;
                Storyboard storyboard = new Storyboard();
                TranslateTransform trans = new TranslateTransform();// { X = 60.0, Y = 0.0 };
                                                                    //backgr.RenderTransformOrigin = new Point(250.0, 0.0);
                backgr.RenderTransform = trans;
                DoubleAnimation moveAnim = new DoubleAnimation();
                moveAnim.Duration = TimeSpan.FromMilliseconds(250);
                moveAnim.From = 0;
                moveAnim.To = 250 - 64;
                //moveAnim.BeginTime = TimeSpan.FromSeconds(0.85);
                SineEase easingFunction = new SineEase();
                easingFunction.EasingMode = EasingMode.EaseIn;
                moveAnim.EasingFunction = easingFunction;
                Storyboard.SetTarget(moveAnim, backgr);
                Storyboard.SetTargetProperty(moveAnim, "(UIElement.RenderTransform).(TranslateTransform.X)");
                //storyboard.Completed += new System.EventHandler(storyboard_Completed);
                storyboard.Children.Add(moveAnim);
                storyboard.Begin();
                Storyboard storyboardscale = new Storyboard();
                ScaleTransform scale = new ScaleTransform();//{ X = 1.0, Y = 1.0 };
                                                            //appBar.RenderTransformOrigin = new Point(250.0/64, 0.0);
                appBar.RenderTransform = scale;
                DoubleAnimation scaleAnim = new DoubleAnimation();
                scaleAnim.Duration = TimeSpan.FromMilliseconds(250);
                scaleAnim.From = 1.0;
                scaleAnim.To = 250.0 / 64;
                SineEase easingFunctionscale = new SineEase();
                easingFunctionscale.EasingMode = EasingMode.EaseIn;
                scaleAnim.EasingFunction = easingFunctionscale;
                Storyboard.SetTarget(scaleAnim, appBar);
                Storyboard.SetTargetProperty(scaleAnim, "(UIElement.RenderTransform).(ScaleTransform.ScaleX)");
                //storyboardscale.Completed += new System.EventHandler(storyboard_Completed);
                storyboardscale.Children.Add(scaleAnim);
                storyboardscale.Begin();


                textBlock1.Text = "Гамбургер открыт";
                hamburgerButton.Width = 250;
            }
            else
            {
                isAppBarOpen = false;
                Storyboard storyboard = new Storyboard();
                TranslateTransform trans = new TranslateTransform();// { X = 60.0, Y = 0.0 };
                                                                    //backgr.RenderTransformOrigin = new Point(250.0, 0.0);
                backgr.RenderTransform = trans;
                DoubleAnimation moveAnim = new DoubleAnimation();
                moveAnim.Duration = TimeSpan.FromMilliseconds(250);
                moveAnim.From = 250-64;
                moveAnim.To = 0;
                //moveAnim.BeginTime = TimeSpan.FromSeconds(0.85);
                SineEase easingFunction = new SineEase();
                easingFunction.EasingMode = EasingMode.EaseInOut;
                moveAnim.EasingFunction = easingFunction;
                Storyboard.SetTarget(moveAnim, backgr);
                Storyboard.SetTargetProperty(moveAnim, "(UIElement.RenderTransform).(TranslateTransform.X)");
                //storyboard.Completed += new System.EventHandler(storyboard_Completed);
                storyboard.Children.Add(moveAnim);
                storyboard.Begin();

                Storyboard storyboardscale = new Storyboard();
                ScaleTransform scale = new ScaleTransform();//{ X = 1.0, Y = 1.0 };
                                                            //appBar.RenderTransformOrigin = new Point(250.0/64, 0.0);
                appBar.RenderTransform = scale;
                DoubleAnimation scaleAnim = new DoubleAnimation();
                scaleAnim.Duration = TimeSpan.FromMilliseconds(250);
                scaleAnim.From = 250.0 / 64;
                scaleAnim.To = 1;
                SineEase easingFunctionscale = new SineEase();
                easingFunctionscale.EasingMode = EasingMode.EaseInOut;
                scaleAnim.EasingFunction = easingFunctionscale;
                Storyboard.SetTarget(scaleAnim, appBar);
                Storyboard.SetTargetProperty(scaleAnim, "(UIElement.RenderTransform).(ScaleTransform.ScaleX)");
                //storyboardscale.Completed += new System.EventHandler(storyboard_Completed);
                storyboardscale.Children.Add(scaleAnim);
                storyboardscale.Begin();
                
                textBlock1.Text = "Гамбургер закрыт";
                hamburgerButton.Width = 64;
            }
        }


        private async void MapTapped(object sender, TappedRoutedEventArgs e)
        {
            if (ifResultShown)
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
            Storyboard storyboard = new Storyboard();
            TranslateTransform trans = new TranslateTransform();
            resultGrid.RenderTransform = trans;
            DoubleAnimation moveAnim = new DoubleAnimation();
            moveAnim.Duration = TimeSpan.FromMilliseconds(600);
            moveAnim.From = 0;
            moveAnim.To = 636;
            //moveAnim.BeginTime = TimeSpan.FromSeconds(0.85);
            SineEase easingFunction = new SineEase();
            easingFunction.EasingMode = EasingMode.EaseIn;
            moveAnim.EasingFunction = easingFunction;
            Storyboard.SetTarget(moveAnim, resultGrid);
            Storyboard.SetTargetProperty(moveAnim, "(UIElement.RenderTransform).(TranslateTransform.Y)");
            //storyboard.Completed += new System.EventHandler(storyboard_Completed);
            storyboard.Children.Add(moveAnim);
            storyboard.Begin();
            resultAnimation.Begin();
            controlsBGAnimationReverse.Begin();
            textBoxAnimationReverse.Begin();

            ifResultShown = true;
            
            button.Visibility = Visibility.Collapsed;
            button5.Visibility = Visibility.Collapsed;

            tipLable.Text = "Цены на такси:";



            Storyboard storyboardsourse = new Storyboard();
            TranslateTransform moveY1 = new TranslateTransform();
            sourseLable.RenderTransform = moveY1;
            DoubleAnimation moveY1Anim = new DoubleAnimation();
            moveY1Anim.Duration = TimeSpan.FromMilliseconds(600);
            moveY1Anim.From = 0;
            moveY1Anim.To = -270;
            Storyboard.SetTarget(moveY1Anim, sourseLable);
            Storyboard.SetTargetProperty(moveY1Anim, "(UIElement.RenderTransform).(TranslateTransform.Y)");
            storyboardsourse.Children.Add(moveY1Anim);
            storyboardsourse.Begin();

            Storyboard storyboardsourseBG = new Storyboard();
            TranslateTransform moveY2 = new TranslateTransform();
            sourseLableBG.RenderTransform = moveY2;
            DoubleAnimation moveY2Anim = new DoubleAnimation();
            moveY2Anim.Duration = TimeSpan.FromMilliseconds(600);
            moveY2Anim.From = 0;
            moveY2Anim.To = -270;
            Storyboard.SetTarget(moveY2Anim, sourseLableBG);
            Storyboard.SetTargetProperty(moveY2Anim, "(UIElement.RenderTransform).(TranslateTransform.Y)");
            storyboardsourseBG.Children.Add(moveY2Anim);
            storyboardsourseBG.Begin();

            Storyboard storyboardAim = new Storyboard();
            TranslateTransform moveY3 = new TranslateTransform();
            aimLable.RenderTransform = moveY3;
            DoubleAnimation moveY3Anim = new DoubleAnimation();
            moveY3Anim.Duration = TimeSpan.FromMilliseconds(600);
            moveY3Anim.From = 0;
            moveY3Anim.To = -270;
            Storyboard.SetTarget(moveY3Anim, aimLable);
            Storyboard.SetTargetProperty(moveY3Anim, "(UIElement.RenderTransform).(TranslateTransform.Y)");
            storyboardAim.Children.Add(moveY3Anim);
            storyboardAim.Begin();

            Storyboard storyboardAimBG = new Storyboard();
            TranslateTransform moveY4 = new TranslateTransform();
            aimLableBG.RenderTransform = moveY4;
            DoubleAnimation moveY4Anim = new DoubleAnimation();
            moveY4Anim.Duration = TimeSpan.FromMilliseconds(600);
            moveY4Anim.From = 0;
            moveY4Anim.To = -270;
            Storyboard.SetTarget(moveY4Anim, aimLableBG);
            Storyboard.SetTargetProperty(moveY4Anim, "(UIElement.RenderTransform).(TranslateTransform.Y)");
            storyboardAimBG.Children.Add(moveY4Anim);
            storyboardAimBG.Begin();

            Storyboard storyboardsourseHead = new Storyboard();
            TranslateTransform moveY5 = new TranslateTransform();
            sourseLableHead.RenderTransform = moveY5;
            DoubleAnimation moveY5Anim = new DoubleAnimation();
            moveY5Anim.Duration = TimeSpan.FromMilliseconds(600);
            moveY5Anim.From = 0;
            moveY5Anim.To = -270;
            Storyboard.SetTarget(moveY5Anim, sourseLableHead);
            Storyboard.SetTargetProperty(moveY5Anim, "(UIElement.RenderTransform).(TranslateTransform.Y)");
            storyboardsourseHead.Children.Add(moveY5Anim);
            storyboardsourseHead.Begin();

            Storyboard storyboardaimHead = new Storyboard();
            TranslateTransform moveY6 = new TranslateTransform();
            aimLableHead.RenderTransform = moveY6;
            DoubleAnimation moveY6Anim = new DoubleAnimation();
            moveY6Anim.Duration = TimeSpan.FromMilliseconds(600);
            moveY6Anim.From = 0;
            moveY6Anim.To = -270;
            Storyboard.SetTarget(moveY6Anim, aimLableHead);
            Storyboard.SetTargetProperty(moveY6Anim, "(UIElement.RenderTransform).(TranslateTransform.Y)");
            storyboardaimHead.Children.Add(moveY6Anim);
            storyboardaimHead.Begin();

            Storyboard storyboardClearButton = new Storyboard();
            TranslateTransform moveY7 = new TranslateTransform();
            button6.RenderTransform = moveY7;
            DoubleAnimation moveY7Anim = new DoubleAnimation();
            moveY7Anim.Duration = TimeSpan.FromMilliseconds(600);
            moveY7Anim.From = 0;
            moveY7Anim.To = 230;
            Storyboard.SetTarget(moveY7Anim, button6);
            Storyboard.SetTargetProperty(moveY7Anim, "(UIElement.RenderTransform).(TranslateTransform.Y)");
            storyboardClearButton.Children.Add(moveY7Anim);
            storyboardClearButton.Begin();
        }

        private async Task countingPerform ()
        {
            string tmpSourseLoc = Convert.ToString(sourseLocation.Latitude).Substring(0, Convert.ToString(sourseLocation.Latitude).IndexOf(',')) + '.' + Convert.ToString(sourseLocation.Latitude).Substring(Convert.ToString(sourseLocation.Latitude).IndexOf(',') + 1, Convert.ToString(sourseLocation.Latitude).Length - Convert.ToString(sourseLocation.Latitude).IndexOf(',') - 1);
            tmpSourseLoc += "," + Convert.ToString(sourseLocation.Longitude).Substring(0, Convert.ToString(sourseLocation.Longitude).IndexOf(',')) + '.' + Convert.ToString(sourseLocation.Longitude).Substring(Convert.ToString(sourseLocation.Longitude).IndexOf(',') + 1, Convert.ToString(sourseLocation.Longitude).Length - Convert.ToString(sourseLocation.Longitude).IndexOf(',') -1 );
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
                //await myDB.DeleteAsync();
            }
        }

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

            if (ifResultShown)
            {
                Storyboard storyboard = new Storyboard();
                TranslateTransform trans = new TranslateTransform();
                resultGrid.RenderTransform = trans;
                DoubleAnimation moveAnim = new DoubleAnimation();
                moveAnim.Duration = TimeSpan.FromMilliseconds(600);
                moveAnim.From = 636;
                moveAnim.To = 0;
                SineEase easingFunction = new SineEase();
                easingFunction.EasingMode = EasingMode.EaseIn;
                moveAnim.EasingFunction = easingFunction;
                Storyboard.SetTarget(moveAnim, resultGrid);
                Storyboard.SetTargetProperty(moveAnim, "(UIElement.RenderTransform).(TranslateTransform.Y)");
                //storyboard.Completed += new System.EventHandler(storyboard_Completed);
                storyboard.Children.Add(moveAnim);
                storyboard.Begin();
                resultAnimationReverse.Begin();
                controlsBGAnimation.Begin();
                textBoxAnimation.Begin();
                ifResultShown = false;




                Storyboard storyboardsourse = new Storyboard();
                TranslateTransform moveY1 = new TranslateTransform();
                sourseLable.RenderTransform = moveY1;
                DoubleAnimation moveY1Anim = new DoubleAnimation();
                moveY1Anim.Duration = TimeSpan.FromMilliseconds(600);
                moveY1Anim.From = -270;
                moveY1Anim.To = 0;
                Storyboard.SetTarget(moveY1Anim, sourseLable);
                Storyboard.SetTargetProperty(moveY1Anim, "(UIElement.RenderTransform).(TranslateTransform.Y)");
                storyboardsourse.Children.Add(moveY1Anim);
                storyboardsourse.Begin();

                Storyboard storyboardsourseBG = new Storyboard();
                TranslateTransform moveY2 = new TranslateTransform();
                sourseLableBG.RenderTransform = moveY2;
                DoubleAnimation moveY2Anim = new DoubleAnimation();
                moveY2Anim.Duration = TimeSpan.FromMilliseconds(600);
                moveY2Anim.From = -270;
                moveY2Anim.To = 0;
                Storyboard.SetTarget(moveY2Anim, sourseLableBG);
                Storyboard.SetTargetProperty(moveY2Anim, "(UIElement.RenderTransform).(TranslateTransform.Y)");
                storyboardsourseBG.Children.Add(moveY2Anim);
                storyboardsourseBG.Begin();

                Storyboard storyboardAim = new Storyboard();
                TranslateTransform moveY3 = new TranslateTransform();
                aimLable.RenderTransform = moveY3;
                DoubleAnimation moveY3Anim = new DoubleAnimation();
                moveY3Anim.Duration = TimeSpan.FromMilliseconds(600);
                moveY3Anim.From = -270;
                moveY3Anim.To = 0;
                Storyboard.SetTarget(moveY3Anim, aimLable);
                Storyboard.SetTargetProperty(moveY3Anim, "(UIElement.RenderTransform).(TranslateTransform.Y)");
                storyboardAim.Children.Add(moveY3Anim);
                storyboardAim.Begin();

                Storyboard storyboardAimBG = new Storyboard();
                TranslateTransform moveY4 = new TranslateTransform();
                aimLableBG.RenderTransform = moveY4;
                DoubleAnimation moveY4Anim = new DoubleAnimation();
                moveY4Anim.Duration = TimeSpan.FromMilliseconds(600);
                moveY4Anim.From = -270;
                moveY4Anim.To = 0;
                Storyboard.SetTarget(moveY4Anim, aimLableBG);
                Storyboard.SetTargetProperty(moveY4Anim, "(UIElement.RenderTransform).(TranslateTransform.Y)");
                storyboardAimBG.Children.Add(moveY4Anim);
                storyboardAimBG.Begin();

                Storyboard storyboardsourseHead = new Storyboard();
                TranslateTransform moveY5 = new TranslateTransform();
                sourseLableHead.RenderTransform = moveY5;
                DoubleAnimation moveY5Anim = new DoubleAnimation();
                moveY5Anim.Duration = TimeSpan.FromMilliseconds(600);
                moveY5Anim.From = -270;
                moveY5Anim.To = 0;
                Storyboard.SetTarget(moveY5Anim, sourseLableHead);
                Storyboard.SetTargetProperty(moveY5Anim, "(UIElement.RenderTransform).(TranslateTransform.Y)");
                storyboardsourseHead.Children.Add(moveY5Anim);
                storyboardsourseHead.Begin();

                Storyboard storyboardaimHead = new Storyboard();
                TranslateTransform moveY6 = new TranslateTransform();
                aimLableHead.RenderTransform = moveY6;
                DoubleAnimation moveY6Anim = new DoubleAnimation();
                moveY6Anim.Duration = TimeSpan.FromMilliseconds(600);
                moveY6Anim.From = -270;
                moveY6Anim.To = 0;
                Storyboard.SetTarget(moveY6Anim, aimLableHead);
                Storyboard.SetTargetProperty(moveY6Anim, "(UIElement.RenderTransform).(TranslateTransform.Y)");
                storyboardaimHead.Children.Add(moveY6Anim);
                storyboardaimHead.Begin();

                Storyboard storyboardClearButton = new Storyboard();
                TranslateTransform moveY7 = new TranslateTransform();
                button6.RenderTransform = moveY7;
                DoubleAnimation moveY7Anim = new DoubleAnimation();
                moveY7Anim.Duration = TimeSpan.FromMilliseconds(600);
                moveY7Anim.From = 230;
                moveY7Anim.To = 0;
                Storyboard.SetTarget(moveY7Anim, button6);
                Storyboard.SetTargetProperty(moveY7Anim, "(UIElement.RenderTransform).(TranslateTransform.Y)");
                storyboardClearButton.Children.Add(moveY7Anim);
                storyboardClearButton.Begin();


                button.Visibility = Visibility.Visible;
                button5.Visibility = Visibility.Visible;
            }
        }

        private void enteredInTextBox(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                button5_Click(sender, new RoutedEventArgs());
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
                    if (((start = datalines[ko-1].IndexOf("<pos>")) != -1) && (counter == selectedIndex))
                    {
                        int posLenght = datalines[ko-1].Length;
                        pos = datalines[ko-1].Substring(start + 5, posLenght - 5 - start - 6);

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
            catch { MessageDialog dialog = new MessageDialog("Возникла непредвиденная ошибка. Очистите карту и попробуйте построить маршрут снова. Если ошибка повторится, сообщите нам о ней через форму обратной связи.", "Ошибка #007001");  }
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
                    int ko = 0; int start ; string pos = "";
                    foreach (var line in alldata)
                    {
                        datalines[ko] = line.ToString();
                        ko++;
                        if ((start = datalines[ko-1].IndexOf("<text>")) != -1)
                        {
                            int nameLeng = datalines[ko-1].Length;
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
    }
}