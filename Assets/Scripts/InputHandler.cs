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
        Vector2 movement = _move.ReadValue<Vector2>();
        player.PlayerMove(movement.y, movement.x);

        Vector2 look = _look.ReadValue<Vector2>();
        if (camManager.shoulderCam.Priority > 2)
            camManager.aimCam.AimCameraControls(look);
        else
            camManager.aimCam.FollowCameraControls();

        if (_aim.WasPressedThisFrame() || _aim.WasReleasedThisFrame())
        {
            player.isAiming = !player.isAiming;

            if (player.isAiming)
            {
                camManager.shoulderCam.Priority = 3;
                camManager.followCam.Priority = 1;
            }
            else
            {
                camManager.followCam.Priority = 3;
                camManager.shoulderCam.Priority = 1;
            }
        }

        if (_shoot.WasPressedThisFrame() || _shoot.WasReleasedThisFrame() || Input.GetKeyDown(KeyCode.K) || Input.GetKeyUp(KeyCode.K))
        {
            player.Shoot();
        }

        if (_lockOn.WasPressedThisFrame())
        {
            player.lockedOn = !player.lockedOn;

            camManager.targetLockCam.Priority = player.lockedOn ? 5 : 1;

        }

        if ((_ATB.WasPressedThisFrame() && !menu.menuOpen) || (_cancel.WasPressedThisFrame() && menu.menuOpen))
        {
            menu.ToggleMenu();
        }

        if (_jump.WasPressedThisFrame() && player.canJump)
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
                    float angle = Mathf.Atan2(movement.x, movement.y) * Mathf.Rad2Deg;
                    if (angle < 0f) angle += 360f;

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

                    player.StrafeJump(directionToJump);
                }
            }
        }
    }
}
