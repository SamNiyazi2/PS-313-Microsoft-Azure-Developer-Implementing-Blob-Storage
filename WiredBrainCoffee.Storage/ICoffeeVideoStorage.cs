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
        Task<CloudBlockBlob> UploadVideoAsync(byte[] videoByteArray, string blobname);

        Task<bool> CheckIfBlobExistsAsync(string blobName);

        Task<IEnumerable<CloudBlockBlob>> ListVideoBlobsAsync(string prefix);



        Task DownloadVideoAsync(CloudBlockBlob cloudBlockBlob, Stream targetStream);
        Task DeleteVideoAsync(CloudBlockBlob cloudBlockBlob);


        (string title, string description) GetBlobMetadata(CloudBlockBlob cloudBlockBlob);


        Task UpdateMetadataAsync(CloudBlockBlob cloudBlockBlob, string title, string description);

        Task ReloadMetadataAsync(CloudBlockBlob cloudBlockBlob);

    }
}