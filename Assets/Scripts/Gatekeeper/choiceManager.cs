using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChoiceManager : MonoBehaviour
{
    [System.Serializable]
    public class QuestionSet
    {
        public string button1;
        public string button2;
        public string button3;
    }

    [System.Serializable]
    public class ReplySet
    {
        public string reply1;
        public string reply2;
        public string reply3;
    }

    [System.Serializable]
    public class Visitor
    {
        public string visitorName;
        public string visitorType;

        public Sprite visitorSprite;     // Sprite for this visitor
        public ReplySet[] replies;       // 5 stages
    }

    [Header("Universal Questions")]
    public QuestionSet[] questionSets;

    [Header("Buttons")]
    public Button button1;
    public Button button2;
    public Button button3;

    public TMP_Text button1Text;
    public TMP_Text button2Text;
    public TMP_Text button3Text;

    [Header("Chat")]
    public Transform content;
    public GameObject messagePrefab;
    public ScrollRect scrollRect;

    [Header("Visitor Visual")]
    public GameObject visitorObject;      // Hierarchy object to enable/disable
    public Image visitorImage;            // UI Image component

    [Header("Visitors")]
    public Visitor[] visitors;

    private Visitor currentVisitor;
    private int currentIndex = 0;

    void Start()
    {
        button1.onClick.AddListener(() => SelectChoice(1));
        button2.onClick.AddListener(() => SelectChoice(2));
        button3.onClick.AddListener(() => SelectChoice(3));

        visitorObject.SetActive(false);

        LoadNewVisitor();
    }

    void LoadNewVisitor()
    {
        ClearChat();

        currentVisitor = visitors[Random.Range(0, visitors.Length)];
        currentIndex = 0;

        // Activate hierarchy object and set sprite
        visitorObject.SetActive(true);
        visitorImage.sprite = currentVisitor.visitorSprite;

        CreateMessage(currentVisitor.visitorName + " approaches checkpoint.", false);

        UpdateChoices();
    }

    void UpdateChoices()
    {
        if (currentIndex >= questionSets.Length)
            return;

        button1Text.text = questionSets[currentIndex].button1;
        button2Text.text = questionSets[currentIndex].button2;
        button3Text.text = questionSets[currentIndex].button3;
    }

    void SelectChoice(int choiceNumber)
    {
        StartCoroutine(ChatFlow(choiceNumber));
    }

    IEnumerator ChatFlow(int choiceNumber)
    {
        SetButtons(false);

        ReplySet currentReply = currentVisitor.replies[currentIndex];
        QuestionSet currentQuestion = questionSets[currentIndex];

        string playerChoice = "";
        string npcReply = "";

        if (choiceNumber == 1)
        {
            playerChoice = currentQuestion.button1;
            npcReply = currentReply.reply1;
        }
        else if (choiceNumber == 2)
        {
            playerChoice = currentQuestion.button2;
            npcReply = currentReply.reply2;
        }
        else
        {
            playerChoice = currentQuestion.button3;
            npcReply = currentReply.reply3;
        }

        bool isFinalStage = currentIndex == questionSets.Length - 1;

        // Final stage Wait and Shoot fake text
        if (isFinalStage && choiceNumber == 3)
        {
            CreateMessage("Allow Entry!", true);

            Debug.Log("Start Wait And Shoot Minigame");
        }
        else
        {
            CreateMessage(playerChoice, true);
        }

        yield return new WaitForSeconds(1f);

        CreateMessage(npcReply, false);

        yield return new WaitForSeconds(0.5f);

        currentIndex++;

        // If conversation finished
        if (currentIndex >= questionSets.Length)
        {
            // Deactivate hierarchy object
            visitorObject.SetActive(false);

            yield return new WaitForSeconds(1f);

            LoadNewVisitor();
        }
        else
        {
            UpdateChoices();
        }

        SetButtons(true);
    }

    void CreateMessage(string msg, bool isPlayer)
    {
        GameObject newMsg = Instantiate(messagePrefab, content);
        TMP_Text txt = newMsg.GetComponent<TMP_Text>();

        txt.text = msg;

        if (isPlayer)
            txt.alignment = TextAlignmentOptions.Right;
        else
            txt.alignment = TextAlignmentOptions.Left;

        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f;
    }

    void SetButtons(bool state)
    {
        button1.interactable = state;
        button2.interactable = state;
        button3.interactable = state;
    }

    void ClearChat()
    {
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }
    }
}