using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 10f; // New variable for jump strength
    
    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck; // Drag a small, empty child object here
    [SerializeField] private LayerMask groundLayer;  // Set this to your ground layer in Inspector
    private bool isGrounded;
    private const float groundCheckRadius = 0.1f; // Radius for the ground check

    private Rigidbody2D player;
    private InputAction moveAction;
    private InputAction jumpAction; // New action variable for jumping
    private Vector2 currentInputVector; 

    void Start()
    {
        player = GetComponent<Rigidbody2D>();
        
        // Find Movement Action
        moveAction = InputSystem.actions.FindAction("Move");
        
        // Find Jump Action (You must define a "Jump" action in your Input Action Asset)
        jumpAction = InputSystem.actions.FindAction("Jump");

        // Set up the Jump action to trigger the Jump() method when pressed
        if (jumpAction != null)
        {
            jumpAction.performed += context => TryJump();
            jumpAction.Enable(); // Must be enabled to listen for input
        }
    }

    void OnDestroy()
    {
        // Clean up the event subscription when the object is destroyed
        if (jumpAction != null)
        {
            jumpAction.performed -= context => TryJump();
            jumpAction.Disable();
        }
    }

    void Update()
    {
        // Read the movement input for horizontal movement
        currentInputVector = moveAction.ReadValue<Vector2>();

        // Check if the player is currently touching the ground
        CheckGroundStatus();
    }

    void FixedUpdate()
    {
        // Apply horizontal movement based on input
        float targetXVelocity = currentInputVector.x * moveSpeed;
        float currentYVelocity = player.linearVelocity.y;
        
        Vector2 targetVelocity = new Vector2(targetXVelocity, currentYVelocity);
        player.linearVelocity = targetVelocity;
    }

    // --- New Jumping Methods ---

    private void CheckGroundStatus()
    {
        // Uses Physics2D to draw a small circle at the 'groundCheck' position
        // If the circle overlaps anything on the 'groundLayer', the player is grounded.
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void TryJump()
    {
        // Only allow a jump if the player is currently grounded
        if (isGrounded)
        {
            // Set the vertical velocity instantly to the jump force
            player.linearVelocity = new Vector2(player.linearVelocity.x, jumpForce);
        }
    }
}