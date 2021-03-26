using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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

    public TextMeshProUGUI health_text;
    public TextMeshProUGUI energy_text;

    public string GetName()
    {
        return elemontal_name;
    }
    public float GetHealth()
    {
        return healthPoints;
    }
    public float GetEnergy()
    {
        return energyPoints;
    }
    public void ChangeText(string _text)
    {
        health_text.text = _text;
    }
}
