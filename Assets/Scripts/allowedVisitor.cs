using UnityEngine;

public class AutoMoveSprite : MonoBehaviour
{
    [Header("Movement Points")]
    [SerializeField] private Vector2 startPosition;
    [SerializeField] private Vector2 targetPosition;

    [Header("Speed")]
    [SerializeField] private float minSpeed = 2f;
    [SerializeField] private float maxSpeed = 5f;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private string speedFloat = "Speed";

    [Header("Choice Manager")]
    [SerializeField] private ChoiceManager choiceManager;

    private float moveSpeed;
    private bool isMoving = false;
    private bool firstMove = true;

    public bool MovementFinished => !isMoving;

    void Start()
    {
        transform.position = startPosition;
    }

    void Update()
    {
        if (!isMoving) return;

        transform.position = Vector2.MoveTowards(
            transform.position,
            targetPosition,
            moveSpeed * Time.deltaTime
        );

        if (Vector2.Distance(transform.position, targetPosition) <= 0.01f)
        {
            FinishMovement();
        }
    }

    public void StartMovement()
    {
        transform.position = startPosition;

        if (firstMove)
        {
            moveSpeed = minSpeed;
            firstMove = false;
        }
        else
        {
            moveSpeed = Random.Range(minSpeed, maxSpeed);
        }

        isMoving = true;

        if (animator != null)
            animator.SetFloat(speedFloat, moveSpeed);
    }

    public void StopMovement()
    {
        isMoving = false;

        if (animator != null)
            animator.SetFloat(speedFloat, 0f);
    }

    void FinishMovement()
    {
        isMoving = false;

        if (animator != null)
            animator.SetFloat(speedFloat, 0f);

        transform.position = startPosition;
        gameObject.SetActive(false);

        // IMPORTANT FIX:
        // If player missed Wait & Shoot and visitor escaped,
        // notify ChoiceManager to continue game.
        if (choiceManager != null)
            choiceManager.OnWalkingVisitorFinished();
    }

    private void OnDisable()
    {
        isMoving = false;

        if (animator != null)
            animator.SetFloat(speedFloat, 0f);

        transform.position = startPosition;
    }
}