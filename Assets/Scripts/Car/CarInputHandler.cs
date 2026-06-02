using UnityEngine;

public class CarInputHandler : MonoBehaviour
{
    public Vector2 MoveInput { get; private set; }
    public bool IsBraking { get; private set; }

    public float HorizontalInput => MoveInput.x;
    public float VerticalInput => MoveInput.y;

    private InputSystem_Actions inputActions;

    private void Awake()
    {
        inputActions = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        if (inputActions != null)
        {
            inputActions.Enable();
        }
    }

    private void OnDisable()
    {
        if (inputActions != null)
        {
            inputActions.Disable();
        }
    }

    private void Update()
    {
        if (inputActions != null)
        {
            // Read the 2D Vector for movement (X is steering, Y is gas/reverse)
            MoveInput = inputActions.Player.Move.ReadValue<Vector2>();

            // Read the jump button as the brake input (Spacebar by default)
            IsBraking = inputActions.Player.Jump.IsPressed();
        }
    }
}
