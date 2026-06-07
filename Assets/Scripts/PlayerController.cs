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
    float gravity, addedGravity;
    Rigidbody rb;
    Vector3 angularVelocity;
    Vector3 moveDirection;
    [Space(15)]
    [Header("Camera")]
    public bool isAiming;
    public Transform camOrientation; // Grab orientation for rotation
    [SerializeField] CinemachineRotationComposer rotationComposer;
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

        angularVelocity = new Vector3(0f, 100f, 0f);

        gravity = Physics.gravity.y;
        addedGravity = Physics.gravity.y * 4.2f;

        rotationComposer.Composition.DeadZone.Enabled = true;
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

        if (rb.useGravity && rb.linearVelocity.y < 0f)
        {
            if (Physics.gravity.y != addedGravity) Physics.gravity = new Vector3(0f, addedGravity, 0f);
        }

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
            if (!rotationComposer.Composition.DeadZone.Enabled) rotationComposer.Composition.DeadZone.Enabled = true;
            float angleDiff = Vector3.SignedAngle(transform.forward, moveDirection, Vector3.up);
            rb.angularVelocity = new Vector3(rb.angularVelocity.x, angleDiff * 0.15f, rb.angularVelocity.z);
        }
        else if (isAiming)
        {
            if (rotationComposer.Composition.DeadZone.Enabled) rotationComposer.Composition.DeadZone.Enabled = false;

            Quaternion deltaRotation = Quaternion.Euler(angularVelocity * Time.fixedDeltaTime);
            Quaternion targetRotation = Quaternion.Euler(transform.localEulerAngles.x, Camera.main.transform.localEulerAngles.y, transform.localEulerAngles.z);

            if (Quaternion.Angle(rb.rotation, targetRotation) > 0.1f)
            {
                rb.MoveRotation(targetRotation * deltaRotation);
            }

            Debug.Log(rb.angularVelocity);
        }
        else
        {
            if (!rotationComposer.Composition.DeadZone.Enabled) rotationComposer.Composition.DeadZone.Enabled = true;

            rb.angularVelocity = Vector3.zero;
        }
    }

    void GroundCheck()
    {
        // Ground check 
        grounded = Physics.BoxCast(transform.position, transform.localScale * 0.25f, Vector3.down, out floorHit, transform.rotation, 1.05f, isGround);
        if (grounded && Physics.gravity.y != gravity) Physics.gravity = new Vector3(0f, gravity, 0f);
        //Debug.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y - 1.2f, transform.position.z), Color.magenta);
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

    public void TakeDamage()
    {
        Debug.Log("YOU GOT HIT");
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
