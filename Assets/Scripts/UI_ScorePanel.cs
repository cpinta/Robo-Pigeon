using TMPro;
using UnityEngine;

public class UI_ScorePanel : MonoBehaviour
{
    [SerializeField] TMP_Text lblScore;
    [SerializeField] Transform trScoreAddDisplay;
    [SerializeField] UI_ScoreAdditionItem prefabScoreAddItem;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GM.Instance.scoreChanged.AddListener(ScoreChanged);
    }

    // Update is called once per frame
    void Update()
    {
        lblScore.text = GM.Instance.score.ToString();
    }

    void ScoreChanged(string description, int amount)
    {
        UI_ScoreAdditionItem uiScoreAddItem = Instantiate(prefabScoreAddItem, trScoreAddDisplay);
        uiScoreAddItem.Setup(description, amount);
    }
}
