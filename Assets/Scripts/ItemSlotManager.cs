using UnityEngine;
using System;
using UnityEngine.InputSystem;
using Unity.VisualScripting;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class ItemSlotManager : MonoBehaviour
{
    public PlayerManager playerManager;
    public string itemType;
    public string itemFileLocation;
    public int level;
    public string[] effects;
    public TextMeshProUGUI displayName;
    private UnityEngine.UI.Image image;
    public string id;
    public GameObject itemObject;

    void Start()
    {
        displayName.text = itemFileLocation;
        image = GetComponent<UnityEngine.UI.Image>();
        itemObject = Resources.Load<GameObject>(itemFileLocation);
        
    }
    void Update()
    {
        Vector2 localMousePosition = image.rectTransform.InverseTransformPoint(Input.mousePosition);
        if (playerManager.inputManager.attack_Input && image.rectTransform.rect.Contains(localMousePosition))
        {
            playerManager.inventoryManager.SwapAnItem(gameObject.GetComponent<ItemSlotManager>());
        }
        
    }
}
