using UnityEngine;
using UnityEngine.UI;
using System.Text;

public class HighscoreDisplay : MonoBehaviour
{
    public Text highscoreText; // À assigner dans l’inspecteur Unity

    void Start()
    {
        highscoreText.text = GetFormattedHighscores();
    }

    string GetFormattedHighscores()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Classement des vagues atteintes :\n");
        for (int i = 0; i < 5; i++)
        {
            int score = PlayerPrefs.GetInt("highscore_" + i, 0);
            sb.AppendLine((i + 1) + ". " + score + " vagues");
        }
        return sb.ToString();
    }
}