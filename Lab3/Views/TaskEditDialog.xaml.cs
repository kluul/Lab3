using System.Windows;
using Lab3.ViewModels;

namespace Lab3.Views;

public partial class TaskEditDialog : Window
{
    public TaskEditDialog(TaskEditViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
    }
}
