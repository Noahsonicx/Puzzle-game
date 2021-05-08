using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using UnityEngine;


public class LoadSaveManager : MonoBehaviour
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

    public void LoadFromSave(string filename)
    {
        wm.ResetWorld();
        string path = @"..\Bootleg_Pokemon\Assets\Bootleg_Pokemon\ConfigData\SaveData\" + filename + ".txt";
        using FileStream fs = File.OpenRead(path);
        using var sr = new StreamReader(fs);
        
        // Get save file name
        string line = sr.ReadLine();
        Debug.Log(line);

        // Get dungeon name:
        line = sr.ReadLine();
        string dungeon_name = line.Substring(line.IndexOf("= ") + 2);
        Debug.Log("Dungeon name is:" + dungeon_name);
        wm.SetDungeonName(dungeon_name);
        LoadLevelForSave(dungeon_name);

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
            string m1 = enemy_stat[5];
            print("In LoadFrom Save move1:" + m1);
            string m2 = enemy_stat[6];
            print("In LoadFrom Save move2:" + m2);
            string m3 = enemy_stat[7];
            print("In LoadFrom Save move3:" + m3);
            string m4 = enemy_stat[8];
            print("In LoadFrom Save move4:" + m4);

            Debug.Log("Enemy cur_hp:" + enemy_cur_hp + " max_hp: " + enemy_max_hp);
            //TODO: Make a spawn enemy in world manager
            wm.LoadEnemies(enemy_elemont, enemy_x, enemy_y, enemy_cur_hp, enemy_max_hp, m1, m2, m3, m4);
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
        float player_cur_energy = float.Parse(player_stat[5], CultureInfo.InvariantCulture.NumberFormat);
        float player_max_energy = float.Parse(player_stat[6], CultureInfo.InvariantCulture.NumberFormat);


        line = sr.ReadLine();
        if (line != "PlayerMoveset:") Debug.LogError("Text Differs for: " + line);
        line = sr.ReadLine();
        string[] player_moves = line.Split(',');
        string move1 = player_moves[0];
        string move2 = player_moves[1];
        string move3 = player_moves[2];
        string move4 = player_moves[3];

        wm.LoadPlayer(player_elemont, player_x, player_y, player_cur_hp, player_max_hp, player_cur_energy, player_max_energy, move1, move2, move3, move4);

        wm.SetLoadStatusReady();
    }

    public void LoadNextLevel()
    {
        wm.ResetWorld();
        string filename = wm.GetNextDungeonName();
        string path = @"..\Bootleg_Pokemon\Assets\Bootleg_Pokemon\ConfigData\Level\" + filename + ".txt";
        FileStream fs = File.OpenRead(path);
        StreamReader sr = new StreamReader(fs);

        Debug.Log("Finished Resetting World and opening filestream");

        string line = sr.ReadLine();
        wm.SetDungeonName(line.Substring(line.IndexOf(": ") + 2)); // Get Current Dungeon Name
        //Debug.Log(line);

        line = sr.ReadLine();
        wm.SetNextDungeonName(line.Substring(line.IndexOf(": ") + 2)); // Get Next Dungeon Name
        //Debug.Log("dim substring: " + dimension_substring);

        line = sr.ReadLine();
        string dimension_substring = line.Substring(line.IndexOf(": ") + 2); // Get Dungeon Dimention
        string[] dimension = dimension_substring.Split(',');
        int x_size = int.Parse(dimension[0], CultureInfo.InvariantCulture.NumberFormat);
        int y_size = int.Parse(dimension[1], CultureInfo.InvariantCulture.NumberFormat);
        Debug.Log("In LoadSaveManager, X: " + x_size + " Y: " + y_size);
        wm.SetDungeonDimension(x_size, y_size);
        wm.PrepareWorld();

        for (int y = y_size - 1; y >= 0; y--)
        {
            line = sr.ReadLine();
            Debug.Log("Line is: " + line);
            string[] asset_line = line.Split(' ');
            int line_index = 0;
            for (int x = 0; x < x_size; x++)
            {
                if (asset_line[line_index] != "--") wm.LoadEnvironment(asset_line[line_index], (x, y));
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

        while(line != "end-enemy")
        {
            line = sr.ReadLine();
            string[] enemy_line = line.Split(',');
            for(int i = 0; i < enemy_construct.Count; i++)
            {
                if(enemy_construct[i].enemy_ID == enemy_line[0])
                {
                    string elemontal_key = enemy_line[1];
                    float tmp_cur_hp = float.Parse(enemy_line[2], CultureInfo.InvariantCulture.NumberFormat);
                    float tmp_max_hp = float.Parse(enemy_line[3], CultureInfo.InvariantCulture.NumberFormat);
                    string move1 = enemy_line[4];
                    string move2 = enemy_line[5];
                    string move3 = enemy_line[6];
                    string move4 = enemy_line[7];
                    wm.LoadEnemies(elemontal_key, enemy_construct[i].x, enemy_construct[i].y, tmp_cur_hp, tmp_max_hp, move1, move2, move3, move4);
                }
            }
        }
        wm.SetLoadStatusReady();
        // TODO: Item Construct depending on how inventory is implemented.
    }
    public void LoadStartLevel(string filename) 
    {
        wm.ResetWorld();
        string defaultSaveFilePath = Application.dataPath + @"\Bootleg_Pokemon\ConfigData\SaveData\saveslot0.txt";
        FileStream fs = File.OpenRead(defaultSaveFilePath);
        StreamReader sr = new StreamReader(fs);

        string saveLine = sr.ReadLine();
        saveLine = sr.ReadLine();
        string currentDungeon = saveLine.Substring(saveLine.IndexOf(": ") + 2);

        saveLine = sr.ReadLine();
        if (saveLine != "PlayerStatistics:") Debug.LogError("Text Differs for: " + saveLine);
        saveLine = sr.ReadLine();
        string[] player_stat = saveLine.Split(',');
        string player_elemont = player_stat[0];
        float player_x = float.Parse(player_stat[1], CultureInfo.InvariantCulture.NumberFormat);
        float player_y = float.Parse(player_stat[2], CultureInfo.InvariantCulture.NumberFormat);
        float player_cur_hp = float.Parse(player_stat[3], CultureInfo.InvariantCulture.NumberFormat);
        float player_max_hp = float.Parse(player_stat[4], CultureInfo.InvariantCulture.NumberFormat);
        float player_cur_energy = float.Parse(player_stat[5], CultureInfo.InvariantCulture.NumberFormat);
        float player_max_energy = float.Parse(player_stat[6], CultureInfo.InvariantCulture.NumberFormat);


        saveLine = sr.ReadLine();
        if (saveLine != "PlayerMoveset:") Debug.LogError("Text Differs for: " + saveLine);
        saveLine = sr.ReadLine();
        string[] player_moves = saveLine.Split(',');
        string move1 = player_moves[0];
        string move2 = player_moves[1];
        string move3 = player_moves[2];
        string move4 = player_moves[3];

        wm.LoadPlayer(player_elemont, player_x, player_y, player_cur_hp, player_max_hp, player_cur_energy, player_max_energy, move1, move2, move3, move4);

        fs.Close();
        sr.Close();

        string path = @"..\Bootleg_Pokemon\Assets\Bootleg_Pokemon\ConfigData\Level\" + currentDungeon + ".txt";
        FileStream fs_dungeon = File.OpenRead(path);
        StreamReader sr_dungeon = new StreamReader(fs_dungeon);

        //Debug.Log("Finished Resetting World and opening filestream");

        string line = sr_dungeon.ReadLine();
        wm.SetDungeonName(line.Substring(line.IndexOf(": ") + 2)); // Get Current Dungeon Name
       
        line = sr_dungeon.ReadLine();
        wm.SetNextDungeonName(line.Substring(line.IndexOf(": ") + 2)); // Get Next Dungeon Name
        //Debug.Log("dim substring: " + dimension_substring);

        line = sr_dungeon.ReadLine();
        string dimension_substring = line.Substring(line.IndexOf(": ") + 2); // Get Dungeon Dimention
        string[] dimension = dimension_substring.Split(',');
        int x_size = int.Parse(dimension[0], CultureInfo.InvariantCulture.NumberFormat);
        int y_size = int.Parse(dimension[1], CultureInfo.InvariantCulture.NumberFormat);
        Debug.Log("In LoadSaveManager, X: " + x_size + " Y: " + y_size);
        wm.SetDungeonDimension(x_size, y_size);
        wm.PrepareWorld();

        for (int y = y_size - 1; y >= 0; y--)
        {
            line = sr_dungeon.ReadLine();
            Debug.Log("Line is: " + line);
            string[] asset_line = line.Split(' ');
            int line_index = 0;
            for (int x = 0; x < x_size; x++)
            {
                if (asset_line[line_index] != "--") wm.LoadEnvironment(asset_line[line_index], (x, y));
                line_index++;
            }
        }

        line = sr_dungeon.ReadLine();
        if (line != "end-dungeon") Debug.LogError("Load Level Over-Read dungeon layout in file: " + filename + ".txt");

        line = sr_dungeon.ReadLine();
        List<EnemyData> enemy_construct = new List<EnemyData>();
        for (int y = y_size - 1; y >= 0; y--)
        {
            line = sr_dungeon.ReadLine();
            Debug.Log("Line is: " + line);
            string[] enemy_line = line.Split(' ');
            int line_index = 0;
            for (int x = 0; x < x_size; x++)
            {
                if (enemy_line[line_index] != "--") enemy_construct.Add(new EnemyData(enemy_line[line_index], x, y));
                line_index++;
            }
        }
        line = sr_dungeon.ReadLine();
        line = sr_dungeon.ReadLine();
        if (line != "EnemiesStatistics") Debug.Log("Load Level Over-Read enemy layout in file: " + filename + ".txt");
        Debug.Log("Line is: " + line);

        while (line != "end-enemy")
        {
            line = sr_dungeon.ReadLine();
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
                    wm.LoadEnemies(elemontal_key, enemy_construct[i].x, enemy_construct[i].y, tmp_cur_hp, tmp_max_hp, m1, m2, m3, m4);
                }
            }
        }
        wm.SetLoadStatusReady();
        // TODO: Item Construct depending on how inventory is implemented.
    }
    public void LoadLevelForSave(string level_name)
    {
        string path = @"..\Bootleg_Pokemon\Assets\Bootleg_Pokemon\ConfigData\Level\" + level_name + ".txt";
        FileStream fs = File.OpenRead(path);
        StreamReader sr = new StreamReader(fs);

        string line = sr.ReadLine();
        wm.SetDungeonName(line.Substring(line.IndexOf(": ") + 2));
        //Debug.Log(line);

        line = sr.ReadLine();
        wm.SetNextDungeonName(line.Substring(line.IndexOf(": ") + 2));
        //Debug.Log("dim substring: " + dimension_substring);

        line = sr.ReadLine();
        string dimension_substring = line.Substring(line.IndexOf(": ") + 2);
        string[] dimension = dimension_substring.Split(',');
        int x_size = int.Parse(dimension[0], CultureInfo.InvariantCulture.NumberFormat);
        int y_size = int.Parse(dimension[1], CultureInfo.InvariantCulture.NumberFormat);
        Debug.Log("In LoadSaveManager, X: " + x_size + " Y: " + y_size);
        wm.SetDungeonDimension(x_size, y_size); 
        wm.PrepareWorld();

        for (int y = y_size - 1; y >= 0; y--)
        {
            line = sr.ReadLine();
            Debug.Log("Line is: " + line);
            string[] asset_line = line.Split(' ');
            int line_index = 0;
            for (int x = 0; x < x_size; x++)
            {
                if (asset_line[line_index] != "--") wm.LoadEnvironment(asset_line[line_index], (x, y));
                line_index++;
            }
        }
        line = sr.ReadLine();
        if (line != "end-dungeon") Debug.LogError("Load Level Over-Read in file: " + level_name + ".txt");
    }

}
