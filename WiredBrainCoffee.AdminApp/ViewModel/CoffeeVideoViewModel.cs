using Microsoft.Azure.Storage.Blob;

namespace WiredBrainCoffee.AdminApp.ViewModel
{
    public class CoffeeVideoViewModel : ViewModelBase
    {

        private CloudBlockBlob cloudBlockBlob;

        // 05/19/2021 05:43 am - SSN - [20210519-0529] - [004] - M03-05 - Show the blob URI of the uploaded blob
        public CoffeeVideoViewModel(CloudBlockBlob cloudBlockBlob)
        {
            this.cloudBlockBlob = cloudBlockBlob;
        }

        //public string BlobName { get; set; }
        public string BlobName => cloudBlockBlob.Name;

        // public string BlobUri { get; set; }
        public string BlobUri => cloudBlockBlob.Uri.ToString();

    }

}
