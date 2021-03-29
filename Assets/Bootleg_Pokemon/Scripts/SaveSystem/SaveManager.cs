using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    public GameObject wm_object;
    public WorldManager wm;
    // Start is called before the first frame update
    void Start()
    {
        wm = wm_object.GetComponent<WorldManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    // Save the state of the player
    private void Save(string saveslot)
    {
        using FileStream fs = File.Create(saveslot + "\n");
        using var sr = new StreamWriter(fs);

        sr.WriteLine(saveslot + "\n");

        //Save Dungeon Name:
        sr.WriteLine("DungeonName = " + SceneManager.GetActiveScene().name + "\n");

        // Save Enemy List
        foreach(GameObject g in wm.GetEnemyList())
        {
            string enemy_info = "";
            EnemyMovement em = g.GetComponent<EnemyMovement>();
            enemy_info = "[" + em.gameObject.name;
            enemy_info += "," + em.GetLocation().Item1.ToString() + "," + em.GetLocation().Item2.ToString();
            enemy_info += "," + em.GetMaxHealth() + "," + em.GetCurrentHealth() + "]\n";
            sr.WriteLine(enemy_info);
        }

        //Save Item List (TODO once items have been implemented)

        //Save PlayerStats
        sr.WriteLine("PlayerStatistics:\n");
        PlayerMovement player = wm.GetPlayer().GetComponent<PlayerMovement>();

        string player_stats = "";
        player_stats += "[" + player.gameObject.name; //ElemontalName
        player_stats += "," + player.GetCurrentHealth() + "," + player.GetMaxHealth();
        player_stats += "," + player.GetCurrentEnergy() + "," + player.GetMaxEnergy();
        sr.WriteLine(player_stats);

        sr.WriteLine("PlayerMoveset:\n");

        string playermove = "[";
        foreach(Moveset m in player.GetMoveSet())
        {
            playermove += m.GetMoveName() + ",";
        }
        playermove += "]";

        sr.WriteLine(player);

        // Save Player Inventory (TODO: Once player inventory has been implemented)
    }
}
