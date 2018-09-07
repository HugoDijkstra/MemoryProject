using System;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Drawing;

namespace NewMemoryGame
{
    class ImageGetter
    {
        public static List<PictureBox> GetImagesByTheme(string theme, int amount)
        {
            List<PictureBox> returnValue = new List<PictureBox>();

            Console.WriteLine("https://www.google.com/search?q=" + theme + "&tbm=isch");

            HttpWebRequest webRequest = WebRequest.CreateHttp("https://www.google.com/search?q=" + theme + "&tbm=isch");
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
            for (int i = 0; i < splitString.Length; i++)
            {
                int readFrom = splitString[i].IndexOf(lookFor) + lookFor.Length + 1;
                string imageLocation = splitString[i].Substring(readFrom);
                imageLocation = imageLocation.Substring(0, imageLocation.IndexOf('"'));
                try
                {
                    PictureBox pb = GetImageFromWeb(imageLocation);
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
            Console.Read();
            return returnValue;
        }

        private static PictureBox GetImageFromWeb(string url)
        {
            HttpWebRequest request = WebRequest.CreateHttp(url);
            WebResponse response = request.GetResponse();
            PictureBox newPictureBox = new PictureBox();
            try
            {
                Image newImage = Image.FromStream(response.GetResponseStream());
                newPictureBox.Size = newImage.Size;
                newPictureBox.Image = newImage;
                return newPictureBox;
            }
            catch (System.Exception e)
            {
                Console.Write(e);
                return null;
            }
        }

        public static List<PictureBox> GetImagesFromFolder(string path)
        {
            List<PictureBox> returnValue = new List<PictureBox>();

            try
            {
                string[] filenames = Directory.GetFiles(path);
                foreach (string filePath in filenames)
                {
                    try
                    {
                        Image image = Image.FromFile(filePath);
                        PictureBox pb = new PictureBox();
                        pb.Size = image.Size;
                        pb.Image = image;
                        returnValue.Add(pb);
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
