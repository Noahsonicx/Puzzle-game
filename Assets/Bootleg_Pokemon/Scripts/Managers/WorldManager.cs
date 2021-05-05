using System.Collections.Generic;
using System.IO;
using System.Globalization;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    public struct EnvironmentContruct
    {
        public string asset_key;
        public (float, float) asset_loc;

        public EnvironmentContruct(string _key, (float, float) _loc) {

            asset_key = _key;
            asset_loc = _loc;
        }
    }

    public struct EnemyConstruct
    {
        public string elemontal_key;
        public (float, float) enemy_loc;
        public float cur_health;
        public float max_health;
        //TODO: Add Moveset once implemented on enemy movement

        public EnemyConstruct(string _key, float _x, float _y, float _cur_h, float _max_h)
        {
            elemontal_key = _key;
            enemy_loc = (_x,_y);
            cur_health = _cur_h;
            max_health = _max_h;
        }
    }
    public struct PlayerConstruct
    {
        public string elemontal_key;
        public (float, float) player_loc;
        public float cur_health;
        public float max_health;
        public float cur_energy;
        public float max_energy;
        public string move1;
        public string move2;
        public string move3;
        public string move4;
        public PlayerConstruct(string _key, float _x, float _y, float _cur_h, float _max_h, float _cur_e, float _max_e, string m1, string m2, string m3, string m4)
        {
            elemontal_key = _key;
            player_loc = (_x, _y);
            cur_health = _cur_h;
            max_health = _max_h;
            cur_energy = _cur_e;
            max_energy = _max_e;
            move1 = m1;
            move2 = m2;
            move3 = m3;
            move4 = m4;
        }
    }
    // World Settings and Contents
    private int x_size;
    private int y_size;
    
    //Status Checks
    [SerializeField] private bool level_loaded = false;
    [SerializeField] private bool dictionary_ready = false;
    [SerializeField] private bool load_data_ready = false;

    public string dungeon_name;
    public string next_dungeon_name;
    private WorldElements[,] world_array;
    private List<GameObject> enemy_list = new List<GameObject>();
    private GameObject player;
    public (float, float) player_loc;

    //Construct used to hold data when loading
    public List<EnvironmentContruct> asset_construct = new List<EnvironmentContruct>();
    public List<EnemyConstruct> enemy_construct = new List<EnemyConstruct>();
    public PlayerConstruct player_construct;

    // Temporary Reference to Player Prefab
    public GameObject playerPrefab;

    //Other Managers involved
    public GameObject environment_asset_manager;
    public GameObject elemontal_asset_manager;
    public GameObject moveset_manager;

    // Turn Base System Variables
    private bool player_turn = true;
    private bool enemy_turn = false;

    public GameObject world_origin;
    private Vector3 spawn_location;

    private State player_state = null;
    private bool enemy_around_player = false;
    public bool decide_to_attack = false;

    public GameObject[] enemy_around_player_list;
    public GameObject EmptyObject;

    // UI for player action
    public GameObject AttackButton;
    public GameObject InventoryButton; // TODO: Add Button

    // UI for player's moveset
    public GameObject AttackPanel;
    public GameObject AttackMove1;
    public GameObject AttackMove2;
    public GameObject AttackMove3;
    public GameObject AttackMove4;

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

    // Start is called before the first frame update
    void Start()
    {       
        enemy_around_player_list = new GameObject[] {EmptyObject, EmptyObject, EmptyObject, EmptyObject, EmptyObject, EmptyObject, EmptyObject, EmptyObject};
    }

    // Update is called once per frame
    void Update()
    {
        if (!level_loaded)
        {
            if (!dictionary_ready)
            {
                if (!environment_asset_manager.GetComponent<EnvironmentAssetsDictionaryManager>().GetDictionaryReadyStatus()
                    || !moveset_manager.GetComponent<MoveSetDictionary>().GetDictionaryReadyStatus()
                    || !elemontal_asset_manager.GetComponent<ElemontalAssetsDictionaryManager>().GetDictionaryReadyStatus())
                {
                    Debug.Log("Still Loading Dictionaries");
                }
                else dictionary_ready = true;
            }
            else
            {
                if (load_data_ready)
                {
                    PrepareWorld();
                    // Get the spawn Location
                    spawn_location = world_origin.GetComponent<Transform>().position;

                    // Add World Objects here (E.g Walls & Creatures (Player and Enemy))
                    // Step 1:
                    // Add Dungeon Walls (By adding x,y location of each wall as a tuple into wall_location)

                    SpawnWalls();
                    //Debug.Log("Finished Loading Walls");
                    // Step 2:
                    // Add Player into the world
                    SpawnPlayer();
                    //Debug.Log("Finished Loading Player");
                    // Step 3:
                    // Add Enemies into the world
                    SpawnEnemy();
                    //Debug.Log("Finished Loading Enemy");
                    level_loaded = true; // Level finished loading. Start Turn System after this.
                                         //Debug.Log("Finished Loading Level");
                }

            }
        }
        else
        {
            TurnSystem();
        }
    }

    private void SpawnWalls()
    {
        
        try
        {
            foreach (EnvironmentContruct ec in asset_construct)
            {
                GameObject prefab = environment_asset_manager.GetComponent<EnvironmentAssetsDictionaryManager>().GetAsset(ec.asset_key);
                if (prefab == null)
                {
                    throw (new AssetNotInDictionaryException("No Asset called: 'Wall' in Dictionary"));
                }

                //TODO:
                Vector2 relative_spawn_loc = new Vector2(spawn_location.x + ec.asset_loc.Item1, spawn_location.y + ec.asset_loc.Item2);
                if (world_array[(int)ec.asset_loc.Item1, (int)ec.asset_loc.Item2] == null) Debug.Log("WorldArray is Empty for this slot" + (int)ec.asset_loc.Item1 + "/" + (int)ec.asset_loc.Item2);
                if (world_array[(int)ec.asset_loc.Item1, (int)ec.asset_loc.Item2].environment == null) Debug.Log("Environment of WorldElement is empty");
                    world_array[(int)ec.asset_loc.Item1, (int)ec.asset_loc.Item2].environment = Instantiate(prefab, relative_spawn_loc, new Quaternion());
                Debug.Log("Spawning wall at location: " + ec.asset_loc.Item1 + "/" + ec.asset_loc.Item2);
                world_array[(int)ec.asset_loc.Item1, (int)ec.asset_loc.Item2].environment.gameObject.tag = "Wall";
                
            }
        }
        catch(AssetNotInDictionaryException e)
        {
            Debug.LogError(e.Message);
        }
       
    }
    private void SpawnPlayer()
    {
        // This should essentially save to file so that character stats are saved between levels and when loading game.
        world_array[(int)player_construct.player_loc.Item1, (int)player_construct.player_loc.Item2].character = Instantiate(playerPrefab, new Vector2(spawn_location.x + player_construct.player_loc.Item1, spawn_location.y + player_construct.player_loc.Item2), new Quaternion());
        player = world_array[(int)player_construct.player_loc.Item1, (int)player_construct.player_loc.Item2].character;
        player.name = playerPrefab.name + "| Player";
        player.AddComponent<PlayerMovement>();
        PlayerMovement pl_movement = player.GetComponent<PlayerMovement>();
        pl_movement.SetPlayerLocation(player_construct.player_loc);
        pl_movement.SetCurrentHealth(player_construct.cur_health);
        pl_movement.SetMaxHealth(player_construct.max_health);
        pl_movement.SetCurrentEnergy(player_construct.cur_energy);
        pl_movement.SetMaxEnergy(player_construct.max_energy);
        pl_movement.LearnMove(0, moveset_manager.GetComponent<MoveSetDictionary>().GetMoveset(player_construct.move1));
        pl_movement.LearnMove(1, moveset_manager.GetComponent<MoveSetDictionary>().GetMoveset(player_construct.move2));
        pl_movement.LearnMove(2, moveset_manager.GetComponent<MoveSetDictionary>().GetMoveset(player_construct.move3));
        pl_movement.LearnMove(3, moveset_manager.GetComponent<MoveSetDictionary>().GetMoveset(player_construct.move4));
        pl_movement.SetMovesetUI(AttackPanel, AttackMove1, AttackMove2, AttackMove3, AttackMove4);
        pl_movement.SetEnemyTargetUI(target_enemy_panel, target_e1, target_e2, target_e3, target_e4, target_e5, target_e6, target_e7, target_e8);

    }
    private void SpawnEnemy()
    {
        int enemy_ID = 1;
        try
        {

            if (enemy_construct.Count == 0) Debug.Log("No enemies to spawn");
            foreach (EnemyConstruct ec in enemy_construct)
            {

                GameObject prefab = elemontal_asset_manager.GetComponent<ElemontalAssetsDictionaryManager>().GetAsset(ec.elemontal_key);
                if (prefab == null)
                {
                    throw (new AssetNotInDictionaryException("No Asset called '"+ ec.elemontal_key +"' in Dictionary"));
                }
                
                Vector2 relative_spawn_loc = new Vector2(spawn_location.x + ec.enemy_loc.Item1, spawn_location.y + ec.enemy_loc.Item2);
                GameObject tmp_enemy = Instantiate(prefab, relative_spawn_loc, new Quaternion());
                tmp_enemy.name = ec.elemontal_key + "| ID: " + enemy_ID;
                tmp_enemy.gameObject.tag = "Enemy";
                tmp_enemy.AddComponent<EnemyMovement>();
                tmp_enemy.GetComponent<EnemyMovement>().SetLocation(ec.enemy_loc.Item1, ec.enemy_loc.Item2);
                Debug.Log("Enemy = CurHealth: " + ec.cur_health);
                tmp_enemy.GetComponent<EnemyMovement>().SetCurrentHealth(ec.cur_health);
                tmp_enemy.GetComponent<EnemyMovement>().SetMaxHealth(ec.max_health);
                world_array[(int)ec.enemy_loc.Item1, (int)ec.enemy_loc.Item2].character = tmp_enemy;
                enemy_list.Add(tmp_enemy);
                enemy_ID++;
            }
        } catch (AssetNotInDictionaryException e)
        {
            Debug.LogError(e.Message);
        }
    }
    private void TurnSystem()
    {
        //Debug.Log("In Turn System");
        if (player_turn)
        {
            //Debug.Log("Waiting for player input");
            // Wait for player input
            if (Input.GetKeyDown("w") || Input.GetKeyDown("a") || Input.GetKeyDown("s") || Input.GetKeyDown("d"))
            {
                Debug.Log("Input Key pressed");
                player_state = player.GetComponent<PlayerMovement>().Move();

                if (player_state == null) return;
                else
                {
                    if (PlayerAction(player_state))
                    {
                        
                        player_state = null;
                        player_turn = false;
                        enemy_turn = true;
                    }
                }
            }
            if (decide_to_attack && enemy_around_player)
            {
                Debug.Log("Player attacking");
                //Debug.Log("Player is looking to attack");
                player_state = player.GetComponent<PlayerMovement>().Attack();
                if(player_state != null)
                {
                    Debug.Log("State received");
                    if (player_state.GetStateName().Equals("Attack")) Debug.Log("State is Attack");
                    if(PlayerAction(player_state))
                    {
                        player_turn = false;
                        enemy_turn = true;
                        decide_to_attack = false;  
                    }

                }
            }

        }

        if(enemy_turn)
        {
            // Wait for all enemies to have moved

            enemy_turn = false;
            player_turn = true;
        }
        //Debug.Log("C_Hp: " + player.GetComponent<PlayerMovement>().GetCurrentHealth());
    }

    public int CountNumberOfEnemiesAroundPlayer()
    {
        int count = 0;
        int emptycount = 0;

        for(int i = 0; i < enemy_around_player_list.Length; i++)
        {
            if(!enemy_around_player_list[i].gameObject.name.Contains("EmptyObject"))
            {
                count++;
            }
            else
            {
                emptycount++;
            }
        }

        //Debug.Log("Enemy Count around player = " + count);
        //Debug.Log("EmptyObjectCount = " + emptycount);
        return count;
    }
    public void GetEnemyAroundPlayer()
    {
        //Debug.Log("Looking for enemies around player");


        if (!world_array[(int)player_loc.Item1, (int)player_loc.Item2 + 1].character.gameObject.name.Equals(EmptyObject.name)&& world_array[(int)player_loc.Item1, (int)player_loc.Item2 + 1].character.gameObject.tag == "Enemy")
        {
            //Debug.Log("There is an enemy on top");
            enemy_around_player_list[0] = (world_array[(int)player_loc.Item1, (int)player_loc.Item2 + 1].character); //Get top enemy
        }
        else enemy_around_player_list[0] = EmptyObject; //Get top enemy

        if (!world_array[(int)player_loc.Item1 + 1, (int)player_loc.Item2 + 1].character.gameObject.name.Equals(EmptyObject.name) && world_array[(int)player_loc.Item1 + 1, (int)player_loc.Item2 + 1].character.gameObject.tag == "Enemy")
        {
            //Debug.Log("There is an enemy on top");
            enemy_around_player_list[1] = (world_array[(int)player_loc.Item1 + 1, (int)player_loc.Item2 + 1].character); //Get top enemy
        }
        else enemy_around_player_list[1] = EmptyObject; //Get top enemy

        if (!world_array[(int)player_loc.Item1 + 1, (int)player_loc.Item2].character.gameObject.name.Equals(EmptyObject.name)&& world_array[(int)player_loc.Item1 + 1, (int)player_loc.Item2].character.gameObject.tag == "Enemy")
        {
            //Debug.Log("There is an enemy on right");
            enemy_around_player_list[2] = (world_array[(int)player_loc.Item1 + 1, (int)player_loc.Item2].character); //Get right enemy
        }
        else enemy_around_player_list[2] = EmptyObject; //Get right enemy

        if (!world_array[(int)player_loc.Item1 + 1, (int)player_loc.Item2 - 1].character.gameObject.name.Equals(EmptyObject.name)&& world_array[(int)player_loc.Item1 + 1, (int)player_loc.Item2 - 1].character.gameObject.tag == "Enemy")
        {
            //Debug.Log("There is an enemy on right");
            enemy_around_player_list[3] = (world_array[(int)player_loc.Item1 + 1, (int)player_loc.Item2 - 1].character); //Get bottom-right enemy
        }
        else enemy_around_player_list[3] = EmptyObject; //Get bottom-right enemy
        
        if (!world_array[(int)player_loc.Item1, (int)player_loc.Item2 - 1].character.gameObject.name.Equals(EmptyObject.name) && world_array[(int)player_loc.Item1, (int)player_loc.Item2 - 1].character.gameObject.tag == "Enemy")
        {
            //Debug.Log("There is an enemy to the bottom");
            enemy_around_player_list[4] = (world_array[(int)player_loc.Item1, (int)player_loc.Item2-1].character); //Get bottom enemy
        }
        else enemy_around_player_list[4] = EmptyObject; //Get bottom enemy

        if (!world_array[(int)player_loc.Item1 - 1, (int)player_loc.Item2 - 1].character.gameObject.name.Equals(EmptyObject.name) && world_array[(int)player_loc.Item1 - 1, (int)player_loc.Item2 - 1].character.gameObject.tag == "Enemy")
        {
            //Debug.Log("There is an enemy to the bottom");
            enemy_around_player_list[5] = (world_array[(int)player_loc.Item1 - 1, (int)player_loc.Item2 - 1].character); //Get bottom-left enemy
        }
        else enemy_around_player_list[5] = EmptyObject; //Get bottom-left enemy

        if (!world_array[(int)player_loc.Item1 - 1, (int)player_loc.Item2].character.gameObject.name.Equals(EmptyObject.name) && world_array[(int)player_loc.Item1 - 1, (int)player_loc.Item2].character.gameObject.tag == "Enemy") 
        {
            //Debug.Log("There is an enemy to the left");
            enemy_around_player_list[6] = (world_array[(int)player_loc.Item1 - 1, (int)player_loc.Item2].character); //Get left enemy
        }
        else enemy_around_player_list[6] = (EmptyObject); //Get left enemy

        if (!world_array[(int)player_loc.Item1 - 1, (int)player_loc.Item2 + 1].character.gameObject.name.Equals(EmptyObject.name) && world_array[(int)player_loc.Item1 - 1, (int)player_loc.Item2 + 1].character.gameObject.tag == "Enemy")
        {
            //Debug.Log("There is an enemy to the left");
            enemy_around_player_list[7] = (world_array[(int)player_loc.Item1 - 1, (int)player_loc.Item2 + 1].character); //Get left enemy
        }
        else enemy_around_player_list[7] = (EmptyObject); //Get left enemy
   

        if (CountNumberOfEnemiesAroundPlayer() != 0)
        {
            //Debug.Log("enemy is nearby");
            enemy_around_player = true;
            AttackButton.SetActive(true);
        }
        else
        {
            enemy_around_player = false;
            AttackButton.SetActive(false);
            
        }
    }
    private bool PlayerAction(State st)
    {
        bool move_success = false;
        (float, float) new_player_loc = player.GetComponent<PlayerMovement>().GetPlayerLocation();
        
        switch (st.GetStateName())
        {
            case "Walk":
                //Debug.Log("State is Walk");
                Walk tmp = (Walk)st;
                switch (tmp.GetDirection())
                {
                    case "Up":
                        //Debug.Log("Dir is Up");
                        new_player_loc.Item2 += 1.0f;
                        //Debug.Log("New loc:" + new_player_loc.Item1 + " " + new_player_loc.Item2);
                        if (world_array[(int)new_player_loc.Item1, (int)new_player_loc.Item2] == null || 
                            !world_array[(int)new_player_loc.Item1, (int)new_player_loc.Item2].environment.gameObject.tag.Contains("Wall") &&
                            !world_array[(int)new_player_loc.Item1, (int)new_player_loc.Item2].character.gameObject.tag.Contains("Enemy"))
                        {
                            //Debug.Log("Moving Player Up");
                            player.transform.position = new Vector2(spawn_location.x + new_player_loc.Item1, spawn_location.y + new_player_loc.Item2);
                            world_array[(int)new_player_loc.Item1, (int)new_player_loc.Item2].character = player;
                            world_array[(int)player_loc.Item1, (int)player_loc.Item2].character = EmptyObject;
                            player_loc = new_player_loc;
                            player.GetComponent<PlayerMovement>().SetPlayerLocation(new_player_loc);
                            move_success = true;
                            decide_to_attack = false;

                            GetEnemyAroundPlayer();
                            player.GetComponent<PlayerMovement>().SetEnemyList(enemy_around_player_list);
                            player.GetComponent<PlayerMovement>().ResetMoveAndEnemy();
                            ResetUI();

                        }
                        else if (world_array[(int)new_player_loc.Item1, (int)new_player_loc.Item2] != null) Debug.Log("Up is not null");
                        else if (world_array[(int)new_player_loc.Item1, (int)new_player_loc.Item2].environment.gameObject.tag.Contains("Wall")) Debug.Log("Up is a wall");
                        else if (world_array[(int)new_player_loc.Item1, (int)new_player_loc.Item2].character.gameObject.tag.Contains("Enemy")) Debug.Log("Up is an enemy");
                        else Debug.Log("Something else broke");

                        break;
                    case "Down":
                        //Debug.Log("Dir is Up");
                        new_player_loc.Item2 -= 1.0f;
                        //Debug.Log("New loc:" + new_player_loc.Item1 + " " + new_player_loc.Item2);
                        if (world_array[(int)new_player_loc.Item1, (int)new_player_loc.Item2] == null ||
                            !world_array[(int)new_player_loc.Item1, (int)new_player_loc.Item2].environment.gameObject.tag.Contains("Wall") &&
                            !world_array[(int)new_player_loc.Item1, (int)new_player_loc.Item2].character.gameObject.tag.Contains("Enemy"))
                        {
                            Debug.Log("Moving Player Down");
                            player.transform.position = new Vector2(spawn_location.x + new_player_loc.Item1, spawn_location.y + new_player_loc.Item2);
                            world_array[(int)new_player_loc.Item1, (int)new_player_loc.Item2].character = player;
                            world_array[(int)player_loc.Item1, (int)player_loc.Item2].character = EmptyObject;
                            player_loc = new_player_loc;
                            player.GetComponent<PlayerMovement>().SetPlayerLocation(new_player_loc);
                            move_success = true;
                            decide_to_attack = false;

                            GetEnemyAroundPlayer();
                            player.GetComponent<PlayerMovement>().SetEnemyList(enemy_around_player_list);
                            player.GetComponent<PlayerMovement>().ResetMoveAndEnemy();
                            ResetUI();
                            
                        }
                        else Debug.Log("There is a Wall Down");
                        break;
                    case "Left":
                        //Debug.Log("Dir is Up");
                        new_player_loc.Item1 -= 1.0f;
                        //Debug.Log("New loc:" + new_player_loc.Item1 + " " + new_player_loc.Item2);
                        if (world_array[(int)new_player_loc.Item1, (int)new_player_loc.Item2] == null ||
                            !world_array[(int)new_player_loc.Item1, (int)new_player_loc.Item2].environment.gameObject.tag.Contains("Wall") &&
                            !world_array[(int)new_player_loc.Item1, (int)new_player_loc.Item2].character.gameObject.tag.Contains("Enemy"))
                        {
                            Debug.Log("Moving Player Left");
                            player.transform.position = new Vector2(spawn_location.x + new_player_loc.Item1, spawn_location.y + new_player_loc.Item2);
                            world_array[(int)new_player_loc.Item1, (int)new_player_loc.Item2].character = player;
                            world_array[(int)player_loc.Item1, (int)player_loc.Item2].character = EmptyObject;
                            player_loc = new_player_loc;
                            player.GetComponent<PlayerMovement>().SetPlayerLocation(new_player_loc);
                            move_success = true;
                            decide_to_attack = false;

                            GetEnemyAroundPlayer();
                            player.GetComponent<PlayerMovement>().SetEnemyList(enemy_around_player_list);
                            player.GetComponent<PlayerMovement>().ResetMoveAndEnemy();
                            ResetUI();
                          
                        }
                        else Debug.Log("There is a Wall Left");
                        break;
                    case "Right":
                        //Debug.Log("Dir is Up");
                        new_player_loc.Item1 += 1.0f;
                        //Debug.Log("New loc:" + new_player_loc.Item1 + " " + new_player_loc.Item2);
                        if (world_array[(int)new_player_loc.Item1, (int)new_player_loc.Item2] == null ||
                            !world_array[(int)new_player_loc.Item1, (int)new_player_loc.Item2].environment.gameObject.tag.Contains("Wall") &&
                            !world_array[(int)new_player_loc.Item1, (int)new_player_loc.Item2].character.gameObject.tag.Contains("Enemy"))
                        {
                            Debug.Log("Moving Player Right");
                            player.transform.position = new Vector2(spawn_location.x + new_player_loc.Item1, spawn_location.y + new_player_loc.Item2);
                            world_array[(int)new_player_loc.Item1, (int)new_player_loc.Item2].character = player;
                            world_array[(int)player_loc.Item1, (int)player_loc.Item2].character = EmptyObject;
                            player_loc = new_player_loc;
                            player.GetComponent<PlayerMovement>().SetPlayerLocation(new_player_loc);
                            move_success = true;
                            decide_to_attack = false;

                            GetEnemyAroundPlayer();
                            player.GetComponent<PlayerMovement>().SetEnemyList(enemy_around_player_list);
                            player.GetComponent<PlayerMovement>().ResetMoveAndEnemy();
                            ResetUI();
                            
                        }
                        else Debug.Log("There is a Wall Right");
                        break;
                }
                break;

            case "Attack":
                Attack atk_state = (Attack)st;
                if(atk_state.target.gameObject.GetComponent<PlayerMovement>()) 
                {
                    Debug.Log("Gameobject name: " + atk_state.target.gameObject.name);
                    Debug.Log("Target is Player");
                    switch (atk_state.move.GetStatAffected())
                    {
                        case "Health":
                            Debug.Log("Player Health Affected");
                            player.GetComponent<PlayerMovement>().HealDamage(atk_state.move.GetValue());
                            move_success = true;
                            break;
                        case "Mana":
                            Debug.Log("Player Mana Affected");
                            move_success = true;
                            break;
                            // Add more stats later   
                    }
                }
                else
                {
                    Debug.Log("Target is Enemy");
                    switch (atk_state.move.GetStatAffected())
                    {
                        case "Health":
                            Debug.Log("Enemy Health Affected");
                            if (atk_state.target.gameObject.GetComponent<EnemyMovement>().TakeDamage(atk_state.move.GetValue()))
                            {
                                var tmp_x = atk_state.target.GetComponent<EnemyMovement>().location.Item1;
                                var tmp_y = atk_state.target.GetComponent<EnemyMovement>().location.Item2;
                                enemy_list.Remove(world_array[(int)tmp_x, (int)tmp_y].character);
                                Destroy(world_array[(int)tmp_x, (int)tmp_y].character);
                                world_array[(int)tmp_x, (int)tmp_y].character = EmptyObject;

                            }
                            move_success = true;
                            break;
                        case "Mana":
                            Debug.Log("Enemy Mana Affected");
                            move_success = true;
                            break;
                            // Add more stats later   
                    }
                }

                AttackPanel.SetActive(false);
                target_enemy_panel.SetActive(false);
                player.GetComponent<PlayerMovement>().ResetMoveAndEnemy();
                break;
        }

        return move_success;

    }

    private void ResetUI()
    {
        if (target_enemy_panel.activeSelf == true)target_enemy_panel.SetActive(false);
        if(AttackPanel.activeSelf == true)AttackPanel.SetActive(false);
    }
    public void DecideToAttackSignal(bool d)
    {
        decide_to_attack = d;
    }
    public void SetDungeonDimension(int _x, int _y)
    {
        x_size = _x;
        y_size = _y;
    }
    public GameObject GetPlayer()
    {
        return player;
    }
    public List<GameObject> GetEnemyList()
    {
        return enemy_list;
    }
    public string GetDungeonname()
    {
        return dungeon_name;
    }public void SetDungeonName(string dungeon)
    {
        dungeon_name = dungeon;
    }
    public string GetNextDungeonName()
    {
        return next_dungeon_name;
    }
    public void SetNextDungeonName(string dungeon)
    {
        next_dungeon_name = dungeon;
    }
    public void SetLoadStatusReady()
    {
        load_data_ready = true;
    }
    public void LoadEnvironment(string _key, (float, float) _loc)
    {
        asset_construct.Add(new EnvironmentContruct(_key, _loc));        
    }
    public void LoadEnemies(string _key, float _enemy_x, float _enemy_y, float _cur_hp, float _max_hp)
    {
        enemy_construct.Add(new EnemyConstruct(_key, _enemy_x, _enemy_y, _cur_hp, _max_hp));
    }
 
    public void LoadPlayer(string _key, float _enemy_x, float _enemy_y, float _cur_hp, float _max_hp, float _cur_energy, float _max_energy,string m1, string m2, string m3, string m4)
    {
        player_construct = new PlayerConstruct(_key, _enemy_x, _enemy_y, _cur_hp, _max_hp, _cur_energy, _max_energy, m1, m2, m3, m4);
    }

    public void PrepareWorld()
    {
        world_array = new WorldElements[x_size, y_size];
        Debug.Log("In Prepare world, x:" + x_size + " & y:" + y_size);
        for (int x = 0; x < x_size; x++)
        {
            for (int y = 0; y < y_size; y++)
            {
                world_array[x, y] = new WorldElements(EmptyObject);
            }
        }
    }

    public void ResetWorld()
    {
        level_loaded = false;
        load_data_ready = false;
        if(world_array != null)
        {
            for (int x = 0; x < x_size; x++)
            {
                for (int y = 0; y < y_size; y++)
                {
                    if(world_array[x,y].character.name != EmptyObject.name)
                    {
                        Destroy(world_array[x, y].character.gameObject);
                        world_array[x, y].character = EmptyObject;
                    }
                    if(world_array[x,y].environment.name != EmptyObject.name)
                    {
                        Destroy(world_array[x, y].environment.gameObject);
                        world_array[x, y].environment = EmptyObject;
                    }
                    if(world_array[x,y].item.name != EmptyObject.name)
                    {
                        Destroy(world_array[x, y].item.gameObject);
                        world_array[x, y].item = EmptyObject;
                    }
                }
            }
        }
        
        enemy_list.Clear();
        enemy_construct.Clear();
        asset_construct.Clear();
        PrepareWorld();
    }
}
