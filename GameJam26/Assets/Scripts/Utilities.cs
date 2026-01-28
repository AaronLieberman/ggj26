using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

public static class Utilities
{
	public static bool FastMode
	{
		get { return Time.timeScale > 1; }
		set { Time.timeScale = value ? 10 : 1; }
	}

	public static T GetRootComponent<T>() where T : Component
	{
		return SceneManager.GetActiveScene().GetRootGameObjects()
			.Select(a => a.GetComponent<T>())
			.Single(a => a != null);
	}

	public static IEnumerable<T> GetRootComponents<T>() where T : Component
	{
		return SceneManager.GetActiveScene().GetRootGameObjects()
			.SelectMany(a => a.GetComponents<T>());
	}

	public static T GetRootComponentRecursive<T>() where T : Component
	{
		return SceneManager.GetActiveScene().GetRootGameObjects()
			.Select(a => a.GetComponentInChildren<T>())
			.SingleOrDefault(a => a != null);
	}

	public static IEnumerable<T> GetRootComponentsRecursive<T>() where T : Component
	{
		return SceneManager.GetActiveScene().GetRootGameObjects()
			.SelectMany(a => a.GetComponentsInChildren<T>());
	}

	public static void DestroyAllChildren(Transform transform, bool immediate = false)
	{
		while (transform.childCount > 0)
		{
			var child = transform.GetChild(transform.childCount - 1);
#if UNITY_EDITOR
			GameObject.DestroyImmediate(child.gameObject);
#else
			// generally, DestroyImmediate is consider bad to use
			if (immediate)
			{
				GameObject.DestroyImmediate(child.gameObject);
			}
			else
			{
				child.transform.parent.SetParent(null);
				GameObject.Destroy(child.gameObject);
			}
#endif
		}
	}

	public static IEnumerable<Transform> GetParents(this Transform transform)
	{
		var currentParent = transform.parent;
		while (currentParent != null)
		{
			yield return currentParent;
			currentParent = currentParent.parent;
		}
	}

	public static T FindParentWithComponent<T>(this Transform child) where T : Component
	{
		return child.GetParents()
			.Select(parent => parent.GetComponent<T>())
			.FirstOrDefault(component => component != null);
	}

	public static Vector2Int ToVec2I(Vector3Int v3i)
	{
		return new Vector2Int(v3i.x, v3i.y);
	}

	public static Vector3Int ToVec3I(Vector2Int v2i)
	{
		return new Vector3Int(v2i.x, v2i.y, 0);
	}

	public static IEnumerator DoAndWait(Task task)
	{
		yield return new WaitUntil(() => task.IsCompleted);
	}

	public static IEnumerator WaitForSeconds(float seconds)
	{
		yield return new WaitForSeconds(seconds / Time.timeScale);
	}

	public static int IncrementIndex(int currentIndex, int indexMax)
	{
		int newIndex = currentIndex + 1;
		if (currentIndex > indexMax)
		{
			newIndex = 0;
		}
		return newIndex;
	}
}
