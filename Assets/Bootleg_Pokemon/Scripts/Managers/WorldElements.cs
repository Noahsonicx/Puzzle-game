using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldElements
{
    public GameObject environment;
    public GameObject item;

    private GameObject _character;
    public GameObject character
    {
        get
        {
            return _character;
        }
        set
        {
            _character = value;
        }
    }

    public WorldElements(GameObject empty)
    {
        environment = empty;
        item = empty;
        character = empty;
    }
}
