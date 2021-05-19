using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using System;
using System.Threading.Tasks;

namespace WiredBrainCoffee.Storage
{
  public class CoffeeVideoStorage : ICoffeeVideoStorage
  {
        private readonly string blobStorageConnectionString;
        private readonly string blobContainerName = "CoffeeVideos";
        public CoffeeVideoStorage(string blobStorageConnectionString)
    {
            this.blobStorageConnectionString = blobStorageConnectionString;
        }

    public async Task UploadVideoAsync(byte[] videoByteArray, string blobName)
    {
            // 05/19/2021 12:06 am - SSN - [20210518-2359] - [001] - M03-03 - Upload a blob to a container 

            //string accountName = "";
            //string keyValue = "";

          
            //var cloudStorageAccount_1 = new CloudStorageAccount(
            //    new Microsoft.Azure.Storage.Auth.StorageCredentials( accountName, keyValue)
            //    ,true);


            if ( string.IsNullOrWhiteSpace(blobStorageConnectionString))
            {
                throw new Exception("Calling with null or empty storage connection string");
            }

            var cloudStorageAccount_2 = CloudStorageAccount.Parse(blobStorageConnectionString);

            var cloudBlobClient = cloudStorageAccount_2.CreateCloudBlobClient();

            var cloudBlobContainer = cloudBlobClient.GetContainerReference(blobContainerName);

            await cloudBlobContainer.CreateIfNotExistsAsync(BlobContainerPublicAccessType.Blob, null, null);

            var cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(blobName);

            await cloudBlockBlob.UploadFromByteArrayAsync(videoByteArray, 0, videoByteArray.Length);


        }

        public async Task<bool> CheckIfBlobExistsAsync(string blobName)
    {
      // TODO: Check if the blob exists in Blob Storage
      return false;
    }
  }
}
