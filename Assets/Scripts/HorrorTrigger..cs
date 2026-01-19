using UnityEngine;
using System.Collections;

public class HorrorTrigger : MonoBehaviour
{
    [Header("Settings")]
    public Sonar sonarSystem;   // 아까 만든 소나 스크립트 연결
    public GameObject monster;       // 괴물 오브젝트
    public Transform startPoint;     // 시작 위치
    public Transform endPoint;       // 끝 위치

    [Header("Values")]
    public float monsterSpeed = 15f; // 괴물 속도 (빠를수록 무서움)
    public float scaredPingInterval = 0.2f; // 빨라질 소나 간격

    private bool hasTriggered = false; // 한 번만 발동하게

    void OnTriggerEnter(Collider other)
    {
        // 플레이어가 밟았고, 아직 발동 안 했으면
        if (other.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true;
            StartCoroutine(PlayHorrorSequence());
        }
    }

    IEnumerator PlayHorrorSequence()
    {
        // 1. 소나 소리 미친 듯이 가속
        float originalInterval = sonarSystem.pingInterval; // 원래 속도 기억
        sonarSystem.pingInterval = scaredPingInterval;

        // 2. 괴물 소환 및 배치
        monster.transform.position = startPoint.position;
        monster.transform.LookAt(endPoint); // 도착점 바라보기
        monster.SetActive(true);

        // 3. 괴물 이동 (Start -> End)
        while (Vector3.Distance(monster.transform.position, endPoint.position) > 0.5f)
        {
            // MoveTowards로 강제 이동 시킴
            monster.transform.position = Vector3.MoveTowards(
                monster.transform.position,
                endPoint.position,
                monsterSpeed * Time.deltaTime
            );
            yield return null; // 다음 프레임까지 대기
        }

        // 4. 상황 종료
        monster.SetActive(false); // 괴물 삭제(숨김)

        // (선택) 소나 속도 복구? 아니면 계속 공포 유지?
        sonarSystem.pingInterval = originalInterval; // 복구하려면 주석 해제
    }
}