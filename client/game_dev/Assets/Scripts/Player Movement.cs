using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Animator animator;

    // 마지막 이동 방향
    public Vector2 LastMoveDirection { get; private set; } = Vector2.down;
    [HideInInspector] public bool canMove = true;

    public bool IsMoving => moveInput.sqrMagnitude > 0.01f; // tutorialmananger에서 사용자 움직임 감지

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        if (!canMove || PauseController.IsGamePaused)
        {
            return;
        }

        rb.linearVelocity = moveInput * moveSpeed;
    }

    private bool IsTyping()
    {
        if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject != null)
        {
            if (EventSystem.current.currentSelectedGameObject.GetComponent<TMP_InputField>() != null)
            {
                return true; // 현재 입력 필드에 포커스가 있음
            }
        }
        return false;

    }

    public void Move(InputAction.CallbackContext context)
    {
        // 입력은 항상 먼저 읽는다
        moveInput = context.ReadValue<Vector2>();

        // pause 상태면 입력값 초기화
        if (!canMove || PauseController.IsGamePaused)
        {
            moveInput = Vector2.zero;

            rb.linearVelocity = Vector2.zero;

            animator.SetBool("isWalking", false);

            return;
        }

        // TMP_InputField 입력 중이면 이동 금지
        if (IsTyping())
        {
            moveInput = Vector2.zero;

            rb.linearVelocity = Vector2.zero;

            animator.SetBool("isWalking", false);

            return;
        }

        bool isMoving = moveInput.sqrMagnitude > 0.01f;

        animator.SetBool("isWalking", isMoving);

        if (isMoving)
        {
            animator.SetFloat("LastInputX", animator.GetFloat("InputX"));
            animator.SetFloat("LastInputY", animator.GetFloat("InputY"));
        }

        animator.SetFloat("InputX", moveInput.x);
        animator.SetFloat("InputY", moveInput.y);
    }
    // 충돌 시 방향 유지 멈춤
    public void StopPlayer()
    {
        rb.linearVelocity = Vector2.zero; // linearVelocity → velocity로 변경 권장
        moveInput = Vector2.zero;
        animator.SetBool("isWalking", false);

        // 마지막 이동 방향으로 idle 유지
        animator.SetFloat("LastInputX", animator.GetFloat("InputX"));
        animator.SetFloat("LastInputY", animator.GetFloat("InputY"));
    }
}