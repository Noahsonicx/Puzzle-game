using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walk : State
{
    [SerializeField]
    private string direction;

    public Walk(string dir)
    {
        this.state_name = "Walk";
        this.direction = dir;
    }

    public string GetDirection()
    {
        return this.direction;
    }
}
