using UnityEngine;

public class SimpleFish : MonoBehaviour
{
    [Header("Fish Settings")]
    public float moveSpeed = 3f;
    public float turnSpeed = 2f;
    public float obstacleRange = 5f;

    private Vector3 targetDirection;

    void Start()
    {
        PickNewDirection();
    }

    void Update()
    {
        // 1. 앞으로 전진
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);

        // 2. 장애물 감지
        if (Physics.Raycast(transform.position, transform.forward, obstacleRange))
        {
            PickNewDirection();
        }

        // 3. 회전 (부드럽게)
        // 방향 벡터가 0이면 에러 나니까 안전장치 추가
        if (targetDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        }
    }

    void PickNewDirection()
    {
        // [수정됨] 원래 구(Sphere) 모양으로 뽑던 걸
        targetDirection = Random.onUnitSphere;

        // ★ 핵심: Y축(위아래)을 0으로 죽여버림 ★
        targetDirection.y = 0;

        // Y를 0으로 만들면 벡터 길이가 줄어들어서 속도가 느려질 수 있음.
        // 다시 길이를 1로 맞춰줌 (Normalize)
        targetDirection.Normalize();
    }
}