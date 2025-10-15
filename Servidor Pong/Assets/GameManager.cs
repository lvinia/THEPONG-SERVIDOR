using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int myScore = 0;
    public int enemyScore = 0;

    [Header("UI")]
    public Text myScoreText;
    public Text enemyScoreText;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void AddMyPoint()
    {
        myScore++;
        UpdateUI();
    }

    public void AddEnemyPoint()
    {
        enemyScore++;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (myScoreText) myScoreText.text = myScore.ToString();
        if (enemyScoreText) enemyScoreText.text = enemyScore.ToString();
    }
}