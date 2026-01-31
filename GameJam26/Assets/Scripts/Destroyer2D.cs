using UnityEngine;

public class Destroyer : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.LogFormat("Destroying collided object: {0}", collision.name);
        Destroy(collision.gameObject);
    }
}
