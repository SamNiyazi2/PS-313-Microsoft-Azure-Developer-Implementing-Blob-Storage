﻿using Autofac;
using System;
using WiredBrainCoffee.AdminApp.Service;
using WiredBrainCoffee.AdminApp.Settings;
using WiredBrainCoffee.AdminApp.View;
using WiredBrainCoffee.AdminApp.ViewModel;
using WiredBrainCoffee.Storage;

namespace WiredBrainCoffee.AdminApp.Startup
{
    class Bootstrapper
    {
        public IContainer Bootstrap()
        {
            var builder = new ContainerBuilder();

            //builder.RegisterType<MainViewModel>().AsSelf().SingleInstance();
            builder.RegisterType<MainViewModel>().As<IMainViewModel>().AsSelf().SingleInstance();

            
            builder.RegisterType<CoffeeVideoViewModel>().AsSelf();

            builder.RegisterType<CoffeeVideoStorage>().As<ICoffeeVideoStorage>().WithParameter("blobStorageConnectionString", Environment.GetEnvironmentVariable("ps312AzureTableConnectionString_azureTable"));

            builder.RegisterType<AddCoffeeVideoDialog>().AsSelf();
            builder.RegisterType<AddCoffeeVideoDialogViewModel>().As<IAddCoffeeVideoDialogViewModel>().AsSelf();

            builder.RegisterType<AddCoffeeVideoDialogService>().As<IAddCoffeeVideoDialogService>();
            builder.RegisterType<MessageDialogService>().As<IMessageDialogService>();
            builder.RegisterType<FilePickerDialogService>().As<IFilePickerDialogService>();

            return builder.Build();
        }
    }
}
