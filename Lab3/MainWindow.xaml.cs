using System.Windows;
using Lab3.Services;
using Lab3.ViewModels;
using Lab3.Views;

namespace Lab3;

public partial class MainWindow : Window
{
    private readonly MainViewModel _viewModel;

    public MainWindow()
    {
        InitializeComponent();

        var repository = new JsonTaskRepository();
        _viewModel = new MainViewModel(repository)
        {
            ShowEditDialog = ShowEditDialog,
            ShowConfirmDialog = ShowConfirmDialog
        };

        DataContext = _viewModel;
        Loaded += (_, _) => _viewModel.LoadCommand.Execute(null);
    }

    private bool? ShowEditDialog(TaskEditViewModel vm)
    {
        var dialog = new TaskEditDialog(vm) { Owner = this };
        return dialog.ShowDialog();
    }

    private bool ShowConfirmDialog(string message)
    {
        var result = MessageBox.Show(message, "Подтверждение",
            MessageBoxButton.YesNo, MessageBoxImage.Question);
        return result == MessageBoxResult.Yes;
    }
}
