using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionHandler : MonoBehaviour
{
    [SerializeField] private RectTransform ballTransform;
    private bool _inTransition;

    public static SceneTransitionHandler Instance;

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        ballTransform.anchoredPosition = Vector2.down * 2000;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoad;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoad;
    }

    /*
     * Menu - 0
     * Game - 1
     * Ending - 2
     */
    public void StartSceneTransition(int sceneDestination)
    {
        if (_inTransition)
        {
            return;
        }
        _inTransition = true;

        ballTransform.LeanRotateZ(45, 0)
            .setIgnoreTimeScale(true);
        ballTransform.LeanRotateZ(0, 1)
            .setIgnoreTimeScale(true);

        ballTransform.LeanMoveY(0, 1)
            .setEaseOutCubic()
            .setIgnoreTimeScale(true)
            .setOnComplete(() =>
            {
                Time.timeScale = 1;
                SceneManager.LoadScene(sceneDestination);
            });
    }

    private void OnSceneLoad(Scene arg0, LoadSceneMode arg1)
    {
        if (!_inTransition)
        {
            return;
        }

        EndSceneTransition();
    }

    private void EndSceneTransition()
    {
        ballTransform.LeanRotateZ(-45, 1)
            .setIgnoreTimeScale(true);

        ballTransform.LeanMoveY(-2000, 1)
            .setEaseInCubic()
            .setIgnoreTimeScale(true)
            .setOnComplete(() => {
                _inTransition = false;
            });
    }
}
