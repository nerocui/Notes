using Microsoft.UI.Xaml;
using System;

namespace Notes.Helpers
{
    internal static class Extensions
    {
        internal static Visibility IsVisible(this bool vis)
        {
            return vis ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
