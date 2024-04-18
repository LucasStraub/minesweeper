using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class ClipInstance : MonoBehaviour
{
    public static Action<ClipInstance> OnParticleStopped;

    [SerializeField] private AudioSource _audioSource;

    public void Play()
    {
        if (_audioSource != null)
        {
            _audioSource.Play();
            StartCoroutine(PlayCo());
        }
    }

    private IEnumerator PlayCo()
    {
        while (_audioSource.isPlaying)
        {
            yield return null;
        }
        OnParticleStopped?.Invoke(this);
        SetActive(false);
    }

    public void SetActive(bool value)
    {
        gameObject.SetActive(value);
    }

    public void SetAudioClip(AudioClip clip)
    {
        if (_audioSource != null)
            _audioSource.clip = clip;
    }

    public void SetPitch(float pitch)
    {
        if (_audioSource != null)
            _audioSource.pitch = pitch;
    }
}
