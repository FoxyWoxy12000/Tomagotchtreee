using UnityEngine;
using System;
using System.Runtime.InteropServices;

public class ClickThroughController : MonoBehaviour
{
    public LayerMask interactiveLayer;
    private Camera cam;
    private IntPtr hwnd;

    // Win32
    private const int GWL_EXSTYLE = -20;
    private const uint WS_EX_TRANSPARENT = 0x00000020;
    private const uint WS_EX_LAYERED = 0x00080000;

    [DllImport("user32.dll")]
    private static extern IntPtr GetActiveWindow();

    [DllImport("user32.dll")]
    private static extern uint GetWindowLong(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll")]
    private static extern uint SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);

    void Start()
    {
        cam = Camera.main;
        hwnd = GetActiveWindow();

        // Ensure layered window
        uint style = GetWindowLong(hwnd, GWL_EXSTYLE);
        SetWindowLong(hwnd, GWL_EXSTYLE, style | WS_EX_LAYERED);
    }

    void Update()
    {
        if (cam == null) return;

        Vector3 mousePos = Input.mousePosition;
        Vector2 worldPos = cam.ScreenToWorldPoint(mousePos);

        // 🔍 Check if the mouse is touching INTERACTIVE objects
        Collider2D hit = Physics2D.OverlapPoint(worldPos, interactiveLayer);

        if (hit != null)
            DisableClickThrough();   // game should get input
        else
            EnableClickThrough();    // desktop should get input
    }

    void EnableClickThrough()
    {
        uint style = GetWindowLong(hwnd, GWL_EXSTYLE);
        SetWindowLong(hwnd, GWL_EXSTYLE, style | WS_EX_TRANSPARENT | WS_EX_LAYERED);
    }

    void DisableClickThrough()
    {
        uint style = GetWindowLong(hwnd, GWL_EXSTYLE);
        SetWindowLong(hwnd, GWL_EXSTYLE, (style & ~WS_EX_TRANSPARENT) | WS_EX_LAYERED);
    }
}
