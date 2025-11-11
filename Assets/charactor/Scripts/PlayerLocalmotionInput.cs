using CharactorController;
using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-2)]
public class PlayerLocalmotionInput : MonoBehaviour, PlayerControll.IPlayerLocalMotionMapActions
{
    public PlayerControll PlayerControls { get; private set; }
    public Vector2 MovementInput { get; private set; }
    public Vector2 LookInput { get; private set; }

    public bool IsSprinting { get; private set; }
    public bool ThrowModifierHeld { get; private set; }
    public bool ThrowPressed { get; set; } // ✅ Make setter public

    private void OnEnable()
    {
        PlayerControls = new PlayerControll();
        PlayerControls.Enable();

        PlayerControls.PlayerLocalMotionMap.Enable();
        PlayerControls.PlayerLocalMotionMap.SetCallbacks(this);
    }

    private void OnDisable()
    {
        PlayerControls.PlayerLocalMotionMap.Disable();
        PlayerControls.PlayerLocalMotionMap.RemoveCallbacks(this);
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        IsSprinting = context.ReadValueAsButton();
    }

    public void OnMovement(InputAction.CallbackContext context)
    {
        MovementInput = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        LookInput = context.ReadValue<Vector2>();
    }

    public void OnThrowModifier(InputAction.CallbackContext context)
    {
        ThrowModifierHeld = context.ReadValueAsButton();
    }

    public void OnThrow(InputAction.CallbackContext context)
    {
        if (context.performed)
            ThrowPressed = true;
        else if (context.canceled)
            ThrowPressed = false;
    }

}
