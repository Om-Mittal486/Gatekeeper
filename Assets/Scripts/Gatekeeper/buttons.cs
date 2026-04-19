using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ChoiceManager choiceManager;
    [SerializeField] private GameObject instructionsPanel;

    [Header("Scene Name")]
    [SerializeField] private string mainMenuScene = "MainMenu";

    // RETRY BUTTON
    public void RetryGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // INSTRUCTIONS BUTTON
    public void ShowInstructions()
    {
        if (instructionsPanel != null)
            instructionsPanel.SetActive(true);
    }

    // OPTIONAL CLOSE INSTRUCTIONS
    public void HideInstructions()
    {
        if (instructionsPanel != null)
            instructionsPanel.SetActive(false);
    }

    // MAIN MENU BUTTON
    public void LoadMainMenu()
    {
        SceneManager.LoadScene(mainMenuScene);
    }
}