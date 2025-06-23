using UnityEngine;

public class TowerVisuals : MonoBehaviour
{
    [SerializeField] private SpriteRenderer baseRenderer;
    [SerializeField] private SpriteRenderer cannonRenderer;
    
    [Header("Fire Tower")]
    [SerializeField] private Sprite fireBaseSprite;
    [SerializeField] private Sprite fireCannonSprite;
    
    [Header("Ice Tower")]
    [SerializeField] private Sprite iceBaseSprite;
    [SerializeField] private Sprite iceCannonSprite;
    
    [Header("Wind Tower")]
    [SerializeField] private Sprite windBaseSprite;
    [SerializeField] private Sprite windCannonSprite;
    
    [Header("Basic Tower")]
    [SerializeField] private Sprite basicBaseSprite;
    [SerializeField] private Sprite basicCannonSprite;

    public void SetVisuals(TowerType type)
    {
        switch (type)
        {
            case TowerType.Fire:
                baseRenderer.sprite = fireBaseSprite;
                cannonRenderer.sprite = fireCannonSprite;
                break;
            case TowerType.Ice:
                baseRenderer.sprite = iceBaseSprite;
                cannonRenderer.sprite = iceCannonSprite;
                break;
            case TowerType.Wind:
                baseRenderer.sprite = windBaseSprite;
                cannonRenderer.sprite = windCannonSprite;
                break;
            default:
                baseRenderer.sprite = basicBaseSprite;
                cannonRenderer.sprite = basicCannonSprite;
                break;
        }
    }
}