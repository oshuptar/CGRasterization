using Avalonia.Controls;
using CGRasterization.App.ViewModels;

namespace CGRasterization.App.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        DataContext = new MainWindowViewModel();
        InitializeComponent();
    }
}