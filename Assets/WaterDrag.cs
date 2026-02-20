/*using UnityEngine;
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

        *//*// When mouse button pressed down
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
        }*//*

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
}*/

using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class WaterDrag : MonoBehaviour
{
    private bool dragging = false;
    private Vector3 offset;
    private Camera cam;
    private WaterSpawner waterscrippy;
    private SpriteRenderer spriteRendererrr;

    // NEW: For smooth following
    [Header("Drag Settings")]
    public float followSpeed = 10f; // How fast water follows cursor (lower = more lag)
    private Vector3 targetPosition; // Where we want to move to

    // NEW: For tracking active tweens
    private Coroutine scaleTween;
    private Coroutine colorTween;

    void Start()
    {
        spriteRendererrr = this.gameObject.GetComponent<SpriteRenderer>();
        waterscrippy = GameObject.FindGameObjectWithTag("WaterSpawner").GetComponent<WaterSpawner>();
        cam = Camera.main;

        // NEW: Start with spawn-in animation
        StartCoroutine(SpawnInAnimation());
    }

    void Update()
    {
        // Get mouse position using NEW Input System
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 mousePos = cam.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, 0));
        mousePos.z = -2;

        RaycastHit2D hitt = Physics2D.Raycast(mousePos, Vector2.zero);
        if (hitt.collider != null && hitt.collider.gameObject == this.gameObject)
        {
            // NEW: Smooth tween instead of instant change
            TweenScale(new Vector3(1.2f, 1.2f, 1.2f), 0.1f);
            TweenColor(new Color32(0, 206, 255, 255), 0.1f);

            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                Debug.Log("GRABBED " + gameObject.name);
                dragging = true;
                offset = transform.position - mousePos;
                targetPosition = mousePos + offset; // NEW: Set initial target
            }
        }
        else
        {
            // NEW: Smooth tween back to normal
            TweenScale(new Vector3(1, 1, 1), 0.1f);
            TweenColor(new Color32(0, 170, 211, 255), 0.1f);
        }

        // While dragging
        if (dragging)
        {
            // NEW: Update target position instead of directly setting position
            targetPosition = mousePos + offset;

            // NEW: Smoothly move towards target (this creates the lag effect)
            transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);

            TweenScale(new Vector3(1.5f, 1.5f, 1.5f), 0.08f);
            TweenColor(new Color32(0, 11, 138, 255), 0.08f);
        }

        // When mouse button released
        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            if (dragging)
            {
                Debug.Log("WATER BUCKET RELEASED " + gameObject.name);
                TweenScale(new Vector3(1, 1, 1), 0.15f);
                TweenColor(new Color32(0, 170, 211, 255), 0.15f);
            }
            dragging = false;
        }
    }

    private void OnTriggerStay2D(Collider2D collision) 
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


    // ========== NEW TWEEN METHODS ==========

    // Smoothly scales the object over time
    void TweenScale(Vector3 targetScale, float duration)
    {
        // Stop any existing scale tween to prevent conflicts
        if (scaleTween != null) StopCoroutine(scaleTween);
        scaleTween = StartCoroutine(ScaleTween(targetScale, duration));
    }

    // Smoothly changes color over time
    void TweenColor(Color targetColor, float duration)
    {
        // Stop any existing color tween to prevent conflicts
        if (colorTween != null) StopCoroutine(colorTween);
        colorTween = StartCoroutine(ColorTween(targetColor, duration));
    }

    // Coroutine that handles the scale animation
    IEnumerator ScaleTween(Vector3 targetScale, float duration)
    {
        Vector3 startScale = transform.localScale;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            // Smooth interpolation using "smoothstep" formula for nice easing
            t = t * t * (3f - 2f * t);
            transform.localScale = Vector3.Lerp(startScale, targetScale, t);
            yield return null; // Wait one frame
        }

        transform.localScale = targetScale; // Ensure we hit exact target
    }

    // Coroutine that handles the color animation
    IEnumerator ColorTween(Color targetColor, float duration)
    {
        Color startColor = spriteRendererrr.color;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            // Smooth interpolation
            t = t * t * (3f - 2f * t);
            spriteRendererrr.color = Color.Lerp(startColor, targetColor, t);
            yield return null; // Wait one frame
        }

        spriteRendererrr.color = targetColor; // Ensure we hit exact target
    }

    // NEW: Animation when water first spawns
    IEnumerator SpawnInAnimation()
    {
        // Start at scale 0
        transform.localScale = Vector3.zero;
        spriteRendererrr.color = new Color32(0, 170, 211, 0); // Start transparent

        float duration = 0.25f;
        float elapsed = 0f;
        Vector3 targetScale = Vector3.one;
        Color targetColor = new Color32(0, 170, 211, 255);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            // "Ease out back" formula - gives a nice bouncy effect
            t = 1f - Mathf.Pow(1f - t, 3f);

            transform.localScale = Vector3.Lerp(Vector3.zero, targetScale, t);
            spriteRendererrr.color = Color.Lerp(new Color32(0, 170, 211, 0), targetColor, t);
            yield return null;
        }

        transform.localScale = targetScale;
        spriteRendererrr.color = targetColor;
    }
}