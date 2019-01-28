using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

    private AudioSource currentSource;

    public AudioClip mainMenuSound;

    private bool transition;

    void Start()
    {
        currentSource = GetComponent<AudioSource>();
        currentSource.volume = .1f; 
    }

    public void StartMenuLoop()
    {

        currentSource.clip = mainMenuSound;

        currentSource.Play();
    }

    public IEnumerator StopSound(bool fadeOut)
    {
        if (currentSource != null)
        {
            float startVolume = currentSource.volume;

            while (currentSource.volume > 0)
            {
                currentSource.volume -= startVolume * Time.deltaTime / 0.5f;
                yield return null;
            }

            currentSource.Stop();
            currentSource.volume = startVolume;
        }
    }

    void Update()
    {
        if (currentSource == null) return;

        if (!currentSource.isPlaying && !transition)
        {
            transition = true;
            currentSource.PlayScheduled(AudioSettings.dspTime + 2f);
        }
        else if (currentSource.isPlaying && transition) transition = false;
    }


}
