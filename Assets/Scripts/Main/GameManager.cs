using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Serializable]
    private struct Challenge
    {
        public string name;
        public string description;
        public int[] difficultyGoalValues;
        public GameObject rule;
    }

    [SerializeField] private LayerMask ballLayerMask;
    [SerializeField] private List<Country> countries;
    [SerializeField] private List<Challenge> challenges;

    private Camera cam;
    private Ball ball;
    private GameInterface _interface;
    private GamePresentationSequence presentationSequence;
    private List<Country> _selectedCountries = new List<Country>();
    private List<Challenge> _selectedChallenges = new List<Challenge>();
    private System.Random rnd = new System.Random();
    private Country _playerCountry;

    private string[] classifications =
    {
        "Fase de grupos",
        "Fase de grupos",
        "Fase de grupos",
        "Oitavas de final",
        "Quartas de final",
        "semifinal",
        "final",
    };

    private int[] difficultyProgression =
    {
        0,
        0,
        0,
        1,
        1,
        1,
        2
    };

    public bool GameStarted { get; set; }
    public int Score { get; private set; }
    public int Life { get; private set; }
    public int CurrentGame { get; private set; }
    private GameObject _currentRule;

    private int _goal;

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        Instance = this;
        CurrentGame = -1;
    }

    private void Start()
    {
        AudioManager.instance.PlayMusic("GameMusic");
        AudioManager.instance.FadeMusic(1, .3f);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void SetValues()
    {
        cam = Camera.main;
        ball = FindObjectOfType<Ball>();
        _interface = FindObjectOfType<GameInterface>();
        presentationSequence = FindObjectOfType<GamePresentationSequence>();

        Time.timeScale = 1;
        Score = 0;
        Life = 3;
        CurrentGame++;
    }

    public void StartFirstMatch(Country selectedCountry)
    {
        _playerCountry = selectedCountry;
        _selectedCountries.Add(selectedCountry);

        SetNewMatch();
    }

    private void SetNewMatch()
    {
        var availableCountries = countries.FindAll(c => !_selectedCountries.Contains(c));
        var chosenCountryIndex = rnd.Next(availableCountries.Count);

        _selectedCountries.Add(availableCountries[chosenCountryIndex]);

        var availableChallenges = challenges.FindAll(c => !_selectedChallenges.Contains(c));
        var chosenChallengeIndex = rnd.Next(availableChallenges.Count);

        _selectedChallenges.Add(availableChallenges[chosenChallengeIndex]);
        var chosenChallenge = challenges[chosenChallengeIndex];

        if(_currentRule != null)
        {
            Destroy(_currentRule);
        }

        _currentRule = Instantiate(challenges[chosenChallengeIndex].rule);
        _currentRule.TryGetComponent(out Rule _rule);
        _rule.Initiate(ball);

        _goal = chosenChallenge.difficultyGoalValues[difficultyProgression[CurrentGame]];
        var fullChallengeDescription = chosenChallenge.description + "\nObjetivo: atingir " + _goal + " pontos";
        var rivalCountry = availableCountries[chosenCountryIndex];

        presentationSequence.SetValues(classifications[CurrentGame], fullChallengeDescription, _playerCountry.flag, rivalCountry.flag, _playerCountry.name, rivalCountry.name);
        presentationSequence.StartPresentationSequence();
    }

    public void ReceiveTouch(Vector2 screenPos)
    {
        var worldPoint = Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, -cam.transform.position.z));

        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero, .01f, ballLayerMask);

        if (hit)
        {
            ball.ApplyForce(hit.point);
        }
    }

    public void IncrementScore()
    {
        Score++;
        _interface.SetScoreText(Score);

        if(Score >= _goal)
        {
            AudioManager.instance.PlaySFX("crowdHyped");
            _interface.OpenVictoryPanel();
        }
    }

    public void ResetScore(bool withSoundAlert = true)
    {
        if (withSoundAlert)
        {
            AudioManager.instance.PlaySFX("pointSoftReset");
        }

        Score = 0;
        _interface.SetScoreText(Score);
    }

    public bool RemoveLife()
    {
        Life--;
        _interface.SetLifeText(Life);

        if(Life == 0)
        {
            AudioManager.instance.PlaySFX("crowdDisappointed");
            _interface.OpenLosePanel(classifications[CurrentGame]);
        }

        return Life != 0;
    }

    public void RemoveSingletonOperation()
    {
        Instance = null;
        SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetActiveScene());
    }

    public void SetPauseGame(bool pauseValue)
    {
        Time.timeScale = pauseValue ? 0 : 1;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SetValues();
        if(CurrentGame == 0)
        {
            StartFirstMatch(countries[PlayerPrefs.GetInt("country")]);
        }
        else
        {
            SetNewMatch();
        }
    }
}
