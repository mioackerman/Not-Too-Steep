using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    // A set storing all key IDs the player has collected
    private HashSet<string> keys = new HashSet<string>();

    // Called when the player picks up a key
    public void AddKey(string keyId)
    {
        keys.Add(keyId);
        Debug.Log("Collected key: " + keyId);
    }

    // Used by doors to check whether the player can open them
    public bool HasKey(string keyId)
    {
        return keys.Contains(keyId);
    }
}
