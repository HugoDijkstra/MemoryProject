using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace MemoryProjectFull
{
    class ImageGetter
    {
        public static List<string> GetUrlsByTheme(string theme, int amount)
        {
            List<string> returnValue = new List<string>();
            try
            {
                Console.WriteLine(("https://www.google.com/search?q=" + theme + "&tbm=isch").Replace(' ', '+'));

                HttpWebRequest webRequest = WebRequest.CreateHttp(("https://www.google.com/search?q=" + theme + "&tbm=isch&tbs=iar:t").Replace(' ', '+'));
                webRequest.Accept = "text/html, application/xhtml+xml, */*";
                webRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; rv:11.0) like Gecko";
                WebResponse webResponse = webRequest.GetResponse();
                Stream data = webResponse.GetResponseStream();

                string html = "";
                using (StreamReader sr = new StreamReader(data))
                {
                    html = sr.ReadToEnd();
                }
                string[] splits = { "class=\"rg_ic rg_i\"" };

                string[] splitString = html.Split(splits, StringSplitOptions.None);

                string lookFor = "src=";
                int imageAmount = 0;
                for (int i = 0; i < splitString.Length; i++)
                {
                    int readFrom = splitString[i].IndexOf(lookFor) + lookFor.Length + 1;
                    string imageLocation = splitString[i].Substring(readFrom);
                    imageLocation = imageLocation.Substring(0, imageLocation.IndexOf('"'));
                    try
                    {
                        returnValue.Add(imageLocation);
                    }
                    catch (System.Exception e)
                    {
                        Console.WriteLine(e.ToString());
                    }
                }
                File.WriteAllText("html.html", html);
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e);
            }
            return returnValue;
        }
        public static List<BitmapImage> GetImagesByTheme(string theme, int amount, int wantedWidht)
        {
            List<BitmapImage> returnValue = new List<BitmapImage>();
            try
            {
                Console.WriteLine(("https://www.google.com/search?q=" + theme + "&tbm=isch").Replace(' ', '+'));

                HttpWebRequest webRequest = WebRequest.CreateHttp(("https://www.google.com/search?q=" + theme + "&tbm=isch&tbs=iar:t").Replace(' ', '+'));
                webRequest.Accept = "text/html, application/xhtml+xml, */*";
                webRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; rv:11.0) like Gecko";
                WebResponse webResponse = webRequest.GetResponse();
                Stream data = webResponse.GetResponseStream();

                string html = "";
                using (StreamReader sr = new StreamReader(data))
                {
                    html = sr.ReadToEnd();
                }
                string[] splits = { "class=\"rg_ic rg_i\"" };

                string[] splitString = html.Split(splits, StringSplitOptions.None);

                string lookFor = "src=";
                int imageAmount = 0;
                for (int i = 0; i < splitString.Length; i++)
                {
                    int readFrom = splitString[i].IndexOf(lookFor) + lookFor.Length + 1;
                    string imageLocation = splitString[i].Substring(readFrom);
                    imageLocation = imageLocation.Substring(0, imageLocation.IndexOf('"'));
                    try
                    {
                        BitmapImage pb = GetImageFromWeb(imageLocation, wantedWidht);
                        if (pb == null)
                        {
                            continue;
                        }
                        returnValue.Add(pb);
                    }
                    catch (System.Exception e)
                    {
                        Console.WriteLine(e.ToString());
                    }
                }
                File.WriteAllText("html.html", html);
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e);
            }
            return returnValue;
        }

        public static BitmapImage GetImageFromWeb(string url, int decodeWidth)
        {
            if (!url.Contains("http"))
                return null;
            BitmapImage bitmap = new BitmapImage();
            try
            {
                bitmap = new BitmapImage(new Uri(url, UriKind.Absolute)) { DecodePixelWidth = decodeWidth };
                return bitmap;
            }
            catch (System.Exception e)
            {
                Console.Write(e);
                return null;
            }
        }

        public static List<BitmapImage> GetImagesFromFolder(string path)
        {
            List<BitmapImage> returnValue = new List<BitmapImage>();

            try
            {
                string[] filenames = Directory.GetFiles(path);
                foreach (string filePath in filenames)
                {
                    try
                    {
                        BitmapImage image = new BitmapImage();
                        returnValue.Add(image);
                    }
                    catch (System.Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
            }
            catch (System.Exception e)
            {
                Console.Write(e.ToString());
            }
            return returnValue;
        }
    }
}
