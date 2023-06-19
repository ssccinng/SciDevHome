using System.Collections.ObjectModel;
using System.Diagnostics;
using Grpc.Net.ClientFactory;
using Microsoft.UI.Xaml.Controls;

using SciDevHome.Client.WinUI.ViewModels;
using SciDevHome.Server;

namespace SciDevHome.Client.WinUI.Views;

public sealed partial class DirctoryPathViewPage : Page
{



    public DirctoryPathViewViewModel ViewModel
    {
        get;
    }
    public DirctoryPathViewPage()
    {
        ViewModel = App.GetService<DirctoryPathViewViewModel>();
        InitializeComponent();
        BreadcrumbBar1.ItemsSource = new ObservableCollection<string> { "Home", "Documents", "Design", "Northwind", "Images", "Folder1", "Folder2", "Folder3" };


    }

    private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
    }

    private void BreadcrumbBar1_ItemClicked(BreadcrumbBar sender, BreadcrumbBarItemClickedEventArgs args)
    {
        var items = BreadcrumbBar1.ItemsSource as ObservableCollection<string>;
        Debug.WriteLine(sender.Name);
        Debug.WriteLine(args.Item);
        Debug.WriteLine(args.Index);
        for (int i = items.Count - 1; i >= args.Index + 1; i--)
        {
            items.RemoveAt(i);
        }


    }

    private void Page_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        // 此段工具化
        ViewModel.RefreshClient();
    }
}
