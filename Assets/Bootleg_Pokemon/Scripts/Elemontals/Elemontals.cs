using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elemontals : MonoBehaviour
{
    [SerializeField]
    protected string elemontal_name;
    [SerializeField]
    protected float healthPoints;
    [SerializeField]
    protected float energyPoints;
    [SerializeField]
    protected Moveset[] listOfMoves; 

    public string GetName()
    {
        return elemontal_name;
    }
}
