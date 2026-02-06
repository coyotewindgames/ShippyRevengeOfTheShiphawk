using UnityEngine;

public class TrackPackedItems : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private int amountpacked = 0;
    private BoxCollider bc;
    void Start()
    {
        bc = GetComponent<BoxCollider>();
        if (bc == null)
        {
            Debug.LogWarning("TrackPackedItems: No Rigidbody component found on " + gameObject.name);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("canPickUp"))
        {
            amountpacked++;
            if (amountpacked >= 3)
            {
                Debug.Log("Game Over You Win Congrats!");
            }
        }
    }



}
