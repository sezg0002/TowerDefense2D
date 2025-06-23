using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public int Amount;
        public GameObject Enemy;
        public float SpawnTime;
        public float RestTime;

        [System.NonSerialized]
        public int InitialAmount;
        [System.NonSerialized]
        public float InitialRestTime;
        [System.NonSerialized]
        public float InitialSpawnTime;
    }

    public List<Wave> Waves;
    private int waveIndex = 0;
    private Wave currentWave;
    private float spawnTime = 2.0f;

    private int difficultyLevel = 0;

    void OnEnable()
    {
        if (Waves.Count > 0)
            currentWave = Waves[0];

        foreach (var wave in Waves)
        {
            wave.InitialAmount = wave.Amount;
            wave.InitialRestTime = wave.RestTime;
            wave.InitialSpawnTime = wave.SpawnTime;
        }
    }

    void Update()
    {
        if (Waves.Count == 0) return;

        if (waveIndex >= Waves.Count)
        {
            difficultyLevel++;
            foreach (var wave in Waves)
            {
                if (wave.Enemy != null && wave.Enemy.CompareTag("Enemy"))
                {
                    wave.Amount = Mathf.RoundToInt(wave.InitialAmount * (1 + 0.75f * difficultyLevel)); // +75% par cycle
                    wave.SpawnTime = Mathf.Max(0.08f, wave.InitialSpawnTime * Mathf.Pow(0.82f, difficultyLevel)); // spawn très rapproché
                }
                else
                {
                    wave.Amount = Mathf.RoundToInt(wave.InitialAmount * (1 + 0.35f * difficultyLevel));
                    wave.SpawnTime = Mathf.Max(0.15f, wave.InitialSpawnTime * Mathf.Pow(0.93f, difficultyLevel));
                }
                wave.RestTime = Mathf.Max(0.1f, wave.InitialRestTime * Mathf.Pow(0.82f, difficultyLevel)); // pauses très courtes
            }
            waveIndex = 0;
            currentWave = Waves[waveIndex];
            spawnTime = currentWave.SpawnTime; 
            // Notifie GameManager du début de la nouvelle "loop" de vagues
            GameManager.Instance.OnNewWaveStart(difficultyLevel * Waves.Count + waveIndex + 1);
            return;
        }

        if (currentWave.RestTime < 0)
        {
            waveIndex += 1;
            if (waveIndex >= Waves.Count) return;
            currentWave = Waves[waveIndex];
            // Notifie GameManager du début de chaque vague
            GameManager.Instance.OnNewWaveStart(difficultyLevel * Waves.Count + waveIndex + 1);
            return;
        }

        if (currentWave.Amount <= 0)
        {
            currentWave.RestTime -= Time.deltaTime;
            return;
        }

        if (spawnTime < 0)
        {
            Spawn(currentWave.Enemy);
            spawnTime = currentWave.SpawnTime;
            currentWave.Amount--;
            return;
        }

        spawnTime -= Time.deltaTime;
    }

    private void Spawn(GameObject prototype)
    {
        var spawnedEnemy = Pool.Instance.ActivateObject(prototype.tag);
        spawnedEnemy.SetActive(true);

        EnemyScript script = spawnedEnemy.GetComponent<EnemyScript>();
        if (script != null)
        {
            float healthMultiplier = Mathf.Pow(1.22f, difficultyLevel);
            float speedMultiplier = Mathf.Pow(1.11f, difficultyLevel);
            script.MaxHealth = script.MaxHealth * healthMultiplier;
            PathFollower pathFollower = spawnedEnemy.GetComponent<PathFollower>();
            if (pathFollower != null)
                pathFollower.Speed *= speedMultiplier;
        }

        EnemyManagerScript.Instance.RegisterEnemy(spawnedEnemy);
    }
}