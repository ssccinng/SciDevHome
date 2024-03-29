﻿using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml;

using SciDevHome.Client.WinUI.Activation;
using SciDevHome.Client.WinUI.Contracts.Services;
using SciDevHome.Client.WinUI.Core.Contracts.Services;
using SciDevHome.Client.WinUI.Core.Services;
using SciDevHome.Client.WinUI.Helpers;
using SciDevHome.Client.WinUI.Models;
using SciDevHome.Client.WinUI.Notifications;
using SciDevHome.Client.WinUI.Services;
using SciDevHome.Client.WinUI.ViewModels;
using SciDevHome.Client.WinUI.Views;
using SciDevHome.Server;

using Windows.Services.Maps;
using SciDevHome.Message;
using SciDevHome.Utils;

namespace SciDevHome.Client.WinUI;

// To learn more about WinUI 3, see https://docs.microsoft.com/windows/apps/winui/winui3/.
public partial class App : Application
{
    // The .NET Generic Host provides dependency injection, configuration, logging, and other services.
    // https://docs.microsoft.com/dotnet/core/extensions/generic-host
    // https://docs.microsoft.com/dotnet/core/extensions/dependency-injection
    // https://docs.microsoft.com/dotnet/core/extensions/configuration
    // https://docs.microsoft.com/dotnet/core/extensions/logging
    public IHost Host
    {
        get;
    }

    public static T GetService<T>()
        where T : class
    {
        if ((App.Current as App)!.Host.Services.GetService(typeof(T)) is not T service)
        {
            throw new ArgumentException($"{typeof(T)} needs to be registered in ConfigureServices within App.xaml.cs.");
        }

        return service;
    }

    public static WindowEx MainWindow { get; } = new MainWindow();

    public static UIElement? AppTitlebar { get; set; }

    public App()
    {
        InitializeComponent();

        Host = Microsoft.Extensions.Hosting.Host.
        CreateDefaultBuilder().
        UseContentRoot(AppContext.BaseDirectory).
        ConfigureServices((context, services) =>
        {
            // Default Activation Handler
            services.AddTransient<ActivationHandler<LaunchActivatedEventArgs>, DefaultActivationHandler>();

            // Other Activation Handlers
            services.AddTransient<IActivationHandler, AppNotificationActivationHandler>();

            // Services
            services.AddSingleton<IAppNotificationService, AppNotificationService>();
            services.AddSingleton<ILocalSettingsService, LocalSettingsService>();
            services.AddSingleton<IThemeSelectorService, ThemeSelectorService>();
            services.AddTransient<INavigationViewService, NavigationViewService>();

            services.AddSingleton<IActivationService, ActivationService>();
            services.AddSingleton<IPageService, PageService>();
            services.AddSingleton<INavigationService, NavigationService>();

            // Core Services
            services.AddSingleton<ISampleDataService, SampleDataService>();
            services.AddSingleton<IFileService, FileService>();

            // Views and ViewModels
            services.AddTransient<SoftwareDownloadDetailViewModel>();
            services.AddTransient<SoftwareDownloadDetailPage>();
            services.AddTransient<SoftwareDownloadViewModel>();
            services.AddTransient<SoftwareDownloadPage>();
            services.AddTransient<ServerConnectViewModel>();
            services.AddTransient<ServerConnectPage>();
            services.AddTransient<DirctoryPathViewViewModel>();
            services.AddTransient<DirctoryPathViewPage>();
            services.AddTransient<SettingsViewModel>();
            services.AddTransient<SettingsPage>();
            services.AddTransient<ClientListDetailViewModel>();
            services.AddTransient<ClientListDetailPage>();
            services.AddTransient<ClientListViewModel>();
            services.AddTransient<ClientListPage>();
            services.AddTransient<MainViewModel>();
            services.AddTransient<MainPage>();
            services.AddTransient<ShellPage>();
            services.AddTransient<ShellViewModel>();

            services.AddSingleton<DevHomeClientService>();

            // 读取文件

            services.AddGrpcClient<Greeter.GreeterClient>("test", options =>
            {
                // 需要读取配置， 但这样只能单例
                // options.Address = new Uri("http://172.168.35.51:45152");
                options.Address = new Uri("http://127.0.0.1:45152");
            });

            // Configuration
            services.Configure<LocalSettingsOptions>(context.Configuration.GetSection(nameof(LocalSettingsOptions)));
        }).
        Build();

        App.GetService<IAppNotificationService>().Initialize();

        UnhandledException += App_UnhandledException;
    }





    private void App_UnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
    {
        // TODO: Log and handle exceptions as appropriate.
        // https://docs.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.application.unhandledexception.
    }

    protected async override void OnLaunched(LaunchActivatedEventArgs args)
    {
        base.OnLaunched(args);

        App.GetService<IAppNotificationService>().Show(string.Format("AppNotificationSamplePayload".GetLocalized(), AppContext.BaseDirectory));

        await App.GetService<IActivationService>().ActivateAsync(args);
        await Init();
    }
    
    
    async Task Init()
    {
        var ViewModel = App.GetService<MainViewModel>();
        MainViewModel.Saves = await SaveFileManager.LoadAsync("devhomeSetting.json");

        if (MainViewModel.Saves.ClientId == string.Empty)
        {
            var regRes = await ViewModel.Client.RegisterAsync(new SciDevHome.Server.ClientInfo { Name = "临流" });
            MainViewModel.Saves.ClientId = regRes.ClientId;
            await Utils.SaveFileManager.SaveAsync("devhomeSetting.json", MainViewModel.Saves);
        }
        if (MainViewModel.Saves.ComputerName == string.Empty)
        {
            MainViewModel.Saves.ComputerName = ZQDHelper.GetRandomName();
            await Utils.SaveFileManager.SaveAsync("devhomeSetting.json", MainViewModel.Saves);

        }

        var stream = ViewModel.Client.Connect();
        
        // 啊
        await stream.RequestStream.WriteAsync(new Server.ConnectRequest
        {
            Cmd = "InitClient",
            Data = JsonSerializer.Serialize(new ClientIdUpdateMessage { ClientId = MainViewModel.Saves.ClientId ,
                Name = MainViewModel.Saves.ComputerName


            })
        });
        // 不要放main里
        new Thread(() => { MainViewModel.ListenServer(stream); }).Start();

    }
}
