using System;
using UnityEngine;

public class GenericTrigger : MonoBehaviour
{
    public EventHandler<Collider2D> TriggerEnter2D;

    private void OnTriggerEnter2D(Collider2D collision)
    {
		TriggerEnter2D?.Invoke(this, collision);
	}
}
