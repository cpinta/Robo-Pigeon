using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SocialPlatforms.Impl;
using Random = UnityEngine.Random;

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

    public bool isInDebugMode = true;

    public Camera cam;
    public PlayerController player;

    public Van prefabVan;
    public Newspaper prefabNewspaper;
    public GameObject prefabHouseYellow;
    public GameObject prefabHouseRed;
    public GameObject prefabHousePurple;
    public GameObject prefabHouseBlue;

    public Material matInvisible;

    public UnityEvent<string, int> scoreChanged;

    //score vars
    public int score = 0;
    public int highscore = 0;

    string strGrabNewspaperDesc = "Stole Newspaper";
    int grabNewspaperScoreAmount = 5;

    public int currentStreak = 0;
    public int highestStreak = 0;
    float streakTime = 4;
    float streakTimer = 0;
    float perStreakScoreMultiplier = 0.05f;

    public float playerDistanceToBorder = 1000;
    [SerializeField] Vector3 cityBorderDimensions = new Vector3(600, 300, 600);

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

        // balls
        string balls = "balls";
        int[] intArray = balls.ToIntArray();
        Random.InitState(intArray.Select((t, i) => t * Convert.ToInt32(Math.Pow(10, intArray.Length - i - 1))).Sum());
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

    public void PlayerGrabbedNewspaper()
    {
        ChangeScore(grabNewspaperScoreAmount, strGrabNewspaperDesc);
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
        if (player != null) 
        {
            Vector3 playerPos = player.transform.position;
            Vector3 borderDistances = ((cityBorderDimensions / 2) - new Vector3(Mathf.Abs(playerPos.x), Mathf.Abs(playerPos.y), Mathf.Abs(playerPos.z)));
            playerDistanceToBorder = Mathf.Min(borderDistances.x, Mathf.Min(borderDistances.y, borderDistances.z));
            Debug.Log(playerDistanceToBorder);
        }
    }

    void Restart()
    {
        playerDistanceToBorder = 1000;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(Vector3.zero + (Vector3.up * cityBorderDimensions.y/2), cityBorderDimensions);
    }
}
