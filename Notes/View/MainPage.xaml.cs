using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using Notes.ViewModel;

namespace Notes.View;

public sealed partial class MainPage : Page
{
    internal MainViewModel ViewModel { get; set; }
    public MainPage()
    {
        this.InitializeComponent();
        ViewModel = new MainViewModel(new RelayCommand<string>(value => NoteBox.Text = value));
        DataContext = ViewModel;
        ViewModel.Load();
    }

    private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (ViewModel != null && ViewModel.SelectedNote != null)
        {
            ViewModel.SelectedNote.Content = ((TextBox)sender).Text;
        }
    }
}
