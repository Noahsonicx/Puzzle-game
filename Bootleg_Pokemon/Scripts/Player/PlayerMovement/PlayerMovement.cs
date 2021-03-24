using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public enum AllPossibleState
    {
        Walk,
        Stay,
        Attack,
    }


    [SerializeField]
    private State player_state;
    private Moveset[] attacks = new Moveset[4];
    public List<GameObject> enemy_list = new List<GameObject>();

    private int attack_chosen = -1;
    private int enemy_chosen = -1;

    //private bool move_status = false; // Has player made a move

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
 
        
    }
    public State Move()
    {
        //Debug.Log("In Player Move System");
        if (Input.GetKeyDown("w"))
        {
            // Move Player "Up" one block
            player_state = new Walk("Up");
            //move_status = true;
        }
        if (Input.GetKeyDown("a"))
        {
            // Move Player "Left" one block
            player_state = new Walk("Left");
            //move_status = true;
        }
        if (Input.GetKeyDown("s"))
        {
            //Move Player "Down" one block
            player_state = new Walk("Down");
            //move_status = true;
        }
        if (Input.GetKeyDown("d"))
        {
            // Move Player "Right" one block
            player_state = new Walk("Right");
            //move_status = true;
        }
        return player_state;
    }

    public State Attack()
    {
        if (attack_chosen != -1 && enemy_chosen != -1)
        {
            return player_state = new Attack(enemy_list[enemy_chosen], attacks[attack_chosen]);
        }
        return null;

    }

    public void LearnMove(int index, Moveset move)
    {
        // This should pull a warning if trying to delete and learn a move
        attacks[index] = move;
    }

    public void SetEnemyList(List<GameObject> _enemies)
    {
        enemy_list = _enemies;
    }

    public void OnAttackButtonClick(int i)
    {
        attack_chosen = i;
    }

    public void OnEnemyButtonClick(int i)
    {
        enemy_chosen = i;
    }
}
