using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class DesktopWindowHelper : MonoBehaviour
{
    const int GWL_EXSTYLE = -20;
    const int GWL_STYLE = -16;

    const uint SWP_NOSIZE = 0x0001;
    const uint SWP_NOMOVE = 0x0002;
    const uint SWP_NOACTIVATE = 0x0010;
    const uint SWP_SHOWWINDOW = 0x0040;

    static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
    static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);
    static readonly IntPtr HWND_BOTTOM = new IntPtr(1);

    [DllImport("user32.dll")] static extern IntPtr GetActiveWindow();
    [DllImport("user32.dll")] static extern bool SetWindowPos(IntPtr hWnd, IntPtr insert, int X, int Y, int cx, int cy, uint flags);

    IntPtr hwnd;

    void Awake()
    {
        hwnd = GetActiveWindow();
    }

    public void SetTopMost(bool enable)
    {
        SetWindowPos(hwnd,
            enable ? HWND_TOPMOST : HWND_NOTOPMOST,
            0, 0, 0, 0,
            SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE | SWP_SHOWWINDOW);
    }

    public void SetAsWallpaper()
    {
        SetWindowPos(hwnd,
            HWND_BOTTOM,
            0, 0, 0, 0,
            SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE | SWP_SHOWWINDOW);
    }

    public void SetNormalWindow()
    {
        SetWindowPos(hwnd,
            HWND_NOTOPMOST,
            0, 0, 0, 0,
            SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE | SWP_SHOWWINDOW);
    }
    public void MoveToMonitor(int monitorIndex)
    {
        var screens = Display.displays;

        if (monitorIndex < 0 || monitorIndex >= screens.Length)
        {
            Debug.LogWarning("Invalid monitor number");
            return;
        }

        int x = screens[monitorIndex].systemWidth * monitorIndex;
        int y = 0;

        SetWindowPos(hwnd, IntPtr.Zero, x, y, 0, 0,
            0x0001 | 0x0002 | 0x0040);
    }
}
