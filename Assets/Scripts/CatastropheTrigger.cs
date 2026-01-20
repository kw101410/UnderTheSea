using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CatastropheTrigger : MonoBehaviour
{
    [Header("--- [Target Objects] ---")]
    public Image brokenGlassImage;
    public GameObject alarmLight;
    public ParticleSystem waterLeak;
    public Transform playerCamera;

    [Header("--- [Monster Settings] ---")]
    public GameObject monster;
    public Animator monsterAnim;
    public Transform monsterSpawnPoint;

    [Header("--- [Audio] ---")]
    public AudioSource sfxSource;
    public AudioSource alarmSource;
    public AudioClip impactSound;
    public AudioClip monsterScream;

    [Header("--- [Settings] ---")]
    public float impactShake = 1.0f;
    public float monsterJumpSpeed = 3.0f;

    [Header("--- [★ 각도 교정 (중요) ★] ---")]
    [Tooltip("괴물이 옆을 보면 90, -90 넣어보셈")]
    public float rotationOffset = 0f;

    private bool hasTriggered = false;
    private Vector3 originalCamPos;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true;
            originalCamPos = playerCamera.localPosition;
            StartCoroutine(StartCatastrophe());
        }
    }

    // ★ 유니티 LookAt 대신 쓰는 "기울기 방지" 함수
    void LookAtPlayerOnlyY(Transform obj, Vector3 targetPos)
    {
        // 1. 방향 계산
        Vector3 dir = targetPos - obj.position;

        // 2. Y축(높이) 차이 제거 -> 무조건 수평으로만 보게 함 (X, Z 회전 방지)
        dir.y = 0;

        // 3. 회전값 계산
        if (dir != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir);
            // 4. 오프셋 적용 (옆으로 서면 이걸로 돌림)
            obj.rotation = targetRot * Quaternion.Euler(0, rotationOffset, 0);
        }
    }

    IEnumerator StartCatastrophe()
    {
        // 1. 충돌 & 흔들림
        if (sfxSource) sfxSource.PlayOneShot(impactSound);
        if (brokenGlassImage) brokenGlassImage.gameObject.SetActive(true);

        float crashTime = 0f;
        while (crashTime < 0.5f)
        {
            playerCamera.localPosition = originalCamPos + Random.insideUnitSphere * impactShake;
            crashTime += Time.deltaTime;
            yield return null;
        }
        playerCamera.localPosition = originalCamPos;

        // 2. 경고등 & 물
        if (alarmLight) alarmLight.SetActive(true);
        if (waterLeak) waterLeak.Play();
        if (alarmSource) alarmSource.Play();

        // 3. 괴물 등장
        yield return new WaitForSeconds(0.2f);

        monster.transform.position = monsterSpawnPoint.position;
        monster.SetActive(true);

        // [등장 시] 기울기 없이 Y축만 돌려서 쳐다보기
        LookAtPlayerOnlyY(monster.transform, playerCamera.position);

        if (sfxSource && monsterScream) sfxSource.PlayOneShot(monsterScream);

        // 4. 돌진 (플레이어 코앞까지만)
        float attackTime = 0f;
        Vector3 startPos = monster.transform.position;

        // 목표: 카메라 보는 방향 앞 1.2m, 높이 살짝 아래
        Vector3 endPos = playerCamera.position + (playerCamera.forward * 8f);
        endPos.y -= 2f;

        while (attackTime < 1.0f)
        {
            attackTime += Time.deltaTime * monsterJumpSpeed;

            // 이동
            monster.transform.position = Vector3.Lerp(startPos, endPos, attackTime);

            // [이동 중] 매 프레임 기울기 없이 쳐다보기
            LookAtPlayerOnlyY(monster.transform, playerCamera.position);

            yield return null;
        }

        Debug.Log("연출 끝");
    }
}