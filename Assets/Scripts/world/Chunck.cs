using UnityEngine;

namespace world
{
    public class Chunck : MonoBehaviour
    {
        public delegate void ChunckEnter(Transform transform);

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.CompareTag("Generator")) return;

            OnChunckEnter?.Invoke(collision.transform);
        }

        public event ChunckEnter OnChunckEnter;
    }
}