using UnityEngine;
using System.Collections;

public class CinematicTrigger : MonoBehaviour
{
    [Header("--- [Target] ---")]
    public Sonar sonarSystem;   // 소나 시스템
    public GameObject monster;       // 괴물
    public Transform startPoint;     // 괴물 시작 위치
    public Transform endPoint;       // 괴물 끝 위치
    public Light spotLight;          // 플레이어 조명
    public Transform playerCamera;   // 메인 카메라

    [Header("--- [Audio Clips] ---")]
    public AudioSource sfxSource;
    public AudioClip heavyBreath;    // 숨소리
    public AudioClip switchClick;    // 스위치 소리
    public AudioClip lightSlam;      // 조명 쾅!
    public AudioClip metalCreak;     // 끼기긱
    public AudioClip monsterGrowl;   // 괴성

    [Header("--- [Settings] ---")]
    public float monsterSpeed = 20f; // 괴물 속도
    public float shakeIntensity = 0.2f; // 흔들림 강도

    [Tooltip("소나 소리가 빨라지는 데 걸리는 시간 (초)")]
    public float tensionDuration = 4.0f; // 4초 동안 서서히 빨라짐

    private bool hasTriggered = false;
    private Vector3 originalCamPos;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true;
            originalCamPos = playerCamera.localPosition;
            StartCoroutine(PlaySequence());
        }
    }

    IEnumerator PlaySequence()
    {
        // 1. [빌드업] 숨소리 재생
        if (sfxSource) sfxSource.PlayOneShot(heavyBreath);

        // 현재 소나 속도(3.5)에서 목표 속도(0.2)까지 서서히 빨라짐
        float currentResponceTime = 0f;
        float startInterval = sonarSystem.pingInterval; // 3.5
        float targetInterval = 0.2f; // 0.2

        // tensionDuration(4초) 동안 루프 돔
        while (currentResponceTime < tensionDuration)
        {
            currentResponceTime += Time.deltaTime;

            // Lerp로 서서히 값 변경 (3.5 -> 3.0 -> ... -> 0.2)
            sonarSystem.pingInterval = Mathf.Lerp(startInterval, targetInterval, currentResponceTime / tensionDuration);

            // 덩달아 카메라도 조금씩 더 심하게 떨림 (점점 불안해짐)
            float currentShake = Mathf.Lerp(0f, shakeIntensity * 0.3f, currentResponceTime / tensionDuration);
            playerCamera.localPosition = originalCamPos + Random.insideUnitSphere * currentShake;

            yield return null;
        }

        // 확실하게 0.2로 고정
        sonarSystem.pingInterval = targetInterval;


        // 2. [클라이맥스] 스위치 -> 조명 쾅!
        if (sfxSource) sfxSource.PlayOneShot(switchClick);
        yield return new WaitForSeconds(0.2f); // 딸깍 후 잠깐 정적

        spotLight.gameObject.SetActive(true); // 조명 ON

        // 쾅! 끼기긱! 으아악! (동시 재생)
        if (sfxSource)
        {
            sfxSource.PlayOneShot(lightSlam);
            sfxSource.PlayOneShot(metalCreak);
            sfxSource.PlayOneShot(monsterGrowl);
        }

        // 3. [괴물 등장]
        monster.transform.position = startPoint.position;
        monster.transform.LookAt(endPoint);
        monster.SetActive(true);

        while (Vector3.Distance(monster.transform.position, endPoint.position) > 0.5f)
        {
            monster.transform.position = Vector3.MoveTowards(
                monster.transform.position,
                endPoint.position,
                monsterSpeed * Time.deltaTime
            );

            // 이때는 카메라 미친 듯이 흔들림
            playerCamera.localPosition = originalCamPos + Random.insideUnitSphere * shakeIntensity;

            yield return null;
        }

        // 4. [상황 종료]
        monster.SetActive(false);
        spotLight.gameObject.SetActive(false);
        playerCamera.localPosition = originalCamPos;

        
       sonarSystem.pingInterval = startInterval; 
    }
}