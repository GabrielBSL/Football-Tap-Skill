using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VictoryManager : MonoBehaviour
{
    [SerializeField] private List<Country> countries;

    [SerializeField] private Image countryFlagImage;
    [SerializeField] private TextMeshProUGUI countryNameText;

    private void Awake()
    {
        countryFlagImage.sprite = countries[PlayerPrefs.GetInt("country")].flag;
        countryNameText.text = countries[PlayerPrefs.GetInt("country")].name;

        AudioManager.instance.PlayMusic("VictoryMusic");
        AudioManager.instance.FadeMusic(1, .3f);
    }

    public void GoToMenu()
    {
        AudioManager.instance.PlaySFX("uiClick");
        SceneTransitionHandler.Instance.StartSceneTransition(0);
    }
}
