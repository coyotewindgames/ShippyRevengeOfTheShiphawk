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
        while (currentIndex < zones.Length)
        {
            Transform zone = zones[currentIndex];
            SpawnAtZone(zone);

            if (currentIndex == zones.Length - 1)
            {
                OnFinalZoneReached();
                yield break;
            }

            float delay = UnityEngine.Random.Range(minDelay, maxDelay);
            yield return new WaitForSeconds(delay);
            currentIndex++;
        }
    }

    private void OnFinalZoneReached()
    {
        Debug.Log("Completed all zones. Stopping Move.");
        if (enemyPrefab != null)
        {
            var enemy = enemyPrefab.GetComponent<EnemyController>();
            if (enemy != null)
                enemy.EnterFinalChaseMode();
        }
    }

    void SpawnAtZone(Transform zone)
    {
        if (zone == null) return;
        var spawnPos = new Vector3(zone.position.x, zone.position.y, UnityEngine.Random.Range(zone.position.z - 30f, zone.position.z -1f)   );
        spawnPos.y += 0.5f; 
        enemyPrefab.transform.position = spawnPos;
    }
    
    public int getCurrentZone()
    {
        if (zones.Length == 0)
            return -1;

        Transform zone = zones[currentIndex];
        return currentIndex;
    }
    public void setCurrentZone(int index)
    {
        if (zones.Length == 0 || index < 0 || index >= zones.Length)
            return;

        currentIndex = index;
        Transform zone = zones[currentIndex];
    }
}
