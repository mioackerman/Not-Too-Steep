using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [Tooltip("Index along the route, starting from 0.")]
    public int index;

    private bool _visited = false;

    private void OnTriggerEnter(Collider other)
    {
        // Only react to the player
        if (!other.CompareTag("Player")) return;
        if (_visited) return;

        _visited = true;

        // Notify RaceManager that this checkpoint was reached
        if (RaceManager.Instance != null)
        {
            RaceManager.Instance.OnCheckpoint(index);
        }

        // Optionally hide or disable the checkpoint once visited
        var renderer = GetComponent<MeshRenderer>();
        if (renderer != null) renderer.enabled = false;

        var col = GetComponent<Collider>();
        if (col != null) col.enabled = false;
    }
}
