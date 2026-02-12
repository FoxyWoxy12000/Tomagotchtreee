using UnityEngine;
using UnityEngine.InputSystem;

public class WaterDrag : MonoBehaviour
{
    private bool dragging = false;
    private Vector3 offset;
    private Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        // Get mouse position using NEW Input System
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 mousePos = cam.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, 0));
        mousePos.z = -1;

        // When mouse button pressed down
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            // Cast a ray to see what we hit
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null && hit.collider.gameObject == this.gameObject)
            {
                Debug.Log("GRABBED " + gameObject.name);
                dragging = true;
                offset = transform.position - mousePos;
            }
        }

        // While dragging
        if (dragging)
        {
            transform.position = mousePos + offset;
        }

        // When mouse button released
        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            if (dragging)
            {
                Debug.Log("RELEASED " + gameObject.name);
            }
            dragging = false;
        }
    }
}