using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using Grpc.Net.ClientFactory;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using SciDevHome.Client.WinUI.ViewModels;
using SciDevHome.Data;
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
        // BreadcrumbBar1.ItemsSource = new ObservableCollection<string> { "Home", "Documents", "Design", "Northwind", "Images", "Folder1", "Folder2", "Folder3" };


    }

    private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // 魔改des
        ViewModel.GetPath("");

    }

    private void BreadcrumbBar1_ItemClicked(BreadcrumbBar sender, BreadcrumbBarItemClickedEventArgs args)
    {
        // var items = BreadcrumbBar1.ItemsSource as ObservableCollection<string>;
        // Debug.WriteLine(sender.Name);
        // Debug.WriteLine(args.Item);
        // Debug.WriteLine(args.Index);
        // for (int i = items.Count - 1; i >= args.Index + 1; i--)
        // {
        //     items.RemoveAt(i);
        // }


    }

    private void Page_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        // 此段工具化
        ViewModel.RefreshClient();
    }

    private async void NowFloderView_ItemClick(object sender, ItemClickEventArgs e)
    {
        
        // 最好是双击
        // 若是磁盘 可能需要去除/
        if (e.ClickedItem == null)
        {
            return;
        }
        var item = e.ClickedItem as Folder;
        if (item.IsDirectory)
        {
            ViewModel.BaseFolderPath.Add(item.Name);
            if (await ViewModel.GetPathFilename(item.Name))
            {
                
            }
            else
            {
                ViewModel.BaseFolderPath.RemoveAt(ViewModel.BaseFolderPath.Count - 1);
                ContentDialog dialog = new ContentDialog
                {
                    Title = "获取路径失败",
                    Content = "可能存在权限问题",
                    CloseButtonText = "关闭"
                };
                dialog.XamlRoot = this.XamlRoot;


                ContentDialogResult result = await dialog.ShowAsync();
            }
            

        }
        else
        {
            ViewModel.GetFileDetail(item.Name);
            // 检查同步
            // 然后尝试打开 
            // 获取文件信息
            // 这里只能依靠viewmodel? 不对 要更泛用话才是
        }
    }

    private void ClientFolderPath_OnItemClicked(BreadcrumbBar sender, BreadcrumbBarItemClickedEventArgs args)
    {
        var items = ViewModel.BaseFolderPath;
        for (int i = items.Count - 1; i >= args.Index + 1; i--)
        {
            items.RemoveAt(i);
        }
        
        ViewModel.GetPathFilename("");
    }

    private async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
        ContentDialog dialog1 = new ContentDialog
        {
            
        };
        dialog1.XamlRoot = this.XamlRoot;

        await dialog1.ShowAsync();
    }
}

public class BoolToFontIconConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is bool boolValue)
        {
            return boolValue ? "\uE838" : "\uF56E";
        }
        return string.Empty;
    }

    public object ConvertBack(object value, Type targetType, 
        object parameter, string language)
    {
        throw new NotImplementedException();
    }
    
}
