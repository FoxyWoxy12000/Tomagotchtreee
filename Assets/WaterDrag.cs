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

    [Header("Drag Settings")]
    public float followSpeed = 10f;
    private Vector3 targetPosition;

    private Coroutine scaleTween;
    private Coroutine colorTween;

    [Header("Particle Effects")]
    public GameObject waterBurstParticle; // Assign in Inspector!
    public GameObject waterDripParticle;  // Assign in Inspector!
    private GameObject activeWaterDrip;   // Instance of drip particle

    void Start()
    {
        spriteRendererrr = this.gameObject.GetComponent<SpriteRenderer>();
        waterscrippy = GameObject.FindGameObjectWithTag("WaterSpawner").GetComponent<WaterSpawner>();
        cam = Camera.main;

        StartCoroutine(SpawnInAnimation());
    }

    void Update()
    {
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 mousePos = cam.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, 0));
        mousePos.z = -1;

        RaycastHit2D hitt = Physics2D.Raycast(mousePos, Vector2.zero);
        if (hitt.collider != null && hitt.collider.gameObject == this.gameObject)
        {
            TweenScale(new Vector3(1.2f, 1.2f, 1.2f), 0.1f);
            TweenColor(new Color32(0, 206, 255, 255), 0.1f);

            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                Debug.Log("GRABBED " + gameObject.name);
                dragging = true;
                offset = transform.position - mousePos;
                targetPosition = mousePos + offset;

                // NEW: Start water drip particles when dragging starts
                if (waterDripParticle != null)
                {
                    activeWaterDrip = Instantiate(waterDripParticle, transform.position, Quaternion.identity);
                    activeWaterDrip.transform.SetParent(transform); // Follow the water
                }
            }
        }
        else
        {
            TweenScale(new Vector3(1, 1, 1), 0.1f);
            TweenColor(new Color32(0, 170, 211, 255), 0.1f);
        }

        if (dragging)
        {
            targetPosition = mousePos + offset;
            transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);

            TweenScale(new Vector3(1.5f, 1.5f, 1.5f), 0.08f);
            TweenColor(new Color32(0, 11, 138, 255), 0.08f);
        }

        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            if (dragging)
            {
                Debug.Log("RELEASED " + gameObject.name);
                TweenScale(new Vector3(1, 1, 1), 0.15f);
                TweenColor(new Color32(0, 170, 211, 255), 0.15f);

                // NEW: Stop water drip particles
                if (activeWaterDrip != null)
                {
                    Destroy(activeWaterDrip);
                }
            }
            dragging = false;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("tree") && !dragging)
        {
            Debug.Log("watering da treeeeeeee");
            StartCoroutine(WaterTreeEffect());
        }
    }

    // NEW: Shrink and burst effect
    IEnumerator WaterTreeEffect()
    {
        // Stop accepting input
        GetComponent<Collider2D>().enabled = false;

        // Stop drip particles if active
        if (activeWaterDrip != null)
        {
            Destroy(activeWaterDrip);
        }

        // Shrink animation
        float duration = 0.3f;
        float elapsed = 0f;
        Vector3 startScale = transform.localScale;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            t = t * t * (3f - 2f * t); // Smooth easing

            transform.localScale = Vector3.Lerp(startScale, Vector3.zero, t);
            // Fade out
            Color c = spriteRendererrr.color;
            c.a = Mathf.Lerp(1f, 0f, t);
            spriteRendererrr.color = c;

            yield return null;
        }

        // Spawn burst particle
        if (waterBurstParticle != null)
        {
            GameObject burst = Instantiate(waterBurstParticle, transform.position, Quaternion.identity);
            Destroy(burst, 2f); // Destroy particle after 2 seconds
        }

        // Spawn new water
        waterscrippy.SpawnNewWater();

        // Destroy this water
        Destroy(this.gameObject);
    }

    void TweenScale(Vector3 targetScale, float duration)
    {
        if (scaleTween != null) StopCoroutine(scaleTween);
        scaleTween = StartCoroutine(ScaleTween(targetScale, duration));
    }

    void TweenColor(Color targetColor, float duration)
    {
        if (colorTween != null) StopCoroutine(colorTween);
        colorTween = StartCoroutine(ColorTween(targetColor, duration));
    }

    IEnumerator ScaleTween(Vector3 targetScale, float duration)
    {
        Vector3 startScale = transform.localScale;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            t = t * t * (3f - 2f * t);
            transform.localScale = Vector3.Lerp(startScale, targetScale, t);
            yield return null;
        }

        transform.localScale = targetScale;
    }

    IEnumerator ColorTween(Color targetColor, float duration)
    {
        Color startColor = spriteRendererrr.color;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            t = t * t * (3f - 2f * t);
            spriteRendererrr.color = Color.Lerp(startColor, targetColor, t);
            yield return null;
        }

        spriteRendererrr.color = targetColor;
    }

    IEnumerator SpawnInAnimation()
    {
        transform.localScale = Vector3.zero;
        spriteRendererrr.color = new Color32(0, 170, 211, 0);

        float duration = 0.2f;
        float elapsed = 0f;
        Vector3 targetScale = Vector3.one;
        Color targetColor = new Color32(0, 170, 211, 255);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            t = 1f - Mathf.Pow(1f - t, 3f);

            transform.localScale = Vector3.Lerp(Vector3.zero, targetScale, t);
            spriteRendererrr.color = Color.Lerp(new Color32(0, 170, 211, 0), targetColor, t);
            yield return null;
        }

        transform.localScale = targetScale;
        spriteRendererrr.color = targetColor;
    }
}