using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MovementController : MonoBehaviour
{
    [SerializeField] public Animator anim;
    NavMeshAgent agent;
    public float charMoveSpeed = 5f;
    public float charRunSpeed = 10f;
    public float jumpForce = 5f;
    public LayerMask groundLayer;
    public bool isMoving;
    public bool isRunning;
    public bool isGrounded;
    public Vector3 movement;
    public GameObject charLogo;
    private Rigidbody rb;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        agent.speed = charMoveSpeed;
        isMoving = false;
        isRunning = false;
        anim.SetBool("isMoving", false);
        anim.SetBool("isRunning", false);
        GameObject logo = Instantiate(charLogo, transform);
        logo.transform.localPosition = new Vector3(0, 0, 0);
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }

    void Update()
    {
        if (GameManager.Instance.chatBox.isFocused)
        {
            StopMovement();
            return;
        }
        InputMovement();
        MouseClickMovement();
        UpdateMovementState();
        HandleJump();
        HandleSprint();
    }

    void InputMovement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        movement = new Vector3(horizontal, 0, vertical).normalized;
        isMoving = (horizontal != 0 || vertical != 0);
        anim.SetBool("isMoving", isMoving);
        if (isMoving)
        {
            agent.ResetPath();
            float currentSpeed = isRunning ? charRunSpeed : charMoveSpeed;
            agent.Move(movement * currentSpeed * Time.deltaTime);
            if (movement != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(movement);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 360 * Time.deltaTime);
            }
        }
    }

    void MouseClickMovement()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, groundLayer))
            {
                agent.SetDestination(hitInfo.point);
            }
        }
    }

    void UpdateMovementState()
    {
        isMoving = isMoving || agent.velocity.magnitude > 0.1f;
        anim.SetBool("isMoving", isMoving);
    }

    void StopMovement()
    {
        agent.ResetPath();
        isMoving = false;
        isRunning = false;
        anim.SetBool("isMoving", false);
    }

    void HandleJump()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1f, groundLayer);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Space key pressed");

            if (isGrounded)
            {
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                anim.SetBool("isJumping", true);
                Debug.Log("Character is Jumping => " + Vector3.up * jumpForce);
            }
            else
            {
                Debug.Log("Character is not grounded");
            }
        }
    }

    void HandleSprint()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            isRunning = true;
            agent.speed = charRunSpeed;
            anim.SetBool("isRunning", isRunning);
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            isRunning = false;
            agent.speed = charMoveSpeed;
            anim.SetBool("isRunning", isRunning);
        }
    }
}