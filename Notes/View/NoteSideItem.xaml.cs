using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Notes.Model;

namespace Notes.View;

public sealed partial class NoteSideItem : UserControl
{
    public static DependencyProperty ItemProperty = DependencyProperty.Register(
        nameof(Item),
        typeof(Item),
        typeof(NoteSideItem),
        new PropertyMetadata(null, OnItemChange));

    private static void OnItemChange(DependencyObject d, DependencyPropertyChangedEventArgs args)
    {
        if (d is NoteSideItem self && args.NewValue != null)
        {
            self.DataContext = self.Item;
        }
    }

    public Item Item
    {
        get => (Item)GetValue(ItemProperty);
        set => SetValue(ItemProperty, value);
    }
    public NoteSideItem()
    {
        this.InitializeComponent();
    }

    private void TextBox_KeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            Item.Editing = false;
        }
    }
}
