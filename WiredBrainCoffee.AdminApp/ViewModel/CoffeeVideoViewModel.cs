using Microsoft.Azure.Storage.Blob;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage.Streams;
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



        private readonly IAddCoffeeVideoDialogService _addCoffeeVideoDialogService;


        // 05/19/2021 05:43 am - SSN - [20210519-0529] - [004] - M03-05 - Show the blob URI of the uploaded blob
        public CoffeeVideoViewModel(
            CloudBlockBlob cloudBlockBlob,
            ICoffeeVideoStorage _coffeeVideoStorage,
            IFilePickerDialogService _filePickerDialogService,
            IMessageDialogService _messageDialogService,
            IMainViewModel _mainViewModel,


 IAddCoffeeVideoDialogService _addCoffeeVideoDialogService
            )
        {
            this._cloudBlockBlob = cloudBlockBlob;
            this._coffeeVideoStorage = _coffeeVideoStorage;
            this._filePickerDialogService = _filePickerDialogService;
            this._messageDialogService = _messageDialogService;

            this._mainViewModel = _mainViewModel;

            this._addCoffeeVideoDialogService = _addCoffeeVideoDialogService;

            UpdateViewModelPropertiesFromMetadata();
        }


        //public string BlobName { get; set; }
        public string BlobName => _cloudBlockBlob.Name;

        // public string BlobUri { get; set; }
        public string BlobUri => _cloudBlockBlob.Uri.ToString();

        public string BlobUriWithSasToken => _coffeeVideoStorage.GetBlobUriWithSasToken(_cloudBlockBlob);

        private string _title;

        public string Title
        {
            get { return _title; }
            set
            {
                if (_title != value)
                {
                    _title = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsMetadataChanged));
                }
            }
        }

        private string _description;

        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsMetadataChanged));

            }
        }


        public bool IsMetadataChanged
        {
            get
            {
                var (title, description) = _coffeeVideoStorage.GetBlobMetadata(_cloudBlockBlob);
                return !string.Equals(Title, title) || !string.Equals(Description, description);
            }
        }


        public async Task OverwriteCoffeeVideoAsync()
        {


            byte[] BlobByteArray;

            try
            {
                var storageFile = await _filePickerDialogService.ShowMp4FileOpenDialogAsync();

                if (storageFile != null)
                {
                    // BlobNameWithoutExtension = Path.GetFileNameWithoutExtension(storageFile.Name);

                    var randomAccessStream = await storageFile.OpenReadAsync();
                    BlobByteArray = new byte[randomAccessStream.Size];
                    using (var dataReader = new DataReader(randomAccessStream))
                    {
                        await dataReader.LoadAsync((uint)randomAccessStream.Size);
                        dataReader.ReadBytes(BlobByteArray);
                    }

                    _mainViewModel.StartLoading($"Uploading your video ");

                    await _coffeeVideoStorage.OverwriteVideoAsync(_cloudBlockBlob, BlobByteArray);
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






        public async Task UpdateMetadataAsync()
        {
            try
            {
                _mainViewModel.StartLoading($"Updating metadata");
                await _coffeeVideoStorage.UpdateMetadataAsync(_cloudBlockBlob, Title, Description);
                OnPropertyChanged(nameof(IsMetadataChanged));
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



        public async Task ReloadMetadataAsync()
        {
            try
            {
                _mainViewModel.StartLoading($"Reloading metadata");
                await _coffeeVideoStorage.ReloadMetadataAsync(_cloudBlockBlob);
                UpdateViewModelPropertiesFromMetadata();
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

        private void UpdateViewModelPropertiesFromMetadata()
        {
            var (title, description) = _coffeeVideoStorage.GetBlobMetadata(_cloudBlockBlob);
            Title = title;
            Description = description;
        }

    }


}
