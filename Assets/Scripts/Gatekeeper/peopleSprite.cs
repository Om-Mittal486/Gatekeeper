using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class OrderedSpriteOnEnable : MonoBehaviour
{
    public Sprite[] sprites;   // Assign in Inspector

    private SpriteRenderer sr;
    private int currentIndex = 0;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void OnEnable()
    {
        if (sprites.Length == 0) return;

        sr.sprite = sprites[currentIndex];

        currentIndex++;

        if (currentIndex >= sprites.Length)
            currentIndex = 0; // Restart from first sprite
    }
}