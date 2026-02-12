using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class movetomouse : MonoBehaviour
{
    public Vector3 mousepos;
    public bool Dragging;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        // Get mouse position in screen space
        Vector2 screenPos = Mouse.current.position.ReadValue();

        // Convert to world space
        mousepos = mainCamera.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, mainCamera.nearClipPlane));

        // For 2D, set z position appropriately
        mousepos.z = -2; // or whatever z-depth you want

        //Debug.Log(mousepos.ToString());
        this.gameObject.transform.position = mousepos;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "water")
        {
            if (Input.GetMouseButtonDown(0))
            {
                collision.gameObject.transform.position = mousepos;
            }
        }
    }
    /*void OnMouseDown()
    {
        gameObject.transform.position = mousepos;
    }*/
}