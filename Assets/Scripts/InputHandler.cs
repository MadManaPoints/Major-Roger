using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class InputHandler : MonoBehaviour
{
    PlayerController player;
    public InputAction _move, _look, _aim, _jump;

    void Awake()
    {
        _move = InputSystem.actions.FindAction("Move");
        _look = InputSystem.actions.FindAction("Look");
        _aim = InputSystem.actions.FindAction("Aim");
        _jump = InputSystem.actions.FindAction("Jump");
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
            player.PlayerAim();
        }

        if (_jump.WasPressedThisFrame() && player.canJump)
        {
            player.Jump();
        }
    }
}
