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
    // World Settings and Contents
    private static int x_size = 6;
    private static int y_size = 6;
    public bool level_loaded = false;

    public string dungeon_name;
    private WorldElements[,] world_array = new WorldElements[x_size, y_size];
    private List<GameObject> enemy_list = new List<GameObject>();
    private GameObject player;
    private (float, float) player_loc;
    public List<EnvironmentContruct> asset_locations = new List<EnvironmentContruct>();
    public List<EnemyConstruct> enemy_locations = new List<EnemyConstruct>();

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
        LoadLevel("Trial");
        
        enemy_around_player_list = new GameObject[] {EmptyObject, EmptyObject, EmptyObject, EmptyObject, EmptyObject, EmptyObject, EmptyObject, EmptyObject};
    }

    // Update is called once per frame
    void Update()
    {
        if (!level_loaded) {
            while (!environment_asset_manager.GetComponent<EnvironmentAssetsDictionaryManager>().GetDictionaryReadyStatus())
            {
                Debug.Log("Still Loading Dictionaries");
            }
            
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

        TurnSystem();
    }

    private void SpawnWalls()
    {
        
        try
        {
            foreach (EnvironmentContruct ec in asset_locations)
            {
                GameObject prefab = environment_asset_manager.GetComponent<EnvironmentAssetsDictionaryManager>().GetAsset(ec.asset_key);
                if (prefab == null)
                {
                    throw (new AssetNotInDictionaryException("No Asset called: 'Wall' in Dictionary"));
                }

                //TODO:
                Vector2 relative_spawn_loc = new Vector2(spawn_location.x + ec.asset_loc.Item1, spawn_location.y + ec.asset_loc.Item2);
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
        player_loc = ((4.0f, 1.0f));
        world_array[(int)player_loc.Item1, (int)player_loc.Item2].character = Instantiate(playerPrefab, new Vector2(spawn_location.x + player_loc.Item1, spawn_location.y + player_loc.Item2), new Quaternion());
        player = world_array[(int)player_loc.Item1, (int)player_loc.Item2].character;
        player.AddComponent<PlayerMovement>();
        player.GetComponent<PlayerMovement>().SetPlayerLocation(player_loc);
        player.GetComponent<PlayerMovement>().LearnMove(0, moveset_manager.GetComponent<MoveSetDictionary>().GetMoveset("Blaze"));
        player.GetComponent<PlayerMovement>().LearnMove(1, moveset_manager.GetComponent<MoveSetDictionary>().GetMoveset("Pound"));
        player.GetComponent<PlayerMovement>().LearnMove(2, moveset_manager.GetComponent<MoveSetDictionary>().GetMoveset("Bless"));
        player.GetComponent<PlayerMovement>().LearnMove(3, moveset_manager.GetComponent<MoveSetDictionary>().GetMoveset("Fire Dance"));
        player.GetComponent<PlayerMovement>().SetMovesetUI(AttackPanel, AttackMove1, AttackMove2, AttackMove3, AttackMove4);
        player.GetComponent<PlayerMovement>().SetEnemyTargetUI(target_enemy_panel, target_e1, target_e2, target_e3, target_e4, target_e5, target_e6, target_e7, target_e8);

    }
    private void SpawnEnemy()
    {
        try
        {
            

            foreach (EnemyConstruct ec in enemy_locations)
            {

                GameObject prefab = elemontal_asset_manager.GetComponent<ElemontalAssetsDictionaryManager>().GetAsset(ec.elemontal_key);
                if (prefab == null)
                {
                    throw (new AssetNotInDictionaryException("No Asset called 'Elemont2' in Dictionary"));
                }

                Vector2 relative_spawn_loc = new Vector2(spawn_location.x + ec.enemy_loc.Item1, spawn_location.y + ec.enemy_loc.Item2);
                GameObject tmp_enemy = Instantiate(prefab, relative_spawn_loc, new Quaternion());
                tmp_enemy.gameObject.tag = "Enemy";
                tmp_enemy.AddComponent<EnemyMovement>();
                tmp_enemy.GetComponent<EnemyMovement>().SetLocation(ec.enemy_loc.Item1, ec.enemy_loc.Item2);
                tmp_enemy.GetComponent<EnemyMovement>().SetCurrentHealth(ec.cur_health);
                tmp_enemy.GetComponent<EnemyMovement>().SetMaxHealth(ec.max_health);
                world_array[(int)ec.enemy_loc.Item1, (int)ec.enemy_loc.Item2].character = tmp_enemy;
                enemy_list.Add(tmp_enemy);
            }
        } catch (AssetNotInDictionaryException e)
        {
            Debug.LogError(e.Message);
        }
    }
    private void TurnSystem()
    {
        
        //Debug.Log("In Turn System");
        if(player_turn)
        {
            //Debug.Log("Waiting for player input");
            // Wait for player input
            if (Input.GetKeyDown("w") || Input.GetKeyDown("a") || Input.GetKeyDown("s") || Input.GetKeyDown("d"))
            {
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
                //Debug.Log("Player is looking to attack");
                player_state = player.GetComponent<PlayerMovement>().Attack();
                if(player_state != null)
                {
                    Debug.Log("State received");
                    if (player_state.GetStateName().Equals("Attack")) Debug.Log("State is Attack");
                    if(PlayerAction(player_state))
                    {
                        // TODO: Attack the enemy intended
                    }

                    player_turn = false;
                    enemy_turn = true;
                    decide_to_attack = false;
                }
            }

        }

        if(enemy_turn)
        {
            // Wait for all enemies to have moved

            enemy_turn = false;
            player_turn = true;
        }
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
        (float, float) new_player_loc = player_loc;
        
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
    }
    public void LoadLevel(string level_name)
    {
        string path = @"..\Bootleg_Pokemon\Assets\Bootleg_Pokemon\Scripts\Level\" + level_name + ".txt";
        FileStream fs = File.OpenRead(path);
        StreamReader sr = new StreamReader(fs);

        string line = sr.ReadLine();

        dungeon_name = line.Substring(line.IndexOf(": ") + 2);
        //Debug.Log(line);
        line = sr.ReadLine();
        string dimension_substring = line.Substring(line.IndexOf(": ") + 2);
        //Debug.Log("dim substring: " + dimension_substring);
        string[] dimension = dimension_substring.Split(',');

        x_size = int.Parse(dimension[0], CultureInfo.InvariantCulture.NumberFormat);
        y_size = int.Parse(dimension[1], CultureInfo.InvariantCulture.NumberFormat);
        //Debug.Log("dimensions are: " + x_size + " / " + y_size);

        world_array = new WorldElements[x_size, y_size];

        for (int x = 0; x < x_size; x++)
        {
            for (int y = 0; y < y_size; y++)
            {
                world_array[x, y] = new WorldElements(EmptyObject);
            }
        }

        for(int y = y_size - 1; y >= 0; y--)
        {
            line = sr.ReadLine();
            Debug.Log("Line is: " + line);
            string[] asset_line = line.Split(' ');
            int line_index = 0;
            for(int x = 0; x < x_size; x++)
            {
                //Debug.Log("length of array; " + asset_line.Length);
                //Debug.Log("at assetline[0]" + asset_line[0]);
                //Debug.Log("at assetline[1]" + asset_line[1]);
                //Debug.Log(asset_line[1]);
                if(asset_line[line_index] != "--") asset_locations.Add(new EnvironmentContruct(asset_line[line_index], (x, y)));
                line_index++;
            }
        }
        
    }
    public void LoadEnemies(string _key, float _enemy_x, float _enemy_y, float _cur_hp, float _max_hp)
    {
        enemy_locations.Add(new EnemyConstruct(_key, _enemy_x, _enemy_y, _cur_hp, _max_hp));
    }
   
}
