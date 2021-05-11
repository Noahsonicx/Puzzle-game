using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class GameDataManager : MonoBehaviour
{

    public bool newGame = false;
    public bool loadGame = false;

    public string loadFileName;
    public string elemontalPicked
    {
        get; set;
    }

    public GameObject eDM;
    public ElemontalAssetsDictionaryManager elemontalDictionary;
    // Start is called before the first frame update
    void Start()
    {
        elemontalDictionary = eDM.GetComponent<ElemontalAssetsDictionaryManager>();
    }

    // Update is called once per frame
    void Update()
    {

        if (newGame)
        {
            if(elemontalDictionary.GetDictionaryReadyStatus()) //Dictionary is all loaded
                if(elemontalPicked != null)
                {
                    string filePath = @"..\Bootleg_Pokemon\Assets\Bootleg_Pokemon\ConfigData\SaveData\StartPlayerFile.txt";
                    using FileStream fs = File.Create(filePath);
                    using var sr = new StreamWriter(fs);

                    string playerStat = "";
                    GameObject elemontal = elemontalDictionary.GetAsset(elemontalPicked);

                    playerStat += elemontal.name;
                    playerStat += "," + elemontal.GetComponent<Elemontals>().GetHealth();
                    playerStat += "," + elemontal.GetComponent<Elemontals>().GetHealth();
                    playerStat += "," + elemontal.GetComponent<Elemontals>().GetEnergy();
                    playerStat += "," + elemontal.GetComponent<Elemontals>().GetEnergy();

                    sr.WriteLine("PlayerStatistics:");
                    sr.WriteLine(playerStat);
                    sr.WriteLine("PlayerMoveSet:");
                    sr.WriteLine(elemontal.GetComponent<Elemontals>().GetStartingMoves());
                }
        }

        if(loadGame)
        {

        }
    }

    public void CreateNewGame()
    {
        // Select an element. Depending on what element you choose, the elemontal you get depends on this. 
        // Once elemontal is picked, look through dictionary for the stats and write them to a file called 'StartPlayerFile.txt'
        newGame = true;


    }

    public void LoadGame()
    {

    }
}
