using UnityEngine;

public class TrackPackedItems : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private int amountpacked = 0;
    private BoxCollider bc;
    void Start()
    {
        // Try to find BoxCollider on this GameObject first
        bc = GetComponent<BoxCollider>();
        
        // If not found, try to find it in children
        if (bc == null)
        {
            bc = GetComponentInChildren<BoxCollider>();
        }
        
        // If still not found, try to find it in parent
        if (bc == null)
        {
            bc = GetComponentInParent<BoxCollider>();
        }
        
        if (bc == null)
        {
            Debug.LogError("TrackPackedItems: No BoxCollider component found on " + gameObject.name + " or its children/parent");
            
            // Debug: List all components on this GameObject
            Component[] components = GetComponents<Component>();
            Debug.Log("Components on " + gameObject.name + ":");
            foreach (Component comp in components)
            {
                Debug.Log("- " + comp.GetType().Name);
            }
        }
        else
        {
            Debug.Log("TrackPackedItems: BoxCollider found on " + gameObject.name);
            
            // Make sure it's set as a trigger for OnTriggerEnter to work
            if (!bc.isTrigger)
            {
                Debug.LogWarning("TrackPackedItems: BoxCollider is not set as Trigger - setting it now");
                bc.isTrigger = true;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Total packs " + amountpacked+ " (Tag: " + other.gameObject.tag + ")");
        if (other.CompareTag("canPickUp"))
        {
            amountpacked++;
            if (amountpacked >= 4)
            {
                GameManager.Instance.SetGameWin(true);
            }
        }
    }



}
