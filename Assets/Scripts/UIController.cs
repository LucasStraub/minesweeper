using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public static Action OnButtonClick;
    public static Action<int> OnDropDownValueChanged;

    [Header("UI")]
    [SerializeField] private RectTransform _canvas;

    [SerializeField] private TextMeshProUGUI _flagsTextUI;

    [SerializeField] private Button _startButton;
    [SerializeField] private Sprite[] _startButtonSprites;

    [SerializeField] private TextMeshProUGUI _timerUI;

    [SerializeField] private TMP_Dropdown _difficultyDropdown;

    private void OnEnable()
    {
        if (_startButton != null)
            _startButton.onClick.AddListener(ButtonClicked);
        if (_difficultyDropdown != null)
            _difficultyDropdown.onValueChanged.AddListener(DropdownValueChanged);
    }

    private void OnDisable()
    {
        if (_startButton != null)
            _startButton.onClick.RemoveListener(ButtonClicked);
        if (_difficultyDropdown != null)
            _difficultyDropdown.onValueChanged.RemoveListener(DropdownValueChanged);
    }

    private void DropdownValueChanged(int value)
    {
        OnDropDownValueChanged?.Invoke(value);
    }

    private void ButtonClicked()
    {
        OnButtonClick?.Invoke();
    }


    public void SetTimer(float timer)
    {
        if (_timerUI != null)
            _timerUI.text = ((int)Mathf.Floor(timer)).ToString("D3");
    }

    public void SetFlags(int flags)
    {
        if (_flagsTextUI != null)
            _flagsTextUI.text = flags.ToString("D3");
    }

    public void SetCanvasSize(Vector2Int size)
    {
        if (_canvas != null)
        {
            _canvas.sizeDelta = (size + new Vector2Int(2, 4)) * 16;
            _canvas.position = new Vector3(size.x % 2, size.y % 2) / 2;
        }
    }

    public void SetDropdownOptions(List<string> options)
    {
        if (_difficultyDropdown != null)
        {
            _difficultyDropdown.ClearOptions();
            _difficultyDropdown.AddOptions(options);
        }
    }

    public void SetButtonSprite(int value)
    {
        if (_startButton != null && _startButtonSprites != null && _startButtonSprites.Length > value)
            _startButton.image.sprite = _startButtonSprites[value];
    }
}
