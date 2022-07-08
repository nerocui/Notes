using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Notes.Model;
public class Item : ObservableObject
{

    private string _name;
    private bool _editing;
    private DateTimeOffset _createdAt;
    private string _content;

    public ObservableCollection<Item> Children { get; set; }
    public string Content
    {
        get => _content;
        set => SetProperty(ref _content, value);
    }
    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }
    public bool Editing
    {
        get => _editing;
        set => SetProperty(ref _editing, value);
    }
    public DateTimeOffset CreatedAt
    {
        get => _createdAt;
        set => SetProperty(ref _createdAt, value);
    }

    public Item()
    {
        Children = new();
    }

    public Item Self => this;

    public void Rename()
    {
        Editing = true;
    }

    public ItemModel ToModel()
    {
        var childrenToAdd = new List<ItemModel>();
        foreach (var item in Children)
        {
            childrenToAdd.Add(item.ToModel());
        }
        return new ItemModel
        {
            Name = Name,
            Content = Content,
            CreatedAt = CreatedAt,
            Editing = Editing,
            Children = childrenToAdd,
        };
    }
}
