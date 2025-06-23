using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuildMenuItemScript : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerUpHandler, IPointerExitHandler
{
    public Sprite BaseSprite;
    public Sprite HoverSprite;
    public Sprite ClickSprite;
    public GameObject Prototype;
    
    [Header("Factory Configuration")]
    public TowerFactory.TowerType towerType = TowerFactory.TowerType.Basic;
    public bool useFactory = true; 

    private Text text;
    private Text price;
    private GameObject parent;
    private GameObject rangeSprite;
    private float range;
    private Image image;
    private bool pressed = false;
    private bool disabled = false;

    void Update()
    {
        var enoughMoney = GameManager.Instance.EnoughMoneyForTurret(Prototype.tag);
        
        int cost = useFactory ? TowerFactory.GetTowerBaseCost(towerType) : GameManager.Instance.MoneyForTurret(Prototype.tag);
        price.text = "$" + cost;

        if(disabled && enoughMoney)
        {
            disabled = false;
            text.color = new Color(0, 0, 0, 1);
            price.color = new Color(0, 0, 0, 1);
        }
        else if(!disabled && !enoughMoney)
        {
            disabled = true;
            text.color = new Color(0, 0, 0, 0.25f);
            price.color = new Color(0, 0, 0, 0.25f);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (disabled || pressed) return;

        gameObject.transform.Translate(0, -3f, 0);
        image.sprite = ClickSprite;
        pressed = true;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Update();
        if (disabled) return;

        image.sprite = HoverSprite;
        rangeSprite.SetActive(true);
        rangeSprite.transform.localScale = new Vector3(16 * range, 16 * range, 1);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        rangeSprite.SetActive(false);

        if (disabled) return;

        image.sprite = BaseSprite;

        if (!pressed) return;
        
        gameObject.transform.Translate(0, 3f, 0);
        pressed = false;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (disabled || !pressed) return;
       
        GameObject instance;
        
        if (useFactory)
        {
            instance = TowerFactory.CreateTower(towerType, parent.transform.position);
            
            int cost = TowerFactory.GetTowerBaseCost(towerType);
            GameManager.Instance.Money -= cost;
            
            Debug.Log("Tourelle créée via Factory: " + TowerFactory.GetTowerName(towerType));
        }
        else
        {
            instance = Instantiate(Prototype, parent.transform.position, Quaternion.identity);
            GameManager.Instance.TurretBuilt(instance);
        }

        rangeSprite.SetActive(false);
        parent.SetActive(false);
    }

    void Start()
    {
        parent = GetComponentInParent<BuildLocationScript>().gameObject;
        image = GetComponent<Image>();
        text = transform.Find("Name").gameObject.GetComponent<Text>();
        price = transform.Find("Price").gameObject.GetComponent<Text>();
        rangeSprite = parent.transform.Find("Range").gameObject;
        
        if (Prototype != null)
        {
            var cannonScript = Prototype.transform.Find("Cannon");
            if (cannonScript != null)
            {
                range = cannonScript.GetComponent<CannonScript>().RangeValue;
            }
        }
        
        if (useFactory && text != null)
        {
            text.text = TowerFactory.GetTowerName(towerType);
        }
    }
}