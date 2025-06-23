using UnityEngine;

public class TowerInfo : MonoBehaviour
{
    [SerializeField] private TowerFactory.TowerType towerType;
    [SerializeField] private string towerName;
    [SerializeField] private int baseCost;
    [SerializeField] private float creationTime;

    public TowerFactory.TowerType TowerType { get { return towerType; } }
    public string TowerName { get { return towerName; } }
    public int BaseCost { get { return baseCost; } }
    public float CreationTime { get { return creationTime; } }

    void Awake()
    {
        creationTime = Time.time;
    }

    public void SetTowerType(TowerFactory.TowerType type, string name, int cost)
    {
        towerType = type;
        towerName = name;
        baseCost = cost;
    }

    public string GetDescription()
    {
        return string.Format("{0} (Type: {1}, Cost: ${2}, Age: {3:F1}s)", 
            towerName, towerType, baseCost, Time.time - creationTime);
    }
}