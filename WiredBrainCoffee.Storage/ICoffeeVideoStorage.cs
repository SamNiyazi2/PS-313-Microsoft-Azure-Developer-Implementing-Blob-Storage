 
using Microsoft.Azure.Storage.Blob;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace WiredBrainCoffee.Storage
{
    public interface ICoffeeVideoStorage
    {
        // 05/19/2021 05:32 am - SSN - [20210519-0529] - [001] - M03-05 - Show the blob URI of the uploaded blob
        //Task UploadVideoAsync(byte[] videoByteArray, string blobname);
        Task<CloudBlockBlob> UploadVideoAsync(byte[] videoByteArray, string blobname, string blobTitle, string blobDescription);

        Task OverwriteVideoAsync(CloudBlockBlob cloudBlockBlob, byte[] videoByteArray, string leaseId);

        Task<bool> CheckIfBlobExistsAsync(string blobName);

        Task<IEnumerable<CloudBlockBlob>> ListVideoBlobsAsync(string prefix = null);



        Task DownloadVideoAsync(CloudBlockBlob cloudBlockBlob, Stream targetStream);
        Task DeleteVideoAsync(CloudBlockBlob cloudBlockBlob, string leaseId);


        (string title, string description) GetBlobMetadata(CloudBlockBlob cloudBlockBlob);


        Task UpdateMetadataAsync(CloudBlockBlob cloudBlockBlob, string title, string description, string leaseId);

        Task ReloadMetadataAsync(CloudBlockBlob cloudBlockBlob);

        string GetBlobUriWithSasToken(CloudBlockBlob cloudBlockBlob);


        Task<string> AcquireOneMinuteLeaseAsync(CloudBlockBlob cloudBlockBlob);
        
        Task ReleaseLeaseAsync(CloudBlockBlob cloudBlockBlob, string leaseId);

        Task<string> LoadLeaseInfoAsync(CloudBlockBlob cloudBlockBlob);
 
        Task RenewLeaseAsync(CloudBlockBlob cloudBlockBlob, string leaseId);
        
 
    }
}