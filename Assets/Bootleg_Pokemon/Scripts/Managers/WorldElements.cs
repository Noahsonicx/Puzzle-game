using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldElements
{
    public GameObject environment;
    public GameObject item;
    public GameObject character;

    public WorldElements(GameObject empty)
    {
        environment = empty;
        item = empty;
        character = empty;
    }
}
