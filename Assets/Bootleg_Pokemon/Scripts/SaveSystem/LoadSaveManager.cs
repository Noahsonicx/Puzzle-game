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
        string path = @"..\Bootleg_Pokemon\Assets\Bootleg_Pokemon\Scripts\SaveSystem\" + filename + ".txt";
        using FileStream fs = File.OpenRead(path);
        using var sr = new StreamReader(fs);
        
        // Get save file name
        string line = sr.ReadLine();
        Debug.Log(line);

        // Get dungeon name:
        line = sr.ReadLine();
        string dungeon_name = line.Substring(line.IndexOf("= ") + 2);
        Debug.Log(dungeon_name);

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

            //TODO: Make a spawn enemy in wm
            
        }

        line = sr.ReadLine();
        if (line != "PlayerStatistics") Debug.LogError("Text Differs for: " + line);
        line = sr.ReadLine();
        string[] player_stat = line.Split(',');
        string player_elemont = player_stat[0];
        float player_x = float.Parse(player_stat[1], CultureInfo.InvariantCulture.NumberFormat);
        float player_y = float.Parse(player_stat[2], CultureInfo.InvariantCulture.NumberFormat);
        float player_cur_hp = float.Parse(player_stat[3], CultureInfo.InvariantCulture.NumberFormat);
        float player_max_hp = float.Parse(player_stat[4], CultureInfo.InvariantCulture.NumberFormat);


        
    }
}
