using Microsoft.Azure.Storage.Blob;
using System;
using System.IO;
using System.Threading.Tasks;
using WiredBrainCoffee.AdminApp.Service;
using WiredBrainCoffee.Storage;

namespace WiredBrainCoffee.AdminApp.ViewModel
{
    public class CoffeeVideoViewModel : ViewModelBase
    {

        private CloudBlockBlob _cloudBlockBlob;
        private readonly ICoffeeVideoStorage _coffeeVideoStorage;
        private readonly IFilePickerDialogService _filePickerDialogService;
        private readonly IMessageDialogService _messageDialogService;
        private IMainViewModel _mainViewModel;


        // 05/19/2021 05:43 am - SSN - [20210519-0529] - [004] - M03-05 - Show the blob URI of the uploaded blob
        public CoffeeVideoViewModel(
            CloudBlockBlob cloudBlockBlob,
            ICoffeeVideoStorage _coffeeVideoStorage,
            IFilePickerDialogService _filePickerDialogService,
            IMessageDialogService _messageDialogService,
            IMainViewModel _mainViewModel
            )
        {
            this._cloudBlockBlob = cloudBlockBlob;
            this._coffeeVideoStorage = _coffeeVideoStorage;
            this._filePickerDialogService = _filePickerDialogService;
            this._messageDialogService = _messageDialogService;

            this._mainViewModel = _mainViewModel;

        }


        //public string BlobName { get; set; }
        public string BlobName => _cloudBlockBlob.Name;

        // public string BlobUri { get; set; }
        public string BlobUri => _cloudBlockBlob.Uri.ToString();


        public async Task DownloadVideoToFileAsync()
        {
            try
            {
                var storageFile = await _filePickerDialogService.ShowMp4FileSaveDialogAsync(BlobName);
                if (storageFile != null)
                {
                    _mainViewModel.StartLoading($"Downloading your video {BlobName}");
                    using (var streamToWrite = await storageFile.OpenStreamForWriteAsync())
                    {
                        await _coffeeVideoStorage.DownloadVideoAsync(_cloudBlockBlob, streamToWrite);
                    }
                }
            }
            catch (Exception ex)
            {
                await _messageDialogService.ShowInfoDialogAsync(ex.Message, "Error");
            }
            finally
            {
                _mainViewModel.StopLoading();
            }
        }

        public async Task DeleteVideoAsync()
        {
            try
            {
                var isOk = await _messageDialogService.ShowOkCancelDialogAsync($"Delete the video {_cloudBlockBlob.Name}?", "Question");
                if (isOk)
                {
                    _mainViewModel.StartLoading($"Deleting your video {BlobName}");
                    await _coffeeVideoStorage.DeleteVideoAsync(_cloudBlockBlob);
                    _mainViewModel.RemoveCoffeeVideoViewModel(this);
                    _mainViewModel.StopLoading();
                    _mainViewModel = null;
                }
            }
            catch (Exception ex)
            {
                await _messageDialogService.ShowInfoDialogAsync(ex.Message, "Error");
            }
            finally
            {
                if (_mainViewModel != null)
                {
                    _mainViewModel.StopLoading();
                }
            }
        }

    }

   
}
