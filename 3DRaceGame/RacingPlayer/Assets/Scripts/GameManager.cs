using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public int score = 0;
    public TMP_Text scoreText;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void AddScore()
    {
        score++;
        scoreText.text = "Score: " + score;
        Debug.Log("Score atualizado: " + score);
    }
}
