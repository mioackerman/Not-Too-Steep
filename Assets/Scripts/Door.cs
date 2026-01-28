using System.Collections;
using UnityEngine;

public class Door : MonoBehaviour
{
    public string requiredKeyId = "KeyA";

    // How high the door should move up when opening
    public float openHeight = 3f;

    // How long the opening animation should take (in seconds)
    public float openDuration = 1f;

    private bool isOpen = false;
    private bool isOpening = false;

    private Vector3 closedPosition;
    private Vector3 openPosition;

    private void Awake()
    {
        // Cache the closed and open positions
        closedPosition = transform.position;
        openPosition = closedPosition + Vector3.up * openHeight;
    }

    public void TryOpen(string keyId)
    {
        if (isOpen || isOpening)
            return;

        if (keyId != requiredKeyId)
            return;

        // Start opening animation
        StartCoroutine(OpenRoutine());
    }

    private IEnumerator OpenRoutine()
    {
        isOpening = true;

        float elapsed = 0f;

        while (elapsed < openDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / openDuration);

            // Smoothly move the door from closed to open position
            transform.position = Vector3.Lerp(closedPosition, openPosition, t);

            yield return null;
        }

        // Ensure final position is exactly the open position
        transform.position = openPosition;

        isOpen = true;
        isOpening = false;

        // Optional: disable collider after fully open so player never gets stuck
        // var col = GetComponent<Collider>();
        // if (col != null) col.enabled = false;
    }
}
