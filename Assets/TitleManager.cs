using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
    [SerializeField] Button startButton;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip acTitleMusic;

    [SerializeField] TMP_Text txtPigeon;
    [SerializeField] TMP_Text txtSubText;

    [SerializeField] float currentTime;
    [SerializeField] float destinationTime;
    [SerializeField] float fullNameTime;
    float cameraDestinationY;

    float songPlayDelay = 1;
    float songPlayDelayTimer = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.Stop();
        cameraDestinationY = Camera.main.transform.position.y;
        Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, 0.1f, Camera.main.transform.position.z);

        txtPigeon.enabled = false;
        txtSubText.enabled = false;
        startButton.gameObject.SetActive(false);
        startButton.onClick.AddListener(StartGame);
    }

    // Update is called once per frame
    void Update()
    {

        if(songPlayDelayTimer > songPlayDelay)
        {
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
            if (currentTime < destinationTime)
            {
                currentTime = audioSource.time;
            }
            else
            {
                currentTime = destinationTime;
            }
            Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, cameraDestinationY * (currentTime / destinationTime), Camera.main.transform.position.z);

            if(destinationTime < audioSource.time)
            {
                txtPigeon.enabled = true;
            }
            if(fullNameTime < audioSource.time)
            {
                txtSubText.enabled = true;
                startButton.enabled = true;
                startButton.gameObject.SetActive(true);
            }
        }
        else
        {
            songPlayDelayTimer += Time.deltaTime;
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Scenes/Scene");
    }
}
