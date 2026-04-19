using UnityEngine;

public class AutoMoveSprite : MonoBehaviour
{
    [Header("Movement Points")]
    [SerializeField] private Vector2 startPosition;
    [SerializeField] private Vector2 targetPosition;

    [Header("Random Speed Range")]
    [SerializeField] private float minSpeed = 2f;
    [SerializeField] private float maxSpeed = 6f;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private string speedParameter = "Speed";

    private float moveSpeed;
    private bool isMoving = false;
    private static bool firstTime = true;

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

        if (animator != null)
            animator.SetFloat(speedParameter, moveSpeed);

        // Reached target
        if (Vector2.Distance(transform.position, targetPosition) < 0.01f)
        {
            FinishMovement();
        }
    }

    public void StartMovement()
    {
        if (isMoving) return;

        transform.position = startPosition;

        if (firstTime)
        {
            moveSpeed = minSpeed;
            firstTime = false;
        }
        else
        {
            moveSpeed = Random.Range(minSpeed, maxSpeed);
        }

        isMoving = true;

        if (animator != null)
            animator.SetFloat(speedParameter, moveSpeed);
    }

    public void StopMovement()
    {
        isMoving = false;

        if (animator != null)
            animator.SetFloat(speedParameter, 0f);
    }

    void FinishMovement()
    {
        isMoving = false;

        if (animator != null)
            animator.SetFloat(speedParameter, 0f);

        transform.position = startPosition;

        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        isMoving = false;

        if (animator != null)
            animator.SetFloat(speedParameter, 0f);

        transform.position = startPosition;
    }
}