using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SocialPlatforms.Impl;

public class GM : MonoBehaviour
{
    public static GM Instance
    {
        get
        {
            return instance;
        }
    }
    private static GM instance;

    public Camera cam;
    public PlayerController player;

    public Van prefabVan;

    public UnityEvent<string, int> scoreChanged;

    //score vars
    public int score = 0;
    public int highscore = 0;

    public int currentStreak = 0;
    public int highestStreak = 0;
    float streakTime = 4;
    float streakTimer = 0;
    float perStreakScoreMultiplier = 0.05f;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.LogError("More than one GameManager in scene!");
            Destroy(this);
        }
    }

    public void ChangeScore(int amount, string description)
    {
        scoreChanged.Invoke(description, amount);


        score += amount;
        if (score < 0)
        {
            score = 0;
        }
    }

    public void AddStreak()
    {
        currentStreak++;
        if (currentStreak > highestStreak)
        {
            highestStreak = currentStreak;
        }
    }

    public void EndStreak()
    {
        currentStreak = 0;
    }



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
