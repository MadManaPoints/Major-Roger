using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class InputHandler : MonoBehaviour
{
    PlayerController player;
    public InputAction _move, _look, _aim, _jump, _lockOn;

    void Awake()
    {
        _move = InputSystem.actions.FindAction("Move");
        _look = InputSystem.actions.FindAction("Look");
        _aim = InputSystem.actions.FindAction("Aim");
        _jump = InputSystem.actions.FindAction("Jump");
        _lockOn = InputSystem.actions.FindAction("LockOn");
    }

    void Start()
    {
        player = PlayerController.Instance;
    }

    void Update()
    {

        Vector2 movement = _move.ReadValue<Vector2>();
        player.PlayerMove(movement.y, movement.x);

        Vector2 rotate = _look.ReadValue<Vector2>();
        //player.PlayerRotate(rotate);

        if (_aim.WasPressedThisFrame() || _aim.WasReleasedThisFrame())
        {
            player.isAiming = !player.isAiming;
        }

        if (_lockOn.WasPressedThisFrame())
        {
            player.lockedOn = !player.lockedOn;
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
