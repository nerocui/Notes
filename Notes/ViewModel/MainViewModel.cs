using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Notes.Helpers;
using Notes.Model;
using System;
using System.Windows.Input;

namespace Notes.ViewModel;

internal class MainViewModel : ObservableObject
{
    private bool _loading;
    private Item _parentFolder;
    private Visibility _hasSelectedNote = Visibility.Collapsed;
    private Visibility _isEmpty = Visibility.Visible;
    private Item _selectedNote;

    public bool Loading
    {
        get => _loading;
        set => SetProperty(ref _loading, value);
    }
    public Item ParentFolder
    {
        get => _parentFolder;
        set => SetProperty(ref _parentFolder, value);
    }
    public Item SelectedNote
    {
        get => _selectedNote;
        set
        {
            SetProperty(ref _selectedNote, value);
            HasSelectedNote = (value != null).IsVisible();
            IsEmpty = HasSelectedNote == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
            if (value != null && LoadText != null && LoadText.CanExecute(value.Content))
            {
                LoadText.Execute(value.Content);
            }
        }
    }
    public Visibility HasSelectedNote
    {
        get => _hasSelectedNote;
        set => SetProperty(ref _hasSelectedNote, value);
    }

    public Visibility IsEmpty
    {
        get => _isEmpty;
        set => SetProperty(ref _isEmpty, value);
    }

    public ICommand LoadText { get; }

    public MainViewModel(ICommand loadText)
    {
        LoadText = loadText;
        WindowHelpers.AppWindow.Closing += HandleClosing;
    }

    private async void HandleClosing(AppWindow window, AppWindowClosingEventArgs args)
    {
        try
        {
            args.Cancel = true;
            var res = await StorageHelper.SaveFile<ItemModel>(ParentFolder.ToModel());
            if (res)
            {
                window.Closing -= HandleClosing;
                window.Destroy();
            }
        }
        catch (Exception e)
        {

        }
    }

    public void Create()
    {
        var newitem = new Item() { Editing = true };
        ParentFolder.Children.Add(newitem);
        SelectedNote = newitem;
    }

    public async void Load()
    {
        Loading = true;
        try
        {
            var folder = await StorageHelper.GetFile<ItemModel>();
            if (folder == null)
            {
                ParentFolder = new Item()
                {
                    CreatedAt = DateTimeOffset.Now,
                };
                await StorageHelper.SaveFile<ItemModel>(ParentFolder.ToModel());
            }
            else
            {
                ParentFolder = folder.ToItem();
            }
        }
        catch(Exception e)
        {
            ParentFolder = new Item()
            {
                CreatedAt = DateTimeOffset.Now,
            };
            await StorageHelper.SaveFile<ItemModel>(ParentFolder.ToModel());
        }
        Loading = false;
    }
}
