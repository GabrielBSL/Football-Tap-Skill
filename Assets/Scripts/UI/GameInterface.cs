using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameInterface : MonoBehaviour
{
    [Header("Main")]
    [SerializeField] private Image bgImage;
    [SerializeField] private CanvasScaler mainCanvas;
    [SerializeField] private GameObject pauseCanvas;
    [SerializeField] private GameObject presentationCanvas;
    [SerializeField] private GameObject versusPanel;
    [SerializeField] private TextMeshProUGUI finalClassificationText;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider musicSlider;

    [Header("HUD")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI lifeText;

    [Header("Panels")]
    [SerializeField] private RectTransform pausePanel;
    [SerializeField] private RectTransform exitPanel;
    [SerializeField] private RectTransform configPanel;
    [SerializeField] private RectTransform victoryPanel;
    [SerializeField] private RectTransform losePanel;

    private Stack<RectTransform> panelSequence = new Stack<RectTransform>();

    private float screenWidth;
    private float screenHeight;

    private float bgImageAlphaInitial;
    private CanvasGroup currentCanvasGroup;

    private void Awake()
    {
        versusPanel.SetActive(true);
        presentationCanvas.SetActive(true);

        bgImageAlphaInitial = bgImage.color.a;

        screenWidth = mainCanvas.referenceResolution.x * 3 / 2;
        screenHeight = mainCanvas.referenceResolution.y * 3 / 2;

        pausePanel.anchoredPosition = new Vector3(pausePanel.anchoredPosition.x, pausePanel.anchoredPosition.y - screenHeight);
        exitPanel.anchoredPosition = new Vector3(exitPanel.anchoredPosition.x, exitPanel.anchoredPosition.y - screenHeight);
        configPanel.anchoredPosition = new Vector3(configPanel.anchoredPosition.x, configPanel.anchoredPosition.y - screenHeight);
        victoryPanel.anchoredPosition = new Vector3(victoryPanel.anchoredPosition.x, victoryPanel.anchoredPosition.y - screenHeight);
        losePanel.anchoredPosition = new Vector3(losePanel.anchoredPosition.x, losePanel.anchoredPosition.y - screenHeight);

        pausePanel.gameObject.SetActive(true);
    }

    private void Start()
    {
        sfxSlider.value = PlayerPrefs.GetFloat("sfx_volume");
        musicSlider.value = PlayerPrefs.GetFloat("music_volume");
    }

    public void EndPresentation()
    {
        versusPanel.SetActive(false);
        pauseCanvas.SetActive(false);
    }

    public void OpenPausePanel()
    {
        BGEffect(true);
        AudioManager.instance.FadeMusic(.3f, 1);
        GameManager.Instance.SetPauseGame(true);
        AudioManager.instance.PlaySFX("uiClick");

        PanelEffect(pausePanel, true);
        pauseCanvas.SetActive(true);

        panelSequence.Push(pausePanel);
    }

    public void OpenConfigPanel()
    {
        PanelEffect(panelSequence.Peek(), false);
        PanelEffect(configPanel, true);

        panelSequence.Push(configPanel);
        AudioManager.instance.PlaySFX("uiClick");
    }

    public void OpenExitPanel()
    {
        PanelEffect(panelSequence.Peek(), false);
        PanelEffect(exitPanel, true);

        panelSequence.Push(exitPanel);
        AudioManager.instance.PlaySFX("uiClick");
    }

    public void OpenLosePanel(string classification)
    {
        finalClassificationText.text = "classificacao: " + classification;

        BGEffect(true);
        AudioManager.instance.FadeMusic(.3f, 1);
        GameManager.Instance.SetPauseGame(true);

        PanelEffect(losePanel, true);
        pauseCanvas.SetActive(true);

        panelSequence.Push(losePanel);
    }

    public void OpenVictoryPanel()
    {
        BGEffect(true);
        AudioManager.instance.FadeMusic(.3f, 1);
        GameManager.Instance.SetPauseGame(true);

        PanelEffect(victoryPanel, true);
        pauseCanvas.SetActive(true);

        panelSequence.Push(victoryPanel);
    }

    public void ReturnPanel()
    {
        PanelEffect(panelSequence.Pop(), false, panelSequence.Count == 0);
        AudioManager.instance.PlaySFX("uiClick");

        if (panelSequence.Count == 0)
        {
            BGEffect(false);
            AudioManager.instance.FadeMusic(1, 1);
        }
        else
        {
            PanelEffect(panelSequence.Peek(), true);
        }
    }

    public void RestartChampionship()
    {
        StartSceneTransition(1);
        GameManager.Instance.RemoveSingletonOperation();
    }

    public void StartSceneTransition(int sceneIndex)
    {
        if(GameManager.Instance.CurrentGame == 6)
        {
            sceneIndex = 2;
        }
        if(sceneIndex != 1)
        {
            GameManager.Instance.RemoveSingletonOperation();
        }

        SceneTransitionHandler.Instance.StartSceneTransition(sceneIndex);
    }

    public void SetScoreText(int value)
    {
        scoreText.text = value.ToString();
    }

    public void SetLifeText(int value)
    {
        lifeText.text = "x" + value.ToString();
    }

    public void SetSFXVolume(float newVolume)
    {
        AudioManager.instance.SetSFXVolume(newVolume);
    }

    public void SetMusicVolume(float newVolume)
    {
        AudioManager.instance.SetMusicVolume(newVolume);
    }

    private void PanelEffect(RectTransform panelToEffect, bool showIn, bool unPauseAfter = false)
    {
        panelToEffect.TryGetComponent(out currentCanvasGroup);
        currentCanvasGroup.interactable = false;

        panelToEffect.LeanMoveY(showIn ? 0 : panelToEffect.anchoredPosition.y - screenHeight, 1)
            .setEaseOutCubic()
            .setIgnoreTimeScale(true)
            .setOnComplete(() =>
            {
                currentCanvasGroup.interactable = true;
                if (unPauseAfter)
                {
                    pauseCanvas.SetActive(false);
                    GameManager.Instance.SetPauseGame(false);
                }
            });
    }

    private void BGEffect(bool fadeIn)
    {
        bgImage.color = new Color(0, 0, 0, fadeIn ? 0 : bgImageAlphaInitial);

        LeanTween.value(fadeIn ? 0 : bgImageAlphaInitial, fadeIn ? bgImageAlphaInitial : 0, 1f)
            .setIgnoreTimeScale(true)
            .setOnUpdate((float value) =>
            {
                bgImage.color = new Color(0, 0, 0, value);
            });
    }
}
