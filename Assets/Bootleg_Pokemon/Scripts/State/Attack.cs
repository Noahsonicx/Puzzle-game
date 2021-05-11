using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : State 
{
    public GameObject target { get; set; }
    public Moveset move { get; set; }
    public Attack(GameObject _target, Moveset _move)
    {
        this.state_name = "Attack";
        this.target = _target;
        this.move = _move;
    }
}
