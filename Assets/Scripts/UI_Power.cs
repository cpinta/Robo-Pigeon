using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Power : MonoBehaviour
{
    [SerializeField] TMP_Text text;
    [SerializeField] Slider slider;

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
        }
    }
}
