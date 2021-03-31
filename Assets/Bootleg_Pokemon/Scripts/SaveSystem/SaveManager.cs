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
    public void Save(string filename)
    {
        string path = @"..\Bootleg_Pokemon\Assets\Bootleg_Pokemon\Scripts\SaveSystem\" + filename + ".txt";    
        using FileStream fs = File.Create(path);
        using var sr = new StreamWriter(fs);

        sr.WriteLine(filename);

        //Save Dungeon Name:
        sr.WriteLine("DungeonName = " + SceneManager.GetActiveScene().name);

        sr.WriteLine("Enemies on Level");
        // Save Enemy List
        foreach(GameObject g in wm.GetEnemyList())
        {
            string enemy_info = "";
            EnemyMovement em = g.GetComponent<EnemyMovement>();
            enemy_info = em.gameObject.name;
            enemy_info += "," + em.GetLocation().Item1.ToString() + "," + em.GetLocation().Item2.ToString();
            enemy_info += "," + em.GetMaxHealth() + "," + em.GetCurrentHealth();
            sr.WriteLine(enemy_info);
        }

        sr.Write("end - enemy");

        //Save Item List (TODO once items have been implemented)

        //Save PlayerStats
        sr.WriteLine("PlayerStatistics:");
        PlayerMovement player = wm.GetPlayer().GetComponent<PlayerMovement>();

        string player_stats = "";
        player_stats += player.gameObject.name; //ElemontalName
        player_stats += "," + player.GetPlayerLocation().Item1 + "," + player.GetPlayerLocation().Item2;
        player_stats += "," + player.GetCurrentHealth() + "," + player.GetMaxHealth();
        player_stats += "," + player.GetCurrentEnergy() + "," + player.GetMaxEnergy();
        Debug.Log(player_stats);
        sr.WriteLine(player_stats);

        sr.WriteLine("PlayerMoveset:");

        string playermove = "";
        foreach(Moveset m in player.GetMoveSet())
        {
            playermove += m.GetMoveName() + ",";
        }

        sr.WriteLine(playermove);

        // Save Player Inventory (TODO: Once player inventory has been implemented)
    }
}
