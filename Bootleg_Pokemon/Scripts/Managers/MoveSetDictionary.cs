using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSetDictionary : MonoBehaviour
{
    private List<Moveset> move_list = new List<Moveset>();
    private Dictionary<string, Moveset> move_dictionary = new Dictionary<string, Moveset>();
    private bool load_status = false;
    // Start is called before the first frame update
    void Start()
    {
        //Make some moves here. TODO: Will implement reading from text

        Moveset move1 = new Moveset("Blaze", "Fire", "Enemy", "Health", 10.0f, 5.0f);
        Moveset move2 = new Moveset("Pound", "Normal", "Enemy", "Health", 5.0f, 3.0f);
        Moveset move3 = new Moveset("Bless", "Normal", "Self", "Health", 20.0f, 10.0f);
        Moveset move4 = new Moveset("Fire Dance", "Fire", "Enemy", "Defence", 3.0f, 5.0f);

        // Add to List first
        move_list.Add(move1);
        move_list.Add(move2);
        move_list.Add(move3);
        move_list.Add(move4);

        //After making the moves add to dictionary

        foreach(Moveset m in move_list)
        {
            move_dictionary.Add(m.GetMoveName(), m);
        }

        load_status = true;
    }

    public bool GetDictionaryReadyStatus()
    {
        return load_status;
    }
    public Moveset GetMoveset(string key)
    {
        if (!move_dictionary.ContainsKey(key)) return null;
        return move_dictionary[key];
    }
    public Dictionary<string, Moveset> GetMoveDictionary()
    {
        return move_dictionary;
    }
}
