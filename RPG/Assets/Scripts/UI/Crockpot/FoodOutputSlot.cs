﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FoodOutputSlot : MonoBehaviour
{
    public CrockpotUI crockpotUI;

    public Food food;
    public Text itemCountText;
    public Image icon;
    public Button button;


    public void Start() {
        button = GetComponentInChildren<Button>();

        button.onClick.AddListener(AddResultToInventory);
    }


    public void AddFood(Food newFood) {
        food = newFood;

        icon.enabled = true;
        icon.sprite = food.icon;

        itemCountText.enabled = false;
    }

    public void ClearSlot() {
        food = null;

        icon.enabled = false;
        icon.sprite = null;

        itemCountText.enabled = false;
    }

    public void AddResultToInventory() {
        if(food != null ){
            if(Inventory.instance.items.Count < Inventory.instance.space) {
                Inventory.instance.AddItem(food);
                crockpotUI.output = null;
                ClearSlot();
                if (crockpotUI.onItemChangedCallback != null) {
                    crockpotUI.onItemChangedCallback.Invoke();
                }
            }
        }
    }
}
