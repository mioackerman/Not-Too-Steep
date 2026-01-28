using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ExitTrigger : MonoBehaviour
{
    private void Reset()
    {
        // Ensure this collider is a trigger
        var col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Log absolutely everything that touches this trigger
        Debug.Log("[ExitTrigger] Entered by: " + other.name + " | tag = " + other.tag);

        // If you want to be super sure, temporarily comment out the tag check:
        if (!other.CompareTag("Player"))
        {
            Debug.Log("[ExitTrigger] Not the player, ignoring.");
            return;
        }

        if (GameManager.Instance == null)
        {
            Debug.Log("[ExitTrigger] GameManager.Instance is NULL!");
            return;
        }

        Debug.Log("[ExitTrigger] Player reached exit, calling GameManager.Finish()");
        GameManager.Instance.Finish();
    }
}
