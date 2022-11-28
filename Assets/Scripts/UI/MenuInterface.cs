using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuInterface : MonoBehaviour
{
    [Header("Main")]
    [SerializeField] private Image bgImage;
    [SerializeField] private GameObject panelParent;
    [SerializeField] private GameObject confirmSelectionBox;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider musicSlider;

    [Header("Panels")]
    [SerializeField] private RectTransform countryPanel;
    [SerializeField] private RectTransform configPanel;
    [SerializeField] private RectTransform creditsPanel;
    [SerializeField] private RectTransform exitPanel;

    private Stack<RectTransform> panelSequence = new Stack<RectTransform>();

    private float bgImageAlphaInitial;
    private CanvasGroup currentCanvasGroup;

    private void Awake()
    {
        countryPanel.anchoredPosition += Vector2.down * 4000;
        configPanel.anchoredPosition += Vector2.down * 4000;
        creditsPanel.anchoredPosition += Vector2.down * 4000;
        exitPanel.anchoredPosition += Vector2.down * 4000;

        bgImageAlphaInitial = bgImage.color.a;
        panelParent.SetActive(false);
        confirmSelectionBox.SetActive(false);
    }

    private void Start()
    {
        sfxSlider.value = PlayerPrefs.GetFloat("sfx_volume");
        musicSlider.value = PlayerPrefs.GetFloat("music_volume");
    }

    public void OpenCountryPanel()
    {
        if(panelSequence.Count > 0)
        {
            PanelEffect(panelSequence.Peek(), false);
        }
        else
        {
            BGEffect(true);
        }
        PanelEffect(countryPanel, true);
        AudioManager.instance.PlaySFX("uiClick");

        panelSequence.Push(countryPanel);
    }

    public void OpenConfigPanel()
    {
        if (panelSequence.Count > 0)
        {
            PanelEffect(panelSequence.Peek(), false);
        }
        else
        {
            BGEffect(true);
        }
        PanelEffect(configPanel, true);
        AudioManager.instance.PlaySFX("uiClick");

        panelSequence.Push(configPanel);
    }

    public void OpenCreditsPanel()
    {
        if (panelSequence.Count > 0)
        {
            PanelEffect(panelSequence.Peek(), false);
        }
        else
        {
            BGEffect(true);
        }
        PanelEffect(creditsPanel, true);
        AudioManager.instance.PlaySFX("uiClick");

        panelSequence.Push(creditsPanel);
    }

    public void OpenExitPanel()
    {
        if (panelSequence.Count > 0)
        {
            PanelEffect(panelSequence.Peek(), false);
        }
        else
        {
            BGEffect(true);
        }
        PanelEffect(exitPanel, true);
        AudioManager.instance.PlaySFX("uiClick");

        panelSequence.Push(exitPanel);
    }

    public void ReturnPanel()
    {
        PanelEffect(panelSequence.Pop(), false);
        AudioManager.instance.PlaySFX("uiClick");

        if (panelSequence.Count == 0)
        {
            BGEffect(false);
        }
    }

    public void SetSFXVolume(float newVolume)
    {
        AudioManager.instance.SetSFXVolume(newVolume);
    }

    public void SetMusicVolume(float newVolume)
    {
        AudioManager.instance.SetMusicVolume(newVolume);
    }

    public void ShowConfirmSelection()
    {
        confirmSelectionBox.SetActive(true);
    }

    public void InitiateSceneTransition()
    {
        AudioManager.instance.PlaySFX("crowdHyped2");
        currentCanvasGroup.interactable = false;
        SceneTransitionHandler.Instance.StartSceneTransition(1);
    }

    private void PanelEffect(RectTransform panelToEffect, bool showIn)
    {
        if(panelSequence.Count == 0)
        {
            panelParent.SetActive(true);
        }

        panelToEffect.TryGetComponent(out currentCanvasGroup);
        currentCanvasGroup.interactable = false;

        panelToEffect.LeanMoveY(showIn ? 0 : panelToEffect.anchoredPosition.y - 4000, 1)
            .setEaseOutCubic()
            .setIgnoreTimeScale(true)
            .setOnComplete(() =>
            {
                currentCanvasGroup.interactable = true;
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
            })
            .setOnComplete(() => {
                if (!fadeIn)
                {
                    panelParent.SetActive(false);
                }
            });
    }

    public void QuitGame()
    {
        FindObjectOfType<MenuManager>().CloseGame();
    }
}
