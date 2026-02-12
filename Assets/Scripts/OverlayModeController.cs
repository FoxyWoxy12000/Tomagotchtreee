using UnityEngine;

public class OverlayModeController : MonoBehaviour
{
    public DesktopWindowHelper desktopHelper;
    public ClickThroughController clickController;

    public enum Mode { Wallpaper, Overlay, Auto }
    public Mode currentMode = Mode.Auto;

    public float idleTimeout = 5f;
    float idleTimer = 0f;

    void Start()
    {
        ApplyMode();
    }

    void Update()
    {
        if (Input.anyKey || Input.mouseScrollDelta.sqrMagnitude > 0.01f
            || Mathf.Abs(Input.GetAxis("Mouse X")) > 0.01f
            || Mathf.Abs(Input.GetAxis("Mouse Y")) > 0.01f)
        {
            idleTimer = 0f;
        }
        else
        {
            idleTimer += Time.unscaledDeltaTime;
        }

        if (currentMode == Mode.Auto)
        {
            if (idleTimer > idleTimeout)
                desktopHelper.SetAsWallpaper();
            else
                desktopHelper.SetTopMost(true);
        }
    }

    public void ApplyMode()
    {
        switch (currentMode)
        {
            case Mode.Wallpaper: desktopHelper.SetAsWallpaper(); break;
            case Mode.Overlay: desktopHelper.SetTopMost(true); break;
            case Mode.Auto: break;
        }
    }
}
