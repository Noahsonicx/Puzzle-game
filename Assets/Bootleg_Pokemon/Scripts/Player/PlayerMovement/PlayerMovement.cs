using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    //UI element
    public GameObject MovePanel;
    public GameObject Move1;
    public GameObject Move2;
    public GameObject Move3;
    public GameObject Move4;

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
        MovePanel.SetActive(true);

        for(int i = 0; i < 4; i++)
        {
            if(attacks[i] == null)
            {
                Debug.Log("Move is null");
                continue;
            }
            Debug.Log(attacks[i].GetMoveName());
        }
        TextMeshProUGUI move1Text = Move1.GetComponentInChildren<TextMeshProUGUI>();
        TextMeshProUGUI move2Text = Move2.GetComponentInChildren<TextMeshProUGUI>();
        TextMeshProUGUI move3Text = Move3.GetComponentInChildren<TextMeshProUGUI>();
        TextMeshProUGUI move4Text = Move4.GetComponentInChildren<TextMeshProUGUI>();

        if (move1Text == null || move2Text == null || move3Text == null || move1Text == null) Debug.Log("One of the buttons are null");
        move1Text.text = attacks[0].GetMoveName();
        move2Text.text = attacks[1].GetMoveName();
        move3Text.text = attacks[2].GetMoveName();
        move4Text.text = attacks[3].GetMoveName();

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

    public void SetUI(GameObject _panel, GameObject _mb1, GameObject _mb2, GameObject _mb3, GameObject _mb4)
    {
        MovePanel = _panel;
        Move1 = _mb1;
        Move2 = _mb2;
        Move3 = _mb3;
        Move4 = _mb4;

        Move1.GetComponent<Button>().onClick.AddListener(delegate { OnAttackButtonClick(0); });
        Move2.GetComponent<Button>().onClick.AddListener(delegate { OnAttackButtonClick(1); });
        Move3.GetComponent<Button>().onClick.AddListener(delegate { OnAttackButtonClick(2); });
        Move4.GetComponent<Button>().onClick.AddListener(delegate { OnAttackButtonClick(3); });

    }
}
