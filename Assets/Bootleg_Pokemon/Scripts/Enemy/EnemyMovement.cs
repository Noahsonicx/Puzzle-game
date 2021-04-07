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
            // think this should be for "(int y = 0; y <= attackRange; y++)"
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
        // This will get the current position of the the player on enemy grid
        (float, float) player_loacation;
        player_loacation.Item1 = 0f;
        player_loacation.Item2 = 0f;

        // This divides up the quadrant into 4 squares disreguarding exactly up and down, left and right.
        int quadrantSize = (int)((currentVision / 2f)-0.5f);

        // Cycle through each quadrand and coloumn to see if the player is there and thus move to them.

        // Starting with bottom left.
        for (int x = 0; x < quadrantSize; x++)            // This will be x  (x,y).
        {
            for (int y = 0; y < quadrantSize; y++)   // This will be y  (x,y).
            {
                // If there is a player in the bottom left location this enemy will attack it.
                if (Enemy_Array[(int)player_loacation.Item1, (int)player_loacation.Item2].environment.gameObject.tag.Contains("Player"))
                {
                    // Check location at left and down to be either an enemy or wall.
                    // If both free do one randomly if enemy below or left
                    if ((Enemy_Array[(int)(_enemy_position.Item1 - 1), (int)_enemy_position.Item2] == null &&
                        !Enemy_Array[(int)(_enemy_position.Item1 - 1), (int)_enemy_position.Item2].environment.gameObject.tag.Contains("Wall") ||
                        !Enemy_Array[(int)(_enemy_position.Item1 - 1), (int)_enemy_position.Item2].environment.gameObject.tag.Contains("Enemy")) &&
                        (Enemy_Array[(int)(_enemy_position.Item1), (int)_enemy_position.Item2 - 1] == null &&
                        !Enemy_Array[(int)(_enemy_position.Item1), (int)_enemy_position.Item2 - 1].environment.gameObject.tag.Contains("Wall") ||
                        !Enemy_Array[(int)(_enemy_position.Item1), (int)_enemy_position.Item2 - 1].environment.gameObject.tag.Contains("Enemy")))
                    {
                        if (Random.value < .5)
                        {
                            return "Move Left";
                        }
                        else
                        {
                            return "Move Down";
                        }
                    }
                    // If enemy below
                    else if ((Enemy_Array[(int)(_enemy_position.Item1 - 1), (int)_enemy_position.Item2] == null &&
                        !Enemy_Array[(int)(_enemy_position.Item1 - 1), (int)_enemy_position.Item2].environment.gameObject.tag.Contains("Wall") ||
                        !Enemy_Array[(int)(_enemy_position.Item1 - 1), (int)_enemy_position.Item2].environment.gameObject.tag.Contains("Enemy")) &&
                        (Enemy_Array[(int)(_enemy_position.Item1), (int)_enemy_position.Item2 - 1] != null &&
                        Enemy_Array[(int)(_enemy_position.Item1), (int)_enemy_position.Item2 - 1].environment.gameObject.tag.Contains("Wall") ||
                        Enemy_Array[(int)(_enemy_position.Item1), (int)_enemy_position.Item2 - 1].environment.gameObject.tag.Contains("Enemy")))
                    {
                        return "Move Left";
                    }
                    else if ((Enemy_Array[(int)(_enemy_position.Item1 - 1), (int)_enemy_position.Item2] != null &&
                        Enemy_Array[(int)(_enemy_position.Item1 - 1), (int)_enemy_position.Item2].environment.gameObject.tag.Contains("Wall") ||
                        Enemy_Array[(int)(_enemy_position.Item1 - 1), (int)_enemy_position.Item2].environment.gameObject.tag.Contains("Enemy")) &&
                        (Enemy_Array[(int)(_enemy_position.Item1), (int)_enemy_position.Item2 - 1] == null &&
                        !Enemy_Array[(int)(_enemy_position.Item1), (int)_enemy_position.Item2 - 1].environment.gameObject.tag.Contains("Wall") ||
                        !Enemy_Array[(int)(_enemy_position.Item1), (int)_enemy_position.Item2 - 1].environment.gameObject.tag.Contains("Enemy")))
                    {
                        return "Move Down";
                    }
                    // If it gets to here the enemy can not get to the player as there is probs a wall or enemy in the way and will pass (what ever that will be)
                    else
                    {
                        return "Pass";
                    }

                }
                player_loacation.Item1++;
            }
            player_loacation.Item1 -= quadrantSize;
            player_loacation.Item2++;
        }


        // Now check for directly below.

        player_loacation.Item1 = (quadrantSize);
        player_loacation.Item2 = (0f);
        
        // Now below the enemy.
            for (int y = 0; y <= quadrantSize; y++)         // This will be y  (x,y).
            {
                // If there is a player in the bottom left location this enemy will attack it.
                if (Enemy_Array[(int)player_loacation.Item1, (int)player_loacation.Item2].environment.gameObject.tag.Contains("Player"))
                {
                    // Check location down to be either an enemy or wall and if free it will move down.
                    if ((Enemy_Array[(int)(_enemy_position.Item1), (int)_enemy_position.Item2 - 1] == null &&
                        !Enemy_Array[(int)(_enemy_position.Item1), (int)_enemy_position.Item2 - 1].environment.gameObject.tag.Contains("Wall") ||
                        !Enemy_Array[(int)(_enemy_position.Item1), (int)_enemy_position.Item2 - 1].environment.gameObject.tag.Contains("Enemy")))
                    {
                            return "Move Down";
                    }
                    // If it gets to here the enemy can not get to the player as there is probs a wall or enemy in the way and will pass (what ever that will be)
                    else
                    {
                        return "Pass";
                    }

                }
                player_loacation.Item2++;
            }
        }




        return "None";
    }
}
