using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float jumpHeight = 2f; // 점프 높이
    public float gravity = -9.81f; // 중력

    public Animator animator; // 애니메이터를 위한 변수

    private InputSystem_Actions inputSystemActions; // 클래스명 수정
    private CharacterController controller;
    private Vector2 moveInput;
    private bool isRunning;
    private bool isJumping;
    private float ySpeed;

    private void Awake()
    {
        inputSystemActions = new InputSystem_Actions();
        controller = GetComponent<CharacterController>();
    }

    private void OnEnable()
    {
        inputSystemActions.Enable();
        inputSystemActions.Player.Sprint.performed += OnRunPerformed;
        inputSystemActions.Player.Sprint.canceled += OnRunCanceled;
        inputSystemActions.Player.Jump.performed += OnJumpPerformed; // 점프 액션
    }

    private void OnDisable()
    {
        inputSystemActions.Player.Sprint.performed -= OnRunPerformed;
        inputSystemActions.Player.Sprint.canceled -= OnRunCanceled;
        inputSystemActions.Player.Jump.performed -= OnJumpPerformed; // 점프 액션
        inputSystemActions.Disable();
    }

    private void Update()
    {
        // 입력 업데이트
        moveInput = inputSystemActions.Player.Move.ReadValue<Vector2>();

        // 점프 중에는 ySpeed를 사용하여 중력을 적용
        if (controller.isGrounded)
        {
            ySpeed = -0.1f; // 미세한 값으로 항상 캐릭터가 바닥에 닿아 있도록 함
        }
        else
        {
            ySpeed += gravity * Time.deltaTime; // 중력 적용
        }

        // 점프를 위한 입력 처리
        if (isJumping && controller.isGrounded)
        {
            ySpeed = Mathf.Sqrt(jumpHeight * -2f * gravity); // 점프 힘 계산
            isJumping = false;
        }

        // 플레이어 이동
        float currentSpeed = isRunning ? runSpeed : walkSpeed;
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        Vector3 velocity = move * currentSpeed + Vector3.up * ySpeed;
        controller.Move(velocity * Time.deltaTime);

        // 애니메이션 설정
        bool isMoving = moveInput != Vector2.zero;
        animator.SetBool("isWalking", isMoving && !isRunning && !isJumping);
        animator.SetBool("isRunning", isMoving && isRunning && !isJumping);
        animator.SetBool("isJumping", isJumping);
    }

    private void OnRunPerformed(InputAction.CallbackContext context)
    {
        isRunning = true;
    }

    private void OnRunCanceled(InputAction.CallbackContext context)
    {
        isRunning = false;
    }

    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        // 점프 액션이 수행될 때
        if (controller.isGrounded)
        {
            isJumping = true;
        }
    }
}
