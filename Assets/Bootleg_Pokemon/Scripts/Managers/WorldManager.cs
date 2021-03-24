using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldManager : MonoBehaviour
{
    // World Settings and Contents
    private static int x_size = 6;
    private static int y_size = 6;
    public bool level_loaded = false;

    private GameObject[,] world_array = new GameObject[x_size, y_size];
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

    public GameObject AttackButton;

    public GameObject AttackPanel;
    public GameObject AttackMove1;
    public GameObject AttackMove2;
    public GameObject AttackMove3;
    public GameObject AttackMove4;

    // Start is called before the first frame update
    void Start()
    {
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
                world_array[(int)loc.Item1, (int)loc.Item2] = Instantiate(prefab, relative_spawn_loc, new Quaternion());
                world_array[(int)loc.Item1, (int)loc.Item2].gameObject.tag = "Wall";
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
        world_array[(int)player_loc.Item1, (int)player_loc.Item2] = Instantiate(playerPrefab, new Vector2(spawn_location.x + player_loc.Item1, spawn_location.y + player_loc.Item2), new Quaternion());
        player = world_array[(int)player_loc.Item1, (int)player_loc.Item2];
        player.AddComponent<PlayerMovement>();
        player.GetComponent<PlayerMovement>().LearnMove(0, moveset_manager.GetComponent<MoveSetDictionary>().GetMoveset("Blaze"));
        player.GetComponent<PlayerMovement>().LearnMove(1, moveset_manager.GetComponent<MoveSetDictionary>().GetMoveset("Pound"));
        player.GetComponent<PlayerMovement>().LearnMove(2, moveset_manager.GetComponent<MoveSetDictionary>().GetMoveset("Bless"));
        player.GetComponent<PlayerMovement>().LearnMove(3, moveset_manager.GetComponent<MoveSetDictionary>().GetMoveset("Fire Dance"));
        player.GetComponent<PlayerMovement>().SetUI(AttackPanel, AttackMove1, AttackMove2, AttackMove3, AttackMove4);

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
                //tmp_enemy.AddComponent<EnemyMovement>();
                world_array[(int)loc.Item1, (int)loc.Item2] = tmp_enemy;
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
                Debug.Log("Player is looking to attack");
                player_state = player.GetComponent<PlayerMovement>().Attack();
                if(player_state != null)
                {
                    if(PlayerAction(player_state))
                    {
                        // TODO: Attack the enemy intended
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
    }

    public List<GameObject> GetEnemyAroundPlayer()
    {
        Debug.Log("Looking for enemies around player");
        List<GameObject> enemy_list = new List<GameObject>();

        if (world_array[(int)player_loc.Item1, (int)player_loc.Item2 + 1] != null && world_array[(int)player_loc.Item1, (int)player_loc.Item2 + 1].gameObject.tag == "Enemy") enemy_list.Add(world_array[(int)player_loc.Item1, (int)player_loc.Item2].gameObject); //Get top enemy
        if (world_array[(int)player_loc.Item1 + 1, (int)player_loc.Item2] != null && world_array[(int)player_loc.Item1 + 1, (int)player_loc.Item2].gameObject.tag == "Enemy") enemy_list.Add(world_array[(int)player_loc.Item1, (int)player_loc.Item2].gameObject); //Get right enemy
        if (world_array[(int)player_loc.Item1 - 1, (int)player_loc.Item2] != null && world_array[(int)player_loc.Item1 - 1, (int)player_loc.Item2].gameObject.tag == "Enemy") enemy_list.Add(world_array[(int)player_loc.Item1, (int)player_loc.Item2].gameObject); //Get left enemy
        if (world_array[(int)player_loc.Item1, (int)player_loc.Item2 - 1] != null && world_array[(int)player_loc.Item1, (int)player_loc.Item2 - 1].gameObject.tag == "Enemy") enemy_list.Add(world_array[(int)player_loc.Item1, (int)player_loc.Item2].gameObject); //Get bottom enemy

       
        if (enemy_list.Count != 0)
        {
            Debug.Log("enemy is nearby");
            enemy_around_player = true;
            AttackButton.SetActive(true);
        }
        else
        {
            enemy_around_player = false;
            AttackButton.SetActive(false);
            
        }
        return enemy_list;
    }
    private bool PlayerAction(State st)
    {
        bool move_success = false;
        (float, float) new_player_loc = player_loc;
        
        switch (st.GetStateName())
        {
            case "Walk":
                Debug.Log("State is Walk");
                Walk tmp = (Walk)st;
                switch (tmp.GetDirection())
                {
                    case "Up":
                        Debug.Log("Dir is Up");
                        new_player_loc.Item2 += 1.0f;
                        //Debug.Log("New loc:" + new_player_loc.Item1 + " " + new_player_loc.Item2);
                        if (world_array[(int)new_player_loc.Item1, (int)new_player_loc.Item2] == null || 
                            !world_array[(int)new_player_loc.Item1, (int)new_player_loc.Item2].gameObject.tag.Contains("Wall") &&
                            !world_array[(int)new_player_loc.Item1, (int)new_player_loc.Item2].gameObject.tag.Contains("Enemy"))
                        {
                            Debug.Log("Moving Player Up");
                            player.transform.position = new Vector2(spawn_location.x + new_player_loc.Item1, spawn_location.y + new_player_loc.Item2);
                            world_array[(int)new_player_loc.Item1, (int)new_player_loc.Item2] = player;
                            world_array[(int)player_loc.Item1, (int)player_loc.Item2] = null;
                            player_loc = new_player_loc;
                            move_success = true;
                            decide_to_attack = false;

                            player.GetComponent<PlayerMovement>().SetEnemyList(GetEnemyAroundPlayer());

                        }
                        else if (world_array[(int)new_player_loc.Item1, (int)new_player_loc.Item2] != null) Debug.Log("Up is not null");
                        else if (world_array[(int)new_player_loc.Item1, (int)new_player_loc.Item2].gameObject.tag.Contains("Wall")) Debug.Log("Up is a wall");
                        else if (world_array[(int)new_player_loc.Item1, (int)new_player_loc.Item2].gameObject.tag.Contains("Enemy")) Debug.Log("Up is an enemy");
                        else Debug.Log("Something else broke");

                        break;
                    case "Down":
                        //Debug.Log("Dir is Up");
                        new_player_loc.Item2 -= 1.0f;
                        //Debug.Log("New loc:" + new_player_loc.Item1 + " " + new_player_loc.Item2);
                        if (world_array[(int)new_player_loc.Item1, (int)new_player_loc.Item2] == null ||
                            !world_array[(int)new_player_loc.Item1, (int)new_player_loc.Item2].gameObject.tag.Contains("Wall") &&
                            !world_array[(int)new_player_loc.Item1, (int)new_player_loc.Item2].gameObject.tag.Contains("Enemy"))
                        {
                            Debug.Log("Moving Player Down");
                            player.transform.position = new Vector2(spawn_location.x + new_player_loc.Item1, spawn_location.y + new_player_loc.Item2);
                            world_array[(int)new_player_loc.Item1, (int)new_player_loc.Item2] = player;
                            world_array[(int)player_loc.Item1, (int)player_loc.Item2] = null;
                            player_loc = new_player_loc;
                            move_success = true;
                            decide_to_attack = false;

                            player.GetComponent<PlayerMovement>().SetEnemyList(GetEnemyAroundPlayer());
                            
                        }
                        else Debug.Log("There is a Wall Down");
                        break;
                    case "Left":
                        //Debug.Log("Dir is Up");
                        new_player_loc.Item1 -= 1.0f;
                        //Debug.Log("New loc:" + new_player_loc.Item1 + " " + new_player_loc.Item2);
                        if (world_array[(int)new_player_loc.Item1, (int)new_player_loc.Item2] == null ||
                            !world_array[(int)new_player_loc.Item1, (int)new_player_loc.Item2].gameObject.tag.Contains("Wall") &&
                            !world_array[(int)new_player_loc.Item1, (int)new_player_loc.Item2].gameObject.tag.Contains("Enemy"))
                        {
                            Debug.Log("Moving Player Left");
                            player.transform.position = new Vector2(spawn_location.x + new_player_loc.Item1, spawn_location.y + new_player_loc.Item2);
                            world_array[(int)new_player_loc.Item1, (int)new_player_loc.Item2] = player;
                            world_array[(int)player_loc.Item1, (int)player_loc.Item2] = null;
                            player_loc = new_player_loc;
                            move_success = true;
                            decide_to_attack = false;

                            player.GetComponent<PlayerMovement>().SetEnemyList(GetEnemyAroundPlayer());
                          
                        }
                        else Debug.Log("There is a Wall Left");
                        break;
                    case "Right":
                        //Debug.Log("Dir is Up");
                        new_player_loc.Item1 += 1.0f;
                        //Debug.Log("New loc:" + new_player_loc.Item1 + " " + new_player_loc.Item2);
                        if (world_array[(int)new_player_loc.Item1, (int)new_player_loc.Item2] == null ||
                            !world_array[(int)new_player_loc.Item1, (int)new_player_loc.Item2].gameObject.tag.Contains("Wall") &&
                            !world_array[(int)new_player_loc.Item1, (int)new_player_loc.Item2].gameObject.tag.Contains("Enemy"))
                        {
                            Debug.Log("Moving Player Right");
                            player.transform.position = new Vector2(spawn_location.x + new_player_loc.Item1, spawn_location.y + new_player_loc.Item2);
                            world_array[(int)new_player_loc.Item1, (int)new_player_loc.Item2] = player;
                            world_array[(int)player_loc.Item1, (int)player_loc.Item2] = null;
                            player_loc = new_player_loc;
                            move_success = true;
                            decide_to_attack = false;

                            player.GetComponent<PlayerMovement>().SetEnemyList(GetEnemyAroundPlayer());
                            
                        }
                        else Debug.Log("There is a Wall Right");
                        break;
                }
                break;
        }

        return move_success;

    }

    public void DecideToAttackSignal(bool d)
    {
        decide_to_attack = d;
    }
}
