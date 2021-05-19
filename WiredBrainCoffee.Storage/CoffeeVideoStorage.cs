using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using System;
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
              
            CloudBlockBlob cloudBlockBlob = await getCloudBlockBlob(blobName);

            await cloudBlockBlob.UploadFromByteArrayAsync(videoByteArray, 0, videoByteArray.Length);

            return cloudBlockBlob;

        }

        public async Task<bool> CheckIfBlobExistsAsync(string blobName)
        {
            // 05/19/2021 05:57 am - SSN - [20210519-0548] - [001] - M03-06 - Check if a blob exists

            CloudBlockBlob cloudBlockBlob = await getCloudBlockBlob(blobName);

            return await cloudBlockBlob.ExistsAsync();
        }



        private async Task<CloudBlockBlob> getCloudBlockBlob(string blobName)
        {
            if (string.IsNullOrWhiteSpace(blobStorageConnectionString))
            {
                throw new Exception("Calling with null or empty storage connection string [20210519-0558]");
            }

            var cloudStorageAccount_2 = CloudStorageAccount.Parse(blobStorageConnectionString);

            var cloudBlobClient = cloudStorageAccount_2.CreateCloudBlobClient();

            var cloudBlobContainer = cloudBlobClient.GetContainerReference(blobContainerName);

            await cloudBlobContainer.CreateIfNotExistsAsync(BlobContainerPublicAccessType.Blob, null, null);

            var cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(blobName);

            return cloudBlockBlob;

        }
    }
}
