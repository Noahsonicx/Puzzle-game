using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPotion : Potion
{
    public HealthPotion()
    {
        itemName = "HealthPotion";
        type = "Consumable";
        statAffected = "Health";
        valueAffected = 15.0f;
    }
}
