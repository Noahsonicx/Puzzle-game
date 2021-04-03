using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemyMovement : MonoBehaviour
{
    public float CurrentVision => currentVision;
    public WorldElements[,] Enemy_Array => enemy_array;
    public (float, float) Location => location;

    private float max_health;
    private float current_health;
    public (float, float) location;
    public TextMeshProUGUI health_text;
    private WorldElements[,] enemy_array;

    //private float Mana;


    // I know this is bad but it works.

    // The state the enemies are in and how much vision they have in different states.
    private float passiveVision;
    private float aggressiveVision;
    private float currentVision;
    private float attackRange;

    // Tells if this Enemy is aggressive if it has seen the player.
    public bool aggressiveState; 
    private int aggressiveTimer;
 
    // Start is called before the first frame update
    void Start()
    {
        // Kierans AI stuff vision=3x3 or 5x5 and aggressive for 5 steps.
        aggressiveState = false;
        aggressiveTimer = 5;
        passiveVision = 3;
        aggressiveVision = 5;
        currentVision = passiveVision;

        // Attack Range = 3x3
        attackRange = 3;

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

    public void Fill_Enemy_array(WorldElements[,] en_Ar)
    {
        enemy_array = new WorldElements[(int)currentVision, (int)currentVision];
        enemy_array = en_Ar;
    }
    public string Take_Turn()
    {
        // This will get the current position on the enemy grid
        (float, float) enemy_position;
        enemy_position.Item1 = currentVision / 2 - 0.5f;
        enemy_position.Item2 = currentVision / 2 - 0.5f;

        // Will Cycle through each state until one is done

        // First Check if to attack
        if (CheckAttack(enemy_position))
        {
            return "Attack";
        }
        // Second Check to move if enemy is in sight
        string Movedirection = CheckPlayerInRange(enemy_position);
        if (Movedirection != "None")
        {
            return Movedirection;
        }
        // Third if neither the enemy will either rest or pass turn

        return "Pass";
    }
    public bool CheckAttack((float, float) _enemy_position)
    {
        // This will get the current position on the enemy grid

        (float, float) enemy_attackloacation;
        enemy_attackloacation.Item1 = _enemy_position.Item1 - ((attackRange / 2f) - 1f);
        enemy_attackloacation.Item2 = _enemy_position.Item2 + ((attackRange / 2f) - 1f);
        // Cycle through each location.
        for (int x = 0; x < attackRange; x++)            // This will be x  (x,y).
        {
            for (int y = (int)attackRange; y < 0; y--)   // This will be y  (x,y).
            {
                // If there is a player in this location this enemy will attack it.
                if (Enemy_Array[(int)enemy_attackloacation.Item1, (int)enemy_attackloacation.Item2].environment.gameObject.tag == "Player")
                {
                    // Will attack player
                    return true;
                }
                enemy_attackloacation.Item1++;
            }
            enemy_attackloacation.Item1 -= attackRange;
            enemy_attackloacation.Item2++;
        }
        return false;
    }

    public string CheckPlayerInRange((float, float) _enemy_position)
    {
        //// This will get the current position on the enemy grid
        //(float, float) enemy_loacation;
        //enemy_loacation.Item1 = 0f;
        //enemy_loacation.Item2 = 0f;
        //// Cycle through each location.
        //for (int x = 0; x < attackRange; x++)            // This will be x  (x,y).
        //{
        //    for (int y = (int)attackRange; y < 0; y--)   // This will be y  (x,y).
        //    {
        //        // If there is a player in this location this enemy will attack it.
        //        if (Enemy_Array[(int)enemy_attackloacation.Item1, (int)enemy_attackloacation.Item2].environment.gameObject.tag == "Player")
        //        {
        //            // Will attack player
        //            return true;
        //        }
        //        enemy_attackloacation.Item1++;
        //    }
        //    enemy_attackloacation.Item1 -= attackRange;
        //    enemy_attackloacation.Item2++;
        //}
        return "None";
    }
}
