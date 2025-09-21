using System.Runtime.CompilerServices;
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

    [Header("Components")]
    [SerializeField] private Raycaster raycaster;
    [SerializeField] private Transform throwTransform;

    private Vector3 motion = new Vector3();
    private Vector3 xzAcceleration = new Vector3();
    private float yAcceleration = 0.0f;

    private float gravity = 0.0f;
    // Maximum fall speed
    private float terminalGravity = -20.0f;
    private bool isOnGround = false;

    [SerializeField]
    private Transform head;
    private ThrowableObject heldThrowable = null;

    public float fallSpeed = -3.0f;


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
        Debug.Assert(raycaster != null);
    }

    private void Update()
    {
        isOnGround = false;

        if (heldThrowable)
        {
            heldThrowable.transform.forward = head.forward;
        }

        MotionInput();
        ApplyMotion();
        ApplyGravity();
    }

    private void OnEnable()
    {
        controls.Player.Enable();
        controls.Player.Interact.started += OnInteractPerformed;
        controls.Player.Interact.canceled += OnInteractCanceled;
        controls.Player.Jump.performed += OnJumpPerformed;
        controls.Player.Throw.started += OnThrowStarted;
    }

    private void OnDisable()
    {
        controls.Player.Interact.performed -= OnInteractPerformed;
        controls.Player.Interact.canceled -= OnInteractCanceled;
        controls.Player.Jump.performed -= OnJumpPerformed;
        controls.Player.Throw.started -= OnThrowStarted;
        controls.Player.Disable();
    }

    private void MotionInput()
    {
        var movementVector2D = controls.Player.Move.ReadValue<Vector2>();
        var movementVector3D = new Vector3(movementVector2D.x, 0.0f, movementVector2D.y);
        movementVector3D *= movementSpeed;
        xzAcceleration += movementVector3D * Time.deltaTime;
    }

    private void OnThrowStarted(InputAction.CallbackContext ctx)
    {
        ThrowObject(10.0f);
    }


    private void OnJumpPerformed(InputAction.CallbackContext ctx)
    {
        Jump();
    }

    private void Jump()
    {
        if (!isOnGround) return;

        yAcceleration = 5.0f;
    }


    private void OnInteractPerformed(InputAction.CallbackContext ctx)
    {
        if (!heldThrowable)
        {
            PickupObject();
        }
    }

    private void OnInteractCanceled(InputAction.CallbackContext ctx)
    {
        if (heldThrowable)
        {
            // Release throw
            ThrowObject(0.0f);
        }
    }


    private void PickupObject()
    {
        if (heldThrowable)
        {
            return;
        }

        if (!raycaster.IsHitting)
        {
            return;
        }

        var go = raycaster.IntersectedGameObject;
        var throwableObject = go.GetComponent<ThrowableObject>();

        if (throwableObject == null)
        {
            return;
        }

        throwableObject.OnPickup(throwTransform);
        heldThrowable = throwableObject;
    }

    private void ThrowObject(float throwForce)
    {
        if (!heldThrowable)
        {
            return;
        }

        heldThrowable.OnThrow(head.transform.forward * throwForce);
        heldThrowable = null;
    }

    private void ApplyMotion()
    {
        yAcceleration += gravity * Time.deltaTime;
        motion += xzAcceleration * Time.deltaTime;
        motion.y += yAcceleration * Time.deltaTime;

        // Dampening
        xzAcceleration = Vector3.Lerp(xzAcceleration, Vector3.zero, accelerationDampening * Time.deltaTime);

        characterController.Move(transform.TransformDirection(motion));

        // Dampening
        motion = Vector3.Lerp(motion, Vector3.zero, motionDampening * Time.deltaTime);
    }

    private void ApplyGravity()
    {
        gravity += fallSpeed * Time.deltaTime;

        if (gravity < terminalGravity)
        {
            gravity = terminalGravity;
        }

        CollisionFlags flags = characterController.Move(new Vector3(0.0f, gravity, 0.0f));

        if (flags.HasFlag(CollisionFlags.Below))
        {
            isOnGround = true;
            gravity = -(1e-5f);
            yAcceleration = 0.0f;
            motion.y = 0.0f;
        }
    }
}
