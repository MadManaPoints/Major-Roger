using UnityEngine;
using Unity.Cinemachine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;
    public bool playerControl = true;
    [Header("Movement")]
    [SerializeField] float moveSpeed;
    [SerializeField] float rotSpeed;
    [SerializeField] float airMult;
    [SerializeField] float jumpForce;
    public bool canJump;
    bool jumpRequested;
    Rigidbody rb;
    Vector3 angularVelocity;
    Vector3 moveDirection;
    [Space(15)]
    [Header("Camera")]
    public bool isAiming;
    public Transform camOrientation; // Grab orientation for rotation
    public Transform aimCamOrientation; // ^Same for when aiming
    CinemachineCamera aimCam;
    float yRotation, xRotation;
    [Header("Ground Check")]
    [SerializeField] LayerMask isGround;
    [SerializeField] bool grounded = true;
    RaycastHit floorHit;
    [Header("Slope Handling")]
    [SerializeField] float maxSlopeAngle;
    RaycastHit slopeHit;
    bool exitingSlope;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(Instance);
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        rb = GetComponent<Rigidbody>();
        aimCam = GetComponentInChildren<CinemachineCamera>();

        angularVelocity = new Vector3(0f, 1f, 0f);
    }

    public void PlayerMove(float verticalInput, float horizontalInput)
    {
        // Get forward position from camera Y rotation
        Transform orientation = Camera.main.transform;
        orientation.localEulerAngles = new Vector3(0f, orientation.localEulerAngles.y, 0f);

        // Move in direction of camera's flat orientation
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
    }

    public void PlayerRotate()
    {

    }

    public void PlayerAim()
    {
        isAiming = !isAiming;
    }

    void FixedUpdate()
    {
        Movement();
        Rotation();
    }

    void Update()
    {
        GroundCheck();
        //Debug.Log(isAiming);
        if (Input.GetKeyDown(KeyCode.T)) isAiming = !isAiming;
    }

    void Movement()
    {
        // Create move Vector from player inputs on X and Z axis
        Vector3 move = new Vector3(moveDirection.x * moveSpeed, rb.linearVelocity.y, moveDirection.z * moveSpeed);
        ///Debug.Log(move);

        // Turn off gravity on slope
        if (rb.useGravity && OnSlope()) rb.useGravity = false;
        else if (!rb.useGravity && !OnSlope()) rb.useGravity = true;

        Vector3 v = grounded ? move : new Vector3(move.x * airMult, rb.linearVelocity.y, move.z * airMult);
        rb.AddForce(v - rb.linearVelocity, ForceMode.VelocityChange);

        if (jumpRequested)
        {
            jumpRequested = false;
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void Rotation()
    {
        if (moveDirection != Vector3.zero && !isAiming)
        {
            if (aimCam.Priority != 0) aimCam.Priority = 0;

            float angleDiff = Vector3.SignedAngle(transform.forward, moveDirection, Vector3.up);
            rb.angularVelocity = new Vector3(rb.angularVelocity.x, angleDiff * 0.15f, rb.angularVelocity.z);
        }
        else if (isAiming)
        {
            Quaternion deltaRotation = Quaternion.Euler(angularVelocity * Time.fixedDeltaTime);
            Quaternion targetRotation = Quaternion.Euler(transform.localEulerAngles.x, Camera.main.transform.localEulerAngles.y, transform.localEulerAngles.z);
            rb.MoveRotation(targetRotation * deltaRotation);

            if (Quaternion.Angle(rb.rotation, targetRotation) <= 0.1f)
            {
                if (aimCam.Priority != 2) aimCam.Priority = 2;
            }
        }
        else
        {
            if (aimCam.Priority != 0) aimCam.Priority = 0;
            rb.angularVelocity = Vector3.zero;
        }
    }

    void GroundCheck()
    {
        // Ground check 
        grounded = Physics.BoxCast(transform.position, transform.localScale * 0.25f, Vector3.down, out floorHit, transform.rotation, 1.2f, isGround);
        Debug.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y - 1.2f, transform.position.z), Color.magenta);
    }

    public void Jump()
    {
        //rb.linearVelocity = Vector3.zero;
        if (grounded)
        {
            canJump = false;
            jumpRequested = true;
            Invoke(nameof(ResetJump), 0.3f);
        }
    }

    void ResetJump()
    {
        if (exitingSlope) exitingSlope = false;
        canJump = true;
    }

    bool OnSlope()
    {
        //Physics.Raycast(new Vector3(transform.position.x, transform.position.y, transform.position.z), Vector3.down, out slopeHit, 1.2f)
        if (Physics.BoxCast(transform.position, transform.localScale * 0.25f, Vector3.down, out slopeHit, transform.rotation, 1.2f, isGround))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal); // Calculate slope steepness

            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }
}
