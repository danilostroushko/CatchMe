using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverScreen : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerLabel;
    [SerializeField] private Button startButton;

    private void Start()
    {
        startButton.onClick.AddListener(OnRestartClick);
    }

    private void OnEnable()
    {
        TimeSpan time = TimeSpan.FromSeconds(GameController.Instance.gameTimer);
        timerLabel.text = time.ToString(@"mm\:ss\.") + "<size=70%>" + time.ToString(@"ff") + "</size>";
    }

    private void OnRestartClick()
    {
        GameController.Instance.RestartGame();
    }
}
