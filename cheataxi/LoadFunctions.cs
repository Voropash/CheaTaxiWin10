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
    class LoadFunctions
    {
        public static async Task<BitmapImage> LoadImage(StorageFile file)
        {
            BitmapImage bitmapImage = new BitmapImage();
            FileRandomAccessStream stream = (FileRandomAccessStream)await file.OpenAsync(FileAccessMode.Read);
            bitmapImage.SetSource(stream);
            return bitmapImage;
        }

        public static async Task<bool> SmallUpload(string ftpURIInfo, string filename, string username, string password, string UploadLine)
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

    }

}