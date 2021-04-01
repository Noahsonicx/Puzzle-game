using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemyMovement : MonoBehaviour
{
    private float max_health;
    private float current_health;
    public (float, float) location;
    public TextMeshProUGUI health_text;

    //private float Mana;

    public void SetMaxHealth(float _max)
    {
        max_health = _max;
    }
    public float GetMaxHealth()
    {
        return max_health;
    }
    public void SetCurrentHealth(float _curr)
    {
        current_health = _curr;
    }
    public float GetCurrentHealth()
    {
        return current_health;
    }
    public (float,float) GetLocation()
    {
        return location;
    }
    // Start is called before the first frame update
    void Start()
    {
        max_health = GetComponent<Elemontals>().GetHealth(); // Could be modified if it scales by level
        health_text = GetComponent<Elemontals>().health_text;
    }

    // Update is called once per frame
    void Update()
    {
        health_text.text = current_health + "/" + max_health;
    }
    public void SetText()
    {
        GetComponent<Elemontals>().ChangeText(current_health + "/" + max_health);
    }
    public bool TakeDamage(float dmg)
    {
        Debug.Log("Damage Taken: " + dmg);
        Debug.Log("Starting Health: " + current_health);
        current_health -= dmg;
        Debug.Log("Final Health: " + current_health);
        SetText();
        if (current_health <= 0) return true;
        return false;
    }
    public void SetLocation(float x, float y)
    {
        location.Item1 = x;
        location.Item2 = y;
    }
}
