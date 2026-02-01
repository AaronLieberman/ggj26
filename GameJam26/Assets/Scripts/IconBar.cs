using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[ExecuteAlways]
public class IconBar : MonoBehaviour
{
    [SerializeField] GameObject IconPrefab;
    [SerializeField] GridLayoutGroup Container;
    [SerializeField, Range(0, 20)] int InitialCount = 0;

    private List<GameObject> _spawnedIcons = new List<GameObject>();

    void Awake()
    {
        SetCount(InitialCount);
    }

    private void OnValidate()
    {
        if (IconPrefab == null || Container == null) 
        {
            return;
        }

        if (!Application.isPlaying)
            SetCount(InitialCount);
    }

    public void SetCount(int count)
    {
        int currentCount = _spawnedIcons.Count;

        if (count == currentCount)
        {
            return;
        }

        if (count > currentCount)
        {
            for (int i = 0; i < count - currentCount; i++)
            {
                GameObject icon = Instantiate(IconPrefab, Container.transform);
                _spawnedIcons.Add(icon);
            }
        }
        else
        {
            for (int i = 0; i < currentCount - count; i++)
            {
                GameObject icon = _spawnedIcons[_spawnedIcons.Count - 1];
                _spawnedIcons.RemoveAt(_spawnedIcons.Count - 1);
                if (Application.isPlaying)
                {
                    Destroy(icon);
                }
                else
                {
#if UNITY_EDITOR
                    //Destroy(icon);
                    //UnityEditor.Undo.DestroyObjectImmediate(icon);
#else
                    DestroyImmediate(icon);
#endif
                }
            }
        }

        //LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)transform.parent);
    }
}