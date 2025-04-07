using UnityEngine;
using System.Collections;

public class CrowFlockSpawner : MonoBehaviour
{
    public GameObject crowPrefab;
    public int crowCount = 2;
    public float spawnRadius = 10f;
    public Transform spawnPoint;

    void Start()
    {
        StartCoroutine(SpawnFlock());
    }

    IEnumerator SpawnFlock()
    {
        for (int i = 0; i < crowCount; i++)
        {
            Vector3 randomOffset = Random.insideUnitSphere * spawnRadius;
            randomOffset.y = Mathf.Abs(randomOffset.y) + 2f; // keep them above the ground
            Vector3 spawnPos = spawnPoint.position + randomOffset;

            GameObject crow = Instantiate(crowPrefab, spawnPos, Quaternion.identity);

            yield return new WaitForSeconds(0.1f); // slight delay between spawns
        }
    }
}
