using System;
using UnityEngine;

public class ParticleInstance : MonoBehaviour
{
    public static Action<ParticleInstance> OnParticleStopped;

    [SerializeField] private ParticleSystem _particleSystem;
    [SerializeField] private ParticleSystemRenderer _particleSystemRenderer;

    public void OnParticleSystemStopped()
    {
        gameObject.SetActive(false);
        OnParticleStopped?.Invoke(this);
    }

    public void SetMaterial(Material material)
    {
        if (_particleSystemRenderer != null)
            _particleSystemRenderer.sharedMaterial = material;
    }

    public void SetActive(bool value)
    {
        gameObject.SetActive(value);
    }

    public void Play()
    {
        if (_particleSystem != null)
            _particleSystem.Play();
    }
}
