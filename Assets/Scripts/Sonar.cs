using UnityEngine;

public class Sonar : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip pingSound; // 핑 소리 파일 넣기

    [Header("Sonar Speed")]
    [Tooltip("소리가 울리는 간격 (초 단위). 작을수록 빨라짐")]
    public float pingInterval = 2.0f;

    private float timer = 0f;

    void Update()
    {
        // 시간 잰다
        timer += Time.deltaTime;

        // 시간이 간격보다 커지면? 소리 재생
        if (timer >= pingInterval)
        {
            PlayPing();
            timer = 0f; // 타이머 초기화
        }
    }

    void PlayPing()
    {
        if (pingSound != null && audioSource != null)
        {
            // PlayOneShot을 써야 소리가 겹쳐도 끊기지 않고 자연스럽게 울림
            audioSource.PlayOneShot(pingSound);
        }
    }
}