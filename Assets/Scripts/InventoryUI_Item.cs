using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUI_Item : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI ItemName;
    [SerializeField] TextMeshProUGUI ItemCount;
    [SerializeField] Image ItemImage;
    [SerializeField] Button ItemButton;

    ShopItem item;
    System.Action<ShopItem> onItemSelected;

    public void Bind(ShopItem item, System.Action<ShopItem> onItemSelected)
    {
        this.item = item;
        this.onItemSelected = onItemSelected;

        ItemName.text = item.Name;

        ItemButton.onClick.AddListener(() => onItemSelected(item));
    }

    public void SetCanAfford(bool canAfford)
    {
        // Update UI to reflect affordability
        ItemButton.interactable = canAfford;
    }

    public void SetIsSelected(bool isSelected)
    {
        // Update UI to reflect selection state
        // For example, change the background color
        GetComponent<Image>().color = isSelected ? Color.yellow : Color.white;
    }
}