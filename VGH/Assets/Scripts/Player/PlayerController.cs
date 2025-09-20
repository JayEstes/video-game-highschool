using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    private CharacterController characterController;
    private PlayerControls controls;

    [Header("Movement Config")]
    [SerializeField] private float movementSpeed = 50.0f;
    [SerializeField] private float accelerationDampening = 10.0f;
    [SerializeField] private float motionDampening = 15.0f;

    private Vector3 motion = new Vector3();
    private Vector3 acceleration = new Vector3();
    private float gravity = 0.0f;
    // Maximum fall speed
    private float terminalGravity = -20.0f;
    private bool isOnGround = false;

    public bool IsOnGround
    {
        get
        {
            return isOnGround;
        }
    }

    private void Awake()
    {
        controls = new();
    }

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        CursorLock.IsLocked = true;
    }

    private void Update()
    {
        isOnGround = false;
        MotionInput();
        ApplyMotion();
        ApplyGravity();
    }

    private void OnEnable()
    {
        controls.Player.Enable(); 
    }

    private void OnDisable()
    {
        controls.Player.Disable();
    }
    
    private void MotionInput()
    {
        var movementVector2D = controls.Player.Move.ReadValue<Vector2>();
        var movementVector3D = new Vector3(movementVector2D.x, 0.0f, movementVector2D.y);
        movementVector3D *= movementSpeed;
        acceleration += movementVector3D * Time.deltaTime;
    }

    private void ApplyMotion()
    {
        motion += acceleration * Time.deltaTime;

        // Dampening
        acceleration = Vector3.Lerp(acceleration, Vector3.zero, accelerationDampening * Time.deltaTime);

        characterController.Move(transform.TransformDirection(motion));
        
        // Dampening
        motion = Vector3.Lerp(motion, Vector3.zero, motionDampening * Time.deltaTime);
    }

    private void ApplyGravity()
    {
        gravity += Physics.gravity.y * Time.deltaTime;

        if (gravity < terminalGravity)
        {
            gravity = terminalGravity;
        }

        CollisionFlags flags = characterController.Move(new Vector3(0.0f, gravity, 0.0f));
        
        if (flags.HasFlag(CollisionFlags.Below))
        {
            isOnGround = true;
            gravity = -(1e-5f);
        }
    }
}
