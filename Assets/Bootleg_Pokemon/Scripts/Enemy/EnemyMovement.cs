using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemyMovement : MonoBehaviour
{
    private float max_health;
    private float current_health;
    private (float, float) location;
    public TextMeshProUGUI health_text;

    //private float Mana;
 
    // Start is called before the first frame update
    void Start()
    {
        max_health = GetComponent<Elemontals>().GetHealth(); // Could be modified if it scales by level
        current_health = max_health;
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
    public void TakeDamage(float dmg)
    {
        Debug.Log("Damage Taken: " + dmg);
        Debug.Log("Starting Health: " + current_health);
        current_health -= dmg;
        Debug.Log("Final Health: " + current_health);
        SetText();
        if (current_health <= 0) Destroy(this.gameObject);
    }
    public void SetLocation(float x, float y)
    {
        location.Item1 = x;
        location.Item2 = y;
    }
}
