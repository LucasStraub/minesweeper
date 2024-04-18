using System.Collections.Generic;
using UnityEngine;
using Tile = TileController.TileType;

public class ParticleController : MonoBehaviour
{
    [SerializeField] ParticleInstance _particlePrefab;
    private readonly HashSet<ParticleInstance> _particles = new();

    [SerializeField] Material _coveredMaterial;
    [SerializeField] Material _flagMaterial;

    private void OnEnable()
    {
        ParticleInstance.OnParticleStopped += ParticleSystemStopped;
    }

    private void OnDisable()
    {
        ParticleInstance.OnParticleStopped -= ParticleSystemStopped;
    }

    private void ParticleSystemStopped(ParticleInstance particle)
    {
        _particles.Add(particle);
    }

    public void SpawnParticle(Vector3 position, Tile tile)
    {
        ParticleInstance particle = null;

        foreach (var p in _particles)
        {
            particle = p;
            _particles.Remove(particle);
            break;
        }

        if (particle == null)
            particle = Instantiate(_particlePrefab, transform);

        var material = tile switch
        {
            Tile.Covered => _coveredMaterial,
            Tile.Flag => _flagMaterial,
            _ => null
        };

        particle.SetMaterial(material);
        particle.transform.position = position;
        particle.SetActive(true);
        particle.Play();

    }
}
