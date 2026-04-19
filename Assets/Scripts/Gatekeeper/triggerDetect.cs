using UnityEngine;

public class TriggerCheck : MonoBehaviour
{
    [SerializeField] private string targetTag = "Visitor";

    public bool isTouching = false;

    private float lastContactTime = -999f;

    [SerializeField] private float validHitWindow = 0.35f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(targetTag))
        {
            isTouching = true;
            lastContactTime = Time.time;
            Debug.Log("ENTERED GREEN ZONE");
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag(targetTag))
        {
            isTouching = true;
            lastContactTime = Time.time;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(targetTag))
        {
            isTouching = false;
            Debug.Log("LEFT GREEN ZONE");
        }
    }

    public bool CanShoot()
    {
        return Time.time - lastContactTime <= validHitWindow;
    }
}