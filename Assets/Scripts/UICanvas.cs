using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICanvas : MonoBehaviour
{
    public GameObject PanelWin;

    public GameObject PanelDescription;

    public Button btnKillSnake;

    private void Awake()
    {
        PanelWin.SetActive(false);

        PanelDescription.SetActive(false);

        btnKillSnake.onClick.AddListener(OnClickKillSnake);
    }

    // for testing
    private void OnClickKillSnake()
    {
        GameManager.Instance.Snakes.KillRandomSnake();
    }

    internal void ShowPanel()
    {
        PanelWin.SetActive(true);
    }
}
