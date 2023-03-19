using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameScreen : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerLabel;

    private void Update()
    {
        if (GameController.Instance.isPlaying)
        {
            TimeSpan time = TimeSpan.FromSeconds(GameController.Instance.gameTimer);
            timerLabel.text = time.ToString(@"mm\:ss\.") + "<size=70%>" + time.ToString(@"ff") + "</size>";
        }
    }


}