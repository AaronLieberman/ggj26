using UnityEngine;

public class EditorPlaceholder : MonoBehaviour
{
	[SerializeField] private bool destroyInsteadOfHide = true;

	void Start()
	{
		if (destroyInsteadOfHide)
		{
			Destroy(gameObject);
		}
		else
		{
			gameObject.SetActive(false);
		}
	}
}