using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCreator : MonoBehaviour
{
    public GameObject fireB;
    public GameObject earthB;
    public GameObject waterB;
    public GameObject windB;

    public Button fireButton;
    public Button earthButton;
    public Button waterButton;
    public Button windButton;

    public GameObject gDM;
    public GameDataManager gameDataManager;
    
    // Start is called before the first frame update
    void Start()
    {
        gameDataManager = gDM.GetComponent<GameDataManager>();

        fireButton = fireB.GetComponent<Button>();
        earthButton = earthB.GetComponent<Button>();
        waterButton = waterB.GetComponent<Button>();
        windButton = windB.GetComponent<Button>();

        fireButton.onClick.AddListener(delegate { PickElement("Fire"); });
        waterButton.onClick.AddListener(delegate { PickElement("Water"); });
        earthButton.onClick.AddListener(delegate { PickElement("Earth"); });
        windButton.onClick.AddListener(delegate { PickElement("Wind"); });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void PickElement(string element)
    {
        switch (element)
        {
            case "Fire":
                gameDataManager.elemontalPicked = "Elemont";
                break;
            case "Water":
                gameDataManager.elemontalPicked = "Elemont2";
                break;
            case "Earth":
                gameDataManager.elemontalPicked = "Elemont3";
                break;
            case "wind":
                gameDataManager.elemontalPicked = "Elemont4";
                break;
        }
    }
}
