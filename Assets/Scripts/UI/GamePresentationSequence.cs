using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class GamePresentationSequence : MonoBehaviour, IPointerDownHandler
{
    [Header("Main")]
    [SerializeField] private CanvasScaler mainCanvas;
    [SerializeField] private GameObject presentationCanvas;
    [SerializeField] private Image BGImage;
    [SerializeField] private GameInterface gameInterface;

    [Header("Animation Objects")]
    [SerializeField] private GameObject nextGameText;
    [SerializeField] private GameObject classificationText;
    [SerializeField] private GameObject classificationTextShadow;
    [SerializeField] private GameObject leftFlag;
    [SerializeField] private GameObject rightFlag;
    [SerializeField] private GameObject xImage;
    [SerializeField] private GameObject xImageShadow;
    [SerializeField] private GameObject challengeText;
    [SerializeField] private GameObject touchScreenText;

    [Header("Other Texts")]
    [SerializeField] private TextMeshProUGUI leftCountryText;
    [SerializeField] private TextMeshProUGUI rightCountryText;

    private bool _animationFinished = true;
    private bool _touched;
    private CanvasGroup pauseCanvasGroup;

    public void SetValues(string classification, string challenge, Sprite leftFlagSprite, Sprite rightFlagSprite, string leftCountryName, string rightCountryName)
    {
        classificationText.TryGetComponent(out TextMeshProUGUI classificationTextComponent);
        classificationTextShadow.TryGetComponent(out TextMeshProUGUI classificationTextShadowComponent);
        challengeText.TryGetComponent(out TextMeshProUGUI challengeTextComponent);

        classificationTextComponent.text = classificationTextShadowComponent.text = classification;
        challengeTextComponent.text = challenge;

        leftFlag.TryGetComponent(out Image leftFlagImage);
        rightFlag.TryGetComponent(out Image rightFlagImage);

        leftFlagImage.sprite = leftFlagSprite;
        rightFlagImage.sprite = rightFlagSprite;

        leftCountryText.text = leftCountryName;
        rightCountryText.text = rightCountryName;
    }

    public void StartPresentationSequence()
    {
        if (!_animationFinished)
        {
            return;
        }

        StartCoroutine(PresentationSequence());
    }

    private IEnumerator PresentationSequence()
    {
        _animationFinished = false;

        var screenWidth = mainCanvas.referenceResolution.x * 3 / 2;
        var screenHeight = mainCanvas.referenceResolution.y * 3 / 2;

        nextGameText.SetActive(false);
        leftFlag.SetActive(false);
        rightFlag.SetActive(false);
        xImage.SetActive(false);
        xImageShadow.SetActive(false);
        classificationText.SetActive(false);
        classificationTextShadow.SetActive(false);
        challengeText.SetActive(false);
        touchScreenText.SetActive(false);

        #region nextGameTextAnim

        nextGameText.TryGetComponent(out RectTransform nextGameTextTransform);
        nextGameTextTransform.localPosition = Vector3.zero;

        nextGameText.TryGetComponent(out TextMeshProUGUI nextGameTextComponent);

        var nextGameTextString = nextGameTextComponent.text.ToCharArray();
        nextGameTextComponent.text = "";

        nextGameText.SetActive(true);

        for (int i = 0; i < nextGameTextString.Length; i++)
        {
            nextGameTextComponent.text += nextGameTextString[i];
            AudioManager.instance.PlaySFX("keyPress");
            yield return new WaitForSeconds(.08f);
        }

        yield return new WaitForSeconds(.5f);

        nextGameTextTransform.LeanMoveY(0, .8f)
            .setEaseOutCubic();

        #endregion

        yield return new WaitForSeconds(1f);

        #region flagAnim
        leftFlag.TryGetComponent(out RectTransform leftFlagTranform);
        rightFlag.TryGetComponent(out RectTransform rightFlagTranform);

        leftFlagTranform.anchoredPosition = new Vector2(leftFlagTranform.anchoredPosition.x - screenWidth, leftFlagTranform.anchoredPosition.y);
        rightFlagTranform.anchoredPosition = new Vector2(rightFlagTranform.anchoredPosition.x + screenWidth, rightFlagTranform.anchoredPosition.y);

        leftFlag.SetActive(true);
        rightFlag.SetActive(true);

        leftFlagTranform.LeanMoveX(0, .8f)
            .setEaseOutCubic();

        rightFlagTranform.LeanMoveX(0, .8f)
            .setEaseOutCubic();
        #endregion

        yield return new WaitForSeconds(1.2f);

        #region xAndChallengeTextAnim
        xImage.TryGetComponent(out RectTransform xTransform);
        xTransform.eulerAngles = new Vector3(0, 0, 45);
        xTransform.localScale = new Vector3(2, 2, 2);

        xImage.TryGetComponent(out Image xImageComponent);
        xImageComponent.color = new Color(1, 0, 0, 0);

        xImage.SetActive(true);

        LeanTween.value(0, 1, .8f)
            .setOnUpdate((float value) =>
            {
                xImageComponent.color = new Color(1, 0, 0, value);
            });

        xImage.LeanAlpha(1, .8f)
            .setEaseOutCubic();
        xImage.LeanRotateZ(0, .8f)
            .setEaseOutCubic();
        xImage.LeanScale(Vector3.one, .8f)
            .setEaseOutCubic();

        yield return new WaitForSeconds(.8f);

        xImageShadow.TryGetComponent(out Image xImageShadowComponent);
        xImageShadow.SetActive(true);

        LeanTween.value(1, 0, .8f)
            .setOnUpdate((float value) =>
            {
                xImageShadowComponent.color = new Color(1, 0, 0, value);
            });
        xImageShadow.LeanScale(new Vector3(2, 2, 2), .8f)
            .setEaseOutCubic()
            .setOnComplete(() =>
            {
                xImageShadow.SetActive(false);
            });

        classificationTextShadow.TryGetComponent(out TextMeshProUGUI classificationTextShadowComponent);
        classificationTextShadow.SetActive(true);

        LeanTween.value(1, 0, .8f)
            .setOnUpdate((float value) =>
            {
                classificationTextShadowComponent.color = new Color(1, 0, 0, value);
            });

        classificationTextShadow.LeanScale(new Vector3(2, 2, 2), .8f)
            .setEaseOutCubic()
            .setOnComplete(() =>
            {
                classificationTextShadow.SetActive(false);
            });

        classificationText.SetActive(true);
        classificationText.LeanScale(new Vector3(1.1f, 1.1f, 1.1f), 1f)
            .setEaseInOutCubic()
            .setLoopPingPong();

        yield return new WaitForSeconds(1.5f);

        #endregion

        yield return new WaitForSeconds(.5f);

        #region challengeTextAnim
        challengeText.TryGetComponent(out RectTransform challengeTextTransform);

        challengeTextTransform.anchoredPosition = new Vector2(challengeTextTransform.anchoredPosition.x, challengeTextTransform.anchoredPosition.y - screenWidth);
        challengeText.SetActive(true);

        challengeTextTransform.LeanMoveY(0, .8f)
            .setEaseOutCubic();
        #endregion

        yield return new WaitForSeconds(1.5f);

        #region touchScreenStartAnim

        touchScreenText.SetActive(true);
        touchScreenText.LeanScale(new Vector3(1.05f, 1.05f, 1.05f), 1f)
            .setEaseOutCubic()
            .setLoopPingPong();

        touchScreenText.TryGetComponent(out TextMeshProUGUI touchScreenTextComponent);

        LeanTween.value(1, .5f, 1f)
            .setLoopPingPong()
            .setOnUpdate((float value) =>
            {
                touchScreenTextComponent.color = new Color(1, 1, 1, value);
            });

        #endregion

        _animationFinished = true;
    }

    public void ReceiveTouch()
    {
        if (!_animationFinished || _touched)
        {
            return;
        }

        _touched = true;

        presentationCanvas.TryGetComponent(out pauseCanvasGroup);

        pauseCanvasGroup.LeanAlpha(0, .75f).
            setOnComplete(() =>
            {
                BGImage.color = new Color(0, 0, 0, 0);
                pauseCanvasGroup.alpha = 1;
                presentationCanvas.SetActive(false);
            });
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        ReceiveTouch();
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        LeanTween.cancelAll();
        GameManager.Instance.GameStarted = true;
    }
}
