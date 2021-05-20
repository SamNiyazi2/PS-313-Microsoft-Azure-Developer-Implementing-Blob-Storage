using Microsoft.AspNetCore.Html;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WiredBrainCoffee.WebApp.Extensions
{
    public static class ExtensionsCustom
    {
        public static HtmlString ToNoBlank( this string str ){

            if (string.IsNullOrWhiteSpace(str)) return new HtmlString( "&nbsp;");
            return new HtmlString(str);
        }
    }
}
