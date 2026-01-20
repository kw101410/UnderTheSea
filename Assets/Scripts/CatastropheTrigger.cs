using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CatastropheTrigger : MonoBehaviour
{
    [Header("--- [Target Objects] ---")]
    public Transform monsterRoot;   // 껍데기(ROOT)
    public GameObject monsterModel; // 괴물 모델
    public Transform playerCamera;  // 플레이어 카메라
    public Transform spawnPoint;    // 시작 위치

    [Header("--- [연출 옵션] ---")]
    public Image brokenGlass;
    public GameObject alarmLight;
    public ParticleSystem waterLeak;
    public AudioSource sfxSource;
    public AudioClip impactSound;
    public AudioClip screamSound;

    [Header("--- [★ 위치 미세조절 (여기만 봐) ★] ---")]
    [Tooltip("값이 클수록 플레이어보다 멀리(앞에) 멈춤. 뒤로 넘어가면 이 숫자를 키워! (추천: 1.5 ~ 2.0)")]
    public float stopDistance = 1.5f;

    [Tooltip("값이 클수록 괴물을 바닥으로 내림. 너무 높으면 이 숫자를 키워! (추천: 1.5 ~ 1.8)")]
    public float heightDown = 1.6f;

    [Tooltip("괴물 날아오는 속도")]
    public float speed = 3.0f;

    private bool isTriggered = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isTriggered)
        {
            isTriggered = true;
            StartCoroutine(ActionSequence());
        }
    }

    IEnumerator ActionSequence()
    {
        // 1. 파국 연출
        if (sfxSource) sfxSource.PlayOneShot(impactSound);
        if (brokenGlass) brokenGlass.gameObject.SetActive(true);
        if (alarmLight) alarmLight.SetActive(true);
        if (waterLeak) waterLeak.Play();

        yield return new WaitForSeconds(0.2f);

        // 2. 괴물 배치 (시작)
        monsterRoot.position = spawnPoint.position;
        monsterModel.SetActive(true);

        // ★ 쳐다보는 건 Y축만 (기울기 방지)
        Vector3 lookDir = playerCamera.position - monsterRoot.position;
        lookDir.y = 0;
        if (lookDir != Vector3.zero) monsterRoot.rotation = Quaternion.LookRotation(lookDir);

        if (sfxSource) sfxSource.PlayOneShot(screamSound);

        // 3. 돌진 계산
        float timer = 0f;
        Vector3 startPos = monsterRoot.position;

        while (timer < 1.0f)
        {
            timer += Time.deltaTime * speed;

            // ★ [핵심] 도착 지점 실시간 계산
            // 카메라가 보는 방향(forward) 수평으로 가져옴
            Vector3 forwardFlat = playerCamera.forward;
            forwardFlat.y = 0; // 땅 보거나 하늘 봐도 괴물 위치 안 이상해지게 수평 유지
            forwardFlat.Normalize();

            // 목표점: 카메라 위치 + (앞으로 stopDistance만큼) + (아래로 heightDown만큼)
            Vector3 endPos = playerCamera.position + (forwardFlat * stopDistance);
            endPos.y -= heightDown;

            // 이동
            monsterRoot.position = Vector3.Lerp(startPos, endPos, timer);

            // 계속 쳐다보기 (Y축만)
            lookDir = playerCamera.position - monsterRoot.position;
            lookDir.y = 0;
            if (lookDir != Vector3.zero) monsterRoot.rotation = Quaternion.LookRotation(lookDir);

            yield return null;
        }
    }
}