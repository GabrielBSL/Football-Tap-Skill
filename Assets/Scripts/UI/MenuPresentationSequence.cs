using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MenuPresentationSequence : MonoBehaviour
{
    [SerializeField] private RectTransform upperBoxTransform;
    [SerializeField] private RectTransform lowerBoxTransform;
    [SerializeField] private TextMeshProUGUI upperBoxText;
    [SerializeField] private RectTransform gameLogo;

    void Awake()
    {
        StartCoroutine(PresentationSequence());
    }

    private IEnumerator PresentationSequence()
    {
        upperBoxTransform.anchoredPosition += Vector2.up * 1500;
        lowerBoxTransform.anchoredPosition += Vector2.down * 3000;

        var initialBoxText = upperBoxText.text;
        upperBoxText.text = "";

        gameLogo.TryGetComponent(out Image gameLogoImage);
        gameLogoImage.color = new Color(1, 1, 1, 0);

        yield return new WaitForSeconds(.3f);

        upperBoxTransform.LeanMoveY(0, 1)
            .setEaseOutCubic();

        yield return new WaitForSeconds(1f);

        for (int i = 0; i < initialBoxText.Length; i++)
        {
            upperBoxText.text += initialBoxText[i];
            AudioManager.instance.PlaySFX("keyPress");
            yield return new WaitForSeconds(.08f);
        }

        gameLogo.localScale = new Vector3(1.2f, 1.2f, 1.2f);
        gameLogo.LeanScale(Vector3.one, .8f);

        LeanTween.value(0, 1, .8f)
            .setOnUpdate((float value) =>
            {
                gameLogoImage.color = new Color(1, 1, 1, value);
            });

        yield return new WaitForSeconds(1.2f);

        lowerBoxTransform.LeanMoveY(0, 1)
            .setEaseOutCubic();
    }
}
