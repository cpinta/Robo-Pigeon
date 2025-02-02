using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Power : MonoBehaviour
{
    [SerializeField] TMP_Text text;
    [SerializeField] TMP_Text lowPowerText;
    [SerializeField] Slider slider;

    float timeBtBlinks = 0.75f;
    float timeBtBlinksTimer = 0;

    float lowPowerPercent = 0.2f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GM.Instance.player != null)
        {
            slider.value = GM.Instance.player.currentPower / GM.Instance.player.powerMax;
            if(slider.value < lowPowerPercent)
            {
                if(timeBtBlinksTimer > 0)
                {
                    timeBtBlinksTimer -= Time.deltaTime;
                }
                else
                {
                    lowPowerText.enabled = !lowPowerText.enabled;
                    timeBtBlinksTimer = timeBtBlinks;
                }
            }
            else
            {
                lowPowerText.enabled = false;
            }
        }
    }
}
