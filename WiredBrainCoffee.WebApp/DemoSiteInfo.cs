using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WiredBrainCoffee
{
    [Route("/img")]
    public class Image : Controller
    {
        public IActionResult img()
        {
            return File(SSN_HtmlToImageLibrary.DemoSiteInfo.getInfoImage(), "image/bmp");
        }
    }
}