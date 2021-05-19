using Microsoft.Azure.Storage.Blob;
using WiredBrainCoffee.AdminApp.Service;
using WiredBrainCoffee.Storage;

namespace WiredBrainCoffee.AdminApp.ViewModel
{
    public class CoffeeVideoViewModel : ViewModelBase
    {

        private CloudBlockBlob cloudBlockBlob;
        private readonly ICoffeeVideoStorage coffeeVideoStorage;
        private readonly IFilePickerDialogService filePickerDialogService;
        private readonly IMessageDialogService messageDialogService;
        private readonly IMainViewModel mainViewModel;


        // 05/19/2021 05:43 am - SSN - [20210519-0529] - [004] - M03-05 - Show the blob URI of the uploaded blob
        public CoffeeVideoViewModel(
            CloudBlockBlob _cloudBlockBlob
            //ICoffeeVideoStorage _coffeeVideoStorage,
            //IFilePickerDialogService _filePickerDialogService,
            //IMessageDialogService _messageDialogService,
            //IMainViewModel _mainViewModel
            )
        {
            this.cloudBlockBlob = _cloudBlockBlob;
            // this.coffeeVideoStorage = _coffeeVideoStorage
            //this.filePickerDialogService = _filePickerDialogService;
            //this.messageDialogService = _messageDialogService;

            //this.mainViewModel = _mainViewModel;
            
        }


        //public string BlobName { get; set; }
        public string BlobName => cloudBlockBlob.Name;

        // public string BlobUri { get; set; }
        public string BlobUri => cloudBlockBlob.Uri.ToString();

    }

    public interface IMainViewModel
    {
    }

}
