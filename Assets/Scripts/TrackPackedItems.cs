using UnityEngine;

public class TrackPackedItems : MonoBehaviour
{
    private int amountpacked = 0;
    private BoxCollider bc;
    void Start()
    {
        bc = GetComponent<BoxCollider>();
        
        
        if (bc == null)
        {
            
            Component[] components = GetComponents<Component>();
            foreach (Component comp in components)
            {
            }
        }
        else
        {
            
            if (!bc.isTrigger)
            {
                bc.isTrigger = true;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
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
