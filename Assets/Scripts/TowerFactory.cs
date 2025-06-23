using UnityEngine;
using System.Collections.Generic;

public static class TowerFactory
{
    public enum TowerType { Fire, Ice, Wind, Basic }

    private static readonly Dictionary<TowerType, TowerConfig> towerConfigs = new Dictionary<TowerType, TowerConfig>
    {
        {
            TowerType.Fire,
            new TowerConfig
            {
                effectType = CannonScript.EffectType.Fire,
                visualType = global::TowerType.Fire,
                color = new Color(1f, 0.3f, 0.3f),
                baseCost = 50,
                name = "Fire Tower"
            }
        },
        {
            TowerType.Ice,
            new TowerConfig
            {
                effectType = CannonScript.EffectType.Ice,
                visualType = global::TowerType.Ice,
                color = new Color(0.3f, 0.7f, 1f),
                baseCost = 60,
                name = "Ice Tower"
            }
        },
        {
            TowerType.Wind,
            new TowerConfig
            {
                effectType = CannonScript.EffectType.Wind,
                visualType = global::TowerType.Wind,
                color = new Color(0.5f, 1f, 0.5f),
                baseCost = 55,
                name = "Wind Tower"
            }
        },
        {
            TowerType.Basic,
            new TowerConfig
            {
                effectType = CannonScript.EffectType.Default,
                visualType = global::TowerType.Basic,
                color = Color.white,
                baseCost = 40,
                name = "Basic Tower"
            }
        }
    };

    private struct TowerConfig
    {
        public CannonScript.EffectType effectType;
        public global::TowerType visualType;
        public Color color;
        public int baseCost;
        public string name;
    }

    public static GameObject CreateTower(TowerType type, Vector3 position)
    {
        GameObject towerPrefab = Resources.Load<GameObject>("Turret");
        if (towerPrefab == null)
        {
            Debug.LogError("Le préfab Turret n'a pas été trouvé dans Resources/");
            return null;
        }

        GameObject tower = Object.Instantiate(towerPrefab, position, Quaternion.identity);
        
        ConfigureTower(tower, type);
        
        return tower;
    }

    public static void TransformTower(GameObject tower, TowerType newType)
    {
        if (tower == null)
        {
            Debug.LogError("Tower is null!");
            return;
        }

        ConfigureTower(tower, newType);
    }

    private static void ConfigureTower(GameObject tower, TowerType type)
    {
        if (!towerConfigs.ContainsKey(type))
        {
            Debug.LogError("Type de tourelle non configuré : " + type);
            return;
        }

        TowerConfig config = towerConfigs[type];

        CannonScript cannon = tower.GetComponentInChildren<CannonScript>();
        if (cannon != null)
        {
            cannon.SetEffectStrategy(config.effectType);
        }

        TowerVisuals visuals = tower.GetComponentInChildren<TowerVisuals>();
        if (visuals != null)
        {
            visuals.SetVisuals(config.visualType);
        }
        else
        {
            SetTowerColor(tower, config.color);
        }

        TowerInfo towerInfo = tower.GetComponent<TowerInfo>();
        if (towerInfo == null)
        {
            towerInfo = tower.AddComponent<TowerInfo>();
        }
        towerInfo.SetTowerType(type, config.name, config.baseCost);

        Debug.Log("Tourelle configurée : " + config.name + " à la position " + tower.transform.position);
    }

    private static void SetTowerColor(GameObject tower, Color color)
    {
        SpriteRenderer[] renderers = tower.GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer renderer in renderers)
        {
            if (renderer.gameObject.name.Contains("Base") || renderer.gameObject == tower)
            {
                renderer.color = color;
            }
        }
    }

    public static string GetTowerName(TowerType type)
    {
        return towerConfigs.ContainsKey(type) ? towerConfigs[type].name : "Unknown";
    }

    public static int GetTowerBaseCost(TowerType type)
    {
        return towerConfigs.ContainsKey(type) ? towerConfigs[type].baseCost : 0;
    }

    public static TowerType[] GetAvailableTowerTypes()
    {
        return new TowerType[] { TowerType.Fire, TowerType.Ice, TowerType.Wind, TowerType.Basic };
    }
}