using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [Header("Country Panel")]
    [SerializeField] private List<Country> countries;
    [SerializeField] private List<Image> buttonHighlights;

    private int _selectedCountryIndex = -1;
    private MenuInterface _menuInterface;

    private void Awake()
    {
        foreach (var highlight in buttonHighlights)
        {
            highlight.enabled = false;
        }

        _menuInterface = FindObjectOfType<MenuInterface>();
    }

    private void Start()
    {
        AudioManager.instance.PlayMusic("MenuMusic");
        AudioManager.instance.FadeMusic(1, .3f);
    }

    public void SelectCountry(int countryIndex)
    {
        if (_selectedCountryIndex != -1)
        {
            buttonHighlights[_selectedCountryIndex].enabled = false;
        }
        else
        {
            _menuInterface.ShowConfirmSelection();
        }
        
        _selectedCountryIndex = countryIndex;
        buttonHighlights[countryIndex].enabled = true;
        PlayerPrefs.SetInt("country", countryIndex);

        AudioManager.instance.PlaySFX("uiClick");
    }

    public void CloseGame()
    {
        Application.Quit();
    }
}
