using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MusicType
{
    intro,
    game
}

public class MusicManager : MonoBehaviour
{
    public MusicType musicType;
    public float loopStart = -1;
    public float loopEnd = -1;

    AudioSource audioSource;
    float fadeOutTime = 0.5f;


    IEnumerator CoFade(float goalVolume, bool end)
    {
        float startVolume = audioSource.volume;
        float startTime = Time.time;
        float elapsedTime = 0;

        while (elapsedTime < fadeOutTime)
        {
            elapsedTime = Time.time - startTime;
            float percent = elapsedTime / fadeOutTime;
            audioSource.volume = Mathf.Lerp(startVolume, goalVolume, percent);
            yield return new WaitForSeconds(0.01f);
        }

        if (end)
        {
            audioSource.Stop();
            audioSource.volume = 1;
        }

        yield return null;
    }

    IEnumerator CoLoop()
    {
        yield return new WaitForSeconds(loopEnd);

        while (true)
        {
            audioSource.time = loopStart;
            yield return new WaitForSeconds(loopEnd - loopStart);
        }
    }

    public void StartFade(float goalVolume, bool end)
    {
        StartCoroutine(CoFade(goalVolume, end));
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (loopStart != -1 && loopEnd != -1)
        {
            StartCoroutine(CoLoop());
        }
    }

    void Update()
    {
        
    }
}
