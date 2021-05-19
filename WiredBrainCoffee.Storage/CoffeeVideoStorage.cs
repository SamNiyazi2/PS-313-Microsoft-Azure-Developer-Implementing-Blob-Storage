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


            if (string.IsNullOrWhiteSpace(blobStorageConnectionString))
            {
                throw new Exception("Calling with null or empty storage connection string");
            }

            var cloudStorageAccount_2 = CloudStorageAccount.Parse(blobStorageConnectionString);

            var cloudBlobClient = cloudStorageAccount_2.CreateCloudBlobClient();

            var cloudBlobContainer = cloudBlobClient.GetContainerReference(blobContainerName);

            await cloudBlobContainer.CreateIfNotExistsAsync(BlobContainerPublicAccessType.Blob, null, null);

            var cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(blobName);

            await cloudBlockBlob.UploadFromByteArrayAsync(videoByteArray, 0, videoByteArray.Length);

            return cloudBlockBlob;

        }

        public async Task<bool> CheckIfBlobExistsAsync(string blobName)
        {
            // TODO: Check if the blob exists in Blob Storage
            return false;
        }
    }
}
