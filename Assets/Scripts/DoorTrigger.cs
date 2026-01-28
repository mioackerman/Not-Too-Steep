using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DoorTrigger : MonoBehaviour
{
    public Door door;
    public string requiredKeyId = "KeyA";

    private void Reset()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        PlayerInventory inventory = other.GetComponent<PlayerInventory>();
        if (inventory == null)
            return;

        if (inventory.HasKey(requiredKeyId))
        {
            door.TryOpen(requiredKeyId);
        }
        else
        {
            Debug.Log("Door locked. Need key: " + requiredKeyId);
        }
    }
}
