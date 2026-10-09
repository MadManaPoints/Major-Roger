using UnityEngine;
using Unity.Cinemachine;
using System.Collections.Generic;

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
    bool jumpRequested, waitForStrafeJump;
    Vector3 strafeJumpDirection = Vector3.zero;
    float gravity, addedGravity;
    float strafeMovementAirDamp = 1f;
    Rigidbody rb;
    Vector3 angularVelocity;
    Vector3 moveDirection;
    [Space(15)]
    [Header("Camera")]
    public bool isAiming;
    public Transform camOrientation; // Grab orientation for rotation
    [SerializeField] CinemachineRotationComposer rotationComposer;
    [SerializeField] CinemachineOrbitalFollow orbitalFollow;
    [SerializeField] CinemachineBasicMultiChannelPerlin noise;
    [SerializeField] CinemachineInputAxisController cinemachineController;
    [SerializeField] CinemachineCamera cinemachineCam;
    float yRotation, xRotation;
    [Header("Ground Check")]
    [SerializeField] LayerMask isGround;
    [SerializeField] bool grounded = true;
    RaycastHit floorHit;
    [Header("Slope Handling")]
    [SerializeField] float maxSlopeAngle;
    RaycastHit slopeHit;
    bool exitingSlope;
    [Space(15)]
    [Header("Other References")]
    [SerializeField] PlayerHealth health;
    public bool canTakeDamage = true;
    [Space(15)]
    [Header("Targeting")]
    [SerializeField] Transform lockOnTarget;
    [SerializeField] Transform aimTarget;
    [SerializeField] Transform opp;
    public bool lockedOn;
    bool shooting;
    PlayerShoot gun;

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
        health = GetComponent<PlayerHealth>();
        gun = GetComponent<PlayerShoot>();


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

        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
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

        if (shooting)
        {
            gun.Fire();
        }
    }

    void Movement()
    {
        // Create move Vector from player inputs on X and Z axis
        moveDirection.Normalize();
        Vector3 move = new Vector3(moveDirection.x * moveSpeed, rb.linearVelocity.y, moveDirection.z * moveSpeed);
        ///Debug.Log(move);

        // Turn off gravity on slope
        if (rb.useGravity && OnSlope()) rb.useGravity = false;
        else if (!rb.useGravity && !OnSlope()) rb.useGravity = true;

        if (rb.useGravity && rb.linearVelocity.y < 0f)
        {
            if (Physics.gravity.y != addedGravity) Physics.gravity = new Vector3(0f, addedGravity, 0f);
        }

        Vector3 v = grounded && !isAiming ? move : new Vector3(move.x * airMult, rb.linearVelocity.y, move.z * airMult);
        if (!waitForStrafeJump) rb.AddForce(v - rb.linearVelocity, ForceMode.VelocityChange);

        if (jumpRequested)
        {
            jumpRequested = false;
            Debug.Log(strafeJumpDirection);
            rb.AddForce((Vector3.up + strafeJumpDirection) * jumpForce, ForceMode.Impulse);
        }
    }

    void Rotation()
    {
        if (moveDirection != Vector3.zero && !isAiming && !lockedOn)
        {
            //if (cinemachineCam.LookAt != aimTarget) cinemachineCam.LookAt = aimTarget; // Reset aim target
            //if (!cinemachineController.enabled) cinemachineController.enabled = true; // Reset controller input
            if (!rotationComposer.enabled) rotationComposer.enabled = true; // Reset rotation 
            if (!rotationComposer.Composition.DeadZone.Enabled) rotationComposer.Composition.DeadZone.Enabled = true;

            // Get rotation based on movement
            float angleDiff = Vector3.SignedAngle(transform.forward, moveDirection, Vector3.up);
            rb.angularVelocity = new Vector3(rb.angularVelocity.x, angleDiff * 0.15f, rb.angularVelocity.z);
        }
        else if (lockedOn)
        {
            //if (cinemachineController.enabled) cinemachineController.enabled = false; // Prevent controller input for camera
            //if (cinemachineCam.LookAt != lockOnTarget) cinemachineCam.LookAt = lockOnTarget; // Apply lock-on based on group cam

            if (isAiming)
            {
                // Get rotation based on lock-on target
                Vector3 direction = opp.position - transform.position; // Get target direction
                Quaternion targetRotation = Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.z));
                Quaternion deltaRotation = Quaternion.Euler(angularVelocity * Time.fixedDeltaTime);

                if (Quaternion.Angle(rb.rotation, targetRotation) > 0.1f)
                {
                    rb.MoveRotation(targetRotation * deltaRotation); // Rotate toward target
                }
            }
            else
            {
                // Get rotation based on movement
                float angleDiff = Vector3.SignedAngle(transform.forward, moveDirection, Vector3.up);
                rb.angularVelocity = new Vector3(rb.angularVelocity.x, angleDiff * 0.15f, rb.angularVelocity.z);
            }
            //if (rotationComposer.enabled) rotationComposer.enabled = false;
            //if (orbitalFollow.enabled) orbitalFollow.enabled = false;
        }
        else if (isAiming && !lockedOn)
        {
            //if (cinemachineCam.LookAt != aimTarget) cinemachineCam.LookAt = aimTarget; // Reset aim target
            //if (!cinemachineController.enabled) cinemachineController.enabled = true; // Reset controller input

            if (rotationComposer.Composition.DeadZone.Enabled) rotationComposer.Composition.DeadZone.Enabled = false;

            Quaternion deltaRotation = Quaternion.Euler(angularVelocity * Time.fixedDeltaTime);
            Quaternion targetRotation = Quaternion.Euler(transform.localEulerAngles.x, Camera.main.transform.localEulerAngles.y, transform.localEulerAngles.z);

            if (Quaternion.Angle(rb.rotation, targetRotation) > 0.1f)
            {
                rb.MoveRotation(targetRotation * deltaRotation); // Rotate toward camera direction
            }

            //Debug.Log(rb.angularVelocity);
        }
        else
        {
            //if (cinemachineCam.LookAt != aimTarget) cinemachineCam.LookAt = aimTarget; // Reset aim target
            //if (!cinemachineController.enabled) cinemachineController.enabled = true; // Reset controller input
            if (!rotationComposer.enabled) rotationComposer.enabled = true; // Reset rotation
            if (!rotationComposer.Composition.DeadZone.Enabled) rotationComposer.Composition.DeadZone.Enabled = true;

            rb.angularVelocity = Vector3.zero;
        }
    }

    public void Shoot()
    {
        shooting = !shooting;
    }

    void GroundCheck()
    {
        // Ground check 
        grounded = Physics.BoxCast(transform.position, transform.localScale * 0.25f, Vector3.down, out floorHit, transform.rotation, 1.05f, isGround);
        if (grounded && Physics.gravity.y != gravity) Physics.gravity = new Vector3(0f, gravity, 0f);
        if (grounded && waitForStrafeJump) waitForStrafeJump = false;
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

    public void StrafeJump(Vector3 directionToJump)
    {
        if (grounded)
        {
            canJump = false;
            strafeJumpDirection = directionToJump;
            jumpRequested = true;
            Invoke(nameof(ResetJump), 0.2f);
        }
    }

    public void TakeDamage(int amount)
    {
        Debug.Log("YOU GOT HIT");
        canTakeDamage = false;
        noise.enabled = true;
        Invoke(nameof(NoiseReset), 0.2f);
        Invoke(nameof(DamageReset), 1f);
        health.TakeDamage(amount);
    }

    void ResetJump()
    {
        if (exitingSlope) exitingSlope = false;
        if (strafeJumpDirection != Vector3.zero) strafeJumpDirection = Vector3.zero;
        waitForStrafeJump = true;
        canJump = true;
    }

    void NoiseReset()
    {
        noise.enabled = false;
    }

    void DamageReset()
    {
        canTakeDamage = true;
    }

    void StrafeReset()
    {

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
