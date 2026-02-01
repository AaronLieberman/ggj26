using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[ExecuteAlways]
public class IconBar : MonoBehaviour
{
    [SerializeField] GameObject IconPrefab;
    [SerializeField] GameObject IconEndPrefab;
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

        for (int i = 0; i < currentCount ; i++)
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

        if (count == 0)
        {
            return;
        }

        for (int i = 0 ; i < count - 1 ; i++)
        {
            _spawnedIcons.Add(Instantiate(IconPrefab, Container.transform));
        }

        _spawnedIcons.Add(Instantiate(IconEndPrefab, Container.transform));

        //LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)transform.parent);
    }
}