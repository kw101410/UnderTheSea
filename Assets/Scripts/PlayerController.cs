using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float swimSpeed = 15f;      // 수영 속도
    public float mouseSensitivity = 2f; // 마우스 감도

    private Rigidbody rb;
    private float verticalRotation = 0f;
    private Camera playerCamera;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerCamera = GetComponentInChildren<Camera>(); // 자식에 있는 카메라 찾기

        // 마우스 커서 숨기고 고정 (ESC 누르면 풀리게 따로 짜야 함)
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // 1. 마우스 회전 (시선 처리)
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // 좌우 회전 (몸통 돌리기)
        transform.Rotate(Vector3.up * mouseX);

        // 위아래 회전 (카메라만 끄덕끄덕)
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f); // 목 꺾임 방지
        playerCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
    }

    void FixedUpdate()
    {
        // 2. 이동 (물리 힘 적용)
        float moveX = Input.GetAxis("Horizontal"); // A, D
        float moveZ = Input.GetAxis("Vertical");   // W, S

        // 카메라가 보는 방향 기준으로 이동 벡터 계산
        // (보는 방향으로 힘을 가함 = 수영하는 느낌)
        Vector3 moveDirection = playerCamera.transform.forward * moveZ + playerCamera.transform.right * moveX;

        // 힘 적용 (ForceMode.Acceleration이 부드러움)
        rb.AddForce(moveDirection.normalized * swimSpeed, ForceMode.Acceleration);
    }
}