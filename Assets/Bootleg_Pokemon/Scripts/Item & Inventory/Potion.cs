using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion :  Item
{
    [SerializeField]
    protected string statAffected;
    [SerializeField]
    protected float valueAffected;
    
    public string GetStatAffected() 
    {
        return statAffected;
    }
    public float GetValueAffected()
    {
        return valueAffected;
    }
}
