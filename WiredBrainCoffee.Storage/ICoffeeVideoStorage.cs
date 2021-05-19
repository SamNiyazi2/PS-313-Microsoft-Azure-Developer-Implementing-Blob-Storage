using Microsoft.Azure.Storage.Blob;
using System.Threading.Tasks;

namespace WiredBrainCoffee.Storage
{
    public interface ICoffeeVideoStorage
    {
        // 05/19/2021 05:32 am - SSN - [20210519-0529] - [001] - M03-05 - Show the blob URI of the uploaded blob
        //Task UploadVideoAsync(byte[] videoByteArray, string blobname);
        Task<CloudBlockBlob> UploadVideoAsync(byte[] videoByteArray, string blobname);

        Task<bool> CheckIfBlobExistsAsync(string blobName);
    }
}