using UnityEngine;
using TMPro;

public class ShiftResultManager : MonoBehaviour
{
    [Header("Result Screens")]
    [SerializeField] private GameObject shiftPassedScreen;
    [SerializeField] private GameObject shiftFailedScreen;

    [Header("Score Text")]
    [SerializeField] private TMP_Text passedScoreText;
    [SerializeField] private TMP_Text failedScoreText;

    [Header("Reference")]
    [SerializeField] private ChoiceManager choiceManager;

    [Header("BGM")]
    [SerializeField] private AudioSource bgmSource;

    private bool resultShown = false;

    void Start()
    {
        if (shiftPassedScreen != null)
            shiftPassedScreen.SetActive(false);

        if (shiftFailedScreen != null)
            shiftFailedScreen.SetActive(false);
    }

    void Update()
    {
        if (resultShown || choiceManager == null)
            return;

        if (choiceManager.visitedVisitorsCount >= choiceManager.totalVisitors)
        {
            resultShown = true;

            string scoreText =
                "Correct Choices: " + choiceManager.correctCount +
                "\nWrong Choices: " + choiceManager.wrongCount;

            // STOP BGM
            if (bgmSource != null)
                bgmSource.Stop();

            if (choiceManager.wrongCount > 0)
            {
                if (failedScoreText != null)
                    failedScoreText.text = scoreText;

                if (shiftFailedScreen != null)
                    shiftFailedScreen.SetActive(true);
            }
            else
            {
                if (passedScoreText != null)
                    passedScoreText.text = scoreText;

                if (shiftPassedScreen != null)
                    shiftPassedScreen.SetActive(true);
            }
        }
    }
}