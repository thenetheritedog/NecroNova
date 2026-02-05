using NUnit.Framework;
using Unity.Collections;
using UnityEngine;


public class InventoryManager : MonoBehaviour , IDataPersistence
{
    [SerializeField] private PlayerManager playerManager;
    public GameObject itemPrefab;
    public ItemSlotManager currentWeapon;
    [SerializeField] private string id;
    private string idOfItems;
    [ContextMenu("Generate guid for id")]
    private void GenerateGuid()
    {
        id = System.Guid.NewGuid().ToString();
    }
    void Awake()
    {
    }


    public void SwapAnItem(ItemSlotManager itemSlotManager) 
    {
        switch (itemSlotManager.itemType)
        {
            case ("weapon"):
                currentWeapon = itemSlotManager;
                Debug.Log(currentWeapon.itemType);
                break;
            default:
                Debug.Log("gang what te hell are you doing");
                break;
        }
    }
    public void AddItemToInventory(int level, string fileLocation, string[] effects, string id)
    {
        GameObject newItem = Instantiate(itemPrefab);
        ItemSlotManager newItemStats = newItem.GetComponent<ItemSlotManager>();
        newItemStats.level = level;
        newItemStats.itemFileLocation = fileLocation;
        newItemStats.effects = effects;
        newItem.transform.SetParent(playerManager.menu.transform, true);
        newItemStats.playerManager = playerManager;
        newItemStats.id = id;
    }
     public void SaveData(ref GameData data)
     {
        ItemSlotManager[] allItems = FindObjectsOfType<ItemSlotManager>();
        idOfItems = "";
        for (int i = 0; i < allItems.Length; i++)
        {
            string id = allItems[i].id;
            string combinedEffects = "";
            string properties;
            for (int j = 0; j < allItems[i].effects.Length; j++)
            {
                combinedEffects = new string(combinedEffects + ";" + allItems[i].effects[j]);
            }
            properties = new string(allItems[i].level + ";" + allItems[i].itemFileLocation + combinedEffects);
            if (data.itemsInInventory.ContainsKey(id))
            {
                data.itemsInInventory.Remove(id);
            }
            data.itemsInInventory.Add(id, properties);
            idOfItems = new (idOfItems + ";" + id);
        }
        if (idOfItems != "") 
        {
            idOfItems = idOfItems.Remove(0, 1);
        }
        if (data.inventoryAndItemIDs.ContainsKey(id))
        {
            data.inventoryAndItemIDs.Remove(id);
        }
        data.inventoryAndItemIDs.Add(id, idOfItems);
    }
    public void LoadData(GameData data)
    {
        string idOfItems;
        data.inventoryAndItemIDs.TryGetValue(id, out idOfItems);
        if (idOfItems == null)
        {
            return;
        }
        string[] idGiven = idOfItems.Split(';');
        if (idGiven[0] == "")
            return;
        for (int i = 0; i < idGiven.Length; i++)
        {
            string itemValuesCombined;
            data.itemsInInventory.TryGetValue(idGiven[i], out itemValuesCombined);
            string[] itemValuesUncombined = itemValuesCombined.Split(';');
            string[] effects = new string[itemValuesUncombined.Length-2];
            for (int j = 0; j < effects.Length; j++)
            {
                effects[j] = itemValuesUncombined[j+2];
            }
            AddItemToInventory(int.Parse(itemValuesUncombined[0]), itemValuesUncombined[1], effects, idGiven[i]);
        }
    }
}
