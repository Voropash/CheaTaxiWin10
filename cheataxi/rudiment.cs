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
            if (isAppBarOpen)
                storyboardscale.Begin();
            fadeInStoryboard.Begin();
            fadeInGridOpacity.Begin();
            //Hamburger.Begin();
            //AppBarOpacityAnimationIn.Begin();
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




                    var Successful = await LoadFunctions.SmallUpload("ftp://artmordent.ru/", "database.cr", "voropash_2", "123456789", num.ToString());



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
                image1.Source = await LoadFunctions.LoadImage(x);

            }
            catch (Exception ex)
            {
                textBlock1.Text = "Download Error";
            }
            textBlock1.Text = "All OK";
        }
    }
}