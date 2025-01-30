using TMPro;
using UnityEngine;

public class UI_Speedometer : MonoBehaviour
{
    TMP_Text text;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        text = GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GM.Instance.player != null)
        {
            text.text = GM.Instance.player.rb.linearVelocity.magnitude.ToString();
        }
    }
}
