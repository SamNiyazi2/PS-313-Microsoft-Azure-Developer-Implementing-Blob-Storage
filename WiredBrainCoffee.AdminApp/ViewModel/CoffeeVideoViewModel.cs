using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using WiredBrainCoffee.AdminApp.Service;
using WiredBrainCoffee.Storage;

namespace WiredBrainCoffee.AdminApp.ViewModel
{
    public class CoffeeVideoViewModel : ViewModelBase
    {

        private CloudBlockBlob _cloudBlockBlob;
        private readonly DispatcherTimer _leaseRenewTimer;

        private readonly ICoffeeVideoStorage _coffeeVideoStorage;
        private readonly IFilePickerDialogService _filePickerDialogService;
        private readonly IMessageDialogService _messageDialogService;
        private IMainViewModel _mainViewModel;



        private readonly IAddCoffeeVideoDialogService _addCoffeeVideoDialogService;


        // 05/19/2021 05:43 am - SSN - [20210519-0529] - [004] - M03-05 - Show the blob URI of the uploaded blob
        public CoffeeVideoViewModel
            (
                CloudBlockBlob cloudBlockBlob,
                ICoffeeVideoStorage _coffeeVideoStorage,
                IFilePickerDialogService _filePickerDialogService,
                IMessageDialogService _messageDialogService,
                IMainViewModel _mainViewModel,
                IAddCoffeeVideoDialogService _addCoffeeVideoDialogService

            )
        {
            this._cloudBlockBlob = cloudBlockBlob ?? throw new ArgumentNullException(nameof(cloudBlockBlob));

            _leaseRenewTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(45)
            };

            _leaseRenewTimer.Tick += async (e, s) =>
            {
                await _coffeeVideoStorage.RenewLeaseAsync(cloudBlockBlob, LeaseId);
                Debug.WriteLine("Lease renewed");
            };


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


        // SnapshotQualifiedUri shows the normal Uri for a normal blob and the snapshot Uri for a snapshot
        // public string BlobUri => _cloudBlockBlob.Uri.ToString();
        public string BlobUri => _cloudBlockBlob.SnapshotQualifiedUri.ToString();

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

        public bool HasLease => !string.IsNullOrWhiteSpace(LeaseId);

        private string _leaseId;

        public string LeaseId
        {
            get { return _leaseId; }
            set
            {
                _leaseId = value;
                OnPropertyChanged(nameof(LeaseId));
                OnPropertyChanged(nameof(HasLease));
            }
        }


        public bool IsSnapshot => _cloudBlockBlob.IsSnapshot;

        public string SnapshotTime => $"{_cloudBlockBlob.SnapshotTime:MM/dd/yyyy hh:mm:ss tt}";


        public async Task OverwriteCoffeeVideoAsync()
        {


            byte[] BlobByteArray;

            try
            {
                var storageFile = await _filePickerDialogService.ShowMp4FileOpenDialogAsync();

                if (storageFile != null)
                {

                    var randomAccessStream = await storageFile.OpenReadAsync();
                    BlobByteArray = new byte[randomAccessStream.Size];
                    using (var dataReader = new DataReader(randomAccessStream))
                    {
                        await dataReader.LoadAsync((uint)randomAccessStream.Size);
                        dataReader.ReadBytes(BlobByteArray);
                    }

                    _mainViewModel.StartLoading($"Uploading your video ");


                    await _coffeeVideoStorage.OverwriteVideoAsync(_cloudBlockBlob, BlobByteArray, LeaseId);
                }


            }
            catch (StorageException ex) when (ex.RequestInformation.HttpStatusCode == (int)HttpStatusCode.PreconditionFailed && ex.RequestInformation.ErrorCode == "ConditionNotMet")
            {
                await ShowVideoChangedMessageAndReloadAsync();
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
                    await _coffeeVideoStorage.DeleteVideoAsync(_cloudBlockBlob, LeaseId);
                    _mainViewModel.RemoveCoffeeVideoViewModel(this);
                    _mainViewModel.StopLoading();
                    _mainViewModel = null;
                }
            }
            catch (StorageException ex) when (ex.RequestInformation.HttpStatusCode == (int)HttpStatusCode.PreconditionFailed && ex.RequestInformation.ErrorCode == "ConditionNotMet")
            {
                await ShowVideoChangedMessageAndReloadAsync();
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
                await _coffeeVideoStorage.UpdateMetadataAsync(_cloudBlockBlob, Title, Description, LeaseId);
                OnPropertyChanged(nameof(IsMetadataChanged));
            }
            catch (StorageException ex) when (ex.RequestInformation.HttpStatusCode == (int)HttpStatusCode.PreconditionFailed && ex.RequestInformation.ErrorCode == "ConditionNotMet")
            {
                await ShowVideoChangedMessageAndReloadAsync();
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

        public async Task AcquireLeaseAsync()
        {
            try
            {
                LeaseId = await _coffeeVideoStorage.AcquireOneMinuteLeaseAsync(_cloudBlockBlob);
                _leaseRenewTimer.Start();
                await _messageDialogService.ShowInfoDialogAsync($"Lease acquired. Lease Id:{LeaseId}", "Info");
            }
            catch (StorageException ex) when (ex.RequestInformation.HttpStatusCode == (int)HttpStatusCode.PreconditionFailed && ex.RequestInformation.ErrorCode == "ConditionNotMet")
            {
                await ShowVideoChangedMessageAndReloadAsync();
            }
            catch (Exception ex)
            {
                await _messageDialogService.ShowInfoDialogAsync(ex.Message, "Error");
            }
        }


        public async Task RenewLeaseAsync(CloudBlockBlob cloudBlockBlob, string leaseId)
        {
            try
            {
                var accessCondition = new AccessCondition
                {
                    LeaseId = leaseId
                };

                await cloudBlockBlob.RenewLeaseAsync(accessCondition);
            }
            catch (Exception ex)
            {
                await _messageDialogService.ShowInfoDialogAsync(ex.Message, "Error");
            }
        }


        public async Task ReleaseLeaseAsync()
        {
            try
            {
                _leaseRenewTimer.Stop();
                await _coffeeVideoStorage.ReleaseLeaseAsync(_cloudBlockBlob, LeaseId);
                LeaseId = null;
                await _messageDialogService.ShowInfoDialogAsync($"Lease was released", "Info");
            }
            catch (Exception ex)
            {
                await _messageDialogService.ShowInfoDialogAsync(ex.Message, "Error");
            }
        }


        public async Task ShowLeaseInfoAsync()
        {
            try
            {
                var leaseInfo = await _coffeeVideoStorage.LoadLeaseInfoAsync(_cloudBlockBlob);
                await _messageDialogService.ShowInfoDialogAsync(leaseInfo, "Info");
            }
            catch (Exception ex)
            {
                await _messageDialogService.ShowInfoDialogAsync(ex.Message, "Error");
            }
        }


        public async Task CreateSnapshotAsync()
        {
            try
            {
                await _coffeeVideoStorage.CreateSnapshotAsync(_cloudBlockBlob);
                await _messageDialogService.ShowInfoDialogAsync("Snapshot created", "Info");
            }
            catch (Exception ex)
            {
                await _messageDialogService.ShowInfoDialogAsync(ex.Message, "Error");
            }
        }

        public async Task PromoteSnapshotAsync()
        {
            try
            {
                await _coffeeVideoStorage.PromoteSnapshotAsync(_cloudBlockBlob);
                await _mainViewModel.ReloadAfterSnapshotPromotionAsync(this);
                await _messageDialogService.ShowInfoDialogAsync("Snapshot promoted", "Info");
            }
            catch (Exception ex)
            {
                await _messageDialogService.ShowInfoDialogAsync(ex.Message, "Error");
            }
        }


        private void UpdateViewModelPropertiesFromMetadata()
        {
            var (title, description) = _coffeeVideoStorage.GetBlobMetadata(_cloudBlockBlob);
            Title = title;
            Description = description;
        }

        private async Task ShowVideoChangedMessageAndReloadAsync()
        {
            await _messageDialogService.ShowInfoDialogAsync("Someone else has changed this record.  Record was reloaded.", "Info");
            await ReloadMetadataAsync();
            OnPropertyChanged(nameof(BlobUriWithSasToken));
        }

    }


}
