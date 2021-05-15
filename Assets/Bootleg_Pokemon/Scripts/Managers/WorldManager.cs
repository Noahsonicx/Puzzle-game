using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WorldManager : MonoBehaviour
{
    public struct EnemyData
    {
        public string enemy_ID;
        public float x;
        public float y;

        public EnemyData(string _id, float _x, float _y)
        {
            enemy_ID = _id;
            x = _x;
            y = _y;
        }
    }

    public struct ItemData
    {
        public string item_Id;
        public float x;
        public float y;

        public ItemData(string _id, float _x, float _y)
        {
            item_Id = _id;
            x = _x;
            y = _y;
        }
    }

    public struct EnvironmentConstruct
    {
        public string asset_key;
        public (float, float) asset_loc;

        public EnvironmentConstruct(string _key, (float, float) _loc) {

            asset_key = _key;
            asset_loc = _loc;
        }
    }
    public struct ItemConstruct
    {
        public string item_name;
        public (float, float) item_loc;

        public ItemConstruct(string _itemName ,(float,float) _loc)
        {
            item_name = _itemName;
            item_loc = _loc;
        }
    }
    public struct EnemyConstruct
    {
        public string elemontal_key;
        public (float, float) enemy_loc;
        public float cur_health;
        public float max_health;
        public string move1;
        public string move2;
        public string move3;
        public string move4;
        public EnemyConstruct(string _key, float _x, float _y, float _cur_h, float _max_h, string m1, string m2, string m3, string m4 )
        {
            elemontal_key = _key;
            enemy_loc = (_x,_y);
            cur_health = _cur_h;
            max_health = _max_h;
            move1 = m1;
            move2 = m2;
            move3 = m3;
            move4 = m4;
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
        public List<(string, int)> inventoryRecord;
        public PlayerConstruct(string _key, float _x, float _y, float _cur_h, float _max_h, float _cur_e, float _max_e, string m1, string m2, string m3, string m4, List<(string,int)> _inventoryRecord)
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
            inventoryRecord = _inventoryRecord;
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
    [SerializeField]
    private List<GameObject> enemy_list = new List<GameObject>();
    [SerializeField]
    private List<GameObject> item_list = new List<GameObject>();
    private GameObject player;
    public (float, float) player_loc;
    public (float, float) player_locFromLevelCreation = (-1.0f, -1.0f);
    public bool playerDead = false;

    //Construct used to hold data when loading
    public List<EnvironmentConstruct> asset_construct = new List<EnvironmentConstruct>();
    public List<ItemConstruct> item_construct = new List<ItemConstruct>();
    public List<EnemyConstruct> enemy_construct = new List<EnemyConstruct>();
    public PlayerConstruct player_construct;

    // Temporary Reference to Player Prefab
    public GameObject playerPrefab;

    //Other Managers involved
    public GameObject environment_asset_manager;
    public GameObject elemontal_asset_manager;
    public GameObject moveset_manager;
    public GameObject item_manager;

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

    // UI for player's inventory
    public GameObject inventoryPanel;
    public GameObject inventoryButtonPrefab;
    public TextMeshProUGUI emptyInventoryText;

    // UI for main menu
    public GameObject mainMenuPanel;

    // UI for game over 
    public GameObject gameOverPanel;

    // Camera 
    public GameObject mainCamera;

    public void UpdateCameraLocation((float,float) _playerPos)
    {
        mainCamera.transform.position = new Vector3(spawn_location.x + _playerPos.Item1, spawn_location.y + _playerPos.Item2, spawn_location.z);
    }
    
    

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
                    || !elemontal_asset_manager.GetComponent<ElemontalAssetsDictionaryManager>().GetDictionaryReadyStatus()
                    || !item_manager.GetComponent<ItemDictionary>().GetDictionaryReadyStatus())
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

                    SpawnEnvironment();
                    //Debug.Log("Finished Loading Walls");
                    // Step 2:
                    // Add Player into the world
                    SpawnPlayer();
                    //Debug.Log("Finished Loading Player");
                    // Step 3:
                    // Add Enemies into the world
                    SpawnEnemy();
                    //Debug.Log("Finished Loading Enemy");
                    // Step 4:
                    // Add item to the world
                    SpawnItem();
                    
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

    private void SpawnEnvironment()
    {
        
        try
        {
            foreach (EnvironmentConstruct ec in asset_construct)
            {
                GameObject prefab = environment_asset_manager.GetComponent<EnvironmentAssetsDictionaryManager>().GetAsset(ec.asset_key);
                if (prefab == null)
                {
                    throw (new AssetNotInDictionaryException("No Asset called:"+ ec.asset_key + "in Dictionary"));
                }
                Vector2 relative_spawn_loc = new Vector2(spawn_location.x + ec.asset_loc.Item1, spawn_location.y + ec.asset_loc.Item2);
                if (world_array[(int)ec.asset_loc.Item1, (int)ec.asset_loc.Item2] == null) Debug.LogError("WorldArray is Empty for this slot" + (int)ec.asset_loc.Item1 + "/" + (int)ec.asset_loc.Item2);
                if (world_array[(int)ec.asset_loc.Item1, (int)ec.asset_loc.Item2].environment == null) Debug.LogError("Environment field of WorldElement is empty");
                
                world_array[(int)ec.asset_loc.Item1, (int)ec.asset_loc.Item2].environment = Instantiate(prefab, relative_spawn_loc, new Quaternion());
                Debug.Log("Spawning wall at location: " + ec.asset_loc.Item1 + "/" + ec.asset_loc.Item2);
                if (ec.asset_key.Contains("w") || ec.asset_key.Contains("i")) world_array[(int)ec.asset_loc.Item1, (int)ec.asset_loc.Item2].environment.gameObject.tag = "Wall";
                else if (ec.asset_key.Contains("g")) world_array[(int)ec.asset_loc.Item1, (int)ec.asset_loc.Item2].environment.gameObject.tag = "Ground";
                else if (ec.asset_key.Contains("s"))
                {
                    world_array[(int)ec.asset_loc.Item1, (int)ec.asset_loc.Item2].environment.gameObject.tag = "Spawn";
                    player_locFromLevelCreation = ec.asset_loc;
                }
                else if (ec.asset_key.Contains("n")) world_array[(int)ec.asset_loc.Item1, (int)ec.asset_loc.Item2].environment.gameObject.tag = "NextLevelStair";
                // Add More Tags above when spawning more environment element

                world_array[(int)ec.asset_loc.Item1, (int)ec.asset_loc.Item2].environment.gameObject.layer = 6;
                world_array[(int)ec.asset_loc.Item1, (int)ec.asset_loc.Item2].environment.GetComponent<SpriteRenderer>().sortingLayerName = "Environment";


            }
        }
        catch(AssetNotInDictionaryException e)
        {
            Debug.LogError(e.Message);
        }
       
    }

    public void SpawnItem()
    {
        try
        {
            Debug.Log("Item Construct has: '" + item_construct.Count + "' elements");
            int index = 1;
            foreach (ItemConstruct ic in item_construct)
            {
                GameObject prefab = item_manager.GetComponent<ItemDictionary>().GetItem(ic.item_name);
                if (prefab == null)
                {
                    throw (new AssetNotInDictionaryException("No Asset called:" + ic.item_name + " in ItemDictionary"));
                }
                Vector2 relative_spawn_loc = new Vector2(spawn_location.x + ic.item_loc.Item1, spawn_location.y + ic.item_loc.Item2);
                if (world_array[(int)ic.item_loc.Item1, (int)ic.item_loc.Item2] == null) Debug.LogError("WorldArray is Empty for this slot" + (int)ic.item_loc.Item1 + "/" + (int)ic.item_loc.Item2);
                if (world_array[(int)ic.item_loc.Item1, (int)ic.item_loc.Item2].item == null) Debug.LogError("item field of WorldElement is empty");
                world_array[(int)ic.item_loc.Item1, (int)ic.item_loc.Item2].item = Instantiate(prefab, relative_spawn_loc, new Quaternion());
                world_array[(int)ic.item_loc.Item1, (int)ic.item_loc.Item2].item.GetComponent<ItemVisuals>().SetLocation(ic.item_loc.Item1, ic.item_loc.Item2);
                world_array[(int)ic.item_loc.Item1, (int)ic.item_loc.Item2].item.gameObject.layer = 7;
                world_array[(int)ic.item_loc.Item1, (int)ic.item_loc.Item2].item.gameObject.name = ic.item_name + "|" + "ID: " + index;
                world_array[(int)ic.item_loc.Item1, (int)ic.item_loc.Item2].item.gameObject.tag = "Item";
                world_array[(int)ic.item_loc.Item1, (int)ic.item_loc.Item2].item.GetComponent<SpriteRenderer>().sortingLayerName = "Item";
                item_list.Add(world_array[(int)ic.item_loc.Item1, (int)ic.item_loc.Item2].item);
            }
        }
        catch (AssetNotInDictionaryException e)
        {
            Debug.LogError(e.Message);
        }
    }
    private void SpawnPlayer()
    {
        // This should essentially save to file so that character stats are saved between levels and when loading game.
        if (player_construct.player_loc.Item1 == -1 || player_construct.player_loc.Item2 == -1)
        {
            player_loc = player_locFromLevelCreation;
        }
        else player_loc = player_construct.player_loc;

        playerPrefab = elemontal_asset_manager.GetComponent<ElemontalAssetsDictionaryManager>().GetAsset(player_construct.elemontal_key);

        world_array[(int)player_loc.Item1, (int)player_loc.Item2].character = Instantiate(playerPrefab, new Vector2(spawn_location.x + player_loc.Item1, spawn_location.y + player_loc.Item2), new Quaternion());
        player = world_array[(int)player_loc.Item1, (int)player_loc.Item2].character;
        player.name = playerPrefab.name + "| Player";
        player.AddComponent<PlayerMovement>();
        PlayerMovement pl_movement = player.GetComponent<PlayerMovement>();
        pl_movement.SetPlayerLocation(player_loc);
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
        player.gameObject.tag = "Player";
        player.gameObject.layer = 8;
        player.GetComponent<SpriteRenderer>().sortingLayerName = "Character";
        player.GetComponentInChildren<Canvas>().sortingLayerName = "HUD";
        player.AddComponent<InventorySystem>();
        player.GetComponent<InventorySystem>().PrepareInventory();
        Debug.Log("inventory count: " + player_construct.inventoryRecord.Count);
        if(player_construct.inventoryRecord.Count != 0)
        {
            player.GetComponent<InventorySystem>().LoadInventory(player_construct.inventoryRecord);
        }
        player.GetComponent<InventorySystem>().DebugInventoryContent();
        UpdateCameraLocation(player_loc);
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
                tmp_enemy.gameObject.layer = 8;
                tmp_enemy.GetComponent<SpriteRenderer>().sortingLayerName = "Character";
                tmp_enemy.GetComponentInChildren<Canvas>().sortingLayerName = "HUD";
                tmp_enemy.AddComponent<EnemyMovement>();
                tmp_enemy.GetComponent<EnemyMovement>().SetLocation(ec.enemy_loc.Item1, ec.enemy_loc.Item2);
                Debug.Log("Enemy = CurHealth: " + ec.cur_health);
                tmp_enemy.GetComponent<EnemyMovement>().SetCurrentHealth(ec.cur_health);
                tmp_enemy.GetComponent<EnemyMovement>().SetMaxHealth(ec.max_health);
                tmp_enemy.GetComponent<EnemyMovement>().LearnMove(0, moveset_manager.GetComponent<MoveSetDictionary>().GetMoveset(ec.move1));
                tmp_enemy.GetComponent<EnemyMovement>().LearnMove(1, moveset_manager.GetComponent<MoveSetDictionary>().GetMoveset(ec.move2));
                tmp_enemy.GetComponent<EnemyMovement>().LearnMove(2, moveset_manager.GetComponent<MoveSetDictionary>().GetMoveset(ec.move3));
                tmp_enemy.GetComponent<EnemyMovement>().LearnMove(3, moveset_manager.GetComponent<MoveSetDictionary>().GetMoveset(ec.move4));
                world_array[(int)ec.enemy_loc.Item1, (int)ec.enemy_loc.Item2].character = tmp_enemy;
                enemy_list.Add(tmp_enemy);

                TextMeshProUGUI[] textChildrens = tmp_enemy.GetComponentsInChildren<TextMeshProUGUI>();

                foreach (TextMeshProUGUI text in textChildrens)
                {
                    if (text.gameObject.name == "Energy")
                    {
                        text.gameObject.SetActive(false);
                    }
                }

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
        if (!playerDead)
        {
            if (player_turn)
            {
                if(Input.GetKeyDown("p"))
                {
                    if(!inventoryPanel.activeSelf)
                    {
                        DebugInventoryContent();
                        ReadyInventoryButtons();
                        inventoryPanel.SetActive(true);
                    }
                    else
                    {
                        ResetInventoryButtons();
                        inventoryPanel.SetActive(false);
                    }
                }

                GetEnemyAroundPlayer();
                player.GetComponent<PlayerMovement>().SetEnemyList(enemy_around_player_list);
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
                    if (player_state != null)
                    {
                        Debug.Log("State received");
                        if (player_state.GetStateName().Equals("Attack")) Debug.Log("State is Attack");
                        if (PlayerAction(player_state))
                        {
                            player_turn = false;
                            enemy_turn = true;
                            decide_to_attack = false;
                        }

                    }
                }

            }

            if (enemy_turn)
            {
                TakeEnemyTurn();
                enemy_turn = false;
                player_turn = true;
            }
        }
        
    }

    public void DebugInventoryContent()
    {
        foreach (var itemType in player.GetComponent<InventorySystem>().GetInventory())
        {
            Debug.Log("DebugInventoryWorldManager-Item type: " + itemType.Item1);
            foreach (var item in itemType.Item2)
            {
                Debug.Log("DebugInventoryWorldManager-Item name: " + item.GetItem().GetItemName() + " Item Quantity: " + item.GetQuantity().ToString());

            }
        }
    }
                
    public void ReadyInventoryButtons()
    {
        var playerInventory = player.GetComponent<InventorySystem>().GetInventory();

        GameObject scrollViewContent = null;

        foreach(RectTransform rt in inventoryPanel.GetComponentsInChildren<RectTransform>())
        {
            if(rt.name.Contains("Content"))
            {
                scrollViewContent = rt.gameObject;
                scrollViewContent.AddComponent<GridLayout>();
            }
        }

        if(playerInventory.Count == 0)
        {
            emptyInventoryText.enabled = true;
            return;
        }
        else if(playerInventory.Count > 0)
        {
            emptyInventoryText.enabled = false;
        }

        if(scrollViewContent == null) 
        {
            Debug.LogError("Cannot find scrollview");
            return;
        }

        foreach(var itemCategory in playerInventory)
        {
            foreach(var item in itemCategory.Item2)
            {
                Debug.Log("Item: " + item.GetItem().GetItemName() + " with quantity: " + item.GetQuantity());
                if(item.GetQuantity() > 0)
                {
                    GameObject itemBtn = Instantiate(inventoryButtonPrefab, scrollViewContent.transform);
                    foreach (TextMeshProUGUI txt in itemBtn.GetComponentsInChildren<TextMeshProUGUI>())
                    {
                        if (txt.name == "Name") txt.text = item.GetItem().GetItemName();
                        if (txt.name == "Quantity") txt.text = "x"+item.GetQuantity().ToString();
                    }

                    itemBtn.GetComponent<Button>().onClick.AddListener(delegate { UseItemInInventory(item.GetItem().GetItemName()); });
                }
            }

        }
    }

    public void ResetInventoryButtons()
    {
        var playerInventory = player.GetComponent<InventorySystem>().GetInventory();

        GameObject scrollViewContent = null;

        foreach (RectTransform rt in inventoryPanel.GetComponentsInChildren<RectTransform>())
        {
            if (rt.name.Contains("Content"))
            {
                scrollViewContent = rt.gameObject;
                scrollViewContent.AddComponent<GridLayout>();
            }
        }

        foreach(var btn in scrollViewContent.GetComponentsInChildren<Button>())
        {
            Debug.Log("found button in scrollview");
            Destroy(btn.gameObject);
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

    public void AnimateCharacter(GameObject character, string state) 
    {
        Animator animator = character.GetComponent<Animator>();

        switch (state)
        {
            case "WalkUp":
                Debug.Log("DebugAnimateCharacter- state is:" + state);
                animator.SetTrigger(state);
                
                break;
            case "WalkDown":
                Debug.Log("DebugAnimateCharacter- state is:" + state);
                animator.SetTrigger(state);
                break;
            case "WalkLeft":
                Debug.Log("DebugAnimateCharacter- state is:" + state);
                animator.SetTrigger(state);
                break;
            case "WalkRight":
                Debug.Log("DebugAnimateCharacter- state is:" + state);
                animator.SetTrigger(state);
                break;
            case "Attack":
                Debug.Log("DebugAnimateCharacter- state is:" + state);
                animator.SetTrigger(state);
                break;
            case "TakeDamage":
                Debug.Log("DebugAnimateCharacter- state is:" + state);
                animator.SetTrigger(state);
                break;
            case "Death":
                Debug.Log("DebugAnimateCharacter- state is:" + state);
                animator.SetBool(state, true);
                break;
            default:
                Debug.LogError("State Passed in to AnimateCharacter does not correspond to any parameters");
                break;
        }

    }

    public void GetEnemyAroundPlayer()
    {
        //Debug.Log("Looking for enemies around player");


        if (!world_array[(int)player_loc.Item1, (int)player_loc.Item2 + 1].character.gameObject.name.Equals(EmptyObject.name)&& world_array[(int)player_loc.Item1, (int)player_loc.Item2 + 1].character.gameObject.tag == "Enemy")
        {
            //Debug.Log("There is an enemy on top");
            enemy_around_player_list[0] = (world_array[(int)player_loc.Item1, (int)player_loc.Item2 + 1].character); //Get top enemy
            if (enemy_around_player == EmptyObject) Debug.Log("No Enemy Found");
            else
            {
                Debug.Log("Enemy at, X:" + (int)player_loc.Item1 + "Y: " + ((int)player_loc.Item2 + 1));
            }
        }
        else enemy_around_player_list[0] = EmptyObject; //Get top enemy

        if (!world_array[(int)player_loc.Item1 + 1, (int)player_loc.Item2 + 1].character.gameObject.name.Equals(EmptyObject.name) && world_array[(int)player_loc.Item1 + 1, (int)player_loc.Item2 + 1].character.gameObject.tag == "Enemy")
        {
            //Debug.Log("There is an enemy on top");
            enemy_around_player_list[1] = (world_array[(int)player_loc.Item1 + 1, (int)player_loc.Item2 + 1].character); //Get top-right enemy
            if (enemy_around_player == EmptyObject) Debug.Log("No Enemy Found");
            else
            {
                Debug.Log("Enemy at, X:" + ((int)player_loc.Item1 + 1) + "Y: " + ((int)player_loc.Item2 + 1));
            }
        }
        else enemy_around_player_list[1] = EmptyObject; //Get top right enemy

        if (!world_array[(int)player_loc.Item1 + 1, (int)player_loc.Item2].character.gameObject.name.Equals(EmptyObject.name)&& world_array[(int)player_loc.Item1 + 1, (int)player_loc.Item2].character.gameObject.tag == "Enemy")
        {
            //Debug.Log("There is an enemy on right");
            enemy_around_player_list[2] = (world_array[(int)player_loc.Item1 + 1, (int)player_loc.Item2].character); //Get right enemy
            if (enemy_around_player == EmptyObject) Debug.Log("No Enemy Found");
            else
            {
                Debug.Log("Enemy at, X:" + ((int)player_loc.Item1 + 1) + "Y: " + (int)player_loc.Item2);
            }
        }
        else enemy_around_player_list[2] = EmptyObject; //Get right enemy

        if (!world_array[(int)player_loc.Item1 + 1, (int)player_loc.Item2 - 1].character.gameObject.name.Equals(EmptyObject.name)&& world_array[(int)player_loc.Item1 + 1, (int)player_loc.Item2 - 1].character.gameObject.tag == "Enemy")
        {
            //Debug.Log("There is an enemy on right");
            enemy_around_player_list[3] = (world_array[(int)player_loc.Item1 + 1, (int)player_loc.Item2 - 1].character); //Get bottom-right enemy
            if (enemy_around_player == EmptyObject) Debug.Log("No Enemy Found");
            else
            {
                Debug.Log("Enemy at, X:" + ((int)player_loc.Item1 + 1) + "Y: " + ((int)player_loc.Item2 - 1));
            }
        }
        else enemy_around_player_list[3] = EmptyObject; //Get bottom-right enemy
        
        if (!world_array[(int)player_loc.Item1, (int)player_loc.Item2 - 1].character.gameObject.name.Equals(EmptyObject.name) && world_array[(int)player_loc.Item1, (int)player_loc.Item2 - 1].character.gameObject.tag == "Enemy")
        {
            //Debug.Log("There is an enemy to the bottom");
            enemy_around_player_list[4] = (world_array[(int)player_loc.Item1, (int)player_loc.Item2-1].character); //Get bottom enemy
            if (enemy_around_player == EmptyObject) Debug.Log("No Enemy Found");
            else
            {
                Debug.Log("Enemy at, X:" + ((int)player_loc.Item1) + "Y: " + ((int)player_loc.Item2 - 1));
            }
        }
        else enemy_around_player_list[4] = EmptyObject; //Get bottom enemy

        if (!world_array[(int)player_loc.Item1 - 1, (int)player_loc.Item2 - 1].character.gameObject.name.Equals(EmptyObject.name) && world_array[(int)player_loc.Item1 - 1, (int)player_loc.Item2 - 1].character.gameObject.tag == "Enemy")
        {
            //Debug.Log("There is an enemy to the bottom");
            enemy_around_player_list[5] = (world_array[(int)player_loc.Item1 - 1, (int)player_loc.Item2 - 1].character); //Get bottom-left enemy
            if (enemy_around_player == EmptyObject) Debug.Log("No Enemy Found");
            else
            {
                Debug.Log("Enemy at, X:" + ((int)player_loc.Item1 - 1) + "Y: " + ((int)player_loc.Item2 - 1));
            }
        }
        else enemy_around_player_list[5] = EmptyObject; //Get bottom-left enemy

        if (!world_array[(int)player_loc.Item1 - 1, (int)player_loc.Item2].character.gameObject.name.Equals(EmptyObject.name) && world_array[(int)player_loc.Item1 - 1, (int)player_loc.Item2].character.gameObject.tag == "Enemy") 
        {
            //Debug.Log("There is an enemy to the left");
            enemy_around_player_list[6] = (world_array[(int)player_loc.Item1 - 1, (int)player_loc.Item2].character); //Get left enemy
            if (enemy_around_player == EmptyObject) Debug.Log("No Enemy Found");
            else
            {
                Debug.Log("Enemy at, X:" + ((int)player_loc.Item1 - 1) + "Y: " + ((int)player_loc.Item2));
            }
        }
        else enemy_around_player_list[6] = (EmptyObject); //Get left enemy

        if (!world_array[(int)player_loc.Item1 - 1, (int)player_loc.Item2 + 1].character.gameObject.name.Equals(EmptyObject.name) && world_array[(int)player_loc.Item1 - 1, (int)player_loc.Item2 + 1].character.gameObject.tag == "Enemy")
        {
            //Debug.Log("There is an enemy to the left");
            enemy_around_player_list[7] = (world_array[(int)player_loc.Item1 - 1, (int)player_loc.Item2 + 1].character); //Get top-left enemy
            if (enemy_around_player == EmptyObject) Debug.Log("No Enemy Found");
            else
            {
                Debug.Log("Enemy at, X:" + ((int)player_loc.Item1 - 1) + "Y: " + ((int)player_loc.Item2 + 1));
            }
        }
        else enemy_around_player_list[7] = (EmptyObject); //Get top-left enemy
   

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
        (float, float) old_player_loc = player.GetComponent<PlayerMovement>().GetPlayerLocation();
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
                            if(world_array[(int)new_player_loc.Item1, (int)new_player_loc.Item2].environment.gameObject.tag.Contains("NextLevelStair"))
                            {
                                // Player reached the stair for the next level
                                InterimLevelSave();
                                LoadNextLevel();
                            }
                            if (world_array[(int)new_player_loc.Item1, (int)new_player_loc.Item2].item.gameObject.tag.Contains("Item"))
                            {
                                // Player is stepping on an item. Automatically pick it up
                                player.GetComponent<InventorySystem>().PickupItem(world_array[(int)new_player_loc.Item1, (int)new_player_loc.Item2].item);
                                world_array[(int)new_player_loc.Item1, (int)new_player_loc.Item2].item.GetComponent<SpriteRenderer>().enabled = false;
                                if (item_list.Remove(world_array[(int)new_player_loc.Item1, (int)new_player_loc.Item2].item)) Debug.Log("Item" + world_array[(int)new_player_loc.Item1, (int)new_player_loc.Item2].item.GetComponent<ItemVisuals>().itemName + "removed successfully");
                                //Destroy(world_array[(int)new_player_loc.Item1, (int)new_player_loc.Item2].item.gameObject);
                                world_array[(int)new_player_loc.Item1, (int)new_player_loc.Item2].item = EmptyObject;
                            }
                            //Debug.Log("Moving Player Up");
                            AnimateCharacter(player, "WalkUp");
                            player.transform.position = new Vector2(spawn_location.x + new_player_loc.Item1, spawn_location.y + new_player_loc.Item2);
                            world_array[(int)new_player_loc.Item1, (int)new_player_loc.Item2].character = player;
                            //world_array[(int)player_loc.Item1, (int)player_loc.Item2].character = null;
                            world_array[(int)old_player_loc.Item1, (int)old_player_loc.Item2].character = EmptyObject;
                            old_player_loc = new_player_loc;
                            player_loc = new_player_loc;
                            player.GetComponent<PlayerMovement>().SetPlayerLocation(new_player_loc);
                            move_success = true;
                            decide_to_attack = false;
                            player.GetComponent<PlayerMovement>().ResetMoveAndEnemy();
                            ResetUI();

                            UpdateCameraLocation(player_loc);

                        }
                        else if (world_array[(int)new_player_loc.Item1, (int)new_player_loc.Item2] == null) Debug.Log("Up is null");
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
                            if (world_array[(int)new_player_loc.Item1, (int)new_player_loc.Item2].environment.gameObject.tag.Contains("NextLevelStair"))
                            {
                                // Player reached the stair for the next level
                                InterimLevelSave();
                                LoadNextLevel();
                            }
                            if (world_array[(int)new_player_loc.Item1, (int)new_player_loc.Item2].item.gameObject.tag.Contains("Item"))
                            {
                                // Player is stepping on an item. Automatically pick it up
                                player.GetComponent<InventorySystem>().PickupItem(world_array[(int)new_player_loc.Item1, (int)new_player_loc.Item2].item);
                                if (item_list.Remove(world_array[(int)new_player_loc.Item1, (int)new_player_loc.Item2].item)) Debug.Log("Item" + world_array[(int)new_player_loc.Item1, (int)new_player_loc.Item2].item.GetComponent<ItemVisuals>().itemName + "removed successfully");
                                Destroy(world_array[(int)new_player_loc.Item1, (int)new_player_loc.Item2].item);
                                world_array[(int)new_player_loc.Item1, (int)new_player_loc.Item2].item = EmptyObject;
                            }
                            Debug.Log("Moving Player Down");
                            AnimateCharacter(player, "WalkDown");
                            player.transform.position = new Vector2(spawn_location.x + new_player_loc.Item1, spawn_location.y + new_player_loc.Item2);
                            world_array[(int)new_player_loc.Item1, (int)new_player_loc.Item2].character = player;
                            world_array[(int)old_player_loc.Item1, (int)old_player_loc.Item2].character = EmptyObject;
                            old_player_loc = new_player_loc;
                            player_loc = new_player_loc;
                            player.GetComponent<PlayerMovement>().SetPlayerLocation(new_player_loc);
                            move_success = true;
                            decide_to_attack = false;
                            Debug.Log("Getting Enemy Around player");
                            player.GetComponent<PlayerMovement>().ResetMoveAndEnemy();
                            ResetUI();
                            UpdateCameraLocation(player_loc);
                            
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
                            if (world_array[(int)new_player_loc.Item1, (int)new_player_loc.Item2].environment.gameObject.tag.Contains("NextLevelStair"))
                            {
                                // Player reached the stair for the next level
                                InterimLevelSave();
                                LoadNextLevel();
                            }
                            if (world_array[(int)new_player_loc.Item1, (int)new_player_loc.Item2].item.gameObject.tag.Contains("Item"))
                            {
                                // Player is stepping on an item. Automatically pick it up
                                player.GetComponent<InventorySystem>().PickupItem(world_array[(int)new_player_loc.Item1, (int)new_player_loc.Item2].item);
                                if (item_list.Remove(world_array[(int)new_player_loc.Item1, (int)new_player_loc.Item2].item)) Debug.Log("Item" + world_array[(int)new_player_loc.Item1, (int)new_player_loc.Item2].item.GetComponent<ItemVisuals>().itemName + "removed successfully");
                                Destroy(world_array[(int)new_player_loc.Item1, (int)new_player_loc.Item2].item);
                                world_array[(int)new_player_loc.Item1, (int)new_player_loc.Item2].item = EmptyObject;
                            }
                            Debug.Log("Moving Player Left");
                            AnimateCharacter(player, "WalkLeft");
                            player.transform.position = new Vector2(spawn_location.x + new_player_loc.Item1, spawn_location.y + new_player_loc.Item2);
                            world_array[(int)new_player_loc.Item1, (int)new_player_loc.Item2].character = player;
                            world_array[(int)old_player_loc.Item1, (int)old_player_loc.Item2].character = EmptyObject;
                            Debug.Log("Spot left by player is:" + world_array[(int)player_loc.Item1, (int)player_loc.Item2].character.gameObject.name);
                            old_player_loc = new_player_loc;
                            player_loc = new_player_loc;
                            player.GetComponent<PlayerMovement>().SetPlayerLocation(new_player_loc);
                            move_success = true;
                            decide_to_attack = false;
                            player.GetComponent<PlayerMovement>().ResetMoveAndEnemy();
                            ResetUI();
                            UpdateCameraLocation(player_loc);                          
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
                            if (world_array[(int)new_player_loc.Item1, (int)new_player_loc.Item2].environment.gameObject.tag.Contains("NextLevelStair"))
                            {
                                // Player reached the stair for the next level
                                InterimLevelSave();
                                LoadNextLevel();
                            }
                            if (world_array[(int)new_player_loc.Item1, (int)new_player_loc.Item2].item.gameObject.tag.Contains("Item"))
                            {
                                // Player is stepping on an item. Automatically pick it up
                                player.GetComponent<InventorySystem>().PickupItem(world_array[(int)new_player_loc.Item1, (int)new_player_loc.Item2].item);
                                if (item_list.Remove(world_array[(int)new_player_loc.Item1, (int)new_player_loc.Item2].item)) Debug.Log("Item" + world_array[(int)new_player_loc.Item1, (int)new_player_loc.Item2].item.GetComponent<ItemVisuals>().itemName + "removed successfully");
                                Destroy(world_array[(int)new_player_loc.Item1, (int)new_player_loc.Item2].item);
                                world_array[(int)new_player_loc.Item1, (int)new_player_loc.Item2].item = EmptyObject;
                            }
                            Debug.Log("Moving Player Right");
                            AnimateCharacter(player, "WalkRight");
                            player.transform.position = new Vector2(spawn_location.x + new_player_loc.Item1, spawn_location.y + new_player_loc.Item2);
                            world_array[(int)new_player_loc.Item1, (int)new_player_loc.Item2].character = player;
                            world_array[(int)old_player_loc.Item1, (int)old_player_loc.Item2].character = EmptyObject;
                            old_player_loc = new_player_loc;
                            player_loc = new_player_loc;
                            player.GetComponent<PlayerMovement>().SetPlayerLocation(new_player_loc);
                            move_success = true;
                            decide_to_attack = false;                            
                            player.GetComponent<PlayerMovement>().ResetMoveAndEnemy();
                            ResetUI();
                            UpdateCameraLocation(player_loc);
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
                    AnimateCharacter(player, "Attack");
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
                            AnimateCharacter(player, "Attack");
                            if (atk_state.target.gameObject.GetComponent<EnemyMovement>().TakeDamage(atk_state.move.GetValue()))
                            {
                                var tmp_x = atk_state.target.GetComponent<EnemyMovement>().location.Item1;
                                var tmp_y = atk_state.target.GetComponent<EnemyMovement>().location.Item2;
                                AnimateCharacter(world_array[(int)tmp_x, (int)tmp_y].character, "Death");
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
    public List<GameObject> GetItemList()
    {
        return item_list;
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
        asset_construct.Add(new EnvironmentConstruct(_key, _loc));        
    }

    public void LoadItem(string item_name,(float,float) _loc)
    {
        item_construct.Add(new ItemConstruct(item_name, _loc));
    }
    public void LoadEnemies(string _key, float _enemy_x, float _enemy_y, float _cur_hp, float _max_hp, string m1, string m2, string m3, string m4)
    {
        enemy_construct.Add(new EnemyConstruct(_key, _enemy_x, _enemy_y, _cur_hp, _max_hp, m1, m2 , m3, m4));
    }
 
    public void LoadPlayer(string _key, float _enemy_x, float _enemy_y, float _cur_hp, float _max_hp, float _cur_energy, float _max_energy,string m1, string m2, string m3, string m4, List<(string,int)> _itemRecord)
    {
        player_construct = new PlayerConstruct(_key, _enemy_x, _enemy_y, _cur_hp, _max_hp, _cur_energy, _max_energy, m1, m2, m3, m4, _itemRecord);
    }

    public void PrepareWorld()
    {

        mainMenuPanel.SetActive(false);
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
    public void DestroyWorld()
    {
        level_loaded = false;
        load_data_ready = false;
        if (world_array != null)
        {
            for (int x = 0; x < x_size; x++)
            {
                for (int y = 0; y < y_size; y++)
                {
                    if (world_array[x, y].character.name != EmptyObject.name)
                    {
                        Destroy(world_array[x, y].character.gameObject);
                        world_array[x, y].character = EmptyObject;
                    }
                    if (world_array[x, y].environment.name != EmptyObject.name)
                    {
                        Destroy(world_array[x, y].environment.gameObject);
                        world_array[x, y].environment = EmptyObject;
                    }
                    if (world_array[x, y].item.name != EmptyObject.name)
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
        item_construct.Clear();
        if (player != null) ResetInventoryButtons();
        playerDead = false;
        AttackButton.SetActive(false);
    }
    public void ResetWorld()
    {
        DestroyWorld();
        PrepareWorld();
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
        Debug.Log("I'm in this enemy populate map");
        float enemySight;
        (float, float) enemyCoordinates;

        enemySight = _Enemy.GetComponent<EnemyMovement>().CurrentVision; //Vision to see currentvision X currentvision sized grid
        enemyCoordinates = _Enemy.GetComponent<EnemyMovement>().Location;

        float halfEnemySight = (float)Math.Floor(enemySight / 2.0f);

        WorldElements[,] enemyVisionArray = new WorldElements[(int)enemySight, (int)enemySight];

        int x_worldArray = (int)enemyCoordinates.Item1 - (int)halfEnemySight;
        int y_worldArray = (int)enemyCoordinates.Item2 - (int)halfEnemySight;

        int x_maxWorldArray = (int)enemyCoordinates.Item1 + (int)halfEnemySight;
        int y_maxWorldArray = (int)enemyCoordinates.Item2 + (int)halfEnemySight;

        int x_enemyArray = 0;
        int y_enemyArray = 0;

        Debug.Log("Enemy Pos:" + enemyCoordinates.Item1 + "/" + enemyCoordinates.Item2);
        Debug.Log("halfEnemySight:" + halfEnemySight);
        Debug.Log("x_minWA:" + x_worldArray);
        Debug.Log("y_minWA:" + y_worldArray);
        Debug.Log("x_maxWA:" + x_maxWorldArray);
        Debug.Log("y_maxWA:" + y_maxWorldArray);
        while (x_worldArray <= x_maxWorldArray && x_enemyArray < enemySight)
        {

            while (y_worldArray <= y_maxWorldArray && y_enemyArray < enemySight)
            {


                if ((x_worldArray >= 0 && x_worldArray < x_size) && (y_worldArray >= 0 && y_worldArray < y_size))
                {
                    Debug.Log("Adding element to enemy array");
                    enemyVisionArray[x_enemyArray, y_enemyArray] = world_array[x_worldArray, y_worldArray];
                    //Debug.Log("Character of element copied:" + world_array[x_worldArray, y_worldArray].character.gameObject.name);
                    if (enemyVisionArray[x_enemyArray, y_enemyArray] == null)
                    {
                        Debug.Log("enemyVisionArray Element is null when populating array");
                    }
                    if (world_array[x_worldArray, y_worldArray] == null)
                    {
                        Debug.Log("WorldArray Element is null when populating array");
                    }
                }
                else
                {

                    Debug.Log("Inserting element outside of world array as empty WorldElements");
                    Debug.Log("x_WA_null:" + x_worldArray + " y_WA:" + y_worldArray);
                    Debug.Log("x_EA:null" + x_enemyArray + " y_EA:" + y_enemyArray);
                    enemyVisionArray[x_enemyArray, y_enemyArray] = new WorldElements(EmptyObject);
                }

                y_worldArray++;
                y_enemyArray++;
            }

            y_worldArray = (int)enemyCoordinates.Item2 - (int)halfEnemySight;
            y_enemyArray = 0;

            x_worldArray++;
            x_enemyArray++;
        }
        PrintMatrix(world_array);
        _Enemy.GetComponent<EnemyMovement>().Fill_Enemy_array(enemyVisionArray);
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
                    if (array[x, y].character == EmptyObject && array[x, y].environment == EmptyObject) Debug.Log("Empty Spot");
                    else
                    {
                        Debug.Log("Character is: " + array[x, y].character.gameObject + "\n" + "Wall is: " + array[x, y].environment.gameObject);
                    }
                }
                else
                {
                    Debug.Log("Dumbass its null");
                }
            }
        }

        Debug.Log("Counter:" + counter);
    }

    private void THIS_Enemy_MakeAction(GameObject _Enemy)
    {
    
        // This will produce one of the following strings:
        //      "Attack"                                    - If the player IS in attack range of THIS enemy.
        //      "Move " + "Left" / "Down" / "Right" / "Up"  - If the player IS NOT in attack range, AND IS in vision range of THIS enemy, AND the enemy CAN move towards them).
        //      "Cant Move"                                 - If the player IS NOT in attack range, AND IS in vision range of THIS enemy, AND the enemy CAN NOT move towards them).
        //      "Pass"                                      - If the player IS NOT in attack range, AND IS NOT in vision range of THIS enemy.
        
        State st = _Enemy.GetComponent<EnemyMovement>().Take_Turn();
        (float, float) old_enemy_loc = _Enemy.GetComponent<EnemyMovement>().GetLocation();
        (float, float) new_enemy_loc = _Enemy.GetComponent<EnemyMovement>().GetLocation();


        switch (st.GetStateName())
        {
            case "Attack":

                Attack stAttack = (Attack)st;

                Debug.Log("Target is:" + stAttack.target + "Move is:" + stAttack.move.GetMoveName());

                switch(stAttack.move.GetStatAffected())
                {
                    case "Health":
                        stAttack.target.GetComponent<PlayerMovement>().TakeDamage(stAttack.move.GetValue());
                        if(stAttack.target.GetComponent<PlayerMovement>().GetCurrentHealth() <= 0)
                        {
                            float tmp_x = stAttack.target.GetComponent<PlayerMovement>().GetPlayerLocation().Item1;
                            float tmp_y = stAttack.target.GetComponent<PlayerMovement>().GetPlayerLocation().Item2;
                            AnimateCharacter(world_array[(int)tmp_x, (int)tmp_y].character, "Death");
                            Destroy(world_array[(int)tmp_x, (int)tmp_y].character);
                            world_array[(int)tmp_x, (int)tmp_y].character = EmptyObject;
                            playerDead = true;
                            gameOverPanel.SetActive(true);
                        }
                        break;
                }

                ////Debug.Log("Target is Player");
                ////switch (atk_state.move.GetStatAffected())
                ////{
                ////    case "Health":
                ////        Debug.Log("Enemy Health Affected");
                ////        if (atk_state.target.gameObject.GetComponent<EnemyMovement>().TakeDamage(atk_state.move.GetValue()))
                ////        {
                ////            var tmp_x = atk_state.target.GetComponent<EnemyMovement>().location.Item1;
                ////            var tmp_y = atk_state.target.GetComponent<EnemyMovement>().location.Item2;
                ////            Destroy(world_array[(int)tmp_x, (int)tmp_y].character);
                ////            world_array[(int)tmp_x, (int)tmp_y].character = EmptyObject;

                ////        }
                ////        break;
                ////    case "Mana":
                ////        Debug.Log("Enemy Mana Affected");
                ////        // Add more stats later   
                ////        break;

            break;
            case ("Walk"):
                
                Walk st_Walk = (Walk)st;
                switch (st_Walk.GetDirection())
                {

                    case "Left":
                        // Find Enemy Location
                        new_enemy_loc.Item1 -= 1.0f;
                        // Move enemy to new location;
                        AnimateCharacter(_Enemy, "WalkLeft");
                        _Enemy.transform.position = new Vector2(spawn_location.x + new_enemy_loc.Item1, spawn_location.y + new_enemy_loc.Item2);
                        world_array[(int)new_enemy_loc.Item1, (int)new_enemy_loc.Item2].character = _Enemy;
                        world_array[(int)old_enemy_loc.Item1, (int)old_enemy_loc.Item2].character = EmptyObject;
                        // Set old location to empty object
                        _Enemy.GetComponent<EnemyMovement>().SetLocation(new_enemy_loc.Item1, new_enemy_loc.Item2);
                        break;
                    case "Down":
                        // Find Enemy Location
                        new_enemy_loc.Item2 -= 1.0f;
                        // Move enemy to new location;
                        AnimateCharacter(_Enemy, "WalkDown");
                        _Enemy.transform.position = new Vector2(spawn_location.x + new_enemy_loc.Item1, spawn_location.y + new_enemy_loc.Item2);
                        world_array[(int)new_enemy_loc.Item1, (int)new_enemy_loc.Item2].character = _Enemy;
                        world_array[(int)old_enemy_loc.Item1, (int)old_enemy_loc.Item2].character = EmptyObject;
                        // Set old location to empty object
                        _Enemy.GetComponent<EnemyMovement>().SetLocation(new_enemy_loc.Item1, new_enemy_loc.Item2);
                        break;
                    case "Right":
                        // Find Enemy Location
                        new_enemy_loc.Item1 += 1.0f;
                        // Move enemy to new location;
                        AnimateCharacter(_Enemy, "WalkRight");
                        _Enemy.transform.position = new Vector2(spawn_location.x + new_enemy_loc.Item1, spawn_location.y + new_enemy_loc.Item2);
                        world_array[(int)new_enemy_loc.Item1, (int)new_enemy_loc.Item2].character = _Enemy;
                        world_array[(int)old_enemy_loc.Item1, (int)old_enemy_loc.Item2].character = EmptyObject;
                        // Set old location to empty object
                        _Enemy.GetComponent<EnemyMovement>().SetLocation(new_enemy_loc.Item1, new_enemy_loc.Item2);
                        break;
                    case "Up":
                        // Find Enemy Location
                        new_enemy_loc.Item2 += 1.0f;
                        // Move enemy to new location;
                        AnimateCharacter(_Enemy, "WalkUp");
                        _Enemy.transform.position = new Vector2(spawn_location.x + new_enemy_loc.Item1, spawn_location.y + new_enemy_loc.Item2);
                        world_array[(int)new_enemy_loc.Item1, (int)new_enemy_loc.Item2].character = _Enemy;
                        world_array[(int)old_enemy_loc.Item1, (int)old_enemy_loc.Item2].character = EmptyObject;
                        // Set old location to empty object
                        _Enemy.GetComponent<EnemyMovement>().SetLocation(new_enemy_loc.Item1, new_enemy_loc.Item2);
                        break;
                }
            break;   
        }
    }

    public void UseItemInInventory(string itemName)
    {
        player_turn = false;
        enemy_turn = true;
        player.GetComponent<InventorySystem>().UseItem(itemName);
        ResetInventoryButtons();
        inventoryPanel.SetActive(false);
    }

    public void LoadNextLevel()
    {
        ResetWorld();
        string defaultSaveFilePath = Application.dataPath + @"\Bootleg_Pokemon\ConfigData\SaveData\interimsaveslot.txt";
        FileStream fsInterim = File.OpenRead(defaultSaveFilePath);
        StreamReader srInterim = new StreamReader(fsInterim);

        string saveLine = srInterim.ReadLine();
        saveLine = srInterim.ReadLine();
        string currentDungeon = saveLine.Substring(saveLine.IndexOf(": ") + 2);

        saveLine = srInterim.ReadLine();
        if (saveLine != "PlayerStatistics:") Debug.LogError("Text Differs for: " + saveLine);
        saveLine = srInterim.ReadLine();
        string[] player_stat = saveLine.Split(',');
        string player_elemont = player_stat[0];
        float player_x = float.Parse(player_stat[1], CultureInfo.InvariantCulture.NumberFormat);
        float player_y = float.Parse(player_stat[2], CultureInfo.InvariantCulture.NumberFormat);
        float player_cur_hp = float.Parse(player_stat[3], CultureInfo.InvariantCulture.NumberFormat);
        float player_max_hp = float.Parse(player_stat[4], CultureInfo.InvariantCulture.NumberFormat);
        float player_cur_energy = float.Parse(player_stat[5], CultureInfo.InvariantCulture.NumberFormat);
        float player_max_energy = float.Parse(player_stat[6], CultureInfo.InvariantCulture.NumberFormat);


        saveLine = srInterim.ReadLine();
        if (saveLine != "PlayerMoveset:") Debug.LogError("Text Differs for: " + saveLine);
        saveLine = srInterim.ReadLine();
        string[] player_moves = saveLine.Split(',');
        string move1 = player_moves[0];
        string move2 = player_moves[1];
        string move3 = player_moves[2];
        string move4 = player_moves[3];

        List<(string, int)> inventoryRecord = new List<(string, int)>();
        saveLine = srInterim.ReadLine();
        if (saveLine != "PlayerInventory") Debug.LogError("Wrong read in interim save file in LoadNextLevel in WorldManager.cs");
        Debug.Log("saveLine: " + saveLine);
        //saveLine = srInterim.ReadLine();
        while (saveLine != "end-inventory")
        {
            saveLine = srInterim.ReadLine();
            if (saveLine.Contains("Empty"))
            {
                break;
            }
            else if (saveLine.Contains("Item type"))
            {
                saveLine = srInterim.ReadLine();

                while (saveLine != "end-type")
                {
                    string[] itemDeets = saveLine.Split(',');
                    string itemName = itemDeets[0];
                    int itemQuantity = int.Parse(itemDeets[1], CultureInfo.InvariantCulture.NumberFormat);
                    inventoryRecord.Add((itemName, itemQuantity));
                    saveLine = srInterim.ReadLine();
                }
            }
            saveLine = srInterim.ReadLine();
        }

        foreach(var i in inventoryRecord)
        {
            Debug.Log("Item name: " + i.Item1 + " item quantity: " + i.Item2);
        }

        LoadPlayer(player_elemont, player_x, player_y, player_cur_hp, player_max_hp, player_cur_energy, player_max_energy, move1, move2, move3, move4, inventoryRecord);

        fsInterim.Close();
        srInterim.Close();

        string filename = GetNextDungeonName();
        string path = @"..\Bootleg_Pokemon\Assets\Bootleg_Pokemon\ConfigData\Level\" + filename + ".txt";
        FileStream fs = File.OpenRead(path);
        StreamReader sr = new StreamReader(fs);

        Debug.Log("Finished Resetting World and opening filestream");

        string line = sr.ReadLine();
        SetDungeonName(line.Substring(line.IndexOf(": ") + 2)); // Get Current Dungeon Name
        //Debug.Log(line);

        line = sr.ReadLine();
        SetNextDungeonName(line.Substring(line.IndexOf(": ") + 2)); // Get Next Dungeon Name
        //Debug.Log("dim substring: " + dimension_substring);

        line = sr.ReadLine();
        string dimension_substring = line.Substring(line.IndexOf(": ") + 2); // Get Dungeon Dimention
        string[] dimension = dimension_substring.Split(',');
        int x_size = int.Parse(dimension[0], CultureInfo.InvariantCulture.NumberFormat);
        int y_size = int.Parse(dimension[1], CultureInfo.InvariantCulture.NumberFormat);
        Debug.Log("In LoadSaveManager, X: " + x_size + " Y: " + y_size);
        SetDungeonDimension(x_size, y_size);
        PrepareWorld();

        for (int y = y_size - 1; y >= 0; y--)
        {
            line = sr.ReadLine();
            Debug.Log("Line is: " + line);
            string[] asset_line = line.Split(' ');
            int line_index = 0;
            for (int x = 0; x < x_size; x++)
            {
                if (asset_line[line_index] != "--") LoadEnvironment(asset_line[line_index], (x, y));
                line_index++;
            }
        }

        line = sr.ReadLine();
        if (line != "end-dungeon") Debug.LogError("Load Level Over-Read dungeon layout in file: " + filename + ".txt");

        line = sr.ReadLine();
        List<EnemyData> enemy_construct = new List<EnemyData>();
        for (int y = y_size - 1; y >= 0; y--)
        {
            line = sr.ReadLine();
            Debug.Log("Line is: " + line);
            string[] enemy_line = line.Split(' ');
            int line_index = 0;
            for (int x = 0; x < x_size; x++)
            {
                if (enemy_line[line_index] != "--") enemy_construct.Add(new EnemyData(enemy_line[line_index], x, y));
                line_index++;
            }
        }
        line = sr.ReadLine();
        line = sr.ReadLine();
        if (line != "EnemiesStatistics") Debug.Log("Load Level Over-Read enemy layout in file: " + filename + ".txt");
        Debug.Log("Line is: " + line);

        while (line != "end-enemy")
        {
            line = sr.ReadLine();
            string[] enemy_line = line.Split(',');
            for (int i = 0; i < enemy_construct.Count; i++)
            {
                if (enemy_construct[i].enemy_ID == enemy_line[0])
                {
                    string elemontal_key = enemy_line[1];
                    float tmp_cur_hp = float.Parse(enemy_line[2], CultureInfo.InvariantCulture.NumberFormat);
                    float tmp_max_hp = float.Parse(enemy_line[3], CultureInfo.InvariantCulture.NumberFormat);
                    string m1 = enemy_line[4];
                    string m2 = enemy_line[5];
                    string m3 = enemy_line[6];
                    string m4 = enemy_line[7];
                    LoadEnemies(elemontal_key, enemy_construct[i].x, enemy_construct[i].y, tmp_cur_hp, tmp_max_hp, m1, m2, m3, m4);
                }
            }
        }
        line = sr.ReadLine();
        // TODO: Fix reading item in dungeon

        List<ItemData> item_construct = new List<ItemData>();
        for (int y = y_size - 1; y >= 0; y--)
        {
            line = sr.ReadLine();
            Debug.Log("Line is: " + line);
            string[] item_line = line.Split(' ');
            for (int x = 0; x < x_size; x++)
            {
                if (item_line[x] != "--") item_construct.Add(new ItemData(item_line[x], x, y));
            }
        }

        while(line != "end-item")
        {
            line = sr.ReadLine();
            string[] item_line = line.Split(',');
            for (int i = 0; i < item_construct.Count; i++)
            {
                if(item_construct[i].item_Id == item_line[0])
                {
                    string itemID = item_line[0];
                    string itemName = item_line[1];
                    LoadItem(itemName, (item_construct[i].x,item_construct[i].y));
                }
            }
        }

        SetLoadStatusReady();
    }


    private void InterimLevelSave()
    {
        string filename = "interimsaveslot";

        string path = @"..\Bootleg_Pokemon\Assets\Bootleg_Pokemon\ConfigData\SaveData\" + filename + ".txt";
        using FileStream fs = File.Create(path);
        using var sr = new StreamWriter(fs);

        sr.WriteLine(filename);

        //Save Dungeon Name:
        sr.WriteLine("DungeonName: " + GetNextDungeonName());
        //Save Item List (TODO: once items have been implemented)

        //Save PlayerStats
        sr.WriteLine("PlayerStatistics:");
        PlayerMovement player = GetPlayer().GetComponent<PlayerMovement>();

        string player_stats = "";
        string player_name = player.gameObject.name.Substring(0, player.gameObject.name.IndexOf("|"));
        player_stats += player_name; //ElemontalName
        player_stats += ",-1,-1";
        player_stats += "," + player.GetCurrentHealth() + "," + player.GetMaxHealth();
        player_stats += "," + player.GetCurrentEnergy() + "," + player.GetMaxEnergy();
        sr.WriteLine(player_stats);

        sr.WriteLine("PlayerMoveset:");

        string playermove = "";
        bool first = true;
        foreach (Moveset m in player.GetMoveSet())
        {
            if (first)
            {
                playermove += m.GetMoveName();
                first = false;
            }
            else
            {
                playermove += "," + m.GetMoveName();
            }
        }

        sr.WriteLine(playermove);

        sr.WriteLine("PlayerInventory");
        var inventory = player.GetComponent<InventorySystem>().GetInventory();

        //player.GetComponent<InventorySystem>().DebugInventoryContent();

        foreach(var itemType in inventory)
        {
            sr.WriteLine("Item type: " + itemType.Item1);
            foreach(var item in itemType.Item2)
            {
                sr.WriteLine(item.GetItem().GetItemName()+ "," + item.GetQuantity());

            }
            sr.WriteLine("end-type");
        }
        
        if(inventory.Count <= 0)
        {
            sr.WriteLine("empty");
        }

        sr.WriteLine("end-inventory");
        sr.Close();
        fs.Close();
    }
}
