using UnityEngine;
using UnityEngine.UI;

public class ChoiceButtonSound : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button button1;
    [SerializeField] private Button button2;
    [SerializeField] private Button button3;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip clickSound;

    void Start()
    {
        if (button1 != null)
            button1.onClick.AddListener(PlayClick);

        if (button2 != null)
            button2.onClick.AddListener(PlayClick);

        if (button3 != null)
            button3.onClick.AddListener(PlayClick);
    }

    void PlayClick()
    {
        if (audioSource != null && clickSound != null)
            audioSource.PlayOneShot(clickSound);
    }
}