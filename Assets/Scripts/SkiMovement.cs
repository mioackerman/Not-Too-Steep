using UnityEngine;

public class SkiMovement : MonoBehaviour
{
    public float maxSpeed = 25f;
    public float acceleration = 10f;
    public float turnSpeed = 80f;
    public float friction = 2f;

    private float currentSpeed = 0f;
    private Vector3 moveDir;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 2f))
        {
            Vector3 slopeDir = Vector3.ProjectOnPlane(Vector3.down, hit.normal).normalized;

            currentSpeed += acceleration * Time.fixedDeltaTime;
            currentSpeed = Mathf.Clamp(currentSpeed, 0, maxSpeed);

            moveDir = (transform.forward + slopeDir * 0.3f).normalized;

            currentSpeed -= friction * Time.fixedDeltaTime;
            currentSpeed = Mathf.Max(0, currentSpeed);

            rb.linearVelocity = moveDir * currentSpeed;
        }
    }
}
