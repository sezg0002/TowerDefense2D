using UnityEngine;

public class TowerSelection : MonoBehaviour
{
    public GameObject upgradeUIPrefab;
    private GameObject currentUI;
    
    private bool isSelectable = true;
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) 
        {
            HandleMouseClick();
        }
    }
    
    void HandleMouseClick()
    {
        if (currentUI != null && currentUI.activeSelf) return;
        
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f; 
        
        float detectionRadius = 1.0f; 
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(mousePos, detectionRadius);
        
        foreach (Collider2D hitCollider in hitColliders)
        {
            if (hitCollider != null && (hitCollider.gameObject == gameObject || 
                                       hitCollider.transform.IsChildOf(transform)))
            {
                OpenUpgradeUI();
                return;
            }
        }
    }
    
    void OpenUpgradeUI()
    {
        GameObject canvasObj = GameObject.Find("Canvas");
        Canvas mainCanvas = null;
        if (canvasObj != null)
        {
            mainCanvas = canvasObj.GetComponent<Canvas>();
        }

        if (mainCanvas == null)
        {
            Canvas[] allCanvas = FindObjectsOfType<Canvas>();
            foreach (Canvas c in allCanvas)
            {
                if (mainCanvas == null || c.sortingOrder > mainCanvas.sortingOrder)
                    mainCanvas = c;
            }
        }

        if (mainCanvas == null)
        {
            Debug.LogError("Aucun Canvas trouvé pour créer l'UI!");
            return;
        }

        currentUI = Instantiate(upgradeUIPrefab, mainCanvas.transform);
        currentUI.SetActive(true);

        RectTransform rt = currentUI.GetComponent<RectTransform>();
        if (rt != null)
        {
            rt.anchoredPosition = Vector2.zero;
            rt.SetAsLastSibling();
        }

        if (mainCanvas.GetComponent<UnityEngine.UI.GraphicRaycaster>() == null)
        {
            mainCanvas.gameObject.AddComponent<UnityEngine.UI.GraphicRaycaster>();
        }

        var cannon = GetComponentInChildren<CannonScript>();
        if (cannon == null)
        {
            Debug.LogError("Aucun CannonScript trouvé sur la tour !");
            return;
        }

        UpgradeUI upgradeUI = currentUI.GetComponent<UpgradeUI>();
        if (upgradeUI != null)
        {
            upgradeUI.SetTargetCannon(cannon, this);
        }
    }

    public void ChangeTowerType(TowerFactory.TowerType newType)
    {
        TowerFactory.TransformTower(gameObject, newType);
        
        Debug.Log("Tourelle transformée en : " + TowerFactory.GetTowerName(newType));

        CloseUI();
    }

    public void CloseUI()
    {
        if (currentUI != null) 
        {
            currentUI.SetActive(false);
            currentUI = null;
        }
    }
}