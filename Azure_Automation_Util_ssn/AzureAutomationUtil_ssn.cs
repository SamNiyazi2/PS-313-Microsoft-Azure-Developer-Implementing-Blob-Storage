
// Install-Package Microsoft.Azure.Management.Automation

//using Microsoft.Azure.Management.Automation;
//using Newtonsoft.Json;


// install-package System.Text.Json

// install-package Microsoft.IdentityModel.Clients.ActiveDirectory
// For AuthenticationContext


using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

// 05/19/2021 12:49 am - SSN - [20210519-0034] - [001] - M03-04 - Get the connection string (Blob)


namespace Azure_Automation_Util_ssn
{
    public class AzureAutomationUtil_ssn
    {

        //public static void getVariable(string variableName)
        //{
        //    // https://docs.microsoft.com/en-us/dotnet/api/microsoft.azure.management.automation.variableoperationsextensions.getasync?view=azure-dotnet
        //    VariableOperationsExtensions.GetAsync()

        //}


        //public static void createVariable(string variableName)
        //{
        //    string resourceGroupName = "";
        //    string automationAccount = "";
        //    Microsoft.Azure.Management.Automation.Models.VariableCreateOrUpdateParameters parameters = null;
        //    // https://docs.microsoft.com/en-us/dotnet/api/microsoft.azure.management.automation.ivariableoperations.createorupdateasync?view=azure-dotnet#Microsoft_Azure_Management_Automation_IVariableOperations_CreateOrUpdateAsync_System_String_System_String_Microsoft_Azure_Management_Automation_Models_VariableCreateOrUpdateParameters_System_Threading_CancellationToken_
        //    VariableOperationsExtensions.CreateOrUpdateAsync(resourceGroupName, automationAccount, parameters, null);

        //}




        public static async Task get1Async()
        { // get Access token 

            //  https://stackoverflow.com/questions/61964132/how-to-call-azure-rest-api-in-c-sharp
            // https://docs.microsoft.com/en-us/archive/blogs/benjaminperkins/how-to-securely-connect-to-azure-from-c-and-run-rest-apis



            string tenantId = "60bd2dcc-0e49-4a06-b26b-0e8cc59c23aa";
            string authenticationContextURL = "https://loing.window.net/" + tenantId;

            var authenticationContext = new AuthenticationContext(authenticationContextURL);

            string CLIENT_ID = "1d826670-d068-4283-a80e-04bedc8bf3cf";
            string CIENT_SECRET = "8ihhfYBRUB7eaq1M2pq~pZ5RH4i2MQejqp";

             CLIENT_ID = "sam2003@niyazi.com";
             CIENT_SECRET = "Just#GetMeIn$";

            var credentials = new ClientCredential(CLIENT_ID, CIENT_SECRET);

            string url = "https://management.azure.com";

            // Failing 
            // authority_not_in_valid_list: 'authority' is not in the list of valid addresses
            // Possible remedy
            // https://github.com/Azure-Samples/active-directory-dotnet-native-aspnetcore-v2/tree/master/2.%20Web%20API%20now%20calls%20Microsoft%20Graph
            var result = await authenticationContext.AcquireTokenAsync(url, credentials);


            string token = result.AccessToken;



        }


        public static void get1_b() //_Azure_Token()
        {
            // https://stackoverflow.com/questions/50378067/which-grant-type-should-be-used-for-the-on-behalf-of-token-in-azure
            // https://docs.microsoft.com/en-us/azure/active-directory/develop/v2-oauth2-on-behalf-of-flow
            // https://docs.microsoft.com/en-us/azure/active-directory/develop/v2-permissions-and-consent


            string tenantId = "60bd2dcc-0e49-4a06-b26b-0e8cc59c23aa";

            string urlBase = $"https://login.microsoftonline.com/";
            string urlSuffix = $"/{{tenantId}}/oauth2/token";

            using (var client = new HttpClient(new HttpClientHandler()))
            {
                client.BaseAddress = new Uri(urlBase);

                // Needs to be post
                HttpResponseMessage response = client.GetAsync(urlSuffix).Result;

                //                HttpResponseMessage response = client.PostAsync(urlSuffix).Result;

                string result = response.Content.ReadAsStringAsync().Result;

                //                var final =  JsonSerializer.Deserialize(result);

                // https://login.microsoftonline.com/{{tenantId}}/oauth2/token 
            }
        }

        public static void get2()
        {
            // GET https://management.azure.com/subscriptions/{subscriptionId}/resourceGroups/{resourceGroupName}/providers/Microsoft.Automation/automationAccounts/{automationAccountName}/variables/{variableName}?api-version=2015-10-31

            string subscriptionID = "d6723a94-6723-4e85-b988-03bf28d6ea51";
            string resourceGroupName = "Default-Web-CentralUS";
            string automationAccountName = "SSN-AA-GUI-20190322";
            string variableName = "Disk-01-Key-1-20190323";

            string urlBase = $"https://management.azure.com/";
            string urlSuffix = $"subscriptions/{subscriptionID}/resourceGroups/{resourceGroupName}/providers/Microsoft.Automation/automationAccounts/{automationAccountName}/variables/{variableName}?api-version=2015-10-31";

            using (var client = new HttpClient(new HttpClientHandler()))
            {
                client.BaseAddress = new Uri(urlBase);
                HttpResponseMessage response = client.GetAsync(urlSuffix).Result;

                string result = response.Content.ReadAsStringAsync().Result;

                //                var final =  JsonSerializer.Deserialize(result);

                // https://login.microsoftonline.com/{{tenantId}}/oauth2/token 
            }
        }

    }
}
