using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// 05/19/2021 04:54 am - SSN - [20210519-0034] - [002] - M03-04 - Get the connection string (Blob)

namespace WiredBrainCoffee.AdminApp.Settings
{
    class AppSettings
    {
        public static readonly string ConnectionString = Environment.GetEnvironmentVariable("ps312AzureTableConnectionString_azureTable");
        // public static readonly string ConnectionString = "UseDevelopmentStorage=true";
    }
}
