using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.Reflection;

// 06/05/2021 04:51 pm - SSN - Creating to display site info on GitHub in an image

namespace SSN_HtmlToImageLibrary
{
    public class DemoSiteInfo
    {
          

        public static byte[] getInfoImage(string targetImageFile)
        {

            ///Bitmap bm = new Bitmap("./a.bmp");
            ///

            ImageCreator x = new ImageCreator(targetImageFile);

            x.createImage();

            byte[] img = File.ReadAllBytes(targetImageFile);

            return img;

        }
    }

    class ImageCreator
    {
        public string TargetFileName { get; }

        public ImageCreator(string targetFileName)
        {
            TargetFileName = targetFileName;
        }

        public void createImage()
        {
string assemblyFilename=            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "System.Windows.Forms.dll");
       

            Assembly asm = Assembly.LoadFile(assemblyFilename);

            //WebBrowser browser = new WebBrowser();
            //browser.DocumentCompleted += Browser_DocumentCompleted;
            //browser.DocumentText = "<h1>This is a test</h1>";

        }

        //private void Browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        //{
        //    WebBrowser browser = (WebBrowser)sender;
        //    using (Bitmap bitmap =

        //        new Bitmap(browser.Width, browser.Height))
        //    {
        //        browser.DrawToBitmap(bitmap, new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height));
        //        bitmap.Save(TargetFileName, System.Drawing.Imaging.ImageFormat.Bmp);
        //    }
        //}
    }

}

