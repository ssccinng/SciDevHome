using System.Text.Json;
using Microsoft.UI.Xaml.Controls;

using SciDevHome.Client.WinUI.ViewModels;
using SciDevHome.Message;
using SciDevHome.Utils;

namespace SciDevHome.Client.WinUI.Views;

public sealed partial class MainPage : Page
{
    public MainViewModel ViewModel
    {
        get;
    }

    public MainPage()
    {
        ViewModel = App.GetService<MainViewModel>();
        InitializeComponent();
    }
    async Task Init()
    { 
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
    private async void Page_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
       await Init();
    }
}
