using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HandMenu : MonoBehaviour
{
    [SerializeField]
    Canvas ShopMenu;
    [SerializeField]
    Canvas InventoryMenu;

    [SerializeField]
    InputActionProperty toggleMenuAction;
    [SerializeField]
    InputActionProperty switchMenuAction;

    bool isMenuToggled = false;
    bool isShopMenuActive = false;
    
    void Start()
    {
        ShopMenu.gameObject.SetActive(false);
        InventoryMenu.gameObject.SetActive(false);
    }

    void Update()
    {
        if (toggleMenuAction.action.WasPressedThisFrame()){
            isMenuToggled = !isMenuToggled;
            ShopMenu.gameObject.SetActive(isMenuToggled);
            InventoryMenu.gameObject.SetActive(isMenuToggled);

            if (isMenuToggled)
                if(isShopMenuActive){
                    InventoryMenu.gameObject.SetActive(false);
                }
                else{
                    ShopMenu.gameObject.SetActive(false);
                }
        }

        if (isMenuToggled == true && switchMenuAction.action.WasPressedThisFrame()){
            Debug.Log("Switching menu");
            isShopMenuActive = !isShopMenuActive;
            ShopMenu.gameObject.SetActive(isShopMenuActive);
            InventoryMenu.gameObject.SetActive(!isShopMenuActive);
        }
    }
}
