using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum WarningScreenState
{
    None = 0,
    NearBorder = 1,
    Hit = 2,
    BatteryLow = 3,
}

public class UI_WarningPanel : MonoBehaviour
{
    Image panelBackground;
    TMP_Text centerText;

    [SerializeField] Color red;
    [SerializeField] Color lowBatteryColor;
    [SerializeField] Color black;

    [SerializeField] float highAlpha;
    [SerializeField] float textShake;

    bool blinkText = false;
    bool textTransparent = false;
    [SerializeField] float timeBtBlinks;
    float timeBtBlinksTimer;

    float nearBorderDistance = 40;

    string strNearBorderWarning = "WARNING:\nApproaching City Limits\nTurn Back!";

    WarningScreenState state;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        panelBackground = GetComponent<Image>();
        centerText = GetComponentInChildren<TMP_Text>();
        SetState(WarningScreenState.None);
    }

    // Update is called once per frame
    void Update()
    {
        if (blinkText)
        {
            if (timeBtBlinksTimer > 0) 
            {
                timeBtBlinksTimer -= Time.deltaTime;
            }
            else
            {
                centerText.alpha = centerText.alpha == highAlpha ? 0 : highAlpha;
                timeBtBlinksTimer = timeBtBlinks;
            }
        }
        else
        {
            centerText.alpha = highAlpha;
        }



        switch (state)
        {
            case WarningScreenState.None:
                centerText.enabled = false;
                if (GM.Instance.playerDistanceToBorder < nearBorderDistance)
                {
                    SetState(WarningScreenState.NearBorder);
                }
                break;
            case WarningScreenState.NearBorder:
                centerText.enabled = true;
                if (GM.Instance.playerDistanceToBorder > nearBorderDistance)
                {
                    SetState(WarningScreenState.None);
                }
                break;
            case WarningScreenState.Hit:
                centerText.enabled = true;
                break;
        }
    }

    public void SetState(WarningScreenState newState)
    {
        state = newState;

        switch (state)
        {
            case WarningScreenState.None:
                panelBackground.color = Color.clear;
                blinkText = false;
                SetDisplayText("");
                break;
            case WarningScreenState.NearBorder:
                panelBackground.color = red;
                blinkText = true;
                SetDisplayText(strNearBorderWarning);
                break;
            case WarningScreenState.Hit:
                panelBackground.color = red;
                blinkText = true;
                SetDisplayText("");
                break;
        }
    }

    public void SetDisplayText(string text)
    {
        centerText.text = text;
    }
}
