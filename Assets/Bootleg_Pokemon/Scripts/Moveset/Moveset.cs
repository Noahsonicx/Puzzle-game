using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moveset
{
    private string move_name;
    private string element;
    private string target;
    private string stat_affected;
    private float value;
    private float mana_cost;

    public Moveset(string _name, string _element, string _target, string _stat_affected, float _value, float _mana_cost)
    {
        move_name = _name;
        element = _element;
        target = _target;
        stat_affected = _stat_affected;
        value = _value;
        mana_cost = _mana_cost;
    }

    public string GetMoveName()
    {
        return move_name;
    }

    public string GetElement()
    {
        return element;
    }
    public string GetTarget()
    {
        return target;
    }
    public string GetStatAffected()
    {
        return stat_affected;
    }
    public float GetValue()
    {
        return value;
    }
    public float GetManaCost()
    {
        return mana_cost;
    }
}
