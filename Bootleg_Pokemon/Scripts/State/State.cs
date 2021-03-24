using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State
{
    [SerializeField]
    protected string state_name;

    public string GetStateName()
    {
        return state_name;
    }
}
