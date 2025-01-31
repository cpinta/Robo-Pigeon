using TMPro;
using UnityEngine;

public class UI_ScoreAdditionItem : MonoBehaviour
{
    [SerializeField] TMP_Text lblDescription;
    [SerializeField] TMP_Text lblScore;

    float aliveTime = 5;
    float aliveTimer = 5;

    // Update is called once per frame
    void Update()
    {
        if(aliveTimer > 0)
        {
            aliveTimer -= Time.deltaTime;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Setup(string description, int amount)
    {
        lblDescription.text = description;
        lblScore.text = (amount < 0 ? "-" : "+") + amount;
        aliveTimer = aliveTime;
    }
}
