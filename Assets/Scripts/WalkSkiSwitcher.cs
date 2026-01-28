using UnityEngine;
using StarterAssets;

[RequireComponent(typeof(CharacterController))]
public class WalkSkiSwitcher : MonoBehaviour
{
    [Header("References")]
    public ThirdPersonController thirdPersonController;   // Walking controller from Starter Assets
    public Animator animator;                             // Player animator (on Player object)
    public Transform cameraRoot;                          // Camera pivot

    [Header("Ski Settings")]
    public float skiMaxSpeed = 30f;       // Maximum ski speed
    public float skiAcceleration = 10f;   // Acceleration when moving forward (on ground)
    public float skiFriction = 3f;        // Base friction applied every frame
    public float brakeDecel = 20f;        // Extra deceleration when braking (S key)
    public float turnSpeed = 120f;        // Horizontal turning speed using A/D
    public LayerMask groundLayers = ~0;   // Layers considered as ground

    [Header("Air / Lip Settings")]
    public float slopeAngleChangeThreshold = 20f; // Degrees of slope change to trigger airtime
    public float hopUpVelocity = 0.1f;            // Very small hop when slope gets much steeper
    public float lipOffUpVelocity = 0.0f;         // Small lift when going off a lip / to flatter ground
    public float groundCheckDistance = 1.5f;      // Distance used to detect landing

    [Header("Camera Look (ski mode only)")]
    public float lookSensitivityX = 200f;
    public float lookSensitivityY = 120f;
    public float minPitch = -40f;
    public float maxPitch = 70f;

    [Header("Ski Visuals")]
    public GameObject[] skiAndPoleObjects;   // Skis and poles to show only while skiing


    private CharacterController controller;
    private bool isSkiing = false;
    private bool isBraking = false;       // Triggers braking animation

    private float skiSpeed = 0f;          // Scalar horizontal speed along slope/forward
    private float verticalVelocity = 0f;  // Y-velocity used during airtime
    private bool isAirborne = false;      // True when we are in the air

    private float yaw;                    // Camera yaw (around Y)
    private float pitch;                  // Camera pitch (around X)

    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    void Start()
    {
        if (cameraRoot != null)
        {
            Vector3 angles = cameraRoot.localEulerAngles;
            yaw = angles.y;
            pitch = angles.x;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            isSkiing = !isSkiing;

            if (thirdPersonController != null)
                thirdPersonController.enabled = !isSkiing;

            animator.SetBool("IsSkiing", isSkiing);

            UpdateSkiVisuals();   // Toggle skis and poles

            if (!isSkiing)
            {
                // Reset ski state when returning to walk mode
                skiSpeed = 0f;
                verticalVelocity = 0f;
                isAirborne = false;
                animator.SetBool("IsBraking", false);
                animator.SetFloat("Speed", 0f);
            }
        }


        if (isSkiing)
        {
            HandleCameraLook();          // Mouse rotates camera ONLY in ski mode
            HandleSkiInputAndMovement();
        }
    }

    private void HandleCameraLook()
    {
        if (cameraRoot == null) return;

        float mouseX = Input.GetAxis("Mouse X") * lookSensitivityX * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * lookSensitivityY * Time.deltaTime;

        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        cameraRoot.localRotation = Quaternion.Euler(pitch, yaw, 0f);
    }

    private void HandleSkiInputAndMovement()
    {
        // --- TURNING (A / D), only rotates the character, not the camera ---
        float turnInput = Input.GetAxis("Horizontal"); // A = -1, D = +1
        if (Mathf.Abs(turnInput) > 0.01f)
        {
            float yawDelta = turnInput * turnSpeed * Time.deltaTime;
            transform.Rotate(0f, yawDelta, 0f);
        }

        // --- BRAKING (S key) ---
        isBraking = Input.GetKey(KeyCode.S);
        animator.SetBool("IsBraking", isBraking);

        // Base friction / braking, applied every frame
        float totalFriction = skiFriction + (isBraking ? brakeDecel : 0f);
        skiSpeed -= totalFriction * Time.deltaTime;
        skiSpeed = Mathf.Max(0f, skiSpeed); // do not go negative

        // If already airborne, handle air movement and exit
        if (isAirborne)
        {
            HandleAirborneMovement();
            return;
        }

        Vector3 move = Vector3.zero;

        // Raycast straight down from current position to find current slope
        if (Physics.Raycast(
            transform.position + Vector3.up * 0.3f,
            Vector3.down,
            out RaycastHit hit,
            3f,
            groundLayers))
        {
            // Direction of the ski on the slope (player forward projected onto slope)
            Vector3 forwardOnSlope = Vector3.ProjectOnPlane(transform.forward, hit.normal).normalized;

            // Pure downhill direction from gravity projected onto the slope
            Vector3 downhillDir = Vector3.ProjectOnPlane(Vector3.down, hit.normal).normalized;

            // Alignment between where player is pointing and true downhill direction:
            //  1   = fully downhill (max acceleration)
            //  0   = across the slope (no acceleration)
            // -1   = straight uphill (gravity slows you)
            float alignment = Vector3.Dot(forwardOnSlope, downhillDir) + 0.3f;

            // --- PLAYER FORWARD BOOST (W key) ---
            float forwardBoost = Input.GetKey(KeyCode.W) ? 0.7f : 0f;

            // --- COMBINED ACCELERATION ---
            float accel = skiAcceleration * alignment + forwardBoost;
            skiSpeed += accel * Time.deltaTime;
            skiSpeed = Mathf.Clamp(skiSpeed, 0f, skiMaxSpeed);


            // --- LOOK AHEAD TO CHECK UPCOMING SLOPE ---
            Vector3 forwardStart = transform.position + Vector3.up * 0.3f + transform.forward * 1.0f;

            if (Physics.Raycast(
                forwardStart,
                Vector3.down,
                out RaycastHit forwardHit,
                3f,
                groundLayers))
            {
                float currentSlopeAngle = Vector3.Angle(hit.normal, Vector3.up);
                float nextSlopeAngle = Vector3.Angle(forwardHit.normal, Vector3.up);
                float angleDelta = Mathf.Abs(nextSlopeAngle - currentSlopeAngle);

                // Only trigger hop / lip when speed is high enough
                if (skiSpeed >= 10f && angleDelta > slopeAngleChangeThreshold)
                {
                    if (nextSlopeAngle > currentSlopeAngle)
                    {
                        // Gets steeper ahead
                        verticalVelocity = hopUpVelocity;
                        isAirborne = true;
                    }
                    else
                    {
                        // Flatter or lip ahead
                        verticalVelocity = lipOffUpVelocity;
                        isAirborne = true;
                    }
                }
            }
            else
            {
                // No ground ahead (cliff)
                if (skiSpeed >= 10f)
                {
                    isAirborne = true;
                    verticalVelocity = lipOffUpVelocity;
                }
            }

            // If we just entered airborne state
            if (isAirborne)
            {
                HandleAirborneMovement();
                return;
            }

            // Ground movement
            move = forwardOnSlope * skiSpeed;
            move += Physics.gravity;

            controller.Move(move * Time.deltaTime);
            animator.SetFloat("Speed", skiSpeed);
        }
        else
        {
            // No ground under feet
            if (skiSpeed >= 10f)
            {
                isAirborne = true;
                HandleAirborneMovement();
            }
            else
            {
                // Low speed: just apply gravity
                Vector3 fallMove = Physics.gravity * Time.deltaTime;
                controller.Move(fallMove);
            }
        }
    }


    private void UpdateSkiVisuals()
    {
        if (skiAndPoleObjects == null) return;

        bool show = isSkiing;

        for (int i = 0; i < skiAndPoleObjects.Length; i++)
        {
            if (skiAndPoleObjects[i] != null)
            {
                skiAndPoleObjects[i].SetActive(show);
            }
        }
    }



    private void HandleAirborneMovement()
    {
        // Horizontal direction in air: keep using current facing direction
        Vector3 horizontalDir = transform.forward;
        horizontalDir.y = 0f;
        if (horizontalDir.sqrMagnitude > 0.0001f)
        {
            horizontalDir.Normalize();
        }

        // Integrate gravity on vertical velocity
        verticalVelocity += Physics.gravity.y * Time.deltaTime;

        Vector3 move = horizontalDir * skiSpeed;
        move.y = verticalVelocity;

        controller.Move(move * Time.deltaTime);

        // Animator speed based on horizontal velocity
        Vector3 horizontalVel = move;
        horizontalVel.y = 0f;
        animator.SetFloat("Speed", horizontalVel.magnitude);

        // Check for landing
        if (Physics.Raycast(
                transform.position + Vector3.up * 0.3f,
                Vector3.down,
                out RaycastHit hit,
                groundCheckDistance,
                groundLayers) &&
            verticalVelocity <= 0f)
        {
            isAirborne = false;
            verticalVelocity = 0f;

            // Small snap down to ensure grounded contact
            controller.Move(Vector3.down * 0.1f);
        }
    }
}
