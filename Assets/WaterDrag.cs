using UnityEngine;
using UnityEngine.InputSystem;

public class WaterDrag : MonoBehaviour
{
    private bool dragging = false;
    private Vector3 offset;
    private Camera cam;
    private WaterSpawner waterscrippy;
    private SpriteRenderer spriteRendererrr;

    void Start()
    {
        spriteRendererrr = this.gameObject.GetComponent<SpriteRenderer>();
        waterscrippy = GameObject.FindGameObjectWithTag("WaterSpawner").GetComponent<WaterSpawner>();
        cam = Camera.main;
    }

    void Update()
    {
        // Get mouse position using NEW Input System
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 mousePos = cam.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, 0));
        mousePos.z = -1;
        
        RaycastHit2D hitt = Physics2D.Raycast(mousePos, Vector2.zero);
        if (hitt.collider != null && hitt.collider.gameObject == this.gameObject)
        {
            this.gameObject.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
            spriteRendererrr.color = new Color32(0, 206, 255, 255);
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                Debug.Log("GRABBED " + gameObject.name);
                dragging = true;
                offset = transform.position - mousePos;
            }

        } else
        {
            this.gameObject.transform.localScale = new Vector3(1, 1, 1);
            spriteRendererrr.color = new Color32(0, 170, 211, 255);
        }

        /*// When mouse button pressed down
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
        }*/

        // While dragging
        if (dragging)
        {
            transform.position = mousePos + offset;
            this.gameObject.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            spriteRendererrr.color = new Color32(0,11,138,255);
        }

        // When mouse button released
        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            if (dragging)
            {
                Debug.Log("RELEASED " + gameObject.name);
                this.gameObject.transform.localScale = new Vector3(1,1,1);
                spriteRendererrr.color = new Color32(0,170,211,255);
            }
            dragging = false;
        }
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "tree") 
        {
            Debug.Log("watering da treeeeeeee");
            if (!dragging)
            {
                waterscrippy.SpawnNewWater();
                Destroy(this.gameObject);
            }
        }
    }
}