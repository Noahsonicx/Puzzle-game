using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterSelectorManager : MonoBehaviour
{
    public SaveManager sm;

    public struct QnA {

        public string question;
        public string answer1;
        public string answer2;
        public string answer3;
        public string answer4;
        public int numAnswer;
        public QnA(string _q, string _a1, string _a2, string _a3, string _a4)
        {
            question = _q;
            answer1 = _a1;
            answer2 = _a2;
            answer3 = _a3;
            answer4 = _a4;
            numAnswer = 4;

        }
        public QnA(string _q, string _a1, string _a2, string _a3)
        {
            question = _q;
            answer1 = _a1;
            answer2 = _a2;
            answer3 = _a3;
            answer4 = "";
            numAnswer = 3;
        }
        public QnA(string _q, string _a1, string _a2)
        {
            question = _q;
            answer1 = _a1;
            answer2 = _a2;
            answer3 = "";
            answer4 = "";
            numAnswer = 2;
        }
        public string GetElementKey(string answer)
        {
            switch (answer)
            {
                case "a1":
                    return answer1.Substring(answer1.IndexOf("-") + 2);
                case "a2":
                    return answer2.Substring(answer2.IndexOf("-") + 2);
                case "a3":
                    return answer3.Substring(answer3.IndexOf("-") + 2);
                case "a4":
                    return answer4.Substring(answer4.IndexOf("-") + 2);
            }
            return null;
        }
        public string GetAnswerString(string answer)
        {
            switch (answer)
            {
                case "a1":
                    return answer1.Substring(0, answer1.LastIndexOf(" -"));
                case "a2":
                    return answer2.Substring(0, answer2.LastIndexOf(" -"));
                case "a3":
                    return answer3.Substring(0, answer3.LastIndexOf(" -"));
                case "a4":
                    return answer4.Substring(0, answer4.LastIndexOf(" -"));
            }
            return null;
        }
    }

    Dictionary<string, QnA> questionList = new Dictionary<string, QnA>();
    public string elementChosen = "";

    //UI Elements
    public GameObject characterCreatorPanel;
    public GameObject characterConfirmationPanel;
    public TextMeshProUGUI questionText;
    public GameObject answer1ButtonGObj;
    public Button answer1Button;

    public GameObject answer2ButtonGObj;
    public Button answer2Button;

    public GameObject answer3ButtonGObj;
    public Button answer3Button;

    public GameObject answer4ButtonGObj;
    public Button answer4Button;


    // Start is called before the first frame update
    void Start()
    {
        // When creating new questions, the elements each answer represents is in the form of "- <element1>,<element2>"
        // When there is only ONE element that the answer represent, that is the element the player receives. 
        
        QnA q1 = new QnA("What do you like to do in your free time",
                         "Sports - Air,Fire",
                         "Watch a movie - Air,Earth",
                         "Take long naps - Earth,Water",
                         "Play video games - Fire,Water");
        questionList.Add("Air,Earth,Fire,Water", q1);

        QnA q2 = new QnA("What colour do you feel the most affinity with:",
                         "Yellow - Air",
                         "Red - Fire");
        questionList.Add("Air,Fire", q2);

        QnA q3 = new QnA("What colour do you feel the most affinity with:",
                         "Yellow - Air",
                         "Green - Earth");
        questionList.Add("Air,Earth", q3);

        QnA q4 = new QnA("What colour do you feel the most affinity with:",
                         "Blue - Water",
                         "Red - Fire");
        questionList.Add("Fire,Water", q4);

        QnA q5 = new QnA("What colour do you feel the most affinity with:",
                         "Green - Earth",
                         "Blue - Water");
        questionList.Add("Earth,Water", q5);


        answer1Button = answer1ButtonGObj.GetComponent<Button>();
        answer2Button = answer2ButtonGObj.GetComponent<Button>();
        answer3Button = answer3ButtonGObj.GetComponent<Button>();
        answer4Button = answer4ButtonGObj.GetComponent<Button>();
    }

    public void ConfirmElement()
    {
        sm.SaveStartFile(elementChosen);
        characterConfirmationPanel.SetActive(false);
    }

    public void Run()
    {
        characterCreatorPanel.SetActive(true);
        elementChosen = "Air,Earth,Fire,Water";
        QnA currentQn = questionList[elementChosen];
        SetUIForQuestion(currentQn);
    }
    private void ChooseAnswer(string _elementChosen) 
    {
        elementChosen = _elementChosen;
        if(elementChosen.Split(',').Length > 1)
        {
            SetUIForQuestion(questionList[elementChosen]);
        }
        else
        {
            Debug.Log("Player chose element " + elementChosen);
            characterCreatorPanel.SetActive(false);
            characterConfirmationPanel.SetActive(true);
            characterConfirmationPanel.GetComponentInChildren<TextMeshProUGUI>().text = "Your element affinity is " + elementChosen;
        }
    }
    private void SetUIForQuestion(QnA questionSet)
    {
        questionText.text = questionSet.question;

        switch(questionSet.numAnswer)
        {
            case 2:
                answer1Button.gameObject.SetActive(true);
                answer2Button.gameObject.SetActive(true);
                answer3Button.gameObject.SetActive(false);
                answer4Button.gameObject.SetActive(false);

                answer1Button.onClick.RemoveAllListeners();
                answer1Button.GetComponentInChildren<TextMeshProUGUI>().text = questionSet.GetAnswerString("a1");
                answer1Button.onClick.AddListener(delegate { ChooseAnswer(questionSet.GetElementKey("a1")); });

                answer2Button.onClick.RemoveAllListeners();
                answer2Button.GetComponentInChildren<TextMeshProUGUI>().text = questionSet.GetAnswerString("a2");
                answer2Button.onClick.AddListener(delegate { ChooseAnswer(questionSet.GetElementKey("a2")); });

                break;
            case 3:
                answer1Button.gameObject.SetActive(true);
                answer2Button.gameObject.SetActive(true);
                answer3Button.gameObject.SetActive(true);
                answer4Button.gameObject.SetActive(false);

                answer1Button.onClick.RemoveAllListeners();
                answer1Button.GetComponentInChildren<TextMeshProUGUI>().text = questionSet.GetAnswerString("a1");
                answer1Button.onClick.AddListener(delegate { ChooseAnswer(questionSet.GetElementKey("a1")); });

                answer2Button.onClick.RemoveAllListeners();
                answer2Button.GetComponentInChildren<TextMeshProUGUI>().text = questionSet.GetAnswerString("a2");
                answer2Button.onClick.AddListener(delegate { ChooseAnswer(questionSet.GetElementKey("a2")); });

                answer3Button.onClick.RemoveAllListeners();
                answer3Button.GetComponentInChildren<TextMeshProUGUI>().text = questionSet.GetAnswerString("a3");
                answer3Button.onClick.AddListener(delegate { ChooseAnswer(questionSet.GetElementKey("a3")); });
                break;
            case 4:
                answer1Button.gameObject.SetActive(true);
                answer2Button.gameObject.SetActive(true);
                answer3Button.gameObject.SetActive(true);
                answer4Button.gameObject.SetActive(true);

                answer1Button.onClick.RemoveAllListeners();
                answer1Button.GetComponentInChildren<TextMeshProUGUI>().text = questionSet.GetAnswerString("a1");
                answer1Button.onClick.AddListener(delegate { ChooseAnswer(questionSet.GetElementKey("a1")); });

                answer2Button.onClick.RemoveAllListeners();
                answer2Button.GetComponentInChildren<TextMeshProUGUI>().text = questionSet.GetAnswerString("a2");
                answer2Button.onClick.AddListener(delegate { ChooseAnswer(questionSet.GetElementKey("a2")); });

                answer3Button.onClick.RemoveAllListeners();
                answer3Button.GetComponentInChildren<TextMeshProUGUI>().text = questionSet.GetAnswerString("a3");
                answer3Button.onClick.AddListener(delegate { ChooseAnswer(questionSet.GetElementKey("a3")); });

                answer4Button.onClick.RemoveAllListeners();
                answer4Button.GetComponentInChildren<TextMeshProUGUI>().text = questionSet.GetAnswerString("a4");
                answer4Button.onClick.AddListener(delegate { ChooseAnswer(questionSet.GetElementKey("a4")); });
                break;
        }
    }
}

