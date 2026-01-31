using UnityEngine;

public class EnemyController : MonoBehaviour
{
public ZoneController zoneController;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnScanHit()
    {
        Debug.Log("Enemy hit by scan laser!");
        if (zoneController != null)
        {
            var currentZone = zoneController.getCurrentZone();
            Debug.Log("Enemy was in zone: " + currentZone);
      
            if(currentZone > 0)
            {
                int newZone = Mathf.Max(0, currentZone - 1);
                zoneController.setCurrentZone(newZone);
            }
        }
    }
}
