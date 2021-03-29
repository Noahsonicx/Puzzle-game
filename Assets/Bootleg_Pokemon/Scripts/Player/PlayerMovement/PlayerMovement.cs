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
    public GameObject[] enemy_list = new GameObject[8];

    private int attack_chosen = -1;
    private int enemy_chosen = -1;

    //Stats
    private float max_health;
    [SerializeField]
    private float current_health;

    private float max_energy;
    [SerializeField]
    private float current_energy;

    //Stat UI
    TextMeshProUGUI health_text;
    TextMeshProUGUI energy_text;

    //UI element
    public GameObject MovePanel;
    public GameObject Move1;
    public GameObject Move2;
    public GameObject Move3;
    public GameObject Move4;

    // UI for player's enemy targets
    public GameObject target_enemy_panel;
    public GameObject target_e1;
    public GameObject target_e2;
    public GameObject target_e3;
    public GameObject target_e4;
    public GameObject target_e5;
    public GameObject target_e6;
    public GameObject target_e7;
    public GameObject target_e8;

    //private bool move_status = false; // Has player made a move

    //Getter Functions
    public float GetCurrentHealth()
    {
        return current_health;
    }
    public float GetMaxHealth()
    {
        return max_health;
    }

    public float GetCurrentEnergy()
    {
        return current_energy;
    }
    
    public float GetMaxEnergy()
    {
        return max_energy;
    }
    public Moveset[] GetMoveSet()
    {
        return attacks;
    }

    // Start is called before the first frame update
    void Start()
    {
        //Get Base stats here from elemontals
        max_health = GetComponent<Elemontals>().GetHealth();
        current_health = max_health;

        max_energy = GetComponent<Elemontals>().GetEnergy();
        current_energy = max_energy;

        //Get Stat UI
        health_text = GetComponentInChildren<Elemontals>().health_text;
        energy_text = GetComponent<Elemontals>().energy_text;
        energy_text.enabled = false;
        energy_text.enabled = true;

        //TODO: Temporary
        TakeDamage(5);
    }

    // Update is called once per frame
    void Update()
    {
        health_text.text = current_health + "/" + max_health;
        energy_text.text = current_energy + "/" + max_energy;
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

        for(int i = 0; i < 4; i++) // Get Move
        {
            if(attacks[i] == null)
            {
                //Debug.Log("Move is null");
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

        Moveset move;

        Debug.Log("Attack chosen: " + attack_chosen);

        if (attack_chosen != -1)
        {
            move = attacks[attack_chosen];

            if (move.GetManaCost() > current_energy)
            {
                //TODO: Make UI warning pop up
                Debug.Log("Not enough mana to use: " + move.GetMoveName());
            }
            else
            {
                if (move.GetTarget().Contains("Self"))
                {
                    //Debug.Log("Ability targets self");

                    return player_state = new Attack(this.gameObject, move);
                }
                else if (move.GetTarget().Contains("Enemy"))
                {

                    UpdateEnemyTargetUI();
                    //Debug.Log("Ability targets enemy");
                    Debug.Log("Enemy_chosen: " + enemy_chosen);
                    if (enemy_chosen != -1)
                    {
                        current_energy -= move.GetManaCost();
                        return player_state = new Attack(enemy_list[enemy_chosen], move);

                    }
                }
            }
        }
        return null;

    }

    public void LearnMove(int index, Moveset move)
    {
        // This should pull a warning if trying to delete and learn a move
        attacks[index] = move;
    }

    public void SetEnemyList(GameObject[] enemies)
    {
        for(int i = 0; i < enemies.Length; i++)
        {
            enemy_list[i] = (enemies[i]);
        }
    }

    public void OnAttackButtonClick(int i)
    {
        attack_chosen = i;
    }

    public void OnEnemyButtonClick(int i)
    {
        enemy_chosen = i;
    }

    public void SetMovesetUI(GameObject _panel, GameObject _mb1, GameObject _mb2, GameObject _mb3, GameObject _mb4)
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
    public void SetEnemyTargetUI(GameObject _panel, GameObject _mb1, GameObject _mb2, GameObject _mb3, GameObject _mb4, GameObject _mb5, GameObject _mb6, GameObject _mb7, GameObject _mb8)
    {
        target_enemy_panel = _panel;
        target_e1 = _mb1;
        target_e2 = _mb2;
        target_e3 = _mb3;
        target_e4 = _mb4;
        target_e5 = _mb5;
        target_e6 = _mb6;
        target_e7 = _mb7;
        target_e8 = _mb8;

        target_e1.GetComponent<Button>().onClick.AddListener(delegate { OnEnemyButtonClick(0); });
        target_e2.GetComponent<Button>().onClick.AddListener(delegate { OnEnemyButtonClick(1); });
        target_e3.GetComponent<Button>().onClick.AddListener(delegate { OnEnemyButtonClick(2); });
        target_e4.GetComponent<Button>().onClick.AddListener(delegate { OnEnemyButtonClick(3); });
        target_e5.GetComponent<Button>().onClick.AddListener(delegate { OnEnemyButtonClick(4); });
        target_e6.GetComponent<Button>().onClick.AddListener(delegate { OnEnemyButtonClick(5); });
        target_e7.GetComponent<Button>().onClick.AddListener(delegate { OnEnemyButtonClick(6); });
        target_e8.GetComponent<Button>().onClick.AddListener(delegate { OnEnemyButtonClick(7); });

    }
    public void UpdateEnemyTargetUI() 
    {
        target_enemy_panel.SetActive(true);
        if (enemy_list[0].gameObject.name.Contains("EmptyObject")) // Check for Top enemy
        {
            target_e1.SetActive(false);
        }
        else
        {
            target_e1.GetComponentInChildren<TextMeshProUGUI>().text = enemy_list[0].GetComponent<Elemontals>().GetName() + "\n(Top)"; 
            target_e1.SetActive(true);
        }
        
        if(enemy_list[1].gameObject.name.Contains("EmptyObject")) // Check for Right enemy
        {
            Debug.Log("Right Should be off");
            target_e2.SetActive(false);
        }
        else
        {
            target_e2.GetComponentInChildren<TextMeshProUGUI>().text = enemy_list[1].GetComponent<Elemontals>().GetName() + "\n(Top-Right)";
            target_e2.SetActive(true);
        }
        
        if(enemy_list[2].gameObject.name.Contains("EmptyObject"))
        {
            target_e3.SetActive(false);
        }
        else
        {
            target_e3.GetComponentInChildren<TextMeshProUGUI>().text = enemy_list[2].GetComponent<Elemontals>().GetName() + "\n(Right)";
            target_e3.SetActive(true);
        }
        
        if(enemy_list[3].gameObject.name.Contains("EmptyObject"))
        {
            target_e4.SetActive(false);
        }
        else
        {
            target_e4.GetComponentInChildren<TextMeshProUGUI>().text = enemy_list[3].GetComponent<Elemontals>().GetName() + "\n(Bottom-Right";
            target_e4.SetActive(true);
        }

        if(enemy_list[4].gameObject.name.Contains("EmptyObject"))
        {
            target_e5.SetActive(false);
        }
        else
        {
            target_e5.GetComponentInChildren<TextMeshProUGUI>().text = enemy_list[4].GetComponent<Elemontals>().GetName() + "\n(Bottom)";
            target_e5.SetActive(true);
        }

        if(enemy_list[5].gameObject.name.Contains("EmptyObject"))
        {
            target_e6.SetActive(false);
        }
        else
        {
            target_e6.GetComponentInChildren<TextMeshProUGUI>().text = enemy_list[5].GetComponent<Elemontals>().GetName() + "\n(Bottom-Left)";
            target_e6.SetActive(true);
        }

        if(enemy_list[6].gameObject.name.Contains("EmptyObject"))
        {
            target_e7.SetActive(false);
        }
        else
        {
            target_e7.GetComponentInChildren<TextMeshProUGUI>().text = enemy_list[6].GetComponent<Elemontals>().GetName() + "\n(Left)";
            target_e7.SetActive(true);
        }

        if(enemy_list[7].gameObject.name.Contains("EmptyObject"))
        {
            target_e8.SetActive(false);
        }
        else
        {
            target_e8.GetComponentInChildren<TextMeshProUGUI>().text = enemy_list[7].GetComponent<Elemontals>().GetName() + "\n(Top-Left)";
            target_e8.SetActive(true);
        }


    }

    public void ResetMoveAndEnemy()
    {
        attack_chosen = -1;
        enemy_chosen = -1;
    }
    public void SetText()
    {
        GetComponent<Elemontals>().ChangeText(current_health + "/" + max_health);
    }
    public void TakeDamage(float dmg)
    {
        //Debug.Log("Damage Taken: " + dmg);
        //Debug.Log("Starting Health: " + current_health);
        current_health -= dmg;
        SetText();
        //Debug.Log("Final Health: " + current_health);
        //if (current_health <= 0) Destroy(this.gameObject);
    }

    public void HealDamage(float val)
    {
        current_health += val;
        if(current_health > max_health)
        {
            current_health = max_health;
        }
        SetText();
    }
}
