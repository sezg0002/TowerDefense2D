using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager gameManagerInstance;
    public static GameManager Instance
    {
        get
        {
            if (gameManagerInstance == null)
                gameManagerInstance = FindObjectOfType<GameManager>();
            return gameManagerInstance;
        }
    }

    public const int MaxLives = 5;
    public int InitialMoney;

    public int Level;
    public GameObject VictoryText;
    public GameObject GameOverText;

    public int InitialTurretPrice;
    public int InitialRocketPrice;
    public int TurretPriceAddition;
    public int RocketPriceAddition;

    private int turretPrice;
    private int rocketPrice;

    public static int Lives;
    private int money;
    private HealthDrawerScript healthDrawer;
    private MoneyDrawer moneyDrawer;

    private int remainingEnemies;

    // Variable pour le suivi des vagues
    public int WavesSurvived { get; private set; }

    public int Money
    {
        get { return money; }
        set
        {
            money = value;
            if (moneyDrawer != null)
                moneyDrawer.Draw(money);
        }
    }

    void Start()
    {
        money = InitialMoney;
        turretPrice = InitialTurretPrice;
        rocketPrice = InitialRocketPrice;
        healthDrawer = GetComponent<HealthDrawerScript>();
        moneyDrawer = GetComponent<MoneyDrawer>();

        Lives = MaxLives;
        moneyDrawer.Draw(InitialMoney);
        remainingEnemies = GetComponent<EnemySpawner>().Waves.Sum(w => w.Amount);
        WavesSurvived = 0; // Initialisation du compteur de vagues
    }

    // À appeler à chaque début de vague dans EnemySpawner
    public void OnNewWaveStart(int waveNumber)
    {
        WavesSurvived = waveNumber;
    }

    public void EnemyEscaped(GameObject enemy)
    {
        Lives--;
        CameraShaker.Instance.Shake();
        healthDrawer.Draw(Lives);

        if (Lives <= 0)
        {
            GameOver();
        }

        remainingEnemies--;
    }

    public void EnemyKilled(GameObject enemy)
    {
        remainingEnemies--;
    }

    public int GetMoney()
    {
        return money;
    }

    public void AddMoney(int value)
    {
        Money += value;
    }

    public void TurretBuilt(GameObject turret)
    {
        if (turret.CompareTag("turretTower"))
        {
            Money -= turretPrice;
            turretPrice += TurretPriceAddition;
        }
        else
        {
            Money -= rocketPrice;
            rocketPrice += RocketPriceAddition;
        }
    }

    public void CoinCollected(GameObject coin)
    {
        Money += CoinScript.Value;
    }

    public bool EnoughMoneyForTurret(string tag)
    {
        if (tag == "turretTower")
            return Money >= turretPrice;
        return Money >= rocketPrice;
    }

    public int MoneyForTurret(string tag)
    {
        return tag == "turretTower" ? turretPrice : rocketPrice;
    }

    public void GameOver()
    {
        GameOverText.SetActive(true);
        SaveScore(WavesSurvived);
        Invoke("BackToMainMenu", 5.0f);
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("Menu_screen");
    }

    // SCOREBOARD : Sauvegarde les 5 meilleurs scores dans PlayerPrefs 
    void SaveScore(int score)
    {
        List<int> highscores = new List<int>();
        for (int i = 0; i < 5; i++)
            highscores.Add(PlayerPrefs.GetInt("highscore_" + i, 0));
        highscores.Add(score);

        highscores.Sort();
        highscores.Reverse();
        while (highscores.Count > 5)
            highscores.RemoveAt(highscores.Count - 1);

        for (int i = 0; i < highscores.Count; i++)
            PlayerPrefs.SetInt("highscore_" + i, highscores[i]);
        PlayerPrefs.Save();
    }
}