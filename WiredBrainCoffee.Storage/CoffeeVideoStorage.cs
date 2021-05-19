using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WiredBrainCoffee.Storage
{
    public class CoffeeVideoStorage : ICoffeeVideoStorage
    {
        private readonly string blobStorageConnectionString;
        private readonly string blobContainerName = "coffeevideos";
        public CoffeeVideoStorage(string blobStorageConnectionString)
        {
            this.blobStorageConnectionString = blobStorageConnectionString;
        }

        // 05/19/2021 05:34 am - SSN - [20210519-0529] - [002] - M03-05 - Show the blob URI of the uploaded blob
        // public async Task UploadVideoAsync(byte[] videoByteArray, string blobName)
        public async Task<CloudBlockBlob> UploadVideoAsync(byte[] videoByteArray, string blobName)
        {
            // 05/19/2021 12:06 am - SSN - [20210518-2359] - [001] - M03-03 - Upload a blob to a container 

            CloudBlockBlob cloudBlockBlob = await getCloudBlockBlobAsync(blobName);

            cloudBlockBlob.Properties.ContentType = "video/mp4";

            await cloudBlockBlob.UploadFromByteArrayAsync(videoByteArray, 0, videoByteArray.Length);

            return cloudBlockBlob;

        }

        public async Task<bool> CheckIfBlobExistsAsync(string blobName)
        {
            // 05/19/2021 05:57 am - SSN - [20210519-0548] - [001] - M03-06 - Check if a blob exists

            CloudBlockBlob cloudBlockBlob = await getCloudBlockBlobAsync(blobName);

            return await cloudBlockBlob.ExistsAsync();
        }


        public async Task<IEnumerable<CloudBlockBlob>> ListVideoBlobsAsync(string prefix = null)
        {
            var cloudBlockBlobs = new List<CloudBlockBlob>();
            var cloudBlobContainer = await getCloudBlobContainerAsync();

            BlobContinuationToken token = null;
            bool useFlagBlobListing = true;
            int maxResults = 2;
            BlobRequestOptions blobRequestOptions = null;
            OperationContext operationContext = null;


            do
            {
                // Option 1
                //var blobResultSegment = await cloudBlobContainer.ListBlobsSegmentedAsync(null);

                // Option 2
                //var blobResultSegment = await cloudBlobContainer.ListBlobsSegmentedAsync(null, useFlagBlobListing, BlobListingDetails.None, 
                //                                maxResults, token, blobRequestOptions, operationContext);
                
                // Option 3
                var blobResultSegment = await cloudBlobContainer.ListBlobsSegmentedAsync(prefix, token);

                cloudBlockBlobs.AddRange(blobResultSegment.Results.OfType<CloudBlockBlob>());
                token = blobResultSegment.ContinuationToken;

            } while (token != null);

            return cloudBlockBlobs;

        }


        private async Task<CloudBlockBlob> getCloudBlockBlobAsync(string blobName)
        {
            var cloudBlobContainer = await getCloudBlobContainerAsync();
            var cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(blobName);

            return cloudBlockBlob;
        }

        ///////////////////////////////////    GetCoffeeVideosContainerAsync

        private async Task<CloudBlobContainer> getCloudBlobContainerAsync()
        {

            if (string.IsNullOrWhiteSpace(blobStorageConnectionString))
            {
                throw new Exception("Calling with null or empty storage connection string [20210519-0558]");
            }

            var cloudStorageAccount_2 = CloudStorageAccount.Parse(blobStorageConnectionString);

            var cloudBlobClient = cloudStorageAccount_2.CreateCloudBlobClient();

            var cloudBlobContainer = cloudBlobClient.GetContainerReference(blobContainerName);

            await cloudBlobContainer.CreateIfNotExistsAsync(BlobContainerPublicAccessType.Blob, null, null);

            return cloudBlobContainer;

        }


        public async Task DownloadVideoAsync(CloudBlockBlob cloudBlockBlob, Stream targetStream)
        {
            await cloudBlockBlob.DownloadToStreamAsync(targetStream);
        }

        public async Task DeleteVideoAsync(CloudBlockBlob cloudBlockBlob)
        {
            await cloudBlockBlob.DeleteAsync();
        }

    }
}
