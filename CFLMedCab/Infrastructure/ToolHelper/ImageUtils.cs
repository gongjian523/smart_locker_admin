using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace CFLMedCab.Infrastructure.ToolHelper
{
    public class ImageUtils
    { 
        /// <summary>
        /// 异步方法：联网从服务端获取Json数据
        /// </summary>
        /// <param name="uri">资源的绝对路径（服务器IP + 资源的相对路径）</param>
        /// <returns>资源的内容</returns>
        public static async Task<string> GetJsonDataFromWebServerAsync(string uri)
        {
            WebClient client = new WebClient();
            string result = await client.DownloadStringTaskAsync(new Uri(uri));
            return result;
        }


        /// <summary>
        /// 异步方法：联网获取Bitmap二进制数据
        /// </summary>
        /// <param name="uri">资源的绝对路径（服务器IP + 资源的相对路径）</param>
        /// <returns>Bitmap图片</returns>
        public static async Task<Image> GetBitmapFromWebServerAsync(string uri)
        {
            WebClient client = new WebClient();
            byte[] result = await client.DownloadDataTaskAsync(new Uri(uri));

            Image image = ByteArrayToImage(result);

            return image;
        }

        public static Image GetBitmapFromWebServer(string uri)
        {
            WebClient client = new WebClient();
            byte[] result = client.DownloadData(new Uri(uri));

            Image image = ByteArrayToImage(result);

            return image;
        }


        /// <summary>  
        /// byte[] --> Image  
        /// </summary>  
        /// <param name="byteArrayIn">二进制图片流</param>  
        /// <returns>System.Drawing.Image</returns>  
        public static System.Drawing.Image ByteArrayToImage(byte[] byteArrayIn)
        {
            if (byteArrayIn == null)
                return null;
            using (MemoryStream ms = new MemoryStream(byteArrayIn))
            {
                System.Drawing.Image image = System.Drawing.Image.FromStream(ms);
                ms.Flush();
                return image;
            }
        }

        /// <summary>
        /// Bitmap --> BitmapImage
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns>BitmapImage</returns>
        public static BitmapImage BitmapToBitmapImage(Bitmap bitmap)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Png);

                stream.Position = 0;
                BitmapImage result = new BitmapImage();
                result.BeginInit();
                // According to MSDN, "The default OnDemand cache option retains access to the stream until the image is needed."
                // Force the bitmap to load right now so we can dispose the stream.
                result.CacheOption = BitmapCacheOption.OnLoad;
                result.StreamSource = stream;
                result.EndInit();
                result.Freeze();

                return result;
            }
        }

        ////[] byteArray = System.Text.Encoding.BigEndianUnicode.GetBytes(data2);
        //MemoryStream ms = new MemoryStream(data2);
        //System.Drawing.Image image = System.Drawing.Image.FromStream(ms);
        //ms.Flush();

        //MemoryStream stream = new MemoryStream();
        //Bitmap bitmap = new Bitmap(image);
        //bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
        //stream.Position = 0;

        //BitmapImage bi = new BitmapImage();
        //bi.BeginInit();
        //bi.CacheOption = BitmapCacheOption.OnLoad;
        //bi.StreamSource = stream;
        ////bi.UriSource = new Uri(@"D:\01-代码\smart_locker_admin\CFLMedCab\Resources\Images\OpenDoor.png");
        //bi.EndInit();
        //bi.Freeze();

    }
}