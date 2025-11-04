using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections; // Required for Coroutines

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 15f;
    [SerializeField] private float jumpForce = 10f; 
    
    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck; 
    [SerializeField] private LayerMask groundLayer;  

    [SerializeField] private Animator _animator;

    [Header("PowerUp")]
    [SerializeField] private float pushBoostSpeed = 20f;
    [SerializeField] private float pushDuration = 3.0f;
    [SerializeField] private float doublePushBoostSpeed = 25f; 
    [SerializeField] private float doublePushDuration = 2.0f; 

    private bool isGrounded;
    private const float groundCheckRadius = 0.1f; 

    private Rigidbody2D player;
    private InputAction moveAction;
    private InputAction jumpAction; 
    private Vector2 currentInputVector; 

    void Start()
    {
        player = GetComponent<Rigidbody2D>();
        
        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");

        if (jumpAction != null)
        {
            jumpAction.performed += context => TryJump();
            jumpAction.Enable(); 
        }
    }

    void OnDestroy()
    {
        if (jumpAction != null)
        {
            jumpAction.performed -= context => TryJump();
            jumpAction.Disable();
        }
    }

    void Update()
    {
        currentInputVector = moveAction.ReadValue<Vector2>();
        CheckGroundStatus();
    }

    void FixedUpdate()
    {
        bool isMoving = Mathf.Abs(currentInputVector.x) > 0.01f;
        _animator.SetBool("isGrounded", isGrounded);
        
        if (isMoving) 
        {
            _animator.SetBool("isRolling", true);
        }
        else 
        {
            _animator.SetBool("isRolling", false);
        }
        
        // Apply horizontal movement based on input
        float targetXVelocity = currentInputVector.x * moveSpeed;
        float currentYVelocity = player.linearVelocity.y;
        
        Vector2 targetVelocity = new Vector2(targetXVelocity, currentYVelocity);
        player.linearVelocity = targetVelocity;
    }

    private void CheckGroundStatus()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void TryJump()
    {
        if (isGrounded)
        {
            player.linearVelocity = new Vector2(player.linearVelocity.x, jumpForce);
            _animator.SetTrigger("Jump");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PowerUp"))
        {
            StopAllCoroutines();
            StartCoroutine(PowerUpEffect(pushBoostSpeed, pushDuration, "PushSingle"));
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("PowerUp2"))
        {
            StopAllCoroutines(); 
            StartCoroutine(PowerUpEffect(doublePushBoostSpeed, doublePushDuration, "PushDouble"));
            Destroy(other.gameObject);
        }
    }

    IEnumerator PowerUpEffect(float boostSpeed, float duration, string animTriggerName)
    {
        if (_animator != null)
        {
            _animator.SetTrigger(animTriggerName);
        }

        float originalSpeed = moveSpeed;
        moveSpeed = boostSpeed; 

        yield return new WaitForSeconds(duration); 

        moveSpeed = originalSpeed; 
    }
}