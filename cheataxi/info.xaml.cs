using System;
using Windows.UI.Xaml;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Popups;

// Шаблон элемента пустой страницы задокументирован по адресу http://go.microsoft.com/fwlink/?LinkId=234238

namespace cheataxi
{
    public sealed partial class info : Page
    {

        private static Boolean isAppBarOpen = false;
        private static Boolean isAppBarHBOpened = false;
        private static Boolean OwnSemaphoreBtn2 = true;
        private static Boolean OwnSemaphoreBtn3 = true;
        private string[] datalines;

        private MainPage dataContext;

        private async void getInfo()
        {
            StorageFile myDB;
            try
            {
                string address = @"http://artmordent.ru/cheataxi/multiplylines.cr";
                StorageFile tempFile2 = await Windows.Storage.ApplicationData.Current.LocalFolder.CreateFileAsync("info", CreationCollisionOption.ReplaceExisting);
                BackgroundDownloader manager2 = new BackgroundDownloader();
                var operation = manager2.CreateDownload(new Uri(address), tempFile2);
                IProgress<DownloadOperation> progressH = new Progress<DownloadOperation>((p) => { });
                await operation.StartAsync().AsTask(progressH);                
                myDB = await Windows.Storage.ApplicationData.Current.LocalFolder.GetFileAsync("info");
                
                // Read the data
                var alldata = await Windows.Storage.FileIO.ReadLinesAsync(myDB);
                string[] datalines = new string[alldata.Count];
                Int32 ko = 0;
                foreach (var line in alldata)
                {
                    datalines[ko] = line.ToString();
                    ko++;
                    infoBlockMain.Text += line.ToString() + "\r\n";
                }
                
                await myDB.DeleteAsync();


            }
            catch (Exception ex)
            {
                MessageDialog dialog = new MessageDialog("Ошибка загрузки информации. Проверьте ваше соединение или попробуйте позже.", "Ошибка загрузки");
            }
        }

        public info()
        {
            this.InitializeComponent();
            this.InitializeComponent();
            loading.Visibility = Visibility.Visible;
            //Hamburger.Begin();
            AppBarOpacityAnimationIn.Begin();
            AppBarOpacityAnimationInBackground.Begin();            
            this.DataContext = dataContext;

            getInfo();
            
            loading.Visibility = Visibility.Collapsed;
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
                
                hamburgerButton.Width = 64;
            }
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
                
            }
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.GoBack();
        }
    }
}
