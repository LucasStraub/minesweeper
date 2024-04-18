using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float _duration = 0.025f;
    [SerializeField] private float _force = 0.025f;

    private Vector3 _initialPos;

    private void Awake()
    {
        _initialPos = transform.position;
    }

    public void Shake()
    {
        StartCoroutine(ShakeCo(_duration, _force));
    }

    private IEnumerator ShakeCo(float time, float amount)
    {
        var timer = 0f;
        while (timer < time)
        {
            var t = Mathf.InverseLerp(0, time, timer);

            transform.localPosition += amount * t * Random.insideUnitSphere;

            yield return null;
            timer += Time.deltaTime;
        }

        transform.localPosition = _initialPos;
    }
}