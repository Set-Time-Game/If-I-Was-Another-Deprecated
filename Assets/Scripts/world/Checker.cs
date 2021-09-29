using UnityEngine;

public class Checker : MonoBehaviour
{
    public bool crossed;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player") || crossed) return;
    }
}