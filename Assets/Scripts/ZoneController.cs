using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq.Expressions;

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
        if (zones.Length == 0)
        {
                List<Transform> children = new List<Transform>();
                foreach (Transform t in transform) children.Add(t);
                zones = children.ToArray();
        }
        StartCoroutine(ProgressZone());
    }

    IEnumerator ProgressZone()
    {
        if (zones.Length == 0 || enemyPrefab == null)
            yield break;

        currentIndex = 0;
        while (true)
        {
            Transform zone = zones[currentIndex];
            SpawnAtZone(zone);

            float delay = UnityEngine.Random.Range(minDelay, maxDelay);
            yield return new WaitForSeconds(delay);
            currentIndex = (currentIndex + 1) % zones.Length;

            if(currentIndex == zones.Length - 1)
            {
                Debug.Log("Completed all zones. Stopping Move.");
                yield break;
             }
    }
    }
    void SpawnAtZone(Transform zone)
    {
        if (zone == null) return;
        var spawnPos = new Vector3(zone.position.x, zone.position.y, UnityEngine.Random.Range(zone.position.z - 30f, zone.position.z -1f)   );
        spawnPos.y += 0.5f; 
        enemyPrefab.transform.position = spawnPos;
    }
}
