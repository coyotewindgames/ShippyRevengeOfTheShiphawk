using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ZoneController : MonoBehaviour
{
    [Tooltip("Assign zone Transforms in order. If empty, will try to find objects tagged 'Zone' or use this object's children.")]
    public Transform[] zones;

    [Tooltip("Enemy prefab to spawn at each zone.")]
    public GameObject enemyPrefab;

    [Tooltip("Minimum delay between spawns (seconds)")]
    public float minDelay = 5f;

    [Tooltip("Maximum delay between spawns (seconds)")]
    public float maxDelay = 7f;

    int currentIndex = 0;

    void Start()
    {
        // Try to auto-gather zones if none assigned in Inspector
        if (zones == null || zones.Length == 0)
        {
            var found = GameObject.FindGameObjectsWithTag("Zone");
            if (found != null && found.Length > 0)
            {
                zones = new Transform[found.Length];
                for (int i = 0; i < found.Length; i++) zones[i] = found[i].transform;
            }
            else
            {
                // fallback: use this object's immediate children
                List<Transform> children = new List<Transform>();
                foreach (Transform t in transform) children.Add(t);
                zones = children.ToArray();
            }
        }

        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        if (zones == null || zones.Length == 0 || enemyPrefab == null)
            yield break;

        currentIndex = 0;
        while (true)
        {
            Transform zone = zones[currentIndex];
            SpawnAtZone(zone);

            float delay = Random.Range(minDelay, maxDelay);
            yield return new WaitForSeconds(delay);

            currentIndex = (currentIndex + 1) % zones.Length;
        }
    }

    void SpawnAtZone(Transform zone)
    {
        if (zone == null) return;
        Vector3 spawnPos = zone.position;
        spawnPos.y += 0.5f; // spawn slightly above the plane
        Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
    }
}
