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

        private void omniBoxAnimation()
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
    }
}