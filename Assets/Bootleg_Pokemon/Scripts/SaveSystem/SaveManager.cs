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
        sr.WriteLine("DungeonName = " + wm.GetDungeonname());

        sr.WriteLine("Enemies on Level");
        // Save Enemy List
        foreach(GameObject g in wm.GetEnemyList())
        {
            string enemy_info = "";
            EnemyMovement em = g.GetComponent<EnemyMovement>();
            string enemy_name = em.gameObject.name.Substring(0,em.gameObject.name.IndexOf("|"));
            Debug.Log("enemy name in save manager" + enemy_name);
            enemy_info = enemy_name;
            enemy_info += "," + em.GetLocation().Item1.ToString() + "," + em.GetLocation().Item2.ToString();
            enemy_info += "," + em.GetCurrentHealth() + "," + em.GetMaxHealth();
            sr.WriteLine(enemy_info);
        }

        sr.Write("end-enemy\n");

        //Save Item List (TODO once items have been implemented)

        //Save PlayerStats
        sr.WriteLine("PlayerStatistics:");
        PlayerMovement player = wm.GetPlayer().GetComponent<PlayerMovement>();

        string player_stats = "";
        string player_name = player.gameObject.name.Substring(0, player.gameObject.name.IndexOf("|"));
        player_stats += player_name; //ElemontalName
        player_stats += "," + player.GetPlayerLocation().Item1 + "," + player.GetPlayerLocation().Item2;
        Debug.Log("In save manager, player_x: " + player.GetPlayerLocation().Item1 + " player_y: " + player.GetPlayerLocation().Item2);
        player_stats += "," + player.GetCurrentHealth() + "," + player.GetMaxHealth();
        player_stats += "," + player.GetCurrentEnergy() + "," + player.GetMaxEnergy();
        Debug.Log(player_stats);
        sr.WriteLine(player_stats);

        sr.WriteLine("PlayerMoveset:");

        string playermove = "";
        bool first = true;
        foreach(Moveset m in player.GetMoveSet())
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

        // Save Player Inventory (TODO: Once player inventory has been implemented)
    }
}
