using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
namespace Notes.Model;

public class ItemModel
{
    public ICollection<ItemModel> Children { get; set; }
    public string Content { get; set; }
    public string Name { get; set; }
    public bool Editing { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    public Item ToItem()
    {
        ObservableCollection<Item> childrenToAdd = new ObservableCollection<Item>();
        foreach (var model in Children)
        {
            childrenToAdd.Add(model.ToItem());
        }
        return new Item
        {
            Children = childrenToAdd,
            Content = Content,
            Name = Name,
            Editing = Editing,
            CreatedAt = CreatedAt,
        };
    }
}
