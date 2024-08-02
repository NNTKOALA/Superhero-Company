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
    public LayerMask groundLayer;
    public bool isMoving;
    public bool isRunning;
    public Vector3 movement;
    public GameObject charLogo;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = charMoveSpeed;
        isMoving = false;
        isRunning = false;
        anim.SetBool("isMoving", false);
        anim.SetBool("isRunning", false);
        GameObject logo = Instantiate(charLogo, transform);
        logo.transform.localPosition = new Vector3(0, 1.8f, 0);
        logo.AddComponent<FaceCamera>();
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
        if (InputManager.Instance.IsMoveToMouse())
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

    void HandleSprint()
    {
        if (InputManager.Instance.IsRunning() && isMoving == true)
        {
            isRunning = true;
            agent.speed = charRunSpeed;
            anim.SetBool("isRunning", isRunning);
        }
        else
        {
            isRunning = false;
            agent.speed = charMoveSpeed;
            anim.SetBool("isRunning", isRunning);
        }
    }
}