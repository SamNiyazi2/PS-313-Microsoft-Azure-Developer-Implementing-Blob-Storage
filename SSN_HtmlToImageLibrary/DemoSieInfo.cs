using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;

// 06/05/2021 04:51 pm - SSN - Creating to display site info on GitHub in an image

namespace SSN_HtmlToImageLibrary
{
    public class DemoSiteInfo
    {

        public static byte[] getInfoImage()
        {
            ///Bitmap bm = new Bitmap("./a.bmp");
            ///

            byte[] img = File.ReadAllBytes( "./a.bmp");

            return img;
            
        }
    }
}
