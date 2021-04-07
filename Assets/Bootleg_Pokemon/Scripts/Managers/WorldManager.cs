using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    // World Settings and Contents
    private static int x_size = 6;
    private static int y_size = 6;
    public bool level_loaded = false;

    private WorldElements[,] world_array = new WorldElements[x_size, y_size];
    private List<GameObject> enemy_list = new List<GameObject>();
    private GameObject player;
    private (float, float) player_loc;
    public List<(float, float)> wall_locations = new List<(float, float)>();
    public List<(float, float)> enemy_locations = new List<(float, float)>();

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
        for(int x = 0; x < x_size; x++)
        {
            for(int y = 0; y < y_size; y++)
            {
                world_array[x, y] = new WorldElements(EmptyObject);
            }
        }
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
            wall_locations.Add((0.0f, 0.0f));
            
            wall_locations.Add((0.0f, 1.0f));
            
            wall_locations.Add((0.0f, 2.0f));
            wall_locations.Add((0.0f, 3.0f));
            wall_locations.Add((0.0f, 4.0f));
            wall_locations.Add((0.0f, 5.0f));

            wall_locations.Add((0.0f, 5.0f));
            wall_locations.Add((1.0f, 5.0f));
            wall_locations.Add((2.0f, 5.0f));
            wall_locations.Add((3.0f, 5.0f));
            wall_locations.Add((4.0f, 5.0f));
            wall_locations.Add((5.0f, 5.0f));

            wall_locations.Add((5.0f, 0.0f));
            wall_locations.Add((5.0f, 1.0f));
            wall_locations.Add((5.0f, 2.0f));
            wall_locations.Add((5.0f, 3.0f));
            wall_locations.Add((5.0f, 4.0f));
            wall_locations.Add((5.0f, 5.0f));

            wall_locations.Add((0.0f, 0.0f));
            
            wall_locations.Add((1.0f, 0.0f));
            
            wall_locations.Add((2.0f, 0.0f));
            wall_locations.Add((3.0f, 0.0f));
            wall_locations.Add((4.0f, 0.0f));
            wall_locations.Add((5.0f, 0.0f));
            
            SpawnWalls();
            //Debug.Log("Finished Loading Walls");
            // Step 2:
            // Add Player into the world
            SpawnPlayer();
            //Debug.Log("Finished Loading Player");
            // Step 3:
            // Add Enemies into the world
            enemy_locations.Add((2.0f, 2.0f));
            enemy_locations.Add((1.0f, 4.0f));

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
            GameObject prefab = environment_asset_manager.GetComponent<EnvironmentAssetsDictionaryManager>().GetAsset("Wall");
            if (prefab == null)
            {
                throw (new AssetNotInDictionaryException("No Asset called: 'Wall' in Dictionary"));
            }
 
            foreach ((float, float) loc in wall_locations)
            {
                Vector2 relative_spawn_loc = new Vector2(spawn_location.x + loc.Item1, spawn_location.y + loc.Item2);
                world_array[(int)loc.Item1, (int)loc.Item2].environment = Instantiate(prefab, relative_spawn_loc, new Quaternion());
                Debug.Log("Spawning wall at location: " + loc.Item1 + "/" + loc.Item2);
                world_array[(int)loc.Item1, (int)loc.Item2].environment.gameObject.tag = "Wall";
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
            GameObject prefab = elemontal_asset_manager.GetComponent<ElemontalAssetsDictionaryManager>().GetAsset("Elemont2");
            if (prefab == null)
            {
                throw (new AssetNotInDictionaryException("No Asset called 'Elemont2' in Dictionary"));
            }

            foreach ((float,float) loc in enemy_locations)
            {
                Vector2 relative_spawn_loc = new Vector2(spawn_location.x + loc.Item1, spawn_location.y + loc.Item2);
                GameObject tmp_enemy = Instantiate(prefab, relative_spawn_loc, new Quaternion());
                tmp_enemy.gameObject.tag = "Enemy";
                tmp_enemy.AddComponent<EnemyMovement>();
                tmp_enemy.GetComponent<EnemyMovement>().SetLocation(loc.Item1, loc.Item2);
                //tmp_enemy.AddComponent<EnemyMovement>();
                world_array[(int)loc.Item1, (int)loc.Item2].character = tmp_enemy;
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
            TakeEnemyTurn();
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

        if (!world_array[(int)player_loc.Item1 + 1, (int)player_loc.Item2 + 1].character.gameObject.name.Equals(EmptyObject.name)&& world_array[(int)player_loc.Item1 + 1, (int)player_loc.Item2 + 1].character.gameObject.tag == "Enemy")
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

    private void TakeEnemyTurn()
    {
        // Wait for all enemies to have moved

        // ----- KIERAN ----- //
        foreach (GameObject TempEnemy in enemy_list)
        {
            THIS_Enemy_TakeTurn(TempEnemy);
        }
    }

    // This will run the turn for the enemy placed into the method
    private void THIS_Enemy_TakeTurn(GameObject _Enemy)
    {
        // ----- KIERAN ----- //
        THIS_Enemy_PopulateMap(_Enemy);
        THIS_Enemy_MakeAction(_Enemy);


    }

    // This will populate the internal map for the inputed enemy
    private void THIS_Enemy_PopulateMap(GameObject _Enemy)
    {
        // ----- KIERAN ----- //

        // Checks the enemy - Sight and Coordinates for each enemy
        float enemySight;
        (float, float) enemyCoordinates;
        (float, float) enemyCoordinates_bottomLeft;

        WorldElements[,] enemy_world_array;

        enemySight = _Enemy.GetComponent<EnemyMovement>().CurrentVision;
        enemyCoordinates = _Enemy.GetComponent<EnemyMovement>().Location;

        // Getting the bottom left cordinate
        enemyCoordinates_bottomLeft.Item1 = enemyCoordinates.Item1 - ((enemySight / 2f) - 0.5f);     // This will be x  (x,y)
        enemyCoordinates_bottomLeft.Item2 = enemyCoordinates.Item2 - ((enemySight / 2f) - 0.5f);     // This will be y  (x,y)

        enemy_world_array = new WorldElements[(int)enemySight, (int)enemySight];

        // Goes through and makes the enemy world array and if out of range will put a wall there
        for (int x = 0; x < enemySight; x++)            // This will be x  (x,y)
        {
            for (int y = 0; y < enemySight; y++)        // This will be y  (x,y)
            {

                // !!!!!!!!!!!!! ------------------------ NEED HELP ------------------------ !!!!!!!!!!!!!
                // Want to populate the enemy_world_array with a wall if the location is out of bounds.

                // If it is not in range will put a wall there.
                if (x < 0 || x >= x_size || y < 0 || y >= y_size)
                {
                    GameObject enemy_WallPrefab = environment_asset_manager.GetComponent<EnvironmentAssetsDictionaryManager>().GetAsset("Wall");
                    if (enemy_WallPrefab == null)
                    {
                        throw (new AssetNotInDictionaryException("No Asset called: 'Wall' in Dictionary"));
                    }

                    Vector2 enemy_Wall_spawn_loc = new Vector2(x, y);
                    enemy_world_array[x, y].environment = Instantiate(enemy_WallPrefab, enemy_Wall_spawn_loc, new Quaternion());
                    enemy_world_array[x, y].environment.gameObject.tag = "Wall";
                }

                // If it is in range will copy world_array over to enemy_world_array.
                else
                {
                    enemy_world_array[x, y] = world_array[(int)(enemyCoordinates_bottomLeft.Item1 + x), (int)(enemyCoordinates_bottomLeft.Item1 + y)];
                }
            }
            _Enemy.GetComponent<EnemyMovement>().Fill_Enemy_array(enemy_world_array);
        }
    }

    private void THIS_Enemy_MakeAction(GameObject _Enemy)
    {
        string THIS_enemyAction;

        // This will produce one of the following strings:
        //      "Attack"                                    - If the player IS in attack range of THIS enemy.
        //      "Move " + "Left" / "Down" / "Right" / "Up"  - If the player IS NOT in attack range, AND IS in vision range of THIS enemy, AND the enemy CAN move towards them).
        //      "Cant Move"                                 - If the player IS NOT in attack range, AND IS in vision range of THIS enemy, AND the enemy CAN NOT move towards them).
        //      "Pass"                                      - If the player IS NOT in attack range, AND IS NOT in vision range of THIS enemy.
        THIS_enemyAction = _Enemy.GetComponent<EnemyMovement>().Take_Turn();

    }
}