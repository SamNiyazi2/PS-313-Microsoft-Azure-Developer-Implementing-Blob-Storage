﻿using System.Threading.Tasks;
using WiredBrainCoffee.Storage;
using System;
using System.Collections.ObjectModel;
using WiredBrainCoffee.AdminApp.Service;

namespace WiredBrainCoffee.AdminApp.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private bool _isLoading;
        private string _loadingMessage;
        private readonly ICoffeeVideoStorage _coffeeVideoStorage;
        private readonly IAddCoffeeVideoDialogService _addCoffeeVideoDialogService;
        private readonly IMessageDialogService _messageDialogService;
        private CoffeeVideoViewModel _selectedCoffeeVideoViewModel;

        public MainViewModel(ICoffeeVideoStorage coffeeVideoStorage,
          IAddCoffeeVideoDialogService addCoffeeVideoDialogService,
          IMessageDialogService messageDialogService)
        {
            _coffeeVideoStorage = coffeeVideoStorage;
            _addCoffeeVideoDialogService = addCoffeeVideoDialogService;
            _messageDialogService = messageDialogService;
            CoffeeVideos = new ObservableCollection<CoffeeVideoViewModel>();
        }

        
        // 05/19/2021 07:27 am - SSN - [20210519-0709] - [002] - M04-02 - List the blobs of a container

        public async Task  LoadCoffeeVideosAsync()
        {
            try
            {
                
                
                // Todo 
                string prefix = "";




                var cloudBlockBlobs = await _coffeeVideoStorage.ListVideoBlobsAsync(prefix);
                CoffeeVideos.Clear();

                foreach( var cloudBlockBlob in cloudBlockBlobs)
                {
                    CoffeeVideos.Add(new CoffeeVideoViewModel(cloudBlockBlob));
                }
            }
            catch (Exception ex)
            {

                await _messageDialogService.ShowInfoDialogAsync(ex.Message, "Error");
            }
            finally
            {
               // Todo
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

        public string LoadingMessage
        {
            get { return _loadingMessage; }
            set
            {
                _loadingMessage = value;
                OnPropertyChanged();
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
                           dialogData.BlobName);


                    // 05/19/2021 05:40 am - SSN - [20210519-0529] - [003] - M03-05 - Show the blob URI of the uploaded blob
                    // TODO: Initialize CoffeeVideoViewModel with uploaded data
                    //var coffeeVideoViewModel = new CoffeeVideoViewModel
                    //{
                    //    BlobName = dialogData.BlobName,
                    //    BlobUri = "The Blob URI"
                    //};
                    var coffeeVideoViewModel = new CoffeeVideoViewModel(cloudBlockBlob);

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
