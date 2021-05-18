using System.Threading.Tasks;

namespace WiredBrainCoffee.Storage
{
  public interface ICoffeeVideoStorage
  {
    Task UploadVideoAsync(byte[] videoByteArray, string blobname);
    Task<bool> CheckIfBlobExistsAsync(string blobName);
  }
}