using UnityEngine;
using UnityEngine.UI;

public class UpgradeUI : MonoBehaviour
{
    public static UpgradeUI Instance;

    public Button fireButton;
    public Button iceButton;
    public Button windButton;
    public Button closeButton;

    public Button upgradeButton;
    public Text upgradeCostText;
    
    public Text towerInfoText;

    private CannonScript targetCannon;
    private TowerSelection selector;

    void Awake()
    {
        Instance = this;
    }

    public void SetTargetCannon(CannonScript cannon, TowerSelection selection)
    {
        targetCannon = cannon;
        selector = selection;
        gameObject.SetActive(true);
        
        transform.position = Camera.main.WorldToScreenPoint(cannon.transform.position);
        RefreshUpgradeButton();
        RefreshTowerInfo();
    }

    void Start()
    {
        fireButton.onClick.AddListener(() => OnChangeType(TowerFactory.TowerType.Fire));
        iceButton.onClick.AddListener(() => OnChangeType(TowerFactory.TowerType.Ice));
        windButton.onClick.AddListener(() => OnChangeType(TowerFactory.TowerType.Wind));
        closeButton.onClick.AddListener(CloseUI);
        
        if (upgradeButton != null)
            upgradeButton.onClick.AddListener(OnUpgrade);
    }

    void OnChangeType(TowerFactory.TowerType type)
    {
        if (targetCannon == null || selector == null) return;
        
        selector.ChangeTowerType(type);
        
    }

    void RefreshUpgradeButton()
    {
        if (upgradeButton == null || upgradeCostText == null) return;

        if (targetCannon != null && targetCannon.CanUpgrade())
        {
            int cost = targetCannon.GetNextUpgradeCost();
            upgradeCostText.text = "Upgrade: $" + cost;
            upgradeButton.interactable = (GameManager.Instance.Money >= cost);
        }
        else if (targetCannon != null)
        {
            upgradeCostText.text = "Max Level";
            upgradeButton.interactable = false;
        }
        else
        {
            upgradeCostText.text = "";
            upgradeButton.interactable = false;
        }
    }

    void RefreshTowerInfo()
    {
        if (towerInfoText == null || targetCannon == null) return;

        TowerInfo towerInfo = targetCannon.GetComponentInParent<TowerInfo>();
        if (towerInfo != null)
        {
            towerInfoText.text = towerInfo.GetDescription();
        }
        else
        {
            towerInfoText.text = "Tower Info: Unknown";
        }
    }

    void OnUpgrade()
    {
        if (targetCannon == null) return;
        
        int cost = targetCannon.GetNextUpgradeCost();
        if (GameManager.Instance.Money >= cost)
        {
            GameManager.Instance.Money -= cost;
            targetCannon.Upgrade();
            RefreshUpgradeButton();
            RefreshTowerInfo(); 
        }
    }

    void CloseUI()
    {
        gameObject.SetActive(false);
        if (selector != null)
        {
            selector.CloseUI();
        }
    }
}