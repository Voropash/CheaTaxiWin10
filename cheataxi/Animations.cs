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
        private void closeAppBarAnimation()
        {
            Storyboard storyboard = new Storyboard();
            TranslateTransform trans = new TranslateTransform();
            backgr.RenderTransform = trans;
            DoubleAnimation moveAnim = new DoubleAnimation();
            moveAnim.Duration = TimeSpan.FromMilliseconds(250);
            moveAnim.From = 250 - 64;
            moveAnim.To = 0;
            SineEase easingFunction = new SineEase();
            easingFunction.EasingMode = EasingMode.EaseInOut;
            moveAnim.EasingFunction = easingFunction;
            Storyboard.SetTarget(moveAnim, backgr);
            Storyboard.SetTargetProperty(moveAnim, "(UIElement.RenderTransform).(TranslateTransform.X)");
            storyboard.Children.Add(moveAnim);
            storyboard.Begin();

            Storyboard storyboardscale = new Storyboard();
            ScaleTransform scale = new ScaleTransform();
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
            storyboardscale.Children.Add(scaleAnim);
            storyboardscale.Begin();
            // change Size of HB
            hamburgerButton.Width = 64;
            infoButton.Width = 64;
        }

        private void openAppBarAnimation()
        {
            Storyboard storyboard = new Storyboard();
            TranslateTransform trans = new TranslateTransform();
            backgr.RenderTransform = trans;
            DoubleAnimation moveAnim = new DoubleAnimation();
            moveAnim.Duration = TimeSpan.FromMilliseconds(250);
            moveAnim.From = 0;
            moveAnim.To = 250 - 64;
            SineEase easingFunction = new SineEase();
            easingFunction.EasingMode = EasingMode.EaseIn;
            moveAnim.EasingFunction = easingFunction;
            Storyboard.SetTarget(moveAnim, backgr);
            Storyboard.SetTargetProperty(moveAnim, "(UIElement.RenderTransform).(TranslateTransform.X)");
            storyboard.Children.Add(moveAnim);
            storyboard.Begin();

            Storyboard storyboardscale = new Storyboard();
            ScaleTransform scale = new ScaleTransform();
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
            storyboardscale.Children.Add(scaleAnim);
            storyboardscale.Begin();
            // change Size of HB
            hamburgerButton.Width = 250;
            infoButton.Width = 250;
        }
    }
}