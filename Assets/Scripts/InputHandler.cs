using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class InputHandler : MonoBehaviour
{
    PlayerController player;
    CameraManager camManager;
    AttackMenu menu;
    public InputAction _move, _look, _aim, _jump, _lockOn, _cancel, _ATB, _menuUp, _menuDown, _shoot;

    void Awake()
    {
        _move = InputSystem.actions.FindAction("Move");
        _look = InputSystem.actions.FindAction("Look");
        _aim = InputSystem.actions.FindAction("Aim");
        _jump = InputSystem.actions.FindAction("Jump");
        _lockOn = InputSystem.actions.FindAction("LockOn");
        _cancel = InputSystem.actions.FindAction("Cancel");
        _ATB = InputSystem.actions.FindAction("ATB");
        _menuUp = InputSystem.actions.FindAction("MenuUp");
        _menuDown = InputSystem.actions.FindAction("MenuDown");
        _shoot = InputSystem.actions.FindAction("Shoot");
    }

    void Start()
    {
        player = PlayerController.Instance;
        camManager = CameraManager.Instance;
        menu = AttackMenu.Instance;
    }

    void Update()
    {
        // Move player based on directional inputs
        Vector2 movement = _move.ReadValue<Vector2>();
        player.PlayerMove(movement.y, movement.x);

        // Rotate camera
        Vector2 look = _look.ReadValue<Vector2>();
        if (camManager.shoulderCam.Priority > 2)
            camManager.aimCam.AimCameraControls(look); // Based on analog/mouse movement
        else
            camManager.aimCam.FollowCameraControls(); // Follows main camera when inactive

        // Menu selection inputs
        if (_menuDown.WasPressedThisFrame())
        {
            menu.UpdateSelection(1); // Move up
        }
        else if (_menuUp.WasPressedThisFrame())
        {
            menu.UpdateSelection(-1); // Move down
        }

        // Check if left trigger is pressed or released
        if (_aim.WasPressedThisFrame() || _aim.WasReleasedThisFrame())
        {
            player.isAiming = !player.isAiming; // Toggle aim

            // Set camera priority based on aim mode
            if (player.isAiming)
            {
                // Over-the-shoulder cam
                camManager.shoulderCam.Priority = 3;
                camManager.followCam.Priority = 1;
            }
            else
            {
                // Orbital cam
                camManager.followCam.Priority = 3;
                camManager.shoulderCam.Priority = 1;
            }
        }

        // Check if right trigger is pressed or released
        if (_shoot.WasPressedThisFrame() || _shoot.WasReleasedThisFrame())
        {
            player.Shoot(); // Alert player 
        }

        // Toggle lock-on with left shoulder button
        if (_lockOn.WasPressedThisFrame())
        {
            player.lockedOn = !player.lockedOn;

            // Set camera priority based on lock mode
            camManager.targetLockCam.Priority = player.lockedOn ? 5 : 1;

        }

        // Toggle abilities menu
        if ((_ATB.WasPressedThisFrame() && !menu.menuOpen) || (_cancel.WasPressedThisFrame() && menu.menuOpen))
        {
            menu.ToggleMenu();
        }

        // Listen for jump 
        if (_jump.WasPressedThisFrame())
        {
            // If inside menu, confirm menu selection instead of triggering jump
            if (menu.menuOpen)
            {
                menu.ConfirmSelection();
            }
            else
            {
                // If not in strafe mode, start regular jump
                if (!player.isAiming || (player.isAiming && movement.magnitude < 0.4f))
                {
                    player.Jump();
                }
                else
                {
                    // Get direction for strafe dump
                    if (movement.magnitude >= 0.4f)
                    {
                        Vector3 directionToJump;

                        // Convert right stick angle from radians to degrees
                        float angle = Mathf.Atan2(movement.x, movement.y) * Mathf.Rad2Deg;
                        if (angle < 0f) angle += 360f; // If degrees go negative, shift 360 degrees to get correct angle

                        // Update strafe jump direction based on angle
                        if (angle >= 315f || angle <= 45f)
                        {
                            directionToJump = player.transform.forward;
                        }
                        else if (angle >= 225f)
                        {
                            directionToJump = -player.transform.right;
                        }
                        else if (angle >= 135f)
                        {
                            directionToJump = -player.transform.forward;
                        }
                        else
                        {
                            directionToJump = player.transform.right;
                        }

                        // Make player jump
                        player.StrafeJump(directionToJump);
                    }
                }
            }
        }
    }
}
