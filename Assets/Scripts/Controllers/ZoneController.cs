using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq.Expressions;

public class ZoneController : MonoBehaviour
{
    public Transform[] zones;

    public GameObject enemyPrefab;

    [Tooltip("Min Spawn")]
    public float minDelay = 3f;

    [Tooltip("Max Spawn")]
    public float maxDelay = 5f;

    public static GameManager Instance;

    int currentIndex = 0;

    private PlayerController playerController;

    private AudioSource audioSource;
    public AudioClip[] carterClips;

    void Start()
    {
        if (zones.Length == 0)
        {
            List<Transform> children = new List<Transform>();
            foreach (Transform t in transform) children.Add(t);
            zones = children.ToArray();
        }
        playerController = UnityEngine.Object.FindFirstObjectByType<PlayerController>();
        audioSource = GetComponent<AudioSource>();
        StartCoroutine(ProgressZone());
        Instance = GameManager.Instance;
    }

    IEnumerator ProgressZone()
    {
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

            yield return new WaitForSeconds(10f);
            float delay = UnityEngine.Random.Range(minDelay, maxDelay);
            yield return new WaitForSeconds(delay);
            currentIndex++;
         
            AudioClip clip = carterClips[UnityEngine.Random.Range(0, carterClips.Length)];    
            audioSource.PlayOneShot(clip);
            
        }
    }

    private void OnFinalZoneReached()
    {
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
        var spawnPos = new Vector3(zone.position.x, zone.position.y, UnityEngine.Random.Range(zone.position.z  + 10f, zone.position.z - 10f));
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
