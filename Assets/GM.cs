using UnityEngine;

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

    public Camera camera;
    public PlayerController player;

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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
