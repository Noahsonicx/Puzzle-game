using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyPotion : Potion
{
    public EnergyPotion()
    {
        itemName = "EnergyPotion";
        type = "Consumable";
        statAffected = "Energy";
        valueAffected = 10.0f;
    }
}
