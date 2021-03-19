using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    // World Settings and Contents
    private static int x_size = 6;
    private static int y_size = 6;
    public bool level_loaded = false;

    private GameObject[,] world_array = new GameObject[x_size, y_size];
    private List<GameObject> enemy_list = new List<GameObject>();
    private GameObject player;
    public List<(float, float)> wall_locations = new List<(float, float)>();
    public List<(float, float)> enemy_locations = new List<(float, float)>();

    // Temporary Reference to Player
    public GameObject playerPrefab;

    //Other Managers involved
    public GameObject environment_asset_manager;
    public GameObject elemontal_asset_manager;

    // Turn Base System Variables
    private bool player_turn = true;
    private bool enemy_turn = false;

    public GameObject world_origin;
    private Vector3 spawn_location;

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

            // Step 2:
            // Add Player into the world
            SpawnPlayer();

            // Step 3:
            // Add Enemies into the world
            enemy_locations.Add((2.0f, 2.0f));
            enemy_locations.Add((1.0f, 4.0f));

            SpawnEnemy();

            level_loaded = true; // Level finished loading. Start Turn System after this.

            TurnSystem();
        }
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
            }
        }
        catch(AssetNotInDictionaryException e)
        {
            Debug.LogError(e.Message);
        }
       
    }
    private void SpawnPlayer()
    {
        world_array[0, 0] = Instantiate(playerPrefab, new Vector2(0, 0), new Quaternion());
        world_array[0, 0].AddComponent<PlayerMovement>();
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
        while (player_turn)
        {
            // Wait for player input
            player_turn = false;
        }

        while (enemy_turn)
        {
            // Wait for all enemies to have moved
            enemy_turn = false;
        }
    }
}
