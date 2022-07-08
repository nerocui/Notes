using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Notes.Model;
namespace Notes.View
{
    public class NotesTemplateSelector : DataTemplateSelector
    {
        public DataTemplate FolderTemplate { get; set; }
        public DataTemplate NoteTemplate { get; set; }
        protected override DataTemplate SelectTemplateCore(object item)
        {
            var node = item as Item;
            if (node.Children.Count == 0)
            {
                return NoteTemplate;
            }
            else
            {
                return FolderTemplate;
            }
        }
    }
}
