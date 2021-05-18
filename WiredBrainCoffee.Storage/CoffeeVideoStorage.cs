using System.Threading.Tasks;

namespace WiredBrainCoffee.Storage
{
  public class CoffeeVideoStorage : ICoffeeVideoStorage
  {
    public CoffeeVideoStorage()
    {
    }

    public async Task UploadVideoAsync(byte[] videoByteArray, string blobName)
    {
      // TODO: Upload the video to Blob Storage
    }

    public async Task<bool> CheckIfBlobExistsAsync(string blobName)
    {
      // TODO: Check if the blob exists in Blob Storage
      return false;
    }
  }
}
