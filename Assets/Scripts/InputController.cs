using System;
using UnityEngine;

public class InputController : MonoBehaviour
{
    public static Action<Vector3> OnRightMouseButtonDown;
    public static Action<Vector3> OnLeftMouseButtonUp;
    public static Action<Vector3> OnLeftMouseButton;

    public Camera _camera;

    private void Awake()
    {
        _camera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            OnLeftMouseButton?.Invoke(GetMouseWorldPosition());
        }

        if (Input.GetMouseButtonUp(0))
        {
            OnLeftMouseButtonUp?.Invoke(GetMouseWorldPosition());
        }

        if (Input.GetMouseButtonDown(1))
        {
            OnRightMouseButtonDown?.Invoke(GetMouseWorldPosition());
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        return _camera.ScreenToWorldPoint(Input.mousePosition);
    }
}
