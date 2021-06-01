using System.Threading.Tasks;
using WiredBrainCoffee.Storage;
using System;
using System.Collections.ObjectModel;
using WiredBrainCoffee.AdminApp.Service;
using Microsoft.Azure.Storage.Blob;
using System.Linq;

namespace WiredBrainCoffee.AdminApp.ViewModel
{

    public interface IMainViewModel
    {
        void StartLoading(string message);
        void StopLoading();
        void RemoveCoffeeVideoViewModel(CoffeeVideoViewModel coffeeVideoViewModel);
        Task ReloadAfterSnapshotPromotionAsync(CoffeeVideoViewModel coffeeVideoViewModel);
    }

    public class MainViewModel : ViewModelBase, IMainViewModel
    {
        private bool _isLoading;
        private string _loadingMessage;
        private readonly ICoffeeVideoStorage _coffeeVideoStorage;
        private readonly IAddCoffeeVideoDialogService _addCoffeeVideoDialogService;
        private readonly IMessageDialogService _messageDialogService;
        private CoffeeVideoViewModel _selectedCoffeeVideoViewModel;

        private readonly Func<CloudBlockBlob, CoffeeVideoViewModel> _coffeeVideoViewModelCreator;


        private bool _includeSnapshots;

        public bool IncludeSnapshots
        {
            get { return _includeSnapshots; }
            set
            {
                _includeSnapshots = value;
                OnPropertyChanged(nameof(IncludeSnapshots));

            }
        }


        public MainViewModel(ICoffeeVideoStorage coffeeVideoStorage,
          IAddCoffeeVideoDialogService addCoffeeVideoDialogService,
          IMessageDialogService messageDialogService,




        Func<CloudBlockBlob, CoffeeVideoViewModel> coffeeVideoViewModelCreator)


        {
            _coffeeVideoStorage = coffeeVideoStorage;
            _addCoffeeVideoDialogService = addCoffeeVideoDialogService;
            _messageDialogService = messageDialogService;

            _coffeeVideoViewModelCreator = coffeeVideoViewModelCreator;


            CoffeeVideos = new ObservableCollection<CoffeeVideoViewModel>();



        }



        // 05/19/2021 07:27 am - SSN - [20210519-0709] - [002] - M04-02 - List the blobs of a container

        public async Task LoadCoffeeVideosAsync()
        {
            StartLoading("We're loading the videos for you");

            try
            {

                var cloudBlockBlobs = await _coffeeVideoStorage.ListVideoBlobsAsync(Prefix, IncludeSnapshots);
                CoffeeVideos.Clear();

                foreach (var cloudBlockBlob in cloudBlockBlobs)
                {
                    ////////////////////////////////////////// CoffeeVideos.Add(new CoffeeVideoViewModel(cloudBlockBlob));

                    CoffeeVideos.Add(_coffeeVideoViewModelCreator(cloudBlockBlob));
                }
            }
            catch (Exception ex)
            {

                await _messageDialogService.ShowInfoDialogAsync(ex.Message, "Error");
            }
            finally
            {
                StopLoading();
            }

        }





        public ObservableCollection<CoffeeVideoViewModel> CoffeeVideos { get; }

        public CoffeeVideoViewModel SelectedCoffeeVideo
        {
            get { return _selectedCoffeeVideoViewModel; }
            set
            {
                if (_selectedCoffeeVideoViewModel != value)
                {
                    _selectedCoffeeVideoViewModel = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsCoffeeVideoSelected));
                }
            }
        }

        public bool IsCoffeeVideoSelected => SelectedCoffeeVideo != null;

        public async Task AddCoffeeVideoAsync()
        {
            try
            {
                var dialogData = await _addCoffeeVideoDialogService.ShowDialogAsync();

                if (dialogData.DialogResultIsOk)
                {
                    StartLoading($"Uploading your video {dialogData.BlobName}");

                    var cloudBlockBlob = await _coffeeVideoStorage.UploadVideoAsync(
                           dialogData.BlobByteArray,
                           dialogData.BlobName,
                           dialogData.BlobTitle,
                           dialogData.BlobDescription);


                    // 05/19/2021 05:40 am - SSN - [20210519-0529] - [003] - M03-05 - Show the blob URI of the uploaded blob
                    // TODO: Initialize CoffeeVideoViewModel with uploaded data
                    //var coffeeVideoViewModel = new CoffeeVideoViewModel
                    //{
                    //    BlobName = dialogData.BlobName,
                    //    BlobUri = "The Blob URI"
                    //};
                    /////////////////////////////////    var coffeeVideoViewModel = new CoffeeVideoViewModel(cloudBlockBlob);

                    var coffeeVideoViewModel = _coffeeVideoViewModelCreator(cloudBlockBlob);
                    CoffeeVideos.Add(coffeeVideoViewModel);
                    SelectedCoffeeVideo = coffeeVideoViewModel;
                }
            }
            catch (Exception ex)
            {
                await _messageDialogService.ShowInfoDialogAsync(ex.Message, "Error");
            }
            finally
            {
                StopLoading();
            }
        }






        public void RemoveCoffeeVideoViewModel(CoffeeVideoViewModel viewModel)
        {
            if (CoffeeVideos.Contains(viewModel))
            {
                CoffeeVideos.Remove(viewModel);
                if (SelectedCoffeeVideo == viewModel)
                {
                    SelectedCoffeeVideo = null;
                }
            }
        }



        public async Task ReloadAfterSnapshotPromotionAsync(CoffeeVideoViewModel snapshotViewModel)
        {
            var coffeeVideoViewModel = CoffeeVideos.SingleOrDefault(
                        viewModel => viewModel.BlobName == snapshotViewModel.BlobName
                                  && !viewModel.IsSnapshot);

            if (coffeeVideoViewModel != null)
            {
                await coffeeVideoViewModel.ReloadMetadataAsync();
            }
        }




        public string LoadingMessage
        {
            get { return _loadingMessage; }
            set
            {
                _loadingMessage = value;
                OnPropertyChanged();
            }
        }

        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }

        // 05/19/2021 08:44 am - SSN - [20210519-0836] - [001] - M04-04 - Filter blobs with a prefix
        private string _Prefix;

        public string Prefix
        {
            get { return _Prefix; }
            set
            {
                _Prefix = value;
                OnPropertyChanged();
            }
        }



        public void StartLoading(string message)
        {
            LoadingMessage = message;
            IsLoading = true;
        }

        public void StopLoading()
        {
            IsLoading = false;
            LoadingMessage = null;
        }
    }
}
