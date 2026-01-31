using UnityEngine;

public class Destroyer : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.LogFormat("Destroying mask piece: {0}", collision.name);
        Destroy(collision.gameObject);
    }
}
