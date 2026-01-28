using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    // Movement speed
    public float moveSpeed = 5f;

    // Gravity force applied to the character
    public float gravity = -9.81f;

    private CharacterController controller;
    private Vector3 velocity;

    // Optional: Animator for playing movement animations
    public Animator animator;

    void Start()
    {
        // Fetch the CharacterController component on this object
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // Get WASD / arrow key input
        float h = Input.GetAxis("Horizontal"); // Left / Right movement
        float v = Input.GetAxis("Vertical");   // Forward / Backward movement

        // Local movement vector
        Vector3 move = new Vector3(h, 0, v);

        // Convert movement to camera-relative direction
        if (Camera.main != null)
        {
            Vector3 camForward = Camera.main.transform.forward;
            Vector3 camRight = Camera.main.transform.right;

            // Ignore vertical tilt of the camera
            camForward.y = 0f;
            camRight.y = 0f;

            camForward.Normalize();
            camRight.Normalize();

            move = camForward * v + camRight * h;
        }

        // Move the character controller
        controller.Move(move * moveSpeed * Time.deltaTime);

        // Rotate character toward the movement direction
        if (move.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                Time.deltaTime * 10f
            );
        }

        // Apply gravity
        if (controller.isGrounded && velocity.y < 0)
        {
            // Small negative number keeps the controller grounded
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // Animation update (optional)
        if (animator != null)
        {
            bool isMoving = move.sqrMagnitude > 0.01f;
            animator.SetFloat("Speed", isMoving ? 1f : 0f);
        }
    }
}
