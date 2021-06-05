using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WiredBrainCoffee
{
    [Route("/img")]
    public class Image : Controller
    {
        private readonly IHostingEnvironment host;

        public Image(IHostingEnvironment host)
        {
            this.host = host;
        }
        public IActionResult img()
        {
            return File(SSN_HtmlToImageLibrary.DemoSiteInfo.getInfoImage(Path.Combine(host.WebRootPath, "A.bmp")), "image/bmp");
        }
    }
}