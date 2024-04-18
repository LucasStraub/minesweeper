using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tile = TileController.TileType;

public class AudioController : MonoBehaviour
{
    [SerializeField] ClipInstance _clipPrefab;
    private readonly HashSet<ClipInstance> _clips = new();

    [SerializeField] private AudioClip _openAudioClip;
    [SerializeField] private AudioClip _flagAudioClip;
    [SerializeField] private AudioClip _bombAudioClip;
    [SerializeField] private AudioClip _uiAudioClip;

    private float _lastClipSpawned;

    private void OnEnable()
    {
        ClipInstance.OnParticleStopped += ParticleSystemStopped;
    }

    private void OnDisable()
    {
        ClipInstance.OnParticleStopped -= ParticleSystemStopped;
    }

    private void ParticleSystemStopped(ClipInstance clip)
    {
        _clips.Add(clip);
    }

    public void SpawnAudio(Tile tile)
    {
        if (Time.realtimeSinceStartup - _lastClipSpawned < 0.05f)
        {
            return;
        }
        _lastClipSpawned = Time.realtimeSinceStartup;
        ClipInstance clip = null;

        foreach (var item in _clips)
        {
            clip = item;
            _clips.Remove(clip);
            break;
        }

        if (clip == null)
            clip = Instantiate(_clipPrefab, transform);

        var audioClip = tile switch
        {
            Tile.Open => _openAudioClip,
            Tile.Flag => _flagAudioClip,
            Tile.Bomb => _bombAudioClip,
            Tile.Border => _uiAudioClip,
            _ => null
        };

        clip.SetAudioClip(audioClip);
        clip.SetActive(true);
        clip.SetPitch(Random.Range(0.8f, 1.2f));
        clip.Play();
    }
}
