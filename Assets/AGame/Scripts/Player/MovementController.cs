using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MovementController : MonoBehaviour
{
    [SerializeField] public Animator anim;
    NavMeshAgent agent;
    public float charSpeed = 5f;
    public LayerMask groundLayer;
    public bool isMoving;
    public Vector3 movement;

    // Start is called before the first frame update
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = charSpeed;
        isMoving = false;
        anim.SetBool("isMoving", false);

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }

    // Update is called once per frame
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
    }

    // Object movement with W, A, S, D
    void InputMovement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 movement = new Vector3(horizontal, 0, vertical).normalized;

        isMoving = (horizontal != 0 || vertical != 0);

        anim.SetBool("isMoving", isMoving);

        if (isMoving)
        {
            agent.ResetPath();
            agent.Move(movement * charSpeed * Time.deltaTime);

            if (movement != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(movement);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 360 * Time.deltaTime);
            }
        }
    }

    // Object movement with left-mouse click
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
        anim.SetBool("isMoving", false);
    }
}
