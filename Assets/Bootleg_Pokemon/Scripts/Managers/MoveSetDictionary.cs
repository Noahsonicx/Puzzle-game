using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSetDictionary : MonoBehaviour
{
    public List<Moveset> move_list = new List<Moveset>();
    private Dictionary<string, Moveset> move_dictionary = new Dictionary<string, Moveset>();
    private bool load_status = false;
    // Start is called before the first frame update
    void Start()
    {
        //Make some moves here. TODO: Will implement reading from text

        Moveset move0 = new Moveset("Empty", "Empty", "Empty", "Empty", 0, 0);
        Moveset move1 = new Moveset("Blaze", "Fire", "Enemy", "Health", 5.0f, 5.0f);
        Moveset move2 = new Moveset("Pound", "Normal", "Enemy", "Health", 5.0f, 3.0f);
        Moveset move3 = new Moveset("Bless", "Normal", "Self", "Health", 20.0f, 10.0f);
        Moveset move4 = new Moveset("Fire Dance", "Fire", "Enemy", "Defence", 3.0f, 5.0f);
        Moveset move5 = new Moveset("Fullever Blaze", "Fire", "Enemy", "Health", 12.0f, 10.0f);
        Moveset move6 = new Moveset("One For All", "Normal", "Enemy", "Health", 15.0f, 17.0f);
        Moveset move7 = new Moveset("Ice Wall", "Water", "Enemy", "Defence", 3.0f, 5.0f);
        Moveset move8 = new Moveset("Tidal Wave", "Water", "Enemy", "Health", 12.0f, 10.0f);
        Moveset move9 = new Moveset("Ice Shot", "Water", "Enemy", "Health", 6.0f, 5.0f);
        Moveset move10 = new Moveset("Water Whip", "Water", "Enemy", "Health", 3.0f, 2.0f);
        Moveset move11 = new Moveset("Fire Blast", "Fire", "Enemy", "Health", 6.0f, 5.0f);
        Moveset move12 = new Moveset("Earth Tomb", "Earth", "Enemy", "Defence", 3.0f, 5.0f);
        Moveset move13 = new Moveset("Plate Shot", "Earth", "Enemy", "Health", 6.0f, 5.0f);
        Moveset move14 = new Moveset("Rock Barrage", "Earth", "Enemy", "Health", 3.0f, 2.0f);
        Moveset move15 = new Moveset("Full Earthquake", "Earth", "Enemy", "Health", 12.0f, 10.0f);
        Moveset move16 = new Moveset("Fury Wind", "Wind", "Enemy", "Health", 3.0f, 2.0f);
        Moveset move17 = new Moveset("Air Bubble", "Wind", "Enemy", "Defence", 3.0f, 5.0f);
        Moveset move18 = new Moveset("Air Sphere Shot", "Wind", "Enemy", "Health", 6.0f, 5.0f);
        Moveset move19 = new Moveset("Wind Tunnel Blast", "Wind", "Enemy", "Health", 12.0f, 10.0f);
        Moveset move20 = new Moveset("Fist Barrage", "Normal", "Enemy", "Health", 3.0f, 2.0f);

        // Add to List first
        move_list.Add(move0);
        move_list.Add(move1);
        move_list.Add(move2);
        move_list.Add(move3);
        move_list.Add(move4);
        move_list.Add(move5);
        move_list.Add(move6);
        move_list.Add(move7);
        move_list.Add(move8);
        move_list.Add(move9);
        move_list.Add(move10);
        move_list.Add(move11);
        move_list.Add(move12);
        move_list.Add(move13);
        move_list.Add(move14);
        move_list.Add(move15);
        move_list.Add(move16);
        move_list.Add(move17);
        move_list.Add(move18);
        move_list.Add(move19);
        move_list.Add(move20);


        //After making the moves add to dictionary

        foreach (Moveset m in move_list)
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
