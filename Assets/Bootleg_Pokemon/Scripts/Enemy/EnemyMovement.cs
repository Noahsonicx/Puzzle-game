using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemyMovement : MonoBehaviour
{
    public float CurrentVision => currentVision;
    public (float, float) Location => location;

    private float max_health;
    private float current_health;
    public (float, float) location;
    public TextMeshProUGUI health_text;

    private WorldElements[,] enemy_Array;
    private Moveset[] attacks = new Moveset[4];

    private GameObject EmptyObject;

    // The state the enemies are in and how much vision they have.
    private float currentVision;
    private float attackRange;

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
        EmptyObject = GameObject.Find("EmptyObject");

        // Kierans AI stuff vision=5x5.
        currentVision = 5;

        // Attack Range = 3x3.
        attackRange = 3;

        max_health = GetComponent<Elemontals>().GetHealth(); // Could be modified if it scales by level
        current_health = max_health;
        health_text = GetComponent<Elemontals>().health_text;

        enemy_Array = new WorldElements[(int)currentVision, (int)currentVision];
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

    // Will be called in the beginning of the enemy turn and will make a map for the enemy within vision.
    public void Fill_Enemy_array(WorldElements[,] en_Ar)
    {
        Debug.Log("x size of en_Ar:" + en_Ar.GetLength(0));
        Debug.Log("y size of en_Ar:" + en_Ar.GetLength(1));
        Debug.Log("Current Vision:" + currentVision);

        for (int x = 0; x < currentVision; x++)
        {
            for (int y = 0; y < currentVision; y++)
            {
                enemy_Array[x, y] = en_Ar[x, y];
                if (en_Ar[x, y] == null)
                {
                    Debug.Log("Element is null when copying");
                }
            }
        }

        PrintMatrix(enemy_Array);
    }

    // This is called when an enemy takes a turn and will output the result for the enemy.
    // The options are: 
    //      "Attack"                                    - If the player IS in attack range of THIS enemy.
    //      "Move " + "Left" / "Down" / "Right" / "Up"  - If the player IS NOT in attack range, AND IS in vision range of THIS enemy, AND the enemy CAN move towards them).
    //      "Cant Move"                                 - If the player IS NOT in attack range, AND IS in vision range of THIS enemy, AND the enemy CAN NOT move towards them).
    //      "Pass"                                      - If the player IS NOT in attack range, AND IS NOT in vision range of THIS enemy.
    public State Take_Turn()
    {
        // This will get the current position on the enemy grid
        (float, float) enemy_position;
        enemy_position.Item1 = currentVision / 2 - 0.5f;
        enemy_position.Item2 = currentVision / 2 - 0.5f;

        State stAttack = CheckToAttack(enemy_position);

        // Will go through each state until one is done in order of:
        // Attack (check if player in attack range)
        // -> Move (check if player in vision range)
        // --> Pass (produces this if player is not in vision range or move range)

        // First check if to Attack

        if (stAttack != null && stAttack.GetStateName().Contains("Attack") )
        {
            return stAttack;
        }

        // Second check to move if enemy is in vision and to Move (or not if it cant move towards the player).
        State stMove = CheckToMove(enemy_position);
        if (!stMove.GetStateName().Contains("Stay"))
        {
            print(stMove.GetStateName());
            if (stMove.GetStateName().Contains("Walk"))
            {
                Walk stWalk = (Walk)stMove;
                print(stWalk.GetDirection());
                return stMove;
            }
        }

        // Third if neither the enemy will either rest or pass turn.
        return stMove;
    }




    // This is called in Take_Turn.
    // This will be called to check if the player is in attack range of the enemy.
    private State CheckToAttack((float, float) _enemy_position)
    {
        // This will get the current position on the enemy grid.
        (float, float) enemyAtkLocationMin;
        enemyAtkLocationMin.Item1 = _enemy_position.Item1 - ((attackRange / 2f) - 0.5f);
        enemyAtkLocationMin.Item2 = _enemy_position.Item2 - ((attackRange / 2f) - 0.5f);

        (float, float) enemyAtkLocationMax;
        enemyAtkLocationMax.Item1 = _enemy_position.Item1 + ((attackRange / 2f) - 0.5f);
        enemyAtkLocationMax.Item2 = _enemy_position.Item2 + ((attackRange / 2f) - 0.5f);

        //Debug.Log("before loop in check attack, x_min:" + enemyAtkLocationMin.Item1 + " y_min" + enemyAtkLocationMin.Item2);
        //Debug.Log("before loop in check attack, x_max:" + enemyAtkLocationMax.Item1 + " y_max" + enemyAtkLocationMax.Item2);

        // Cycle through each location in probably a 3x3 grid.
        for (int x = (int)enemyAtkLocationMin.Item1; x <= (int)enemyAtkLocationMax.Item1; x++)           // This will be x  (x,y).
        {
            for (int y = (int)enemyAtkLocationMin.Item2; y <= (int)enemyAtkLocationMax.Item2; y++)       // This will be y  (x,y).
            {
                //Debug.Log("In CheckAttack, X is:" + x + "Y is:" + y);
                if (enemy_Array[x, y] != null)
                {

                    //Debug.Log("In CheckAttack, enemyarray["+ x +"," + y +"] is not null");
                    // If there is a player in this location this enemy will return true and will know it can attack it.

                    if (enemy_Array[x, y].character.gameObject.tag == "Player")
                    {
                        //Debug.Log("Player is in range");
                        // Will attack player unless otherwise stated.

                        Moveset move = attacks[(int)Random.Range(0, 3)];
                        Debug.Log("Move is:" + move.GetMoveName());
                        State st = new Attack(FindObjectOfType<PlayerMovement>().gameObject, move);
                        
                        return st;
                    }
                }
                //enemyAtkLocationMin.Item1++;
            }
            //enemyAtkLocationMin.Item1 -= attackRange;
            //enemyAtkLocationMin.Item2++;
        }
        return null;
    }



    // This is called in Take_Turn.
    // This will check if the player is in the enemy array and if so will move in the direction of the player.
    // It is split into:
    // 4 quatrants: (Bottom Left), (Bottom Right), (Top Left), (Top Right) 
    // 4 direct quatrants: (Bottom), (Left), (Right), (Top) 
    // If in none will say "Player Not In Range".
    private State CheckToMove((float, float) _enemy_position)
    {
        State st = new State();
        // This will get the current position of the the player on enemy grid
        (float, float) player_location;
        player_location.Item1 = -1.0f;
        player_location.Item2 = -1.0f;

        // This divides up the quadrant into 4 squares disreguarding exactly up and down, left and right.
        int quadrantSize = (int)((currentVision / 2f) - 0.5f);


        for (int x = 0; x < currentVision; x++)
        {
            for (int y = 0; y < currentVision; y++)
            {
                //Debug.Log("Before Index Error, X & Y is:" + x + "/" + y);
                //Debug.Log("Array Length is:" + enemy_Array.GetLength(0) + "/" + enemy_Array.GetLength(1));
                if (enemy_Array[x, y] != null)
                {
                    Debug.Log("Looking for player: CurrentVision:" + CurrentVision);
                    if (enemy_Array[x, y].character == null)
                    {
                        Debug.Log("Character Field == null");
                    }
                    if (enemy_Array[x, y].character.gameObject.tag == "Player")
                    {
                        Debug.Log("Found Player");
                        player_location.Item1 = x;
                        player_location.Item2 = y;
                    }
                }
                else Debug.Log("Array is null");
            }
        }

        //0-0-0-0-0
        //0-0-0-0-0
        //0-0-U-0-0
        //X-X-0-0-0
        //X-X-0-0-0
        //(x is 0 & 1) and (y is 0 & 1)
        if ((player_location.Item1 >= 0 && player_location.Item1 < quadrantSize) &&
           (player_location.Item2 >= 0 && player_location.Item2 < quadrantSize))
        {
            // They're in bottom left quadrant
            if (IsLocationFreeToMove(_enemy_position.Item1 - 1f, _enemy_position.Item2) &&
                IsLocationFreeToMove(_enemy_position.Item1, _enemy_position.Item2 - 1f))
            {
                if (Random.value < .5)
                {
                    Debug.Log("Check from bottom left quadrant tells to move left by chance");
                    return new Walk("Left");
                }
                else
                {
                    Debug.Log("Check from bottom left quadrant tells to move down by chance");
                    return new Walk("Down");
                }
            }
            // If below is blocked off.
            else if (IsLocationFreeToMove(_enemy_position.Item1 - 1f, _enemy_position.Item2) &&
                    !IsLocationFreeToMove(_enemy_position.Item1, _enemy_position.Item2 - 1f))
            {
                Debug.Log("Check from bottom left quadrant tells to move left since down is blocked off");
                return new Walk("Left");
            }
            // If left is blocked off.
            else if (!IsLocationFreeToMove(_enemy_position.Item1 - 1f, _enemy_position.Item2) &&
                    IsLocationFreeToMove(_enemy_position.Item1, _enemy_position.Item2 - 1f))
            {
                Debug.Log("Check from bottom left quadrant tells to move down since left is blocked off");
                return new Walk("Down");
            }
            // If it gets to here, the enemy can not get to the player (as there is probs a wall or enemy in the way) and will "Cant Move" (what ever that will be)(probs stay aggressive but pass).
            else
            {
                Debug.Log("Check from bottom left quadrant tells to not move");
                return new Stay();
            }
        }

        //0-0-0-0-0
        //0-0-0-0-0
        //0-0-0-0-0
        //0-0-0-X-X
        //0-0-0-X-X
        //(x is 3 & 4) and (y is 0 & 1)
        if ((player_location.Item1 > quadrantSize && player_location.Item1 < currentVision) &&
           (player_location.Item2 >= 0 && player_location.Item2 < quadrantSize))
        {
            // They're in bottom right quadrant
            if (IsLocationFreeToMove(_enemy_position.Item1 + 1f, _enemy_position.Item2) &&
                                IsLocationFreeToMove(_enemy_position.Item1, _enemy_position.Item2 - 1f))
            {
                if (Random.value < .5)
                {
                    Debug.Log("Check from bottom right quadrant tells to move right by chance");
                    return new Walk("Right");
                }
                else
                {
                    Debug.Log("Check from bottom right quadrant tells to move down by chance");
                    return new Walk("Down");
                }
            }
            // If below is blocked off.
            else if (IsLocationFreeToMove(_enemy_position.Item1 + 1f, _enemy_position.Item2) &&
                    !IsLocationFreeToMove(_enemy_position.Item1, _enemy_position.Item2 - 1f))
            {
                Debug.Log("Check from bottom right quadrant tells to move right since down is blocked");
                return new Walk("Right");
            }
            // If left is blocked off.
            else if (!IsLocationFreeToMove(_enemy_position.Item1 + 1f, _enemy_position.Item2) &&
                    IsLocationFreeToMove(_enemy_position.Item1, _enemy_position.Item2 - 1f))
            {
                Debug.Log("Check from bottom right quadrant tells to move down since right is blocked");
                return new Walk("Down");
            }
            // If it gets to here, the enemy can not get to the player (as there is probs a wall or enemy in the way) and will "Cant Move" (what ever that will be)(probs stay aggressive but pass).
            else
            {
                Debug.Log("Check from bottom right quadrant tells to not move");
                return new Walk("Stay");
            }
        }

        //0-0-0-X-X
        //0-0-0-X-X
        //0-0-0-0-0
        //0-0-0-0-0
        //0-0-0-0-0
        //(x is 3 & 4) and (y is 3 & 4)
        if ((player_location.Item1 > quadrantSize && player_location.Item1 < currentVision) &&
           (player_location.Item2 > quadrantSize && player_location.Item2 < currentVision))
        {
            // They're in top right quadrant
            if (IsLocationFreeToMove(_enemy_position.Item1 + 1f, _enemy_position.Item2) &&
                                IsLocationFreeToMove(_enemy_position.Item1, _enemy_position.Item2 + 1f))
            {
                if (Random.value < .5)
                {
                    Debug.Log("Check from top right quadrant tells to move right by chance");
                    return new Walk("Right");
                }
                else
                {
                    Debug.Log("Check from top right quadrant tells to move up by chance");
                    return new Walk("Up");
                }
            }
            // If below is blocked off.
            else if (IsLocationFreeToMove(_enemy_position.Item1 + 1f, _enemy_position.Item2) &&
                    !IsLocationFreeToMove(_enemy_position.Item1, _enemy_position.Item2 + 1f))
            {
                Debug.Log("Check from top right quadrant tells to move right since up is blocked");
                return new Walk("Right");
            }
            // If left is blocked off.
            else if (!IsLocationFreeToMove(_enemy_position.Item1 + 1f, _enemy_position.Item2) &&
                    IsLocationFreeToMove(_enemy_position.Item1, _enemy_position.Item2 + 1f))
            {
                Debug.Log("Check from top right quadrant tells to move up since right is blocked");
                return new Walk("Up");
            }
            // If it gets to here, the enemy can not get to the player (as there is probs a wall or enemy in the way) and will "Cant Move" (what ever that will be)(probs stay aggressive but pass).
            else
            {
                Debug.Log("Check from top right quadrant tells to not move");
                return new Stay();
            }
        }

        //X-X-0-0-0
        //X-X-0-0-0
        //0-0-0-0-0
        //0-0-0-0-0
        //0-0-0-0-0
        //(x is 0 & 1) and (y is 3 & 4)
        if ((player_location.Item1 >= 0 && player_location.Item1 < quadrantSize) &&
           (player_location.Item2 > quadrantSize && player_location.Item2 < currentVision))
        {
            // They're in top left quadrant
            if (IsLocationFreeToMove(_enemy_position.Item1 - 1f, _enemy_position.Item2) &&
                                IsLocationFreeToMove(_enemy_position.Item1, _enemy_position.Item2 + 1f))
            {
                if (Random.value < .5)
                {
                    Debug.Log("Check from top left quadrant tells to move left by chance");
                    return new Walk("Left");
                }
                else
                {
                    Debug.Log("Check from top left quadrant tells to move up by chance");
                    return new Walk("Up");
                }
            }
            // If below is blocked off.
            else if (IsLocationFreeToMove(_enemy_position.Item1 - 1f, _enemy_position.Item2) &&
                    !IsLocationFreeToMove(_enemy_position.Item1, _enemy_position.Item2 + 1f))
            {
                Debug.Log("Check from top left quadrant tells to move left since up is blocked");
                return new Walk("Left");
            }
            // If left is blocked off.
            else if (!IsLocationFreeToMove(_enemy_position.Item1 - 1f, _enemy_position.Item2) &&
                    IsLocationFreeToMove(_enemy_position.Item1, _enemy_position.Item2 + 1f))
            {
                Debug.Log("Check from top left quadrant tells to move up since left is blocked");
                return new Walk("Up");
            }
            // If it gets to here, the enemy can not get to the player (as there is probs a wall or enemy in the way) and will "Cant Move" (what ever that will be)(probs stay aggressive but pass).
            else
            {
                Debug.Log("Check from top left quadrant tells to not move");
                return new Stay();
            }
        }

        //0-0-0-0-0
        //0-0-0-0-0
        //X-X-0-0-0
        //0-0-0-0-0
        //0-0-0-0-0
        if (player_location.Item1 < quadrantSize && player_location.Item1 >= 0 && player_location.Item2 == quadrantSize)
        {
            //They're directly left of the enemy
            if (!IsLocationFreeToMove(_enemy_position.Item1 - 1f, _enemy_position.Item2))
            {
                bool isUpOpen = false;
                bool isDownOpen = false;

                if (IsLocationFreeToMove(_enemy_position.Item1, _enemy_position.Item2 + 1f))
                {
                    // Move up since up is open
                    isUpOpen = true;

                }
                if (IsLocationFreeToMove(_enemy_position.Item1, _enemy_position.Item2 - 1f))
                {
                    // Move down since down is open
                    isDownOpen = true;
                }
                if (isDownOpen && isUpOpen)
                {
                    if (Random.value < .5)
                    {
                        return new Walk("Up");
                    }
                    else
                    {
                        return new Walk("Down");
                    }
                }
                else if (isDownOpen) return new Walk("Down");
                else if (isUpOpen) return new Walk("Up");
                else
                {
                    // Up and down also closed off
                    return new Stay();
                }

            }
            else
            {
                return new Walk("Left");
            }
        }
        if (player_location.Item1 > quadrantSize && player_location.Item1 < currentVision && player_location.Item2 == quadrantSize)
        {
            //They're directly right of the enemy 
            if (!IsLocationFreeToMove(_enemy_position.Item1 + 1f, _enemy_position.Item2))
            {
                bool isUpOpen = false;
                bool isDownOpen = false;

                if (IsLocationFreeToMove(_enemy_position.Item1, _enemy_position.Item2 + 1f))
                {
                    // Move up since up is open
                    isUpOpen = true;

                }
                if (IsLocationFreeToMove(_enemy_position.Item1, _enemy_position.Item2 - 1f))
                {
                    // Move down since down is open
                    isDownOpen = true;
                }
                if (isDownOpen && isUpOpen)
                {
                    if (Random.value < .5)
                    {
                        return new Walk("Up");
                        
                    }
                    else
                    {
                        return new Walk("Down");
                    }
                }
                else if (isDownOpen) return new Walk("Down");
                else if (isUpOpen) return new Walk("Up");
                else
                {
                    // Up and down also closed off
                    return new Stay();
                }

            }
            else return new Walk("Right");
        }
        if (player_location.Item2 > quadrantSize && player_location.Item2 < currentVision && player_location.Item1 == quadrantSize)
        {
            //They're directly up of the enemy
            if (!IsLocationFreeToMove(_enemy_position.Item1, _enemy_position.Item2 + 1f))
            {
                bool isLeftOpen = false;
                bool isRightOpen = false;

                if (IsLocationFreeToMove(_enemy_position.Item1 - 1, _enemy_position.Item2))
                {
                    // Move left since left is open
                    isLeftOpen = true;
                }
                if (IsLocationFreeToMove(_enemy_position.Item1 + 1, _enemy_position.Item2))
                {
                    // Move right since right is open
                    isRightOpen = true;
                }
                if (isLeftOpen && isRightOpen)
                {
                    if (Random.value < .5)
                    {
                        return new Walk("Left");
                    }
                    else
                    {
                        return new Walk("Right");
                    }
                }
                else if (isLeftOpen) return new Walk("Left");
                else if (isRightOpen) return new Walk("Right");
                else if (!isLeftOpen && !isRightOpen)
                {
                    // Up and down also closed off
                    return new Stay();
                }

            }
            else return new Walk("Up");
        }
        if (player_location.Item2 < quadrantSize && player_location.Item2 >= 0 && player_location.Item1 == quadrantSize)
        {
            //They're directly down of the enemy
            if (!IsLocationFreeToMove(_enemy_position.Item1, _enemy_position.Item2 - 1f))
            {
                bool isLeftOpen = false;
                bool isRightOpen = false;

                if (IsLocationFreeToMove(_enemy_position.Item1 - 1, _enemy_position.Item2))
                {
                    // Move left since left is open
                    isLeftOpen = true;
                }
                if (IsLocationFreeToMove(_enemy_position.Item1 + 1, _enemy_position.Item2))
                {
                    // Move right since right is open
                    isRightOpen = true;
                }
                if (isLeftOpen && isRightOpen)
                {
                    if (Random.value < .5)
                    {
                        return new Walk("Left");
                    }
                    else
                    {
                        return new Walk("Right");
                    }
                }
                else if (isLeftOpen) return new Walk("Left");
                else if (isRightOpen) return new Walk("Right");
                else
                {
                    // Up and down also closed off
                    return new Stay();
                }

            }
            else return new Walk("Down");
        }

        // 1-1-1
        // 1-0-1
        // 1-1-1
        // If the player isn't in range it will say this.
        Debug.Log("Player Not In Range, PlayerLocation is: " + player_location.Item1 + "/" + player_location.Item2);
        return new Stay();
    }

    // Only called in "CheckPlayerInRange"
    private bool IsLocationFreeToMove(float _check_position_x, float _check_position_y)
    {
        // Legit just wrote this as it was becoming too tedious writing this out 50+ times.
        // Basically if the input square is free to move it will output true, if not it will be false.
        if (enemy_Array[(int)(_check_position_x), (int)_check_position_y] != null &&
            !enemy_Array[(int)(_check_position_x), (int)_check_position_y].environment.gameObject.tag.Contains("Wall") &&
            !enemy_Array[(int)(_check_position_x), (int)_check_position_y].character.gameObject.tag.Contains("Enemy"))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void PrintMatrix(WorldElements[,] array)
    {
        int counter = 0;
        for (int x = 0; x < array.GetLength(0); x++)
        {
            for (int y = 0; y < array.GetLength(1); y++)
            {
                if (array[x, y] != null)
                {
                    counter++;
                    if (array[x, y].character == EmptyObject && array[x, y].environment == EmptyObject) Debug.Log("In EM, Empty Spot");
                    else
                    {
                        Debug.Log("In EM, At X:" + x + ", Y:" + y + "Environment is: " + array[x, y].environment.gameObject + "\n" + "Character is: " + array[x, y].character.gameObject);
                    }
                }
                else
                {
                    Debug.Log("In EM, Dumbass its null");
                }
            }
        }

        Debug.Log("In EM, Counter:" + counter);
    }
}
