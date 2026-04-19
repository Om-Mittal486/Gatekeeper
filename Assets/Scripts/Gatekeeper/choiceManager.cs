using System.Collections;
using System.Collections.Generic;
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
        public string visitorType; // Normal / Alien / Cannibal
        public Sprite visitorSprite;
        public ReplySet[] replies;

        [Range(1,3)]
        public int correctChoice = 1;
    }

    [Header("Questions")]
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
    public ScrollRect scrollRect;

    public GameObject systemMessagePrefab;
    public GameObject playerMessagePrefab;
    public GameObject npcMessagePrefab;

    [Header("Checkpoint Visitor")]
    public GameObject visitorObject;
    public SpriteRenderer visitorRenderer;

    [Header("Walking Visitor")]
    public AutoMoveSprite autoMoveSprite;

    [Header("Trigger Zone")]
    public TriggerCheck triggerCheck;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip shootClip;

    [Header("Visitors")]
    public Visitor[] visitors;

    [Header("Death Screen")]
    public GameObject killedScreen;

    [Header("Score")]
    public int correctCount = 0;
    public int wrongCount = 0;

    public int totalVisitors => visitors.Length;
    public int visitedVisitorsCount => visitedVisitors.Count;

    private Visitor currentVisitor;
    private int currentIndex = 0;

    private bool isBusy = false;
    private bool waitShootMode = false;
    private bool gameOver = false;

    private List<int> visitedVisitors = new List<int>();

    void Start()
    {
        button1.onClick.AddListener(() => SelectChoice(1));
        button2.onClick.AddListener(Button2Pressed);
        button3.onClick.AddListener(() => SelectChoice(3));

        if (killedScreen != null)
            killedScreen.SetActive(false);

        visitorObject.SetActive(false);

        if (autoMoveSprite != null)
            autoMoveSprite.gameObject.SetActive(false);

        LoadNewVisitor();
    }

    void Button2Pressed()
    {
        if (waitShootMode)
            ShootMovingVisitor();
        else
            SelectChoice(2);
    }

    void LoadNewVisitor()
    {
        if (gameOver) return;

        if (visitedVisitors.Count >= visitors.Length)
        {
            visitorObject.SetActive(false);
            return;
        }

        ClearChat();

        int randomIndex = GetUnvisitedVisitor();

        currentVisitor = visitors[randomIndex];
        visitedVisitors.Add(randomIndex);

        currentIndex = 0;
        waitShootMode = false;
        isBusy = false;

        visitorObject.SetActive(true);
        visitorRenderer.sprite = currentVisitor.visitorSprite;

        RestoreNormalButtons();

        CreateSystemMessage(currentVisitor.visitorName.ToUpper() + " APPROACHES");

        UpdateChoices();
        SetButtons(true);
    }

    int GetUnvisitedVisitor()
    {
        List<int> available = new List<int>();

        for (int i = 0; i < visitors.Length; i++)
        {
            if (!visitedVisitors.Contains(i))
                available.Add(i);
        }

        return available[Random.Range(0, available.Count)];
    }

    void UpdateChoices()
    {
        button1Text.text = questionSets[currentIndex].button1;
        button2Text.text = questionSets[currentIndex].button2;
        button3Text.text = questionSets[currentIndex].button3;
    }

    public void SelectChoice(int choice)
    {
        if (isBusy || waitShootMode || gameOver) return;

        StartCoroutine(ChatFlow(choice));
    }

    IEnumerator ChatFlow(int choice)
    {
        isBusy = true;
        SetButtons(false);

        ReplySet reply = currentVisitor.replies[currentIndex];

        string playerText = "";
        string npcReply = "";

        if (choice == 1)
        {
            playerText = questionSets[currentIndex].button1;
            npcReply = reply.reply1;
        }
        else if (choice == 2)
        {
            playerText = questionSets[currentIndex].button2;
            npcReply = reply.reply2;
        }
        else
        {
            playerText = questionSets[currentIndex].button3;
            npcReply = reply.reply3;
        }

        bool finalStage = currentIndex == questionSets.Length - 1;

        CreatePlayerMessage(playerText);

        yield return new WaitForSeconds(0.8f);

        CreateNPCMessage(npcReply);

        yield return new WaitForSeconds(0.4f);

        if (!finalStage)
        {
            currentIndex++;
            UpdateChoices();
            SetButtons(true);
            isBusy = false;
            yield break;
        }

        RegisterVerdict(choice);

        // ALLOW ENTRY
        if (choice == 1)
        {
            yield return StartCoroutine(StartWalkRoutine(false));
        }
        // SHOOT IMMEDIATELY
        else if (choice == 2)
        {
            // If Alien killed instantly -> player dies
            if (currentVisitor.visitorType.ToLower() == "alien")
            {
                ShowKilledScreen();
                yield break;
            }

            PlayShootSound();

            visitorObject.SetActive(false);

            yield return new WaitForSeconds(0.3f);

            LoadNewVisitor();
        }
        // WAIT AND SHOOT
        else if (choice == 3)
        {
            yield return StartCoroutine(StartWalkRoutine(true));
        }
    }

    void ShowKilledScreen()
    {
        gameOver = true;

        visitorObject.SetActive(false);

        if (autoMoveSprite != null)
            autoMoveSprite.gameObject.SetActive(false);

        SetButtons(false);

        if (killedScreen != null)
            killedScreen.SetActive(true);

        Debug.Log("KILLED BY ALIEN");
    }

    void RegisterVerdict(int selectedChoice)
    {
        if (selectedChoice == currentVisitor.correctChoice)
            correctCount++;
        else
            wrongCount++;
    }

    IEnumerator StartWalkRoutine(bool enableWaitShoot)
    {
        visitorObject.SetActive(false);

        autoMoveSprite.gameObject.SetActive(true);

        SpriteRenderer moveSprite = autoMoveSprite.GetComponent<SpriteRenderer>();

        if (moveSprite != null)
            moveSprite.sprite = currentVisitor.visitorSprite;

        autoMoveSprite.StartMovement();

        if (enableWaitShoot)
        {
            waitShootMode = true;
            ShowShootButtonOnly();
            isBusy = false;
            yield break;
        }

        yield return new WaitUntil(() => autoMoveSprite.MovementFinished);

        LoadNewVisitor();
    }

    public void ShootMovingVisitor()
    {
        if (!waitShootMode || gameOver) return;

        if (triggerCheck != null && triggerCheck.CanShoot())
        {
            StartCoroutine(SuccessfulShootRoutine());
        }
    }

    IEnumerator SuccessfulShootRoutine()
    {
        RegisterVerdict(3);

        isBusy = true;
        waitShootMode = false;

        autoMoveSprite.StopMovement();

        PlayShootSound();

        yield return new WaitForSeconds(0.2f);

        autoMoveSprite.gameObject.SetActive(false);

        LoadNewVisitor();
    }

    void ShowShootButtonOnly()
    {
        button1.gameObject.SetActive(false);
        button3.gameObject.SetActive(false);

        button2.gameObject.SetActive(true);
        button2.interactable = true;
        button2Text.text = "SHOOT";
    }

    void RestoreNormalButtons()
    {
        button1.gameObject.SetActive(true);
        button2.gameObject.SetActive(true);
        button3.gameObject.SetActive(true);
    }

    void PlayShootSound()
    {
        if (audioSource != null && shootClip != null)
            audioSource.PlayOneShot(shootClip);
    }

    void CreateSystemMessage(string msg)
    {
        GameObject obj = Instantiate(systemMessagePrefab, content);
        obj.GetComponentInChildren<TMP_Text>().text = msg;
        ScrollBottom();
    }

    void CreatePlayerMessage(string msg)
    {
        GameObject obj = Instantiate(playerMessagePrefab, content);
        obj.GetComponentInChildren<TMP_Text>().text = msg;
        ScrollBottom();
    }

    void CreateNPCMessage(string msg)
    {
        GameObject obj = Instantiate(npcMessagePrefab, content);
        obj.GetComponentInChildren<TMP_Text>().text = msg;
        ScrollBottom();
    }

    void ScrollBottom()
    {
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
            Destroy(child.gameObject);
    }
}