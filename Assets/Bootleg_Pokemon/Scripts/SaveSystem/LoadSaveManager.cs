using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using UnityEngine;


public class LoadSaveManager : MonoBehaviour
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

    public void Load(string filename)
    {
        wm.ResetWorld();
        string path = @"..\Bootleg_Pokemon\Assets\Bootleg_Pokemon\Scripts\SaveSystem\" + filename + ".txt";
        using FileStream fs = File.OpenRead(path);
        using var sr = new StreamReader(fs);
        
        // Get save file name
        string line = sr.ReadLine();
        Debug.Log(line);

        // Get dungeon name:
        line = sr.ReadLine();
        string dungeon_name = line.Substring(line.IndexOf("= ") + 2);
        Debug.Log("Dungeon name is:" + dungeon_name);
        wm.LoadLevel(dungeon_name);

        // Get Enemies List:
        line = sr.ReadLine();
        if (line != "Enemies on Level") Debug.LogError("Text Differs for: " + line);
        line = sr.ReadLine();
        while (line != "end-enemy")
        {
            string[] enemy_stat = line.Split(',');
            string enemy_elemont = enemy_stat[0];
            float enemy_x = float.Parse(enemy_stat[1], CultureInfo.InvariantCulture.NumberFormat);
            float enemy_y= float.Parse(enemy_stat[2], CultureInfo.InvariantCulture.NumberFormat);
            float enemy_cur_hp = float.Parse(enemy_stat[3], CultureInfo.InvariantCulture.NumberFormat);
            float enemy_max_hp = float.Parse(enemy_stat[4], CultureInfo.InvariantCulture.NumberFormat);

            //TODO: Make a spawn enemy in world manager
            wm.LoadEnemies(enemy_elemont, enemy_x, enemy_y, enemy_cur_hp, enemy_max_hp);
            line = sr.ReadLine();
        }

        line = sr.ReadLine();
        if (line != "PlayerStatistics:") Debug.LogError("Text Differs for: " + line);
        line = sr.ReadLine();
        string[] player_stat = line.Split(',');
        string player_elemont = player_stat[0];
        float player_x = float.Parse(player_stat[1], CultureInfo.InvariantCulture.NumberFormat);
        float player_y = float.Parse(player_stat[2], CultureInfo.InvariantCulture.NumberFormat);
        float player_cur_hp = float.Parse(player_stat[3], CultureInfo.InvariantCulture.NumberFormat);
        float player_max_hp = float.Parse(player_stat[4], CultureInfo.InvariantCulture.NumberFormat);

        line = sr.ReadLine();
        if (line != "PlayerMoveset:") Debug.LogError("Text Differs for: " + line);
        line = sr.ReadLine();
        string[] player_moves = line.Split(',');
        string move1 = player_moves[0];
        string move2 = player_moves[1];
        string move3 = player_moves[2];
        string move4 = player_moves[3];

        //TODO: Make a spawn player in world manager

        wm.SetLoadStatusReady();
    }
}
