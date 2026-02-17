using UnityEngine;

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
        WaterSpawnLocation = new Vector2(Random.Range(-8.28f, 8.28f), Random.Range(-4.4f, 4.4f));
        Instantiate(Water, WaterSpawnLocation, Quaternion.identity);
    }
}
