using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI AvailableFunds;
    [SerializeField] Transform ItemUIRoot;
    [SerializeField] GameObject ItemUIPrefab;
    [SerializeField] List<ShopItem> InventoryItems;

    IPurchaser CurrentPurchaser;
    ShopItem SelectedItem;
    Dictionary<ShopItem, InventoryUI_Item> InventoryItemToUIMap;

    private void Start()
    {
        // Initialize the purchaser and refresh the UI
        CurrentPurchaser = FindObjectOfType<Purchaser>();
        RefreshInventoryUI();
    }

    void RefreshInventoryUI()
    {
        // Clear existing UI elements
        for (int childIndex = ItemUIRoot.childCount - 1; childIndex >= 0; childIndex--)
        {
            var childGO = ItemUIRoot.GetChild(childIndex).gameObject;
            Destroy(childGO);
        }

        InventoryItemToUIMap = new Dictionary<ShopItem, InventoryUI_Item>();

        // Create UI elements for each purchased item
        foreach (var kvp in CurrentPurchaser.GetPurchasedItems())
        {
            var item = kvp.Key;
            var itemGO = Instantiate(ItemUIPrefab, ItemUIRoot);
            var itemUI = itemGO.GetComponent<InventoryUI_Item>();

            itemUI.Bind(item, OnItemSelected);
            InventoryItemToUIMap[item] = itemUI;
        }

        RefreshInventoryUI_Common();
    }

    void RefreshInventoryUI_Common()
    {
        // Update available funds display
        if (CurrentPurchaser != null)
            AvailableFunds.text = $"{(CurrentPurchaser.GetCurrentFunds() / 100f):0.00}";
        else
            AvailableFunds.text = string.Empty;

        // Update each item's affordability status
        foreach (var kvp in InventoryItemToUIMap)
        {
            var item = kvp.Key;
            var itemUI = kvp.Value;

            if (CurrentPurchaser != null)
                itemUI.SetCanAfford(item.Cost <= CurrentPurchaser.GetCurrentFunds());
            else
                itemUI.SetCanAfford(false);
        }
    }

    void OnItemSelected(ShopItem newlySelectedItem)
    {
        // Update the selected item and refresh the UI
        SelectedItem = newlySelectedItem;
        foreach (var kvp in InventoryItemToUIMap)
        {
            var item = kvp.Key;
            var itemUI = kvp.Value;

            itemUI.SetIsSelected(item == SelectedItem);
        }

        RefreshInventoryUI_Common();
    }

    public void OnClickedEquip()
    {
        // Equip the selected item and refresh the UI
        if (SelectedItem != null && CurrentPurchaser.EquipItem(SelectedItem))
        {
            RefreshInventoryUI();
        }
    }
}