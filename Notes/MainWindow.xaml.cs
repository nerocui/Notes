using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Notes.Helpers;
using Notes.View;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;
using WinRT.Interop;
using WinRT;

namespace Notes;
public sealed partial class MainWindow : Window
{
    private AppWindow m_AppWindow;
    WindowsSystemDispatcherQueueHelper m_wsdqHelper; // See separate sample below for implementation
    Microsoft.UI.Composition.SystemBackdrops.MicaController m_micaController;
    Microsoft.UI.Composition.SystemBackdrops.SystemBackdropConfiguration m_configurationSource;
    public MainWindow()
    {
        this.InitializeComponent();
        TrySetMicaBackdrop();
        m_AppWindow = GetAppWindowForCurrentWindow();
        // Check to see if customization is supported.
        // Currently only supported on Windows 11.
        if (AppWindowTitleBar.IsCustomizationSupported())
        {
            var titleBar = m_AppWindow.TitleBar;
            // Hide default title bar.
            titleBar.ButtonBackgroundColor = Colors.Transparent;
            titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
            titleBar.ExtendsContentIntoTitleBar = true;
            AppTitleBar.Loaded += AppTitleBar_Loaded;
            AppTitleBar.SizeChanged += AppTitleBar_SizeChanged;
        }
        var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
        WindowHelpers.Hwnd = hwnd;
        WindowHelpers.XamlRoot = Content.XamlRoot;
        WindowHelpers.AppWindow = m_AppWindow;
        MainPageFrame.Navigate(typeof(MainPage));
    }

    bool TrySetMicaBackdrop()
    {
        if (Microsoft.UI.Composition.SystemBackdrops.MicaController.IsSupported())
        {
            m_wsdqHelper = new WindowsSystemDispatcherQueueHelper();
            m_wsdqHelper.EnsureWindowsSystemDispatcherQueueController();

            // Hooking up the policy object
            m_configurationSource = new Microsoft.UI.Composition.SystemBackdrops.SystemBackdropConfiguration();
            this.Activated += Window_Activated;
            this.Closed += Window_Closed;

            // Initial configuration state.
            m_configurationSource.IsInputActive = true;
            switch (((FrameworkElement)this.Content).ActualTheme)
            {
                case ElementTheme.Dark: m_configurationSource.Theme = Microsoft.UI.Composition.SystemBackdrops.SystemBackdropTheme.Dark; break;
                case ElementTheme.Light: m_configurationSource.Theme = Microsoft.UI.Composition.SystemBackdrops.SystemBackdropTheme.Light; break;
                case ElementTheme.Default: m_configurationSource.Theme = Microsoft.UI.Composition.SystemBackdrops.SystemBackdropTheme.Default; break;
            }

            m_micaController = new Microsoft.UI.Composition.SystemBackdrops.MicaController();

            // Enable the system backdrop.
            // Note: Be sure to have "using WinRT;" to support the Window.As<...>() call.
            m_micaController.AddSystemBackdropTarget(this.As<Microsoft.UI.Composition.ICompositionSupportsSystemBackdrop>());
            m_micaController.SetSystemBackdropConfiguration(m_configurationSource);
            return true; // succeeded
        }

        return false; // Mica is not supported on this system
    }

    private void Window_Activated(object sender, WindowActivatedEventArgs args)
    {
        m_configurationSource.IsInputActive = args.WindowActivationState != WindowActivationState.Deactivated;
    }

    private void Window_Closed(object sender, WindowEventArgs args)
    {
        // Make sure any Mica/Acrylic controller is disposed so it doesn't try to
        // use this closed window.
        if (m_micaController != null)
        {
            m_micaController.Dispose();
            m_micaController = null;
        }
        this.Activated -= Window_Activated;
        m_configurationSource = null;
    }

    private AppWindow GetAppWindowForCurrentWindow()
    {
        IntPtr hWnd = WindowNative.GetWindowHandle(this);
        WindowId wndId = Win32Interop.GetWindowIdFromWindow(hWnd);
        return AppWindow.GetFromWindowId(wndId);
    }

    private void AppTitleBar_Loaded(object sender, RoutedEventArgs e)
    {
        // Check to see if customization is supported.
        // Currently only supported on Windows 11.
        if (AppWindowTitleBar.IsCustomizationSupported())
        {
            SetDragRegionForCustomTitleBar(m_AppWindow);
        }
    }

    private void AppTitleBar_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        // Check to see if customization is supported.
        // Currently only supported on Windows 11.
        if (AppWindowTitleBar.IsCustomizationSupported()
            && m_AppWindow.TitleBar.ExtendsContentIntoTitleBar)
        {
            // Update drag region if the size of the title bar changes.
            SetDragRegionForCustomTitleBar(m_AppWindow);
        }
    }

    [DllImport("Shcore.dll", SetLastError = true)]
    internal static extern int GetDpiForMonitor(IntPtr hmonitor, Monitor_DPI_Type dpiType, out uint dpiX, out uint dpiY);

    internal enum Monitor_DPI_Type : int
    {
        MDT_Effective_DPI = 0,
        MDT_Angular_DPI = 1,
        MDT_Raw_DPI = 2,
        MDT_Default = MDT_Effective_DPI
    }

    private double GetScaleAdjustment()
    {
        IntPtr hWnd = WindowNative.GetWindowHandle(this);
        WindowId wndId = Win32Interop.GetWindowIdFromWindow(hWnd);
        DisplayArea displayArea = DisplayArea.GetFromWindowId(wndId, DisplayAreaFallback.Primary);
        IntPtr hMonitor = Win32Interop.GetMonitorFromDisplayId(displayArea.DisplayId);

        // Get DPI.
        int result = GetDpiForMonitor(hMonitor, Monitor_DPI_Type.MDT_Default, out uint dpiX, out uint _);
        if (result != 0)
        {
            throw new Exception("Could not get DPI for monitor.");
        }

        uint scaleFactorPercent = (uint)(((long)dpiX * 100 + (96 >> 1)) / 96);
        return scaleFactorPercent / 100.0;
    }

    private void SetDragRegionForCustomTitleBar(AppWindow appWindow)
    {
        // Check to see if customization is supported.
        // Currently only supported on Windows 11.
        if (AppWindowTitleBar.IsCustomizationSupported()
            && appWindow.TitleBar.ExtendsContentIntoTitleBar)
        {
            double scaleAdjustment = GetScaleAdjustment();
            List<Windows.Graphics.RectInt32> dragRectsList = new();

            Windows.Graphics.RectInt32 dragRectL;
            dragRectL.X = 0;
            dragRectL.Y = 0;
            dragRectL.Height = (int)(AppTitleBar.ActualHeight * scaleAdjustment);
            dragRectL.Width = (int)((AppTitleBar.ActualWidth) * scaleAdjustment);
            dragRectsList.Add(dragRectL);

            Windows.Graphics.RectInt32[] dragRects = dragRectsList.ToArray();

            appWindow.TitleBar.SetDragRectangles(dragRects);
        }
    }
}
