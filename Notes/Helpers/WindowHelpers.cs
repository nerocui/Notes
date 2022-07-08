using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using System;

namespace Notes.Helpers;

public class WindowHelpers
{
    public static IntPtr Hwnd { get; set; }
    public static XamlRoot XamlRoot { get; set; }
    public static AppWindow AppWindow { get; set; }
}
