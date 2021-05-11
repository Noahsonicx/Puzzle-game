using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Elemontals : MonoBehaviour
{
    public struct abilities
    {
        int level {get;}
        int abilityName {get;}
    }
    [SerializeField]
    private string elemontal_name;
    [SerializeField]
    private float healthPoints;
    [SerializeField]
    protected float energyPoints;
    [SerializeField]
    public List<string> listOfAllPossibleAbilities;
    [SerializeField]
    public List<string> startingAbilities;

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
    public string GetStartingMoves()
    {
        string moveLine = "";
        moveLine += startingAbilities[0] + "," + startingAbilities[1] + "," + startingAbilities[2] + "," + startingAbilities[3];

        return moveLine;
    }
}
