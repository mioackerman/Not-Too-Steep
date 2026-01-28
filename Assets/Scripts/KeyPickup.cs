using UnityEngine;

[RequireComponent(typeof(Collider))]
public class KeyPickup : MonoBehaviour
{
    public string keyId = "KeyA";

    private void Reset()
    {
        Collider col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[KeyPickup] Trigger entered by: {other.name}, tag = {other.tag}");

        // 1) Check tag
        bool isPlayerTag = other.CompareTag("Player");
        Debug.Log($"[KeyPickup] CompareTag(\"Player\") = {isPlayerTag}");

        // 2) Get inventory
        var inventory = other.GetComponent<PlayerInventory>();
        Debug.Log($"[KeyPickup] PlayerInventory component found = {(inventory != null)}");

        // --- TEMP: if you want to force pickup even if tag is wrong, comment out this line:
        if (!isPlayerTag || inventory == null)
        {
            Debug.Log("[KeyPickup] Conditions not met, key not picked up.");
            return;
        }

        inventory.AddKey(keyId);
        Debug.Log("[KeyPickup] Player picked up key: " + keyId);

        Destroy(gameObject);
    }
}
