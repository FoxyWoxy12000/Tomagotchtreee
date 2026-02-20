/*using UnityEngine;

public class WaterSpawner : MonoBehaviour
{
    public GameObject Water;
    public Vector2 WaterSpawnLocation;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SpawnNewWater()
    {
        WaterSpawnLocation = new Vector3(Random.Range(-8.28f, 8.28f), Random.Range(-4.4f, 4.4f), -1);
        Instantiate(Water, WaterSpawnLocation, Quaternion.identity);
    }
}
*/

using UnityEngine;

public class WaterSpawner : MonoBehaviour
{
    public GameObject Water;
    public Vector2 WaterSpawnLocation;

    [Header("Spawn Area")]
    public float minX = -8.28f;
    public float maxX = 8.28f;
    public float minY = -4.4f;
    public float maxY = 4.4f;

    public float minDistanceFromTree = 1.5f; // How far from tree to spawn

    void Start()
    {
    }

    void Update()
    {

    }

    public void SpawnNewWater()
    {
        // Try to find a valid spawn position
        Vector2 spawnPos = GetValidSpawnPosition();
        Instantiate(Water, spawnPos, Quaternion.identity);
    }

    Vector2 GetValidSpawnPosition()
    {
        GameObject tree = GameObject.FindGameObjectWithTag("tree");
        int attempts = 0;
        int maxAttempts = 20; // Prevent infinite loop

        while (attempts < maxAttempts)
        {
            Vector2 randomPos = new Vector2(
                Random.Range(minX, maxX),
                Random.Range(minY, maxY)
            );

            // Check distance from tree
            if (tree != null)
            {
                float distance = Vector2.Distance(randomPos, tree.transform.position);
                if (distance > minDistanceFromTree)
                {
                    return randomPos; // Valid position!
                }
            }
            else
            {
                return randomPos; // No tree, any position is fine
            }

            attempts++;
        }

        // If we can't find a good spot, just return a random one
        return new Vector2(Random.Range(minX, maxX), Random.Range(minY, maxY));
    }
}