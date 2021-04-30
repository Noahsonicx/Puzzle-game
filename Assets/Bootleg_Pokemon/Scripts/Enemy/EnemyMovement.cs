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
    private Moveset[] attacks = new Moveset[1];

    //private float Mana;


    // I know this is bad but it works.

    // The state the enemies are in and how much vision they have.
    private float currentVision;
    private float attackRange;

 
    // Start is called before the first frame update
    void Start()
    {
        // Kierans AI stuff vision=5x5.
        currentVision = 5;

        // Attack Range = 3x3.
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
        print("Spawn enemy at ( x = " + x + ", y = " + y + ").");
    }

    // Will be called in the beginning of the enemy turn and will make a map for the enemy within vision.
    public void Fill_Enemy_array(WorldElements[,] en_Ar)
    {
        enemy_array = new WorldElements[(int)currentVision, (int)currentVision];
        enemy_array = en_Ar;
    }

    // This is called when an enemy takes a turn and will output the result for the enemy.
    // The options are: 
    //      "Attack"                                    - If the player IS in attack range of THIS enemy.
    //      "Move " + "Left" / "Down" / "Right" / "Up"  - If the player IS NOT in attack range, AND IS in vision range of THIS enemy, AND the enemy CAN move towards them).
    //      "Cant Move"                                 - If the player IS NOT in attack range, AND IS in vision range of THIS enemy, AND the enemy CAN NOT move towards them).
    //      "Pass"                                      - If the player IS NOT in attack range, AND IS NOT in vision range of THIS enemy.
    public string Take_Turn()
    {
        // This will get the current position on the enemy grid
        (float, float) enemy_position;
        enemy_position.Item1 = currentVision / 2 - 0.5f;
        enemy_position.Item2 = currentVision / 2 - 0.5f;

        // Will go through each state until one is done in order of:
        // Attack (check if player in attack range)
        // -> Move (check if player in vision range)
        // --> Pass (produces this if player is not in vision range or move range)

        // First check if to Attack
        if (CheckAttack(enemy_position))
        {
            return "Attack";
        }

        // Second check to move if enemy is in vision and to Move (or not if it cant move towards the player).
        string Movedirection = CheckPlayerInRange(enemy_position);
        if (Movedirection != "Player Not In Range")
        {
            return Movedirection;
        }

        // Third if neither the enemy will either rest or pass turn.
        return "Pass";
    }




    // This is called in Take_Turn.
    // This will be called to check if the player is in attack range of the enemy.
    private bool CheckAttack((float, float) _enemy_position)
    {
        // This will get the current position on the enemy grid.
        (float, float) enemy_attackloacation;
        enemy_attackloacation.Item1 = _enemy_position.Item1 - ((attackRange / 2f) - 0.5f);
        enemy_attackloacation.Item2 = _enemy_position.Item2 - ((attackRange / 2f) - 0.5f);
        // Cycle through each location in probably a 3x3 grid.
        for (int x = 0; x < attackRange; x++)           // This will be x  (x,y).
        {
            for (int y = 0; y < attackRange; y++)       // This will be y  (x,y).
            {
                if (Enemy_Array[(int)enemy_attackloacation.Item1, (int)enemy_attackloacation.Item2] != null)
                {
                    // If there is a player in this location this enemy will return true and will know it can attack it.
                    if (Enemy_Array[(int)enemy_attackloacation.Item1, (int)enemy_attackloacation.Item2].environment.gameObject.tag == "Player")
                    {
                        // Will attack player unless otherwise stated.
                        return true;
                    }
                }
                enemy_attackloacation.Item1++;
            }
            enemy_attackloacation.Item1 -= attackRange;
            enemy_attackloacation.Item2++;
        }
        return false;
    }



    // This is called in Take_Turn.
    // This will check if the player is in the enemy array and if so will move in the direction of the player.
    // It is split into:
        // 4 quatrants: (Bottom Left), (Bottom Right), (Top Left), (Top Right) 
        // 4 direct quatrants: (Bottom), (Left), (Right), (Top) 
    // If in none will say "Player Not In Range".
    private string CheckPlayerInRange((float, float) _enemy_position)
    {
        // This will get the current position of the the player on enemy grid
        (float, float) player_loacation;
        player_loacation.Item1 = 0f;
        player_loacation.Item2 = 0f;

        // This divides up the quadrant into 4 squares disreguarding exactly up and down, left and right.
        int quadrantSize = (int)((currentVision / 2f) - 0.5f);

        // Cycle through each quadrand and coloumn to see if the player is there and thus move to them.

        // Starting with bottom left.
        for (int x = 0; x < quadrantSize; x++)            // This will be x  (x,y).
        {
            for (int y = 0; y < quadrantSize; y++)   // This will be y  (x,y).
            {
                if (Enemy_Array[(int)player_loacation.Item1, (int)player_loacation.Item2] != null)
                {
                    // If there is a player in the bottom left location this enemy will move to it.
                    if (Enemy_Array[(int)player_loacation.Item1, (int)player_loacation.Item2].environment.gameObject.tag.Contains("Player"))
                    {
                        // Check location at left and down to be either an enemy or wall.
                        // If both free do one randomly if player below and left
                        if (IsLocationFreeToMove(_enemy_position.Item1 - 1f, _enemy_position.Item2) &&
                                IsLocationFreeToMove(_enemy_position.Item1, _enemy_position.Item2 - 1f))
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
                        // If below is blocked off.
                        else if (IsLocationFreeToMove(_enemy_position.Item1 - 1f, _enemy_position.Item2) &&
                                !IsLocationFreeToMove(_enemy_position.Item1, _enemy_position.Item2 - 1f))
                        {
                            return "Move Left";
                        }
                        // If left is blocked off.
                        else if (!IsLocationFreeToMove(_enemy_position.Item1 - 1f, _enemy_position.Item2) &&
                                IsLocationFreeToMove(_enemy_position.Item1, _enemy_position.Item2 - 1f))
                        {
                            return "Move Down";
                        }
                        // If it gets to here, the enemy can not get to the player (as there is probs a wall or enemy in the way) and will "Cant Move" (what ever that will be)(probs stay aggressive but pass).
                        else
                        {
                            return "Cant Move";
                        }

                    }
                }
                player_loacation.Item1++;
            }
            player_loacation.Item1 -= quadrantSize;
            player_loacation.Item2++;
        }

        // 0-0-0
        // 0-0-0
        // 1-0-0

        // Now check for directly below.
        player_loacation.Item1 = (quadrantSize);
        player_loacation.Item2 = (0f);
        // Now below the enemy.
        for (int y = 0; y < quadrantSize; y++)         // This will be y  (x,y).
        {
            if (Enemy_Array[(int)player_loacation.Item1, (int)player_loacation.Item2] != null)
            {
                // If there is a player in the bottom location this enemy will move to it.
                if (Enemy_Array[(int)player_loacation.Item1, (int)player_loacation.Item2].environment.gameObject.tag.Contains("Player"))
                {
                    // Check location down to be either an enemy or wall and if free it will move down.
                    if (IsLocationFreeToMove(_enemy_position.Item1, _enemy_position.Item2 - 1f))
                    {
                        return "Move Down";
                    }
                    // If it gets to here the enemy can not get to the player as there is probs a wall or enemy in the way and will "Cant Move" (what ever that will be)(probs stay aggressive but pass).
                    else
                    {
                        return "Cant Move";
                    }
                }
            }
            player_loacation.Item2++;
        }

        // 0-0-0
        // 0-0-0
        // 1-1-0

        // Now check for bottom right.
        player_loacation.Item1 = (quadrantSize + 1);
        player_loacation.Item2 = (0f);
        // Starting with bottom left square (of bottom right).
        for (int x = 0; x < quadrantSize; x++)              // This will be x  (x,y).
        {
            for (int y = 0; y < quadrantSize; y++)          // This will be y  (x,y).
            {
                if (Enemy_Array[(int)player_loacation.Item1, (int)player_loacation.Item2] != null)
                {
                    // If there is a player in the bottom right location this enemy will move to it.
                    if (Enemy_Array[(int)player_loacation.Item1, (int)player_loacation.Item2].environment.gameObject.tag.Contains("Player"))
                    {
                        // Check location at right and down to be either an enemy or wall.
                        // If both free do one randomly if player below and right
                        if (IsLocationFreeToMove(_enemy_position.Item1 + 1f, _enemy_position.Item2) &&
                            IsLocationFreeToMove(_enemy_position.Item1, _enemy_position.Item2 - 1f))
                        {
                            if (Random.value < .5)
                            {
                                return "Move Right";
                            }
                            else
                            {
                                return "Move Down";
                            }
                        }
                        // If enemy below
                        else if (!IsLocationFreeToMove(_enemy_position.Item1 + 1f, _enemy_position.Item2) &&
                                 IsLocationFreeToMove(_enemy_position.Item1, _enemy_position.Item2 - 1f))
                        {
                            return "Move Down";
                        }
                        else if (IsLocationFreeToMove(_enemy_position.Item1 + 1f, _enemy_position.Item2) &&
                                 !IsLocationFreeToMove(_enemy_position.Item1, _enemy_position.Item2 - 1f))
                        {
                            return "Move Right";
                        }
                        // If it gets to here the enemy can not get to the player as there is probs a wall or enemy in the way and will "Cant Move" (what ever that will be)(probs stay aggressive but pass).
                        else
                        {
                            return "Cant Move";
                        }

                    }
                }
                player_loacation.Item1++;
            }
            player_loacation.Item1 -= quadrantSize;
            player_loacation.Item2++;
        }

        // 0-0-0
        // 0-0-0
        // 1-1-1

        // Now check for directly left.
        player_loacation.Item1 = (0f);
        player_loacation.Item2 = (quadrantSize);
        // Now left of the enemy.
        for (int x = 0; x < quadrantSize; x++)         // This will be x  (x,y).
        {
            if (Enemy_Array[(int)player_loacation.Item1, (int)player_loacation.Item2] != null)
            {
                // If there is a player in the left location this enemy will move to it.
                if (Enemy_Array[(int)player_loacation.Item1, (int)player_loacation.Item2].environment.gameObject.tag.Contains("Player"))
                {
                    // Check location left to be either an enemy or wall and if free it will move left.
                    if (IsLocationFreeToMove(_enemy_position.Item1 - 1, _enemy_position.Item2))
                    {
                        return "Move Left";
                    }
                    // If it gets to here the enemy can not get to the player as there is probs a wall or enemy in the way and will "Cant Move" (what ever that will be)(probs stay aggressive but pass).
                    else
                    {
                        return "Cant Move";
                    }
                }
            }
            player_loacation.Item1++;
        }

        // 0-0-0
        // 1-0-0
        // 1-1-1

        // Now check for directly right.
        player_loacation.Item1 = (quadrantSize + 1);
        player_loacation.Item2 = (quadrantSize);
        // Now right of the enemy.
        for (int x = 0; x < quadrantSize; x++)         // This will be x  (x,y).
        {
            if (Enemy_Array[(int)player_loacation.Item1, (int)player_loacation.Item2] != null)
            {
                // If there is a player in the right location this enemy move to it.
                if (Enemy_Array[(int)player_loacation.Item1, (int)player_loacation.Item2].environment.gameObject.tag.Contains("Player"))
                {
                    // Check location right to be either an enemy or wall and if free it will move right.
                    if (IsLocationFreeToMove(_enemy_position.Item1 + 1, _enemy_position.Item2))
                    {
                        return "Move Right";
                    }
                    // If it gets to here the enemy can not get to the player as there is probs a wall or enemy in the way and will "Cant Move" (what ever that will be)(probs stay aggressive but pass).
                    else
                    {
                        return "Cant Move";
                    }
                }
            }
            player_loacation.Item1++;
        }

        // 0-0-0
        // 1-0-1
        // 1-1-1

        // Now check for top left.
        player_loacation.Item1 = (0f);
        player_loacation.Item2 = (quadrantSize + 1);
        // Starting with bottom left square (of top left).
        for (int x = 0; x < quadrantSize; x++)              // This will be x  (x,y).
        {
            for (int y = 0; y < quadrantSize; y++)          // This will be y  (x,y).
            {
                if (Enemy_Array[(int)player_loacation.Item1, (int)player_loacation.Item2] != null)
                {
                    // If there is a player in the top left location this enemy will move to it.
                    if (Enemy_Array[(int)player_loacation.Item1, (int)player_loacation.Item2].environment.gameObject.tag.Contains("Player"))
                    {
                        // Check location at left and up to be either an enemy or wall.
                        // If both free do one randomly if player up and left
                        if (IsLocationFreeToMove(_enemy_position.Item1 - 1f, _enemy_position.Item2) &&
                            IsLocationFreeToMove(_enemy_position.Item1, _enemy_position.Item2 + 1f))
                        {
                            if (Random.value < .5)
                            {
                                return "Move Left";
                            }
                            else
                            {
                                return "Move Up";
                            }
                        }
                        // If the Player is in this Quadrant BUT the left is blocked.
                        else if (!IsLocationFreeToMove(_enemy_position.Item1 - 1f, _enemy_position.Item2) &&
                                 IsLocationFreeToMove(_enemy_position.Item1, _enemy_position.Item2 + 1f))
                        {
                            return "Move Up";
                        }
                        // If the Player is in this Quadrant BUT the up is blocked.
                        else if (IsLocationFreeToMove(_enemy_position.Item1 - 1f, _enemy_position.Item2) &&
                                 !IsLocationFreeToMove(_enemy_position.Item1, _enemy_position.Item2 + 1f))
                        {
                            return "Move Left";
                        }
                        // If it gets to here the enemy can not get to the player as there is probs a wall or enemy in the way and will "Cant Move" (what ever that will be)(probs stay aggressive but pass).
                        else
                        {
                            return "Cant Move";
                        }

                    }
                }
                player_loacation.Item1++;
            }
            player_loacation.Item1 -= quadrantSize;
            player_loacation.Item2++;
        }

        // 1-0-0
        // 1-0-1
        // 1-1-1

        // Now check for directly above.
        player_loacation.Item1 = (quadrantSize);
        player_loacation.Item2 = (quadrantSize + 1);
        // Now above of the enemy.
        for (int y = 0; y < quadrantSize; y++)         // This will be y  (x,y).
        {
            if (Enemy_Array[(int)player_loacation.Item1, (int)player_loacation.Item2] != null)
            {
                // If there is a player in the above location this enemy move to it.
                if (Enemy_Array[(int)player_loacation.Item1, (int)player_loacation.Item2].environment.gameObject.tag.Contains("Player"))
                {
                    // Check location above to be either an enemy or wall and if free it will move up.
                    if (IsLocationFreeToMove(_enemy_position.Item1 + 1, _enemy_position.Item2 + 1))
                    {
                        return "Move Up";
                    }
                    // If it gets to here the enemy can not get to the player as there is probs a wall or enemy in the way and will "Cant Move" (what ever that will be)(probs stay aggressive but pass).
                    else
                    {
                        return "Cant Move";
                    }
                }
            }
            player_loacation.Item2++;
        }

        // 1-1-0
        // 1-0-1
        // 1-1-1

        // Now check for top right.
        player_loacation.Item1 = (quadrantSize + 1);
        player_loacation.Item2 = (quadrantSize + 1);
        // Starting with bottom left square (of top right).
        for (int x = 0; x < quadrantSize; x++)              // This will be x  (x,y).
        {
            for (int y = 0; y < quadrantSize; y++)          // This will be y  (x,y).
            {
                if (Enemy_Array[(int)player_loacation.Item1, (int)player_loacation.Item2] != null)
                {
                    // If there is a player in the top left location this enemy will move to it.
                    if (Enemy_Array[(int)player_loacation.Item1, (int)player_loacation.Item2].environment.gameObject.tag.Contains("Player"))
                    {
                        // Check location at right and up to be either an enemy or wall.
                        // If both free do one randomly if player up and right
                        if (IsLocationFreeToMove(_enemy_position.Item1 + 1f, _enemy_position.Item2) &&
                            IsLocationFreeToMove(_enemy_position.Item1, _enemy_position.Item2 + 1f))
                        {
                            if (Random.value < .5)
                            {
                                return "Move Right";
                            }
                            else
                            {
                                return "Move Up";
                            }
                        }
                        // If the Player is in this Quadrant BUT the right is blocked.
                        else if (!IsLocationFreeToMove(_enemy_position.Item1 + 1f, _enemy_position.Item2) &&
                                 IsLocationFreeToMove(_enemy_position.Item1, _enemy_position.Item2 + 1f))
                        {
                            return "Move Up";
                        }
                        // If the Player is in this Quadrant BUT the up is blocked.
                        else if (IsLocationFreeToMove(_enemy_position.Item1 + 1f, _enemy_position.Item2) &&
                                 !IsLocationFreeToMove(_enemy_position.Item1, _enemy_position.Item2 + 1f))
                        {
                            return "Move Right";
                        }
                        // If it gets to here the enemy can not get to the player as there is probs a wall or enemy in the way and will "Cant Move" (what ever that will be)(probs stay aggressive but pass).
                        else
                        {
                            return "Cant Move";
                        }

                    }
                }
                player_loacation.Item1++;
            }
            player_loacation.Item1 -= quadrantSize;
            player_loacation.Item2++;
        }

        // 1-1-1
        // 1-0-1
        // 1-1-1

        // If the player isn't in range it will say this.
        return "Player Not In Range";
    }

    // Only called in "CheckPlayerInRange"
    private bool IsLocationFreeToMove(float _check_position_x, float _check_position_y)
    {
        // Legit just wrote this as it was becoming too tedious writing this out 50+ times.
        // Basically if the input square is free to move it will output true, if not it will be false.
        if (Enemy_Array[(int)(_check_position_x), (int)_check_position_y] == null &&
            !Enemy_Array[(int)(_check_position_x), (int)_check_position_y].environment.gameObject.tag.Contains("Wall") ||
            !Enemy_Array[(int)(_check_position_x), (int)_check_position_y].environment.gameObject.tag.Contains("Enemy"))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}